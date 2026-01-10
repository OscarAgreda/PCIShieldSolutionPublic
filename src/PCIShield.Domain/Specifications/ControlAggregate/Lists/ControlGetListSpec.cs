using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using PCIShieldLib.SharedKernel.Interfaces;
using Ardalis.Specification;
using Ardalis.GuardClauses;
using PCIShield.Domain.Entities;
using PCIShield.Domain.ModelEntityDto;

namespace PCIShield.Domain.Specifications
{
    public sealed class ControlListPagedSpec : PagedSpecification<Control, ControlEntityDto>
    {
        public ControlListPagedSpec(int pageNumber, int pageSize)
        {
            _ = Guard.Against.NegativeOrZero(pageNumber, nameof(pageNumber));
            _ = Guard.Against.NegativeOrZero(pageSize, nameof(pageSize));

            _ = Query
                .OrderByDescending(i => i.ControlId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            _ = Query.Select(x => new ControlEntityDto
            {
                ControlId = x.ControlId,
                TenantId = x.TenantId,
                ControlCode = x.ControlCode,
                RequirementNumber = x.RequirementNumber,
                ControlTitle = x.ControlTitle,
                ControlDescription = x.ControlDescription,
                TestingGuidance = x.TestingGuidance,
                FrequencyDays = x.FrequencyDays,
                IsMandatory = x.IsMandatory,
                EffectiveDate = x.EffectiveDate,
                CreatedAt = x.CreatedAt,
                CreatedBy = x.CreatedBy,
                UpdatedAt = x.UpdatedAt,
                UpdatedBy = x.UpdatedBy,
                IsDeleted = x.IsDeleted,
            })
            .AsNoTracking()
            .AsSplitQuery()
            .EnableCache($"ControlListPagedSpec-{pageNumber}-{pageSize}");
        }
    }
    public sealed class ControlSearchSpec : Specification<Control>
    {
        public ControlSearchSpec(string searchTerm)
        {
            string searchLower = searchTerm?.ToLower() ?? string.Empty;

            Query
                .Where(c =>
                        (c.ControlCode != null && c.ControlCode.ToLower().Contains(searchLower)) ||
                        (c.RequirementNumber != null && c.RequirementNumber.ToLower().Contains(searchLower))                )
                .OrderByDescending(c => c.ControlId);
        }
    }

