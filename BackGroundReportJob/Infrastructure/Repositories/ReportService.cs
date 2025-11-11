using Azure.Storage.Blobs;
using BackGroundReportJob.Enums;
using BackGroundReportJob.Infrastructure.Repositories.Interface;
using BackGroundReportJob.Models;
using BackGroundReportJob.Services.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;

namespace BackGroundReportJob.Infrastructure.Repositories
{
    public class ReportService : IReportService
    {
        private readonly IEmployeeService _employeeService;
        private readonly IReportConfigurationRepository _reportConfigurationRepository;
        private readonly ILogger<ReportService> _logger;
        private readonly string _reportDirectory;
        private readonly IEmailService _emailService;

        public ReportService(
            IEmployeeService employeeService,
            IReportConfigurationRepository reportConfigurationRepository,
            IConfiguration configuration,
            ILogger<ReportService> logger,
            IEmailService emailService)
        {
            _employeeService = employeeService;
            _reportConfigurationRepository = reportConfigurationRepository;
            _logger = logger;
            _reportDirectory = configuration["ReportOutputPath"] ?? @"D:\BackgroundGeneratedReport\Employee";
            _emailService = emailService;
        }
        public async Task<IEnumerable<ReportConfigurationEntity>> GetAllReportConfigurationsAsync()
        {
            return await _reportConfigurationRepository.GetAllAsync();
        }

        public async Task<bool> UpdateReportStatusAsync(Guid id, bool isEnabled)
        {
            var report = await _reportConfigurationRepository.GetByIdAsync(id);
            if (report == null)
                return false;

            report.IsEnabled = isEnabled;
            await _reportConfigurationRepository.UpdateAsync(report);
            return true;
        }

        public async Task GenerateReportsAsync()
        {
            var reportsToGenerate = await _reportConfigurationRepository.GetActiveReportsAsync();

            if (!reportsToGenerate.Any())
            {
                _logger.LogInformation("No reports due for generation at this time.");
                return;
            }

            var employees = await _employeeService.GetAllEmployeesAsync();

            if (!employees.Any())
            {
                _logger.LogInformation("No employees found for report generation.");
                return;
            }

            if (!Directory.Exists(_reportDirectory))
                Directory.CreateDirectory(_reportDirectory);

            // Use Azurite connection string
            string connectionString = "UseDevelopmentStorage=true";
            string containerName = "employee-reports";

            var blobContainerClient = new BlobContainerClient(connectionString, containerName);
            await blobContainerClient.CreateIfNotExistsAsync();

            // ✉️ Get all Admin employees for notifications
            var adminEmployees = employees.Where(e => e.Role == EmployeeRole.Admin && !string.IsNullOrEmpty(e.Email)).ToList();
            if (!adminEmployees.Any())
            {
                _logger.LogWarning("No admin employees found to send report notifications.");
            }

            // 📂 Load HTML email template
            string templatePath = Path.Combine(
                 AppContext.BaseDirectory,
                 "EmailTemplate",
                 "ReportNotificationTemplate.html"
             );

            if (!File.Exists(templatePath))

            {
                _logger.LogError($"Email template not found: {templatePath}");
                return;
            }

            string htmlTemplate = await File.ReadAllTextAsync(templatePath);

            foreach (var report in reportsToGenerate)
            {
                var frequencyText = report.Frequency.ToString();
                var fileName = $"{report.ReportName}_{frequencyText}_{DateTime.Now:yyyyMMdd_HHmm}.csv";
                var filePath = Path.Combine(_reportDirectory, fileName);

                var csv = new StringBuilder();
                csv.AppendLine("Role,Name,RollNumber,Email");

                foreach (var emp in employees)
                    csv.AppendLine($"{emp.Role},{emp.Name},{emp.RollNumber},{emp.Email}");

                // 1️⃣ Save locally
                await File.WriteAllTextAsync(filePath, csv.ToString());
                _logger.LogInformation($"Employee Report saved locally: {filePath}");

                // 2️⃣ Upload to Azurite
                using var stream = File.OpenRead(filePath);
                var blobClient = blobContainerClient.GetBlobClient(fileName);
                await blobClient.UploadAsync(stream, overwrite: true);
                _logger.LogInformation($"Employee Report uploaded successfully: {blobClient.Uri}");

                // 3️⃣ Send Email to Admins
                foreach (var admin in adminEmployees)
                {
                    string personalizedHtml = htmlTemplate
                        .Replace("{{AdminName}}", admin.Name)
                        .Replace("{{ReportName}}", report.ReportName)
                        .Replace("{{GeneratedOn}}", DateTime.Now.ToString("f"))
                        .Replace("{{ReportLink}}", blobClient.Uri.ToString());

                    await _emailService.SendEmailAsync(
                        admin.Email,
                        $"{report.ReportName} Generated Successfully",
                        personalizedHtml
                    );

                    _logger.LogInformation($"Notification email sent to Admin: {admin.Email}");
                }
            }
        }
    }
}
