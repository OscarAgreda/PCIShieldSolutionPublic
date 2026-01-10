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
    public sealed class AssetListPagedSpec : PagedSpecification<Asset, AssetEntityDto>
    {
        public AssetListPagedSpec(int pageNumber, int pageSize)
        {
            _ = Guard.Against.NegativeOrZero(pageNumber, nameof(pageNumber));
            _ = Guard.Against.NegativeOrZero(pageSize, nameof(pageSize));

            _ = Query
                .OrderByDescending(i => i.AssetId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            _ = Query.Include(x => x.Merchant);
            _ = Query.Select(x => new AssetEntityDto
            {
                AssetId = x.AssetId,
                TenantId = x.TenantId,
                MerchantId = x.MerchantId,
                AssetCode = x.AssetCode,
                AssetName = x.AssetName,
                AssetType = x.AssetType,
                IPAddress = x.IPAddress,
                Hostname = x.Hostname,
                IsInCDE = x.IsInCDE,
                NetworkZone = x.NetworkZone,
                LastScanDate = x.LastScanDate,
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
            .EnableCache($"AssetListPagedSpec-{pageNumber}-{pageSize}");
        }
    }
    public sealed class AssetSearchSpec : Specification<Asset>
    {
        public AssetSearchSpec(string searchTerm)
        {
            string searchLower = searchTerm?.ToLower() ?? string.Empty;

            Query
                .Where(c =>
                        (c.AssetCode != null && c.AssetCode.ToLower().Contains(searchLower)) ||
                        (c.AssetName != null && c.AssetName.ToLower().Contains(searchLower)) ||
                        (c.IPAddress != null && c.IPAddress.ToLower().Contains(searchLower)) ||
                        (c.Hostname != null && c.Hostname.ToLower().Contains(searchLower))                )
                .OrderByDescending(c => c.AssetId);
        }
    }

    public sealed class AssetLastCreatedSpec : Specification<Asset>
    {
        public AssetLastCreatedSpec()
        {
            Query
                .OrderByDescending(c => c.LastScanDate)
                .Take(1)
                .AsNoTracking()
                .EnableCache("AssetLastCreatedSpec");
        }
    }
    public sealed class AssetByIdSpec : Specification<Asset, AssetEntityDto>
    {
        public AssetByIdSpec(Guid id)
        {
            _ = Guard.Against.NullOrEmpty(id, nameof(id));

            _ = Query.Where(x => x.AssetId == id);

            _ = Query.Include(x => x.Merchant);
            _ = Query.Include(x => x.ScanSchedules);
            _ = Query.Include(x => x.Vulnerabilities);
            _ = Query.Include(x => x.AssetControls);
            _ = Query.Select(x => new AssetEntityDto
            {
                AssetId = x.AssetId,
                TenantId = x.TenantId,
                MerchantId = x.MerchantId,
                AssetCode = x.AssetCode,
                AssetName = x.AssetName,
                AssetType = x.AssetType,
                IPAddress = x.IPAddress,
                Hostname = x.Hostname,
                IsInCDE = x.IsInCDE,
                NetworkZone = x.NetworkZone,
                LastScanDate = x.LastScanDate,
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
                ScanSchedules = x.ScanSchedules.Select(scanSchedule => new ScanScheduleEntityDto
                {
                    ScanScheduleId = scanSchedule.ScanScheduleId,
                    TenantId = scanSchedule.TenantId,
                    AssetId = scanSchedule.AssetId,
                    ScanType = scanSchedule.ScanType,
                    Frequency = scanSchedule.Frequency,
                    NextScanDate = scanSchedule.NextScanDate,
                    BlackoutStart = scanSchedule.BlackoutStart,
                    BlackoutEnd = scanSchedule.BlackoutEnd,
                    IsEnabled = scanSchedule.IsEnabled,
                    CreatedAt = scanSchedule.CreatedAt,
                    CreatedBy = scanSchedule.CreatedBy,
                    UpdatedAt = scanSchedule.UpdatedAt,
                    UpdatedBy = scanSchedule.UpdatedBy,
                    IsDeleted = scanSchedule.IsDeleted,
                }).ToList(),
                Vulnerabilities = x.Vulnerabilities.Select(vulnerability => new VulnerabilityEntityDto
                {
                    VulnerabilityId = vulnerability.VulnerabilityId,
                    TenantId = vulnerability.TenantId,
                    AssetId = vulnerability.AssetId,
                    VulnerabilityCode = vulnerability.VulnerabilityCode,
                    CVEId = vulnerability.CVEId,
                    Title = vulnerability.Title,
                    Severity = vulnerability.Severity,
                    CVSS = vulnerability.CVSS,
                    DetectedDate = vulnerability.DetectedDate,
                    ResolvedDate = vulnerability.ResolvedDate,
                    Rank = vulnerability.Rank,
                    CreatedAt = vulnerability.CreatedAt,
                    CreatedBy = vulnerability.CreatedBy,
                    UpdatedAt = vulnerability.UpdatedAt,
                    UpdatedBy = vulnerability.UpdatedBy,
                    IsDeleted = vulnerability.IsDeleted,
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
                }).ToList(),
            })
            .AsNoTracking()
            .AsSplitQuery()
            .EnableCache($"AssetByIdSpec-{id.ToString()}");
        }
    }
}

    public sealed class AssetAdvancedGraphSpecV4 : Specification<Asset, AssetEntityDto>
    {
        public AssetAdvancedGraphSpecV4(
            Guid assetId,
            bool includeReferenceData = true,
            bool includeTransactionData = true,
            bool includeHierarchicalData = true,
            int? take = null,
            int? skip = null,
            DateTime? effectiveDate = null)
        {
            Guard.Against.Default(assetId, nameof(assetId));
            Query.Where(c => c.AssetId == assetId);

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

            Query.Select(c => new AssetEntityDto
            {
                AssetId = c.AssetId,
                TenantId = c.TenantId,
                MerchantId = c.MerchantId,
                AssetCode = c.AssetCode,
                AssetName = c.AssetName,
                AssetType = c.AssetType,
                IPAddress = c.IPAddress,
                Hostname = c.Hostname,
                IsInCDE = c.IsInCDE,
                NetworkZone = c.NetworkZone,
                LastScanDate = c.LastScanDate,
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
                ScanSchedules = c.ScanSchedules.Select(scanSchedule => new ScanScheduleEntityDto
                {
                    ScanScheduleId = scanSchedule.ScanScheduleId,
                    TenantId = scanSchedule.TenantId,
                    AssetId = scanSchedule.AssetId,
                    ScanType = scanSchedule.ScanType,
                    Frequency = scanSchedule.Frequency,
                    NextScanDate = scanSchedule.NextScanDate,
                    BlackoutStart = scanSchedule.BlackoutStart,
                    BlackoutEnd = scanSchedule.BlackoutEnd,
                    IsEnabled = scanSchedule.IsEnabled,
                    CreatedAt = scanSchedule.CreatedAt,
                    CreatedBy = scanSchedule.CreatedBy,
                    UpdatedAt = scanSchedule.UpdatedAt,
                    UpdatedBy = scanSchedule.UpdatedBy,
                    IsDeleted = scanSchedule.IsDeleted,
                }).ToList(),
                Vulnerabilities = c.Vulnerabilities.Select(vulnerability => new VulnerabilityEntityDto
                {
                    VulnerabilityId = vulnerability.VulnerabilityId,
                    TenantId = vulnerability.TenantId,
                    AssetId = vulnerability.AssetId,
                    VulnerabilityCode = vulnerability.VulnerabilityCode,
                    CVEId = vulnerability.CVEId,
                    Title = vulnerability.Title,
                    Severity = vulnerability.Severity,
                    CVSS = vulnerability.CVSS,
                    DetectedDate = vulnerability.DetectedDate,
                    ResolvedDate = vulnerability.ResolvedDate,
                    Rank = vulnerability.Rank,
                    CreatedAt = vulnerability.CreatedAt,
                    CreatedBy = vulnerability.CreatedBy,
                    UpdatedAt = vulnerability.UpdatedAt,
                    UpdatedBy = vulnerability.UpdatedBy,
                    IsDeleted = vulnerability.IsDeleted,
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
                }).ToList(),
            })
            .AsNoTracking()
            .AsSplitQuery()
        .EnableCache($"AssetAdvancedGraphSpec-{assetId}");
        }
    }

    public sealed class AssetAdvancedGraphSpecV6 : Specification<Asset, AssetEntityDto>
    {
        public AssetAdvancedGraphSpecV6(
            Guid assetId,
            bool enableIntelligentProjection = true,
            bool enableSemanticAnalysis = true,
            bool enableBlueprintStrategy = true,
            int? take = null,
            int? skip = null)
        {
            Guard.Against.Default(assetId, nameof(assetId));
            Query.Where(c => c.AssetId == assetId);

            if (take.HasValue && skip.HasValue)
            {
                Query.Skip(skip.Value).Take(take.Value);
            }
            Query.Include(c => c.Merchant);
            if (enableBlueprintStrategy)
            {
            Query.Include(c => c.Merchant);

            }

            Query.Select(c => new AssetEntityDto
            {
                AssetId = c.AssetId,
                TenantId = c.TenantId,
                MerchantId = c.MerchantId,
                AssetCode = c.AssetCode,
                AssetName = c.AssetName,
                AssetType = c.AssetType,
                IPAddress = c.IPAddress,
                Hostname = c.Hostname,
                IsInCDE = c.IsInCDE,
                NetworkZone = c.NetworkZone,
                LastScanDate = c.LastScanDate,
                CreatedAt = c.CreatedAt,
                CreatedBy = c.CreatedBy,
                UpdatedAt = c.UpdatedAt,
                UpdatedBy = c.UpdatedBy,
                IsDeleted = c.IsDeleted,
                #region Intelligent Metrics
                IsAssetTypeCritical = c.AssetType >= 7,
                DaysSinceLastScanDate = (DateTime.UtcNow - c.LastScanDate).Value.Days,
                IsInCDEFlag = c.IsInCDE,
                TotalAssetControlCount = c.AssetControls.Count(),
                ActiveAssetControlCount = c.AssetControls.Count(x => !x.IsDeleted),
                LatestAssetControlUpdatedAt = c.AssetControls.Max(x => (DateTime?)x.UpdatedAt),
                TotalScanScheduleCount = c.ScanSchedules.Count(),
                ActiveScanScheduleCount = c.ScanSchedules.Count(x => !x.IsDeleted),
                CriticalScanScheduleCount = c.ScanSchedules.Count(x => x.ScanType >= 7),
                LatestScanScheduleNextScanDate = c.ScanSchedules.Max(x => (DateTime?)x.NextScanDate),
                TotalVulnerabilityCount = c.Vulnerabilities.Count(),
                ActiveVulnerabilityCount = c.Vulnerabilities.Count(x => !x.IsDeleted),
                CriticalVulnerabilityCount = c.Vulnerabilities.Count(x => x.Rank >= 7),
                LatestVulnerabilityDetectedDate = c.Vulnerabilities.Max(x => (DateTime?)x.DetectedDate),
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
                ScanSchedules = c.ScanSchedules.Select(scanSchedule => new ScanScheduleEntityDto
                {
                    ScanScheduleId = scanSchedule.ScanScheduleId,
                    TenantId = scanSchedule.TenantId,
                    AssetId = scanSchedule.AssetId,
                    ScanType = scanSchedule.ScanType,
                    Frequency = scanSchedule.Frequency,
                    NextScanDate = scanSchedule.NextScanDate,
                    BlackoutStart = scanSchedule.BlackoutStart,
                    BlackoutEnd = scanSchedule.BlackoutEnd,
                    IsEnabled = scanSchedule.IsEnabled,
                    CreatedAt = scanSchedule.CreatedAt,
                    CreatedBy = scanSchedule.CreatedBy,
                    UpdatedAt = scanSchedule.UpdatedAt,
                    UpdatedBy = scanSchedule.UpdatedBy,
                    IsDeleted = scanSchedule.IsDeleted,
                }).ToList(),
                Vulnerabilities = c.Vulnerabilities.Select(vulnerability => new VulnerabilityEntityDto
                {
                    VulnerabilityId = vulnerability.VulnerabilityId,
                    TenantId = vulnerability.TenantId,
                    AssetId = vulnerability.AssetId,
                    VulnerabilityCode = vulnerability.VulnerabilityCode,
                    CVEId = vulnerability.CVEId,
                    Title = vulnerability.Title,
                    Severity = vulnerability.Severity,
                    CVSS = vulnerability.CVSS,
                    DetectedDate = vulnerability.DetectedDate,
                    ResolvedDate = vulnerability.ResolvedDate,
                    Rank = vulnerability.Rank,
                    CreatedAt = vulnerability.CreatedAt,
                    CreatedBy = vulnerability.CreatedBy,
                    UpdatedAt = vulnerability.UpdatedAt,
                    UpdatedBy = vulnerability.UpdatedBy,
                    IsDeleted = vulnerability.IsDeleted,
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
                }).ToList(),
            })
            .AsNoTracking()
            .AsSplitQuery()
            .EnableCache($"AssetAdvancedGraphSpecV6-{assetId}");
        }
    }

    public sealed class AssetAdvancedFilterSpec : Specification<Asset>
    {
        public AssetAdvancedFilterSpec(
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
                        case "assetcode":
                            Query.Where(c => c.AssetCode.Contains(filter.Value));
                            break;
                        case "assetname":
                            Query.Where(c => c.AssetName.Contains(filter.Value));
                            break;
                        case "ipaddress":
                            Query.Where(c => c.IPAddress.Contains(filter.Value));
                            break;
                        case "hostname":
                            Query.Where(c => c.Hostname.Contains(filter.Value));
                            break;
                        case "assettype":
                            if (int.TryParse(filter.Value, out int assettype))
                            {
                                Query.Where(c => c.AssetType == assettype);
                            }
                            break;
                        case "isincde":
                            if (bool.TryParse(filter.Value, out bool isincde))
                            {
                                Query.Where(c => c.IsInCDE == isincde);
                            }
                            break;
                        case "lastscandate":
                            if (DateTime.TryParse(filter.Value, out DateTime lastscandate))
                            {
                                Query.Where(c => c.LastScanDate >= lastscandate.AddHours(-6) && c.LastScanDate <= lastscandate.AddHours(6));
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
                Query.OrderByDescending(x => x.AssetId);
            }

            Query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }

        private static IOrderedSpecificationBuilder<Asset> ApplySort(
            ISpecificationBuilder<Asset> query,
            Sort sort
        )
        {
            return sort.Direction == SortDirection.Descending
                ? query.OrderByDescending(GetSortProperty(sort.Field))
                : query.OrderBy(GetSortProperty(sort.Field));
        }

        private static IOrderedSpecificationBuilder<Asset> ApplyAdditionalSort(
            IOrderedSpecificationBuilder<Asset> query,
            Sort sort
        )
        {
            return sort.Direction == SortDirection.Descending
                ? query.ThenByDescending(GetSortProperty(sort.Field))
                : query.ThenBy(GetSortProperty(sort.Field));
        }

        private static Expression<Func<Asset, object>> GetSortProperty(
            string propertyName
        )
        {
            return propertyName.ToLower() switch
            {
                "assetcode" => c => c.AssetCode,
                "assetname" => c => c.AssetName,
                "ipaddress" => c => c.IPAddress,
                "hostname" => c => c.Hostname,
                _ => c => c.AssetId,
            };
        }
    }