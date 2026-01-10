using System;
using PCIShield.Domain.Entities;
using PCIShieldLib.SharedKernel;
using System.Text.Json;

namespace PCIShield.Domain.Events
{
    public class ControlEvidenceCreatedEvent : BaseDomainEvent
    {
        public ControlEvidenceCreatedEvent(ControlEvidence controlEvidence, string action)
        {
            ActionOnMessageReceived = action;
            
            EntityNameType = "ControlEvidence";
            
            Content = System.Text.Json.JsonSerializer.Serialize(controlEvidence, JsonSerializerSettingsSingleton.Instance);
            
            EventType = "ControlEvidenceCreated";
            
            EventId = UuidV7Generator.NewUuidV7();
            
            OccurredOnUtc = DateTime.UtcNow;
        }
    }
}

