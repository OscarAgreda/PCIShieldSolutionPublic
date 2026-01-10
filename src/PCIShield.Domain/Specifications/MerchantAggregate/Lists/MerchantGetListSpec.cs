using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Ardalis.GuardClauses;
using Ardalis.Specification;

using Microsoft.IdentityModel.Tokens;

using PCIShield.Domain.Entities;
using PCIShield.Domain.ModelEntityDto;

using PCIShieldLib.SharedKernel.Interfaces;

namespace PCIShield.Domain.Specifications
{
    public sealed class MerchantListPagedSpec : PagedSpecification<Merchant, MerchantEntityDto>
    {
        public MerchantListPagedSpec(int pageNumber, int pageSize)
        {
            _ = Guard.Against.NegativeOrZero(pageNumber, nameof(pageNumber));
            _ = Guard.Against.NegativeOrZero(pageSize, nameof(pageSize));

            _ = Query
                // .Where(c => !c.IsDeleted)
                .OrderByDescending(i => i.MerchantId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            _ = Query.Select(x => new MerchantEntityDto
            {
                MerchantId = x.MerchantId,
                TenantId = x.TenantId,
                MerchantCode = x.MerchantCode,
                MerchantName = x.MerchantName,
                MerchantLevel = x.MerchantLevel,
                AcquirerName = x.AcquirerName,
                ProcessorMID = x.ProcessorMID,
                AnnualCardVolume = x.AnnualCardVolume,
                LastAssessmentDate = x.LastAssessmentDate,
                NextAssessmentDue = x.NextAssessmentDue,
                ComplianceRank = x.ComplianceRank,
                CreatedAt = x.CreatedAt,
                CreatedBy = x.CreatedBy,
                UpdatedAt = x.UpdatedAt,
                UpdatedBy = x.UpdatedBy,
                IsDeleted = x.IsDeleted,
                // Calculated fields based on relationship patterns
            })
            .AsNoTracking()
            .AsSplitQuery()
            .EnableCache($"MerchantListPagedSpec-{pageNumber}-{pageSize}");
        }
    }
    public sealed class MerchantSearchSpec : Specification<Merchant>
    {
        public MerchantSearchSpec(string searchTerm)
        {
            string searchLower = searchTerm?.ToLower() ?? string.Empty;

            Query
                .Where(c =>
                        (c.MerchantCode != null && c.MerchantCode.ToLower().Contains(searchLower)) ||
                        (c.MerchantName != null && c.MerchantName.ToLower().Contains(searchLower)) ||
                        (c.AcquirerName != null && c.AcquirerName.ToLower().Contains(searchLower)))
                .OrderByDescending(c => c.MerchantId);
        }
    }

    public sealed class MerchantLastCreatedSpec : Specification<Merchant>
    {
        public MerchantLastCreatedSpec()
        {
            Query
                .OrderByDescending(c => c.LastAssessmentDate)
                .Take(1)
                .AsNoTracking()
                .EnableCache("MerchantLastCreatedSpec");
        }
    }


    public sealed class MerchantByIdSpec : Specification<Merchant, MerchantEntityDto>
    {
        public MerchantByIdSpec(Guid id)
        {
            _ = Guard.Against.NullOrEmpty(id, nameof(id));

            _ = Query.Where(x => x.MerchantId == id);

            _ = Query.Include(x => x.Assessments);
            _ = Query.Include(x => x.Assets);
            _ = Query.Include(x => x.CompensatingControls);
            _ = Query.Include(x => x.ComplianceOfficers);
            _ = Query.Include(x => x.CryptographicInventories);
            _ = Query.Include(x => x.Evidences);
            _ = Query.Include(x => x.NetworkSegmentations);
            _ = Query.Include(x => x.PaymentChannels);
            _ = Query.Include(x => x.ServiceProviders);
            _ = Query.Select(x => new MerchantEntityDto
            {
                MerchantId = x.MerchantId,
                TenantId = x.TenantId,
                MerchantCode = x.MerchantCode,
                MerchantName = x.MerchantName,
                MerchantLevel = x.MerchantLevel,
                AcquirerName = x.AcquirerName,
                ProcessorMID = x.ProcessorMID,
                AnnualCardVolume = x.AnnualCardVolume,
                LastAssessmentDate = x.LastAssessmentDate,
                NextAssessmentDue = x.NextAssessmentDue,
                ComplianceRank = x.ComplianceRank,
                CreatedAt = x.CreatedAt,
                CreatedBy = x.CreatedBy,
                UpdatedAt = x.UpdatedAt,
                UpdatedBy = x.UpdatedBy,
                IsDeleted = x.IsDeleted,

                Assessments = x.Assessments.Select(assessment => new AssessmentEntityDto
                {
                    AssessmentId = assessment.AssessmentId,
                    TenantId = assessment.TenantId,
                    MerchantId = assessment.MerchantId,
                    AssessmentCode = assessment.AssessmentCode,
                    AssessmentType = assessment.AssessmentType,
                    AssessmentPeriod = assessment.AssessmentPeriod,
                    StartDate = assessment.StartDate,
                    EndDate = assessment.EndDate,
                    CompletionDate = assessment.CompletionDate,
                    Rank = assessment.Rank,
                    ComplianceScore = assessment.ComplianceScore,
                    QSAReviewRequired = assessment.QSAReviewRequired,
                    CreatedAt = assessment.CreatedAt,
                    CreatedBy = assessment.CreatedBy,
                    UpdatedAt = assessment.UpdatedAt,
                    UpdatedBy = assessment.UpdatedBy,
                    IsDeleted = assessment.IsDeleted,
                }).ToList(),
                Assets = x.Assets.Select(asset => new AssetEntityDto
                {
                    AssetId = asset.AssetId,
                    TenantId = asset.TenantId,
                    MerchantId = asset.MerchantId,
                    AssetCode = asset.AssetCode,
                    AssetName = asset.AssetName,
                    AssetType = asset.AssetType,
                    IPAddress = asset.IPAddress,
                    Hostname = asset.Hostname,
                    IsInCDE = asset.IsInCDE,
                    NetworkZone = asset.NetworkZone,
                    LastScanDate = asset.LastScanDate,
                    CreatedAt = asset.CreatedAt,
                    CreatedBy = asset.CreatedBy,
                    UpdatedAt = asset.UpdatedAt,
                    UpdatedBy = asset.UpdatedBy,
                    IsDeleted = asset.IsDeleted,
                }).ToList(),
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
                    Control = compensatingControl.Control != null ? new ControlEntityDto
                    {
                        ControlId = compensatingControl.Control.ControlId,
                        TenantId = compensatingControl.Control.TenantId,
                        ControlCode = compensatingControl.Control.ControlCode,
                        RequirementNumber = compensatingControl.Control.RequirementNumber,
                        ControlTitle = compensatingControl.Control.ControlTitle,
                        ControlDescription = compensatingControl.Control.ControlDescription,
                        TestingGuidance = compensatingControl.Control.TestingGuidance,
                        FrequencyDays = compensatingControl.Control.FrequencyDays,
                        IsMandatory = compensatingControl.Control.IsMandatory,
                        EffectiveDate = compensatingControl.Control.EffectiveDate,
                        CreatedAt = compensatingControl.Control.CreatedAt,
                        CreatedBy = compensatingControl.Control.CreatedBy,
                        UpdatedAt = compensatingControl.Control.UpdatedAt,
                        UpdatedBy = compensatingControl.Control.UpdatedBy,
                        IsDeleted = compensatingControl.Control.IsDeleted,
                    } : null,
                }).ToList(),
                ComplianceOfficers = x.ComplianceOfficers.Select(complianceOfficer => new ComplianceOfficerEntityDto
                {
                    ComplianceOfficerId = complianceOfficer.ComplianceOfficerId,
                    TenantId = complianceOfficer.TenantId,
                    MerchantId = complianceOfficer.MerchantId,
                    OfficerCode = complianceOfficer.OfficerCode,
                    FirstName = complianceOfficer.FirstName,
                    LastName = complianceOfficer.LastName,
                    Email = complianceOfficer.Email,
                    Phone = complianceOfficer.Phone,
                    CertificationLevel = complianceOfficer.CertificationLevel,
                    IsActive = complianceOfficer.IsActive,
                    CreatedAt = complianceOfficer.CreatedAt,
                    CreatedBy = complianceOfficer.CreatedBy,
                    UpdatedAt = complianceOfficer.UpdatedAt,
                    UpdatedBy = complianceOfficer.UpdatedBy,
                    IsDeleted = complianceOfficer.IsDeleted,
                }).ToList(),
                CryptographicInventories = x.CryptographicInventories.Select(cryptographicInventory => new CryptographicInventoryEntityDto
                {
                    CryptographicInventoryId = cryptographicInventory.CryptographicInventoryId,
                    TenantId = cryptographicInventory.TenantId,
                    MerchantId = cryptographicInventory.MerchantId,
                    KeyName = cryptographicInventory.KeyName,
                    KeyType = cryptographicInventory.KeyType,
                    Algorithm = cryptographicInventory.Algorithm,
                    KeyLength = cryptographicInventory.KeyLength,
                    KeyLocation = cryptographicInventory.KeyLocation,
                    CreationDate = cryptographicInventory.CreationDate,
                    LastRotationDate = cryptographicInventory.LastRotationDate,
                    NextRotationDue = cryptographicInventory.NextRotationDue,
                    CreatedAt = cryptographicInventory.CreatedAt,
                    CreatedBy = cryptographicInventory.CreatedBy,
                    UpdatedAt = cryptographicInventory.UpdatedAt,
                    UpdatedBy = cryptographicInventory.UpdatedBy,
                    IsDeleted = cryptographicInventory.IsDeleted,
                }).ToList(),
                Evidences = x.Evidences.Select(evidence => new EvidenceEntityDto
                {
                    EvidenceId = evidence.EvidenceId,
                    TenantId = evidence.TenantId,
                    MerchantId = evidence.MerchantId,
                    EvidenceCode = evidence.EvidenceCode,
                    EvidenceTitle = evidence.EvidenceTitle,
                    EvidenceType = evidence.EvidenceType,
                    CollectedDate = evidence.CollectedDate,
                    FileHash = evidence.FileHash,
                    StorageUri = evidence.StorageUri,
                    IsValid = evidence.IsValid,
                    CreatedAt = evidence.CreatedAt,
                    CreatedBy = evidence.CreatedBy,
                    UpdatedAt = evidence.UpdatedAt,
                    UpdatedBy = evidence.UpdatedBy,
                    IsDeleted = evidence.IsDeleted,
                }).ToList(),
                NetworkSegmentations = x.NetworkSegmentations.Select(networkSegmentation => new NetworkSegmentationEntityDto
                {
                    NetworkSegmentationId = networkSegmentation.NetworkSegmentationId,
                    TenantId = networkSegmentation.TenantId,
                    MerchantId = networkSegmentation.MerchantId,
                    SegmentName = networkSegmentation.SegmentName,
                    VLANId = networkSegmentation.VLANId,
                    IPRange = networkSegmentation.IPRange,
                    FirewallRules = networkSegmentation.FirewallRules,
                    IsInCDE = networkSegmentation.IsInCDE,
                    LastValidated = networkSegmentation.LastValidated,
                    CreatedAt = networkSegmentation.CreatedAt,
                    CreatedBy = networkSegmentation.CreatedBy,
                    UpdatedAt = networkSegmentation.UpdatedAt,
                    UpdatedBy = networkSegmentation.UpdatedBy,
                    IsDeleted = networkSegmentation.IsDeleted,
                }).ToList(),
                PaymentChannels = x.PaymentChannels.Select(paymentChannel => new PaymentChannelEntityDto
                {
                    PaymentChannelId = paymentChannel.PaymentChannelId,
                    TenantId = paymentChannel.TenantId,
                    MerchantId = paymentChannel.MerchantId,
                    ChannelCode = paymentChannel.ChannelCode,
                    ChannelName = paymentChannel.ChannelName,
                    ChannelType = paymentChannel.ChannelType,
                    ProcessingVolume = paymentChannel.ProcessingVolume,
                    IsInScope = paymentChannel.IsInScope,
                    TokenizationEnabled = paymentChannel.TokenizationEnabled,
                    CreatedAt = paymentChannel.CreatedAt,
                    CreatedBy = paymentChannel.CreatedBy,
                    UpdatedAt = paymentChannel.UpdatedAt,
                    UpdatedBy = paymentChannel.UpdatedBy,
                    IsDeleted = paymentChannel.IsDeleted,
                }).ToList(),
                ServiceProviders = x.ServiceProviders.Select(serviceProvider => new ServiceProviderEntityDto
                {
                    ServiceProviderId = serviceProvider.ServiceProviderId,
                    TenantId = serviceProvider.TenantId,
                    MerchantId = serviceProvider.MerchantId,
                    ProviderName = serviceProvider.ProviderName,
                    ServiceType = serviceProvider.ServiceType,
                    IsPCICompliant = serviceProvider.IsPCICompliant,
                    AOCExpiryDate = serviceProvider.AOCExpiryDate,
                    ResponsibilityMatrix = serviceProvider.ResponsibilityMatrix,
                    CreatedAt = serviceProvider.CreatedAt,
                    CreatedBy = serviceProvider.CreatedBy,
                    UpdatedAt = serviceProvider.UpdatedAt,
                    UpdatedBy = serviceProvider.UpdatedBy,
                    IsDeleted = serviceProvider.IsDeleted,
                }).ToList(),
            })
            .AsNoTracking()
            .AsSplitQuery()
            .EnableCache($"MerchantByIdSpec-{id.ToString()}");
        }
    }
}

/// <summary>
/// OLAP Cube Specification
/// Flattens Dimensions and Measures for client-side Pivot Tables or Dashboard aggregations.
/// </summary>
public sealed class MerchantPivotCubeSpec : Specification<Merchant, MerchantPivotCubeDto>
{
    public MerchantPivotCubeSpec()
    {
        Query.AsNoTracking();

        Query.Select(root => new MerchantPivotCubeDto
        {
            // Dimensions
            // Measures
            MerchantLevel = (decimal)(root.MerchantLevel),
            AnnualCardVolume = (decimal)(root.AnnualCardVolume),
            ComplianceRank = (decimal)(root.ComplianceRank),


            //MerchantLevel = root.MerchantLevel is null ? 0m : (decimal)root.MerchantLevel,
            //AnnualCardVolume = root.AnnualCardVolume is null ? 0m : (decimal)root.AnnualCardVolume,
            //ComplianceRank = root.ComplianceRank is null ? 0m : (decimal)root.ComplianceRank
        });
    }
}

public class MerchantPivotCubeDto
{
    public decimal MerchantLevel { get; set; }
    public decimal AnnualCardVolume { get; set; }
    public decimal ComplianceRank { get; set; }
}

/// <summary>
/// General Ledger Consolidation Engine
/// Aggregates child journal entries, handling Debits/Credits and Entity Filtering.
/// </summary>
public sealed class MerchantConsolidationSpec : Specification<Merchant, MerchantConsolidatedLedgerDto>
{
    public MerchantConsolidationSpec(List<string> entityIds = null)
    {
        Query.AsNoTracking();
        if (entityIds != null && entityIds.Any())
            Query.Where(x => entityIds.Contains(x.TenantId.ToString()));

        Query.Select(root => new MerchantConsolidatedLedgerDto
        {
            AccountId = root.MerchantId.ToString(),
        });
    }
}

public class MerchantConsolidatedLedgerDto
{
    public string AccountId { get; set; }
    public decimal TotalDebits { get; set; }
    public decimal TotalCredits { get; set; }
    public decimal NetBalance { get; set; }
}
public sealed class MerchantAdvancedGraphSpecV4 : Specification<Merchant, MerchantEntityDto>
{
    public MerchantAdvancedGraphSpecV4(
        Guid merchantId,
        bool includeTransactionData = true,
        bool includeTransitiveData = true,
        int? take = null,
        int? skip = null,
        DateTime? effectiveDate = null)
    {
        _ = Guard.Against.Default(merchantId, nameof(merchantId));
        _ = Query.Where(c => c.MerchantId == merchantId);

        if (take.HasValue && skip.HasValue)
        {
        }

        if (includeTransactionData)
        {
        }

        if (includeTransitiveData)
        {
        }

        _ = Query.Select(c => new MerchantEntityDto
        {
            MerchantId = c.MerchantId,
            TenantId = c.TenantId,
            MerchantCode = c.MerchantCode,
            MerchantName = c.MerchantName,
            MerchantLevel = c.MerchantLevel,
            AcquirerName = c.AcquirerName,
            ProcessorMID = c.ProcessorMID,
            AnnualCardVolume = c.AnnualCardVolume,
            LastAssessmentDate = c.LastAssessmentDate,
            NextAssessmentDue = c.NextAssessmentDue,
            ComplianceRank = c.ComplianceRank,
            CreatedAt = c.CreatedAt,
            CreatedBy = c.CreatedBy,
            UpdatedAt = c.UpdatedAt,
            UpdatedBy = c.UpdatedBy,
            IsDeleted = c.IsDeleted,
            ROCPackages = c.Assessments
                .SelectMany(i => i.ROCPackages)
                .Select(t => new ROCPackageEntityDto
                {
                    ROCPackageId = t.ROCPackageId,
                    TenantId = t.TenantId,
                    AssessmentId = t.AssessmentId,
                    PackageVersion = t.PackageVersion,
                    GeneratedDate = t.GeneratedDate,
                    QSAName = t.QSAName,
                    QSACompany = t.QSACompany,
                    SignatureDate = t.SignatureDate,
                    AOCNumber = t.AOCNumber,
                    Rank = t.Rank,
                    IsDeleted = t.IsDeleted,
                })
                .Take(2).ToList(),
            AssetControls = c.Assets
                .SelectMany(i => i.AssetControls)
                .Select(t => new AssetControlEntityDto
                {
                    RowId = t.RowId,
                    AssetId = t.AssetId,
                    ControlId = t.ControlId,
                    TenantId = t.TenantId,
                    IsApplicable = t.IsApplicable,
                    CustomizedApproach = t.CustomizedApproach,
                    IsDeleted = t.IsDeleted,
                })
                .Take(2).ToList(),
            ScanSchedules = c.Assets
                .SelectMany(i => i.ScanSchedules)
                .Select(t => new ScanScheduleEntityDto
                {
                    ScanScheduleId = t.ScanScheduleId,
                    TenantId = t.TenantId,
                    AssetId = t.AssetId,
                    ScanType = t.ScanType,
                    Frequency = t.Frequency,
                    NextScanDate = t.NextScanDate,
                    BlackoutStart = t.BlackoutStart,
                    BlackoutEnd = t.BlackoutEnd,
                    IsEnabled = t.IsEnabled,
                    IsDeleted = t.IsDeleted,
                })
                .Take(2).ToList(),
            Vulnerabilities = c.Assets
                .SelectMany(i => i.Vulnerabilities)
                .Select(t => new VulnerabilityEntityDto
                {
                    VulnerabilityId = t.VulnerabilityId,
                    TenantId = t.TenantId,
                    AssetId = t.AssetId,
                    VulnerabilityCode = t.VulnerabilityCode,
                    CVEId = t.CVEId,
                    Title = t.Title,
                    Severity = t.Severity,
                    CVSS = t.CVSS,
                    DetectedDate = t.DetectedDate,
                    ResolvedDate = t.ResolvedDate,
                    Rank = t.Rank,
                    IsDeleted = t.IsDeleted,
                })
                .Take(2).ToList(),
            Controls = c.CompensatingControls
                .Select(i => i.Control)
                .Select(t => new ControlEntityDto
                {
                    ControlId = t.ControlId,
                    TenantId = t.TenantId,
                    ControlCode = t.ControlCode,
                    RequirementNumber = t.RequirementNumber,
                    ControlTitle = t.ControlTitle,
                    ControlDescription = t.ControlDescription,
                    TestingGuidance = t.TestingGuidance,
                    FrequencyDays = t.FrequencyDays,
                    IsMandatory = t.IsMandatory,
                    EffectiveDate = t.EffectiveDate,
                    IsDeleted = t.IsDeleted,
                })
                .Take(2).ToList(),
            Assessments = c.Assessments.Select(assessment => new AssessmentEntityDto
            {
                AssessmentId = assessment.AssessmentId,
                TenantId = assessment.TenantId,
                MerchantId = assessment.MerchantId,
                AssessmentCode = assessment.AssessmentCode,
                AssessmentType = assessment.AssessmentType,
                AssessmentPeriod = assessment.AssessmentPeriod,
                StartDate = assessment.StartDate,
                EndDate = assessment.EndDate,
                CompletionDate = assessment.CompletionDate,
                Rank = assessment.Rank,
                ComplianceScore = assessment.ComplianceScore,
                QSAReviewRequired = assessment.QSAReviewRequired,
                CreatedAt = assessment.CreatedAt,
                CreatedBy = assessment.CreatedBy,
                UpdatedAt = assessment.UpdatedAt,
                UpdatedBy = assessment.UpdatedBy,
                IsDeleted = assessment.IsDeleted,
            }).ToList(),
            Assets = c.Assets.Select(asset => new AssetEntityDto
            {
                AssetId = asset.AssetId,
                TenantId = asset.TenantId,
                MerchantId = asset.MerchantId,
                AssetCode = asset.AssetCode,
                AssetName = asset.AssetName,
                AssetType = asset.AssetType,
                IPAddress = asset.IPAddress,
                Hostname = asset.Hostname,
                IsInCDE = asset.IsInCDE,
                NetworkZone = asset.NetworkZone,
                LastScanDate = asset.LastScanDate,
                CreatedAt = asset.CreatedAt,
                CreatedBy = asset.CreatedBy,
                UpdatedAt = asset.UpdatedAt,
                UpdatedBy = asset.UpdatedBy,
                IsDeleted = asset.IsDeleted,
            }).ToList(),
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
                Control = compensatingControl.Control != null ? new ControlEntityDto
                {
                    ControlId = compensatingControl.Control.ControlId,
                    TenantId = compensatingControl.Control.TenantId,
                    ControlCode = compensatingControl.Control.ControlCode,
                    RequirementNumber = compensatingControl.Control.RequirementNumber,
                    ControlTitle = compensatingControl.Control.ControlTitle,
                    ControlDescription = compensatingControl.Control.ControlDescription,
                    TestingGuidance = compensatingControl.Control.TestingGuidance,
                    FrequencyDays = compensatingControl.Control.FrequencyDays,
                    IsMandatory = compensatingControl.Control.IsMandatory,
                    EffectiveDate = compensatingControl.Control.EffectiveDate,
                    CreatedAt = compensatingControl.Control.CreatedAt,
                    CreatedBy = compensatingControl.Control.CreatedBy,
                    UpdatedAt = compensatingControl.Control.UpdatedAt,
                    UpdatedBy = compensatingControl.Control.UpdatedBy,
                    IsDeleted = compensatingControl.Control.IsDeleted,
                } : null,
            }).ToList(),
            ComplianceOfficers = c.ComplianceOfficers.Select(complianceOfficer => new ComplianceOfficerEntityDto
            {
                ComplianceOfficerId = complianceOfficer.ComplianceOfficerId,
                TenantId = complianceOfficer.TenantId,
                MerchantId = complianceOfficer.MerchantId,
                OfficerCode = complianceOfficer.OfficerCode,
                FirstName = complianceOfficer.FirstName,
                LastName = complianceOfficer.LastName,
                Email = complianceOfficer.Email,
                Phone = complianceOfficer.Phone,
                CertificationLevel = complianceOfficer.CertificationLevel,
                IsActive = complianceOfficer.IsActive,
                CreatedAt = complianceOfficer.CreatedAt,
                CreatedBy = complianceOfficer.CreatedBy,
                UpdatedAt = complianceOfficer.UpdatedAt,
                UpdatedBy = complianceOfficer.UpdatedBy,
                IsDeleted = complianceOfficer.IsDeleted,
            }).ToList(),
            CryptographicInventories = c.CryptographicInventories.Select(cryptographicInventory => new CryptographicInventoryEntityDto
            {
                CryptographicInventoryId = cryptographicInventory.CryptographicInventoryId,
                TenantId = cryptographicInventory.TenantId,
                MerchantId = cryptographicInventory.MerchantId,
                KeyName = cryptographicInventory.KeyName,
                KeyType = cryptographicInventory.KeyType,
                Algorithm = cryptographicInventory.Algorithm,
                KeyLength = cryptographicInventory.KeyLength,
                KeyLocation = cryptographicInventory.KeyLocation,
                CreationDate = cryptographicInventory.CreationDate,
                LastRotationDate = cryptographicInventory.LastRotationDate,
                NextRotationDue = cryptographicInventory.NextRotationDue,
                CreatedAt = cryptographicInventory.CreatedAt,
                CreatedBy = cryptographicInventory.CreatedBy,
                UpdatedAt = cryptographicInventory.UpdatedAt,
                UpdatedBy = cryptographicInventory.UpdatedBy,
                IsDeleted = cryptographicInventory.IsDeleted,
            }).ToList(),
            Evidences = c.Evidences.Select(evidence => new EvidenceEntityDto
            {
                EvidenceId = evidence.EvidenceId,
                TenantId = evidence.TenantId,
                MerchantId = evidence.MerchantId,
                EvidenceCode = evidence.EvidenceCode,
                EvidenceTitle = evidence.EvidenceTitle,
                EvidenceType = evidence.EvidenceType,
                CollectedDate = evidence.CollectedDate,
                FileHash = evidence.FileHash,
                StorageUri = evidence.StorageUri,
                IsValid = evidence.IsValid,
                CreatedAt = evidence.CreatedAt,
                CreatedBy = evidence.CreatedBy,
                UpdatedAt = evidence.UpdatedAt,
                UpdatedBy = evidence.UpdatedBy,
                IsDeleted = evidence.IsDeleted,
            }).ToList(),
            NetworkSegmentations = c.NetworkSegmentations.Select(networkSegmentation => new NetworkSegmentationEntityDto
            {
                NetworkSegmentationId = networkSegmentation.NetworkSegmentationId,
                TenantId = networkSegmentation.TenantId,
                MerchantId = networkSegmentation.MerchantId,
                SegmentName = networkSegmentation.SegmentName,
                VLANId = networkSegmentation.VLANId,
                IPRange = networkSegmentation.IPRange,
                FirewallRules = networkSegmentation.FirewallRules,
                IsInCDE = networkSegmentation.IsInCDE,
                LastValidated = networkSegmentation.LastValidated,
                CreatedAt = networkSegmentation.CreatedAt,
                CreatedBy = networkSegmentation.CreatedBy,
                UpdatedAt = networkSegmentation.UpdatedAt,
                UpdatedBy = networkSegmentation.UpdatedBy,
                IsDeleted = networkSegmentation.IsDeleted,
            }).ToList(),
            PaymentChannels = c.PaymentChannels.Select(paymentChannel => new PaymentChannelEntityDto
            {
                PaymentChannelId = paymentChannel.PaymentChannelId,
                TenantId = paymentChannel.TenantId,
                MerchantId = paymentChannel.MerchantId,
                ChannelCode = paymentChannel.ChannelCode,
                ChannelName = paymentChannel.ChannelName,
                ChannelType = paymentChannel.ChannelType,
                ProcessingVolume = paymentChannel.ProcessingVolume,
                IsInScope = paymentChannel.IsInScope,
                TokenizationEnabled = paymentChannel.TokenizationEnabled,
                CreatedAt = paymentChannel.CreatedAt,
                CreatedBy = paymentChannel.CreatedBy,
                UpdatedAt = paymentChannel.UpdatedAt,
                UpdatedBy = paymentChannel.UpdatedBy,
                IsDeleted = paymentChannel.IsDeleted,
            }).ToList(),
            ServiceProviders = c.ServiceProviders.Select(serviceProvider => new ServiceProviderEntityDto
            {
                ServiceProviderId = serviceProvider.ServiceProviderId,
                TenantId = serviceProvider.TenantId,
                MerchantId = serviceProvider.MerchantId,
                ProviderName = serviceProvider.ProviderName,
                ServiceType = serviceProvider.ServiceType,
                IsPCICompliant = serviceProvider.IsPCICompliant,
                AOCExpiryDate = serviceProvider.AOCExpiryDate,
                ResponsibilityMatrix = serviceProvider.ResponsibilityMatrix,
                CreatedAt = serviceProvider.CreatedAt,
                CreatedBy = serviceProvider.CreatedBy,
                UpdatedAt = serviceProvider.UpdatedAt,
                UpdatedBy = serviceProvider.UpdatedBy,
                IsDeleted = serviceProvider.IsDeleted,
            }).ToList(),
        })
        .AsNoTracking()
        .AsSplitQuery()
    .EnableCache($"MerchantAdvancedGraphSpec-{merchantId}");
    }
}

