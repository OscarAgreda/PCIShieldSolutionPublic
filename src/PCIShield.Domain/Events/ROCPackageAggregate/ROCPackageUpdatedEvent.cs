using System;
using PCIShield.Domain.Entities;
using PCIShieldLib.SharedKernel;
using System.Text.Json;

namespace PCIShield.Domain.Events
{
    public class ROCPackageUpdatedEvent : BaseDomainEvent
    {
        public ROCPackageUpdatedEvent(ROCPackage rocpackage, string action)
        {
            ActionOnMessageReceived = action;
            
            EntityNameType = "ROCPackage";
            
            Content = System.Text.Json.JsonSerializer.Serialize(rocpackage, JsonSerializerSettingsSingleton.Instance);
            
            EventType = "ROCPackageUpdated";
            
            EventId = UuidV7Generator.NewUuidV7();
            
            OccurredOnUtc = DateTime.UtcNow;
        }
    }
}

