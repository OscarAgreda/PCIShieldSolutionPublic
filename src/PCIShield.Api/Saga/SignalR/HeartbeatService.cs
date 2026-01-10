using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using PCIShield.Infrastructure.Services;
using PCIShield.Domain.Entities;
using PCIShield.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using PCIShieldLib.SharedKernel.Interfaces;
using BlazorMauiShared.Models.Merchant;
using PCIShield.Domain.Specifications;
using System.Collections.Generic;
using PCIShieldCore.Domain.Specifications;

namespace PCIShield.Api.Saga.SignalR
{
    public class HeartbeatService : IHeartbeatService
    {
        private readonly IHubContext<PCIShieldAppApiSignalrHub> _hubContext;
        private readonly IAppLoggerService<PCIShieldAppApiSignalrHub> _logger;
        private readonly IRepository<ComplianceOfficer> _complianceOfficerRepository;
        private readonly IRepository<Merchant> _merchantRepository;
        private readonly ComplianceOfficerMerchantConnections _complianceOfficerMerchantConnections;
        private Timer _heartbeatTimer;

        public HeartbeatService(
            IHubContext<PCIShieldAppApiSignalrHub> hubContext,
            IAppLoggerService<PCIShieldAppApiSignalrHub> logger,
            IRepository<Merchant> merchantRepository,
            IRepository<ComplianceOfficer> complianceOfficerRepository,
            ComplianceOfficerMerchantConnections complianceOfficerMerchantConnections)
        {
            _logger = logger;
            _hubContext = hubContext;
            _merchantRepository = merchantRepository;
            _complianceOfficerRepository = complianceOfficerRepository;
            _complianceOfficerMerchantConnections = complianceOfficerMerchantConnections;
        }

        public void Start()
        {
            _heartbeatTimer = new Timer(SendHeartbeat, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
        }

        public void Stop()
        {
            _heartbeatTimer?.Dispose();
        }

        private async void SendHeartbeat(object state)
        {
            try
            {
                await _hubContext.Clients.All.SendAsync("ReceiveHeartbeatFromServerComplianceOfficer", DateTime.UtcNow);
                await _hubContext.Clients.All.SendAsync("ReceiveHeartbeatFromServerMerchant", DateTime.UtcNow);
                await CheckOfflineMerchants();
                await CheckOfflineComplianceOfficers();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending heartbeat.");
            }
        }

        private async Task CheckOfflineMerchants()
        {
            try
            {
                var connectedMerchantIds = _complianceOfficerMerchantConnections.Connections.Values;
                var spec = new MerchantGetConnectedListSpec(connectedMerchantIds);
                var merchants = await _merchantRepository.ListAsync(spec);
                foreach (var merchant in merchants)
                {
                    if (DateTime.UtcNow - merchant.UpdatedAt > TimeSpan.FromMinutes(2))
                    {
                        await _merchantRepository.UpdateAsync(merchant);

                        var message = new StructuredSignalRMessage
                        {
                            MerchantId = merchant.MerchantId,
                            ReceivedMerchantUserId = merchant.TenantId
                        };
                        if (_complianceOfficerMerchantConnections.Connections.TryGetValue(merchant.TenantId.ToString(), out var complianceOfficerUserId))
                        {
                            await _hubContext.Clients.Group(complianceOfficerUserId).SendAsync("MerchantOfflineNotification", message);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error  CheckOfflineMerchants {e.Message}.");
            }
           
        }

        private async Task CheckOfflineComplianceOfficers()
        {
            try
            {
                var connectedComplianceOfficerIds = _complianceOfficerMerchantConnections.Connections.Keys;
                var spec = new ComplianceOfficerGetConnectedListSpec(connectedComplianceOfficerIds);
                var complianceOfficers = await _complianceOfficerRepository.ListAsync(spec);
                foreach (var complianceOfficer in complianceOfficers)
                {
                    if (DateTime.UtcNow - complianceOfficer.UpdatedAt > TimeSpan.FromMinutes(2))
                    {
                        await _complianceOfficerRepository.UpdateAsync(complianceOfficer);

                        var message = new StructuredSignalRMessage
                        {
                            ComplianceOfficerId = complianceOfficer.ComplianceOfficerId,
                            ReceivedComplianceOfficerUserId = complianceOfficer.TenantId ,
                        };
                        if (_complianceOfficerMerchantConnections.Connections.TryGetValue(complianceOfficer.TenantId.ToString(), out var merchantUserId))
                        {
                            await _hubContext.Clients.Group(merchantUserId).SendAsync("ComplianceOfficerOfflineNotification", message);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error  CheckOfflineComplianceOfficers {e.Message}.");
            }
           
        }
    }
}
