using System;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using PCIShield.Infrastructure.Services;
public interface IMailKitEmailServicePlus
{
    Task SendEmailAsync(string toName, string toEmail, string subject, string body);
}
public class MailKitEmailServicePlus : IMailKitEmailServicePlus
{
    private readonly IConfiguration _configuration;
    private readonly IAppLoggerService<MailKitEmailServicePlus> _logger;
    public MailKitEmailServicePlus(IConfiguration configuration, IAppLoggerService<MailKitEmailServicePlus> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }
    public async Task SendEmailAsync(string toName, string toEmail, string subject, string body)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_configuration["EmailSettings:from_name"], _configuration["EmailSettings:from_email"]));
            message.To.Add(new MailboxAddress(toName, toEmail));
            message.Subject = subject;
            message.Body = new TextPart("plain") { Text = body };
            var credential = GoogleCredential.FromFile("path_to_your_credentials.json")
                .CreateScoped("https://mail.google.com/")
                .UnderlyingCredential as ServiceAccountCredential;
            var accessToken = await credential.GetAccessTokenForRequestAsync();
            using var client = new SmtpClient();
            await client.ConnectAsync(_configuration["EmailSettings:host"], int.Parse(_configuration["EmailSettings:port"]), SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(new SaslMechanismOAuth2(credential.Id, accessToken));
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email.");
            throw;
        }
    }
}