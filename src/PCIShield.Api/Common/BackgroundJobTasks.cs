using Microsoft.Extensions.Logging;
using PCIShield.Infrastructure.Services;
namespace PCIShield.Api.Common
{
    public class ApiBackgroundJobTasks
    {
        private readonly IAppLoggerService<ApiBackgroundJobTasks> _logger;
        public ApiBackgroundJobTasks(IAppLoggerService<ApiBackgroundJobTasks> logger)
        {
            _logger = logger;
        }
        public void DoSomethingRegularly()
        {
            _logger.LogInformation("Doing something regularly  enqued...");
        }
    }
}
