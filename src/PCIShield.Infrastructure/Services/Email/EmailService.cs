using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using LanguageExt;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using PCIShieldLib.SharedKernel;

using static LanguageExt.Prelude;

namespace PCIShield.Infrastructure.Services.Email
{
    public interface IEmailStrategy
    {
        Task<Either<Exception, Unit>> SendEmailAsync(
            string toEmail,
            string fromEmail,
            string subject,
            string htmlBody,
            CancellationToken cancellationToken = default);
    }
    public sealed class SmtpEmailStrategy : IEmailStrategy
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SmtpEmailStrategy> _logger;

        public SmtpEmailStrategy(
            IConfiguration configuration,
            ILogger<SmtpEmailStrategy> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<Either<Exception, Unit>> SendEmailAsync(
            string toEmail,
            string fromEmail,
            string subject,
            string htmlBody,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var smtpHost = _configuration["EmailSettings:SmtpHost"]
                    ?? throw new InvalidOperationException("SmtpHost not configured");
                var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
                var smtpUsername = _configuration["EmailSettings:SmtpUsername"]
                    ?? throw new InvalidOperationException("SmtpUsername not configured");
                var smtpPassword = _configuration["EmailSettings:SmtpPassword"]
                    ?? throw new InvalidOperationException("SmtpPassword not configured");
                var enableSsl = bool.Parse(_configuration["EmailSettings:EnableSsl"] ?? "true");

                using var smtpClient = new SmtpClient(smtpHost, smtpPort)
                {
                    Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                    EnableSsl = enableSsl,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Timeout = 30000
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail),
                    Subject = subject,
                    Body = htmlBody,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                await smtpClient.SendMailAsync(mailMessage, cancellationToken);

                _logger.LogInformation(
                    "Email sent successfully to {ToEmail} via SMTP",
                    toEmail
                );

                return Right(unit);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to send email to {ToEmail} via SMTP",
                    toEmail
                );
                return Left<Exception, Unit>(ex);
            }
        }
    }

    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string fromEmail, string subject, string htmlBody);
    }
    public sealed class EmailService : IEmailService
    {
        private readonly IEmailStrategy _strategy;
        private readonly ILogger<EmailService> _logger;

        public EmailService(
            IEmailStrategy strategy,
            ILogger<EmailService> logger)
        {
            _strategy = strategy;
            _logger = logger;
        }

        public async Task SendEmailAsync(
            string toEmail,
            string fromEmail,
            string subject,
            string htmlBody)
        {
            _logger.LogInformation(
                "Attempting to send email to {ToEmail} with subject: {Subject}",
                toEmail,
                subject
            );

            var result = await _strategy.SendEmailAsync(
                toEmail,
                fromEmail,
                subject,
                htmlBody
            );

            result.Match(
                Right: _ => _logger.LogInformation("Email sent successfully"),
                Left: ex => _logger.LogError(ex, "Email sending failed")
            );
            result.IfLeft(ex => throw ex);
        }
    }
}
