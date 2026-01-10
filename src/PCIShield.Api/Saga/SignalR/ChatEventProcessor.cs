using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;

using PCIShield.BlazorMauiShared.EventsCustom;
using PCIShield.Domain.Events;
using PCIShield.Domain.ModelsDto;
using PCIShield.Infrastructure.Services;

using PCIShieldLib.SharedKernel;
namespace PCIShield.Api.Saga.SignalR
{
    public class ChatEventProcessor
    {

        private readonly IHubContext<PCIShieldAppApiSignalrHub> _hubContext;
        private readonly IAppLoggerService<ChatEventProcessor> _logger;
        private readonly IMerchantComplianceOfficerMessagePublisherService _rabbitMqPublisherService;
        public ChatEventProcessor(IMerchantComplianceOfficerMessagePublisherService rabbitMqPublisherService, IAppLoggerService<ChatEventProcessor> logger, IHubContext<PCIShieldAppApiSignalrHub> hubContext)
        {

            _hubContext = hubContext;
            this._rabbitMqPublisherService = rabbitMqPublisherService;
            this._logger = logger;
        }
        public async Task ClearUserQueues(string userId)
        {
            try
            { }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"Error clearing RabbitMQ queues for user {userId}.");
            }
        }

        public async Task<ChatProcessEventResult> ProcessComplianceOfficerInitChatResponse(ComplianceOfficerChatMessageSinalRModel message, bool fromInternal)
        {
            var complianceOfficerEvent = new ComplianceOfficerRespondedToInitChatMessageEvent(message, message.Message, message.PCIShieldAppPowerUserId);
            var messageType = message.MessageType;
            var routingKey = string.Empty;
            if (messageType == "FirstEverComplianceOfficerFromMaui1")
            {
                routingKey = "complianceOfficer_first_ever_chat_to_server1";
                if (!fromInternal)
                {
                    this.PublishEventToQueue(complianceOfficerEvent, routingKey, message.MessageId);
                }
            }
            else if (messageType == "RocketPapaya2")
            {
                routingKey = "complianceOfficer_responded_init_chat2"; 
                if (!fromInternal)
                {
                    this.PublishEventToQueue(complianceOfficerEvent, routingKey, message.MessageId);
                }
            }
            else if (messageType == "CheeseStack")
            {
                routingKey = "complianceOfficer_continued_regular_chat";
                if (!fromInternal)
                {
                    var messageId = message.MessageId.ToString();
                    string? userId = message.ReceivedMerchantUserId.ToString();
                    await _hubContext.Clients.Group(userId)
                        .SendAsync("JumpingCatUpdate", message);

                    string? complianceOfficerUserId = message.ReceivedComplianceOfficerUserId.ToString();
                    await _hubContext.Clients.Group(complianceOfficerUserId)
                        .SendAsync("FromComplianceOfficerToServerThenBackToComplianceOfficer", message);
                    this.PublishEventToQueue(complianceOfficerEvent, routingKey, message.MessageId);
                }
            }
            try
            {
                complianceOfficerEvent.Message = message.Message;
                complianceOfficerEvent.EventId = message.MessageId;
                complianceOfficerEvent.OccurredOnUtc = DateTime.UtcNow;
                complianceOfficerEvent.ActionOnMessageReceived = message.Message;
                complianceOfficerEvent.EntityNameType = "Merchant";
                complianceOfficerEvent.EventType = routingKey;
                complianceOfficerEvent.Consumer = "SignalR";
                complianceOfficerEvent.UserId = message.UserId;
                complianceOfficerEvent.TenantId = message.TenantId;
                complianceOfficerEvent.IsProcessed = false;
                complianceOfficerEvent.Content = System.Text.Json.JsonSerializer.Serialize(message, JsonSerializerSettingsSingleton.Instance);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error ProcessComplianceOfficerInitChatResponse.");
            }
            return new ChatProcessEventResult() { DomainEvent = complianceOfficerEvent, RoutingKey = routingKey };
        }
        public async Task<ChatProcessEventResult> ProcessMerchantInitiatedTextChat(MerchantChatMessageSinalRModel message, bool fromInternal)
        {

            var merchantEvent = new MerchantChatMessageSinalRModelEvent(message, message.Message, message.PCIShieldAppPowerUserId);
            var messageType = message.MessageType;
            var routingKey = string.Empty;
            try
            {
                if (messageType == "FirstEverMerchantFromMaui")
                {
                    routingKey = "merchant_first_auto_chat";
                    if (!fromInternal)
                    {
                        this.PublishEventToQueue(merchantEvent, routingKey, message.MessageId);
                    }
                }
                else if (messageType == "CustKoalaKarnivalQuery")
                {
                    routingKey = "merchant_first_auto_chat";
                    if (!fromInternal)
                    {
                        this.PublishEventToQueue(merchantEvent, routingKey, message.MessageId);
                    }
                }
                else if (messageType == "CustFrogFiestaFeedback")
                {
                    routingKey = "merchant_sent_regular_chat";
                    if (!fromInternal)
                    {

                        string? complianceOfficerUserId = message.ReceivedComplianceOfficerUserId.ToString();
                        await _hubContext.Clients.Group(complianceOfficerUserId)
                            .SendAsync("FunnyBusinessMessage", message);

                        string? merchantUserId = message.ReceivedMerchantUserId.ToString();
                        await _hubContext.Clients.Group(merchantUserId)
                            .SendAsync("FromMerchantToServerThenBackToMerchant", message);
                        this.PublishEventToQueue(merchantEvent, routingKey, message.MessageId);
                    }
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error ProcessMerchantInitiatedTextChat.");
            }
            try
            {
                merchantEvent.Message = message.Message;
                merchantEvent.EventId = message.MessageId;
                merchantEvent.OccurredOnUtc = DateTime.UtcNow;
                merchantEvent.ActionOnMessageReceived = message.Message;
                merchantEvent.EntityNameType = "Merchant";
                merchantEvent.EventType = routingKey;
                merchantEvent.Consumer = "SignalR";
                merchantEvent.UserId = message.ReceivedMerchantUserId;
                merchantEvent.TenantId = message.TenantId;
                merchantEvent.IsProcessed = false;
                merchantEvent.Content = System.Text.Json.JsonSerializer.Serialize(message, JsonSerializerSettingsSingleton.Instance);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error ProcessMerchantInitiatedTextChat.");
            }
            return new ChatProcessEventResult() { DomainEvent = merchantEvent, RoutingKey = routingKey };
        }
        private void PublishEventToQueue(BaseDomainEvent domainEvent, string routingKey, Guid messageMessageId)
        {
            try
            {
                this._rabbitMqPublisherService.PublishDomainEventToQueue(domainEvent, routingKey, messageMessageId);
            }
          
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"Error publishing event to queue {routingKey}.");
            }
        }
    }
}