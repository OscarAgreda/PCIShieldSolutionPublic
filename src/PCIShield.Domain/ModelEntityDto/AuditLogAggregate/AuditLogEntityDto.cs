using System;
using System.Collections.Generic;
using System.Linq;
using PCIShieldLib.SharedKernel;
using PCIShieldLib.SharedKernel.Interfaces;
using Ardalis.GuardClauses;

namespace PCIShield.Domain.ModelEntityDto
{
    
    public class AuditLogEntityDto : IEntityDto
    {
        public Guid AuditLogId { get;  set; }
        
        public Guid TenantId { get;  set; }
        
        public string EntityType { get;  set; }
        
        public Guid EntityId { get;  set; }
        
        public string Action { get;  set; }
        
        public string? OldValues { get;  set; }
        
        public string? NewValues { get;  set; }
        
        public Guid UserId { get;  set; }
        
        public string? IPAddress { get;  set; }
        
        public AuditLogEntityDto() {}
        
    }
}

