using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace PortfolioMichael.Services
{
    // 1. Definition of the Settings Class
    public class EmailSettings
    {
        public string? SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string? SenderEmail { get; set; }
        public string? SenderName { get; set; }
        public string? RecipientEmail { get; set; }
        public string? SenderPassword { get; set; }
    }

    // 2. Definition of the Interface
    public interface IEmailService
    {
        Task<bool> SendContactFormAsync(string name, string email, string subject, string message);
    }

    // 3. Implementation using MailKit
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;
        private readonly ILogger<EmailService> _logger;
        private readonly IConfiguration _configuration;

        public EmailService(IOptions<EmailSettings> settings, ILogger<EmailService> logger, IConfiguration configuration)
        {
            _settings = settings.Value;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<bool> SendContactFormAsync(string name, string email, string subject, string message)
        {
            try
            {
                // Get password from Configuration or Environment
                var password = _configuration["EmailSettings:SenderPassword"]
                               ?? Environment.GetEnvironmentVariable("GMAIL_APP_PASSWORD");

                if (string.IsNullOrEmpty(password))
                {
                    _logger.LogError("SMTP Password is missing from configuration.");
                    return false;
                }

                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
                emailMessage.To.Add(new MailboxAddress("Michael Mujabi", _settings.RecipientEmail));
                emailMessage.ReplyTo.Add(new MailboxAddress(name, email));
                emailMessage.Subject = $"Portfolio Contact: {subject}";

                emailMessage.Body = new TextPart("plain")
                {
                    Text = $"Name: {name}\n" +
                           $"Email: {email}\n" +
                           $"Date: {DateTime.Now}\n\n" +
                           $"Message:\n{message}"
                };

                using (var client = new SmtpClient())
                {
                    // 2026 Security: Connect with StartTls
                    await client.ConnectAsync(_settings.SmtpServer, _settings.SmtpPort, SecureSocketOptions.StartTls);

                    await client.AuthenticateAsync(_settings.SenderEmail, password);
                    await client.SendAsync(emailMessage);
                    await client.DisconnectAsync(true);
                }

                _logger.LogInformation("Contact email successfully sent via MailKit.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MailKit SMTP error occurred.");
                return false;
            }
        }
    }
}