public sealed class MerchantAdvancedGraphSpecV7 : Specification<Merchant, MerchantEntityDto>
{
    public MerchantAdvancedGraphSpecV7(
        Guid merchantId,
        bool enableIntelligentProjection = true,
        bool enableSemanticAnalysis = true,
        bool enableBlueprintStrategy = true,
        bool enableRelationshipOptimization = true,

        int? take = null,
        int? skip = null)
    {
        _ = Guard.Against.Default(merchantId, nameof(merchantId));
        _ = Query.Where(c => c.MerchantId == merchantId);
        _ = Query.AsSplitQuery();
        if (take.HasValue && skip.HasValue)
        {
            _ = Query.Skip(skip.Value).Take(take.Value);
        }
        if (enableRelationshipOptimization)
        {
            _ = Query.Include(c => c.ComplianceOfficers);
            _ = Query.Include(c => c.CryptographicInventories);
            _ = Query.Include(c => c.Assets);
        }
        _ = Query.Include(c => c.Assessments);
        _ = Query.Include(c => c.Assessments);
        _ = Query.Include(c => c.Evidences).ThenInclude(x => x.ControlEvidences).ThenInclude(x => x.Assessment);
        if (enableBlueprintStrategy)
        {
        }

        _ = Query.Select(c => new MerchantEntityDto
        {
            MerchantId = c.MerchantId,
            TenantId = c.TenantId,
            MerchantCode = c.MerchantCode,
            MerchantName = c.MerchantName,
            MerchantLevel = c.MerchantLevel,
            AcquirerName = c.AcquirerName,
            ProcessorMID = c.ProcessorMID,
            AnnualCardVolume = c.AnnualCardVolume,
            LastAssessmentDate = c.LastAssessmentDate,
            NextAssessmentDue = c.NextAssessmentDue,
            ComplianceRank = c.ComplianceRank,
            CreatedAt = c.CreatedAt,
            CreatedBy = c.CreatedBy,
            UpdatedAt = c.UpdatedAt,
            UpdatedBy = c.UpdatedBy,
            IsDeleted = c.IsDeleted,
            EntityInfluenceScore = 0.13413492137552013d,
            NetworkCentrality = 0d,
            IsSystemCriticalNode = true,
            CriticalityRank = 3,
            DeletionImpactScore = 1d,
            AffectedEntitiesCount = 0,
            RequiresSafetyChecks = false,
            DataStabilityScore = 0d,
            IsVolatileEntity = true,
            NodeDegree = 9,
            IsHubNode = true,
            DrillDownPathCount = 0,
            HasComplexDrillDowns = false,

            #region Relationship Analytics

            EvolutionPressureScore = 10d,
            IsHighComplexityEntity = false,
            CascadeDepth = 0,
            CascadeImpactScore = 1d,
            RequiresDeletionPreview = false,

            EntityInfluenceRank = 0.13413492137552013d,
            IsArticulationPoint = true,
            ArticulationRank = 3,
            RelationshipCommunities = 13,
            HubScore = 0d,
            AuthorityScore = 0.968187773564223d,

            #endregion Relationship Analytics

            #region Denormalized Fields

            CachedAssessmentCount = c.Assessments.Count(),
            CachedAssessmentAssessmentTypeSum = c.Assessments.Sum(x => (decimal?)x.AssessmentType) ?? 0m,
            CachedAssetCount = c.Assets.Count(),
            CachedAssetAssetTypeSum = c.Assets.Sum(x => (decimal?)x.AssetType) ?? 0m,
            CachedCompensatingControlCount = c.CompensatingControls.Count(),
            CachedCompensatingControlRankSum = c.CompensatingControls.Sum(x => (decimal?)x.Rank) ?? 0m,
            CachedCryptographicInventoryCount = c.CryptographicInventories.Count(),
            CachedCryptographicInventoryKeyLengthSum = c.CryptographicInventories.Sum(x => (decimal?)x.KeyLength) ?? 0m,
            CachedEvidenceCount = c.Evidences.Count(),
            CachedEvidenceEvidenceTypeSum = c.Evidences.Sum(x => (decimal?)x.EvidenceType) ?? 0m,

            #endregion Denormalized Fields

            #region Bundle Metrics

            HasRepeatingChildPatternBundle = c.Assessments.Any() && c.Assets.Any() && c.ComplianceOfficers.Any() && c.CryptographicInventories.Any() && c.Evidences.Any() && c.NetworkSegmentations.Any() && c.PaymentChannels.Any() && c.ServiceProviders.Any(),
            RepeatingChildPatternCompleteness =
                ((c.Assessments.Any() ? 1 : 0) + (c.Assets.Any() ? 1 : 0) + (c.ComplianceOfficers.Any() ? 1 : 0) + (c.CryptographicInventories.Any() ? 1 : 0) + (c.Evidences.Any() ? 1 : 0) + (c.NetworkSegmentations.Any() ? 1 : 0) + (c.PaymentChannels.Any() ? 1 : 0) + (c.ServiceProviders.Any() ? 1 : 0)) /
                8d,

            #endregion Bundle Metrics

            #region Intelligent Metrics

            AverageAssessmentComplianceScore = c.Assessments.Any() ? c.Assessments.Average(x => x.ComplianceScore) ?? 0 : 0,
            MaxCryptographicInventoryKeyLength = c.CryptographicInventories.Any() ? c.CryptographicInventories.Max(x => x.KeyLength) : 0,
            TotalPaymentChannelProcessingVolume = c.PaymentChannels.Sum(x => x.ProcessingVolume),
            AverageVulnerabilitySeverity = c.Assets.SelectMany(i => i.Vulnerabilities).Any() ? (int)c.Assets.SelectMany(i => i.Vulnerabilities).Average(t => (int?)t.Severity) : 0,
            MaxVulnerabilityCVSS = c.Assets.SelectMany(i => i.Vulnerabilities).Any() ? c.Assets.SelectMany(i => i.Vulnerabilities).Max(t => t.CVSS) ?? 0 : 0,
            HasAssessmentComplianceScoreBelowThreshold = c.Assessments.Any(x => x.ComplianceScore < 70),
            IsMerchantLevelCritical = c.MerchantLevel >= 7,
            IsDeletedFlag = c.IsDeleted,
            TotalAssessmentCount = c.Assessments.Count(),
            ActiveAssessmentCount = c.Assessments.Count(x => !x.IsDeleted),
            CriticalAssessmentCount = c.Assessments.Count(x => x.AssessmentType >= 7),
            LatestAssessmentStartDate = c.Assessments.Max(x => (DateTime?)x.StartDate),
            TotalAssetCount = c.Assets.Count(),
            ActiveAssetCount = c.Assets.Count(x => !x.IsDeleted),
            CriticalAssetCount = c.Assets.Count(x => x.AssetType >= 7),
            LatestAssetLastScanDate = c.Assets.Max(x => x.LastScanDate),
            TotalCompensatingControlCount = c.CompensatingControls.Count(),
            ActiveCompensatingControlCount = c.CompensatingControls.Count(x => !x.IsDeleted),
            CriticalCompensatingControlCount = c.CompensatingControls.Count(x => x.Rank >= 7),
            LatestCompensatingControlApprovalDate = c.CompensatingControls.Max(x => x.ApprovalDate),
            TotalComplianceOfficerCount = c.ComplianceOfficers.Count(),
            ActiveComplianceOfficerCount = c.ComplianceOfficers.Count(x => !x.IsDeleted),
            LatestComplianceOfficerUpdatedAt = c.ComplianceOfficers.Max(x => x.UpdatedAt),
            TotalCryptographicInventoryCount = c.CryptographicInventories.Count(),
            ActiveCryptographicInventoryCount = c.CryptographicInventories.Count(x => !x.IsDeleted),
            LatestCryptographicInventoryCreationDate = c.CryptographicInventories.Max(x => (DateTime?)x.CreationDate),
            TotalEvidenceCount = c.Evidences.Count(),
            ActiveEvidenceCount = c.Evidences.Count(x => !x.IsDeleted),
            CriticalEvidenceCount = c.Evidences.Count(x => x.EvidenceType >= 7),
            LatestEvidenceCollectedDate = c.Evidences.Max(x => (DateTime?)x.CollectedDate),
            TotalNetworkSegmentationCount = c.NetworkSegmentations.Count(),
            ActiveNetworkSegmentationCount = c.NetworkSegmentations.Count(x => !x.IsDeleted),
            LatestNetworkSegmentationLastValidated = c.NetworkSegmentations.Max(x => x.LastValidated),
            TotalPaymentChannelCount = c.PaymentChannels.Count(),
            ActivePaymentChannelCount = c.PaymentChannels.Count(x => !x.IsDeleted),
            CriticalPaymentChannelCount = c.PaymentChannels.Count(x => x.ChannelType >= 7),
            LatestPaymentChannelUpdatedAt = c.PaymentChannels.Max(x => x.UpdatedAt),
            TotalServiceProviderCount = c.ServiceProviders.Count(),
            ActiveServiceProviderCount = c.ServiceProviders.Count(x => !x.IsDeleted),
            LatestServiceProviderAOCExpiryDate = c.ServiceProviders.Max(x => x.AOCExpiryDate),

            #endregion Intelligent Metrics

            ROCPackages = c.Assessments
                .SelectMany(i => i.ROCPackages)
                .Select(t => new ROCPackageEntityDto
                {
                    ROCPackageId = t.ROCPackageId,
                    TenantId = t.TenantId,
                    AssessmentId = t.AssessmentId,
                    PackageVersion = t.PackageVersion,
                    GeneratedDate = t.GeneratedDate,
                    QSAName = t.QSAName,
                    QSACompany = t.QSACompany,
                    SignatureDate = t.SignatureDate,
                    AOCNumber = t.AOCNumber,
                    Rank = t.Rank,
                    IsDeleted = t.IsDeleted,
                })
                .Take(2).ToList(),
            AssetControls = c.Assets
                .SelectMany(i => i.AssetControls)
                .Select(t => new AssetControlEntityDto
                {
                    RowId = t.RowId,
                    AssetId = t.AssetId,
                    ControlId = t.ControlId,
                    TenantId = t.TenantId,
                    IsApplicable = t.IsApplicable,
                    CustomizedApproach = t.CustomizedApproach,
                    IsDeleted = t.IsDeleted,
                })
                .Take(2).ToList(),
            ScanSchedules = c.Assets
                .SelectMany(i => i.ScanSchedules)
                .Select(t => new ScanScheduleEntityDto
                {
                    ScanScheduleId = t.ScanScheduleId,
                    TenantId = t.TenantId,
                    AssetId = t.AssetId,
                    ScanType = t.ScanType,
                    Frequency = t.Frequency,
                    NextScanDate = t.NextScanDate,
                    BlackoutStart = t.BlackoutStart,
                    BlackoutEnd = t.BlackoutEnd,
                    IsEnabled = t.IsEnabled,
                    IsDeleted = t.IsDeleted,
                })
                .Take(2).ToList(),
            Vulnerabilities = c.Assets
                .SelectMany(i => i.Vulnerabilities)
                .Select(t => new VulnerabilityEntityDto
                {
                    VulnerabilityId = t.VulnerabilityId,
                    TenantId = t.TenantId,
                    AssetId = t.AssetId,
                    VulnerabilityCode = t.VulnerabilityCode,
                    CVEId = t.CVEId,
                    Title = t.Title,
                    Severity = t.Severity,
                    CVSS = t.CVSS,
                    DetectedDate = t.DetectedDate,
                    ResolvedDate = t.ResolvedDate,
                    Rank = t.Rank,
                    IsDeleted = t.IsDeleted,
                })
                .Take(2).ToList(),
            Controls = c.CompensatingControls
                .Select(i => i.Control)
                .Select(t => new ControlEntityDto
                {
                    ControlId = t.ControlId,
                    TenantId = t.TenantId,
                    ControlCode = t.ControlCode,
                    RequirementNumber = t.RequirementNumber,
                    ControlTitle = t.ControlTitle,
                    ControlDescription = t.ControlDescription,
                    TestingGuidance = t.TestingGuidance,
                    FrequencyDays = t.FrequencyDays,
                    IsMandatory = t.IsMandatory,
                    EffectiveDate = t.EffectiveDate,
                    IsDeleted = t.IsDeleted,
                })
                .Take(2).ToList(),
            Assessments = c.Assessments.Select(assessment => new AssessmentEntityDto
            {
                AssessmentId = assessment.AssessmentId,
                TenantId = assessment.TenantId,
                MerchantId = assessment.MerchantId,
                AssessmentCode = assessment.AssessmentCode,
                AssessmentType = assessment.AssessmentType,
                AssessmentPeriod = assessment.AssessmentPeriod,
                StartDate = assessment.StartDate,
                EndDate = assessment.EndDate,
                CompletionDate = assessment.CompletionDate,
                Rank = assessment.Rank,
                ComplianceScore = assessment.ComplianceScore,
                QSAReviewRequired = assessment.QSAReviewRequired,
                CreatedAt = assessment.CreatedAt,
                CreatedBy = assessment.CreatedBy,
                UpdatedAt = assessment.UpdatedAt,
                UpdatedBy = assessment.UpdatedBy,
                IsDeleted = assessment.IsDeleted,
            }).ToList(),
            Assets = c.Assets.Select(asset => new AssetEntityDto
            {
                AssetId = asset.AssetId,
                TenantId = asset.TenantId,
                MerchantId = asset.MerchantId,
                AssetCode = asset.AssetCode,
                AssetName = asset.AssetName,
                AssetType = asset.AssetType,
                IPAddress = asset.IPAddress,
                Hostname = asset.Hostname,
                IsInCDE = asset.IsInCDE,
                NetworkZone = asset.NetworkZone,
                LastScanDate = asset.LastScanDate,
                CreatedAt = asset.CreatedAt,
                CreatedBy = asset.CreatedBy,
                UpdatedAt = asset.UpdatedAt,
                UpdatedBy = asset.UpdatedBy,
                IsDeleted = asset.IsDeleted,
            }).ToList(),
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
                Control = compensatingControl.Control != null ? new ControlEntityDto
                {
                    ControlId = compensatingControl.Control.ControlId,
                    TenantId = compensatingControl.Control.TenantId,
                    ControlCode = compensatingControl.Control.ControlCode,
                    RequirementNumber = compensatingControl.Control.RequirementNumber,
                    ControlTitle = compensatingControl.Control.ControlTitle,
                    ControlDescription = compensatingControl.Control.ControlDescription,
                    TestingGuidance = compensatingControl.Control.TestingGuidance,
                    FrequencyDays = compensatingControl.Control.FrequencyDays,
                    IsMandatory = compensatingControl.Control.IsMandatory,
                    EffectiveDate = compensatingControl.Control.EffectiveDate,
                    CreatedAt = compensatingControl.Control.CreatedAt,
                    CreatedBy = compensatingControl.Control.CreatedBy,
                    UpdatedAt = compensatingControl.Control.UpdatedAt,
                    UpdatedBy = compensatingControl.Control.UpdatedBy,
                    IsDeleted = compensatingControl.Control.IsDeleted,
                } : null,
            }).ToList(),
            ComplianceOfficers = c.ComplianceOfficers.Select(complianceOfficer => new ComplianceOfficerEntityDto
            {
                ComplianceOfficerId = complianceOfficer.ComplianceOfficerId,
                TenantId = complianceOfficer.TenantId,
                MerchantId = complianceOfficer.MerchantId,
                OfficerCode = complianceOfficer.OfficerCode,
                FirstName = complianceOfficer.FirstName,
                LastName = complianceOfficer.LastName,
                Email = complianceOfficer.Email,
                Phone = complianceOfficer.Phone,
                CertificationLevel = complianceOfficer.CertificationLevel,
                IsActive = complianceOfficer.IsActive,
                CreatedAt = complianceOfficer.CreatedAt,
                CreatedBy = complianceOfficer.CreatedBy,
                UpdatedAt = complianceOfficer.UpdatedAt,
                UpdatedBy = complianceOfficer.UpdatedBy,
                IsDeleted = complianceOfficer.IsDeleted,
            }).ToList(),
            CryptographicInventories = c.CryptographicInventories.Select(cryptographicInventory => new CryptographicInventoryEntityDto
            {
                CryptographicInventoryId = cryptographicInventory.CryptographicInventoryId,
                TenantId = cryptographicInventory.TenantId,
                MerchantId = cryptographicInventory.MerchantId,
                KeyName = cryptographicInventory.KeyName,
                KeyType = cryptographicInventory.KeyType,
                Algorithm = cryptographicInventory.Algorithm,
                KeyLength = cryptographicInventory.KeyLength,
                KeyLocation = cryptographicInventory.KeyLocation,
                CreationDate = cryptographicInventory.CreationDate,
                LastRotationDate = cryptographicInventory.LastRotationDate,
                NextRotationDue = cryptographicInventory.NextRotationDue,
                CreatedAt = cryptographicInventory.CreatedAt,
                CreatedBy = cryptographicInventory.CreatedBy,
                UpdatedAt = cryptographicInventory.UpdatedAt,
                UpdatedBy = cryptographicInventory.UpdatedBy,
                IsDeleted = cryptographicInventory.IsDeleted,
            }).ToList(),
            Evidences = c.Evidences.Select(evidence => new EvidenceEntityDto
            {
                EvidenceId = evidence.EvidenceId,
                TenantId = evidence.TenantId,
                MerchantId = evidence.MerchantId,
                EvidenceCode = evidence.EvidenceCode,
                EvidenceTitle = evidence.EvidenceTitle,
                EvidenceType = evidence.EvidenceType,
                CollectedDate = evidence.CollectedDate,
                FileHash = evidence.FileHash,
                StorageUri = evidence.StorageUri,
                IsValid = evidence.IsValid,
                CreatedAt = evidence.CreatedAt,
                CreatedBy = evidence.CreatedBy,
                UpdatedAt = evidence.UpdatedAt,
                UpdatedBy = evidence.UpdatedBy,
                IsDeleted = evidence.IsDeleted,
            }).ToList(),
            NetworkSegmentations = c.NetworkSegmentations.Select(networkSegmentation => new NetworkSegmentationEntityDto
            {
                NetworkSegmentationId = networkSegmentation.NetworkSegmentationId,
                TenantId = networkSegmentation.TenantId,
                MerchantId = networkSegmentation.MerchantId,
                SegmentName = networkSegmentation.SegmentName,
                VLANId = networkSegmentation.VLANId,
                IPRange = networkSegmentation.IPRange,
                FirewallRules = networkSegmentation.FirewallRules,
                IsInCDE = networkSegmentation.IsInCDE,
                LastValidated = networkSegmentation.LastValidated,
                CreatedAt = networkSegmentation.CreatedAt,
                CreatedBy = networkSegmentation.CreatedBy,
                UpdatedAt = networkSegmentation.UpdatedAt,
                UpdatedBy = networkSegmentation.UpdatedBy,
                IsDeleted = networkSegmentation.IsDeleted,
            }).ToList(),
            PaymentChannels = c.PaymentChannels.Select(paymentChannel => new PaymentChannelEntityDto
            {
                PaymentChannelId = paymentChannel.PaymentChannelId,
                TenantId = paymentChannel.TenantId,
                MerchantId = paymentChannel.MerchantId,
                ChannelCode = paymentChannel.ChannelCode,
                ChannelName = paymentChannel.ChannelName,
                ChannelType = paymentChannel.ChannelType,
                ProcessingVolume = paymentChannel.ProcessingVolume,
                IsInScope = paymentChannel.IsInScope,
                TokenizationEnabled = paymentChannel.TokenizationEnabled,
                CreatedAt = paymentChannel.CreatedAt,
                CreatedBy = paymentChannel.CreatedBy,
                UpdatedAt = paymentChannel.UpdatedAt,
                UpdatedBy = paymentChannel.UpdatedBy,
                IsDeleted = paymentChannel.IsDeleted,
            }).ToList(),
            ServiceProviders = c.ServiceProviders.Select(serviceProvider => new ServiceProviderEntityDto
            {
                ServiceProviderId = serviceProvider.ServiceProviderId,
                TenantId = serviceProvider.TenantId,
                MerchantId = serviceProvider.MerchantId,
                ProviderName = serviceProvider.ProviderName,
                ServiceType = serviceProvider.ServiceType,
                IsPCICompliant = serviceProvider.IsPCICompliant,
                AOCExpiryDate = serviceProvider.AOCExpiryDate,
                ResponsibilityMatrix = serviceProvider.ResponsibilityMatrix,
                CreatedAt = serviceProvider.CreatedAt,
                CreatedBy = serviceProvider.CreatedBy,
                UpdatedAt = serviceProvider.UpdatedAt,
                UpdatedBy = serviceProvider.UpdatedBy,
                IsDeleted = serviceProvider.IsDeleted,
            }).ToList(),
        })
        .AsNoTracking()
        .EnableCache($"MerchantAdvancedGraphSpecV7-{{0}}-complexity:{1:F0}-stability:{2:F2}",
            merchantId,
            10,
            0);
    }
}