    public sealed class ControlLastCreatedSpec : Specification<Control>
    {
        public ControlLastCreatedSpec()
        {
            Query
                .OrderByDescending(c => c.EffectiveDate)
                .Take(1)
                .AsNoTracking()
                .EnableCache("ControlLastCreatedSpec");
        }
    }
    public sealed class ControlByIdSpec : Specification<Control, ControlEntityDto>
    {
        public ControlByIdSpec(Guid id)
        {
            _ = Guard.Against.NullOrEmpty(id, nameof(id));

            _ = Query.Where(x => x.ControlId == id);

            _ = Query.Include(x => x.CompensatingControls);
            _ = Query.Include(x => x.AssessmentControls);
            _ = Query.Include(x => x.AssetControls);
            _ = Query.Include(x => x.ControlEvidences);
            _ = Query.Select(x => new ControlEntityDto
            {
                ControlId = x.ControlId,
                TenantId = x.TenantId,
                ControlCode = x.ControlCode,
                RequirementNumber = x.RequirementNumber,
                ControlTitle = x.ControlTitle,
                ControlDescription = x.ControlDescription,
                TestingGuidance = x.TestingGuidance,
                FrequencyDays = x.FrequencyDays,
                IsMandatory = x.IsMandatory,
                EffectiveDate = x.EffectiveDate,
                CreatedAt = x.CreatedAt,
                CreatedBy = x.CreatedBy,
                UpdatedAt = x.UpdatedAt,
                UpdatedBy = x.UpdatedBy,
                IsDeleted = x.IsDeleted,

                CompensatingControls = x.CompensatingControls.Select(compensatingControl => new CompensatingControlEntityDto
                {
                    CompensatingControlId = compensatingControl.CompensatingControlId,
                    TenantId = compensatingControl.TenantId,
                    ControlId = compensatingControl.ControlId,
                    MerchantId = compensatingControl.MerchantId,
                    Justification = compensatingControl.Justification,
                    ImplementationDetails = compensatingControl.ImplementationDetails,
                    ApprovedBy = compensatingControl.ApprovedBy,
                    ApprovalDate = compensatingControl.ApprovalDate,
                    ExpiryDate = compensatingControl.ExpiryDate,
                    Rank = compensatingControl.Rank,
                    CreatedAt = compensatingControl.CreatedAt,
                    CreatedBy = compensatingControl.CreatedBy,
                    UpdatedAt = compensatingControl.UpdatedAt,
                    UpdatedBy = compensatingControl.UpdatedBy,
                    IsDeleted = compensatingControl.IsDeleted,
                    Merchant = compensatingControl.Merchant != null ? new MerchantEntityDto
                    {
                        MerchantId = compensatingControl.Merchant.MerchantId,
                        TenantId = compensatingControl.Merchant.TenantId,
                        MerchantCode = compensatingControl.Merchant.MerchantCode,
                        MerchantName = compensatingControl.Merchant.MerchantName,
                        MerchantLevel = compensatingControl.Merchant.MerchantLevel,
                        AcquirerName = compensatingControl.Merchant.AcquirerName,
                        ProcessorMID = compensatingControl.Merchant.ProcessorMID,
                        AnnualCardVolume = compensatingControl.Merchant.AnnualCardVolume,
                        LastAssessmentDate = compensatingControl.Merchant.LastAssessmentDate,
                        NextAssessmentDue = compensatingControl.Merchant.NextAssessmentDue,
                        ComplianceRank = compensatingControl.Merchant.ComplianceRank,
                        CreatedAt = compensatingControl.Merchant.CreatedAt,
                        CreatedBy = compensatingControl.Merchant.CreatedBy,
                        UpdatedAt = compensatingControl.Merchant.UpdatedAt,
                        UpdatedBy = compensatingControl.Merchant.UpdatedBy,
                        IsDeleted = compensatingControl.Merchant.IsDeleted,
                    } : null,
                }).ToList(),
                AssessmentControls = x.AssessmentControls.Select(assessmentControl => new AssessmentControlEntityDto
                {
                    RowId = assessmentControl.RowId,
                    AssessmentId = assessmentControl.AssessmentId,
                    ControlId = assessmentControl.ControlId,
                    TenantId = assessmentControl.TenantId,
                    TestResult = assessmentControl.TestResult,
                    TestDate = assessmentControl.TestDate,
                    TestedBy = assessmentControl.TestedBy,
                    Notes = assessmentControl.Notes,
                    CreatedAt = assessmentControl.CreatedAt,
                    CreatedBy = assessmentControl.CreatedBy,
                    UpdatedAt = assessmentControl.UpdatedAt,
                    UpdatedBy = assessmentControl.UpdatedBy,
                    IsDeleted = assessmentControl.IsDeleted,
                    Control = new ControlEntityDto
                    {
                        ControlId = assessmentControl.Control.ControlId,
                        TenantId = assessmentControl.Control.TenantId,
                        ControlCode = assessmentControl.Control.ControlCode,
                        RequirementNumber = assessmentControl.Control.RequirementNumber,
                        ControlTitle = assessmentControl.Control.ControlTitle,
                        ControlDescription = assessmentControl.Control.ControlDescription,
                        TestingGuidance = assessmentControl.Control.TestingGuidance,
                        FrequencyDays = assessmentControl.Control.FrequencyDays,
                        IsMandatory = assessmentControl.Control.IsMandatory,
                        EffectiveDate = assessmentControl.Control.EffectiveDate,
                        CreatedAt = assessmentControl.Control.CreatedAt,
                        CreatedBy = assessmentControl.Control.CreatedBy,
                        UpdatedAt = assessmentControl.Control.UpdatedAt,
                        UpdatedBy = assessmentControl.Control.UpdatedBy,
                        IsDeleted = assessmentControl.Control.IsDeleted,
                    },
                    Assessment = new AssessmentEntityDto
                    {
                        AssessmentId = assessmentControl.Assessment.AssessmentId,
                        TenantId = assessmentControl.Assessment.TenantId,
                        MerchantId = assessmentControl.Assessment.MerchantId,
                        AssessmentCode = assessmentControl.Assessment.AssessmentCode,
                        AssessmentType = assessmentControl.Assessment.AssessmentType,
                        AssessmentPeriod = assessmentControl.Assessment.AssessmentPeriod,
                        StartDate = assessmentControl.Assessment.StartDate,
                        EndDate = assessmentControl.Assessment.EndDate,
                        CompletionDate = assessmentControl.Assessment.CompletionDate,
                        Rank = assessmentControl.Assessment.Rank,
                        ComplianceScore = assessmentControl.Assessment.ComplianceScore,
                        QSAReviewRequired = assessmentControl.Assessment.QSAReviewRequired,
                        CreatedAt = assessmentControl.Assessment.CreatedAt,
                        CreatedBy = assessmentControl.Assessment.CreatedBy,
                        UpdatedAt = assessmentControl.Assessment.UpdatedAt,
                        UpdatedBy = assessmentControl.Assessment.UpdatedBy,
                        IsDeleted = assessmentControl.Assessment.IsDeleted,
                    },
                }).ToList(),
                AssetControls = x.AssetControls.Select(assetControl => new AssetControlEntityDto
                {
                    RowId = assetControl.RowId,
                    AssetId = assetControl.AssetId,
                    ControlId = assetControl.ControlId,
                    TenantId = assetControl.TenantId,
                    IsApplicable = assetControl.IsApplicable,
                    CustomizedApproach = assetControl.CustomizedApproach,
                    CreatedAt = assetControl.CreatedAt,
                    CreatedBy = assetControl.CreatedBy,
                    UpdatedAt = assetControl.UpdatedAt,
                    UpdatedBy = assetControl.UpdatedBy,
                    IsDeleted = assetControl.IsDeleted,
                    Control = new ControlEntityDto
                    {
                        ControlId = assetControl.Control.ControlId,
                        TenantId = assetControl.Control.TenantId,
                        ControlCode = assetControl.Control.ControlCode,
                        RequirementNumber = assetControl.Control.RequirementNumber,
                        ControlTitle = assetControl.Control.ControlTitle,
                        ControlDescription = assetControl.Control.ControlDescription,
                        TestingGuidance = assetControl.Control.TestingGuidance,
                        FrequencyDays = assetControl.Control.FrequencyDays,
                        IsMandatory = assetControl.Control.IsMandatory,
                        EffectiveDate = assetControl.Control.EffectiveDate,
                        CreatedAt = assetControl.Control.CreatedAt,
                        CreatedBy = assetControl.Control.CreatedBy,
                        UpdatedAt = assetControl.Control.UpdatedAt,
                        UpdatedBy = assetControl.Control.UpdatedBy,
                        IsDeleted = assetControl.Control.IsDeleted,
                    },
                    Asset = new AssetEntityDto
                    {
                        AssetId = assetControl.Asset.AssetId,
                        TenantId = assetControl.Asset.TenantId,
                        MerchantId = assetControl.Asset.MerchantId,
                        AssetCode = assetControl.Asset.AssetCode,
                        AssetName = assetControl.Asset.AssetName,
                        AssetType = assetControl.Asset.AssetType,
                        IPAddress = assetControl.Asset.IPAddress,
                        Hostname = assetControl.Asset.Hostname,
                        IsInCDE = assetControl.Asset.IsInCDE,
                        NetworkZone = assetControl.Asset.NetworkZone,
                        LastScanDate = assetControl.Asset.LastScanDate,
                        CreatedAt = assetControl.Asset.CreatedAt,
                        CreatedBy = assetControl.Asset.CreatedBy,
                        UpdatedAt = assetControl.Asset.UpdatedAt,
                        UpdatedBy = assetControl.Asset.UpdatedBy,
                        IsDeleted = assetControl.Asset.IsDeleted,
                    },
                }).ToList(),
                ControlEvidences = x.ControlEvidences.Select(controlEvidence => new ControlEvidenceEntityDto
                {
                    RowId = controlEvidence.RowId,
                    ControlId = controlEvidence.ControlId,
                    EvidenceId = controlEvidence.EvidenceId,
                    AssessmentId = controlEvidence.AssessmentId,
                    TenantId = controlEvidence.TenantId,
                    IsPrimary = controlEvidence.IsPrimary,
                    CreatedAt = controlEvidence.CreatedAt,
                    CreatedBy = controlEvidence.CreatedBy,
                    UpdatedAt = controlEvidence.UpdatedAt,
                    UpdatedBy = controlEvidence.UpdatedBy,
                    IsDeleted = controlEvidence.IsDeleted,
                    Control = new ControlEntityDto
                    {
                        ControlId = controlEvidence.Control.ControlId,
                        TenantId = controlEvidence.Control.TenantId,
                        ControlCode = controlEvidence.Control.ControlCode,
                        RequirementNumber = controlEvidence.Control.RequirementNumber,
                        ControlTitle = controlEvidence.Control.ControlTitle,
                        ControlDescription = controlEvidence.Control.ControlDescription,
                        TestingGuidance = controlEvidence.Control.TestingGuidance,
                        FrequencyDays = controlEvidence.Control.FrequencyDays,
                        IsMandatory = controlEvidence.Control.IsMandatory,
                        EffectiveDate = controlEvidence.Control.EffectiveDate,
                        CreatedAt = controlEvidence.Control.CreatedAt,
                        CreatedBy = controlEvidence.Control.CreatedBy,
                        UpdatedAt = controlEvidence.Control.UpdatedAt,
                        UpdatedBy = controlEvidence.Control.UpdatedBy,
                        IsDeleted = controlEvidence.Control.IsDeleted,
                    },
                    Evidence = new EvidenceEntityDto
                    {
                        EvidenceId = controlEvidence.Evidence.EvidenceId,
                        TenantId = controlEvidence.Evidence.TenantId,
                        MerchantId = controlEvidence.Evidence.MerchantId,
                        EvidenceCode = controlEvidence.Evidence.EvidenceCode,
                        EvidenceTitle = controlEvidence.Evidence.EvidenceTitle,
                        EvidenceType = controlEvidence.Evidence.EvidenceType,
                        CollectedDate = controlEvidence.Evidence.CollectedDate,
                        FileHash = controlEvidence.Evidence.FileHash,
                        StorageUri = controlEvidence.Evidence.StorageUri,
                        IsValid = controlEvidence.Evidence.IsValid,
                        CreatedAt = controlEvidence.Evidence.CreatedAt,
                        CreatedBy = controlEvidence.Evidence.CreatedBy,
                        UpdatedAt = controlEvidence.Evidence.UpdatedAt,
                        UpdatedBy = controlEvidence.Evidence.UpdatedBy,
                        IsDeleted = controlEvidence.Evidence.IsDeleted,
                    },
                }).ToList(),
            })
            .AsNoTracking()
            .AsSplitQuery()
            .EnableCache($"ControlByIdSpec-{id.ToString()}");
        }
    }
}

