using System;
using PCIShield.Domain.Entities;
using PCIShieldLib.SharedKernel;
using System.Text.Json;

namespace PCIShield.Domain.Events
{
    public class PaymentPageCreatedEvent : BaseDomainEvent
    {
        public PaymentPageCreatedEvent(PaymentPage paymentPage, string action)
        {
            ActionOnMessageReceived = action;
            
            EntityNameType = "PaymentPage";
            
            Content = System.Text.Json.JsonSerializer.Serialize(paymentPage, JsonSerializerSettingsSingleton.Instance);
            
            EventType = "PaymentPageCreated";
            
            EventId = UuidV7Generator.NewUuidV7();
            
            OccurredOnUtc = DateTime.UtcNow;
        }
    }
}