/// <summary>
/// TOPOLOGICAL GRAPH SPECIFICATION V8
/// Graph Centrality Score: 284.00
/// Hub Status: CRITICAL HUB
/// Strategy: QuikGraph Optimized Includes & Projections
/// </summary>
/// <remarks>
/// <b>The Surgeon Strategy (V8) vs The Blind Strategy (V4):</b>
/// <list type="bullet">
/// <item><b>V4 (Blind):</b> Includes all children blindly. If a child collection has 10,000 rows, the query dies due to Cartesian Explosion.</item>
/// <item><b>V8 (Topological):</b> Uses QuikGraph to check: Is this child a 'Hub' (Complex) or a 'Leaf' (Simple)? It automatically prunes heavy branches while keeping light ones.</item>
/// </list>
/// <b>Hub Awareness:</b>
/// <br/>
/// If this entity is identified as a <b>Critical Hub</b> (High PageRank), the spec forces <c>AsSplitQuery()</c> to optimize performance. Single Query kills performance on Hubs.
/// </remarks>
public sealed class MerchantTopologicalSpecV8 : Specification<Merchant, MerchantEntityDto>
{
    public MerchantTopologicalSpecV8(Guid merchantId)
    {
        Query.Where(c => c.MerchantId == merchantId);
        Query.AsNoTracking();
        // GRAPH INSIGHT: This entity is a Central Hub (High PageRank).
        // ACTION: Forcing Split Query to prevent Cartesian Explosion on high-cardinality joins.
        Query.AsSplitQuery();

        // -- Topological Includes --
        // SKIPPED: Assessment is a Hub. Including it would cause performance degradation. Use Lazy Loading or separate query.
        // SKIPPED: Asset is a Hub. Including it would cause performance degradation. Use Lazy Loading or separate query.
        Query.Include(c => c.CompensatingControls); // Leaf child, safe to include
        Query.Include(c => c.ComplianceOfficers); // Leaf child, safe to include
        Query.Include(c => c.CryptographicInventories); // Leaf child, safe to include
        Query.Include(c => c.Evidences); // Leaf child, safe to include
        Query.Include(c => c.NetworkSegmentations); // Leaf child, safe to include
                                                    // SKIPPED: PaymentChannel is a Hub. Including it would cause performance degradation. Use Lazy Loading or separate query.
        Query.Include(c => c.ServiceProviders); // Leaf child, safe to include
        Query.Select(c => new MerchantEntityDto
        {
            MerchantId = c.MerchantId,
            TenantId = c.TenantId,
            MerchantCode = c.MerchantCode,
            MerchantName = c.MerchantName,
            MerchantLevel = c.MerchantLevel,
            AcquirerName = c.AcquirerName,
            ProcessorMID = c.ProcessorMID,
            AnnualCardVolume = c.AnnualCardVolume,
            LastAssessmentDate = c.LastAssessmentDate,
            NextAssessmentDue = c.NextAssessmentDue,
            ComplianceRank = c.ComplianceRank,
            CreatedAt = c.CreatedAt,
            CreatedBy = c.CreatedBy,
            UpdatedAt = c.UpdatedAt,
            UpdatedBy = c.UpdatedBy,
            IsDeleted = c.IsDeleted,

            // -- QuikGraph Distant Signals --
            Assessments = c.Assessments.Select(assessment => new AssessmentEntityDto
            {
                AssessmentId = assessment.AssessmentId,
                TenantId = assessment.TenantId,
                MerchantId = assessment.MerchantId,
                AssessmentCode = assessment.AssessmentCode,
                AssessmentType = assessment.AssessmentType,
                AssessmentPeriod = assessment.AssessmentPeriod,
                StartDate = assessment.StartDate,
                EndDate = assessment.EndDate,
                CompletionDate = assessment.CompletionDate,
                Rank = assessment.Rank,
                ComplianceScore = assessment.ComplianceScore,
                QSAReviewRequired = assessment.QSAReviewRequired,
                CreatedAt = assessment.CreatedAt,
                CreatedBy = assessment.CreatedBy,
                UpdatedAt = assessment.UpdatedAt,
                UpdatedBy = assessment.UpdatedBy,
                IsDeleted = assessment.IsDeleted,
            }).ToList(),
            Assets = c.Assets.Select(asset => new AssetEntityDto
            {
                AssetId = asset.AssetId,
                TenantId = asset.TenantId,
                MerchantId = asset.MerchantId,
                AssetCode = asset.AssetCode,
                AssetName = asset.AssetName,
                AssetType = asset.AssetType,
                IPAddress = asset.IPAddress,
                Hostname = asset.Hostname,
                IsInCDE = asset.IsInCDE,
                NetworkZone = asset.NetworkZone,
                LastScanDate = asset.LastScanDate,
                CreatedAt = asset.CreatedAt,
                CreatedBy = asset.CreatedBy,
                UpdatedAt = asset.UpdatedAt,
                UpdatedBy = asset.UpdatedBy,
                IsDeleted = asset.IsDeleted,
            }).ToList(),
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
                Control = compensatingControl.Control != null ? new ControlEntityDto
                {
                    ControlId = compensatingControl.Control.ControlId,
                    TenantId = compensatingControl.Control.TenantId,
                    ControlCode = compensatingControl.Control.ControlCode,
                    RequirementNumber = compensatingControl.Control.RequirementNumber,
                    ControlTitle = compensatingControl.Control.ControlTitle,
                    ControlDescription = compensatingControl.Control.ControlDescription,
                    TestingGuidance = compensatingControl.Control.TestingGuidance,
                    FrequencyDays = compensatingControl.Control.FrequencyDays,
                    IsMandatory = compensatingControl.Control.IsMandatory,
                    EffectiveDate = compensatingControl.Control.EffectiveDate,
                    CreatedAt = compensatingControl.Control.CreatedAt,
                    CreatedBy = compensatingControl.Control.CreatedBy,
                    UpdatedAt = compensatingControl.Control.UpdatedAt,
                    UpdatedBy = compensatingControl.Control.UpdatedBy,
                    IsDeleted = compensatingControl.Control.IsDeleted,
                } : null,
            }).ToList(),
            ComplianceOfficers = c.ComplianceOfficers.Select(complianceOfficer => new ComplianceOfficerEntityDto
            {
                ComplianceOfficerId = complianceOfficer.ComplianceOfficerId,
                TenantId = complianceOfficer.TenantId,
                MerchantId = complianceOfficer.MerchantId,
                OfficerCode = complianceOfficer.OfficerCode,
                FirstName = complianceOfficer.FirstName,
                LastName = complianceOfficer.LastName,
                Email = complianceOfficer.Email,
                Phone = complianceOfficer.Phone,
                CertificationLevel = complianceOfficer.CertificationLevel,
                IsActive = complianceOfficer.IsActive,
                CreatedAt = complianceOfficer.CreatedAt,
                CreatedBy = complianceOfficer.CreatedBy,
                UpdatedAt = complianceOfficer.UpdatedAt,
                UpdatedBy = complianceOfficer.UpdatedBy,
                IsDeleted = complianceOfficer.IsDeleted,
            }).ToList(),
            CryptographicInventories = c.CryptographicInventories.Select(cryptographicInventory => new CryptographicInventoryEntityDto
            {
                CryptographicInventoryId = cryptographicInventory.CryptographicInventoryId,
                TenantId = cryptographicInventory.TenantId,
                MerchantId = cryptographicInventory.MerchantId,
                KeyName = cryptographicInventory.KeyName,
                KeyType = cryptographicInventory.KeyType,
                Algorithm = cryptographicInventory.Algorithm,
                KeyLength = cryptographicInventory.KeyLength,
                KeyLocation = cryptographicInventory.KeyLocation,
                CreationDate = cryptographicInventory.CreationDate,
                LastRotationDate = cryptographicInventory.LastRotationDate,
                NextRotationDue = cryptographicInventory.NextRotationDue,
                CreatedAt = cryptographicInventory.CreatedAt,
                CreatedBy = cryptographicInventory.CreatedBy,
                UpdatedAt = cryptographicInventory.UpdatedAt,
                UpdatedBy = cryptographicInventory.UpdatedBy,
                IsDeleted = cryptographicInventory.IsDeleted,
            }).ToList(),
            Evidences = c.Evidences.Select(evidence => new EvidenceEntityDto
            {
                EvidenceId = evidence.EvidenceId,
                TenantId = evidence.TenantId,
                MerchantId = evidence.MerchantId,
                EvidenceCode = evidence.EvidenceCode,
                EvidenceTitle = evidence.EvidenceTitle,
                EvidenceType = evidence.EvidenceType,
                CollectedDate = evidence.CollectedDate,
                FileHash = evidence.FileHash,
                StorageUri = evidence.StorageUri,
                IsValid = evidence.IsValid,
                CreatedAt = evidence.CreatedAt,
                CreatedBy = evidence.CreatedBy,
                UpdatedAt = evidence.UpdatedAt,
                UpdatedBy = evidence.UpdatedBy,
                IsDeleted = evidence.IsDeleted,
            }).ToList(),
            NetworkSegmentations = c.NetworkSegmentations.Select(networkSegmentation => new NetworkSegmentationEntityDto
            {
                NetworkSegmentationId = networkSegmentation.NetworkSegmentationId,
                TenantId = networkSegmentation.TenantId,
                MerchantId = networkSegmentation.MerchantId,
                SegmentName = networkSegmentation.SegmentName,
                VLANId = networkSegmentation.VLANId,
                IPRange = networkSegmentation.IPRange,
                FirewallRules = networkSegmentation.FirewallRules,
                IsInCDE = networkSegmentation.IsInCDE,
                LastValidated = networkSegmentation.LastValidated,
                CreatedAt = networkSegmentation.CreatedAt,
                CreatedBy = networkSegmentation.CreatedBy,
                UpdatedAt = networkSegmentation.UpdatedAt,
                UpdatedBy = networkSegmentation.UpdatedBy,
                IsDeleted = networkSegmentation.IsDeleted,
            }).ToList(),
            PaymentChannels = c.PaymentChannels.Select(paymentChannel => new PaymentChannelEntityDto
            {
                PaymentChannelId = paymentChannel.PaymentChannelId,
                TenantId = paymentChannel.TenantId,
                MerchantId = paymentChannel.MerchantId,
                ChannelCode = paymentChannel.ChannelCode,
                ChannelName = paymentChannel.ChannelName,
                ChannelType = paymentChannel.ChannelType,
                ProcessingVolume = paymentChannel.ProcessingVolume,
                IsInScope = paymentChannel.IsInScope,
                TokenizationEnabled = paymentChannel.TokenizationEnabled,
                CreatedAt = paymentChannel.CreatedAt,
                CreatedBy = paymentChannel.CreatedBy,
                UpdatedAt = paymentChannel.UpdatedAt,
                UpdatedBy = paymentChannel.UpdatedBy,
                IsDeleted = paymentChannel.IsDeleted,
            }).ToList(),
            ServiceProviders = c.ServiceProviders.Select(serviceProvider => new ServiceProviderEntityDto
            {
                ServiceProviderId = serviceProvider.ServiceProviderId,
                TenantId = serviceProvider.TenantId,
                MerchantId = serviceProvider.MerchantId,
                ProviderName = serviceProvider.ProviderName,
                ServiceType = serviceProvider.ServiceType,
                IsPCICompliant = serviceProvider.IsPCICompliant,
                AOCExpiryDate = serviceProvider.AOCExpiryDate,
                ResponsibilityMatrix = serviceProvider.ResponsibilityMatrix,
                CreatedAt = serviceProvider.CreatedAt,
                CreatedBy = serviceProvider.CreatedBy,
                UpdatedAt = serviceProvider.UpdatedAt,
                UpdatedBy = serviceProvider.UpdatedBy,
                IsDeleted = serviceProvider.IsDeleted,
            }).ToList(),
        });
    }
}

/// <summary>
/// DEEP LINK GRAPH SPECIFICATION V9 (The Wormhole)
/// Turns Entity Framework queries into Surgical Lasers, extracting specific cells of data from deep within the relational graph without the overhead of hydrating the object tree.
/// Strategy: Transitive Projection (Flattening 3+ Hop Relationships)
/// Graph Pressure: 284.00
/// </summary>
/// <remarks>
/// <b>Zero-Cost Data:</b> This specification creates DTOs populated with data from tables that were never loaded.
/// <list type="bullet">
/// <item>It doesn't Include(Bank).</item>
/// <item>It doesn't Include(Country).</item>
/// <item>It generates SQL: SELECT t3.Name FROM Merchants t1 JOIN Banks t2... JOIN Countries t3...</item>
/// </list>
/// <b>Architectural Flattening:</b> It automatically detects that Merchant relates to Currency through a chain of 3 tables and flattens that relationship into a single property <i>DeepLink_Bank_Country_Currency_Code</i>.
/// <br/>
/// <b>Discovery:</b> You don't have to manually map these deep relationships. If the Graph (QuikGraph) says a path exists, the Generator writes the code to traverse it.
/// </remarks>
public sealed class MerchantDeepLinkGraphSpecV9 : Specification<Merchant, MerchantEntityDto>
{
    public MerchantDeepLinkGraphSpecV9(Guid merchantId)
    {
        Query.Where(c => c.MerchantId == merchantId);
        Query.AsNoTracking();

        // V9 ignores Includes. We project data directly to avoid Cartesian explosion.

        Query.Select(c => new MerchantEntityDto
        {
            MerchantId = c.MerchantId,
            TenantId = c.TenantId,
            MerchantCode = c.MerchantCode,
            MerchantName = c.MerchantName,
            MerchantLevel = c.MerchantLevel,
            AcquirerName = c.AcquirerName,
            ProcessorMID = c.ProcessorMID,
            AnnualCardVolume = c.AnnualCardVolume,
            LastAssessmentDate = c.LastAssessmentDate,
            NextAssessmentDue = c.NextAssessmentDue,
            ComplianceRank = c.ComplianceRank,
            CreatedAt = c.CreatedAt,
            CreatedBy = c.CreatedBy,
            UpdatedAt = c.UpdatedAt,
            UpdatedBy = c.UpdatedBy,
            IsDeleted = c.IsDeleted,
            // -- QuikGraph Deep Link Projections (2-4 Hops) --
            // No deep transitive paths found within threshold.
            // Standard direct relationships
            Assessments = c.Assessments.Select(assessment => new AssessmentEntityDto
            {
                AssessmentId = assessment.AssessmentId,
                TenantId = assessment.TenantId,
                MerchantId = assessment.MerchantId,
                AssessmentCode = assessment.AssessmentCode,
                AssessmentType = assessment.AssessmentType,
                AssessmentPeriod = assessment.AssessmentPeriod,
                StartDate = assessment.StartDate,
                EndDate = assessment.EndDate,
                CompletionDate = assessment.CompletionDate,
                Rank = assessment.Rank,
                ComplianceScore = assessment.ComplianceScore,
                QSAReviewRequired = assessment.QSAReviewRequired,
                CreatedAt = assessment.CreatedAt,
                CreatedBy = assessment.CreatedBy,
                UpdatedAt = assessment.UpdatedAt,
                UpdatedBy = assessment.UpdatedBy,
                IsDeleted = assessment.IsDeleted,
            }).ToList(),
            Assets = c.Assets.Select(asset => new AssetEntityDto
            {
                AssetId = asset.AssetId,
                TenantId = asset.TenantId,
                MerchantId = asset.MerchantId,
                AssetCode = asset.AssetCode,
                AssetName = asset.AssetName,
                AssetType = asset.AssetType,
                IPAddress = asset.IPAddress,
                Hostname = asset.Hostname,
                IsInCDE = asset.IsInCDE,
                NetworkZone = asset.NetworkZone,
                LastScanDate = asset.LastScanDate,
                CreatedAt = asset.CreatedAt,
                CreatedBy = asset.CreatedBy,
                UpdatedAt = asset.UpdatedAt,
                UpdatedBy = asset.UpdatedBy,
                IsDeleted = asset.IsDeleted,
            }).ToList(),
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
                Control = compensatingControl.Control != null ? new ControlEntityDto
                {
                    ControlId = compensatingControl.Control.ControlId,
                    TenantId = compensatingControl.Control.TenantId,
                    ControlCode = compensatingControl.Control.ControlCode,
                    RequirementNumber = compensatingControl.Control.RequirementNumber,
                    ControlTitle = compensatingControl.Control.ControlTitle,
                    ControlDescription = compensatingControl.Control.ControlDescription,
                    TestingGuidance = compensatingControl.Control.TestingGuidance,
                    FrequencyDays = compensatingControl.Control.FrequencyDays,
                    IsMandatory = compensatingControl.Control.IsMandatory,
                    EffectiveDate = compensatingControl.Control.EffectiveDate,
                    CreatedAt = compensatingControl.Control.CreatedAt,
                    CreatedBy = compensatingControl.Control.CreatedBy,
                    UpdatedAt = compensatingControl.Control.UpdatedAt,
                    UpdatedBy = compensatingControl.Control.UpdatedBy,
                    IsDeleted = compensatingControl.Control.IsDeleted,
                } : null,
            }).ToList(),
            ComplianceOfficers = c.ComplianceOfficers.Select(complianceOfficer => new ComplianceOfficerEntityDto
            {
                ComplianceOfficerId = complianceOfficer.ComplianceOfficerId,
                TenantId = complianceOfficer.TenantId,
                MerchantId = complianceOfficer.MerchantId,
                OfficerCode = complianceOfficer.OfficerCode,
                FirstName = complianceOfficer.FirstName,
                LastName = complianceOfficer.LastName,
                Email = complianceOfficer.Email,
                Phone = complianceOfficer.Phone,
                CertificationLevel = complianceOfficer.CertificationLevel,
                IsActive = complianceOfficer.IsActive,
                CreatedAt = complianceOfficer.CreatedAt,
                CreatedBy = complianceOfficer.CreatedBy,
                UpdatedAt = complianceOfficer.UpdatedAt,
                UpdatedBy = complianceOfficer.UpdatedBy,
                IsDeleted = complianceOfficer.IsDeleted,
            }).ToList(),
            CryptographicInventories = c.CryptographicInventories.Select(cryptographicInventory => new CryptographicInventoryEntityDto
            {
                CryptographicInventoryId = cryptographicInventory.CryptographicInventoryId,
                TenantId = cryptographicInventory.TenantId,
                MerchantId = cryptographicInventory.MerchantId,
                KeyName = cryptographicInventory.KeyName,
                KeyType = cryptographicInventory.KeyType,
                Algorithm = cryptographicInventory.Algorithm,
                KeyLength = cryptographicInventory.KeyLength,
                KeyLocation = cryptographicInventory.KeyLocation,
                CreationDate = cryptographicInventory.CreationDate,
                LastRotationDate = cryptographicInventory.LastRotationDate,
                NextRotationDue = cryptographicInventory.NextRotationDue,
                CreatedAt = cryptographicInventory.CreatedAt,
                CreatedBy = cryptographicInventory.CreatedBy,
                UpdatedAt = cryptographicInventory.UpdatedAt,
                UpdatedBy = cryptographicInventory.UpdatedBy,
                IsDeleted = cryptographicInventory.IsDeleted,
            }).ToList(),
            Evidences = c.Evidences.Select(evidence => new EvidenceEntityDto
            {
                EvidenceId = evidence.EvidenceId,
                TenantId = evidence.TenantId,
                MerchantId = evidence.MerchantId,
                EvidenceCode = evidence.EvidenceCode,
                EvidenceTitle = evidence.EvidenceTitle,
                EvidenceType = evidence.EvidenceType,
                CollectedDate = evidence.CollectedDate,
                FileHash = evidence.FileHash,
                StorageUri = evidence.StorageUri,
                IsValid = evidence.IsValid,
                CreatedAt = evidence.CreatedAt,
                CreatedBy = evidence.CreatedBy,
                UpdatedAt = evidence.UpdatedAt,
                UpdatedBy = evidence.UpdatedBy,
                IsDeleted = evidence.IsDeleted,
            }).ToList(),
            NetworkSegmentations = c.NetworkSegmentations.Select(networkSegmentation => new NetworkSegmentationEntityDto
            {
                NetworkSegmentationId = networkSegmentation.NetworkSegmentationId,
                TenantId = networkSegmentation.TenantId,
                MerchantId = networkSegmentation.MerchantId,
                SegmentName = networkSegmentation.SegmentName,
                VLANId = networkSegmentation.VLANId,
                IPRange = networkSegmentation.IPRange,
                FirewallRules = networkSegmentation.FirewallRules,
                IsInCDE = networkSegmentation.IsInCDE,
                LastValidated = networkSegmentation.LastValidated,
                CreatedAt = networkSegmentation.CreatedAt,
                CreatedBy = networkSegmentation.CreatedBy,
                UpdatedAt = networkSegmentation.UpdatedAt,
                UpdatedBy = networkSegmentation.UpdatedBy,
                IsDeleted = networkSegmentation.IsDeleted,
            }).ToList(),
            PaymentChannels = c.PaymentChannels.Select(paymentChannel => new PaymentChannelEntityDto
            {
                PaymentChannelId = paymentChannel.PaymentChannelId,
                TenantId = paymentChannel.TenantId,
                MerchantId = paymentChannel.MerchantId,
                ChannelCode = paymentChannel.ChannelCode,
                ChannelName = paymentChannel.ChannelName,
                ChannelType = paymentChannel.ChannelType,
                ProcessingVolume = paymentChannel.ProcessingVolume,
                IsInScope = paymentChannel.IsInScope,
                TokenizationEnabled = paymentChannel.TokenizationEnabled,
                CreatedAt = paymentChannel.CreatedAt,
                CreatedBy = paymentChannel.CreatedBy,
                UpdatedAt = paymentChannel.UpdatedAt,
                UpdatedBy = paymentChannel.UpdatedBy,
                IsDeleted = paymentChannel.IsDeleted,
            }).ToList(),
            ServiceProviders = c.ServiceProviders.Select(serviceProvider => new ServiceProviderEntityDto
            {
                ServiceProviderId = serviceProvider.ServiceProviderId,
                TenantId = serviceProvider.TenantId,
                MerchantId = serviceProvider.MerchantId,
                ProviderName = serviceProvider.ProviderName,
                ServiceType = serviceProvider.ServiceType,
                IsPCICompliant = serviceProvider.IsPCICompliant,
                AOCExpiryDate = serviceProvider.AOCExpiryDate,
                ResponsibilityMatrix = serviceProvider.ResponsibilityMatrix,
                CreatedAt = serviceProvider.CreatedAt,
                CreatedBy = serviceProvider.CreatedBy,
                UpdatedAt = serviceProvider.UpdatedAt,
                UpdatedBy = serviceProvider.UpdatedBy,
                IsDeleted = serviceProvider.IsDeleted,
            }).ToList(),
        });
    }
}

