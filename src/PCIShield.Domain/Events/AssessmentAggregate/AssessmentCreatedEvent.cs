using System;
using PCIShield.Domain.Entities;
using PCIShieldLib.SharedKernel;
using System.Text.Json;

namespace PCIShield.Domain.Events
{
    public class AssessmentCreatedEvent : BaseDomainEvent
    {
        public AssessmentCreatedEvent(Assessment assessment, string action)
        {
            ActionOnMessageReceived = action;
            
            EntityNameType = "Assessment";
            
            Content = System.Text.Json.JsonSerializer.Serialize(assessment, JsonSerializerSettingsSingleton.Instance);
            
            EventType = "AssessmentCreated";
            
            EventId = UuidV7Generator.NewUuidV7();
            
            OccurredOnUtc = DateTime.UtcNow;
        }
    }
}

