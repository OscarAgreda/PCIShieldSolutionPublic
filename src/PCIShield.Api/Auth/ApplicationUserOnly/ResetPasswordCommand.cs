using System;
using System.Threading;
using System.Threading.Tasks;

using LanguageExt;

using Microsoft.AspNetCore.Identity;

using PCIShield.Infrastructure.Data;
using PCIShield.Infrastructure.Services;
using PCIShieldLib.SharedKernel.Interfaces;

using static LanguageExt.Prelude;

namespace PCIShield.Api.Auth.ApplicationUserOnly
{
    public class ResetPasswordCommand : AuthBusinessLogicSagaHandlerTemplate
    {
        private readonly UserManager<CustomPCIShieldUser> _userManager;
        private readonly IRepository<Domain.Entities.ApplicationUser> _applicationUserRepository;
        private readonly IAppLoggerService<ResetPasswordCommand> _logger;

        public ResetPasswordCommand(
            UserManager<CustomPCIShieldUser> userManager,
            IRepository<Domain.Entities.ApplicationUser> applicationUserRepository,
            IAppLoggerService<ResetPasswordCommand> logger)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _applicationUserRepository = applicationUserRepository ?? throw new ArgumentNullException(nameof(applicationUserRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task<Either<Exception, Unit>> ExecuteSpecificAsync(
            AuthUpdateContext context,
            CancellationToken cancellationToken)
        {
            try
            {
                if (context is not ResetPasswordAuthContext resetContext)
                {
                    return Left<Exception, Unit>(new InvalidOperationException(
                        "Invalid context type for ResetPasswordCommand"));
                }
                if (string.IsNullOrWhiteSpace(resetContext.Email) ||
                    string.IsNullOrWhiteSpace(resetContext.Token) ||
                    string.IsNullOrWhiteSpace(resetContext.NewPassword))
                {
                    _logger.LogWarning("Reset password attempt with missing required fields");
                    return Left<Exception, Unit>(new InvalidOperationException(
                        "Email, token, and new password are required"));
                }
                var user = await _userManager.FindByEmailAsync(resetContext.Email);
                if (user == null)
                {
                    _logger.LogWarning(
                        "Reset password attempt for non-existent email: {Email}",
                        resetContext.Email);
                    return Left<Exception, Unit>(new InvalidOperationException(
                        "Invalid token or email"));
                }
                if (await _userManager.IsLockedOutAsync(user))
                {
                    _logger.LogWarning(
                        "Reset password attempt for locked user: {UserId}",
                        user.Id);
                    return Left<Exception, Unit>(new InvalidOperationException(
                        "This account is currently locked. Please contact support."));
                }
                var resetResult = await _userManager.ResetPasswordAsync(
                    user,
                    resetContext.Token,
                    resetContext.NewPassword);

                if (!resetResult.Succeeded)
                {
                    var errors = string.Join(", ", resetResult.Errors.ToString());
                    _logger.LogWarning(
                        "Password reset failed for user {UserId}: {Errors}",
                        user.Id,
                        errors);
                    return Left<Exception, Unit>(new InvalidOperationException(
                        $"Password reset failed: {errors}"));
                }
                await _userManager.UpdateSecurityStampAsync(user);
                var applicationUser = await _applicationUserRepository.GetByIdAsync(
                    user.ApplicationUserId,
                    cancellationToken);

                if (applicationUser != null)
                {
                    applicationUser.SetFailedLoginCount(0);
                    applicationUser.SetUpdatedDate(DateTime.UtcNow);

                    await _applicationUserRepository.UpdateAsync(applicationUser, cancellationToken);
                }

                _logger.LogInformation(
                    "Password successfully reset for user {UserId}",
                    user.Id);
                context.CurrentIdentityUser = user;
                context.CurrentApplicationUser = applicationUser;

                return Right(unit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password");
                return Left<Exception, Unit>(ex);
            }
        }
    }
}