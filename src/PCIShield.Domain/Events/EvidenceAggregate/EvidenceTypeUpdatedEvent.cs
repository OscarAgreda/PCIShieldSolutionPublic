using System;
using PCIShield.Domain.Entities;
using PCIShieldLib.SharedKernel;
using System.Text.Json;

namespace PCIShield.Domain.Events
{
    public class EvidenceTypeUpdatedEvent : BaseDomainEvent
    {
        public EvidenceTypeUpdatedEvent(EvidenceType evidenceType, string action)
        {
            ActionOnMessageReceived = action;
            
            EntityNameType = "EvidenceType";
            
            Content = System.Text.Json.JsonSerializer.Serialize(evidenceType, JsonSerializerSettingsSingleton.Instance);
            
            EventType = "EvidenceTypeUpdated";
            
            EventId = UuidV7Generator.NewUuidV7();
            
            OccurredOnUtc = DateTime.UtcNow;
        }
    }
}

