using System;
using System.Threading;
using System.Threading.Tasks;
using PCIShield.Domain.Entities;
using PCIShield.Infrastructure.Data;
using PCIShield.Infrastructure.Services;
using PCIShieldLib.SharedKernel.Interfaces;
using LanguageExt;
using Microsoft.AspNetCore.Identity;
using static LanguageExt.Prelude;

namespace PCIShield.Api.Auth.ApplicationUserOnly
{
    public class LogoutUserCommand : AuthBusinessLogicSagaHandlerTemplate
    {
        private readonly UserManager<CustomPCIShieldUser> _userManager;
        private readonly SignInManager<CustomPCIShieldUser> _signInManager;
        private readonly IRepository<ApplicationUser> _userRepository;
        private readonly IAppLoggerService<LogoutUserCommand> _logger;

        public LogoutUserCommand(
            UserManager<CustomPCIShieldUser> userManager,
            SignInManager<CustomPCIShieldUser> signInManager,
            IRepository<ApplicationUser> userRepository,
            IAppLoggerService<LogoutUserCommand> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userRepository = userRepository;
            _logger = logger;
        }

        protected override async Task<Either<Exception, Unit>> ExecuteSpecificAsync(
            AuthUpdateContext context,
            CancellationToken cancellationToken)
        {
            try
            {
                if (context is LogoutAuthContext logoutContext)
                {
                    var user = await _userManager.FindByIdAsync(logoutContext.UserId);
                    if (user != null)
                    {
                        user.RefreshToken = null;
                        await _userManager.UpdateAsync(user);
                        await _userManager.UpdateSecurityStampAsync(user);
                        var applicationUser = await _userRepository.GetByIdAsync(user.ApplicationUserId, cancellationToken);
                        if (applicationUser != null)
                        {
                            applicationUser.SetIsOnline(false);
                            applicationUser.SetIsLoggedIntoApp(false);
                            applicationUser.SetAvailabilityRank("Offline");
                            await _userRepository.UpdateAsync(applicationUser, cancellationToken);
                        }

                        await _signInManager.SignOutAsync();
                    }

                    return Right<Exception, Unit>(unit);
                }

                return Right<Exception, Unit>(unit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging out user");
                return Left<Exception, Unit>(ex);
            }
        }
    }
}