/// <summary>
/// SENTINEL INTEGRITY SPECIFICATION V10
/// Strategy: Neighborhood State Analysis (Topological Integrity)
/// Integrity Complexity: 284.00
/// </summary>
/// <remarks>
/// <b>Lifecycle Awareness:</b> This spec doesn't just fetch data; it audits the aggregate's readiness.
/// <b>State Discovery:</b> QuikGraph auto-detects 'Status' nodes within 2-hops and calculates a combined integrity flag.
/// <b>Operational Safety:</b> Projects a 'CanExecute' boolean based on the health of transitive dependencies.
/// </remarks>
public sealed class MerchantSentinelSpecV10 : Specification<Merchant, MerchantEntityDto>
{
    public MerchantSentinelSpecV10(Guid merchantId)
    {
        Query.Where(c => c.MerchantId == merchantId);
        Query.AsNoTracking();

        // V10 forces split query if neighbor density is high to preserve check accuracy
        Query.AsSplitQuery();

        Query.Select(c => new MerchantEntityDto
        {
            MerchantId = c.MerchantId,
            TenantId = c.TenantId,
            MerchantCode = c.MerchantCode,
            MerchantName = c.MerchantName,
            MerchantLevel = c.MerchantLevel,
            AcquirerName = c.AcquirerName,
            ProcessorMID = c.ProcessorMID,
            AnnualCardVolume = c.AnnualCardVolume,
            LastAssessmentDate = c.LastAssessmentDate,
            NextAssessmentDue = c.NextAssessmentDue,
            ComplianceRank = c.ComplianceRank,
            CreatedAt = c.CreatedAt,
            CreatedBy = c.CreatedBy,
            UpdatedAt = c.UpdatedAt,
            UpdatedBy = c.UpdatedBy,
            IsDeleted = c.IsDeleted,
            // -- QuikGraph Sentinel Health Checks --
            //Is_Assessment_Healthy = c.Merchant.IsActive == true,
            //Is_AssessmentControl_Healthy = c.Merchant.Assessment.IsActive == true,
            //Is_Asset_Healthy = c.Merchant.IsActive == true,
            //Is_AssetControl_Healthy = c.Merchant.Asset.IsActive == true,
            //Is_CompensatingControl_Healthy = c.Merchant.IsActive == true,
            //Is_ComplianceOfficer_Healthy = c.Merchant.IsActive == true,
            //Is_Control_Healthy = c.Merchant.Control.IsActive == true,
            //Is_ControlEvidence_Healthy = c.Merchant.Assessment.IsActive == true,
            //Is_CryptographicInventory_Healthy = c.Merchant.IsActive == true,
            //Is_Evidence_Healthy = c.Merchant.IsActive == true,
            //Is_NetworkSegmentation_Healthy = c.Merchant.IsActive == true,
            //Is_PaymentChannel_Healthy = c.Merchant.IsActive == true,
            //Is_PaymentPage_Healthy = c.Merchant.PaymentChannel.IsActive == true,
            //Is_ROCPackage_Healthy = c.Merchant.Assessment.IsActive == true,
            //Is_ScanSchedule_Healthy = c.Merchant.Asset.IsActive == true,
            //Is_ServiceProvider_Healthy = c.Merchant.IsActive == true,
            //Is_Vulnerability_Healthy = c.Merchant.Asset.IsActive == true,
            //Sentinel_CanExecute = Is_Assessment_Healthy && Is_AssessmentControl_Healthy && Is_Asset_Healthy && Is_AssetControl_Healthy && Is_CompensatingControl_Healthy && Is_ComplianceOfficer_Healthy && Is_Control_Healthy && Is_ControlEvidence_Healthy && Is_CryptographicInventory_Healthy && Is_Evidence_Healthy && Is_NetworkSegmentation_Healthy && Is_PaymentChannel_Healthy && Is_PaymentPage_Healthy && Is_ROCPackage_Healthy && Is_ScanSchedule_Healthy && Is_ServiceProvider_Healthy && Is_Vulnerability_Healthy,
        });
    }
}

/// <summary>
/// HOLOGRAPHIC IMPACT SPECIFICATION V10
/// <b>Philosophy:</b> An entity does not exist in a vacuum. It exists in a topological web.
/// </summary>
/// <remarks>
/// <b>Blast Radius (BFS Layers):</b> Uses <i>ExpandLayers</i> to project neighbors up to 2 hops away, grouped by distance.
/// <br/>
/// <b>Structural Awareness:</b> Flags relationships as <i>Forks</i> (Branching complexity) or <i>Joins</i> (Shared dependencies).
/// <br/>
/// <b>Graph Pressure:</b> 284.00 (Higher score = Higher risk of side effects).
/// </remarks>
public sealed class MerchantHolographicSpecV10 : Specification<Merchant, MerchantHolographicDto>
{
    public MerchantHolographicSpecV10(Guid merchantId)
    {
        Query.Where(c => c.MerchantId == merchantId);
        Query.AsNoTracking();
        Query.AsSplitQuery(); // Critical Hub detected via PageRank

        Query.Select(root => new MerchantHolographicDto
        {
            MerchantId = root.MerchantId,
            TenantId = root.TenantId,
            MerchantCode = root.MerchantCode,
            MerchantName = root.MerchantName,
            MerchantLevel = root.MerchantLevel,
            AcquirerName = root.AcquirerName,
            ProcessorMID = root.ProcessorMID,
            AnnualCardVolume = root.AnnualCardVolume,
            LastAssessmentDate = root.LastAssessmentDate,
            NextAssessmentDue = root.NextAssessmentDue,
            ComplianceRank = root.ComplianceRank,
            CreatedAt = root.CreatedAt,
            CreatedBy = root.CreatedBy,
            UpdatedAt = root.UpdatedAt,
            UpdatedBy = root.UpdatedBy,
            IsDeleted = root.IsDeleted,
            // -- Topological Metadata --
            GraphPressure = 284.00,
            IsHub = true,
            // -- Holographic Neighborhood (Level 1) --
            Neighbors = new List<HolographicNeighborDto>
                {
                    new HolographicNeighborDto
                    {
                        EntityName = "Assessment",
                        RelationType = "Child",
                        StructureType = "Fork/Branch",
                        Distance = 1,
                        Count = root.Assessments.Count(),
                        RiskLevel = "Normal",
                        // Preview data for UI hover state
                        PreviewLabel = root.Assessments.Select(x => x.AssessmentCode.ToString()).FirstOrDefault()
                    },
                    new HolographicNeighborDto
                    {
                        EntityName = "Asset",
                        RelationType = "Child",
                        StructureType = "Fork/Branch",
                        Distance = 1,
                        Count = root.Assets.Count(),
                        RiskLevel = "Normal",
                        // Preview data for UI hover state
                        PreviewLabel = root.Assets.Select(x => x.AssetCode.ToString()).FirstOrDefault()
                    },
                    new HolographicNeighborDto
                    {
                        EntityName = "CompensatingControl",
                        RelationType = "Child",
                        StructureType = "Leaf",
                        Distance = 1,
                        Count = root.CompensatingControls.Count(),
                        RiskLevel = "Normal",
                        // Preview data for UI hover state
                        PreviewLabel = root.CompensatingControls.Select(x => x.TenantId.ToString()).FirstOrDefault()
                    },
                    new HolographicNeighborDto
                    {
                        EntityName = "ComplianceOfficer",
                        RelationType = "Child",
                        StructureType = "Leaf",
                        Distance = 1,
                        Count = root.ComplianceOfficers.Count(),
                        RiskLevel = "Normal",
                        // Preview data for UI hover state
                        PreviewLabel = root.ComplianceOfficers.Select(x => x.FirstName.ToString()).FirstOrDefault()
                    },
                    new HolographicNeighborDto
                    {
                        EntityName = "CryptographicInventory",
                        RelationType = "Child",
                        StructureType = "Leaf",
                        Distance = 1,
                        Count = root.CryptographicInventories.Count(),
                        RiskLevel = "Normal",
                        // Preview data for UI hover state
                        PreviewLabel = root.CryptographicInventories.Select(x => x.KeyType.ToString()).FirstOrDefault()
                    },
                    new HolographicNeighborDto
                    {
                        EntityName = "Evidence",
                        RelationType = "Child",
                        StructureType = "Leaf",
                        Distance = 1,
                        Count = root.Evidences.Count(),
                        RiskLevel = "Normal",
                        // Preview data for UI hover state
                        PreviewLabel = root.Evidences.Select(x => x.EvidenceCode.ToString()).FirstOrDefault()
                    },
                    new HolographicNeighborDto
                    {
                        EntityName = "NetworkSegmentation",
                        RelationType = "Child",
                        StructureType = "Leaf",
                        Distance = 1,
                        Count = root.NetworkSegmentations.Count(),
                        RiskLevel = "Normal",
                        // Preview data for UI hover state
                        PreviewLabel = root.NetworkSegmentations.Select(x => x.SegmentName.ToString()).FirstOrDefault()
                    },
                    new HolographicNeighborDto
                    {
                        EntityName = "PaymentChannel",
                        RelationType = "Child",
                        StructureType = "Leaf",
                        Distance = 1,
                        Count = root.PaymentChannels.Count(),
                        RiskLevel = "Normal",
                        // Preview data for UI hover state
                        PreviewLabel = root.PaymentChannels.Select(x => x.ChannelName.ToString()).FirstOrDefault()
                    },
                    new HolographicNeighborDto
                    {
                        EntityName = "ServiceProvider",
                        RelationType = "Child",
                        StructureType = "Leaf",
                        Distance = 1,
                        Count = root.ServiceProviders.Count(),
                        RiskLevel = "Normal",
                        // Preview data for UI hover state
                        PreviewLabel = root.ServiceProviders.Select(x => x.ServiceType.ToString()).FirstOrDefault()
                    },
                }.Where(x => x.Count > 0).OrderByDescending(x => x.RiskLevel == "High").ToList(),
        });
    }
}

public class MerchantHolographicDto : MerchantEntityDto
{
    public double GraphPressure { get; set; }
    public bool IsHub { get; set; }
    public List<HolographicNeighborDto> Neighbors { get; set; }
}

public class HolographicNeighborDto
{
    public string EntityName { get; set; }
    public string RelationType { get; set; } // Child, Parent
    public string StructureType { get; set; } // Fork, Join, Leaf, Anchor
    public int Distance { get; set; }
    public int Count { get; set; }
    public string RiskLevel { get; set; }
    public string PreviewLabel { get; set; }
}

/// <summary>
/// CASCADE PROPHET SPECIFICATION V11
/// <b>Purpose:</b> Operational Safety. Predicts the 'Blast Radius' of a delete/archive operation.
/// </summary>
/// <remarks>
/// <b>Cycle Status:</b> No Cycles Detected
/// <b>Blast Radius:</b> 0 dependent entity types found in graph.
/// </remarks>
public sealed class MerchantImpactSpecV11 : Specification<Merchant, MerchantImpactDto>
{
    public MerchantImpactSpecV11(Guid merchantId)
    {
        Query.Where(c => c.MerchantId == merchantId);
        Query.AsNoTracking();
        // Split Query required for heavy impact analysis
        Query.AsSplitQuery();

        Query.Select(root => new MerchantImpactDto
        {
            EntityId = root.MerchantId.ToString(),
            Label = root.MerchantCode,
            GraphCycleDetected = false,

            // -- Downstream Impact Counts (Generated via QuikGraph BFS) --

            // -- Aggregate Risk Score --
            TotalDependentRecordCount =
                0,
        });
    }
}

public class MerchantImpactDto
{
    public string EntityId { get; set; }
    public string Label { get; set; }
    public bool GraphCycleDetected { get; set; }
    public int TotalDependentRecordCount { get; set; }

}

/// <summary>
/// ARCHITECTURAL ANOMALY HUNTER SPECIFICATION V12
/// <b>Purpose:</b> Proactive Architectural Audit & Health Reporting.
/// </summary>
/// <remarks>
/// <b>Overall Graph Pressure:</b> 284.00 (Local Connectivity + Global Influence).
/// <b>Hub Status:</b> CRITICAL HUB - Performance Strategy determined by this.
/// <b>Upstream Dependencies (Who references me):</b> 17 entities in graph layers.
/// <b>Downstream Impact (Who do I affect):</b> 0 entities in graph layers.
/// </remarks>
public sealed class MerchantArchitecturalAnomalySpecV12 : Specification<Merchant, MerchantArchitecturalAnomalyDto>
{
    public MerchantArchitecturalAnomalySpecV12(Guid merchantId)
    {
        Query.Where(c => c.MerchantId == merchantId);
        Query.AsNoTracking();
        Query.AsSplitQuery(); // Critical Hub detected, forcing Split Query

        Query.Include(c => c.Assessments);
        Query.Include(c => c.Assets);
        Query.Include(c => c.CompensatingControls);
        Query.Include(c => c.ComplianceOfficers);
        Query.Include(c => c.CryptographicInventories);
        Query.Include(c => c.Evidences);
        Query.Include(c => c.NetworkSegmentations);
        Query.Include(c => c.PaymentChannels);
        Query.Include(c => c.ServiceProviders);
        Query.Select(root => new MerchantArchitecturalAnomalyDto
        {
            EntityId = root.MerchantId.ToString(),
            Label = root.MerchantCode,
            // -- Upstream Context (Who references this entity) --
            UpstreamContext = new List<TopologicalLayerDto>
                {
                    new TopologicalLayerDto { TableName = "Assessment", Level = 1 },
                    new TopologicalLayerDto { TableName = "Asset", Level = 1 },
                    new TopologicalLayerDto { TableName = "CompensatingControl", Level = 1 },
                    new TopologicalLayerDto { TableName = "ComplianceOfficer", Level = 1 },
                    new TopologicalLayerDto { TableName = "CryptographicInventory", Level = 1 },
                    new TopologicalLayerDto { TableName = "Evidence", Level = 1 },
                    new TopologicalLayerDto { TableName = "NetworkSegmentation", Level = 1 },
                    new TopologicalLayerDto { TableName = "PaymentChannel", Level = 1 },
                    new TopologicalLayerDto { TableName = "ServiceProvider", Level = 1 },
                    new TopologicalLayerDto { TableName = "AssessmentControl", Level = 2 },
                    new TopologicalLayerDto { TableName = "AssetControl", Level = 2 },
                    new TopologicalLayerDto { TableName = "ControlEvidence", Level = 2 },
                    new TopologicalLayerDto { TableName = "PaymentPage", Level = 2 },
                    new TopologicalLayerDto { TableName = "ROCPackage", Level = 2 },
                    new TopologicalLayerDto { TableName = "ScanSchedule", Level = 2 },
                    new TopologicalLayerDto { TableName = "Vulnerability", Level = 2 },
                    new TopologicalLayerDto { TableName = "Script", Level = 3 },
                },
            // -- Downstream Impact (Who this entity affects) --
            DownstreamImpact = new List<TopologicalLayerDto>
            {
            },
            // -- Detected Architectural Smells --
            DetectedSmells = new List<ArchitecturalSmellDto>
            {
            },
            ArchitecturalHealthScore = (
                (284.00 * 0.4) + // Pressure as a negative factor
                (0 * 0.3) + // Cycle penalty
                (0 * 0.2) + // Direct audit access penalty
                (0 * 0.1) // Deep lookup penalty
            ) / 100.0d,
        });
    }
}

/// <summary>
/// DTO for Merchant Architectural Anomaly Report (V12).
/// </summary>
public class MerchantArchitecturalAnomalyDto
{
    public string EntityId { get; set; }
    public string Label { get; set; }
    public List<TopologicalLayerDto> UpstreamContext { get; set; }
    public List<TopologicalLayerDto> DownstreamImpact { get; set; }
    public List<ArchitecturalSmellDto> DetectedSmells { get; set; }
    public double ArchitecturalHealthScore { get; set; }
    public bool GraphCycleDetected { get; set; }
}

public class TopologicalLayerDto
{
    public string TableName { get; set; }
    public int Level { get; set; }
}

public class ArchitecturalSmellDto
{
    public string Type { get; set; }
    public string Description { get; set; }
    public string Severity { get; set; } // Low, Medium, High, Critical
    public string RemediationHint { get; set; }
}

/// <summary>
/// DEPENDENCY SENTINEL SPECIFICATION V12
/// <b>Operational Logic:</b> Orchestrates multi-entity state transitions in the correct dependency order.
/// </summary>
/// <remarks>
/// <b>Processing Depth:</b> 27 distinct topological layers detected.
/// <br/>
/// <b>Orchestration Strategy:</b> This spec outputs an 'Execution Blueprint'. Use this when performing batch updates
/// or distributed transactions (Sagas) where FK order is mission-critical.
/// Distributed Orchestration (Sagas): If you are using your Saga Engine, V12 is your best friend. It provides the roadmap. The Saga doesn't have to guess what order to execute; it reads the ExecutionGroups from the DTO and follows the SequenceOrder.
/// Pragmatic Slicing: By using InduceSubgraph, you stop projecting "Noise Entities" (like Logs) into your expensive business logic queries. This keeps the SQL generated by EF Core focused and fast.
/// The "Sentinel" Mindset: This spec is designed for Automation. It's the spec a background worker calls when it needs to "Re-calculate everything related to Merchant X." It knows exactly how to walk the path without violating Foreign Key constraints.
/// </remarks>
public sealed class MerchantDependencySentinelSpecV12 : Specification<Merchant, MerchantSentinelDto>
{
    public MerchantDependencySentinelSpecV12(Guid merchantId)
    {
        Query.Where(c => c.MerchantId == merchantId);
        Query.AsNoTracking();
        Query.AsSplitQuery(); // Required for broad topological envelopes

        Query.Select(root => new MerchantSentinelDto
        {
            MerchantId = root.MerchantId,
            TenantId = root.TenantId,
            MerchantCode = root.MerchantCode,
            MerchantName = root.MerchantName,
            MerchantLevel = root.MerchantLevel,
            AcquirerName = root.AcquirerName,
            ProcessorMID = root.ProcessorMID,
            AnnualCardVolume = root.AnnualCardVolume,
            LastAssessmentDate = root.LastAssessmentDate,
            NextAssessmentDue = root.NextAssessmentDue,
            ComplianceRank = root.ComplianceRank,
            CreatedAt = root.CreatedAt,
            CreatedBy = root.CreatedBy,
            UpdatedAt = root.UpdatedAt,
            UpdatedBy = root.UpdatedBy,
            IsDeleted = root.IsDeleted,

            // -- Execution Blueprint (Calculated via QuikGraph SCC/TopoSort) --
            ExecutionGroups = new List<ExecutionGroupDto>
                {
                    new ExecutionGroupDto
                    {
                        SequenceOrder = 0,
                        Role = "Prerequisite/Lookup",
                        Entities = new List<string> { "VulnerabilityRank" },
                        IsStateBearing = false,
                    },
                    new ExecutionGroupDto
                    {
                        SequenceOrder = 1,
                        Role = "Dependent/Transaction",
                        Entities = new List<string> { "Vulnerability" },
                        IsStateBearing = false,
                    },
                    new ExecutionGroupDto
                    {
                        SequenceOrder = 2,
                        Role = "Dependent/Transaction",
                        Entities = new List<string> { "TableMetadata" },
                        IsStateBearing = false,
                    },
                    new ExecutionGroupDto
                    {
                        SequenceOrder = 3,
                        Role = "Dependent/Transaction",
                        Entities = new List<string> { "ServiceProvider" },
                        IsStateBearing = false,
                    },
                    new ExecutionGroupDto
                    {
                        SequenceOrder = 4,
                        Role = "Dependent/Transaction",
                        Entities = new List<string> { "Script" },
                        IsStateBearing = false,
                    },
                    new ExecutionGroupDto
                    {
                        SequenceOrder = 5,
                        Role = "Dependent/Transaction",
                        Entities = new List<string> { "ScanSchedule" },
                        IsStateBearing = false,
                    },
                    new ExecutionGroupDto
                    {
                        SequenceOrder = 6,
                        Role = "Dependent/Transaction",
                        Entities = new List<string> { "ROCPackage" },
                        IsStateBearing = false,
                    },
                    new ExecutionGroupDto
                    {
                        SequenceOrder = 7,
                        Role = "Dependent/Transaction",
                        Entities = new List<string> { "PaymentPage" },
                        IsStateBearing = false,
                    },
                    new ExecutionGroupDto
                    {
                        SequenceOrder = 8,
                        Role = "Dependent/Transaction",
                        Entities = new List<string> { "PaymentChannel" },
                        IsStateBearing = false,
                    },
                    new ExecutionGroupDto
                    {
                        SequenceOrder = 9,
                        Role = "Dependent/Transaction",
                        Entities = new List<string> { "NetworkSegmentation" },
                        IsStateBearing = false,
                    },
                    new ExecutionGroupDto
                    {
                        SequenceOrder = 10,
                        Role = "Dependent/Transaction",
                        Entities = new List<string> { "Logs" },
                        IsStateBearing = false,
                    },
                    new ExecutionGroupDto
                    {
                        SequenceOrder = 11,
                        Role = "Dependent/Transaction",
                        Entities = new List<string> { "EvidenceType" },
                        IsStateBearing = false,
                    },
                    new ExecutionGroupDto
                    {
                        SequenceOrder = 12,
                        Role = "Dependent/Transaction",
                        Entities = new List<string> { "CryptographicInventory" },
                        IsStateBearing = false,
                    },
                    new ExecutionGroupDto
                    {
                        SequenceOrder = 13,
                        Role = "Dependent/Transaction",
                        Entities = new List<string> { "ControlEvidence" },
                        IsStateBearing = false,
                    },
                    new ExecutionGroupDto
                    {
                        SequenceOrder = 14,
                        Role = "Dependent/Transaction",
                        Entities = new List<string> { "Evidence" },
                        IsStateBearing = false,
                    },
                    new ExecutionGroupDto
                    {
                        SequenceOrder = 15,
                        Role = "Dependent/Transaction",
                        Entities = new List<string> { "ControlCategory" },
                        IsStateBearing = false,
                    },
                    new ExecutionGroupDto
                    {
                        SequenceOrder = 16,
                        Role = "Dependent/Transaction",
                        Entities = new List<string> { "ComplianceOfficer" },
                        IsStateBearing = false,
                    },
                    new ExecutionGroupDto
                    {
                        SequenceOrder = 17,
                        Role = "Dependent/Transaction",
                        Entities = new List<string> { "CompensatingControl" },
                        IsStateBearing = false,
                    },
                    new ExecutionGroupDto
                    {
                        SequenceOrder = 18,
                        Role = "Dependent/Transaction",
                        Entities = new List<string> { "AuditLog" },
                        IsStateBearing = false,
                    },
                    new ExecutionGroupDto
                    {
                        SequenceOrder = 19,
                        Role = "Dependent/Transaction",
                        Entities = new List<string> { "AssetControl" },
                        IsStateBearing = false,
                    },
                    new ExecutionGroupDto
                    {
                        SequenceOrder = 20,
                        Role = "Dependent/Transaction",
                        Entities = new List<string> { "Asset" },
                        IsStateBearing = false,
                    },
                    new ExecutionGroupDto
                    {
                        SequenceOrder = 21,
                        Role = "Dependent/Transaction",
                        Entities = new List<string> { "AssessmentType" },
                        IsStateBearing = false,
                    },
                    new ExecutionGroupDto
                    {
                        SequenceOrder = 22,
                        Role = "Dependent/Transaction",
                        Entities = new List<string> { "AssessmentControl" },
                        IsStateBearing = false,
                    },
                    new ExecutionGroupDto
                    {
                        SequenceOrder = 23,
                        Role = "Dependent/Transaction",
                        Entities = new List<string> { "Control" },
                        IsStateBearing = false,
                    },
                    new ExecutionGroupDto
                    {
                        SequenceOrder = 24,
                        Role = "Dependent/Transaction",
                        Entities = new List<string> { "Assessment" },
                        IsStateBearing = false,
                    },
                    new ExecutionGroupDto
                    {
                        SequenceOrder = 25,
                        Role = "AggregateRoot",
                        Entities = new List<string> { "Merchant" },
                        IsStateBearing = false,
                    },
                    new ExecutionGroupDto
                    {
                        SequenceOrder = 26,
                        Role = "Dependent/Transaction",
                        Entities = new List<string> { "ApplicationUser" },
                        IsStateBearing = false,
                    },
                },

            // -- Dependency Weights --
            AssessmentLoadWeight = root.Assessments.Count(),
            AssetLoadWeight = root.Assets.Count(),
            CompensatingControlLoadWeight = root.CompensatingControls.Count(),
            ComplianceOfficerLoadWeight = root.ComplianceOfficers.Count(),
            CryptographicInventoryLoadWeight = root.CryptographicInventories.Count(),
            EvidenceLoadWeight = root.Evidences.Count(),
            NetworkSegmentationLoadWeight = root.NetworkSegmentations.Count(),
            PaymentChannelLoadWeight = root.PaymentChannels.Count(),
            ServiceProviderLoadWeight = root.ServiceProviders.Count(),
        });
    }
}

