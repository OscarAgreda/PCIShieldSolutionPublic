using System;
using System.Collections.Generic;
using System.Linq;
using PCIShieldLib.SharedKernel;
using PCIShieldLib.SharedKernel.Interfaces;
using Ardalis.GuardClauses;

namespace PCIShield.Domain.ModelEntityDto
{
    
    public class AssessmentEntityDto : IEntityDto
    {
        public Guid AssessmentId { get;  set; }
        
        public Guid TenantId { get;  set; }
        
        public string AssessmentCode { get;  set; }
        
        public int AssessmentType { get;  set; }
        
        public string AssessmentPeriod { get;  set; }
        
        public DateTime StartDate { get;  set; }
        
        public DateTime EndDate { get;  set; }
        
        public DateTime? CompletionDate { get;  set; }
        
        public int Rank { get;  set; }
        
        public decimal? ComplianceScore { get;  set; }
        
        public bool QSAReviewRequired { get;  set; }
        
        public DateTime CreatedAt { get;  set; }
        
        public Guid CreatedBy { get;  set; }
        
        public DateTime? UpdatedAt { get;  set; }
        
        public Guid? UpdatedBy { get;  set; }
        
        public bool IsDeleted { get;  set; }
        
        public Guid MerchantId { get;  set; }
        
        public List<ROCPackageEntityDto> ROCPackages { get; set; } = new();
        public MerchantEntityDto Merchant { get; set; } = new();
        public List<AssessmentControlEntityDto> AssessmentControls { get; set; } = new();
        public List<ControlEvidenceEntityDto> ControlEvidences { get; set; } = new();
        public int ActiveAssessmentControlCount { get; set; }
        public int ActiveControlEvidenceCount { get; set; }
        public int ActiveROCPackageCount { get; set; }
        public int AverageControlFrequencyDays { get; set; }
        public int ControlFrequencyDays { get; set; }
        public int CriticalROCPackageCount { get; set; }
        public int DaysSinceStartDate { get; set; }
        public bool HasControlFrequencyDaysOverdue { get; set; }
        public bool IsAssessmentTypeCritical { get; set; }
        public bool IsDeletedFlag { get; set; }
        public bool IsRankCritical { get; set; }
        public DateTime? LatestAssessmentControlTestDate { get; set; }
        public DateTime? LatestControlEvidenceUpdatedAt { get; set; }
        public DateTime? LatestROCPackageGeneratedDate { get; set; }
        public int TotalAssessmentControlCount { get; set; }
        public int TotalControlEvidenceCount { get; set; }
        public int TotalROCPackageCount { get; set; }
        public Guid ScanScheduleId { get; set; }
        public ScanScheduleEntityDto? ScanSchedule { get; set; }
        public Guid VulnerabilityId { get; set; }
        public VulnerabilityEntityDto? Vulnerability { get; set; }
        public Guid PaymentPageId { get; set; }
        public PaymentPageEntityDto? PaymentPage { get; set; }
        public Guid ScriptId { get; set; }
        public ScriptEntityDto? Script { get; set; }
        
        public AssessmentEntityDto() {}
        
    }
}

