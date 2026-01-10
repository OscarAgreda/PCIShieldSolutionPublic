using System;
using PCIShield.Domain.Entities;
using PCIShieldLib.SharedKernel;
using System.Text.Json;

namespace PCIShield.Domain.Events
{
    public class NetworkSegmentationUpdatedEvent : BaseDomainEvent
    {
        public NetworkSegmentationUpdatedEvent(NetworkSegmentation networkSegmentation, string action)
        {
            ActionOnMessageReceived = action;
            
            EntityNameType = "NetworkSegmentation";
            
            Content = System.Text.Json.JsonSerializer.Serialize(networkSegmentation, JsonSerializerSettingsSingleton.Instance);
            
            EventType = "NetworkSegmentationUpdated";
            
            EventId = UuidV7Generator.NewUuidV7();
            
            OccurredOnUtc = DateTime.UtcNow;
        }
    }
}