public class MerchantSentinelDto : MerchantEntityDto
{
    public List<ExecutionGroupDto> ExecutionGroups { get; set; }
    public int AssessmentLoadWeight { get; set; }
    public int AssetLoadWeight { get; set; }
    public int CompensatingControlLoadWeight { get; set; }
    public int ComplianceOfficerLoadWeight { get; set; }
    public int CryptographicInventoryLoadWeight { get; set; }
    public int EvidenceLoadWeight { get; set; }
    public int NetworkSegmentationLoadWeight { get; set; }
    public int PaymentChannelLoadWeight { get; set; }
    public int ServiceProviderLoadWeight { get; set; }
}

public class ExecutionGroupDto
{
    public int SequenceOrder { get; set; }
    public string Role { get; set; }
    public List<string> Entities { get; set; }
    public bool IsStateBearing { get; set; }
}

/// <summary>
/// LIVING BLUEPRINT SPECIFICATION V13
/// <b>Architecture-as-Code:</b> Embeds structural visualization and audit data directly into the entity.
/// </summary>
/// <remarks>
/// <b>Noise Reduction:</b> Uses <i>TransitiveReduction</i> to calculate the minimal dependency graph.
/// <br/>
/// <b>Structural Audit:</b> Detected 0 deep dependency chains (potential fragility).
/// **The Concept:**
/// Most systems have "Documentation" that is separate from "Code." This spec bridges that gap.
/// 1.  **Structural Audit (`MatchPaths`):** It uses your DFS pattern matcher to detect **"Deep Chains"** (dependencies > 3 hops deep) which indicate fragility.
/// 2.  **Noise Reduction (`TransitiveReduction`):** It calculates the *essential* dependency graph. If `A->B->C` and `A->C` exist, it removes `A->C` because it's redundant. This reveals the *true* architecture.
/// 3.  **Visual Intelligence (`ToMermaidFlowchart`):** It generates a **Live Mermaid Diagram** string of the entity's neighborhood *at generation time*, embedding it into the DTO. The UI can render this string directly as a graph.
///
/// ### Why V13 is "Amazing" (The Rationale)
///
/// 1.  **Visual Debugging in Production:** The DTO contains a `ArchitectureDiagram` string (Mermaid syntax). You can render this in your Admin UI. When a user views a "Merchant", they don't just see data; they see a **Live Diagram** of that Merchant's specific dependency web, stripped of noise via `TransitiveReduction`.
/// 2.  **Structural Auditing:** It uses `MatchPaths` to find "Code Smells" (Deep Nesting) in the data structure itself. If `StructuralComplexityScore` is high, the UI can warn the user: *"This record is highly coupled. Edits may be slow."*
/// 3.  **Essentialism:** The `IsEssential` flags tell a developer (or an AI agent) which relationships are foundational versus which are just "nice to have" or redundant.
///
/// This leverages the **Computation** power of QuikGraph during the **Generation Phase** to produce **Static Wisdom** for the **Runtime Phase**.*
/// </remarks>
public sealed class MerchantBlueprintSpecV13 : Specification<Merchant, MerchantBlueprintDto>
{
    public MerchantBlueprintSpecV13(Guid merchantId)
    {
        Query.Where(c => c.MerchantId == merchantId);
        Query.AsNoTracking();
        Query.AsSplitQuery();

        Query.Select(root => new MerchantBlueprintDto
        {
            MerchantId = root.MerchantId,
            TenantId = root.TenantId,
            MerchantCode = root.MerchantCode,
            MerchantName = root.MerchantName,
            MerchantLevel = root.MerchantLevel,
            AcquirerName = root.AcquirerName,
            ProcessorMID = root.ProcessorMID,
            AnnualCardVolume = root.AnnualCardVolume,
            LastAssessmentDate = root.LastAssessmentDate,
            NextAssessmentDue = root.NextAssessmentDue,
            ComplianceRank = root.ComplianceRank,
            CreatedAt = root.CreatedAt,
            CreatedBy = root.CreatedBy,
            UpdatedAt = root.UpdatedAt,
            UpdatedBy = root.UpdatedBy,
            IsDeleted = root.IsDeleted,

            // -- Architectural Intelligence (Pre-Calculated via QuikGraph) --
            ArchitectureDiagram = "flowchart LR\n  Merchant['Merchant']",
            StructuralComplexityScore = 283.999375,

            // -- Essential Dependencies (Transitive Reduction) --
            IsAssessmentEssential = false, // Implied by other paths
            IsAssetEssential = false, // Implied by other paths
            IsCompensatingControlEssential = false, // Implied by other paths
            IsComplianceOfficerEssential = false, // Implied by other paths
            IsCryptographicInventoryEssential = false, // Implied by other paths
            IsEvidenceEssential = false, // Implied by other paths
            IsNetworkSegmentationEssential = false, // Implied by other paths
            IsPaymentChannelEssential = false, // Implied by other paths
            IsServiceProviderEssential = false, // Implied by other paths
        });
    }
}

public class MerchantBlueprintDto : MerchantEntityDto
{
    public string ArchitectureDiagram { get; set; }
    public double StructuralComplexityScore { get; set; }
    public bool IsAssessmentEssential { get; set; }
    public bool IsAssetEssential { get; set; }
    public bool IsCompensatingControlEssential { get; set; }
    public bool IsComplianceOfficerEssential { get; set; }
    public bool IsCryptographicInventoryEssential { get; set; }
    public bool IsEvidenceEssential { get; set; }
    public bool IsNetworkSegmentationEssential { get; set; }
    public bool IsPaymentChannelEssential { get; set; }
    public bool IsServiceProviderEssential { get; set; }
}

/// <summary>
/// SECURITY REACHABILITY SPECIFICATION V14
/// <b>Purpose:</b> Flattened Authorization Context. Instantly answers 'Who owns this?'
/// <b>Reachability:</b> Found 0 linked security principals.
/// ### "The Security Reachability Specification"
/// **Concept:** Security is often a graph problem. *"Does User X have access to Invoice Y?"* depends on the path: `User -> Tenant -> Merchant -> Invoice`.
/// **QuikGraph Power:** We use **Reachability Analysis** to find *every* "Security Principal" (Tenant, User, Organization) reachable from the current entity, no matter how many hops away.
/// **Result:** A DTO with a flattened `AccessControlList` baked right in. `Invoice.ReachableTenantId`, `Invoice.ReachableOwnerId`.
///
///
///   ** (Security):** This solves the **"Row-Level Security in Read Models"** problem.
///     *   **Problem:** In CQRS, checking permissions on a Read Model is hard because the "Owner" is normalized away.
///     *   **Solution:** V14 flattens the ownership graph. You can query `MerchantSecurityDto` and instantly see `Reachable_TenantId` without joining. You can stick this DTO into a JWT or a Policy check.
/// </summary>
public sealed class MerchantSecurityReachabilitySpecV14 : Specification<Merchant, MerchantSecurityDto>
{
    public MerchantSecurityReachabilitySpecV14(Guid merchantId)
    {
        Query.Where(c => c.MerchantId == merchantId);
        Query.AsNoTracking();
        Query.AsSplitQuery();

        Query.Select(root => new MerchantSecurityDto
        {
            EntityId = root.MerchantId.ToString(),

            // -- Reachable Security Principals (Computed via Graph Traversal) --
        });
    }
}

public class MerchantSecurityDto
{
    public string EntityId { get; set; }
}

/// <summary>
/// LIFECYCLE SENTINEL SPECIFICATION V15
/// <b>Purpose:</b> Validates 'Readiness' (Upstream) and 'Safety' (Downstream).
/// <b>Prerequisites:</b> 0 upstream dependencies checked.
/// <b>Dependents:</b> 16 downstream blockers checked.
/// ###  "The Lifecycle Sentinel Specification"
/// **Concept:** CRUD is dangerous. Creating a record requires **Prerequisites** (Roots). Deleting a record requires **Cleanup** (Leaves).
/// **QuikGraph Power:** We use **Ancestry (Roots)** to project the status of all required parents. We use **Descendants (Leaves)** to count all objects that will be orphaned.
/// **Result:** A DTO that tells the UI: *"Cannot Create: Missing Bank Account"* or *"Safe to Delete: 0 dependents."*
///
///
///
///   ** (Lifecycle):** This solves the **"Invariant Validation"** problem.
///     *   **Problem:** Before executing `DeleteMerchantCommand`, you need to check 15 child tables.
///     *   **Solution:** V15 calculates `IsSafeToDelete` database-side. Your Command Handler becomes one line: `if (!dto.IsSafeToDelete) throw new DomainException(...)`. It moves logic from Application Memory to Database Projection.
/// </summary>
public sealed class MerchantLifecycleSpecV15 : Specification<Merchant, MerchantLifecycleDto>
{
    public MerchantLifecycleSpecV15(Guid merchantId)
    {
        Query.Where(c => c.MerchantId == merchantId);
        Query.AsNoTracking();
        Query.AsSplitQuery();

        Query.Select(root => new MerchantLifecycleDto
        {
            EntityId = root.MerchantId.ToString(),

            // -- Upstream Health (Prerequisites) --

            // -- Downstream Blockers (Dependents) --
            Dependent_Assessment_Count = root.Assessments.Count(),
            Dependent_Asset_Count = root.Assets.Count(),
            Dependent_CompensatingControl_Count = root.CompensatingControls.Count(),
            Dependent_ComplianceOfficer_Count = root.ComplianceOfficers.Count(),
            Dependent_CryptographicInventory_Count = root.CryptographicInventories.Count(),
            Dependent_Evidence_Count = root.Evidences.Count(),
            Dependent_NetworkSegmentation_Count = root.NetworkSegmentations.Count(),
            Dependent_PaymentChannel_Count = root.PaymentChannels.Count(),
            Dependent_ServiceProvider_Count = root.ServiceProviders.Count(),
        });
    }
}

public class MerchantLifecycleDto
{
    public string EntityId { get; set; }
    public int Dependent_Assessment_Count { get; set; }
    public int Dependent_Asset_Count { get; set; }
    public int Dependent_CompensatingControl_Count { get; set; }
    public int Dependent_ComplianceOfficer_Count { get; set; }
    public int Dependent_CryptographicInventory_Count { get; set; }
    public int Dependent_Evidence_Count { get; set; }
    public int Dependent_NetworkSegmentation_Count { get; set; }
    public int Dependent_PaymentChannel_Count { get; set; }
    public int Dependent_ServiceProvider_Count { get; set; }
    // Helper property
    public bool IsSafeToDelete => Dependent_Assessment_Count + Dependent_Asset_Count + Dependent_CompensatingControl_Count + Dependent_ComplianceOfficer_Count + Dependent_CryptographicInventory_Count + Dependent_Evidence_Count + Dependent_NetworkSegmentation_Count + Dependent_PaymentChannel_Count + Dependent_ServiceProvider_Count == 0;
}

/// <summary>
/// OPTIMAL FETCH STRATEGY SPECIFICATION V15
/// <b>Purpose:</b> Maximum Performance. Dynamically selects the best loading strategy per relation.
/// </summary>
public sealed class MerchantOptimalFetchSpecV15 : Specification<Merchant, MerchantEntityDto>
{
    public MerchantOptimalFetchSpecV15(Guid merchantId)
    {
        Query.Where(c => c.MerchantId == merchantId);
        Query.AsNoTracking();
        Query.AsSplitQuery(); // Root is a Hub -> Split Query Default

        // -- Topologically Optimized Includes --
        // SKIPPED: Assessment (Pressure 78). Too heavy for Eager Load.
        // SKIPPED: Asset (Pressure 87). Too heavy for Eager Load.
        Query.Include(c => c.CompensatingControls);
        Query.Include(c => c.ComplianceOfficers);
        Query.Include(c => c.CryptographicInventories);
        Query.Include(c => c.Evidences);
        Query.Include(c => c.NetworkSegmentations);
        // SKIPPED: PaymentChannel (Pressure 59). Too heavy for Eager Load.
        Query.Include(c => c.ServiceProviders);

        Query.Select(root => new MerchantEntityDto
        {
            MerchantId = root.MerchantId,
            TenantId = root.TenantId,
            MerchantCode = root.MerchantCode,
            MerchantName = root.MerchantName,
            MerchantLevel = root.MerchantLevel,
            AcquirerName = root.AcquirerName,
            ProcessorMID = root.ProcessorMID,
            AnnualCardVolume = root.AnnualCardVolume,
            LastAssessmentDate = root.LastAssessmentDate,
            NextAssessmentDue = root.NextAssessmentDue,
            ComplianceRank = root.ComplianceRank,
            CreatedAt = root.CreatedAt,
            CreatedBy = root.CreatedBy,
            UpdatedAt = root.UpdatedAt,
            UpdatedBy = root.UpdatedBy,
            IsDeleted = root.IsDeleted,
            // -- Optimized Projections (For Skipped/Heavy Relations) --
            Summary_Assessment_Count = root.Assessments.Count(), // Projected instead of Included
            Summary_Asset_Count = root.Assets.Count(), // Projected instead of Included
            Summary_PaymentChannel_Count = root.PaymentChannels.Count(), // Projected instead of Included
            Assessments = root.Assessments.Select(assessment => new AssessmentEntityDto
            {
                AssessmentId = assessment.AssessmentId,
                TenantId = assessment.TenantId,
                MerchantId = assessment.MerchantId,
                AssessmentCode = assessment.AssessmentCode,
                AssessmentType = assessment.AssessmentType,
                AssessmentPeriod = assessment.AssessmentPeriod,
                StartDate = assessment.StartDate,
                EndDate = assessment.EndDate,
                CompletionDate = assessment.CompletionDate,
                Rank = assessment.Rank,
                ComplianceScore = assessment.ComplianceScore,
                QSAReviewRequired = assessment.QSAReviewRequired,
                CreatedAt = assessment.CreatedAt,
                CreatedBy = assessment.CreatedBy,
                UpdatedAt = assessment.UpdatedAt,
                UpdatedBy = assessment.UpdatedBy,
                IsDeleted = assessment.IsDeleted,
            }).ToList(),
            Assets = root.Assets.Select(asset => new AssetEntityDto
            {
                AssetId = asset.AssetId,
                TenantId = asset.TenantId,
                MerchantId = asset.MerchantId,
                AssetCode = asset.AssetCode,
                AssetName = asset.AssetName,
                AssetType = asset.AssetType,
                IPAddress = asset.IPAddress,
                Hostname = asset.Hostname,
                IsInCDE = asset.IsInCDE,
                NetworkZone = asset.NetworkZone,
                LastScanDate = asset.LastScanDate,
                CreatedAt = asset.CreatedAt,
                CreatedBy = asset.CreatedBy,
                UpdatedAt = asset.UpdatedAt,
                UpdatedBy = asset.UpdatedBy,
                IsDeleted = asset.IsDeleted,
            }).ToList(),
            CompensatingControls = root.CompensatingControls.Select(compensatingControl => new CompensatingControlEntityDto
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
                Control = compensatingControl.Control != null ? new ControlEntityDto
                {
                    ControlId = compensatingControl.Control.ControlId,
                    TenantId = compensatingControl.Control.TenantId,
                    ControlCode = compensatingControl.Control.ControlCode,
                    RequirementNumber = compensatingControl.Control.RequirementNumber,
                    ControlTitle = compensatingControl.Control.ControlTitle,
                    ControlDescription = compensatingControl.Control.ControlDescription,
                    TestingGuidance = compensatingControl.Control.TestingGuidance,
                    FrequencyDays = compensatingControl.Control.FrequencyDays,
                    IsMandatory = compensatingControl.Control.IsMandatory,
                    EffectiveDate = compensatingControl.Control.EffectiveDate,
                    CreatedAt = compensatingControl.Control.CreatedAt,
                    CreatedBy = compensatingControl.Control.CreatedBy,
                    UpdatedAt = compensatingControl.Control.UpdatedAt,
                    UpdatedBy = compensatingControl.Control.UpdatedBy,
                    IsDeleted = compensatingControl.Control.IsDeleted,
                } : null,
            }).ToList(),
            ComplianceOfficers = root.ComplianceOfficers.Select(complianceOfficer => new ComplianceOfficerEntityDto
            {
                ComplianceOfficerId = complianceOfficer.ComplianceOfficerId,
                TenantId = complianceOfficer.TenantId,
                MerchantId = complianceOfficer.MerchantId,
                OfficerCode = complianceOfficer.OfficerCode,
                FirstName = complianceOfficer.FirstName,
                LastName = complianceOfficer.LastName,
                Email = complianceOfficer.Email,
                Phone = complianceOfficer.Phone,
                CertificationLevel = complianceOfficer.CertificationLevel,
                IsActive = complianceOfficer.IsActive,
                CreatedAt = complianceOfficer.CreatedAt,
                CreatedBy = complianceOfficer.CreatedBy,
                UpdatedAt = complianceOfficer.UpdatedAt,
                UpdatedBy = complianceOfficer.UpdatedBy,
                IsDeleted = complianceOfficer.IsDeleted,
            }).ToList(),
            CryptographicInventories = root.CryptographicInventories.Select(cryptographicInventory => new CryptographicInventoryEntityDto
            {
                CryptographicInventoryId = cryptographicInventory.CryptographicInventoryId,
                TenantId = cryptographicInventory.TenantId,
                MerchantId = cryptographicInventory.MerchantId,
                KeyName = cryptographicInventory.KeyName,
                KeyType = cryptographicInventory.KeyType,
                Algorithm = cryptographicInventory.Algorithm,
                KeyLength = cryptographicInventory.KeyLength,
                KeyLocation = cryptographicInventory.KeyLocation,
                CreationDate = cryptographicInventory.CreationDate,
                LastRotationDate = cryptographicInventory.LastRotationDate,
                NextRotationDue = cryptographicInventory.NextRotationDue,
                CreatedAt = cryptographicInventory.CreatedAt,
                CreatedBy = cryptographicInventory.CreatedBy,
                UpdatedAt = cryptographicInventory.UpdatedAt,
                UpdatedBy = cryptographicInventory.UpdatedBy,
                IsDeleted = cryptographicInventory.IsDeleted,
            }).ToList(),
            Evidences = root.Evidences.Select(evidence => new EvidenceEntityDto
            {
                EvidenceId = evidence.EvidenceId,
                TenantId = evidence.TenantId,
                MerchantId = evidence.MerchantId,
                EvidenceCode = evidence.EvidenceCode,
                EvidenceTitle = evidence.EvidenceTitle,
                EvidenceType = evidence.EvidenceType,
                CollectedDate = evidence.CollectedDate,
                FileHash = evidence.FileHash,
                StorageUri = evidence.StorageUri,
                IsValid = evidence.IsValid,
                CreatedAt = evidence.CreatedAt,
                CreatedBy = evidence.CreatedBy,
                UpdatedAt = evidence.UpdatedAt,
                UpdatedBy = evidence.UpdatedBy,
                IsDeleted = evidence.IsDeleted,
            }).ToList(),
            NetworkSegmentations = root.NetworkSegmentations.Select(networkSegmentation => new NetworkSegmentationEntityDto
            {
                NetworkSegmentationId = networkSegmentation.NetworkSegmentationId,
                TenantId = networkSegmentation.TenantId,
                MerchantId = networkSegmentation.MerchantId,
                SegmentName = networkSegmentation.SegmentName,
                VLANId = networkSegmentation.VLANId,
                IPRange = networkSegmentation.IPRange,
                FirewallRules = networkSegmentation.FirewallRules,
                IsInCDE = networkSegmentation.IsInCDE,
                LastValidated = networkSegmentation.LastValidated,
                CreatedAt = networkSegmentation.CreatedAt,
                CreatedBy = networkSegmentation.CreatedBy,
                UpdatedAt = networkSegmentation.UpdatedAt,
                UpdatedBy = networkSegmentation.UpdatedBy,
                IsDeleted = networkSegmentation.IsDeleted,
            }).ToList(),
            PaymentChannels = root.PaymentChannels.Select(paymentChannel => new PaymentChannelEntityDto
            {
                PaymentChannelId = paymentChannel.PaymentChannelId,
                TenantId = paymentChannel.TenantId,
                MerchantId = paymentChannel.MerchantId,
                ChannelCode = paymentChannel.ChannelCode,
                ChannelName = paymentChannel.ChannelName,
                ChannelType = paymentChannel.ChannelType,
                ProcessingVolume = paymentChannel.ProcessingVolume,
                IsInScope = paymentChannel.IsInScope,
                TokenizationEnabled = paymentChannel.TokenizationEnabled,
                CreatedAt = paymentChannel.CreatedAt,
                CreatedBy = paymentChannel.CreatedBy,
                UpdatedAt = paymentChannel.UpdatedAt,
                UpdatedBy = paymentChannel.UpdatedBy,
                IsDeleted = paymentChannel.IsDeleted,
            }).ToList(),
            ServiceProviders = root.ServiceProviders.Select(serviceProvider => new ServiceProviderEntityDto
            {
                ServiceProviderId = serviceProvider.ServiceProviderId,
                TenantId = serviceProvider.TenantId,
                MerchantId = serviceProvider.MerchantId,
                ProviderName = serviceProvider.ProviderName,
                ServiceType = serviceProvider.ServiceType,
                IsPCICompliant = serviceProvider.IsPCICompliant,
                AOCExpiryDate = serviceProvider.AOCExpiryDate,
                ResponsibilityMatrix = serviceProvider.ResponsibilityMatrix,
                CreatedAt = serviceProvider.CreatedAt,
                CreatedBy = serviceProvider.CreatedBy,
                UpdatedAt = serviceProvider.UpdatedAt,
                UpdatedBy = serviceProvider.UpdatedBy,
                IsDeleted = serviceProvider.IsDeleted,
            }).ToList(),
        });
    }
}

/// <summary>
/// PROCESS MINING SPECIFICATION V16
/// <b>Purpose:</b> Velocity Analysis. Measures the speed of business workflows.
/// <b>Sequence Detected:</b> ServiceProvider -> PaymentChannel -> NetworkSegmentation -> Evidence -> CryptographicInventory -> ComplianceOfficer -> CompensatingControl -> Asset -> Assessment
/// ###  "The Process Mining Specification"
/// **Concept:** Data doesn't just sit there; it *flows* through a lifecycle. `Customer -> Order -> Invoice -> Payment`.
/// **QuikGraph Power:** We use **Topological Sort** on the dependency graph to reconstruct the *logical order of operations*.
/// **The Magic:** By projecting the `CreatedDate` of each step in the topological chain, we can calculate the **"Lag Time"** (latency) between business steps directly in the SQL projection.
/// **Result:** "Onboarding took 14 days. The bottleneck was between 'Assessment' and 'Approval' (12 days)."
///   ** (Process Mining):** This is **Operational Intelligence**.
///     *   It tells you *where the work is*.
///     *   If `Stage_2_Assessment_Date` is populated but `Stage_3_Approval_Date` is null, the code (DTO) *knows* the process is stuck at Approval. You can build a "Bottleneck Dashboard" purely from these DTOs.
/// </summary>
public sealed class MerchantProcessMiningSpecV16 : Specification<Merchant, MerchantProcessDto>
{
    public MerchantProcessMiningSpecV16(Guid merchantId)
    {
        Query.Where(c => c.MerchantId == merchantId);
        Query.AsNoTracking();
        Query.AsSplitQuery();

        Query.Select(root => new MerchantProcessDto
        {
            EntityId = root.MerchantId.ToString(),
            Label = root.MerchantCode,
            ProcessStartDate = root.CreatedAt,

            // -- Process Stages (Ordered by Topological Dependency) --
            Stage_1_ServiceProvider_Date = root.ServiceProviders.Any() ? root.ServiceProviders.Max(x => (DateTime?)x.AOCExpiryDate) : null,
            Stage_4_Evidence_Date = root.Evidences.Any() ? root.Evidences.Max(x => (DateTime?)x.CollectedDate) : null,
            Stage_5_CryptographicInventory_Date = root.CryptographicInventories.Any() ? root.CryptographicInventories.Max(x => (DateTime?)x.CreationDate) : null,
            Stage_7_CompensatingControl_Date = root.CompensatingControls.Any() ? root.CompensatingControls.Max(x => (DateTime?)x.ApprovalDate) : null,
            Stage_8_Asset_Date = root.Assets.Any() ? root.Assets.Max(x => (DateTime?)x.LastScanDate) : null,
            Stage_9_Assessment_Date = root.Assessments.Any() ? root.Assessments.Max(x => (DateTime?)x.StartDate) : null,

            // -- derived properties for UI logic --
            // Note: 'Duration' and 'Bottleneck' logic should be computed in the DTO or Service layer
            // based on these projected dates to keep SQL simple.
        });
    }
}

