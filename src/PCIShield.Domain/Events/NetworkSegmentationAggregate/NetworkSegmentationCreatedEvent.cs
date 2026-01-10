using System;
using PCIShield.Domain.Entities;
using PCIShieldLib.SharedKernel;
using System.Text.Json;

namespace PCIShield.Domain.Events
{
    public class NetworkSegmentationCreatedEvent : BaseDomainEvent
    {
        public NetworkSegmentationCreatedEvent(NetworkSegmentation networkSegmentation, string action)
        {
            ActionOnMessageReceived = action;
            
            EntityNameType = "NetworkSegmentation";
            
            Content = System.Text.Json.JsonSerializer.Serialize(networkSegmentation, JsonSerializerSettingsSingleton.Instance);
            
            EventType = "NetworkSegmentationCreated";
            
            EventId = UuidV7Generator.NewUuidV7();
            
            OccurredOnUtc = DateTime.UtcNow;
        }
    }
}

