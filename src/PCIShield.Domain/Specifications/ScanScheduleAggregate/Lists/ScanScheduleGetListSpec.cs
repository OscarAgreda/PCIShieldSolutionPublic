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
    public sealed class ScanScheduleListPagedSpec : PagedSpecification<ScanSchedule, ScanScheduleEntityDto>
    {
        public ScanScheduleListPagedSpec(int pageNumber, int pageSize)
        {
            _ = Guard.Against.NegativeOrZero(pageNumber, nameof(pageNumber));
            _ = Guard.Against.NegativeOrZero(pageSize, nameof(pageSize));

            _ = Query
                .OrderByDescending(i => i.ScanScheduleId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            _ = Query.Include(x => x.Asset);
            _ = Query.Select(x => new ScanScheduleEntityDto
            {
                ScanScheduleId = x.ScanScheduleId,
                TenantId = x.TenantId,
                AssetId = x.AssetId,
                ScanType = x.ScanType,
                Frequency = x.Frequency,
                NextScanDate = x.NextScanDate,
                BlackoutStart = x.BlackoutStart,
                BlackoutEnd = x.BlackoutEnd,
                IsEnabled = x.IsEnabled,
                CreatedAt = x.CreatedAt,
                CreatedBy = x.CreatedBy,
                UpdatedAt = x.UpdatedAt,
                UpdatedBy = x.UpdatedBy,
                IsDeleted = x.IsDeleted,
                Asset = x.Asset != null ? new AssetEntityDto
                {
                    AssetId = x.Asset.AssetId,
                    TenantId = x.Asset.TenantId,
                    MerchantId = x.Asset.MerchantId,
                    AssetCode = x.Asset.AssetCode,
                    AssetName = x.Asset.AssetName,
                    AssetType = x.Asset.AssetType,
                    IPAddress = x.Asset.IPAddress,
                    Hostname = x.Asset.Hostname,
                    IsInCDE = x.Asset.IsInCDE,
                    NetworkZone = x.Asset.NetworkZone,
                    LastScanDate = x.Asset.LastScanDate,
                    CreatedAt = x.Asset.CreatedAt,
                    CreatedBy = x.Asset.CreatedBy,
                    UpdatedAt = x.Asset.UpdatedAt,
                    UpdatedBy = x.Asset.UpdatedBy,
                    IsDeleted = x.Asset.IsDeleted,
                } : null,
            })
            .AsNoTracking()
            .AsSplitQuery()
            .EnableCache($"ScanScheduleListPagedSpec-{pageNumber}-{pageSize}");
        }
    }
    public sealed class ScanScheduleSearchSpec : Specification<ScanSchedule>
    {
        public ScanScheduleSearchSpec(string searchTerm)
        {
            string searchLower = searchTerm?.ToLower() ?? string.Empty;

            Query
                .Where(c =>
                        (c.Frequency != null && c.Frequency.ToLower().Contains(searchLower))                )
                .OrderByDescending(c => c.ScanScheduleId);
        }
    }

    public sealed class ScanScheduleLastCreatedSpec : Specification<ScanSchedule>
    {
        public ScanScheduleLastCreatedSpec()
        {
            Query
                .OrderByDescending(c => c.NextScanDate)
                .Take(1)
                .AsNoTracking()
                .EnableCache("ScanScheduleLastCreatedSpec");
        }
    }
    public sealed class ScanScheduleByIdSpec : Specification<ScanSchedule, ScanScheduleEntityDto>
    {
        public ScanScheduleByIdSpec(Guid id)
        {
            _ = Guard.Against.NullOrEmpty(id, nameof(id));

            _ = Query.Where(x => x.ScanScheduleId == id);

            _ = Query.Include(x => x.Asset);
            _ = Query.Select(x => new ScanScheduleEntityDto
            {
                ScanScheduleId = x.ScanScheduleId,
                TenantId = x.TenantId,
                AssetId = x.AssetId,
                ScanType = x.ScanType,
                Frequency = x.Frequency,
                NextScanDate = x.NextScanDate,
                BlackoutStart = x.BlackoutStart,
                BlackoutEnd = x.BlackoutEnd,
                IsEnabled = x.IsEnabled,
                CreatedAt = x.CreatedAt,
                CreatedBy = x.CreatedBy,
                UpdatedAt = x.UpdatedAt,
                UpdatedBy = x.UpdatedBy,
                IsDeleted = x.IsDeleted,

                Asset = x.Asset != null ? new AssetEntityDto
                {
                    AssetId = x.Asset.AssetId,
                    TenantId = x.Asset.TenantId,
                    MerchantId = x.Asset.MerchantId,
                    AssetCode = x.Asset.AssetCode,
                    AssetName = x.Asset.AssetName,
                    AssetType = x.Asset.AssetType,
                    IPAddress = x.Asset.IPAddress,
                    Hostname = x.Asset.Hostname,
                    IsInCDE = x.Asset.IsInCDE,
                    NetworkZone = x.Asset.NetworkZone,
                    LastScanDate = x.Asset.LastScanDate,
                    CreatedAt = x.Asset.CreatedAt,
                    CreatedBy = x.Asset.CreatedBy,
                    UpdatedAt = x.Asset.UpdatedAt,
                    UpdatedBy = x.Asset.UpdatedBy,
                    IsDeleted = x.Asset.IsDeleted,
                } : null,
            })
            .AsNoTracking()
            .AsSplitQuery()
            .EnableCache($"ScanScheduleByIdSpec-{id.ToString()}");
        }
    }
}

    public sealed class ScanScheduleAdvancedGraphSpecV4 : Specification<ScanSchedule, ScanScheduleEntityDto>
    {
        public ScanScheduleAdvancedGraphSpecV4(
            Guid scanScheduleId,
            int? take = null,
            int? skip = null,
            DateTime? effectiveDate = null)
        {
            Guard.Against.Default(scanScheduleId, nameof(scanScheduleId));
            Query.Where(c => c.ScanScheduleId == scanScheduleId);

            if (take.HasValue && skip.HasValue)
            {
            }
            Query.Include(c => c.Asset);

            Query.Select(c => new ScanScheduleEntityDto
            {
                ScanScheduleId = c.ScanScheduleId,
                TenantId = c.TenantId,
                AssetId = c.AssetId,
                ScanType = c.ScanType,
                Frequency = c.Frequency,
                NextScanDate = c.NextScanDate,
                BlackoutStart = c.BlackoutStart,
                BlackoutEnd = c.BlackoutEnd,
                IsEnabled = c.IsEnabled,
                CreatedAt = c.CreatedAt,
                CreatedBy = c.CreatedBy,
                UpdatedAt = c.UpdatedAt,
                UpdatedBy = c.UpdatedBy,
                IsDeleted = c.IsDeleted,
                Asset = c.Asset != null ? new AssetEntityDto
                {
                    AssetId = c.Asset.AssetId,
                    TenantId = c.Asset.TenantId,
                    MerchantId = c.Asset.MerchantId,
                    AssetCode = c.Asset.AssetCode,
                    AssetName = c.Asset.AssetName,
                    AssetType = c.Asset.AssetType,
                    IPAddress = c.Asset.IPAddress,
                    Hostname = c.Asset.Hostname,
                    IsInCDE = c.Asset.IsInCDE,
                    NetworkZone = c.Asset.NetworkZone,
                    LastScanDate = c.Asset.LastScanDate,
                    CreatedAt = c.Asset.CreatedAt,
                    CreatedBy = c.Asset.CreatedBy,
                    UpdatedAt = c.Asset.UpdatedAt,
                    UpdatedBy = c.Asset.UpdatedBy,
                    IsDeleted = c.Asset.IsDeleted,
                } : null,
            })
            .AsNoTracking()
            .AsSplitQuery()
        .EnableCache($"ScanScheduleAdvancedGraphSpec-{scanScheduleId}");
        }
    }

    public sealed class ScanScheduleAdvancedGraphSpecV6 : Specification<ScanSchedule, ScanScheduleEntityDto>
    {
        public ScanScheduleAdvancedGraphSpecV6(
            Guid scanScheduleId,
            bool enableIntelligentProjection = true,
            bool enableSemanticAnalysis = true,
            bool enableBlueprintStrategy = true,
            int? take = null,
            int? skip = null)
        {
            Guard.Against.Default(scanScheduleId, nameof(scanScheduleId));
            Query.Where(c => c.ScanScheduleId == scanScheduleId);

            if (take.HasValue && skip.HasValue)
            {
                Query.Skip(skip.Value).Take(take.Value);
            }
            Query.Include(c => c.Asset);
            if (enableBlueprintStrategy)
            {
            Query.Include(c => c.Asset);

            }

            Query.Select(c => new ScanScheduleEntityDto
            {
                ScanScheduleId = c.ScanScheduleId,
                TenantId = c.TenantId,
                AssetId = c.AssetId,
                ScanType = c.ScanType,
                Frequency = c.Frequency,
                NextScanDate = c.NextScanDate,
                BlackoutStart = c.BlackoutStart,
                BlackoutEnd = c.BlackoutEnd,
                IsEnabled = c.IsEnabled,
                CreatedAt = c.CreatedAt,
                CreatedBy = c.CreatedBy,
                UpdatedAt = c.UpdatedAt,
                UpdatedBy = c.UpdatedBy,
                IsDeleted = c.IsDeleted,
                #region Intelligent Metrics
                IsScanTypeCritical = c.ScanType >= 7,
                DaysSinceNextScanDate = (DateTime.UtcNow - c.NextScanDate).Days,
                IsEnabledFlag = c.IsEnabled,
                #endregion
                #region Semantic Pattern Fields
                #endregion
                Asset = c.Asset != null ? new AssetEntityDto
                {
                    AssetId = c.Asset.AssetId,
                    TenantId = c.Asset.TenantId,
                    MerchantId = c.Asset.MerchantId,
                    AssetCode = c.Asset.AssetCode,
                    AssetName = c.Asset.AssetName,
                    AssetType = c.Asset.AssetType,
                    IPAddress = c.Asset.IPAddress,
                    Hostname = c.Asset.Hostname,
                    IsInCDE = c.Asset.IsInCDE,
                    NetworkZone = c.Asset.NetworkZone,
                    LastScanDate = c.Asset.LastScanDate,
                    CreatedAt = c.Asset.CreatedAt,
                    CreatedBy = c.Asset.CreatedBy,
                    UpdatedAt = c.Asset.UpdatedAt,
                    UpdatedBy = c.Asset.UpdatedBy,
                    IsDeleted = c.Asset.IsDeleted,
                } : null,
            })
            .AsNoTracking()
            .AsSplitQuery()
            .EnableCache($"ScanScheduleAdvancedGraphSpecV6-{scanScheduleId}");
        }
    }

    public sealed class ScanScheduleAdvancedFilterSpec : Specification<ScanSchedule>
    {
        public ScanScheduleAdvancedFilterSpec(
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
                        case "frequency":
                            Query.Where(c => c.Frequency.Contains(filter.Value));
                            break;
                        case "scantype":
                            if (int.TryParse(filter.Value, out int scantype))
                            {
                                Query.Where(c => c.ScanType == scantype);
                            }
                            break;
                        case "nextscandate":
                            if (DateTime.TryParse(filter.Value, out DateTime nextscandate))
                            {
                                Query.Where(c => c.NextScanDate >= nextscandate.AddHours(-6) && c.NextScanDate <= nextscandate.AddHours(6));
                            }
                            break;
                        case "isenabled":
                            if (bool.TryParse(filter.Value, out bool isenabled))
                            {
                                Query.Where(c => c.IsEnabled == isenabled);
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
                Query.OrderByDescending(x => x.ScanScheduleId);
            }

            Query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }

        private static IOrderedSpecificationBuilder<ScanSchedule> ApplySort(
            ISpecificationBuilder<ScanSchedule> query,
            Sort sort
        )
        {
            return sort.Direction == SortDirection.Descending
                ? query.OrderByDescending(GetSortProperty(sort.Field))
                : query.OrderBy(GetSortProperty(sort.Field));
        }

        private static IOrderedSpecificationBuilder<ScanSchedule> ApplyAdditionalSort(
            IOrderedSpecificationBuilder<ScanSchedule> query,
            Sort sort
        )
        {
            return sort.Direction == SortDirection.Descending
                ? query.ThenByDescending(GetSortProperty(sort.Field))
                : query.ThenBy(GetSortProperty(sort.Field));
        }

        private static Expression<Func<ScanSchedule, object>> GetSortProperty(
            string propertyName
        )
        {
            return propertyName.ToLower() switch
            {
                "frequency" => c => c.Frequency,
                _ => c => c.ScanScheduleId,
            };
        }
    }