using System;
using PCIShield.Domain.Entities;
using PCIShieldLib.SharedKernel;
using System.Text.Json;

namespace PCIShield.Domain.Events
{
    public class ControlEvidenceUpdatedEvent : BaseDomainEvent
    {
        public ControlEvidenceUpdatedEvent(ControlEvidence controlEvidence, string action)
        {
            ActionOnMessageReceived = action;
            
            EntityNameType = "ControlEvidence";
            
            Content = System.Text.Json.JsonSerializer.Serialize(controlEvidence, JsonSerializerSettingsSingleton.Instance);
            
            EventType = "ControlEvidenceUpdated";
            
            EventId = UuidV7Generator.NewUuidV7();
            
            OccurredOnUtc = DateTime.UtcNow;
        }
    }
}

