using System;
using System.Threading;
using System.Threading.Tasks;
using PCIShield.Infrastructure.Data;
using PCIShield.Infrastructure.Services;
using LanguageExt;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using static LanguageExt.Prelude;

namespace PCIShield.Api.Auth.ApplicationUserOnly
{
    public class ValidateCredentialsCommand : AuthBusinessLogicSagaHandlerTemplate
    {
        private readonly UserManager<CustomPCIShieldUser> _userManager;
        private readonly SignInManager<CustomPCIShieldUser> _signInManager;
        private readonly IAppLoggerService<ValidateCredentialsCommand> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ValidateCredentialsCommand(
            UserManager<CustomPCIShieldUser> userManager,
            SignInManager<CustomPCIShieldUser> signInManager,
            IAppLoggerService<ValidateCredentialsCommand> logger,
            IServiceScopeFactory serviceScopeFactory)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task<Either<Exception, Unit>> ExecuteSpecificAsync(
            AuthUpdateContext context,
            CancellationToken cancellationToken)
        {
            try
            {
                if (context is LoginAuthContext loginContext)
                {
                    var user = await _userManager.FindByNameAsync(loginContext.LoginRequest.Username);
                    if (user == null)
                        return Left<Exception, Unit>(new UnauthorizedAccessException("Invalid credentials"));

                    using var scope = _serviceScopeFactory.CreateScope();
                    var signInManager = scope.ServiceProvider.GetRequiredService<SignInManager<CustomPCIShieldUser>>();
                    if (signInManager.IsSignedIn(loginContext.HttpContext?.User))
                    {
                        var currentUserId = _userManager.GetUserId(loginContext.HttpContext.User);
                        if (user.Id != currentUserId)
                            await signInManager.SignOutAsync();
                    }

                    var result = await signInManager.PasswordSignInAsync(
                        loginContext.LoginRequest.Username,
                        loginContext.LoginRequest.Password,
                        isPersistent: true,
                        lockoutOnFailure: true);
                    if (result.RequiresTwoFactor && !string.IsNullOrEmpty(loginContext.LoginRequest.TwoFactorCode))
                    {
                        result = await signInManager.TwoFactorAuthenticatorSignInAsync(
                            loginContext.LoginRequest.TwoFactorCode,
                            isPersistent: true,
                            rememberClient: true);
                    }
                    else if (result.RequiresTwoFactor &&
                             !string.IsNullOrEmpty(loginContext.LoginRequest.TwoFactorRecoveryCode))
                    {
                        result = await signInManager.TwoFactorRecoveryCodeSignInAsync(
                            loginContext.LoginRequest.TwoFactorRecoveryCode);
                    }

                    if (!result.Succeeded)
                        return Left<Exception, Unit>(new UnauthorizedAccessException("Authentication failed"));

                    context.CurrentIdentityUser = user;
                    return Right<Exception, Unit>(unit);
                }

                return Right<Exception, Unit>(unit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating credentials");
                return Left<Exception, Unit>(ex);
            }
        }
    }
}