using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BlazorMauiShared.Models.Merchant;

using PCIShield.BlazorMauiShared.Models;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using PCIShield.Domain.Entities;
using PCIShield.Domain.Events;
using PCIShield.Domain.ModelsDto;
using PCIShield.Domain.Specifications;
using PCIShield.Infrastructure.Data;
using PCIShield.Infrastructure.Services;

using PCIShieldLib.SharedKernel.Interfaces;
using PCIShield.BlazorMauiShared.CustomDto;

namespace PCIShield.Api.Saga.SignalR
{
    [Authorize(Policy = "MerchantOrComplianceOfficer")]
    public class PCIShieldAppApiSignalrHub : Hub, IPCIShieldAppApiSignalrHub
    {
   
        private readonly IRepository<ComplianceOfficer> _complianceOfficerRepository;
        private readonly ChatEventProcessor _chatEventProcessor;
        private readonly IRepository<Merchant> _merchantRepository;
        private readonly IAppLoggerService<PCIShieldAppApiSignalrHub> _logger;
        private readonly IMediator _mediator;
        private readonly Dictionary<string, TaskCompletionSource<bool>> _pendingResponses = new();
        private readonly ProcessedMessagesStore _processedMessagesStore;
        private readonly IMerchantComplianceOfficerMessagePublisherService _rabbitMqPublisherService;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        
        private readonly ComplianceOfficerMerchantConnections _complianceOfficerMerchantConnections;

        public PCIShieldAppApiSignalrHub(
        
            ProcessedMessagesStore processedMessagesStore,
        
            ChatEventProcessor chatEventProcessor,
            IMerchantComplianceOfficerMessagePublisherService rabbitMqPublisherService,
            IServiceScopeFactory serviceScopeFactory,
            IAppLoggerService<PCIShieldAppApiSignalrHub> logger,
            IMediator mediator,
            IRepository<Merchant> merchantRepository,
            IRepository<ComplianceOfficer> complianceOfficerRepository,
            ComplianceOfficerMerchantConnections complianceOfficerMerchantConnections)
        {
           
            _processedMessagesStore = processedMessagesStore;
            _merchantRepository = merchantRepository;
            _complianceOfficerRepository = complianceOfficerRepository;
         
            _chatEventProcessor = chatEventProcessor;
            _rabbitMqPublisherService = rabbitMqPublisherService;
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
            _mediator = mediator;
            _complianceOfficerMerchantConnections = complianceOfficerMerchantConnections;
        }

        public async Task JoinUserGroupConnection(string userId)
        {
            _logger.LogInformation($"Adding connection {Context.ConnectionId} to group for user {userId}.");
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
        }

        private bool IsValidJson(string strInput)
        {
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || (strInput.StartsWith("[") && strInput.EndsWith("]")))
            {
                try
                {
                    var obj = JToken.Parse(strInput);
                    return true;
                }
                catch (JsonReaderException jex)
                {
                    _logger.LogError(jex, $"Invalid JSON string. Error in line {jex.LineNumber}, position {jex.LinePosition}: {jex.Message}. String: {strInput}");
                    return false;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error occurred while validating JSON.");
                    return false;
                }
            }
            else
            {
                _logger.LogWarning($"String does not start and end with proper JSON tokens: {strInput}");
                return false;
            }
        }
    }
}
