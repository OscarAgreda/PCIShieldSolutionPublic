using System;
using PCIShield.Domain.Entities;
using PCIShieldLib.SharedKernel;
using System.Text.Json;

namespace PCIShield.Domain.Events
{
    public class AssetUpdatedEvent : BaseDomainEvent
    {
        public AssetUpdatedEvent(Asset asset, string action)
        {
            ActionOnMessageReceived = action;
            
            EntityNameType = "Asset";
            
            Content = System.Text.Json.JsonSerializer.Serialize(asset, JsonSerializerSettingsSingleton.Instance);
            
            EventType = "AssetUpdated";
            
            EventId = UuidV7Generator.NewUuidV7();
            
            OccurredOnUtc = DateTime.UtcNow;
        }
    }
}

