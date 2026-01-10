using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PCIShield.Infrastructure.Services;
using LanguageExt;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using static LanguageExt.Prelude;

namespace PCIShield.Api.Auth.ApplicationUserOnly
{
    public class GenerateJwtTokenCommand : AuthBusinessLogicSagaHandlerTemplate
    {
        private readonly IConfiguration _configuration;
        private readonly IAppLoggerService<GenerateJwtTokenCommand> _logger;

        public GenerateJwtTokenCommand(
            IConfiguration configuration,
            IAppLoggerService<GenerateJwtTokenCommand> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        protected override async Task<Either<Exception, Unit>> ExecuteSpecificAsync(
            AuthUpdateContext context,
            CancellationToken cancellationToken)
        {
            try
            {
                return await context.GeneratedClaims.MatchAsync(
                     async claims =>
                    {
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
                        return Right<Exception, Unit>(unit);
                    },
                     ex =>
                    {
                        _logger.LogError(ex, "Error generating JWT token - claims generation failed");
                        return Task.FromResult(Left<Exception, Unit>(ex));
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating JWT token");
                return Left<Exception, Unit>(ex);
            }
        }
    }
}