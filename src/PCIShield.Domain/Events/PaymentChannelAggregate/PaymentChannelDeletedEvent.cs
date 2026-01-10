using System;
using PCIShield.Domain.Entities;
using PCIShieldLib.SharedKernel;
using System.Text.Json;

namespace PCIShield.Domain.Events
{
    public class PaymentChannelDeletedEvent : BaseDomainEvent
    {
        public PaymentChannelDeletedEvent(PaymentChannel paymentChannel, string action)
        {
            ActionOnMessageReceived = action;
            
            EntityNameType = "PaymentChannel";
            
            Content = System.Text.Json.JsonSerializer.Serialize(paymentChannel, JsonSerializerSettingsSingleton.Instance);
            
            EventType = "PaymentChannelDeleted";
            
            EventId = UuidV7Generator.NewUuidV7();
            
            OccurredOnUtc = DateTime.UtcNow;
        }
    }
}

