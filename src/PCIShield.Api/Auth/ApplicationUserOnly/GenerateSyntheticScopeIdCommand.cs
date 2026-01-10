using System;
using System.Threading;
using System.Threading.Tasks;
using PCIShield.Infrastructure.Services;
using LanguageExt;
using Microsoft.Extensions.Configuration;
using static LanguageExt.Prelude;

namespace PCIShield.Api.Auth.ApplicationUserOnly
{
    public class GenerateTenantIdCommand : AuthBusinessLogicSagaHandlerTemplate
    {
        private readonly IConfiguration _configuration;
        private readonly IAppLoggerService<GenerateTenantIdCommand> _logger;

        public GenerateTenantIdCommand(
            IConfiguration configuration,
            IAppLoggerService<GenerateTenantIdCommand> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        protected override async Task<Either<Exception, Unit>> ExecuteSpecificAsync(AuthUpdateContext context, CancellationToken cancellationToken)
        {
            try
            {
                if (context.CurrentIdentityUser == null)
                {
                    _logger.LogError("Cannot generate synthetic tenant ID without user context");
                    return Left<Exception, Unit>(new InvalidOperationException("User context is required for tenant ID generation"));
                }
                var deploymentLabel = _configuration["App:DeploymentLabel"];
                if (string.IsNullOrWhiteSpace(deploymentLabel))
                {
                    _logger.LogWarning("DeploymentLabel not configured, defaulting to 'dev'");
                    deploymentLabel = "dev";
                }
                var userId = context.CurrentIdentityUser.Id;
                string sessionId;

                if (!string.IsNullOrWhiteSpace(context.RefreshToken))
                {
                    sessionId = context.RefreshToken;
                    _logger.LogInformation("Using refresh token as session ID for tenant scope");
                }
                else
                {
                    sessionId = Guid.NewGuid().ToString("N");
                    _logger.LogWarning("RefreshToken not available, generated temporary session ID. This may indicate incorrect saga execution order.");
                }
                var syntheticTenantId = $"{deploymentLabel}:{userId}:{sessionId}";
                context.TenantId = syntheticTenantId;

                _logger.LogInformation(
                    "Generated synthetic tenant ID for user {UserId}. Environment: {Environment}, SessionPrefix: {SessionPrefix}",
                    userId,
                    deploymentLabel,
                    sessionId.Substring(0, Math.Min(8, sessionId.Length))
                );

                return await Task.FromResult(Right<Exception, Unit>(unit));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Critical error generating synthetic tenant ID");
                return Left<Exception, Unit>(ex);
            }
        }
    }
}