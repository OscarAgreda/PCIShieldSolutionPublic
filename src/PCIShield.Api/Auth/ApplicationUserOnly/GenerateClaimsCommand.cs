using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using PCIShield.Infrastructure.Data;
using PCIShield.Infrastructure.Services;
using LanguageExt;
using Microsoft.AspNetCore.Identity;
using static LanguageExt.Prelude;

namespace PCIShield.Api.Auth.ApplicationUserOnly
{
    public class GenerateClaimsCommand : AuthBusinessLogicSagaHandlerTemplate
    {
        private readonly UserManager<CustomPCIShieldUser> _userManager;
        private readonly IClaimsGenerationService _claimsService;
        private readonly IAppLoggerService<GenerateClaimsCommand> _logger;

        public GenerateClaimsCommand(
            UserManager<CustomPCIShieldUser> userManager,
            IClaimsGenerationService claimsService,
            IAppLoggerService<GenerateClaimsCommand> logger)
        {
            _userManager = userManager;
            _claimsService = claimsService;
            _logger = logger;
        }

        protected override async Task<Either<Exception, Unit>> ExecuteSpecificAsync(
            AuthUpdateContext context,
            CancellationToken cancellationToken)
        {
            try
            {
                Guard.Against.Null(context.CurrentIdentityUser, nameof(context.CurrentIdentityUser));

                var user = context.CurrentIdentityUser;
                var roles = await _userManager.GetRolesAsync(user);
                var claims = await _claimsService.GenerateClaimsForUserAsync(
                    user, 
                    roles, 
                    context.PCIShieldAppPowerUserId, 
                    context.TenantId);

                context.GeneratedClaims = Right<Exception, List<Claim>>(claims);
                return Right<Exception, Unit>(unit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating claims");
                context.GeneratedClaims = Left<Exception, List<Claim>>(ex);
                return Left<Exception, Unit>(ex);
            }
        }
    }
}