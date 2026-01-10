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
    public sealed class AssessmentListPagedSpec : PagedSpecification<Assessment, AssessmentEntityDto>
    {
        public AssessmentListPagedSpec(int pageNumber, int pageSize)
        {
            _ = Guard.Against.NegativeOrZero(pageNumber, nameof(pageNumber));
            _ = Guard.Against.NegativeOrZero(pageSize, nameof(pageSize));

            _ = Query
                .OrderByDescending(i => i.AssessmentId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            _ = Query.Include(x => x.Merchant);
            _ = Query.Select(x => new AssessmentEntityDto
            {
                AssessmentId = x.AssessmentId,
                TenantId = x.TenantId,
                MerchantId = x.MerchantId,
                AssessmentCode = x.AssessmentCode,
                AssessmentType = x.AssessmentType,
                AssessmentPeriod = x.AssessmentPeriod,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                CompletionDate = x.CompletionDate,
                Rank = x.Rank,
                ComplianceScore = x.ComplianceScore,
                QSAReviewRequired = x.QSAReviewRequired,
                CreatedAt = x.CreatedAt,
                CreatedBy = x.CreatedBy,
                UpdatedAt = x.UpdatedAt,
                UpdatedBy = x.UpdatedBy,
                IsDeleted = x.IsDeleted,
                Merchant = x.Merchant != null ? new MerchantEntityDto
                {
                    MerchantId = x.Merchant.MerchantId,
                    TenantId = x.Merchant.TenantId,
                    MerchantCode = x.Merchant.MerchantCode,
                    MerchantName = x.Merchant.MerchantName,
                    MerchantLevel = x.Merchant.MerchantLevel,
                    AcquirerName = x.Merchant.AcquirerName,
                    ProcessorMID = x.Merchant.ProcessorMID,
                    AnnualCardVolume = x.Merchant.AnnualCardVolume,
                    LastAssessmentDate = x.Merchant.LastAssessmentDate,
                    NextAssessmentDue = x.Merchant.NextAssessmentDue,
                    ComplianceRank = x.Merchant.ComplianceRank,
                    CreatedAt = x.Merchant.CreatedAt,
                    CreatedBy = x.Merchant.CreatedBy,
                    UpdatedAt = x.Merchant.UpdatedAt,
                    UpdatedBy = x.Merchant.UpdatedBy,
                    IsDeleted = x.Merchant.IsDeleted,
                } : null,
            })
            .AsNoTracking()
            .AsSplitQuery()
            .EnableCache($"AssessmentListPagedSpec-{pageNumber}-{pageSize}");
        }
    }
    public sealed class AssessmentSearchSpec : Specification<Assessment>
    {
        public AssessmentSearchSpec(string searchTerm)
        {
            string searchLower = searchTerm?.ToLower() ?? string.Empty;

            Query
                .Where(c =>
                        (c.AssessmentCode != null && c.AssessmentCode.ToLower().Contains(searchLower)) ||
                        (c.AssessmentPeriod != null && c.AssessmentPeriod.ToLower().Contains(searchLower))                )
                .OrderByDescending(c => c.AssessmentId);
        }
    }

    public sealed class AssessmentLastCreatedSpec : Specification<Assessment>
    {
        public AssessmentLastCreatedSpec()
        {
            Query
                .OrderByDescending(c => c.StartDate)
                .Take(1)
                .AsNoTracking()
                .EnableCache("AssessmentLastCreatedSpec");
        }
    }
    public sealed class AssessmentByIdSpec : Specification<Assessment, AssessmentEntityDto>
    {
        public AssessmentByIdSpec(Guid id)
        {
            _ = Guard.Against.NullOrEmpty(id, nameof(id));

            _ = Query.Where(x => x.AssessmentId == id);

            _ = Query.Include(x => x.Merchant);
            _ = Query.Include(x => x.ROCPackages);
            _ = Query.Include(x => x.AssessmentControls);
            _ = Query.Include(x => x.ControlEvidences);
            _ = Query.Select(x => new AssessmentEntityDto
            {
                AssessmentId = x.AssessmentId,
                TenantId = x.TenantId,
                MerchantId = x.MerchantId,
                AssessmentCode = x.AssessmentCode,
                AssessmentType = x.AssessmentType,
                AssessmentPeriod = x.AssessmentPeriod,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                CompletionDate = x.CompletionDate,
                Rank = x.Rank,
                ComplianceScore = x.ComplianceScore,
                QSAReviewRequired = x.QSAReviewRequired,
                CreatedAt = x.CreatedAt,
                CreatedBy = x.CreatedBy,
                UpdatedAt = x.UpdatedAt,
                UpdatedBy = x.UpdatedBy,
                IsDeleted = x.IsDeleted,

                Merchant = x.Merchant != null ? new MerchantEntityDto
                {
                    MerchantId = x.Merchant.MerchantId,
                    TenantId = x.Merchant.TenantId,
                    MerchantCode = x.Merchant.MerchantCode,
                    MerchantName = x.Merchant.MerchantName,
                    MerchantLevel = x.Merchant.MerchantLevel,
                    AcquirerName = x.Merchant.AcquirerName,
                    ProcessorMID = x.Merchant.ProcessorMID,
                    AnnualCardVolume = x.Merchant.AnnualCardVolume,
                    LastAssessmentDate = x.Merchant.LastAssessmentDate,
                    NextAssessmentDue = x.Merchant.NextAssessmentDue,
                    ComplianceRank = x.Merchant.ComplianceRank,
                    CreatedAt = x.Merchant.CreatedAt,
                    CreatedBy = x.Merchant.CreatedBy,
                    UpdatedAt = x.Merchant.UpdatedAt,
                    UpdatedBy = x.Merchant.UpdatedBy,
                    IsDeleted = x.Merchant.IsDeleted,
                } : null,
                ROCPackages = x.ROCPackages.Select(rocpackage => new ROCPackageEntityDto
                {
                    ROCPackageId = rocpackage.ROCPackageId,
                    TenantId = rocpackage.TenantId,
                    AssessmentId = rocpackage.AssessmentId,
                    PackageVersion = rocpackage.PackageVersion,
                    GeneratedDate = rocpackage.GeneratedDate,
                    QSAName = rocpackage.QSAName,
                    QSACompany = rocpackage.QSACompany,
                    SignatureDate = rocpackage.SignatureDate,
                    AOCNumber = rocpackage.AOCNumber,
                    Rank = rocpackage.Rank,
                    CreatedAt = rocpackage.CreatedAt,
                    CreatedBy = rocpackage.CreatedBy,
                    UpdatedAt = rocpackage.UpdatedAt,
                    UpdatedBy = rocpackage.UpdatedBy,
                    IsDeleted = rocpackage.IsDeleted,
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
                    Assessment = new AssessmentEntityDto
                    {
                        AssessmentId = controlEvidence.Assessment.AssessmentId,
                        TenantId = controlEvidence.Assessment.TenantId,
                        MerchantId = controlEvidence.Assessment.MerchantId,
                        AssessmentCode = controlEvidence.Assessment.AssessmentCode,
                        AssessmentType = controlEvidence.Assessment.AssessmentType,
                        AssessmentPeriod = controlEvidence.Assessment.AssessmentPeriod,
                        StartDate = controlEvidence.Assessment.StartDate,
                        EndDate = controlEvidence.Assessment.EndDate,
                        CompletionDate = controlEvidence.Assessment.CompletionDate,
                        Rank = controlEvidence.Assessment.Rank,
                        ComplianceScore = controlEvidence.Assessment.ComplianceScore,
                        QSAReviewRequired = controlEvidence.Assessment.QSAReviewRequired,
                        CreatedAt = controlEvidence.Assessment.CreatedAt,
                        CreatedBy = controlEvidence.Assessment.CreatedBy,
                        UpdatedAt = controlEvidence.Assessment.UpdatedAt,
                        UpdatedBy = controlEvidence.Assessment.UpdatedBy,
                        IsDeleted = controlEvidence.Assessment.IsDeleted,
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
            .EnableCache($"AssessmentByIdSpec-{id.ToString()}");
        }
    }
}

    public sealed class AssessmentAdvancedGraphSpecV4 : Specification<Assessment, AssessmentEntityDto>
    {
        public AssessmentAdvancedGraphSpecV4(
            Guid assessmentId,
            bool includeReferenceData = true,
            bool includeTransactionData = true,
            bool includeHierarchicalData = true,
            int? take = null,
            int? skip = null,
            DateTime? effectiveDate = null)
        {
            Guard.Against.Default(assessmentId, nameof(assessmentId));
            Query.Where(c => c.AssessmentId == assessmentId);

            if (take.HasValue && skip.HasValue)
            {
            }
            Query.Include(c => c.Merchant);

            if (includeReferenceData)
            {
            }

            if (includeTransactionData)
            {
            }

            if (includeHierarchicalData)
            {
            }

            Query.Select(c => new AssessmentEntityDto
            {
                AssessmentId = c.AssessmentId,
                TenantId = c.TenantId,
                MerchantId = c.MerchantId,
                AssessmentCode = c.AssessmentCode,
                AssessmentType = c.AssessmentType,
                AssessmentPeriod = c.AssessmentPeriod,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                CompletionDate = c.CompletionDate,
                Rank = c.Rank,
                ComplianceScore = c.ComplianceScore,
                QSAReviewRequired = c.QSAReviewRequired,
                CreatedAt = c.CreatedAt,
                CreatedBy = c.CreatedBy,
                UpdatedAt = c.UpdatedAt,
                UpdatedBy = c.UpdatedBy,
                IsDeleted = c.IsDeleted,
                Merchant = c.Merchant != null ? new MerchantEntityDto
                {
                    MerchantId = c.Merchant.MerchantId,
                    TenantId = c.Merchant.TenantId,
                    MerchantCode = c.Merchant.MerchantCode,
                    MerchantName = c.Merchant.MerchantName,
                    MerchantLevel = c.Merchant.MerchantLevel,
                    AcquirerName = c.Merchant.AcquirerName,
                    ProcessorMID = c.Merchant.ProcessorMID,
                    AnnualCardVolume = c.Merchant.AnnualCardVolume,
                    LastAssessmentDate = c.Merchant.LastAssessmentDate,
                    NextAssessmentDue = c.Merchant.NextAssessmentDue,
                    ComplianceRank = c.Merchant.ComplianceRank,
                    CreatedAt = c.Merchant.CreatedAt,
                    CreatedBy = c.Merchant.CreatedBy,
                    UpdatedAt = c.Merchant.UpdatedAt,
                    UpdatedBy = c.Merchant.UpdatedBy,
                    IsDeleted = c.Merchant.IsDeleted,
                } : null,
                ROCPackages = c.ROCPackages.Select(rocpackage => new ROCPackageEntityDto
                {
                    ROCPackageId = rocpackage.ROCPackageId,
                    TenantId = rocpackage.TenantId,
                    AssessmentId = rocpackage.AssessmentId,
                    PackageVersion = rocpackage.PackageVersion,
                    GeneratedDate = rocpackage.GeneratedDate,
                    QSAName = rocpackage.QSAName,
                    QSACompany = rocpackage.QSACompany,
                    SignatureDate = rocpackage.SignatureDate,
                    AOCNumber = rocpackage.AOCNumber,
                    Rank = rocpackage.Rank,
                    CreatedAt = rocpackage.CreatedAt,
                    CreatedBy = rocpackage.CreatedBy,
                    UpdatedAt = rocpackage.UpdatedAt,
                    UpdatedBy = rocpackage.UpdatedBy,
                    IsDeleted = rocpackage.IsDeleted,
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
                    Assessment = new AssessmentEntityDto
                    {
                        AssessmentId = controlEvidence.Assessment.AssessmentId,
                        TenantId = controlEvidence.Assessment.TenantId,
                        MerchantId = controlEvidence.Assessment.MerchantId,
                        AssessmentCode = controlEvidence.Assessment.AssessmentCode,
                        AssessmentType = controlEvidence.Assessment.AssessmentType,
                        AssessmentPeriod = controlEvidence.Assessment.AssessmentPeriod,
                        StartDate = controlEvidence.Assessment.StartDate,
                        EndDate = controlEvidence.Assessment.EndDate,
                        CompletionDate = controlEvidence.Assessment.CompletionDate,
                        Rank = controlEvidence.Assessment.Rank,
                        ComplianceScore = controlEvidence.Assessment.ComplianceScore,
                        QSAReviewRequired = controlEvidence.Assessment.QSAReviewRequired,
                        CreatedAt = controlEvidence.Assessment.CreatedAt,
                        CreatedBy = controlEvidence.Assessment.CreatedBy,
                        UpdatedAt = controlEvidence.Assessment.UpdatedAt,
                        UpdatedBy = controlEvidence.Assessment.UpdatedBy,
                        IsDeleted = controlEvidence.Assessment.IsDeleted,
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
        .EnableCache($"AssessmentAdvancedGraphSpec-{assessmentId}");
        }
    }

    public sealed class AssessmentAdvancedGraphSpecV6 : Specification<Assessment, AssessmentEntityDto>
    {
        public AssessmentAdvancedGraphSpecV6(
            Guid assessmentId,
            bool enableIntelligentProjection = true,
            bool enableSemanticAnalysis = true,
            bool enableBlueprintStrategy = true,
            int? take = null,
            int? skip = null)
        {
            Guard.Against.Default(assessmentId, nameof(assessmentId));
            Query.Where(c => c.AssessmentId == assessmentId);

            if (take.HasValue && skip.HasValue)
            {
                Query.Skip(skip.Value).Take(take.Value);
            }
            Query.Include(c => c.Merchant);
            if (enableBlueprintStrategy)
            {
            Query.Include(c => c.Merchant);

            }

            Query.Select(c => new AssessmentEntityDto
            {
                AssessmentId = c.AssessmentId,
                TenantId = c.TenantId,
                MerchantId = c.MerchantId,
                AssessmentCode = c.AssessmentCode,
                AssessmentType = c.AssessmentType,
                AssessmentPeriod = c.AssessmentPeriod,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                CompletionDate = c.CompletionDate,
                Rank = c.Rank,
                ComplianceScore = c.ComplianceScore,
                QSAReviewRequired = c.QSAReviewRequired,
                CreatedAt = c.CreatedAt,
                CreatedBy = c.CreatedBy,
                UpdatedAt = c.UpdatedAt,
                UpdatedBy = c.UpdatedBy,
                IsDeleted = c.IsDeleted,
                #region Intelligent Metrics
                ControlFrequencyDays = 0,
                IsAssessmentTypeCritical = c.AssessmentType >= 7,
                DaysSinceStartDate = (DateTime.UtcNow - c.StartDate).Days,
                IsRankCritical = c.Rank >= 7,
                IsDeletedFlag = c.IsDeleted,
                TotalAssessmentControlCount = c.AssessmentControls.Count(),
                ActiveAssessmentControlCount = c.AssessmentControls.Count(x => !x.IsDeleted),
                LatestAssessmentControlTestDate = c.AssessmentControls.Max(x => (DateTime?)x.TestDate),
                TotalControlEvidenceCount = c.ControlEvidences.Count(),
                ActiveControlEvidenceCount = c.ControlEvidences.Count(x => !x.IsDeleted),
                LatestControlEvidenceUpdatedAt = c.ControlEvidences.Max(x => (DateTime?)x.UpdatedAt),
                TotalROCPackageCount = c.ROCPackages.Count(),
                ActiveROCPackageCount = c.ROCPackages.Count(x => !x.IsDeleted),
                CriticalROCPackageCount = c.ROCPackages.Count(x => x.Rank >= 7),
                LatestROCPackageGeneratedDate = c.ROCPackages.Max(x => (DateTime?)x.GeneratedDate),
                #endregion
                #region Semantic Pattern Fields
                #endregion
                Merchant = c.Merchant != null ? new MerchantEntityDto
                {
                    MerchantId = c.Merchant.MerchantId,
                    TenantId = c.Merchant.TenantId,
                    MerchantCode = c.Merchant.MerchantCode,
                    MerchantName = c.Merchant.MerchantName,
                    MerchantLevel = c.Merchant.MerchantLevel,
                    AcquirerName = c.Merchant.AcquirerName,
                    ProcessorMID = c.Merchant.ProcessorMID,
                    AnnualCardVolume = c.Merchant.AnnualCardVolume,
                    LastAssessmentDate = c.Merchant.LastAssessmentDate,
                    NextAssessmentDue = c.Merchant.NextAssessmentDue,
                    ComplianceRank = c.Merchant.ComplianceRank,
                    CreatedAt = c.Merchant.CreatedAt,
                    CreatedBy = c.Merchant.CreatedBy,
                    UpdatedAt = c.Merchant.UpdatedAt,
                    UpdatedBy = c.Merchant.UpdatedBy,
                    IsDeleted = c.Merchant.IsDeleted,
                } : null,
                ROCPackages = c.ROCPackages.Select(rocpackage => new ROCPackageEntityDto
                {
                    ROCPackageId = rocpackage.ROCPackageId,
                    TenantId = rocpackage.TenantId,
                    AssessmentId = rocpackage.AssessmentId,
                    PackageVersion = rocpackage.PackageVersion,
                    GeneratedDate = rocpackage.GeneratedDate,
                    QSAName = rocpackage.QSAName,
                    QSACompany = rocpackage.QSACompany,
                    SignatureDate = rocpackage.SignatureDate,
                    AOCNumber = rocpackage.AOCNumber,
                    Rank = rocpackage.Rank,
                    CreatedAt = rocpackage.CreatedAt,
                    CreatedBy = rocpackage.CreatedBy,
                    UpdatedAt = rocpackage.UpdatedAt,
                    UpdatedBy = rocpackage.UpdatedBy,
                    IsDeleted = rocpackage.IsDeleted,
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
                    Assessment = new AssessmentEntityDto
                    {
                        AssessmentId = controlEvidence.Assessment.AssessmentId,
                        TenantId = controlEvidence.Assessment.TenantId,
                        MerchantId = controlEvidence.Assessment.MerchantId,
                        AssessmentCode = controlEvidence.Assessment.AssessmentCode,
                        AssessmentType = controlEvidence.Assessment.AssessmentType,
                        AssessmentPeriod = controlEvidence.Assessment.AssessmentPeriod,
                        StartDate = controlEvidence.Assessment.StartDate,
                        EndDate = controlEvidence.Assessment.EndDate,
                        CompletionDate = controlEvidence.Assessment.CompletionDate,
                        Rank = controlEvidence.Assessment.Rank,
                        ComplianceScore = controlEvidence.Assessment.ComplianceScore,
                        QSAReviewRequired = controlEvidence.Assessment.QSAReviewRequired,
                        CreatedAt = controlEvidence.Assessment.CreatedAt,
                        CreatedBy = controlEvidence.Assessment.CreatedBy,
                        UpdatedAt = controlEvidence.Assessment.UpdatedAt,
                        UpdatedBy = controlEvidence.Assessment.UpdatedBy,
                        IsDeleted = controlEvidence.Assessment.IsDeleted,
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
            .EnableCache($"AssessmentAdvancedGraphSpecV6-{assessmentId}");
        }
    }

    public sealed class AssessmentAdvancedFilterSpec : Specification<Assessment>
    {
        public AssessmentAdvancedFilterSpec(
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
                        case "assessmentcode":
                            Query.Where(c => c.AssessmentCode.Contains(filter.Value));
                            break;
                        case "assessmentperiod":
                            Query.Where(c => c.AssessmentPeriod.Contains(filter.Value));
                            break;
                        case "assessmenttype":
                            if (int.TryParse(filter.Value, out int assessmenttype))
                            {
                                Query.Where(c => c.AssessmentType == assessmenttype);
                            }
                            break;
                        case "startdate":
                            if (DateTime.TryParse(filter.Value, out DateTime startdate))
                            {
                                Query.Where(c => c.StartDate >= startdate.AddHours(-6) && c.StartDate <= startdate.AddHours(6));
                            }
                            break;
                        case "enddate":
                            if (DateTime.TryParse(filter.Value, out DateTime enddate))
                            {
                                Query.Where(c => c.EndDate >= enddate.AddHours(-6) && c.EndDate <= enddate.AddHours(6));
                            }
                            break;
                        case "completiondate":
                            if (DateTime.TryParse(filter.Value, out DateTime completiondate))
                            {
                                Query.Where(c => c.CompletionDate >= completiondate.AddHours(-6) && c.CompletionDate <= completiondate.AddHours(6));
                            }
                            break;
                        case "rank":
                            if (int.TryParse(filter.Value, out int rank))
                            {
                                Query.Where(c => c.Rank == rank);
                            }
                            break;
                        case "compliancescore":
                            if (decimal.TryParse(filter.Value, out decimal compliancescore))
                            {
                               Query.Where(c => c.ComplianceScore >= compliancescore - 0.1m && c.ComplianceScore <= compliancescore + 0.1m);
                            }
                            break;
                        case "qsareviewrequired":
                            if (bool.TryParse(filter.Value, out bool qsareviewrequired))
                            {
                                Query.Where(c => c.QSAReviewRequired == qsareviewrequired);
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
                Query.OrderByDescending(x => x.AssessmentId);
            }

            Query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }

        private static IOrderedSpecificationBuilder<Assessment> ApplySort(
            ISpecificationBuilder<Assessment> query,
            Sort sort
        )
        {
            return sort.Direction == SortDirection.Descending
                ? query.OrderByDescending(GetSortProperty(sort.Field))
                : query.OrderBy(GetSortProperty(sort.Field));
        }

        private static IOrderedSpecificationBuilder<Assessment> ApplyAdditionalSort(
            IOrderedSpecificationBuilder<Assessment> query,
            Sort sort
        )
        {
            return sort.Direction == SortDirection.Descending
                ? query.ThenByDescending(GetSortProperty(sort.Field))
                : query.ThenBy(GetSortProperty(sort.Field));
        }

        private static Expression<Func<Assessment, object>> GetSortProperty(
            string propertyName
        )
        {
            return propertyName.ToLower() switch
            {
                "assessmentcode" => c => c.AssessmentCode,
                "assessmentperiod" => c => c.AssessmentPeriod,
                _ => c => c.AssessmentId,
            };
        }
    }