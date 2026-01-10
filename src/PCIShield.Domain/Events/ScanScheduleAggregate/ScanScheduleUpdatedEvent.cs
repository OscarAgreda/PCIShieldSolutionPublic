using System;
using PCIShield.Domain.Entities;
using PCIShieldLib.SharedKernel;
using System.Text.Json;

namespace PCIShield.Domain.Events
{
    public class ScanScheduleUpdatedEvent : BaseDomainEvent
    {
        public ScanScheduleUpdatedEvent(ScanSchedule scanSchedule, string action)
        {
            ActionOnMessageReceived = action;
            
            EntityNameType = "ScanSchedule";
            
            Content = System.Text.Json.JsonSerializer.Serialize(scanSchedule, JsonSerializerSettingsSingleton.Instance);
            
            EventType = "ScanScheduleUpdated";
            
            EventId = UuidV7Generator.NewUuidV7();
            
            OccurredOnUtc = DateTime.UtcNow;
        }
    }
}

