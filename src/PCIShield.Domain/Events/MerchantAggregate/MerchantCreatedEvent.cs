using System;
using PCIShield.Domain.Entities;
using PCIShieldLib.SharedKernel;
using System.Text.Json;

namespace PCIShield.Domain.Events
{
    public class MerchantCreatedEvent : BaseDomainEvent
    {
        public MerchantCreatedEvent(Merchant merchant, string action)
        {
            ActionOnMessageReceived = action;
            
            EntityNameType = "Merchant";
            
            Content = System.Text.Json.JsonSerializer.Serialize(merchant, JsonSerializerSettingsSingleton.Instance);
            
            EventType = "MerchantCreated";
            
            EventId = UuidV7Generator.NewUuidV7();
            
            OccurredOnUtc = DateTime.UtcNow;
        }
    }
}

