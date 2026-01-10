using System;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using PCIShield.Infrastructure.Data;
using PCIShield.Infrastructure.Services;
using LanguageExt;
using Microsoft.AspNetCore.Identity;
using static LanguageExt.Prelude;
using System.Linq;

namespace PCIShield.Api.Auth.ApplicationUserOnly
{
    public class GenerateRefreshTokenCommand : AuthBusinessLogicSagaHandlerTemplate
    {
        private readonly UserManager<CustomPCIShieldUser> _userManager;
        private readonly IAppLoggerService<GenerateRefreshTokenCommand> _logger;

        public GenerateRefreshTokenCommand(
            UserManager<CustomPCIShieldUser> userManager,
            IAppLoggerService<GenerateRefreshTokenCommand> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        protected override async Task<Either<Exception, Unit>> ExecuteSpecificAsync(AuthUpdateContext context, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("=== Starting GenerateRefreshTokenCommand ===");

                var refreshToken = GenerateRefreshToken();
                _logger.LogInformation("Generated refresh token: {TokenPrefix}...", refreshToken.Substring(0, Math.Min(10, refreshToken.Length)));

                context.RefreshToken = refreshToken;
                _logger.LogInformation("Set context.RefreshToken");

                if (context.CurrentIdentityUser != null)
                {
                    _logger.LogInformation("Updating user's refresh token in database for user: {UserId}", context.CurrentIdentityUser.Id);
                    context.CurrentIdentityUser.RefreshToken = refreshToken;
                    var updateResult = await _userManager.UpdateAsync(context.CurrentIdentityUser);

                    if (!updateResult.Succeeded)
                    {
                        var errors = string.Join(", ", updateResult.Errors.ToList());
                        _logger.LogError("Failed to update user's refresh token: {Errors}", errors);
                        return Left<Exception, Unit>(new InvalidOperationException($"Failed to update refresh token: {errors}"));
                    }

                    _logger.LogInformation("Successfully updated user's refresh token");
                }
                else
                {
                    _logger.LogWarning("CurrentIdentityUser is null, cannot persist refresh token");
                }

                _logger.LogInformation("=== Completed GenerateRefreshTokenCommand ===");
                return Right<Exception, Unit>(unit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating refresh token");
                return Left<Exception, Unit>(ex);
            }
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
     
    }
}