using System;
using PCIShield.Domain.Entities;
using PCIShieldLib.SharedKernel;
using System.Text.Json;

namespace PCIShield.Domain.Events
{
    public class ControlUpdatedEvent : BaseDomainEvent
    {
        public ControlUpdatedEvent(Control control, string action)
        {
            ActionOnMessageReceived = action;
            
            EntityNameType = "Control";
            
            Content = System.Text.Json.JsonSerializer.Serialize(control, JsonSerializerSettingsSingleton.Instance);
            
            EventType = "ControlUpdated";
            
            EventId = UuidV7Generator.NewUuidV7();
            
            OccurredOnUtc = DateTime.UtcNow;
        }
    }
}

