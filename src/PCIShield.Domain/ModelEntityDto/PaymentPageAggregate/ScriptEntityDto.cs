using System;
using System.Collections.Generic;
using System.Linq;
using PCIShieldLib.SharedKernel;
using PCIShieldLib.SharedKernel.Interfaces;
using Ardalis.GuardClauses;

namespace PCIShield.Domain.ModelEntityDto
{
    
    public class ScriptEntityDto : IEntityDto
    {
        public Guid ScriptId { get;  set; }
        
        public Guid TenantId { get;  set; }
        
        public string ScriptUrl { get;  set; }
        
        public string ScriptHash { get;  set; }
        
        public string ScriptType { get;  set; }
        
        public bool IsAuthorized { get;  set; }
        
        public DateTime FirstSeen { get;  set; }
        
        public DateTime LastSeen { get;  set; }
        
        public DateTime CreatedAt { get;  set; }
        
        public Guid CreatedBy { get;  set; }
        
        public DateTime? UpdatedAt { get;  set; }
        
        public Guid? UpdatedBy { get;  set; }
        
        public bool IsDeleted { get;  set; }
        
        public Guid PaymentPageId { get;  set; }
        
        public PaymentPageEntityDto PaymentPage { get; set; } = new();
        public ScriptEntityDto() {}
        
    }
}

