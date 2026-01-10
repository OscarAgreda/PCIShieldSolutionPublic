using System;
using System.Security.Cryptography;
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
    public class LoadUserDataForLoginCommand : AuthBusinessLogicSagaHandlerTemplate
    {
        private readonly IRepository<ApplicationUser> _userRepository;
        private readonly UserManager<CustomPCIShieldUser> _userManager;
        private readonly IAppLoggerService<LoadUserDataForLoginCommand> _logger;

        public LoadUserDataForLoginCommand(
            IRepository<ApplicationUser> userRepository,
            UserManager<CustomPCIShieldUser> userManager,
            IAppLoggerService<LoadUserDataForLoginCommand> logger)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _logger = logger;
        }

        protected override async Task<Either<Exception, Unit>> ExecuteSpecificAsync(
            AuthUpdateContext context,
            CancellationToken cancellationToken)
        {
            try
            {
                if (context is LoginAuthContext loginContext)
                {
                    var applicationUserIdGuid = context.CurrentIdentityUser.ApplicationUserId;
                    context.CurrentApplicationUser = await _userRepository.GetByIdAsync(applicationUserIdGuid, cancellationToken);

                    if (context.CurrentApplicationUser != null)
                    {
                        var roles = await _userManager.GetRolesAsync(context.CurrentIdentityUser);
                        var code = GenerateOneTimeCode();
                        var appUserIdStr = context.CurrentApplicationUser.ApplicationUserId.ToString();
                        var tenantIdStr = context.CurrentApplicationUser.TenantId.ToString();
                        context.PCIShieldAppPowerUserId = $"{appUserIdStr}:{tenantIdStr}:{code}";
                        _logger.LogInformation($"User login data loaded for {context.CurrentIdentityUser.UserName}");

                        loginContext.IsFullyRegistered = context.CurrentApplicationUser.IsFullyRegistered;
                    }
                    else
                    {
                        _logger.LogWarning($"ApplicationUser not found for IdentityUser: {context.CurrentIdentityUser.UserName} (ID: {context.CurrentIdentityUser.Id})");
                        return Left<Exception, Unit>(new InvalidOperationException("Associated application user record not found."));
                    }
                }

                return Right<Exception, Unit>(unit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading user data for login");
                return Left<Exception, Unit>(ex);
            }
        }

        private static string GenerateOneTimeCode()
        {
            using var rng = RandomNumberGenerator.Create();
            var randomNumber = new byte[4];
            rng.GetBytes(randomNumber);
            var value = BitConverter.ToInt32(randomNumber, 0);
            return Math.Abs(value % 1000000).ToString("D6");
        }
    }
}