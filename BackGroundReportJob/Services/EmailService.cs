using BackGroundReportJob.Helpers;
using BackGroundReportJob.Services.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace BackGroundReportJob.Services
{
    public class EmailService : IEmailService
    {
        private readonly string _apiKey;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _apiKey = configuration["SendGridApiKey"] ?? throw new Exception("SendGrid API Key not found in configuration.");
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlContent)
        {
            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress("noreply@backgroundjobreports.com", "Background Job Reports");
            var to = new EmailAddress(toEmail);

            var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlContent);
            var response = await client.SendEmailAsync(msg);

            if (response.IsSuccessStatusCode)
                _logger.LogInformation($"✅ Email sent successfully to {toEmail}");
            else
                _logger.LogWarning($"⚠️ Failed to send email to {toEmail}. Status: {response.StatusCode}");
        }
    }
}
