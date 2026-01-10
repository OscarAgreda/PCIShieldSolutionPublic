using System;
using PCIShield.Domain.Entities;
using PCIShieldLib.SharedKernel;
using System.Text.Json;

namespace PCIShield.Domain.Events
{
    public class ControlCategoryCreatedEvent : BaseDomainEvent
    {
        public ControlCategoryCreatedEvent(ControlCategory controlCategory, string action)
        {
            ActionOnMessageReceived = action;
            
            EntityNameType = "ControlCategory";
            
            Content = System.Text.Json.JsonSerializer.Serialize(controlCategory, JsonSerializerSettingsSingleton.Instance);
            
            EventType = "ControlCategoryCreated";
            
            EventId = UuidV7Generator.NewUuidV7();
            
            OccurredOnUtc = DateTime.UtcNow;
        }
    }
}

