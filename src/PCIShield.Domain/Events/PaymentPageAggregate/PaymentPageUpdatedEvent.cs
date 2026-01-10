using System;
using PCIShield.Domain.Entities;
using PCIShieldLib.SharedKernel;
using System.Text.Json;

namespace PCIShield.Domain.Events
{
    public class PaymentPageUpdatedEvent : BaseDomainEvent
    {
        public PaymentPageUpdatedEvent(PaymentPage paymentPage, string action)
        {
            ActionOnMessageReceived = action;
            
            EntityNameType = "PaymentPage";
            
            Content = System.Text.Json.JsonSerializer.Serialize(paymentPage, JsonSerializerSettingsSingleton.Instance);
            
            EventType = "PaymentPageUpdated";
            
            EventId = UuidV7Generator.NewUuidV7();
            
            OccurredOnUtc = DateTime.UtcNow;
        }
    }
}

