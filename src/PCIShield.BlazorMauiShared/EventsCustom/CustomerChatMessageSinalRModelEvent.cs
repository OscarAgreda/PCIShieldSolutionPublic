using System;
using System.Text.Json.Serialization;
using PCIShieldLib.SharedKernel;
using PCIShield.Domain.ModelsDto;
using Ardalis.GuardClauses;
using MediatR;
namespace PCIShield.Domain.Events
{
    public class MerchantChatMessageSinalRModelEvent : BaseDomainEvent
    {
        [JsonConstructor]
        public MerchantChatMessageSinalRModelEvent()
        {
        }
        public MerchantChatMessageSinalRModelEvent(MerchantChatMessageSinalRModel MerchantInitiatedChatMsg, string message, string pciShieldappAppSuperUserId)
        {
            PCIShieldSolutionuperUserId = pciShieldappAppSuperUserId;
            ActionOnMessageReceived = message;
            EntityNameType = "Merchant";
            Content = System.Text.Json.JsonSerializer.Serialize(MerchantInitiatedChatMsg, JsonSerializerSettingsSingleton.Instance);
            EventType = "MerchantChatMessageSinalRModelEvent";
            EventId = Guid.NewGuid();
            OccurredOnUtc = DateTime.UtcNow;
            Message = message;
        }
    }
}
