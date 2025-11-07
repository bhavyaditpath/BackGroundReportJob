using BackGroundReportJob.Infrastructure.Repositories.Interface;
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

        public ReportService(
            IEmployeeService employeeService,
            IReportConfigurationRepository reportConfigurationRepository,
            IConfiguration configuration,
            ILogger<ReportService> logger)
        {
            _employeeService = employeeService;
            _reportConfigurationRepository = reportConfigurationRepository;
            _logger = logger;
            _reportDirectory = configuration["ReportOutputPath"] ?? @"D:\BackgroundGeneratedReport\Employee";
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

            foreach (var report in reportsToGenerate)
            {
                var frequencyText = report.Frequency.ToString();
                var fileName = $"{report.ReportName}_{frequencyText}_{DateTime.Now:yyyyMMdd_HHmm}.csv";
                var filePath = Path.Combine(_reportDirectory, fileName);

                var csv = new StringBuilder();
                csv.AppendLine("Id,Name,RollNumber");

                foreach (var emp in employees)
                    csv.AppendLine($"{emp.Id},{emp.Name},{emp.RollNumber}");

                await File.WriteAllTextAsync(filePath, csv.ToString());

                _logger.LogInformation($"✅ {frequencyText} Report generated successfully: {filePath}");
            }
        }
    }
}
