using System;
using System.Collections.Generic;

using PCIShieldLib.SharedKernel.Interfaces;

namespace PCIShield.Domain.ModelEntityDto
{
    public class MerchantEntityDto : IEntityDto
    {
        public Guid MerchantId { get; set; }

        public Guid TenantId { get; set; }

        public string MerchantCode { get; set; }

        public string MerchantName { get; set; }

        public int MerchantLevel { get; set; }

        public string AcquirerName { get; set; }

        public string ProcessorMID { get; set; }

        public decimal AnnualCardVolume { get; set; }

        public DateTime? LastAssessmentDate { get; set; }

        public DateTime NextAssessmentDue { get; set; }

        public int ComplianceRank { get; set; }

        public DateTime CreatedAt { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public Guid? UpdatedBy { get; set; }

        public bool IsDeleted { get; set; }

        public List<AssessmentEntityDto> Assessments { get; set; } = new();
        public List<AssetEntityDto> Assets { get; set; } = new();
        public List<CompensatingControlEntityDto> CompensatingControls { get; set; } = new();
        public List<ComplianceOfficerEntityDto> ComplianceOfficers { get; set; } = new();
        public List<CryptographicInventoryEntityDto> CryptographicInventories { get; set; } = new();
        public List<EvidenceEntityDto> Evidences { get; set; } = new();
        public List<NetworkSegmentationEntityDto> NetworkSegmentations { get; set; } = new();
        public List<PaymentChannelEntityDto> PaymentChannels { get; set; } = new();
        public List<ServiceProviderEntityDto> ServiceProviders { get; set; } = new();
        public int ActiveAssessmentCount { get; set; }
        public int ActiveAssetCount { get; set; }
        public int ActiveCompensatingControlCount { get; set; }
        public int ActiveComplianceOfficerCount { get; set; }
        public int ActiveCryptographicInventoryCount { get; set; }
        public int ActiveEvidenceCount { get; set; }
        public int ActiveNetworkSegmentationCount { get; set; }
        public int ActivePaymentChannelCount { get; set; }
        public int ActiveServiceProviderCount { get; set; }
        public int AffectedEntitiesCount { get; set; }
        public int ArticulationRank { get; set; }
        public decimal AssessmentComplianceScore { get; set; }
        public bool AssessmentInfoExceedsRiskThreshold { get; set; }
        public double AuthorityScore { get; set; }
        public decimal AverageAssessmentComplianceScore { get; set; }
        public int AverageControlFrequencyDays { get; set; }
        public int AverageVulnerabilitySeverity { get; set; }
        public decimal CachedAssessmentAssessmentTypeSum { get; set; }
        public int CachedAssessmentCount { get; set; }
        public decimal CachedAssetAssetTypeSum { get; set; }
        public int CachedAssetCount { get; set; }
        public int CachedCompensatingControlCount { get; set; }
        public decimal CachedCompensatingControlRankSum { get; set; }
        public int CachedCryptographicInventoryCount { get; set; }
        public decimal CachedCryptographicInventoryKeyLengthSum { get; set; }
        public int CachedEvidenceCount { get; set; }
        public decimal CachedEvidenceEvidenceTypeSum { get; set; }
        public int CascadeDepth { get; set; }
        public double CascadeImpactScore { get; set; }
        public int ControlFrequencyDays { get; set; }
        public int CriticalAssessmentCount { get; set; }
        public int CriticalAssetCount { get; set; }
        public int CriticalCompensatingControlCount { get; set; }
        public int CriticalEvidenceCount { get; set; }
        public int CriticalityRank { get; set; }
        public int CriticalPaymentChannelCount { get; set; }
        public int CryptographicInventoryKeyLength { get; set; }
        public double DataStabilityScore { get; set; }
        public int DaysSinceLastAssessment { get; set; }
        public int DaysSinceLastAssessmentDate { get; set; }
        public double DeletionImpactScore { get; set; }
        public int DrillDownPathCount { get; set; }
        public double EntityInfluenceRank { get; set; }
        public double EntityInfluenceScore { get; set; }
        public double EvolutionPressureScore { get; set; }
        public bool HasAssessmentComplianceScoreBelowThreshold { get; set; }
        public bool HasComplexDrillDowns { get; set; }
        public bool HasControlFrequencyDaysOverdue { get; set; }
        public bool HasRepeatingChildPatternBundle { get; set; }
        public bool HasUpdatedBy { get; set; }
        public double HubScore { get; set; }
        public bool IsArticulationPoint { get; set; }
        public bool IsDeletedFlag { get; set; }
        public bool IsHighComplexityEntity { get; set; }
        public bool IsHubNode { get; set; }
        public bool IsMerchantLevelCritical { get; set; }
        public bool IsSystemCriticalNode { get; set; }
        public bool IsVolatileEntity { get; set; }
        public DateTime? LatestAssessmentStartDate { get; set; }
        public DateTime? LatestAssetLastScanDate { get; set; }
        public DateTime? LatestCompensatingControlApprovalDate { get; set; }
        public DateTime? LatestComplianceOfficerUpdatedAt { get; set; }
        public DateTime? LatestCryptographicInventoryCreationDate { get; set; }
        public DateTime? LatestEvidenceCollectedDate { get; set; }
        public DateTime? LatestNetworkSegmentationLastValidated { get; set; }
        public DateTime? LatestPaymentChannelUpdatedAt { get; set; }
        public DateTime? LatestServiceProviderAOCExpiryDate { get; set; }
        public int MaxCryptographicInventoryKeyLength { get; set; }
        public decimal MaxVulnerabilityCVSS { get; set; }
        public double NetworkCentrality { get; set; }
        public int NodeDegree { get; set; }
        public decimal PaymentChannelProcessingVolume { get; set; }
        public int RelationshipCommunities { get; set; }
        public double RepeatingChildPatternCompleteness { get; set; }
        public bool RequiresDeletionPreview { get; set; }
        public bool RequiresSafetyChecks { get; set; }
        public int TotalAssessmentCount { get; set; }
        public int TotalAssetCount { get; set; }
        public int TotalCompensatingControlCount { get; set; }
        public int TotalComplianceOfficerCount { get; set; }
        public int TotalCryptographicInventoryCount { get; set; }
        public int TotalEvidenceCount { get; set; }
        public int TotalNetworkSegmentationCount { get; set; }
        public int TotalPaymentChannelCount { get; set; }
        public decimal TotalPaymentChannelProcessingVolume { get; set; }
        public int TotalServiceProviderCount { get; set; }
        public decimal VulnerabilityCVSS { get; set; }
        public int VulnerabilitySeverity { get; set; }
        public bool VulnerabilityInfoExceedsRiskThreshold { get; set; }
        public AssessmentControlEntityDto? AssessmentControl { get; set; }
        public List<AssetControlEntityDto> AssetControls { get; set; } = new();
        public List<ControlEntityDto> Controls { get; set; } = new();
        public ControlEvidenceEntityDto? ControlEvidence { get; set; }
        public PaymentPageEntityDto? PaymentPage { get; set; }
        public List<ROCPackageEntityDto> ROCPackages { get; set; } = new();
        public List<ScanScheduleEntityDto> ScanSchedules { get; set; } = new();
        public List<VulnerabilityEntityDto> Vulnerabilities { get; set; } = new();
        public decimal AnnualCardVolumeWithTax { get; set; }
        public string AnnualCardVolumeFormatted { get; set; }
        public int DaysSinceNextAssessmentDue { get; set; }
        public int DaysSinceCreatedAt { get; set; }
        public object MerchantLevelPercentile { get; set; }
        public bool IsMerchantLevelAboveAverage { get; set; }
        public object ComplianceRankPercentile { get; set; }
        public int Summary_Assessment_Count { get; internal set; }
        public int Summary_Asset_Count { get; internal set; }
        public int Summary_PaymentChannel_Count { get; internal set; }

        public MerchantEntityDto()
        { }
    }
}