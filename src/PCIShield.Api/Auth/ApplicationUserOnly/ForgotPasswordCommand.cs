using System;
using System.Threading;
using System.Threading.Tasks;

using LanguageExt;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

using PCIShield.Domain.Interfaces;
using PCIShield.Infrastructure.Data;
using PCIShield.Infrastructure.Services;
using static LanguageExt.Prelude;

namespace PCIShield.Api.Auth.ApplicationUserOnly
{
    public class ForgotPasswordCommand : AuthBusinessLogicSagaHandlerTemplate
    {
        private readonly UserManager<CustomPCIShieldUser> _userManager;
        private readonly IMimeKitEmailSender _emailService;
        private readonly IConfiguration _configuration;
        private readonly IAppLoggerService<ForgotPasswordCommand> _logger;

        public ForgotPasswordCommand(
            UserManager<CustomPCIShieldUser> userManager,
            IMimeKitEmailSender emailService,
            IConfiguration configuration,
            IAppLoggerService<ForgotPasswordCommand> logger)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task<Either<Exception, Unit>> ExecuteSpecificAsync(
            AuthUpdateContext context,
            CancellationToken cancellationToken)
        {
            try
            {
                if (context is not ForgotPasswordAuthContext forgotPasswordContext)
                {
                    return Left<Exception, Unit>(new InvalidOperationException(
                        "Invalid context type for ForgotPasswordCommand"));
                }
                if (string.IsNullOrWhiteSpace(forgotPasswordContext.Email) ||
                    !forgotPasswordContext.Email.Contains("@"))
                {
                    _logger.LogWarning(
                        "Forgot password attempt with invalid email format: {Email}",
                        forgotPasswordContext.Email);
                    return Right(unit);
                }
                var user = await _userManager.FindByEmailAsync(forgotPasswordContext.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    _logger.LogWarning(
                        "Password reset requested for non-existent or unconfirmed email: {Email}",
                        forgotPasswordContext.Email);
                    return Right(unit);
                }
                if (await _userManager.IsLockedOutAsync(user))
                {
                    _logger.LogWarning(
                        "Password reset requested for locked out user: {UserId}",
                        user.Id);
                    return Right(unit);
                }
                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                var baseResetUrl = forgotPasswordContext.ResetPasswordUrl;
                if (string.IsNullOrWhiteSpace(baseResetUrl))
                {
                    baseResetUrl = _configuration["App:ResetPasswordUrl"];
                }

                if (string.IsNullOrWhiteSpace(baseResetUrl))
                {
                    _logger.LogError("ResetPasswordUrl not configured in app settings");
                    return Left<Exception, Unit>(new InvalidOperationException(
                        "Password reset URL is not configured"));
                }

                var callbackUrl = $"{baseResetUrl}?token={Uri.EscapeDataString(resetToken)}&email={Uri.EscapeDataString(user.Email)}";
                var subject = _configuration["Email:ForgotPassword:Subject"] ?? "Reset Your PCIShield Password";
                var fromEmail = _configuration["EmailSettings:FromEmail"] ?? "noreply@pcishieldapp.app";

                var emailTemplate = _configuration["Email:ForgotPassword:Template"] ?? @"
                    <h2>Password Reset Request</h2>
                    <p>We received a request to reset your PCIShield password.</p>
                    <p>Click the link below to reset your password:</p>
                    <p><a href='{0}'>Reset Password</a></p>
                    <p>If the link doesn't work, copy and paste this URL into your browser:</p>
                    <p>{0}</p>
                    <p><strong>This link will expire in 24 hours.</strong></p>
                    <p>If you didn't request this password reset, please ignore this email. Your password will not be changed.</p>
                    <p>Thank you,<br>The PCIShield Team</p>
                ";

                var emailBody = string.Format(emailTemplate, callbackUrl);
                await _emailService.SendEmailAsync(
                    user.Email,
                    fromEmail,
                    subject,
                    emailBody);

                _logger.LogInformation(
                    "Password reset email sent successfully to {Email}",
                    user.Email);
                await _userManager.UpdateSecurityStampAsync(user);

                return Right(unit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing forgot password request");
                return Left<Exception, Unit>(ex);
            }
        }
    }
}