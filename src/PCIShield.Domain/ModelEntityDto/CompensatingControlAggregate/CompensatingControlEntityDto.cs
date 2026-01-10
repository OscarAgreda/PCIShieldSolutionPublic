using System;
using System.Collections.Generic;
using System.Linq;
using PCIShieldLib.SharedKernel;
using PCIShieldLib.SharedKernel.Interfaces;
using Ardalis.GuardClauses;

namespace PCIShield.Domain.ModelEntityDto
{
    
    public class CompensatingControlEntityDto : IEntityDto
    {
        public Guid CompensatingControlId { get;  set; }
        
        public Guid TenantId { get;  set; }
        
        public string Justification { get;  set; }
        
        public string ImplementationDetails { get;  set; }
        
        public Guid? ApprovedBy { get;  set; }
        
        public DateTime? ApprovalDate { get;  set; }
        
        public DateTime ExpiryDate { get;  set; }
        
        public int Rank { get;  set; }
        
        public DateTime CreatedAt { get;  set; }
        
        public Guid CreatedBy { get;  set; }
        
        public DateTime? UpdatedAt { get;  set; }
        
        public Guid? UpdatedBy { get;  set; }
        
        public bool IsDeleted { get;  set; }
        
        public Guid ControlId { get;  set; }
        
        public Guid MerchantId { get;  set; }
        
        public ControlEntityDto Control { get; set; } = new();
        public MerchantEntityDto Merchant { get; set; } = new();
        public Guid ROCPackageId { get; set; }
        public ROCPackageEntityDto? ROCPackage { get; set; }
        public Guid ScanScheduleId { get; set; }
        public ScanScheduleEntityDto? ScanSchedule { get; set; }
        public Guid VulnerabilityId { get; set; }
        public VulnerabilityEntityDto? Vulnerability { get; set; }
        public Guid PaymentPageId { get; set; }
        public PaymentPageEntityDto? PaymentPage { get; set; }
        public Guid ScriptId { get; set; }
        public ScriptEntityDto? Script { get; set; }
        
        public CompensatingControlEntityDto() {}
        
    }
}

