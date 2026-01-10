using System;
using PCIShield.Domain.Entities;
using PCIShieldLib.SharedKernel;
using System.Text.Json;

namespace PCIShield.Domain.Events
{
    public class AssessmentDeletedEvent : BaseDomainEvent
    {
        public AssessmentDeletedEvent(Assessment assessment, string action)
        {
            ActionOnMessageReceived = action;
            
            EntityNameType = "Assessment";
            
            Content = System.Text.Json.JsonSerializer.Serialize(assessment, JsonSerializerSettingsSingleton.Instance);
            
            EventType = "AssessmentDeleted";
            
            EventId = UuidV7Generator.NewUuidV7();
            
            OccurredOnUtc = DateTime.UtcNow;
        }
    }
}

