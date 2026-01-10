using System;
using PCIShield.Domain.Entities;
using PCIShieldLib.SharedKernel;
using System.Text.Json;

namespace PCIShield.Domain.Events
{
    public class ROCPackageCreatedEvent : BaseDomainEvent
    {
        public ROCPackageCreatedEvent(ROCPackage rocpackage, string action)
        {
            ActionOnMessageReceived = action;
            
            EntityNameType = "ROCPackage";
            
            Content = System.Text.Json.JsonSerializer.Serialize(rocpackage, JsonSerializerSettingsSingleton.Instance);
            
            EventType = "ROCPackageCreated";
            
            EventId = UuidV7Generator.NewUuidV7();
            
            OccurredOnUtc = DateTime.UtcNow;
        }
    }
}