    public sealed class ControlAdvancedGraphSpecV4 : Specification<Control, ControlEntityDto>
    {
        public ControlAdvancedGraphSpecV4(
            Guid controlId,
            bool includeReferenceData = true,
            bool includeTransactionData = true,
            bool includeTransitiveData = true,
            int? take = null,
            int? skip = null,
            DateTime? effectiveDate = null)
        {
            Guard.Against.Default(controlId, nameof(controlId));
            Query.Where(c => c.ControlId == controlId);

            if (take.HasValue && skip.HasValue)
            {
            }

            if (includeReferenceData)
            {
            }

            if (includeTransactionData)
            {
            }

            if (includeTransitiveData)
            {

            }

            Query.Select(c => new ControlEntityDto
            {
                ControlId = c.ControlId,
                TenantId = c.TenantId,
                ControlCode = c.ControlCode,
                RequirementNumber = c.RequirementNumber,
                ControlTitle = c.ControlTitle,
                ControlDescription = c.ControlDescription,
                TestingGuidance = c.TestingGuidance,
                FrequencyDays = c.FrequencyDays,
                IsMandatory = c.IsMandatory,
                EffectiveDate = c.EffectiveDate,
                CreatedAt = c.CreatedAt,
                CreatedBy = c.CreatedBy,
                UpdatedAt = c.UpdatedAt,
                UpdatedBy = c.UpdatedBy,
                IsDeleted = c.IsDeleted,

                Merchants = c.CompensatingControls
                    .Select(i => i.Merchant)
                    .Select(t => new MerchantEntityDto
                    {
                        MerchantId = t.MerchantId,
                        TenantId = t.TenantId,
                        MerchantCode = t.MerchantCode,
                        MerchantName = t.MerchantName,
                        MerchantLevel = t.MerchantLevel,
                        AcquirerName = t.AcquirerName,
                        ProcessorMID = t.ProcessorMID,
                        AnnualCardVolume = t.AnnualCardVolume,
                        LastAssessmentDate = t.LastAssessmentDate,
                        NextAssessmentDue = t.NextAssessmentDue,
                        ComplianceRank = t.ComplianceRank,
                        IsDeleted = t.IsDeleted,
                    })
                    .Take(2).ToList(),
                CompensatingControls = c.CompensatingControls.Select(compensatingControl => new CompensatingControlEntityDto
                {
                    CompensatingControlId = compensatingControl.CompensatingControlId,
                    TenantId = compensatingControl.TenantId,
                    ControlId = compensatingControl.ControlId,
                    MerchantId = compensatingControl.MerchantId,
                    Justification = compensatingControl.Justification,
                    ImplementationDetails = compensatingControl.ImplementationDetails,
                    ApprovedBy = compensatingControl.ApprovedBy,
                    ApprovalDate = compensatingControl.ApprovalDate,
                    ExpiryDate = compensatingControl.ExpiryDate,
                    Rank = compensatingControl.Rank,
                    CreatedAt = compensatingControl.CreatedAt,
                    CreatedBy = compensatingControl.CreatedBy,
                    UpdatedAt = compensatingControl.UpdatedAt,
                    UpdatedBy = compensatingControl.UpdatedBy,
                    IsDeleted = compensatingControl.IsDeleted,
                    Merchant = compensatingControl.Merchant != null ? new MerchantEntityDto
                    {
                        MerchantId = compensatingControl.Merchant.MerchantId,
                        TenantId = compensatingControl.Merchant.TenantId,
                        MerchantCode = compensatingControl.Merchant.MerchantCode,
                        MerchantName = compensatingControl.Merchant.MerchantName,
                        MerchantLevel = compensatingControl.Merchant.MerchantLevel,
                        AcquirerName = compensatingControl.Merchant.AcquirerName,
                        ProcessorMID = compensatingControl.Merchant.ProcessorMID,
                        AnnualCardVolume = compensatingControl.Merchant.AnnualCardVolume,
                        LastAssessmentDate = compensatingControl.Merchant.LastAssessmentDate,
                        NextAssessmentDue = compensatingControl.Merchant.NextAssessmentDue,
                        ComplianceRank = compensatingControl.Merchant.ComplianceRank,
                        CreatedAt = compensatingControl.Merchant.CreatedAt,
                        CreatedBy = compensatingControl.Merchant.CreatedBy,
                        UpdatedAt = compensatingControl.Merchant.UpdatedAt,
                        UpdatedBy = compensatingControl.Merchant.UpdatedBy,
                        IsDeleted = compensatingControl.Merchant.IsDeleted,
                    } : null,
                }).ToList(),
                AssessmentControls = c.AssessmentControls.Select(assessmentControl => new AssessmentControlEntityDto
                {
                    RowId = assessmentControl.RowId,
                    AssessmentId = assessmentControl.AssessmentId,
                    ControlId = assessmentControl.ControlId,
                    TenantId = assessmentControl.TenantId,
                    TestResult = assessmentControl.TestResult,
                    TestDate = assessmentControl.TestDate,
                    TestedBy = assessmentControl.TestedBy,
                    Notes = assessmentControl.Notes,
                    CreatedAt = assessmentControl.CreatedAt,
                    CreatedBy = assessmentControl.CreatedBy,
                    UpdatedAt = assessmentControl.UpdatedAt,
                    UpdatedBy = assessmentControl.UpdatedBy,
                    IsDeleted = assessmentControl.IsDeleted,
                    Control = new ControlEntityDto
                    {
                        ControlId = assessmentControl.Control.ControlId,
                        TenantId = assessmentControl.Control.TenantId,
                        ControlCode = assessmentControl.Control.ControlCode,
                        RequirementNumber = assessmentControl.Control.RequirementNumber,
                        ControlTitle = assessmentControl.Control.ControlTitle,
                        ControlDescription = assessmentControl.Control.ControlDescription,
                        TestingGuidance = assessmentControl.Control.TestingGuidance,
                        FrequencyDays = assessmentControl.Control.FrequencyDays,
                        IsMandatory = assessmentControl.Control.IsMandatory,
                        EffectiveDate = assessmentControl.Control.EffectiveDate,
                        CreatedAt = assessmentControl.Control.CreatedAt,
                        CreatedBy = assessmentControl.Control.CreatedBy,
                        UpdatedAt = assessmentControl.Control.UpdatedAt,
                        UpdatedBy = assessmentControl.Control.UpdatedBy,
                        IsDeleted = assessmentControl.Control.IsDeleted,
                    },
                    Assessment = new AssessmentEntityDto
                    {
                        AssessmentId = assessmentControl.Assessment.AssessmentId,
                        TenantId = assessmentControl.Assessment.TenantId,
                        MerchantId = assessmentControl.Assessment.MerchantId,
                        AssessmentCode = assessmentControl.Assessment.AssessmentCode,
                        AssessmentType = assessmentControl.Assessment.AssessmentType,
                        AssessmentPeriod = assessmentControl.Assessment.AssessmentPeriod,
                        StartDate = assessmentControl.Assessment.StartDate,
                        EndDate = assessmentControl.Assessment.EndDate,
                        CompletionDate = assessmentControl.Assessment.CompletionDate,
                        Rank = assessmentControl.Assessment.Rank,
                        ComplianceScore = assessmentControl.Assessment.ComplianceScore,
                        QSAReviewRequired = assessmentControl.Assessment.QSAReviewRequired,
                        CreatedAt = assessmentControl.Assessment.CreatedAt,
                        CreatedBy = assessmentControl.Assessment.CreatedBy,
                        UpdatedAt = assessmentControl.Assessment.UpdatedAt,
                        UpdatedBy = assessmentControl.Assessment.UpdatedBy,
                        IsDeleted = assessmentControl.Assessment.IsDeleted,
                    },
                }).ToList(),
                AssetControls = c.AssetControls.Select(assetControl => new AssetControlEntityDto
                {
                    RowId = assetControl.RowId,
                    AssetId = assetControl.AssetId,
                    ControlId = assetControl.ControlId,
                    TenantId = assetControl.TenantId,
                    IsApplicable = assetControl.IsApplicable,
                    CustomizedApproach = assetControl.CustomizedApproach,
                    CreatedAt = assetControl.CreatedAt,
                    CreatedBy = assetControl.CreatedBy,
                    UpdatedAt = assetControl.UpdatedAt,
                    UpdatedBy = assetControl.UpdatedBy,
                    IsDeleted = assetControl.IsDeleted,
                    Control = new ControlEntityDto
                    {
                        ControlId = assetControl.Control.ControlId,
                        TenantId = assetControl.Control.TenantId,
                        ControlCode = assetControl.Control.ControlCode,
                        RequirementNumber = assetControl.Control.RequirementNumber,
                        ControlTitle = assetControl.Control.ControlTitle,
                        ControlDescription = assetControl.Control.ControlDescription,
                        TestingGuidance = assetControl.Control.TestingGuidance,
                        FrequencyDays = assetControl.Control.FrequencyDays,
                        IsMandatory = assetControl.Control.IsMandatory,
                        EffectiveDate = assetControl.Control.EffectiveDate,
                        CreatedAt = assetControl.Control.CreatedAt,
                        CreatedBy = assetControl.Control.CreatedBy,
                        UpdatedAt = assetControl.Control.UpdatedAt,
                        UpdatedBy = assetControl.Control.UpdatedBy,
                        IsDeleted = assetControl.Control.IsDeleted,
                    },
                    Asset = new AssetEntityDto
                    {
                        AssetId = assetControl.Asset.AssetId,
                        TenantId = assetControl.Asset.TenantId,
                        MerchantId = assetControl.Asset.MerchantId,
                        AssetCode = assetControl.Asset.AssetCode,
                        AssetName = assetControl.Asset.AssetName,
                        AssetType = assetControl.Asset.AssetType,
                        IPAddress = assetControl.Asset.IPAddress,
                        Hostname = assetControl.Asset.Hostname,
                        IsInCDE = assetControl.Asset.IsInCDE,
                        NetworkZone = assetControl.Asset.NetworkZone,
                        LastScanDate = assetControl.Asset.LastScanDate,
                        CreatedAt = assetControl.Asset.CreatedAt,
                        CreatedBy = assetControl.Asset.CreatedBy,
                        UpdatedAt = assetControl.Asset.UpdatedAt,
                        UpdatedBy = assetControl.Asset.UpdatedBy,
                        IsDeleted = assetControl.Asset.IsDeleted,
                    },
                }).ToList(),
                ControlEvidences = c.ControlEvidences.Select(controlEvidence => new ControlEvidenceEntityDto
                {
                    RowId = controlEvidence.RowId,
                    ControlId = controlEvidence.ControlId,
                    EvidenceId = controlEvidence.EvidenceId,
                    AssessmentId = controlEvidence.AssessmentId,
                    TenantId = controlEvidence.TenantId,
                    IsPrimary = controlEvidence.IsPrimary,
                    CreatedAt = controlEvidence.CreatedAt,
                    CreatedBy = controlEvidence.CreatedBy,
                    UpdatedAt = controlEvidence.UpdatedAt,
                    UpdatedBy = controlEvidence.UpdatedBy,
                    IsDeleted = controlEvidence.IsDeleted,
                    Control = new ControlEntityDto
                    {
                        ControlId = controlEvidence.Control.ControlId,
                        TenantId = controlEvidence.Control.TenantId,
                        ControlCode = controlEvidence.Control.ControlCode,
                        RequirementNumber = controlEvidence.Control.RequirementNumber,
                        ControlTitle = controlEvidence.Control.ControlTitle,
                        ControlDescription = controlEvidence.Control.ControlDescription,
                        TestingGuidance = controlEvidence.Control.TestingGuidance,
                        FrequencyDays = controlEvidence.Control.FrequencyDays,
                        IsMandatory = controlEvidence.Control.IsMandatory,
                        EffectiveDate = controlEvidence.Control.EffectiveDate,
                        CreatedAt = controlEvidence.Control.CreatedAt,
                        CreatedBy = controlEvidence.Control.CreatedBy,
                        UpdatedAt = controlEvidence.Control.UpdatedAt,
                        UpdatedBy = controlEvidence.Control.UpdatedBy,
                        IsDeleted = controlEvidence.Control.IsDeleted,
                    },
                    Evidence = new EvidenceEntityDto
                    {
                        EvidenceId = controlEvidence.Evidence.EvidenceId,
                        TenantId = controlEvidence.Evidence.TenantId,
                        MerchantId = controlEvidence.Evidence.MerchantId,
                        EvidenceCode = controlEvidence.Evidence.EvidenceCode,
                        EvidenceTitle = controlEvidence.Evidence.EvidenceTitle,
                        EvidenceType = controlEvidence.Evidence.EvidenceType,
                        CollectedDate = controlEvidence.Evidence.CollectedDate,
                        FileHash = controlEvidence.Evidence.FileHash,
                        StorageUri = controlEvidence.Evidence.StorageUri,
                        IsValid = controlEvidence.Evidence.IsValid,
                        CreatedAt = controlEvidence.Evidence.CreatedAt,
                        CreatedBy = controlEvidence.Evidence.CreatedBy,
                        UpdatedAt = controlEvidence.Evidence.UpdatedAt,
                        UpdatedBy = controlEvidence.Evidence.UpdatedBy,
                        IsDeleted = controlEvidence.Evidence.IsDeleted,
                    },
                }).ToList(),
            })
            .AsNoTracking()
            .AsSplitQuery()
        .EnableCache($"ControlAdvancedGraphSpec-{controlId}");
        }
    }

