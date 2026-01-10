using System;
using PCIShield.Domain.Entities;
using PCIShieldLib.SharedKernel;
using System.Text.Json;

namespace PCIShield.Domain.Events
{
    public class ScriptCreatedEvent : BaseDomainEvent
    {
        public ScriptCreatedEvent(Script script, string action)
        {
            ActionOnMessageReceived = action;
            
            EntityNameType = "Script";
            
            Content = System.Text.Json.JsonSerializer.Serialize(script, JsonSerializerSettingsSingleton.Instance);
            
            EventType = "ScriptCreated";
            
            EventId = UuidV7Generator.NewUuidV7();
            
            OccurredOnUtc = DateTime.UtcNow;
        }
    }
}

