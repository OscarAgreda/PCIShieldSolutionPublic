using System;
using PCIShield.Domain.Entities;
using PCIShieldLib.SharedKernel;
using System.Text.Json;

namespace PCIShield.Domain.Events
{
    public class CryptographicInventoryUpdatedEvent : BaseDomainEvent
    {
        public CryptographicInventoryUpdatedEvent(CryptographicInventory cryptographicInventory, string action)
        {
            ActionOnMessageReceived = action;
            
            EntityNameType = "CryptographicInventory";
            
            Content = System.Text.Json.JsonSerializer.Serialize(cryptographicInventory, JsonSerializerSettingsSingleton.Instance);
            
            EventType = "CryptographicInventoryUpdated";
            
            EventId = UuidV7Generator.NewUuidV7();
            
            OccurredOnUtc = DateTime.UtcNow;
        }
    }
}

