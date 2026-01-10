using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Search;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using PCIShield.Domain.Interfaces;
namespace PCIShield.Infrastructure.Services
{
    public class MimeKitEmailSender : IMimeKitEmailSender
    {
        private readonly ConcurrentDictionary<string, string> _defaultTemplates = new ConcurrentDictionary<string, string>();
        private readonly IMimeKitEmailConfigService _emailConfigService;
        private readonly IAppLoggerService<MimeKitEmailSender> _logger;
        private readonly IMimeKitSmtpClientFactory _smtpClientFactory;
        public MimeKitEmailSender(IAppLoggerService<MimeKitEmailSender> logger, IMimeKitSmtpClientFactory smtpClientFactory,
            IMimeKitEmailConfigService emailConfigService)
        {
            _logger = logger;
            _smtpClientFactory = smtpClientFactory;
            _emailConfigService = emailConfigService;
        }
        public interface IMimeKitEmailConfigService
        {
            MimeKitEmailConfig GetEmailConfiguration();
        }
        public interface IMimeKitSmtpClientFactory
        {
            SmtpClient CreateSmtpClient();
        }
        public string GetDefaultTemplate(int? tenantId)
        {
            var tenancyKey = tenantId.HasValue ? tenantId.Value.ToString() : "host";
            return _defaultTemplates.GetOrAdd(tenancyKey, key =>
            {
                var assembly = typeof(MimeKitEmailSender).Assembly;
                using var stream = assembly.GetManifestResourceStream("YourNamespace.Path.To.default.html") ?? throw new InvalidOperationException("Email template resource not found.");
                using var reader = new StreamReader(stream, Encoding.UTF8);
                var template = reader.ReadToEnd();
                template = template.Replace("{THIS_YEAR}", DateTime.Now.Year.ToString());
                return template.Replace("{EMAIL_LOGO_URL}", this.GetTenantLogoUrl(tenantId));
            });
        }
        public async Task<IEnumerable<MimeMessage>> GetMessagesAsync()
        {
            var messages = new List<MimeMessage>();
            try
            {
                var emailConfig = _emailConfigService.GetEmailConfiguration();
                using var imapClient = new MailKit.Net.Imap.ImapClient();
                imapClient.ServerCertificateValidationCallback = (s, c, h, e) => true;
                var secureSocketOptions = emailConfig.UseImapSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.Auto;
                await imapClient.ConnectAsync(emailConfig.ImapServer, emailConfig.ImapPort, secureSocketOptions);
                await imapClient.AuthenticateAsync(emailConfig.Username, emailConfig.AppPassword);
                await imapClient.Inbox.OpenAsync(FolderAccess.ReadOnly);
                var uids = await imapClient.Inbox.SearchAsync(SearchQuery.All);
                messages = new List<MimeMessage>();
                foreach (var uid in uids)
                {
                    messages.Add(await imapClient.Inbox.GetMessageAsync(uid));
                }
                await imapClient.DisconnectAsync(true);
            }
            catch (Exception e)
            {
                throw;
            }
            return messages;
        }
        public async Task SendEmailAsync(string to, string from, string subject, string body)
        {
            _logger.LogInformation("Attempting to send email to {to} from {from} with subject {subject}...", to, from,
                subject);
            try
            {
                var emailConfig = _emailConfigService.GetEmailConfiguration();
                using var client = this._smtpClientFactory.CreateSmtpClient();
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                await client.ConnectAsync(emailConfig.SmtpServer, emailConfig.SmtpPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(emailConfig.Username, emailConfig.AppPassword);
                var message = new MimeKitEmailMessageBuilder()
                    .From("pciShieldapp@mail.com")
                    .To(to)
                    .WithSubject(subject)
                    .WithHtmlBody(body)
                    .Build();
                await client.SendAsync(message);
                this._logger.LogInformation("Email sent!");
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email.");
                throw;
            }
        }
        private string GetTenantLogoUrl(int? tenantId)
        {
            return tenantId.HasValue ? $"https://example.com/logos/{tenantId}.png" : "https://example.com/logos/default.png";
        }
        public class MimeKitEmailConfig
        {
            public MimeKitEmailConfig(string smtpServer, int smtpPort, bool useSmtpSsl, string username, string password, string appPassword, string imapServer, int imapPort, bool useImapSsl)
            {
                SmtpServer = smtpServer ?? throw new ArgumentException("SMTP server must be provided.", nameof(smtpServer));
                SmtpPort = smtpPort;
                UseSmtpSsl = useSmtpSsl;
                Username = username ?? throw new ArgumentException("Username must be provided.", nameof(username));
                Password = password ?? throw new ArgumentException("Password must be provided.", nameof(password));
                AppPassword = appPassword ?? throw new ArgumentException("App password must be provided.", nameof(appPassword));
                ImapServer = imapServer ?? throw new ArgumentException("IMAP server must be provided.", nameof(imapServer));
                ImapPort = imapPort;
                UseImapSsl = useImapSsl;
            }
            public string AppPassword { get; }
            public int ImapPort { get; }
            public string ImapServer { get; }
            public string Password { get; }
            public int SmtpPort { get; }
            public string SmtpServer { get; }
            public bool UseImapSsl { get; }
            public string Username { get; }
            public bool UseSmtpSsl { get; }
        }
        public class MimeKitEmailConfigService : IMimeKitEmailConfigService
        {
            private readonly IConfiguration _configuration;
            public MimeKitEmailConfigService(IConfiguration configuration)
            {
                _configuration = configuration;
            }
            public MimeKitEmailConfig GetEmailConfiguration()
            {
                string? smtpServer = _configuration["EmailSettings:SmtpServer"];
                int smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? string.Empty);
                bool useSmtpSsl = bool.Parse(_configuration["EmailSettings:UseSmtpSsl"] ?? string.Empty);
                string username = _configuration["EmailSettings:Username"];
                string password = _configuration["EmailSettings:Password"];
                string appPassword = _configuration["EmailSettings:AppPassword"];
                string? imapServer = _configuration["EmailSettings:ImapServer"];
                int imapPort = int.Parse(_configuration["EmailSettings:ImapPort"] ?? string.Empty);
                bool useImapSsl = bool.Parse(_configuration["EmailSettings:UseImapSsl"] ?? string.Empty);
                return string.IsNullOrWhiteSpace(smtpServer)
                    ? throw new InvalidOperationException("SMTP server configuration is missing.")
                    : smtpPort <= 0
                    ? throw new InvalidOperationException("Invalid SMTP port configuration.")
                    : string.IsNullOrWhiteSpace(username)
                    ? throw new InvalidOperationException("SMTP username configuration is missing.")
                    : string.IsNullOrWhiteSpace(password)
                    ? throw new InvalidOperationException("SMTP password configuration is missing.")
                    : string.IsNullOrWhiteSpace(appPassword)
                    ? throw new InvalidOperationException("SMTP app password configuration is missing.")
                    : string.IsNullOrWhiteSpace(imapServer)
                    ? throw new InvalidOperationException("IMAP server configuration is missing.")
                    : imapPort <= 0
                    ? throw new InvalidOperationException("Invalid IMAP port configuration.")
                    : new MimeKitEmailConfig(smtpServer, smtpPort, useSmtpSsl, username, password, appPassword, imapServer, imapPort, useImapSsl);
            }
        }
        public class MimeKitEmailMessageBuilder
        {
            private readonly MimeMessage _message;
            public MimeKitEmailMessageBuilder()
            {
                _message = new MimeMessage();
            }
            public MimeMessage Build()
            {
                return _message;
            }
            public MimeKitEmailMessageBuilder From(string from)
            {
                _message.From.Add(new MailboxAddress(from, from));
                return this;
            }
            public MimeKitEmailMessageBuilder To(string to)
            {
                _message.To.Add(new MailboxAddress(to, to));
                return this;
            }
            public MimeKitEmailMessageBuilder WithBody(string body)
            {
                _message.Body = new TextPart("plain") { Text = body };
                return this;
            }
            public MimeKitEmailMessageBuilder WithHtmlBody(string body)
            {
                var htmlBody = $@"
                <!DOCTYPE html>
                <html>
                    <head>
                        <style>
                            @import url('https://fonts.googleapis.com/css2?family=Montserrat:wght@400;700&display=swap');
                            body {{
                                font-family: 'Montserrat', sans-serif;
                                background-color: #f4f4f4;
                                margin: 0;
                                padding: 0;
                                color: #333;
                            }}
                            .email-container {{
                                max-width: 600px;
                                margin: 40px auto;
                                background-color: white;
                                border-radius: 8px;
                                box-shadow: 0 4px 8px rgba(0,0,0,0.1);
                            }}
                            .email-header {{
                                background-color: #013ff7;
                                color: white;
                                padding: 20px;
                                text-align: center;
                                border-radius: 8px 8px 0 0;
                                font-size: 24px;
                            }}
                            .email-body {{
                                padding: 20px;
                                line-height: 1.6;
                            }}
                            .email-footer {{
                                text-align: center;
                                padding: 20px;
                                background-color: #f0f0f0;
                                border-radius: 0 0 8px 8px;
                                font-size: 12px;
                                color: #555;
                            }}
                        </style>
                    </head>
                    <body>
                        <div class='email-container'>
                            <div class='email-header'>Mensaje Importante</div>
                            <div class='email-body'>
                                 <p>Por favor, haz clic en el botón de abajo para confirmar tu dirección de correo electrónico:</p>
                                 <a href='{body}' style='background-color: #013ff7; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; font-weight: bold;'>Confirmar Email</a>
                            </div>
                            <div class='email-footer'>
                                Este correo electrónico fue enviado por PCIShield. © {DateTime.Now.Year}
                            </div>
                        </div>
                    </body>
                </html>";
                _message.Body = new TextPart("html") { Text = htmlBody };
                return this;
            }
            public MimeKitEmailMessageBuilder WithSubject(string subject)
            {
                _message.Subject = subject;
                return this;
            }
        }
        public class MimeKitEmailSmtpClientFactory : IMimeKitSmtpClientFactory
        {
            public SmtpClient CreateSmtpClient()
            {
                return new SmtpClient();
            }
        }
    }
}