    public sealed class ControlAdvancedGraphSpecV6 : Specification<Control, ControlEntityDto>
    {
        public ControlAdvancedGraphSpecV6(
            Guid controlId,
            bool enableIntelligentProjection = true,
            bool enableSemanticAnalysis = true,
            bool enableBlueprintStrategy = true,
            int? take = null,
            int? skip = null)
        {
            Guard.Against.Default(controlId, nameof(controlId));
            Query.Where(c => c.ControlId == controlId);

            if (take.HasValue && skip.HasValue)
            {
                Query.Skip(skip.Value).Take(take.Value);
            }
            if (enableBlueprintStrategy)
            {

            }

            Query.Select(c => new ControlEntityDto
            {
                ControlId = c.ControlId,
                TenantId = c.TenantId,
                ControlCode = c.ControlCode,
                RequirementNumber = c.RequirementNumber,
                ControlTitle = c.ControlTitle,
                ControlDescription = c.ControlDescription,
                TestingGuidance = c.TestingGuidance,
                FrequencyDays = c.FrequencyDays,
                IsMandatory = c.IsMandatory,
                EffectiveDate = c.EffectiveDate,
                CreatedAt = c.CreatedAt,
                CreatedBy = c.CreatedBy,
                UpdatedAt = c.UpdatedAt,
                UpdatedBy = c.UpdatedBy,
                IsDeleted = c.IsDeleted,
                #region Intelligent Metrics
                AssessmentComplianceScore = 0,
                AverageMerchantAnnualCardVolume = c.CompensatingControls.Select(i => i.Merchant).Any() ? c.CompensatingControls.Select(i => i.Merchant).Average(t => (decimal?)t.AnnualCardVolume) ?? 0 : 0,
                DaysSinceEffectiveDate = (DateTime.UtcNow - c.EffectiveDate).Days,
                IsMandatoryFlag = c.IsMandatory,
                TotalAssessmentControlCount = c.AssessmentControls.Count(),
                ActiveAssessmentControlCount = c.AssessmentControls.Count(x => !x.IsDeleted),
                LatestAssessmentControlTestDate = c.AssessmentControls.Max(x => (DateTime?)x.TestDate),
                TotalAssetControlCount = c.AssetControls.Count(),
                ActiveAssetControlCount = c.AssetControls.Count(x => !x.IsDeleted),
                LatestAssetControlUpdatedAt = c.AssetControls.Max(x => (DateTime?)x.UpdatedAt),
                TotalCompensatingControlCount = c.CompensatingControls.Count(),
                ActiveCompensatingControlCount = c.CompensatingControls.Count(x => !x.IsDeleted),
                CriticalCompensatingControlCount = c.CompensatingControls.Count(x => x.Rank >= 7),
                LatestCompensatingControlApprovalDate = c.CompensatingControls.Max(x => (DateTime?)x.ApprovalDate),
                TotalControlEvidenceCount = c.ControlEvidences.Count(),
                ActiveControlEvidenceCount = c.ControlEvidences.Count(x => !x.IsDeleted),
                LatestControlEvidenceUpdatedAt = c.ControlEvidences.Max(x => (DateTime?)x.UpdatedAt),
                #endregion
                #region Semantic Pattern Fields
                #endregion

                Merchants = c.CompensatingControls
                    .Select(i => i.Merchant)
                    .Select(t => new MerchantEntityDto
                    {
                        MerchantId = t.MerchantId,
                        TenantId = t.TenantId,
                        MerchantCode = t.MerchantCode,
                        MerchantName = t.MerchantName,
                        MerchantLevel = t.MerchantLevel,
                        AcquirerName = t.AcquirerName,
                        ProcessorMID = t.ProcessorMID,
                        AnnualCardVolume = t.AnnualCardVolume,
                        LastAssessmentDate = t.LastAssessmentDate,
                        NextAssessmentDue = t.NextAssessmentDue,
                        ComplianceRank = t.ComplianceRank,
                        IsDeleted = t.IsDeleted,
                    })
                    .Take(2).ToList(),
                CompensatingControls = c.CompensatingControls.Select(compensatingControl => new CompensatingControlEntityDto
                {
                    CompensatingControlId = compensatingControl.CompensatingControlId,
                    TenantId = compensatingControl.TenantId,
                    ControlId = compensatingControl.ControlId,
                    MerchantId = compensatingControl.MerchantId,
                    Justification = compensatingControl.Justification,
                    ImplementationDetails = compensatingControl.ImplementationDetails,
                    ApprovedBy = compensatingControl.ApprovedBy,
                    ApprovalDate = compensatingControl.ApprovalDate,
                    ExpiryDate = compensatingControl.ExpiryDate,
                    Rank = compensatingControl.Rank,
                    CreatedAt = compensatingControl.CreatedAt,
                    CreatedBy = compensatingControl.CreatedBy,
                    UpdatedAt = compensatingControl.UpdatedAt,
                    UpdatedBy = compensatingControl.UpdatedBy,
                    IsDeleted = compensatingControl.IsDeleted,
                    Merchant = compensatingControl.Merchant != null ? new MerchantEntityDto
                    {
                        MerchantId = compensatingControl.Merchant.MerchantId,
                        TenantId = compensatingControl.Merchant.TenantId,
                        MerchantCode = compensatingControl.Merchant.MerchantCode,
                        MerchantName = compensatingControl.Merchant.MerchantName,
                        MerchantLevel = compensatingControl.Merchant.MerchantLevel,
                        AcquirerName = compensatingControl.Merchant.AcquirerName,
                        ProcessorMID = compensatingControl.Merchant.ProcessorMID,
                        AnnualCardVolume = compensatingControl.Merchant.AnnualCardVolume,
                        LastAssessmentDate = compensatingControl.Merchant.LastAssessmentDate,
                        NextAssessmentDue = compensatingControl.Merchant.NextAssessmentDue,
                        ComplianceRank = compensatingControl.Merchant.ComplianceRank,
                        CreatedAt = compensatingControl.Merchant.CreatedAt,
                        CreatedBy = compensatingControl.Merchant.CreatedBy,
                        UpdatedAt = compensatingControl.Merchant.UpdatedAt,
                        UpdatedBy = compensatingControl.Merchant.UpdatedBy,
                        IsDeleted = compensatingControl.Merchant.IsDeleted,
                    } : null,
                }).ToList(),
                AssessmentControls = c.AssessmentControls.Select(assessmentControl => new AssessmentControlEntityDto
                {
                    RowId = assessmentControl.RowId,
                    AssessmentId = assessmentControl.AssessmentId,
                    ControlId = assessmentControl.ControlId,
                    TenantId = assessmentControl.TenantId,
                    TestResult = assessmentControl.TestResult,
                    TestDate = assessmentControl.TestDate,
                    TestedBy = assessmentControl.TestedBy,
                    Notes = assessmentControl.Notes,
                    CreatedAt = assessmentControl.CreatedAt,
                    CreatedBy = assessmentControl.CreatedBy,
                    UpdatedAt = assessmentControl.UpdatedAt,
                    UpdatedBy = assessmentControl.UpdatedBy,
                    IsDeleted = assessmentControl.IsDeleted,
                    Control = new ControlEntityDto
                    {
                        ControlId = assessmentControl.Control.ControlId,
                        TenantId = assessmentControl.Control.TenantId,
                        ControlCode = assessmentControl.Control.ControlCode,
                        RequirementNumber = assessmentControl.Control.RequirementNumber,
                        ControlTitle = assessmentControl.Control.ControlTitle,
                        ControlDescription = assessmentControl.Control.ControlDescription,
                        TestingGuidance = assessmentControl.Control.TestingGuidance,
                        FrequencyDays = assessmentControl.Control.FrequencyDays,
                        IsMandatory = assessmentControl.Control.IsMandatory,
                        EffectiveDate = assessmentControl.Control.EffectiveDate,
                        CreatedAt = assessmentControl.Control.CreatedAt,
                        CreatedBy = assessmentControl.Control.CreatedBy,
                        UpdatedAt = assessmentControl.Control.UpdatedAt,
                        UpdatedBy = assessmentControl.Control.UpdatedBy,
                        IsDeleted = assessmentControl.Control.IsDeleted,
                    },
                    Assessment = new AssessmentEntityDto
                    {
                        AssessmentId = assessmentControl.Assessment.AssessmentId,
                        TenantId = assessmentControl.Assessment.TenantId,
                        MerchantId = assessmentControl.Assessment.MerchantId,
                        AssessmentCode = assessmentControl.Assessment.AssessmentCode,
                        AssessmentType = assessmentControl.Assessment.AssessmentType,
                        AssessmentPeriod = assessmentControl.Assessment.AssessmentPeriod,
                        StartDate = assessmentControl.Assessment.StartDate,
                        EndDate = assessmentControl.Assessment.EndDate,
                        CompletionDate = assessmentControl.Assessment.CompletionDate,
                        Rank = assessmentControl.Assessment.Rank,
                        ComplianceScore = assessmentControl.Assessment.ComplianceScore,
                        QSAReviewRequired = assessmentControl.Assessment.QSAReviewRequired,
                        CreatedAt = assessmentControl.Assessment.CreatedAt,
                        CreatedBy = assessmentControl.Assessment.CreatedBy,
                        UpdatedAt = assessmentControl.Assessment.UpdatedAt,
                        UpdatedBy = assessmentControl.Assessment.UpdatedBy,
                        IsDeleted = assessmentControl.Assessment.IsDeleted,
                    },
                }).ToList(),
                AssetControls = c.AssetControls.Select(assetControl => new AssetControlEntityDto
                {
                    RowId = assetControl.RowId,
                    AssetId = assetControl.AssetId,
                    ControlId = assetControl.ControlId,
                    TenantId = assetControl.TenantId,
                    IsApplicable = assetControl.IsApplicable,
                    CustomizedApproach = assetControl.CustomizedApproach,
                    CreatedAt = assetControl.CreatedAt,
                    CreatedBy = assetControl.CreatedBy,
                    UpdatedAt = assetControl.UpdatedAt,
                    UpdatedBy = assetControl.UpdatedBy,
                    IsDeleted = assetControl.IsDeleted,
                    Control = new ControlEntityDto
                    {
                        ControlId = assetControl.Control.ControlId,
                        TenantId = assetControl.Control.TenantId,
                        ControlCode = assetControl.Control.ControlCode,
                        RequirementNumber = assetControl.Control.RequirementNumber,
                        ControlTitle = assetControl.Control.ControlTitle,
                        ControlDescription = assetControl.Control.ControlDescription,
                        TestingGuidance = assetControl.Control.TestingGuidance,
                        FrequencyDays = assetControl.Control.FrequencyDays,
                        IsMandatory = assetControl.Control.IsMandatory,
                        EffectiveDate = assetControl.Control.EffectiveDate,
                        CreatedAt = assetControl.Control.CreatedAt,
                        CreatedBy = assetControl.Control.CreatedBy,
                        UpdatedAt = assetControl.Control.UpdatedAt,
                        UpdatedBy = assetControl.Control.UpdatedBy,
                        IsDeleted = assetControl.Control.IsDeleted,
                    },
                    Asset = new AssetEntityDto
                    {
                        AssetId = assetControl.Asset.AssetId,
                        TenantId = assetControl.Asset.TenantId,
                        MerchantId = assetControl.Asset.MerchantId,
                        AssetCode = assetControl.Asset.AssetCode,
                        AssetName = assetControl.Asset.AssetName,
                        AssetType = assetControl.Asset.AssetType,
                        IPAddress = assetControl.Asset.IPAddress,
                        Hostname = assetControl.Asset.Hostname,
                        IsInCDE = assetControl.Asset.IsInCDE,
                        NetworkZone = assetControl.Asset.NetworkZone,
                        LastScanDate = assetControl.Asset.LastScanDate,
                        CreatedAt = assetControl.Asset.CreatedAt,
                        CreatedBy = assetControl.Asset.CreatedBy,
                        UpdatedAt = assetControl.Asset.UpdatedAt,
                        UpdatedBy = assetControl.Asset.UpdatedBy,
                        IsDeleted = assetControl.Asset.IsDeleted,
                    },
                }).ToList(),
                ControlEvidences = c.ControlEvidences.Select(controlEvidence => new ControlEvidenceEntityDto
                {
                    RowId = controlEvidence.RowId,
                    ControlId = controlEvidence.ControlId,
                    EvidenceId = controlEvidence.EvidenceId,
                    AssessmentId = controlEvidence.AssessmentId,
                    TenantId = controlEvidence.TenantId,
                    IsPrimary = controlEvidence.IsPrimary,
                    CreatedAt = controlEvidence.CreatedAt,
                    CreatedBy = controlEvidence.CreatedBy,
                    UpdatedAt = controlEvidence.UpdatedAt,
                    UpdatedBy = controlEvidence.UpdatedBy,
                    IsDeleted = controlEvidence.IsDeleted,
                    Control = new ControlEntityDto
                    {
                        ControlId = controlEvidence.Control.ControlId,
                        TenantId = controlEvidence.Control.TenantId,
                        ControlCode = controlEvidence.Control.ControlCode,
                        RequirementNumber = controlEvidence.Control.RequirementNumber,
                        ControlTitle = controlEvidence.Control.ControlTitle,
                        ControlDescription = controlEvidence.Control.ControlDescription,
                        TestingGuidance = controlEvidence.Control.TestingGuidance,
                        FrequencyDays = controlEvidence.Control.FrequencyDays,
                        IsMandatory = controlEvidence.Control.IsMandatory,
                        EffectiveDate = controlEvidence.Control.EffectiveDate,
                        CreatedAt = controlEvidence.Control.CreatedAt,
                        CreatedBy = controlEvidence.Control.CreatedBy,
                        UpdatedAt = controlEvidence.Control.UpdatedAt,
                        UpdatedBy = controlEvidence.Control.UpdatedBy,
                        IsDeleted = controlEvidence.Control.IsDeleted,
                    },
                    Evidence = new EvidenceEntityDto
                    {
                        EvidenceId = controlEvidence.Evidence.EvidenceId,
                        TenantId = controlEvidence.Evidence.TenantId,
                        MerchantId = controlEvidence.Evidence.MerchantId,
                        EvidenceCode = controlEvidence.Evidence.EvidenceCode,
                        EvidenceTitle = controlEvidence.Evidence.EvidenceTitle,
                        EvidenceType = controlEvidence.Evidence.EvidenceType,
                        CollectedDate = controlEvidence.Evidence.CollectedDate,
                        FileHash = controlEvidence.Evidence.FileHash,
                        StorageUri = controlEvidence.Evidence.StorageUri,
                        IsValid = controlEvidence.Evidence.IsValid,
                        CreatedAt = controlEvidence.Evidence.CreatedAt,
                        CreatedBy = controlEvidence.Evidence.CreatedBy,
                        UpdatedAt = controlEvidence.Evidence.UpdatedAt,
                        UpdatedBy = controlEvidence.Evidence.UpdatedBy,
                        IsDeleted = controlEvidence.Evidence.IsDeleted,
                    },
                }).ToList(),
            })
            .AsNoTracking()
            .AsSplitQuery()
            .EnableCache($"ControlAdvancedGraphSpecV6-{controlId}");
        }
    }

