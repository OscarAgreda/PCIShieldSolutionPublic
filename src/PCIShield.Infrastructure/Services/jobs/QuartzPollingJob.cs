using System;
using System.Threading.Tasks;
using MediatR;
using Quartz;
using PCIShield.Infrastructure.Services;
using PCIShieldLib.SharedKernel;
namespace PCIShield.Infrastructure.Services.jobs
{
    [DisallowConcurrentExecution]
    public class QuartzPollingJob : IJob
    {
        private readonly IAppLoggerService<QuartzPollingJob> _logger;
        private readonly IPollyService _pollyService;
        private readonly IDapperMessagingService _dapperService;
        private readonly IMediator _mediator;
        public QuartzPollingJob(IAppLoggerService<QuartzPollingJob> logger, IPollyService pollyService, IDapperMessagingService dapperService, IMediator mediator)
        {
            _logger = logger;
            _pollyService = pollyService;
            _dapperService = dapperService;
            _mediator = mediator;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("QuartzPollingJob started execution.");
            try
            {
                string sql = "SELECT TOP 1 * FROM OutBoxMessage ORDER BY OccurredOnUtc DESC";
                OutBoxMessage message = await _dapperService.QuerySingleOrDefaultAsync<OutBoxMessage>(sql);
                _logger.LogInformation($"Polled message: {message.Content}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while polling the database.");
            }
            _logger.LogInformation("QuartzPollingJob finished execution.");
        }
    }
}