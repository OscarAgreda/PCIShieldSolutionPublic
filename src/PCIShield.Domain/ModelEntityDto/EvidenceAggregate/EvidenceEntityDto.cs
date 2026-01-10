using System;
using System.Collections.Generic;
using System.Linq;
using PCIShieldLib.SharedKernel;
using PCIShieldLib.SharedKernel.Interfaces;
using Ardalis.GuardClauses;

namespace PCIShield.Domain.ModelEntityDto
{
    
    public class EvidenceEntityDto : IEntityDto
    {
        public Guid EvidenceId { get;  set; }
        
        public Guid TenantId { get;  set; }
        
        public string EvidenceCode { get;  set; }
        
        public string EvidenceTitle { get;  set; }
        
        public int EvidenceType { get;  set; }
        
        public DateTime CollectedDate { get;  set; }
        
        public string? FileHash { get;  set; }
        
        public string? StorageUri { get;  set; }
        
        public bool IsValid { get;  set; }
        
        public DateTime CreatedAt { get;  set; }
        
        public Guid CreatedBy { get;  set; }
        
        public DateTime? UpdatedAt { get;  set; }
        
        public Guid? UpdatedBy { get;  set; }
        
        public bool IsDeleted { get;  set; }
        
        public Guid MerchantId { get;  set; }
        
        public MerchantEntityDto Merchant { get; set; } = new();
        public List<ControlEvidenceEntityDto> ControlEvidences { get; set; } = new();
        public int ActiveControlEvidenceCount { get; set; }
        public int DaysSinceCollectedDate { get; set; }
        public bool IsEvidenceTypeCritical { get; set; }
        public bool IsValidFlag { get; set; }
        public DateTime? LatestControlEvidenceUpdatedAt { get; set; }
        public int TotalControlEvidenceCount { get; set; }
        public Guid ScanScheduleId { get; set; }
        public ScanScheduleEntityDto? ScanSchedule { get; set; }
        public Guid VulnerabilityId { get; set; }
        public VulnerabilityEntityDto? Vulnerability { get; set; }
        public Guid PaymentPageId { get; set; }
        public PaymentPageEntityDto? PaymentPage { get; set; }
        public Guid ScriptId { get; set; }
        public ScriptEntityDto? Script { get; set; }
        
        public EvidenceEntityDto() {}
        
    }
}