    public sealed class ControlAdvancedFilterSpec : Specification<Control>
    {
        public ControlAdvancedFilterSpec(
            int pageNumber,
            int pageSize,
            Dictionary<string, string> filters = null,
            List<Sort> sorting = null
        )
        {
            _ = Guard.Against.NegativeOrZero(pageNumber, nameof(pageNumber));
            _ = Guard.Against.NegativeOrZero(pageSize, nameof(pageSize));

            _ = Query;

            if (filters != null)
            {
                foreach (var filter in filters)
                {
                    switch (filter.Key.ToLower())
                    {
                        case "controlcode":
                            Query.Where(c => c.ControlCode.Contains(filter.Value));
                            break;
                        case "requirementnumber":
                            Query.Where(c => c.RequirementNumber.Contains(filter.Value));
                            break;
                        case "frequencydays":
                            if (int.TryParse(filter.Value, out int frequencydays))
                            {
                                Query.Where(c => c.FrequencyDays == frequencydays);
                            }
                            break;
                        case "ismandatory":
                            if (bool.TryParse(filter.Value, out bool ismandatory))
                            {
                                Query.Where(c => c.IsMandatory == ismandatory);
                            }
                            break;
                        case "effectivedate":
                            if (DateTime.TryParse(filter.Value, out DateTime effectivedate))
                            {
                                Query.Where(c => c.EffectiveDate >= effectivedate.AddHours(-6) && c.EffectiveDate <= effectivedate.AddHours(6));
                            }
                            break;
                        case "createdat":
                            if (DateTime.TryParse(filter.Value, out DateTime createdat))
                            {
                                Query.Where(c => c.CreatedAt >= createdat.AddHours(-6) && c.CreatedAt <= createdat.AddHours(6));
                            }
                            break;
                        case "updatedat":
                            if (DateTime.TryParse(filter.Value, out DateTime updatedat))
                            {
                                Query.Where(c => c.UpdatedAt >= updatedat.AddHours(-6) && c.UpdatedAt <= updatedat.AddHours(6));
                            }
                            break;
                        case "isdeleted":
                            if (bool.TryParse(filter.Value, out bool isdeleted))
                            {
                                Query.Where(c => c.IsDeleted == isdeleted);
                            }
                            break;
                    }
                }
            }

            if (sorting != null && sorting.Any())
            {
                var first = sorting.First();
                var ordered = ApplySort(Query, first);

                foreach (var sort in sorting.Skip(1))
                {
                    ordered = ApplyAdditionalSort(ordered, sort);
                }
            }
            else
            {
                Query.OrderByDescending(x => x.ControlId);
            }

            Query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }

        private static IOrderedSpecificationBuilder<Control> ApplySort(
            ISpecificationBuilder<Control> query,
            Sort sort
        )
        {
            return sort.Direction == SortDirection.Descending
                ? query.OrderByDescending(GetSortProperty(sort.Field))
                : query.OrderBy(GetSortProperty(sort.Field));
        }

        private static IOrderedSpecificationBuilder<Control> ApplyAdditionalSort(
            IOrderedSpecificationBuilder<Control> query,
            Sort sort
        )
        {
            return sort.Direction == SortDirection.Descending
                ? query.ThenByDescending(GetSortProperty(sort.Field))
                : query.ThenBy(GetSortProperty(sort.Field));
        }

        private static Expression<Func<Control, object>> GetSortProperty(
            string propertyName
        )
        {
            return propertyName.ToLower() switch
            {
                "controlcode" => c => c.ControlCode,
                "requirementnumber" => c => c.RequirementNumber,
                _ => c => c.ControlId,
            };
        }
    }