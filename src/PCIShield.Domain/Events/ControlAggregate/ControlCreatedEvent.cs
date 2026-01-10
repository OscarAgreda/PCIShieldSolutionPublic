using System;
using PCIShield.Domain.Entities;
using PCIShieldLib.SharedKernel;
using System.Text.Json;

namespace PCIShield.Domain.Events
{
    public class ControlCreatedEvent : BaseDomainEvent
    {
        public ControlCreatedEvent(Control control, string action)
        {
            ActionOnMessageReceived = action;
            
            EntityNameType = "Control";
            
            Content = System.Text.Json.JsonSerializer.Serialize(control, JsonSerializerSettingsSingleton.Instance);
            
            EventType = "ControlCreated";
            
            EventId = UuidV7Generator.NewUuidV7();
            
            OccurredOnUtc = DateTime.UtcNow;
        }
    }
}

