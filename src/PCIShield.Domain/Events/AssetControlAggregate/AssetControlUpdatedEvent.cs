using System;
using PCIShield.Domain.Entities;
using PCIShieldLib.SharedKernel;
using System.Text.Json;

namespace PCIShield.Domain.Events
{
    public class AssetControlUpdatedEvent : BaseDomainEvent
    {
        public AssetControlUpdatedEvent(AssetControl assetControl, string action)
        {
            ActionOnMessageReceived = action;
            
            EntityNameType = "AssetControl";
            
            Content = System.Text.Json.JsonSerializer.Serialize(assetControl, JsonSerializerSettingsSingleton.Instance);
            
            EventType = "AssetControlUpdated";
            
            EventId = UuidV7Generator.NewUuidV7();
            
            OccurredOnUtc = DateTime.UtcNow;
        }
    }
}

