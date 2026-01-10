using System;
using PCIShield.Domain.Entities;
using PCIShieldLib.SharedKernel;
using System.Text.Json;

namespace PCIShield.Domain.Events
{
    public class AssessmentTypeCreatedEvent : BaseDomainEvent
    {
        public AssessmentTypeCreatedEvent(AssessmentType assessmentType, string action)
        {
            ActionOnMessageReceived = action;
            
            EntityNameType = "AssessmentType";
            
            Content = System.Text.Json.JsonSerializer.Serialize(assessmentType, JsonSerializerSettingsSingleton.Instance);
            
            EventType = "AssessmentTypeCreated";
            
            EventId = UuidV7Generator.NewUuidV7();
            
            OccurredOnUtc = DateTime.UtcNow;
        }
    }
}

