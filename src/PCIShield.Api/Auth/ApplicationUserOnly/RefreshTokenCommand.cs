using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PCIShield.Infrastructure.Data;
using PCIShield.Infrastructure.Services;
using LanguageExt;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using static LanguageExt.Prelude;

namespace PCIShield.Api.Auth.ApplicationUserOnly
{
    public class RefreshTokenCommand : AuthBusinessLogicSagaHandlerTemplate
    {
        private readonly UserManager<CustomPCIShieldUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IClaimsGenerationService _claimsService;
        private readonly IAppLoggerService<RefreshTokenCommand> _logger;

        public RefreshTokenCommand(
            UserManager<CustomPCIShieldUser> userManager,
            IConfiguration configuration,
            IClaimsGenerationService claimsService,
            IAppLoggerService<RefreshTokenCommand> logger)
        {
            _userManager = userManager;
            _configuration = configuration;
            _claimsService = claimsService;
            _logger = logger;
        }

        protected override async Task<Either<Exception, Unit>> ExecuteSpecificAsync(
            AuthUpdateContext context,
            CancellationToken cancellationToken)
        {
            try
            {
                if (context is RefreshTokenAuthContext refreshContext)
                {
                    var user = await _userManager.FindByRefreshTokenAsync(refreshContext.RefreshTokenValue, cancellationToken);

                    if (user == null)
                        return Left<Exception, Unit>(new UnauthorizedAccessException("Invalid refresh token"));

                    var roles = await _userManager.GetRolesAsync(user);
                    var existingClaims = await _userManager.GetClaimsAsync(user);
                    var pciShieldAppPowerUserId = existingClaims.FirstOrDefault(c => c.Type == "PCIShieldSolutionuperUserId")?.Value;
                    var tenantId = existingClaims.FirstOrDefault(c => c.Type == "TenantId")?.Value;
                    var claims = await _claimsService.GenerateClaimsForUserAsync(
                        user, 
                        roles, 
                        pciShieldAppPowerUserId, 
                        tenantId);

                    var token = new JwtSecurityToken(
                        _configuration["JwtSettings:PCISHIELDAPP_ISSUER"],
                        _configuration["JwtSettings:PCISHIELDAPP_AUDIENCE"],
                        claims,
                        expires: DateTime.UtcNow.AddDays(1),
                        signingCredentials: new SigningCredentials(
                            new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(_configuration["JwtSettings:PCISHIELDAPP_SECRET_KEY"] ?? "")),
                            SecurityAlgorithms.HmacSha256));

                    context.GeneratedToken = new JwtSecurityTokenHandler().WriteToken(token);
                    using var rng = RandomNumberGenerator.Create();
                    var randomNumber = new byte[32];
                    rng.GetBytes(randomNumber);
                    var newRefreshToken = Convert.ToBase64String(randomNumber);

                    user.RefreshToken = newRefreshToken;
                    await _userManager.UpdateAsync(user);

                    context.RefreshToken = newRefreshToken;
                    context.CurrentIdentityUser = user;

                    return Right<Exception, Unit>(unit);
                }

                return Right<Exception, Unit>(unit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing token");
                return Left<Exception, Unit>(ex);
            }
        }
    }
}