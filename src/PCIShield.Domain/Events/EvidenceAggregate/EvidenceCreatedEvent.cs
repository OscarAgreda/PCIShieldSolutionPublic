using System;
using PCIShield.Domain.Entities;
using PCIShieldLib.SharedKernel;
using System.Text.Json;

namespace PCIShield.Domain.Events
{
    public class EvidenceCreatedEvent : BaseDomainEvent
    {
        public EvidenceCreatedEvent(Evidence evidence, string action)
        {
            ActionOnMessageReceived = action;
            
            EntityNameType = "Evidence";
            
            Content = System.Text.Json.JsonSerializer.Serialize(evidence, JsonSerializerSettingsSingleton.Instance);
            
            EventType = "EvidenceCreated";
            
            EventId = UuidV7Generator.NewUuidV7();
            
            OccurredOnUtc = DateTime.UtcNow;
        }
    }
}

