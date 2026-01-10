using System;
using System.Collections.Generic;
using System.Linq;
using PCIShieldLib.SharedKernel;
using PCIShieldLib.SharedKernel.Interfaces;
using Ardalis.GuardClauses;

namespace PCIShield.Domain.ModelEntityDto
{
    
    public class ComplianceOfficerEntityDto : IEntityDto
    {
        public Guid ComplianceOfficerId { get;  set; }
        
        public Guid TenantId { get;  set; }
        
        public string OfficerCode { get;  set; }
        
        public string FirstName { get;  set; }
        
        public string LastName { get;  set; }
        
        public string Email { get;  set; }
        
        public string? Phone { get;  set; }
        
        public string? CertificationLevel { get;  set; }
        
        public bool IsActive { get;  set; }
        
        public DateTime CreatedAt { get;  set; }
        
        public Guid CreatedBy { get;  set; }
        
        public DateTime? UpdatedAt { get;  set; }
        
        public Guid? UpdatedBy { get;  set; }
        
        public bool IsDeleted { get;  set; }
        
        public Guid MerchantId { get;  set; }
        
        public MerchantEntityDto Merchant { get; set; } = new();
        public int DaysSinceUpdatedAt { get; set; }
        public bool IsActiveFlag { get; set; }
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
        
        public ComplianceOfficerEntityDto() {}
        
    }
}