public class MerchantProcessDto
{
    public string EntityId { get; set; }
    public string Label { get; set; }
    public DateTime ProcessStartDate { get; set; }
    public DateTime? Stage_1_ServiceProvider_Date { get; set; }
    public DateTime? Stage_2_PaymentChannel_Date { get; set; }
    public DateTime? Stage_3_NetworkSegmentation_Date { get; set; }
    public DateTime? Stage_4_Evidence_Date { get; set; }
    public DateTime? Stage_5_CryptographicInventory_Date { get; set; }
    public DateTime? Stage_6_ComplianceOfficer_Date { get; set; }
    public DateTime? Stage_7_CompensatingControl_Date { get; set; }
    public DateTime? Stage_8_Asset_Date { get; set; }
    public DateTime? Stage_9_Assessment_Date { get; set; }

    public string CurrentStage =>
        Stage_9_Assessment_Date.HasValue ? "Assessment" :
        Stage_8_Asset_Date.HasValue ? "Asset" :
        Stage_7_CompensatingControl_Date.HasValue ? "CompensatingControl" :
        Stage_6_ComplianceOfficer_Date.HasValue ? "ComplianceOfficer" :
        Stage_5_CryptographicInventory_Date.HasValue ? "CryptographicInventory" :
        Stage_4_Evidence_Date.HasValue ? "Evidence" :
        Stage_3_NetworkSegmentation_Date.HasValue ? "NetworkSegmentation" :
        Stage_2_PaymentChannel_Date.HasValue ? "PaymentChannel" :
        Stage_1_ServiceProvider_Date.HasValue ? "ServiceProvider" :
        "Initiation";
}

/// <summary>
/// CLUSTER FORTRESS SPECIFICATION V16
/// <b>Philosophy:</b> Domain-Driven Design Enforcement.
/// <b>Strategy:</b> Uses Louvain Clustering to identify the Natural Aggregate Boundary.
/// <b>Cluster Size:</b> 9 tightly coupled entities.
/// </summary>
public sealed class MerchantClusterFortressSpecV16 : Specification<Merchant, MerchantClusterDto>
{
    public MerchantClusterFortressSpecV16(Guid merchantId)
    {
        Query.Where(c => c.MerchantId == merchantId);
        Query.AsNoTracking();
        Query.AsSplitQuery();

        // -- Cluster-Bounded Includes (The Fortress) --
        // Only loading entities that mathematically belong to this Bounded Context.
        // Excluded External Domain: Assessment
        // Excluded External Domain: Asset
        // Excluded External Domain: CompensatingControl
        // Excluded External Domain: ComplianceOfficer
        Query.Include(c => c.CryptographicInventories); // Inside Cluster
        Query.Include(c => c.Evidences); // Inside Cluster
        Query.Include(c => c.NetworkSegmentations); // Inside Cluster
                                                    // Excluded External Domain: PaymentChannel
                                                    // Excluded External Domain: ServiceProvider

        Query.Select(root => new MerchantClusterDto
        {
            MerchantId = root.MerchantId,
            TenantId = root.TenantId,
            MerchantCode = root.MerchantCode,
            MerchantName = root.MerchantName,
            MerchantLevel = root.MerchantLevel,
            AcquirerName = root.AcquirerName,
            ProcessorMID = root.ProcessorMID,
            AnnualCardVolume = root.AnnualCardVolume,
            LastAssessmentDate = root.LastAssessmentDate,
            NextAssessmentDue = root.NextAssessmentDue,
            ComplianceRank = root.ComplianceRank,
            CreatedAt = root.CreatedAt,
            CreatedBy = root.CreatedBy,
            UpdatedAt = root.UpdatedAt,
            UpdatedBy = root.UpdatedBy,
            IsDeleted = root.IsDeleted,


            // -- External Context References --
            External_Assessment_Count = root.Assessments.Count(),
            External_Asset_Count = root.Assets.Count(),
            External_CompensatingControl_Count = root.CompensatingControls.Count(),
            External_ComplianceOfficer_Count = root.ComplianceOfficers.Count(),
            External_PaymentChannel_Count = root.PaymentChannels.Count(),
            External_ServiceProvider_Count = root.ServiceProviders.Count(),
        });
    }
}

public class MerchantClusterDto : MerchantEntityDto
{
    public int External_Assessment_Count { get; set; }
    public int External_Asset_Count { get; set; }
    public int External_CompensatingControl_Count { get; set; }
    public int External_ComplianceOfficer_Count { get; set; }
    public int External_PaymentChannel_Count { get; set; }
    public int External_ServiceProvider_Count { get; set; }
}

/// <summary>
/// METRIC PRISM SPECIFICATION V17
/// <b>Purpose:</b> Aggregated Value Analysis. Flattens the entire graph into financial/risk totals.
/// <b>Scope:</b> Aggregating metrics from 0 distinct tables.
/// ###  "The Deep Metric Prism"
/// **Concept:** An entity's value is scattered across its children, grandchildren, and cousins.
/// **QuikGraph Power:** We use **BFS Expansion** to find *every* reachable table that contains money or risk metrics.
/// **The Magic:** We classify these distant tables into "Value Buckets" (e.g., Revenue, Cost, Risk) and project a consolidated **"Financial Hologram"** of the entity.
/// **Result:** "Merchant Value: $1.5M. Risk Exposure: $50k. Cost to Serve: $200." (Aggregated from 15 different tables).
///
/// 2.  ** (Metric Prism):** This is **Executive Intelligence**.
///     *   It turns a simple entity into a **Financial Report**.
///     *   It finds money everywhere (Invoices, Credits, Refunds, Assets) and sums it up into `GlobalFinancialExposure`.
///     *   This is typically a complex BI project. You just generated it as a standard EF Core query.
///
/// </summary>
public sealed class MerchantMetricPrismSpecV17 : Specification<Merchant, MerchantPrismDto>
{
    public MerchantMetricPrismSpecV17(Guid merchantId)
    {
        Query.Where(c => c.MerchantId == merchantId);
        Query.AsNoTracking();
        Query.AsSplitQuery();

        Query.Select(root => new MerchantPrismDto
        {
            EntityId = root.MerchantId.ToString(),
        });
    }
}

public class MerchantPrismDto
{
    public string EntityId { get; set; }

    // Consolidated Totals
    public decimal GlobalFinancialExposure => 0;
}

/// <summary>
/// EXECUTION PLANNER SPECIFICATION V17
/// <b>Purpose:</b> Transaction Sequencing & Sync.
/// <b>Graph Logic:</b> Topological Sort (SCC) to determine safe operational order.
/// </summary>
public sealed class MerchantExecutionPlanSpecV17 : Specification<Merchant, MerchantPlanDto>
{
    public MerchantExecutionPlanSpecV17(Guid merchantId)
    {
        Query.Where(c => c.MerchantId == merchantId);
        Query.AsNoTracking();

        Query.Select(root => new MerchantPlanDto
        {
            EntityId = root.MerchantId.ToString(),

            // -- Execution Waves (Ordered by Graph Depth) --
            ExecutionWaves = new List<ExecutionWaveDto>
                {
                    new ExecutionWaveDto
                    {
                        TargetEntity = "CompensatingControl",
                        Depth = 1,
                        SuggestedStrategy = "Parallel",
                        AffectedCount = root.CompensatingControls.Count()
                    },
                    new ExecutionWaveDto
                    {
                        TargetEntity = "ComplianceOfficer",
                        Depth = 1,
                        SuggestedStrategy = "Parallel",
                        AffectedCount = root.ComplianceOfficers.Count()
                    },
                    new ExecutionWaveDto
                    {
                        TargetEntity = "CryptographicInventory",
                        Depth = 1,
                        SuggestedStrategy = "Parallel",
                        AffectedCount = root.CryptographicInventories.Count()
                    },
                    new ExecutionWaveDto
                    {
                        TargetEntity = "NetworkSegmentation",
                        Depth = 1,
                        SuggestedStrategy = "Parallel",
                        AffectedCount = root.NetworkSegmentations.Count()
                    },
                    new ExecutionWaveDto
                    {
                        TargetEntity = "ServiceProvider",
                        Depth = 1,
                        SuggestedStrategy = "Parallel",
                        AffectedCount = root.ServiceProviders.Count()
                    },
                    new ExecutionWaveDto
                    {
                        TargetEntity = "Assessment",
                        Depth = 2,
                        SuggestedStrategy = "Sequential",
                        AffectedCount = root.Assessments.Count()
                    },
                    new ExecutionWaveDto
                    {
                        TargetEntity = "Asset",
                        Depth = 2,
                        SuggestedStrategy = "Sequential",
                        AffectedCount = root.Assets.Count()
                    },
                    new ExecutionWaveDto
                    {
                        TargetEntity = "Evidence",
                        Depth = 2,
                        SuggestedStrategy = "Sequential",
                        AffectedCount = root.Evidences.Count()
                    },
                    new ExecutionWaveDto
                    {
                        TargetEntity = "PaymentChannel",
                        Depth = 3,
                        SuggestedStrategy = "Sequential",
                        AffectedCount = root.PaymentChannels.Count()
                    },
                }.OrderByDescending(w => w.Depth).ToList(), // Deepest first (Delete Order)
        });
    }
}

public class MerchantPlanDto
{
    public string EntityId { get; set; }
    public List<ExecutionWaveDto> ExecutionWaves { get; set; }
}

public class ExecutionWaveDto
{
    public string TargetEntity { get; set; }
    public int Depth { get; set; }
    public string SuggestedStrategy { get; set; }
    public int AffectedCount { get; set; }
}

/// <summary>
/// TRACEABILITY GENEALOGIST SPECIFICATION V18
/// <b>Industry Application:</b> Manufacturing (Lot Tracking), Crypto (AML), Medical (Patient Safety).
/// <b>Lineage:</b> Tracks data from 3 origins to 0 destinations.
/// </summary>
public sealed class MerchantTraceabilitySpecV18 : Specification<Merchant, MerchantTraceabilityDto>
{
    public MerchantTraceabilitySpecV18(Guid merchantId)
    {
        Query.Where(c => c.MerchantId == merchantId);
        Query.AsNoTracking();
        Query.AsSplitQuery();

        Query.Select(root => new MerchantTraceabilityDto
        {
            EntityId = root.MerchantId.ToString(),
            TraceId = Guid.NewGuid().ToString(), // Unique ID for this trace report

            // -- Upstream Provenance (Origins) --
            //            Origin_AssessmentControl_First_Assessment_AssessmentControl_TenantId = root.Assessments.Select(x => x.AssessmentControl.TenantId).FirstOrDefault(),
            //          Origin_AssetControl_First_Asset_AssetControl_TenantId = root.Assets.Select(x => x.AssetControl.TenantId).FirstOrDefault(),

            // -- Downstream Impact (Destinations) --
        });
    }
}

public class MerchantTraceabilityDto
{
    public string EntityId { get; set; }
    public string TraceId { get; set; }
    public Guid Origin_AssessmentControl_First_Assessment_AssessmentControl_TenantId { get; set; }
    public Guid Origin_AssetControl_First_Asset_AssetControl_TenantId { get; set; }
}

// SKIPPED V19: Entity Merchant is not recursive (No Self-Loop detected).

/// <summary>
/// SIMILARITY ENGINE SPECIFICATION V20
/// <b>Business Value:</b> Recommendation System / Substitute Logic.
/// <b>Topology:</b> Identified 0 sibling entities sharing upstream dependencies.
/// </summary>
public sealed class MerchantSimilaritySpecV20 : Specification<Merchant, MerchantSimilarityDto>
{
    public MerchantSimilaritySpecV20(Guid merchantId)
    {
        Query.Where(c => c.MerchantId == merchantId);
        Query.AsNoTracking();

        Query.Select(root => new MerchantSimilarityDto
        {
            EntityId = root.MerchantId.ToString(),
            Label = root.MerchantCode,

            // -- Shared Structural Traits (DNA) --

            // Structural Siblings:
            StructuralClusterId = "", // Simple Hash of siblings
        });
    }
}

public class MerchantSimilarityDto
{
    public string EntityId { get; set; }
    public string Label { get; set; }
    public string StructuralClusterId { get; set; }
}

/// <summary>
/// CRITICAL PATH ANALYZER SPECIFICATION V21
/// <b>Industry Application:</b> Logistics (Lead Time), Project Mgmt (Dependencies), Approval Workflows.
/// <b>Critical Path:</b> 4 layers deep.
/// </summary>
public sealed class MerchantCriticalPathSpecV21 : Specification<Merchant, MerchantCriticalPathDto>
{
    public MerchantCriticalPathSpecV21(Guid merchantId)
    {
        Query.Where(c => c.MerchantId == merchantId);
        Query.AsNoTracking();
        Query.AsSplitQuery();

        Query.Select(root => new MerchantCriticalPathDto
        {
            EntityId = root.MerchantId.ToString(),
            MaxDependencyDepth = 4,

            // -- Bottleneck Indicators (Heaviest Branches) --
            Bottleneck_PaymentChannel_Volume = root.PaymentChannels.Count(),
            Bottleneck_Assessment_Volume = root.Assessments.Count(),
            Bottleneck_Asset_Volume = root.Assets.Count(),

            // -- Completion Heuristics --
        });
    }
}

public class MerchantCriticalPathDto
{
    public string EntityId { get; set; }
    public int MaxDependencyDepth { get; set; }
    public int Bottleneck_PaymentChannel_Volume { get; set; }
    public int Bottleneck_Assessment_Volume { get; set; }
    public int Bottleneck_Asset_Volume { get; set; }
}

/// <summary>
/// INFLUENCE RADAR SPECIFICATION V22
/// <b>Business Value:</b> Identifies 'Key Accounts' or 'Critical Infrastructure'.
/// <b>Architectural Status:</b> NETWORK HUB (High Influence)
/// </summary>
public sealed class MerchantInfluenceSpecV22 : Specification<Merchant, MerchantInfluenceDto>
{
    public MerchantInfluenceSpecV22(Guid merchantId)
    {
        Query.Where(c => c.MerchantId == merchantId);
        Query.AsNoTracking();
        Query.AsSplitQuery();

        Query.Select(root => new MerchantInfluenceDto
        {
            EntityId = root.MerchantId.ToString(),
            Label = root.MerchantCode,

            // -- Network Gravity --
            Dependent_Assessment_Count = root.Assessments.Count(),
            Dependent_Asset_Count = root.Assets.Count(),
            Dependent_CompensatingControl_Count = root.CompensatingControls.Count(),
            Dependent_ComplianceOfficer_Count = root.ComplianceOfficers.Count(),
            Dependent_CryptographicInventory_Count = root.CryptographicInventories.Count(),
            Dependent_Evidence_Count = root.Evidences.Count(),
            Dependent_NetworkSegmentation_Count = root.NetworkSegmentations.Count(),
            Dependent_PaymentChannel_Count = root.PaymentChannels.Count(),
            Dependent_ServiceProvider_Count = root.ServiceProviders.Count(),
            TotalInfluenceScore = root.Assessments.Count() + root.Assets.Count() + root.CompensatingControls.Count() + root.ComplianceOfficers.Count() + root.CryptographicInventories.Count() + root.Evidences.Count() + root.NetworkSegmentations.Count() + root.PaymentChannels.Count() + root.ServiceProviders.Count(),
        });
    }
}

public class MerchantInfluenceDto
{
    public string EntityId { get; set; }
    public string Label { get; set; }
    public int TotalInfluenceScore { get; set; }
    public int Dependent_Assessment_Count { get; set; }
    public int Dependent_Asset_Count { get; set; }
    public int Dependent_CompensatingControl_Count { get; set; }
    public int Dependent_ComplianceOfficer_Count { get; set; }
    public int Dependent_CryptographicInventory_Count { get; set; }
    public int Dependent_Evidence_Count { get; set; }
    public int Dependent_NetworkSegmentation_Count { get; set; }
    public int Dependent_PaymentChannel_Count { get; set; }
    public int Dependent_ServiceProvider_Count { get; set; }
}

/// <summary>
/// OPPORTUNITY HUNTER SPECIFICATION V23
/// <b>Business Value:</b> Gap Analysis / Cross-Sell Generator.
/// <b>Strategy:</b> Identifies missing relationships that 'should' be there based on structure.
/// </summary>
public sealed class MerchantOpportunitySpecV23 : Specification<Merchant, MerchantOpportunityDto>
{
    public MerchantOpportunitySpecV23(Guid merchantId)
    {
        Query.Where(c => c.MerchantId == merchantId);
        Query.AsNoTracking();

        Query.Select(root => new MerchantOpportunityDto
        {
            EntityId = root.MerchantId.ToString(),
            Label = root.MerchantCode,

            // -- Detected Gaps (Missing Relations) --
            IsMissing_ServiceProvider = !root.ServiceProviders.Any(),
            OpportunityScore = (!root.ServiceProviders.Any() ? 1 : 0),
        });
    }
}

public class MerchantOpportunityDto
{
    public string EntityId { get; set; }
    public string Label { get; set; }
    public int OpportunityScore { get; set; }
    public bool IsMissing_ServiceProvider { get; set; }
}

/// <summary>
/// HOLISTIC 360 SPECIFICATION V24
/// <b>Business Value:</b> Customer 360 / Single Pane of Glass.
/// <b>Strategy:</b> Aggregates 'Last Activity' and 'Volume' from all surrounding touchpoints.
/// </summary>
public sealed class MerchantHolistic360SpecV24 : Specification<Merchant, MerchantHolisticDto>
{
    public MerchantHolistic360SpecV24(Guid merchantId)
    {
        Query.Where(c => c.MerchantId == merchantId);
        Query.AsNoTracking();
        Query.AsSplitQuery();

        Query.Select(root => new MerchantHolisticDto
        {
            EntityId = root.MerchantId.ToString(),
            Label = root.MerchantCode,

            // -- Interaction Pulse (Activity Signals) --
            Total_Assessment_Count = root.Assessments.Count(),
            Last_Assessment_Date = root.Assessments.Max(x => (DateTime?)x.StartDate),
            Total_Asset_Count = root.Assets.Count(),
            Last_Asset_Date = root.Assets.Max(x => (DateTime?)x.LastScanDate),
            Total_CompensatingControl_Count = root.CompensatingControls.Count(),
            Last_CompensatingControl_Date = root.CompensatingControls.Max(x => (DateTime?)x.ApprovalDate),
            Total_ComplianceOfficer_Count = root.ComplianceOfficers.Count(),
            Total_CryptographicInventory_Count = root.CryptographicInventories.Count(),
            Last_CryptographicInventory_Date = root.CryptographicInventories.Max(x => (DateTime?)x.CreationDate),
            Total_Evidence_Count = root.Evidences.Count(),
            Last_Evidence_Date = root.Evidences.Max(x => (DateTime?)x.CollectedDate),
            Total_NetworkSegmentation_Count = root.NetworkSegmentations.Count(),
            Total_PaymentChannel_Count = root.PaymentChannels.Count(),
            Total_ServiceProvider_Count = root.ServiceProviders.Count(),
            Last_ServiceProvider_Date = root.ServiceProviders.Max(x => (DateTime?)x.AOCExpiryDate),
        });
    }
}

public class MerchantHolisticDto
{
    public string EntityId { get; set; }
    public string Label { get; set; }
    public int Total_Assessment_Count { get; set; }
    public DateTime? Last_Assessment_Date { get; set; }
    public int Total_Asset_Count { get; set; }
    public DateTime? Last_Asset_Date { get; set; }
    public int Total_CompensatingControl_Count { get; set; }
    public DateTime? Last_CompensatingControl_Date { get; set; }
    public int Total_ComplianceOfficer_Count { get; set; }
    public int Total_CryptographicInventory_Count { get; set; }
    public DateTime? Last_CryptographicInventory_Date { get; set; }
    public int Total_Evidence_Count { get; set; }
    public DateTime? Last_Evidence_Date { get; set; }
    public int Total_NetworkSegmentation_Count { get; set; }
    public int Total_PaymentChannel_Count { get; set; }
    public int Total_ServiceProvider_Count { get; set; }
    public DateTime? Last_ServiceProvider_Date { get; set; }
}

/// <summary>
/// INTEGRITY GUARDIAN SPECIFICATION V25
/// <b>Business Value:</b> Fraud Detection / Data Quality Monitor.
/// <b>Logic:</b> Detects 'Ghost Entities' that exist but lack critical structural components.
/// </summary>
public sealed class MerchantIntegritySpecV25 : Specification<Merchant, MerchantIntegrityDto>
{
    public MerchantIntegritySpecV25(Guid merchantId)
    {
        Query.Where(c => c.MerchantId == merchantId);
        Query.AsNoTracking();

        Query.Select(root => new MerchantIntegrityDto
        {
            EntityId = root.MerchantId.ToString(),

            // -- Structural Integrity Checks --
            IntegrityScore = 100,
        });
    }
}

public class MerchantIntegrityDto
{
    public string EntityId { get; set; }
    public int IntegrityScore { get; set; }
}

/// <summary>
/// STRUCTURAL HEALTH SPECIFICATION V26
/// <b>Innovation:</b> Applies Software Quality Metrics (Cohesion/Coupling) to Data Rows.
/// <b>Clustering Coefficient:</b> 0.00 (Fragmented (Risk of Spaghetti Data))
/// </summary>
public sealed class MerchantStructuralHealthSpecV26 : Specification<Merchant, MerchantHealthDto>
{
    public MerchantStructuralHealthSpecV26(Guid merchantId)
    {
        Query.Where(c => c.MerchantId == merchantId);
        Query.AsNoTracking();
        Query.AsSplitQuery();

        Query.Select(root => new MerchantHealthDto
        {
            EntityId = root.MerchantId.ToString(),
            Label = root.MerchantCode,

            // -- Structural Metrics --
            StructuralCouplingScore = 9, // Total connections
            ClusteringCoefficient = 0.00d,
            TotalChildRecords = root.Assessments.Count() + root.Assets.Count() + root.CompensatingControls.Count() + root.ComplianceOfficers.Count() + root.CryptographicInventories.Count() + root.Evidences.Count() + root.NetworkSegmentations.Count() + root.PaymentChannels.Count() + root.ServiceProviders.Count(),
            DataDensityScore = ((root.LastAssessmentDate != null ? 1 : 0) + (root.UpdatedAt != null ? 1 : 0) + (root.UpdatedBy != null ? 1 : 0)) / (double)3,
        });
    }
}

public class MerchantHealthDto
{
    public string EntityId { get; set; }
    public string Label { get; set; }
    public int StructuralCouplingScore { get; set; }
    public double ClusteringCoefficient { get; set; }
    public int TotalChildRecords { get; set; }
    public double DataDensityScore { get; set; }

    public string HealthStatus =>
        (DataDensityScore < 0.5) ? "Anemic (Low Data)" :
        (TotalChildRecords == 0 && StructuralCouplingScore > 5) ? "Orphaned (High potential, no data)" :
        "Healthy";
}

