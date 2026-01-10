using System;
using PCIShield.Domain.Entities;
using PCIShieldLib.SharedKernel;
using System.Text.Json;

namespace PCIShield.Domain.Events
{
    public class AssessmentControlUpdatedEvent : BaseDomainEvent
    {
        public AssessmentControlUpdatedEvent(AssessmentControl assessmentControl, string action)
        {
            ActionOnMessageReceived = action;
            
            EntityNameType = "AssessmentControl";
            
            Content = System.Text.Json.JsonSerializer.Serialize(assessmentControl, JsonSerializerSettingsSingleton.Instance);
            
            EventType = "AssessmentControlUpdated";
            
            EventId = UuidV7Generator.NewUuidV7();
            
            OccurredOnUtc = DateTime.UtcNow;
        }
    }
}

