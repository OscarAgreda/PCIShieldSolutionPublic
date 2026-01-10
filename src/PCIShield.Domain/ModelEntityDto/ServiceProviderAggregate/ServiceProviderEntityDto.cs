using System;
using System.Collections.Generic;
using System.Linq;
using PCIShieldLib.SharedKernel;
using PCIShieldLib.SharedKernel.Interfaces;
using Ardalis.GuardClauses;

namespace PCIShield.Domain.ModelEntityDto
{
    
    public class ServiceProviderEntityDto : IEntityDto
    {
        public Guid ServiceProviderId { get;  set; }
        
        public Guid TenantId { get;  set; }
        
        public string ProviderName { get;  set; }
        
        public string ServiceType { get;  set; }
        
        public bool IsPCICompliant { get;  set; }
        
        public DateTime? AOCExpiryDate { get;  set; }
        
        public string? ResponsibilityMatrix { get;  set; }
        
        public DateTime CreatedAt { get;  set; }
        
        public Guid CreatedBy { get;  set; }
        
        public DateTime? UpdatedAt { get;  set; }
        
        public Guid? UpdatedBy { get;  set; }
        
        public bool IsDeleted { get;  set; }
        
        public Guid MerchantId { get;  set; }
        
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
        
        public ServiceProviderEntityDto() {}
        
    }
}

