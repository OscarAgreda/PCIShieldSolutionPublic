using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PCIShield.Domain.Events;
using PCIShieldLib.SharedKernel;
namespace PCIShield.Infrastructure.Services
{
    public class DapperMediatREventHandler : INotificationHandler<DapperDomainEvent>
    {
        private readonly IDapperMessagingService _dapperService;
        private readonly IAppLoggerService<DapperMediatREventHandler> _logger;
        private readonly IPollyService _pollyService;
        public DapperMediatREventHandler(IDapperMessagingService dapperService, IAppLoggerService<DapperMediatREventHandler> logger, IPollyService pollyService)
        {
            _dapperService = dapperService;
            _logger = logger;
            _pollyService = pollyService;
        }
        public async Task Handle(DapperDomainEvent message, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling OutBoxMessage.");
            switch (message)
            {
                default:
                    break;
            }
            await Task.CompletedTask;
            return;
        }
        private async Task ProcessDapperCustomerCreatedEvent(DapperDomainEvent message, CancellationToken cancellationToken)
        {
            try
            {
                string sql = "INSERT INTO OutBoxMessage (EventId, UserId, TenantId, EventType, EntityNameType, ActionOnMessageReceived, Content, OccurredOnUtc, IsProcessed) VALUES (@EventId, @UserId, @TenantId, @EventType, @EntityNameType, @ActionOnMessageReceived, @Content, @OccurredOnUtc, 0)";
                await _dapperService.ExecuteAsync(sql, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the message to the database.");
            }
            _logger.LogInformation("Finished handling OutBoxMessage.");
        }
        private async Task ProcessMessageCreatedEvent(DapperDomainEvent message, CancellationToken cancellationToken)
        {
            try
            {
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the message to the database.");
            }
            _logger.LogInformation("Finished handling OutBoxMessage.");
        }
    }
}
