using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Polly;
using Quartz;
using PCIShield.Infrastructure.Services;
using PCIShieldLib.SharedKernel;
namespace PCIShield.Infrastructure.Services.jobs
{
     [DisallowConcurrentExecution]
    public class QuartzSaverJob : IJob
    {
        private readonly IDapperMessagingService _dapperService;
        private readonly IAppLoggerService<QuartzSaverJob> _logger;
        private readonly IPollyService _pollyService;
        public QuartzSaverJob(IDapperMessagingService dapperService, IAppLoggerService<QuartzSaverJob> logger, IPollyService pollyService)
        {
            _dapperService = dapperService;
            _logger = logger;
            _pollyService = pollyService;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("QuartzSaverJob started execution.");
            var message = (OutBoxMessage)context.JobDetail.JobDataMap.Get("message");
            try
            {
                   string sql = "INSERT INTO OutBoxMessage (OutBoxMessageEventId, UserId, TenantId, EventType, EntityNameType, ActionOnMessageReceived, Content, OccurredOnUtc) VALUES (@OutBoxMessageEventId, @UserId, @TenantId, @EventType, @EntityNameType, @ActionOnMessageReceived, @Content, @OccurredOnUtc)";
                   await _dapperService.ExecuteAsync(sql, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the message to the database.");
            }
            _logger.LogInformation("QuartzSaverJob finished execution.");
        }
    }
}
