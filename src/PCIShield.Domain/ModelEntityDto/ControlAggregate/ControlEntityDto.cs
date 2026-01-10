using System;
using System.Collections.Generic;
using System.Linq;
using PCIShieldLib.SharedKernel;
using PCIShieldLib.SharedKernel.Interfaces;
using Ardalis.GuardClauses;

namespace PCIShield.Domain.ModelEntityDto
{
    
    public class ControlEntityDto : IEntityDto
    {
        public Guid ControlId { get;  set; }
        
        public Guid TenantId { get;  set; }
        
        public string ControlCode { get;  set; }
        
        public string RequirementNumber { get;  set; }
        
        public string ControlTitle { get;  set; }
        
        public string ControlDescription { get;  set; }
        
        public string? TestingGuidance { get;  set; }
        
        public int FrequencyDays { get;  set; }
        
        public bool IsMandatory { get;  set; }
        
        public DateTime EffectiveDate { get;  set; }
        
        public DateTime CreatedAt { get;  set; }
        
        public Guid CreatedBy { get;  set; }
        
        public DateTime? UpdatedAt { get;  set; }
        
        public Guid? UpdatedBy { get;  set; }
        
        public bool IsDeleted { get;  set; }
        
        public List<CompensatingControlEntityDto> CompensatingControls { get; set; } = new();
        public List<AssessmentControlEntityDto> AssessmentControls { get; set; } = new();
        public List<AssetControlEntityDto> AssetControls { get; set; } = new();
        public List<ControlEvidenceEntityDto> ControlEvidences { get; set; } = new();
        public int ActiveAssessmentControlCount { get; set; }
        public int ActiveAssetControlCount { get; set; }
        public int ActiveCompensatingControlCount { get; set; }
        public int ActiveControlEvidenceCount { get; set; }
        public decimal AssessmentComplianceScore { get; set; }
        public decimal AverageAssessmentComplianceScore { get; set; }
        public decimal AverageMerchantAnnualCardVolume { get; set; }
        public int CriticalCompensatingControlCount { get; set; }
        public int DaysSinceEffectiveDate { get; set; }
        public bool HasAssessmentComplianceScoreBelowThreshold { get; set; }
        public bool IsMandatoryFlag { get; set; }
        public DateTime? LatestAssessmentControlTestDate { get; set; }
        public DateTime? LatestAssetControlUpdatedAt { get; set; }
        public DateTime? LatestCompensatingControlApprovalDate { get; set; }
        public DateTime? LatestControlEvidenceUpdatedAt { get; set; }
        public int TotalAssessmentControlCount { get; set; }
        public int TotalAssetControlCount { get; set; }
        public int TotalCompensatingControlCount { get; set; }
        public int TotalControlEvidenceCount { get; set; }
        public List<MerchantEntityDto> Merchants { get; set; } = new();
        
        public ControlEntityDto() {}
        
    }
}

