using System;
using System.Threading;
using System.Threading.Tasks;

using PCIShield.Infrastructure.Data;
using PCIShield.Infrastructure.Services;
using LanguageExt;
using static LanguageExt.Prelude;

namespace PCIShield.Api.Auth.ApplicationUserOnly
{
    public class PreLoadCacheCommand : AuthBusinessLogicSagaHandlerTemplate
    {
      
        private readonly IAppLoggerService<PreLoadCacheCommand> _logger;

        public PreLoadCacheCommand(
        
            IAppLoggerService<PreLoadCacheCommand> logger)
        {
          
            _logger = logger;
        }

        protected override async Task<Either<Exception, Unit>> ExecuteSpecificAsync(
            AuthUpdateContext context,
            CancellationToken cancellationToken)
        {
            try
            {
                if (context is LoginAuthContext && context.CurrentApplicationUser != null && !string.IsNullOrEmpty(context.PCIShieldAppPowerUserId))
                {

                    _logger.LogInformation($"Cache preloading would be initiated for ApplicationUserId: {context.CurrentApplicationUser.ApplicationUserId}");
                    return await Task.FromResult(Right<Exception, Unit>(unit));
                }
                return await Task.FromResult(Right<Exception, Unit>(unit));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error preloading cache for ApplicationUser");
                return Left<Exception, Unit>(ex);
            }
        }
    }
}