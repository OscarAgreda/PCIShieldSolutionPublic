using System;
using PCIShield.Domain.Entities;
using PCIShieldLib.SharedKernel;
using System.Text.Json;

namespace PCIShield.Domain.Events
{
    public class AuditLogUpdatedEvent : BaseDomainEvent
    {
        public AuditLogUpdatedEvent(AuditLog auditLog, string action)
        {
            ActionOnMessageReceived = action;
            
            EntityNameType = "AuditLog";
            
            Content = System.Text.Json.JsonSerializer.Serialize(auditLog, JsonSerializerSettingsSingleton.Instance);
            
            EventType = "AuditLogUpdated";
            
            EventId = UuidV7Generator.NewUuidV7();
            
            OccurredOnUtc = DateTime.UtcNow;
        }
    }
}

