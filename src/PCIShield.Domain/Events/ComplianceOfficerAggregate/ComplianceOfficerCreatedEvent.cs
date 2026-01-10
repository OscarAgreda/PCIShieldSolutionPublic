using System;
using PCIShield.Domain.Entities;
using PCIShieldLib.SharedKernel;
using System.Text.Json;

namespace PCIShield.Domain.Events
{
    public class ComplianceOfficerCreatedEvent : BaseDomainEvent
    {
        public ComplianceOfficerCreatedEvent(ComplianceOfficer complianceOfficer, string action)
        {
            ActionOnMessageReceived = action;
            
            EntityNameType = "ComplianceOfficer";
            
            Content = System.Text.Json.JsonSerializer.Serialize(complianceOfficer, JsonSerializerSettingsSingleton.Instance);
            
            EventType = "ComplianceOfficerCreated";
            
            EventId = UuidV7Generator.NewUuidV7();
            
            OccurredOnUtc = DateTime.UtcNow;
        }
    }
}

