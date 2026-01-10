using System;
using PCIShield.Domain.Entities;
using PCIShieldLib.SharedKernel;
using System.Text.Json;

namespace PCIShield.Domain.Events
{
    public class AssessmentTypeUpdatedEvent : BaseDomainEvent
    {
        public AssessmentTypeUpdatedEvent(AssessmentType assessmentType, string action)
        {
            ActionOnMessageReceived = action;
            
            EntityNameType = "AssessmentType";
            
            Content = System.Text.Json.JsonSerializer.Serialize(assessmentType, JsonSerializerSettingsSingleton.Instance);
            
            EventType = "AssessmentTypeUpdated";
            
            EventId = UuidV7Generator.NewUuidV7();
            
            OccurredOnUtc = DateTime.UtcNow;
        }
    }
}

