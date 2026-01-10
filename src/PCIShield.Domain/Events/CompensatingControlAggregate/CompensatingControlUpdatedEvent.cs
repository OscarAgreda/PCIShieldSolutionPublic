using System;
using PCIShield.Domain.Entities;
using PCIShieldLib.SharedKernel;
using System.Text.Json;

namespace PCIShield.Domain.Events
{
    public class CompensatingControlUpdatedEvent : BaseDomainEvent
    {
        public CompensatingControlUpdatedEvent(CompensatingControl compensatingControl, string action)
        {
            ActionOnMessageReceived = action;
            
            EntityNameType = "CompensatingControl";
            
            Content = System.Text.Json.JsonSerializer.Serialize(compensatingControl, JsonSerializerSettingsSingleton.Instance);
            
            EventType = "CompensatingControlUpdated";
            
            EventId = UuidV7Generator.NewUuidV7();
            
            OccurredOnUtc = DateTime.UtcNow;
        }
    }
}

