using System;
using System.Threading;
using System.Threading.Tasks;
using PCIShield.Domain.Interfaces;
using PCIShield.Infrastructure.Data;
using PCIShield.Infrastructure.Services;
using LanguageExt;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using static LanguageExt.Prelude;

namespace PCIShield.Api.Auth.ApplicationUserOnly
{
    public class SendEmailConfirmationCommand : AuthBusinessLogicSagaHandlerTemplate
    {
        private readonly UserManager<CustomPCIShieldUser> _userManager;
        private readonly IMimeKitEmailSender _emailService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly LinkGenerator _linkGenerator;
        private readonly IConfiguration _configuration;
        private readonly IAppLoggerService<SendEmailConfirmationCommand> _logger;

        public SendEmailConfirmationCommand(
            UserManager<CustomPCIShieldUser> userManager,
            IMimeKitEmailSender emailService,
            IHttpContextAccessor httpContextAccessor,
            LinkGenerator linkGenerator,
            IConfiguration configuration,
            IAppLoggerService<SendEmailConfirmationCommand> logger)
        {
            _userManager = userManager;
            _emailService = emailService;
            _httpContextAccessor = httpContextAccessor;
            _linkGenerator = linkGenerator;
            _configuration = configuration;
            _logger = logger;
        }

        protected override async Task<Either<Exception, Unit>> ExecuteSpecificAsync(
            AuthUpdateContext context,
            CancellationToken cancellationToken)
        {
            try
            {
                if (context is RegisterAuthContext)
                {
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(context.CurrentIdentityUser);
                    var httpContext = _httpContextAccessor.HttpContext;
                    var baseUrl = _configuration["EmailConfirmation:BaseUrl"];
                    if (string.IsNullOrWhiteSpace(baseUrl))
                    {
                        var request = httpContext?.Request;
                        baseUrl = $"{request?.Scheme}://{request?.Host}";
                    }
                    var confirmationLink = _linkGenerator.GetUriByAction(
                        httpContext,
                        action: "ConfirmEmailV2",
                        controller: "AccountAuthV2",
                        values: new { 
                            token = token, 
                            email = context.CurrentIdentityUser.Email 
                        },
                        scheme: null,
                        host: null);
                    if (string.IsNullOrWhiteSpace(confirmationLink))
                    {
                        confirmationLink = $"{baseUrl}/api/auth/v2/confirm_email_v2?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(context.CurrentIdentityUser.Email ?? "")}";
                    }
                    var fromEmail = _configuration["EmailSettings:FromEmail"] ?? "noreply@pciShieldapp.app";
                    var fromName = _configuration["EmailSettings:FromName"] ?? "PCIShield App";
                    var subject = _configuration["EmailConfirmation:Subject"] ?? "Confirm Your Email Address";
                    var emailTemplate = _configuration["EmailConfirmation:Template"] ?? 
                        @"<h2>Welcome to PCIShield App!</h2>
                        <p>Please confirm your email address by clicking the link below:</p>
                        <p><a href='{0}'>Confirm Email</a></p>
                        <p>If the link doesn't work, copy and paste this URL into your browser:</p>
                        <p>{0}</p>
                        <p>This link will expire in 24 hours.</p>
                        <p>Thank you,<br>The PCIShield App Team</p>";
                    
                    var body = string.Format(emailTemplate, confirmationLink);
                    await _emailService.SendEmailAsync(
                        context.CurrentIdentityUser.Email, 
                        fromEmail, 
                        subject, 
                        body);
                    context.CurrentApplicationUser?.SetConfirmationEmail(confirmationLink);

                    _logger.LogInformation("Email confirmation sent to {Email} with link {Link}", 
                        context.CurrentIdentityUser.Email, 
                        confirmationLink);

                    return Right<Exception, Unit>(unit);
                }

                return Right<Exception, Unit>(unit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email confirmation");
                return Left<Exception, Unit>(ex);
            }
        }
    }
}