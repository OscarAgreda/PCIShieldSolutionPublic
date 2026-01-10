using System;
using System.Collections.Generic;
using System.Linq;
using PCIShieldLib.SharedKernel;
using PCIShieldLib.SharedKernel.Interfaces;
using Ardalis.GuardClauses;

namespace PCIShield.Domain.ModelEntityDto
{
    
    public class PaymentPageEntityDto : IEntityDto
    {
        public Guid PaymentPageId { get;  set; }
        
        public Guid TenantId { get;  set; }
        
        public string PageUrl { get;  set; }
        
        public string PageName { get;  set; }
        
        public bool IsActive { get;  set; }
        
        public DateTime? LastScriptInventory { get;  set; }
        
        public string? ScriptIntegrityHash { get;  set; }
        
        public DateTime CreatedAt { get;  set; }
        
        public Guid CreatedBy { get;  set; }
        
        public DateTime? UpdatedAt { get;  set; }
        
        public Guid? UpdatedBy { get;  set; }
        
        public bool IsDeleted { get;  set; }
        
        public Guid PaymentChannelId { get;  set; }
        
        public List<ScriptEntityDto> Scripts { get; set; } = new();
        public PaymentChannelEntityDto PaymentChannel { get; set; } = new();
        public PaymentPageEntityDto() {}
        
    }
}