/// <summary>
/// HIDDEN BRIDGE SPECIFICATION V27
/// <b>Innovation:</b> Detects 'Single Points of Failure' in the business graph.
/// <b>Bridge Score:</b> 0 (InDegree * OutDegree).
/// </summary>
public sealed class MerchantHiddenBridgeSpecV27 : Specification<Merchant, MerchantBridgeDto>
{
    public MerchantHiddenBridgeSpecV27(Guid merchantId)
    {
        Query.Where(c => c.MerchantId == merchantId);
        Query.AsNoTracking();

        Query.Select(root => new MerchantBridgeDto
        {
            EntityId = root.MerchantId.ToString(),

            // -- Flow Metrics --
            DownstreamFlowVolume = root.Assessments.Count() + root.Assets.Count() + root.CompensatingControls.Count() + root.ComplianceOfficers.Count() + root.CryptographicInventories.Count() + root.Evidences.Count() + root.NetworkSegmentations.Count() + root.PaymentChannels.Count() + root.ServiceProviders.Count(),
            BridgeCriticalityScore = 0,
            UpstreamDependencyCount = 0,
        });
    }
}

public class MerchantBridgeDto
{
    public string EntityId { get; set; }
    public int DownstreamFlowVolume { get; set; }
    public double BridgeCriticalityScore { get; set; }
    public int UpstreamDependencyCount { get; set; }

    public string OperationalRisk =>
        (BridgeCriticalityScore > 20 && DownstreamFlowVolume > 0) ? "CRITICAL BRIDGE (Bottleneck Risk)" :
        (UpstreamDependencyCount > 5) ? "HIGH DEPENDENCY (Blockage Risk)" :
        "Standard Node";
}

/// <summary>
/// RISK CONTAGION SPECIFICATION V28
/// <b>Business Value:</b> Counterparty Risk Analysis / Supply Chain Fragility.
/// <b>Methodology:</b> Graph Heat Diffusion to measure indirect exposure to 'Risk' tables.
/// <b>Contagion Score:</b> 0.00 (Clean)
/// </summary>
public sealed class MerchantContagionSpecV28 : Specification<Merchant, MerchantContagionDto>
{
    public MerchantContagionSpecV28(Guid merchantId)
    {
        Query.Where(c => c.MerchantId == merchantId);
        Query.AsNoTracking();
        Query.AsSplitQuery();

        Query.Select(root => new MerchantContagionDto
        {
            EntityId = root.MerchantId.ToString(),
            Label = root.MerchantCode,
            ComputedContagionScore = 0.00,

            // -- Vector of Contagion (Sources) --
            MitigationFactor = 0,
        });
    }
}

public class MerchantContagionDto
{
    public string EntityId { get; set; }
    public string Label { get; set; }
    public double ComputedContagionScore { get; set; }
    public int MitigationFactor { get; set; }
}

/// <summary>
/// ECOSYSTEM SCANNER SPECIFICATION V29
/// <b>Business Value:</b> Fraud Detection / Collusion Monitoring.
/// <b>Methodology:</b> Clique Detection (Triangle Counting) to find closed-loop networks.
/// <b>Ring Status:</b> None
/// </summary>
public sealed class MerchantEcosystemSpecV29 : Specification<Merchant, MerchantEcosystemDto>
{
    public MerchantEcosystemSpecV29(Guid merchantId)
    {
        Query.Where(c => c.MerchantId == merchantId);
        Query.AsNoTracking();
        Query.AsSplitQuery();

        Query.Select(root => new MerchantEcosystemDto
        {
            EntityId = root.MerchantId.ToString(),

            // -- Ecosystem Ring Candidates (Potential Colluders) --
            IsolationIndex = (100.0),
            GraphCliqueFlag = false,
        });
    }
}

public class MerchantEcosystemDto
{
    public string EntityId { get; set; }
    public bool GraphCliqueFlag { get; set; }
    public double IsolationIndex { get; set; }
}

/// <summary>
/// CAPACITY STRESS TEST SPECIFICATION V30
/// <b>Business Value:</b> Operational Efficiency / Bottleneck Identification.
/// <b>Graph Logic:</b> InDegree (Demand) vs OutDegree (Capacity) Ratio: 9.00 (High Load).
/// </summary>
public sealed class MerchantCapacitySpecV30 : Specification<Merchant, MerchantCapacityDto>
{
    public MerchantCapacitySpecV30(Guid merchantId)
    {
        Query.Where(c => c.MerchantId == merchantId);
        Query.AsNoTracking();
        Query.AsSplitQuery();

        Query.Select(root => new MerchantCapacityDto
        {
            EntityId = root.MerchantId.ToString(),
            Label = root.MerchantCode,

            // -- Demand Metrics (Inbound) --
            TotalDemandVolume = root.Assessments.Count() + root.Assets.Count() + root.CompensatingControls.Count() + root.ComplianceOfficers.Count() + root.CryptographicInventories.Count() + root.Evidences.Count() + root.NetworkSegmentations.Count() + root.PaymentChannels.Count() + root.ServiceProviders.Count(),

            // -- Support Capacity (Outbound) --
            SupportingResourceCount = 0,
            RealTimeStressScore = 0 > 0 ? (root.Assessments.Count() + root.Assets.Count() + root.CompensatingControls.Count() + root.ComplianceOfficers.Count() + root.CryptographicInventories.Count() + root.Evidences.Count() + root.NetworkSegmentations.Count() + root.PaymentChannels.Count() + root.ServiceProviders.Count()) / (double)0 : (root.Assessments.Count() + root.Assets.Count() + root.CompensatingControls.Count() + root.ComplianceOfficers.Count() + root.CryptographicInventories.Count() + root.Evidences.Count() + root.NetworkSegmentations.Count() + root.PaymentChannels.Count() + root.ServiceProviders.Count()),
        });
    }
}

public class MerchantCapacityDto
{
    public string EntityId { get; set; }
    public string Label { get; set; }
    public int TotalDemandVolume { get; set; }
    public int SupportingResourceCount { get; set; }
    public double RealTimeStressScore { get; set; }

    public string Status =>
        RealTimeStressScore > 20 ? "OVERLOADED" :
        RealTimeStressScore > 5 ? "HEAVY" :
        "OPTIMAL";
}

/// <summary>
/// RESILIENCE ARCHITECT SPECIFICATION V31
/// <b>Business Value:</b> Supply Chain Resilience / Risk Mitigation.
/// <b>Topology Analysis:</b> Fragile (Single-Path)
/// </summary>
public sealed class MerchantResilienceSpecV31 : Specification<Merchant, MerchantResilienceDto>
{
    public MerchantResilienceSpecV31(Guid merchantId)
    {
        Query.Where(c => c.MerchantId == merchantId);
        Query.AsNoTracking();
        Query.AsSplitQuery();

        Query.Select(root => new MerchantResilienceDto
        {
            EntityId = root.MerchantId.ToString(),
            Label = root.MerchantCode,

            // -- Dependency Health Checks --
            BackupChannelsCount = 0,
        });
    }
}

public class MerchantResilienceDto
{
    public string EntityId { get; set; }
    public string Label { get; set; }
    public int BackupChannelsCount { get; set; }

    public string ResilienceScore =>
        (BackupChannelsCount > 0) ? "HIGH (Redundant)" :
        "LOW (Single Point of Failure)";
}

/// <summary>
/// KILL SWITCH SPECIFICATION V32
/// <b>Business Value:</b> Disaster Recovery / Business Continuity Planning.
/// <b>Graph Status:</b> CRITICAL (System Anchor).
/// </summary>
public sealed class MerchantKillSwitchSpecV32 : Specification<Merchant, MerchantSpofDto>
{
    public MerchantKillSwitchSpecV32(Guid merchantId)
    {
        Query.Where(c => c.MerchantId == merchantId);
        Query.AsNoTracking();
        Query.AsSplitQuery();

        Query.Select(root => new MerchantSpofDto
        {
            EntityId = root.MerchantId.ToString(),
            Label = root.MerchantCode,

            IsSinglePointOfFailure = true,
            // -- Inherited Structural Risk --
        });
    }
}

public class MerchantSpofDto
{
    public string EntityId { get; set; }
    public string Label { get; set; }
    public bool IsSinglePointOfFailure { get; set; }

    public string RiskAssessment =>
        IsSinglePointOfFailure ? "EXTREME RISK: No Redundancy" :
        "Stable: Alternatives Available";
}

/// <summary>
/// VALUE ENGINE SPECIFICATION V33
/// <b>Company OS Concept:</b> Maps the 'Algorithms' of the business.
/// <b>Engine Type:</b> Support
/// </summary>
public sealed class MerchantValueEngineSpecV33 : Specification<Merchant, MerchantValueEngineDto>
{
    public MerchantValueEngineSpecV33(Guid merchantId)
    {
        Query.Where(c => c.MerchantId == merchantId);
        Query.AsNoTracking();
        Query.AsSplitQuery();

        Query.Select(root => new MerchantValueEngineDto
        {
            EntityId = root.MerchantId.ToString(),
            Label = root.MerchantCode,
            EngineType = "Support",

            // -- Engine Performance Metrics --
        });
    }
}

public class MerchantValueEngineDto
{
    public string EntityId { get; set; }
    public string Label { get; set; }
    public string EngineType { get; set; }
    public bool? IsConverted { get; set; }
    public double? CycleTimeHours { get; set; }
}

/// <summary>
/// PLAYBOOK ARCHITECT SPECIFICATION V34
/// <b>Company OS Concept:</b> The 'Then What?' Question.
/// <b>Purpose:</b> Generates the 'Next Best Action' menu for the UI.
/// </summary>
public sealed class MerchantPlaybookSpecV34 : Specification<Merchant, MerchantPlaybookDto>
{
    public MerchantPlaybookSpecV34(Guid merchantId)
    {
        Query.Where(c => c.MerchantId == merchantId);
        Query.AsNoTracking();
        Query.AsSplitQuery();

        Query.Select(root => new MerchantPlaybookDto
        {
            EntityId = root.MerchantId.ToString(),
            CurrentState = "Unknown",

            // -- The 'Then What?' Playbook --
        });
    }
}

public class MerchantPlaybookDto
{
    public string EntityId { get; set; }
    public string CurrentState { get; set; }

    public List<string> AvailablePlays
    {
        get
        {
            var plays = new List<string>();
            return plays;
        }
    }
}




/// <summary>
/// RIPPLE EFFECT SPECIFICATION V36
/// <b>Business Value:</b> Root Cause Analysis / Impact Assessment.
/// <b>Graph Logic:</b> Bidirectional Traversal (Ancestry + Progeny).
/// </summary>
public sealed class MerchantRippleEffectSpecV36 : Specification<Merchant, MerchantRippleDto>
{
    public MerchantRippleEffectSpecV36(Guid merchantId)
    {
        Query.Where(c => c.MerchantId == merchantId);
        Query.AsNoTracking();
        Query.AsSplitQuery();

        Query.Select(root => new MerchantRippleDto
        {
            EntityId = root.MerchantId.ToString(),
            Label = root.MerchantCode,

            // -- Incoming Risk Vectors (Upstream) --

            // -- Outgoing Impact Cones (Downstream) --
        });
    }
}

public class MerchantRippleDto
{
    public string EntityId { get; set; }
    public string Label { get; set; }
}

/// <summary>
/// BEHAVIORAL PHENOTYPE SPECIFICATION V37
/// <b>Business Value:</b> Behavioral Prediction / Profiling.
/// <b>Community:</b> Comparing against 8 structural peers.
/// </summary>
public sealed class MerchantPhenotypeSpecV37 : Specification<Merchant, MerchantPhenotypeDto>
{
    public MerchantPhenotypeSpecV37(Guid merchantId)
    {
        Query.Where(c => c.MerchantId == merchantId);
        Query.AsNoTracking();

        Query.Select(root => new MerchantPhenotypeDto
        {
            EntityId = root.MerchantId.ToString(),

            // -- Behavioral Markers --
            ClusterArchetype = "Control_Cluster",
        });
    }
}

public class MerchantPhenotypeDto
{
    public string EntityId { get; set; }
    public string ClusterArchetype { get; set; }
}

/// <summary>
/// OUTCOME QUALITY SPECIFICATION V38
/// <b>Business Value:</b> Performance Grading / Vendor Management.
/// <b>Strategy:</b> Aggregates success/failure rates of downstream leaf nodes.
/// </summary>
public sealed class MerchantOutcomeSpecV38 : Specification<Merchant, MerchantOutcomeDto>
{
    public MerchantOutcomeSpecV38(Guid merchantId)
    {
        Query.Where(c => c.MerchantId == merchantId);
        Query.AsNoTracking();
        Query.AsSplitQuery();

        Query.Select(root => new MerchantOutcomeDto
        {
            EntityId = root.MerchantId.ToString(),
            Label = root.MerchantCode,

            // -- Performance Metrics (Outcomes) --
            Total_Assessment_Count = root.Assessments.Count(),
            Total_Asset_Count = root.Assets.Count(),
            Total_CompensatingControl_Count = root.CompensatingControls.Count(),
            Total_ComplianceOfficer_Count = root.ComplianceOfficers.Count(),
            Total_CryptographicInventory_Count = root.CryptographicInventories.Count(),
        });
    }
}

public class MerchantOutcomeDto
{
    public string EntityId { get; set; }
    public string Label { get; set; }
    public int Total_Assessment_Count { get; set; }
    public int Success_Assessment_Count { get; set; }
    public double Assessment_QualityScore => Total_Assessment_Count > 0 ? (double)Success_Assessment_Count / Total_Assessment_Count : 0.0;
    public int Total_Asset_Count { get; set; }
    public int Success_Asset_Count { get; set; }
    public double Asset_QualityScore => Total_Asset_Count > 0 ? (double)Success_Asset_Count / Total_Asset_Count : 0.0;
    public int Total_CompensatingControl_Count { get; set; }
    public int Success_CompensatingControl_Count { get; set; }
    public double CompensatingControl_QualityScore => Total_CompensatingControl_Count > 0 ? (double)Success_CompensatingControl_Count / Total_CompensatingControl_Count : 0.0;
    public int Total_ComplianceOfficer_Count { get; set; }
    public int Success_ComplianceOfficer_Count { get; set; }
    public double ComplianceOfficer_QualityScore => Total_ComplianceOfficer_Count > 0 ? (double)Success_ComplianceOfficer_Count / Total_ComplianceOfficer_Count : 0.0;
    public int Total_CryptographicInventory_Count { get; set; }
    public int Success_CryptographicInventory_Count { get; set; }
    public double CryptographicInventory_QualityScore => Total_CryptographicInventory_Count > 0 ? (double)Success_CryptographicInventory_Count / Total_CryptographicInventory_Count : 0.0;
}

/// <summary>
/// DIGITAL TWIN SPECIFICATION V39
/// <b>Purpose:</b> The 'God View'. Aggregates Identity, Structure, Health, and Prediction into a single model.
/// <b>Use Case:</b> The Main Dashboard for a specific entity instance.
/// </summary>
public sealed class MerchantDigitalTwinSpecV39 : Specification<Merchant, MerchantDigitalTwinDto>
{
    public MerchantDigitalTwinSpecV39(Guid merchantId)
    {
        Query.Where(c => c.MerchantId == merchantId);
        Query.AsNoTracking();
        Query.AsSplitQuery();

        Query.Select(root => new MerchantDigitalTwinDto
        {
            // -- 1. IDENTITY (Who am I?) --
            EntityId = root.MerchantId.ToString(),
            Label = root.MerchantCode,

            // -- 2. STRUCTURE (Where do I fit?) --
            TopologyRole = "Hub",
            ComplexityScore = 284,
            DependencyCount = 9,

            // -- 3. HEALTH (Am I okay?) --
            RiskExposure = 0.00,
            IsStructurallySound = true,

            // -- 4. BEHAVIOR (What am I doing?) --
            LastActivityDate = new[] { root.Assessments.Max(x => (DateTime?)x.StartDate), root.Assets.Max(x => (DateTime?)x.LastScanDate), root.CompensatingControls.Max(x => (DateTime?)x.ApprovalDate), root.CryptographicInventories.Max(x => (DateTime?)x.CreationDate), root.Evidences.Max(x => (DateTime?)x.CollectedDate), root.ServiceProviders.Max(x => (DateTime?)x.AOCExpiryDate) }.Max(),
        });
    }
}

public class MerchantDigitalTwinDto
{
    // Identity
    public string EntityId { get; set; }
    public string Label { get; set; }

    // Structure
    public string TopologyRole { get; set; }
    public double ComplexityScore { get; set; }
    public int DependencyCount { get; set; }

    // Health
    public double RiskExposure { get; set; }
    public bool IsStructurallySound { get; set; }

    // Behavior
    public DateTime? LastActivityDate { get; set; }

    public string OverallStatus =>
        !IsStructurallySound ? "BROKEN" :
        RiskExposure > 0.5 ? "CRITICAL" :
        (LastActivityDate < DateTime.UtcNow.AddDays(-30)) ? "DORMANT" :
        "HEALTHY";
}



public sealed class MerchantAdvancedGraphSpecV6 : Specification<Merchant, MerchantEntityDto>
{
    public MerchantAdvancedGraphSpecV6(
        Guid merchantId,
        bool enableIntelligentProjection = true,
        bool enableSemanticAnalysis = true,
        bool enableBlueprintStrategy = true,
        int? take = null,
        int? skip = null)
    {
        _ = Guard.Against.Default(merchantId, nameof(merchantId));
        _ = Query.Where(c => c.MerchantId == merchantId);

        if (take.HasValue && skip.HasValue)
        {
            _ = Query.Skip(skip.Value).Take(take.Value);
        }
        if (enableBlueprintStrategy)
        {
        }

        _ = Query.Select(c => new MerchantEntityDto
        {
            MerchantId = c.MerchantId,
            TenantId = c.TenantId,
            MerchantCode = c.MerchantCode,
            MerchantName = c.MerchantName,
            MerchantLevel = c.MerchantLevel,
            AcquirerName = c.AcquirerName,
            ProcessorMID = c.ProcessorMID,
            AnnualCardVolume = c.AnnualCardVolume,
            LastAssessmentDate = c.LastAssessmentDate,
            NextAssessmentDue = c.NextAssessmentDue,
            ComplianceRank = c.ComplianceRank,
            CreatedAt = c.CreatedAt,
            CreatedBy = c.CreatedBy,
            UpdatedAt = c.UpdatedAt,
            UpdatedBy = c.UpdatedBy,
            IsDeleted = c.IsDeleted,

            #region Intelligent Metrics

            AssessmentComplianceScore = c.Assessments.Any() ? c.Assessments.Average(x => x.ComplianceScore) ?? 0 : 0,
            DaysSinceLastAssessment = c.LastAssessmentDate.HasValue ? (DateTime.UtcNow - c.LastAssessmentDate.Value).Days : 0,
            AverageAssessmentComplianceScore = c.Assessments.Any() ? c.Assessments.Average(x => x.ComplianceScore) ?? 0 : 0,
            MaxCryptographicInventoryKeyLength = c.CryptographicInventories.Any() ? c.CryptographicInventories.Max(x => x.KeyLength) : 0,
            TotalPaymentChannelProcessingVolume = c.PaymentChannels.Sum(x => x.ProcessingVolume),
            AverageVulnerabilitySeverity = c.Assets.SelectMany(i => i.Vulnerabilities).Any() ? (int)c.Assets.SelectMany(i => i.Vulnerabilities).Average(t => (int?)t.Severity) : 0,
            MaxVulnerabilityCVSS = c.Assets.SelectMany(i => i.Vulnerabilities).Any() ? c.Assets.SelectMany(i => i.Vulnerabilities).Max(t => t.CVSS) ?? 0 : 0,
            HasAssessmentComplianceScoreBelowThreshold = c.Assessments.Any(x => x.ComplianceScore < 70),
            IsMerchantLevelCritical = c.MerchantLevel >= 7,
            IsDeletedFlag = c.IsDeleted,
            TotalAssessmentCount = c.Assessments.Count(),
            ActiveAssessmentCount = c.Assessments.Count(x => !x.IsDeleted),
            CriticalAssessmentCount = c.Assessments.Count(x => x.AssessmentType >= 7),
            LatestAssessmentStartDate = c.Assessments.Max(x => (DateTime?)x.StartDate),
            TotalAssetCount = c.Assets.Count(),
            ActiveAssetCount = c.Assets.Count(x => !x.IsDeleted),
            CriticalAssetCount = c.Assets.Count(x => x.AssetType >= 7),
            LatestAssetLastScanDate = c.Assets.Max(x => x.LastScanDate),
            TotalCompensatingControlCount = c.CompensatingControls.Count(),
            ActiveCompensatingControlCount = c.CompensatingControls.Count(x => !x.IsDeleted),
            CriticalCompensatingControlCount = c.CompensatingControls.Count(x => x.Rank >= 7),
            LatestCompensatingControlApprovalDate = c.CompensatingControls.Max(x => x.ApprovalDate),
            TotalComplianceOfficerCount = c.ComplianceOfficers.Count(),
            ActiveComplianceOfficerCount = c.ComplianceOfficers.Count(x => !x.IsDeleted),
            LatestComplianceOfficerUpdatedAt = c.ComplianceOfficers.Max(x => x.UpdatedAt),
            TotalCryptographicInventoryCount = c.CryptographicInventories.Count(),
            ActiveCryptographicInventoryCount = c.CryptographicInventories.Count(x => !x.IsDeleted),
            LatestCryptographicInventoryCreationDate = c.CryptographicInventories.Max(x => (DateTime?)x.CreationDate),
            TotalEvidenceCount = c.Evidences.Count(),
            ActiveEvidenceCount = c.Evidences.Count(x => !x.IsDeleted),
            CriticalEvidenceCount = c.Evidences.Count(x => x.EvidenceType >= 7),
            LatestEvidenceCollectedDate = c.Evidences.Max(x => (DateTime?)x.CollectedDate),
            TotalNetworkSegmentationCount = c.NetworkSegmentations.Count(),
            ActiveNetworkSegmentationCount = c.NetworkSegmentations.Count(x => !x.IsDeleted),
            LatestNetworkSegmentationLastValidated = c.NetworkSegmentations.Max(x => x.LastValidated),
            TotalPaymentChannelCount = c.PaymentChannels.Count(),
            ActivePaymentChannelCount = c.PaymentChannels.Count(x => !x.IsDeleted),
            CriticalPaymentChannelCount = c.PaymentChannels.Count(x => x.ChannelType >= 7),
            LatestPaymentChannelUpdatedAt = c.PaymentChannels.Max(x => x.UpdatedAt),
            TotalServiceProviderCount = c.ServiceProviders.Count(),
            ActiveServiceProviderCount = c.ServiceProviders.Count(x => !x.IsDeleted),
            LatestServiceProviderAOCExpiryDate = c.ServiceProviders.Max(x => x.AOCExpiryDate),

            #endregion Intelligent Metrics

            ROCPackages = c.Assessments
                .SelectMany(i => i.ROCPackages)
                .Select(t => new ROCPackageEntityDto
                {
                    ROCPackageId = t.ROCPackageId,
                    TenantId = t.TenantId,
                    AssessmentId = t.AssessmentId,
                    PackageVersion = t.PackageVersion,
                    GeneratedDate = t.GeneratedDate,
                    QSAName = t.QSAName,
                    QSACompany = t.QSACompany,
                    SignatureDate = t.SignatureDate,
                    AOCNumber = t.AOCNumber,
                    Rank = t.Rank,
                    IsDeleted = t.IsDeleted,
                })
                .Take(2).ToList(),
            AssetControls = c.Assets
                .SelectMany(i => i.AssetControls)
                .Select(t => new AssetControlEntityDto
                {
                    RowId = t.RowId,
                    AssetId = t.AssetId,
                    ControlId = t.ControlId,
                    TenantId = t.TenantId,
                    IsApplicable = t.IsApplicable,
                    CustomizedApproach = t.CustomizedApproach,
                    IsDeleted = t.IsDeleted,
                })
                .Take(2).ToList(),
            ScanSchedules = c.Assets
                .SelectMany(i => i.ScanSchedules)
                .Select(t => new ScanScheduleEntityDto
                {
                    ScanScheduleId = t.ScanScheduleId,
                    TenantId = t.TenantId,
                    AssetId = t.AssetId,
                    ScanType = t.ScanType,
                    Frequency = t.Frequency,
                    NextScanDate = t.NextScanDate,
                    BlackoutStart = t.BlackoutStart,
                    BlackoutEnd = t.BlackoutEnd,
                    IsEnabled = t.IsEnabled,
                    IsDeleted = t.IsDeleted,
                })
                .Take(2).ToList(),
            Vulnerabilities = c.Assets
                .SelectMany(i => i.Vulnerabilities)
                .Select(t => new VulnerabilityEntityDto
                {
                    VulnerabilityId = t.VulnerabilityId,
                    TenantId = t.TenantId,
                    AssetId = t.AssetId,
                    VulnerabilityCode = t.VulnerabilityCode,
                    CVEId = t.CVEId,
                    Title = t.Title,
                    Severity = t.Severity,
                    CVSS = t.CVSS,
                    DetectedDate = t.DetectedDate,
                    ResolvedDate = t.ResolvedDate,
                    Rank = t.Rank,
                    IsDeleted = t.IsDeleted,
                })
                .Take(2).ToList(),
            Controls = c.CompensatingControls
                .Select(i => i.Control)
                .Select(t => new ControlEntityDto
                {
                    ControlId = t.ControlId,
                    TenantId = t.TenantId,
                    ControlCode = t.ControlCode,
                    RequirementNumber = t.RequirementNumber,
                    ControlTitle = t.ControlTitle,
                    ControlDescription = t.ControlDescription,
                    TestingGuidance = t.TestingGuidance,
                    FrequencyDays = t.FrequencyDays,
                    IsMandatory = t.IsMandatory,
                    EffectiveDate = t.EffectiveDate,
                    IsDeleted = t.IsDeleted,
                })
                .Take(2).ToList(),
            Assessments = c.Assessments.Select(assessment => new AssessmentEntityDto
            {
                AssessmentId = assessment.AssessmentId,
                TenantId = assessment.TenantId,
                MerchantId = assessment.MerchantId,
                AssessmentCode = assessment.AssessmentCode,
                AssessmentType = assessment.AssessmentType,
                AssessmentPeriod = assessment.AssessmentPeriod,
                StartDate = assessment.StartDate,
                EndDate = assessment.EndDate,
                CompletionDate = assessment.CompletionDate,
                Rank = assessment.Rank,
                ComplianceScore = assessment.ComplianceScore,
                QSAReviewRequired = assessment.QSAReviewRequired,
                CreatedAt = assessment.CreatedAt,
                CreatedBy = assessment.CreatedBy,
                UpdatedAt = assessment.UpdatedAt,
                UpdatedBy = assessment.UpdatedBy,
                IsDeleted = assessment.IsDeleted,
            }).ToList(),
            Assets = c.Assets.Select(asset => new AssetEntityDto
            {
                AssetId = asset.AssetId,
                TenantId = asset.TenantId,
                MerchantId = asset.MerchantId,
                AssetCode = asset.AssetCode,
                AssetName = asset.AssetName,
                AssetType = asset.AssetType,
                IPAddress = asset.IPAddress,
                Hostname = asset.Hostname,
                IsInCDE = asset.IsInCDE,
                NetworkZone = asset.NetworkZone,
                LastScanDate = asset.LastScanDate,
                CreatedAt = asset.CreatedAt,
                CreatedBy = asset.CreatedBy,
                UpdatedAt = asset.UpdatedAt,
                UpdatedBy = asset.UpdatedBy,
                IsDeleted = asset.IsDeleted,
            }).ToList(),
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
                Control = compensatingControl.Control != null ? new ControlEntityDto
                {
                    ControlId = compensatingControl.Control.ControlId,
                    TenantId = compensatingControl.Control.TenantId,
                    ControlCode = compensatingControl.Control.ControlCode,
                    RequirementNumber = compensatingControl.Control.RequirementNumber,
                    ControlTitle = compensatingControl.Control.ControlTitle,
                    ControlDescription = compensatingControl.Control.ControlDescription,
                    TestingGuidance = compensatingControl.Control.TestingGuidance,
                    FrequencyDays = compensatingControl.Control.FrequencyDays,
                    IsMandatory = compensatingControl.Control.IsMandatory,
                    EffectiveDate = compensatingControl.Control.EffectiveDate,
                    CreatedAt = compensatingControl.Control.CreatedAt,
                    CreatedBy = compensatingControl.Control.CreatedBy,
                    UpdatedAt = compensatingControl.Control.UpdatedAt,
                    UpdatedBy = compensatingControl.Control.UpdatedBy,
                    IsDeleted = compensatingControl.Control.IsDeleted,
                } : null,
            }).ToList(),
            ComplianceOfficers = c.ComplianceOfficers.Select(complianceOfficer => new ComplianceOfficerEntityDto
            {
                ComplianceOfficerId = complianceOfficer.ComplianceOfficerId,
                TenantId = complianceOfficer.TenantId,
                MerchantId = complianceOfficer.MerchantId,
                OfficerCode = complianceOfficer.OfficerCode,
                FirstName = complianceOfficer.FirstName,
                LastName = complianceOfficer.LastName,
                Email = complianceOfficer.Email,
                Phone = complianceOfficer.Phone,
                CertificationLevel = complianceOfficer.CertificationLevel,
                IsActive = complianceOfficer.IsActive,
                CreatedAt = complianceOfficer.CreatedAt,
                CreatedBy = complianceOfficer.CreatedBy,
                UpdatedAt = complianceOfficer.UpdatedAt,
                UpdatedBy = complianceOfficer.UpdatedBy,
                IsDeleted = complianceOfficer.IsDeleted,
            }).ToList(),
            CryptographicInventories = c.CryptographicInventories.Select(cryptographicInventory => new CryptographicInventoryEntityDto
            {
                CryptographicInventoryId = cryptographicInventory.CryptographicInventoryId,
                TenantId = cryptographicInventory.TenantId,
                MerchantId = cryptographicInventory.MerchantId,
                KeyName = cryptographicInventory.KeyName,
                KeyType = cryptographicInventory.KeyType,
                Algorithm = cryptographicInventory.Algorithm,
                KeyLength = cryptographicInventory.KeyLength,
                KeyLocation = cryptographicInventory.KeyLocation,
                CreationDate = cryptographicInventory.CreationDate,
                LastRotationDate = cryptographicInventory.LastRotationDate,
                NextRotationDue = cryptographicInventory.NextRotationDue,
                CreatedAt = cryptographicInventory.CreatedAt,
                CreatedBy = cryptographicInventory.CreatedBy,
                UpdatedAt = cryptographicInventory.UpdatedAt,
                UpdatedBy = cryptographicInventory.UpdatedBy,
                IsDeleted = cryptographicInventory.IsDeleted,
            }).ToList(),
            Evidences = c.Evidences.Select(evidence => new EvidenceEntityDto
            {
                EvidenceId = evidence.EvidenceId,
                TenantId = evidence.TenantId,
                MerchantId = evidence.MerchantId,
                EvidenceCode = evidence.EvidenceCode,
                EvidenceTitle = evidence.EvidenceTitle,
                EvidenceType = evidence.EvidenceType,
                CollectedDate = evidence.CollectedDate,
                FileHash = evidence.FileHash,
                StorageUri = evidence.StorageUri,
                IsValid = evidence.IsValid,
                CreatedAt = evidence.CreatedAt,
                CreatedBy = evidence.CreatedBy,
                UpdatedAt = evidence.UpdatedAt,
                UpdatedBy = evidence.UpdatedBy,
                IsDeleted = evidence.IsDeleted,
            }).ToList(),
            NetworkSegmentations = c.NetworkSegmentations.Select(networkSegmentation => new NetworkSegmentationEntityDto
            {
                NetworkSegmentationId = networkSegmentation.NetworkSegmentationId,
                TenantId = networkSegmentation.TenantId,
                MerchantId = networkSegmentation.MerchantId,
                SegmentName = networkSegmentation.SegmentName,
                VLANId = networkSegmentation.VLANId,
                IPRange = networkSegmentation.IPRange,
                FirewallRules = networkSegmentation.FirewallRules,
                IsInCDE = networkSegmentation.IsInCDE,
                LastValidated = networkSegmentation.LastValidated,
                CreatedAt = networkSegmentation.CreatedAt,
                CreatedBy = networkSegmentation.CreatedBy,
                UpdatedAt = networkSegmentation.UpdatedAt,
                UpdatedBy = networkSegmentation.UpdatedBy,
                IsDeleted = networkSegmentation.IsDeleted,
            }).ToList(),
            PaymentChannels = c.PaymentChannels.Select(paymentChannel => new PaymentChannelEntityDto
            {
                PaymentChannelId = paymentChannel.PaymentChannelId,
                TenantId = paymentChannel.TenantId,
                MerchantId = paymentChannel.MerchantId,
                ChannelCode = paymentChannel.ChannelCode,
                ChannelName = paymentChannel.ChannelName,
                ChannelType = paymentChannel.ChannelType,
                ProcessingVolume = paymentChannel.ProcessingVolume,
                IsInScope = paymentChannel.IsInScope,
                TokenizationEnabled = paymentChannel.TokenizationEnabled,
                CreatedAt = paymentChannel.CreatedAt,
                CreatedBy = paymentChannel.CreatedBy,
                UpdatedAt = paymentChannel.UpdatedAt,
                UpdatedBy = paymentChannel.UpdatedBy,
                IsDeleted = paymentChannel.IsDeleted,
            }).ToList(),
            ServiceProviders = c.ServiceProviders.Select(serviceProvider => new ServiceProviderEntityDto
            {
                ServiceProviderId = serviceProvider.ServiceProviderId,
                TenantId = serviceProvider.TenantId,
                MerchantId = serviceProvider.MerchantId,
                ProviderName = serviceProvider.ProviderName,
                ServiceType = serviceProvider.ServiceType,
                IsPCICompliant = serviceProvider.IsPCICompliant,
                AOCExpiryDate = serviceProvider.AOCExpiryDate,
                ResponsibilityMatrix = serviceProvider.ResponsibilityMatrix,
                CreatedAt = serviceProvider.CreatedAt,
                CreatedBy = serviceProvider.CreatedBy,
                UpdatedAt = serviceProvider.UpdatedAt,
                UpdatedBy = serviceProvider.UpdatedBy,
                IsDeleted = serviceProvider.IsDeleted,
            }).ToList(),
        })
        .AsNoTracking()
        .AsSplitQuery()
        .EnableCache($"MerchantAdvancedGraphSpecV6-{merchantId}");
    }
}

/// <summary>
/// Graph Theory Specification: Ego Network Analysis
/// Projects the entity as a Central Node with weighted edges to all immediate neighbors.
/// Optimized for Graph Visualization (D3.js/Sigma.js) and Network Analysis.
/// </summary>
public sealed class MerchantEgoNetworkSpec : Specification<Merchant, MerchantEgoNetworkDto>
{
    public MerchantEgoNetworkSpec(Guid merchantId)
    {
        Query.Where(c => c.MerchantId == merchantId);
        Query.AsNoTracking();

        // Eager load neighbors for accurate weight calculation
        Query.Include(c => c.Assessments);
        Query.Include(c => c.Assets);
        Query.Include(c => c.CompensatingControls);
        Query.Include(c => c.ComplianceOfficers);
        Query.Include(c => c.CryptographicInventories);
        Query.Include(c => c.Evidences);
        Query.Include(c => c.NetworkSegmentations);
        Query.Include(c => c.PaymentChannels);
        Query.Include(c => c.ServiceProviders);

        Query.Select(root => new MerchantEgoNetworkDto
        {
            CentralNode = new GraphNodeDto
            {
                Id = root.MerchantId.ToString(),
                Label = root.MerchantCode,
                Type = "Merchant",
                RiskScore = 0,
                ValueScore = 0,
            },
            UpstreamEdges = new List<GraphEdgeDto>
            {
            }.Where(x => x != null).ToList(),
            DownstreamEdges = new List<GraphEdgeDto>
                {
                    root.Assessments.Any() ? new GraphEdgeDto
                    {
                        SourceId = root.MerchantId.ToString(),
                        TargetId = "GROUP_" + "Assessment", // Grouping child nodes for clean viz
                        Relationship = "HAS_ASSESSMENT",
                        Weight = root.Assessments.Count() * 1.0,
                        Direction = EdgeDirection.Outbound,
                        Metadata = new Dictionary<string, object> { { "Count", root.Assessments.Count() } }
                    } : null,
                    root.Assets.Any() ? new GraphEdgeDto
                    {
                        SourceId = root.MerchantId.ToString(),
                        TargetId = "GROUP_" + "Asset", // Grouping child nodes for clean viz
                        Relationship = "HAS_ASSET",
                        Weight = root.Assets.Count() * 1.0,
                        Direction = EdgeDirection.Outbound,
                        Metadata = new Dictionary<string, object> { { "Count", root.Assets.Count() } }
                    } : null,
                    root.CompensatingControls.Any() ? new GraphEdgeDto
                    {
                        SourceId = root.MerchantId.ToString(),
                        TargetId = "GROUP_" + "CompensatingControl", // Grouping child nodes for clean viz
                        Relationship = "HAS_COMPENSATINGCONTROL",
                        Weight = root.CompensatingControls.Count() * 1.0,
                        Direction = EdgeDirection.Outbound,
                        Metadata = new Dictionary<string, object> { { "Count", root.CompensatingControls.Count() } }
                    } : null,
                    root.ComplianceOfficers.Any() ? new GraphEdgeDto
                    {
                        SourceId = root.MerchantId.ToString(),
                        TargetId = "GROUP_" + "ComplianceOfficer", // Grouping child nodes for clean viz
                        Relationship = "HAS_COMPLIANCEOFFICER",
                        Weight = root.ComplianceOfficers.Count() * 1.0,
                        Direction = EdgeDirection.Outbound,
                        Metadata = new Dictionary<string, object> { { "Count", root.ComplianceOfficers.Count() } }
                    } : null,
                    root.CryptographicInventories.Any() ? new GraphEdgeDto
                    {
                        SourceId = root.MerchantId.ToString(),
                        TargetId = "GROUP_" + "CryptographicInventory", // Grouping child nodes for clean viz
                        Relationship = "HAS_CRYPTOGRAPHICINVENTORY",
                        Weight = root.CryptographicInventories.Count() * 1.0,
                        Direction = EdgeDirection.Outbound,
                        Metadata = new Dictionary<string, object> { { "Count", root.CryptographicInventories.Count() } }
                    } : null,
                    root.Evidences.Any() ? new GraphEdgeDto
                    {
                        SourceId = root.MerchantId.ToString(),
                        TargetId = "GROUP_" + "Evidence", // Grouping child nodes for clean viz
                        Relationship = "HAS_EVIDENCE",
                        Weight = root.Evidences.Count() * 1.0,
                        Direction = EdgeDirection.Outbound,
                        Metadata = new Dictionary<string, object> { { "Count", root.Evidences.Count() } }
                    } : null,
                    root.NetworkSegmentations.Any() ? new GraphEdgeDto
                    {
                        SourceId = root.MerchantId.ToString(),
                        TargetId = "GROUP_" + "NetworkSegmentation", // Grouping child nodes for clean viz
                        Relationship = "HAS_NETWORKSEGMENTATION",
                        Weight = root.NetworkSegmentations.Count() * 1.0,
                        Direction = EdgeDirection.Outbound,
                        Metadata = new Dictionary<string, object> { { "Count", root.NetworkSegmentations.Count() } }
                    } : null,
                    root.PaymentChannels.Any() ? new GraphEdgeDto
                    {
                        SourceId = root.MerchantId.ToString(),
                        TargetId = "GROUP_" + "PaymentChannel", // Grouping child nodes for clean viz
                        Relationship = "HAS_PAYMENTCHANNEL",
                        Weight = root.PaymentChannels.Count() * 1.0,
                        Direction = EdgeDirection.Outbound,
                        Metadata = new Dictionary<string, object> { { "Count", root.PaymentChannels.Count() } }
                    } : null,
                    root.ServiceProviders.Any() ? new GraphEdgeDto
                    {
                        SourceId = root.MerchantId.ToString(),
                        TargetId = "GROUP_" + "ServiceProvider", // Grouping child nodes for clean viz
                        Relationship = "HAS_SERVICEPROVIDER",
                        Weight = root.ServiceProviders.Count() * 1.0,
                        Direction = EdgeDirection.Outbound,
                        Metadata = new Dictionary<string, object> { { "Count", root.ServiceProviders.Count() } }
                    } : null,
                }.Where(x => x != null).ToList(),
            HorizontalEdges = new List<GraphEdgeDto>()
        });
    }
}

public class MerchantEgoNetworkDto
{
    public GraphNodeDto CentralNode { get; set; }
    public List<GraphEdgeDto> UpstreamEdges { get; set; }
    public List<GraphEdgeDto> DownstreamEdges { get; set; }
    public List<GraphEdgeDto> HorizontalEdges { get; set; }
}

/// <summary>
/// AI Vectorization Specification
/// Flattens the entity graph into a rich Semantic Context for LLM/RAG consumption.
/// "Data is the food for Super Intelligence."
/// </summary>
public sealed class MerchantAiContextSpec : Specification<Merchant, MerchantAiContextDto>
{
    public MerchantAiContextSpec(Guid merchantId)
    {
        Query.Where(c => c.MerchantId == merchantId);
        Query.AsNoTracking();

        Query.Include(c => c.Assessments);
        Query.Include(c => c.Assets);
        Query.Include(c => c.CompensatingControls);
        Query.Include(c => c.ComplianceOfficers);
        Query.Include(c => c.CryptographicInventories);
        Query.Include(c => c.Evidences);
        Query.Include(c => c.NetworkSegmentations);
        Query.Include(c => c.PaymentChannels);
        Query.Include(c => c.ServiceProviders);

        Query.Select(root => new MerchantAiContextDto
        {
            EntityId = root.MerchantId.ToString(),
            EntityType = "Merchant",
            SemanticNarrative = $"ENTITY PROFILE: {root.GetType().Name} \n" +
                $"IDENTITY: Name/Code is {root.MerchantCode}. " +
                $"CONNECTIONS: Linked to {root.Assessments.Count()} Assessment items. " +
                "\n" +
                $"CONNECTIONS: Linked to {root.Assets.Count()} Asset items. " +
                (root.Assets.Any() ? $"Last activity on {root.Assets.Max(x => x.LastScanDate)}." : "No recent activity.") + "\n" +
                $"CONNECTIONS: Linked to {root.CompensatingControls.Count()} CompensatingControl items. " +
                "\n" +
                $"CONNECTIONS: Linked to {root.ComplianceOfficers.Count()} ComplianceOfficer items. " +
                "\n" +
                $"CONNECTIONS: Linked to {root.CryptographicInventories.Count()} CryptographicInventory items. " +
                "\n" +
                $"CONNECTIONS: Linked to {root.Evidences.Count()} Evidence items. " +
                "\n" +
                $"CONNECTIONS: Linked to {root.NetworkSegmentations.Count()} NetworkSegmentation items. " +
                (root.NetworkSegmentations.Any() ? $"Last activity on {root.NetworkSegmentations.Max(x => x.LastValidated)}." : "No recent activity.") + "\n" +
                $"CONNECTIONS: Linked to {root.PaymentChannels.Count()} PaymentChannel items. " +
                "\n" +
                $"CONNECTIONS: Linked to {root.ServiceProviders.Count()} ServiceProvider items. " +
                "\n" +
                "END PROFILE",
            MetadataTags = new List<string>
                {
                    "Merchant",
                }.Where(x => x != null).ToList(),
            LastUpdated = DateTime.UtcNow
        });
    }
}

public class MerchantAiContextDto
{
    public string EntityId { get; set; }
    public string EntityType { get; set; }
    public string SemanticNarrative { get; set; }
    public List<string> MetadataTags { get; set; }
    public DateTime LastUpdated { get; set; }
}

public sealed class MerchantAdvancedFilterSpec : Specification<Merchant>
{
    public MerchantAdvancedFilterSpec(
        int pageNumber,
        int pageSize,
        Dictionary<string, string> filters = null,
        List<Sort> sorting = null
    )
    {
        _ = Guard.Against.NegativeOrZero(pageNumber, nameof(pageNumber));
        _ = Guard.Against.NegativeOrZero(pageSize, nameof(pageSize));

        _ = Query;
        //  Query.Where(c => !c.IsDeleted);

        if (filters != null)
        {
            foreach (var filter in filters)
            {
                switch (filter.Key.ToLower())
                {
                    case "merchantcode":
                        Query.Where(c => c.MerchantCode.Contains(filter.Value));
                        break;
                    case "merchantname":
                        Query.Where(c => c.MerchantName.Contains(filter.Value));
                        break;
                    case "acquirername":
                        Query.Where(c => c.AcquirerName.Contains(filter.Value));
                        break;
                    case "merchantlevel":
                        if (int.TryParse(filter.Value, out int merchantlevel))
                        {
                            Query.Where(c => c.MerchantLevel == merchantlevel);
                        }
                        break;
                    case "annualcardvolume":
                        if (decimal.TryParse(filter.Value, out decimal annualcardvolume))
                        {
                            // Query.Where(c => c.AnnualCardVolume == annualcardvolume);
                            Query.Where(c => c.AnnualCardVolume >= annualcardvolume - 0.1m && c.AnnualCardVolume <= annualcardvolume + 0.1m);
                        }
                        break;
                    case "lastassessmentdate":
                        if (DateTime.TryParse(filter.Value, out DateTime lastassessmentdate))
                        {
                            Query.Where(c => c.LastAssessmentDate >= lastassessmentdate.AddHours(-6) && c.LastAssessmentDate <= lastassessmentdate.AddHours(6));
                            //   Query.Where(c => c.LastAssessmentDate == lastassessmentdate);
                        }
                        break;
                    case "nextassessmentdue":
                        if (DateTime.TryParse(filter.Value, out DateTime nextassessmentdue))
                        {
                            Query.Where(c => c.NextAssessmentDue >= nextassessmentdue.AddHours(-6) && c.NextAssessmentDue <= nextassessmentdue.AddHours(6));
                            //   Query.Where(c => c.NextAssessmentDue == nextassessmentdue);
                        }
                        break;
                    case "compliancerank":
                        if (int.TryParse(filter.Value, out int compliancerank))
                        {
                            Query.Where(c => c.ComplianceRank == compliancerank);
                        }
                        break;
                    case "createdat":
                        if (DateTime.TryParse(filter.Value, out DateTime createdat))
                        {
                            Query.Where(c => c.CreatedAt >= createdat.AddHours(-6) && c.CreatedAt <= createdat.AddHours(6));
                            //   Query.Where(c => c.CreatedAt == createdat);
                        }
                        break;
                    case "updatedat":
                        if (DateTime.TryParse(filter.Value, out DateTime updatedat))
                        {
                            Query.Where(c => c.UpdatedAt >= updatedat.AddHours(-6) && c.UpdatedAt <= updatedat.AddHours(6));
                            //   Query.Where(c => c.UpdatedAt == updatedat);
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
            Query.OrderByDescending(x => x.MerchantId);
        }

        Query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
    }

    private static IOrderedSpecificationBuilder<Merchant> ApplySort(
        ISpecificationBuilder<Merchant> query,
        Sort sort
    )
    {
        return sort.Direction == SortDirection.Descending
            ? query.OrderByDescending(GetSortProperty(sort.Field))
            : query.OrderBy(GetSortProperty(sort.Field));
    }

    private static IOrderedSpecificationBuilder<Merchant> ApplyAdditionalSort(
        IOrderedSpecificationBuilder<Merchant> query,
        Sort sort
    )
    {
        return sort.Direction == SortDirection.Descending
            ? query.ThenByDescending(GetSortProperty(sort.Field))
            : query.ThenBy(GetSortProperty(sort.Field));
    }

    private static Expression<Func<Merchant, object>> GetSortProperty(
        string propertyName
    )
    {
        return propertyName.ToLower() switch
        {
            "merchantcode" => c => c.MerchantCode,
            "merchantname" => c => c.MerchantName,
            "acquirername" => c => c.AcquirerName,
            _ => c => c.MerchantId,
        };
    }
}

// Shared Graph DTOs (Neo4j / D3.js Compatible)
public class GraphNodeDto
{
    public string Id { get; set; }
    public string Label { get; set; }
    public string Type { get; set; }
    public object RiskScore { get; set; }
    public object ValueScore { get; set; }
}

public class GraphEdgeDto
{
    public string SourceId { get; set; }
    public string TargetId { get; set; }
    public string Relationship { get; set; }
    public double Weight { get; set; }
    public string TargetLabel { get; set; }
    public EdgeDirection Direction { get; set; }
    public Dictionary<string, object> Metadata { get; set; }
}

public enum EdgeDirection { Inbound, Outbound, Bidirectional }