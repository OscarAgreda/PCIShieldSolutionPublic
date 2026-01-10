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
    public sealed class AssetControlListPagedSpec : PagedSpecification<AssetControl, AssetControlEntityDto>
    {
        public AssetControlListPagedSpec(int pageNumber, int pageSize)
        {
            _ = Guard.Against.NegativeOrZero(pageNumber, nameof(pageNumber));
            _ = Guard.Against.NegativeOrZero(pageSize, nameof(pageSize));

            _ = Query
                .OrderByDescending(i => i.RowId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            _ = Query.Include(x => x.Asset);
            _ = Query.Include(x => x.Control);
            _ = Query.Select(x => new AssetControlEntityDto
            {
                RowId = x.RowId,
                AssetId = x.AssetId,
                ControlId = x.ControlId,
                TenantId = x.TenantId,
                IsApplicable = x.IsApplicable,
                CustomizedApproach = x.CustomizedApproach,
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
                Control = x.Control != null ? new ControlEntityDto
                {
                    ControlId = x.Control.ControlId,
                    TenantId = x.Control.TenantId,
                    ControlCode = x.Control.ControlCode,
                    RequirementNumber = x.Control.RequirementNumber,
                    ControlTitle = x.Control.ControlTitle,
                    ControlDescription = x.Control.ControlDescription,
                    TestingGuidance = x.Control.TestingGuidance,
                    FrequencyDays = x.Control.FrequencyDays,
                    IsMandatory = x.Control.IsMandatory,
                    EffectiveDate = x.Control.EffectiveDate,
                    CreatedAt = x.Control.CreatedAt,
                    CreatedBy = x.Control.CreatedBy,
                    UpdatedAt = x.Control.UpdatedAt,
                    UpdatedBy = x.Control.UpdatedBy,
                    IsDeleted = x.Control.IsDeleted,
                } : null,
            })
            .AsNoTracking()
            .AsSplitQuery()
            .EnableCache($"AssetControlListPagedSpec-{pageNumber}-{pageSize}");
        }
    }
    public sealed class AssetControlSearchSpec : Specification<AssetControl>
    {
        public AssetControlSearchSpec(string searchTerm)
        {
            string searchLower = searchTerm?.ToLower() ?? string.Empty;

            Query
                .Where(c =>
                        (c.Asset.AssetCode != null && c.Asset.AssetCode.ToLower().Contains(searchLower)) ||
                        (c.Asset.AssetName != null && c.Asset.AssetName.ToLower().Contains(searchLower)) ||
                        (c.Asset.IPAddress != null && c.Asset.IPAddress.ToLower().Contains(searchLower)) ||
                        (c.Asset.Hostname != null && c.Asset.Hostname.ToLower().Contains(searchLower)) ||
                        (c.Control.ControlCode != null && c.Control.ControlCode.ToLower().Contains(searchLower)) ||
                        (c.Control.RequirementNumber != null && c.Control.RequirementNumber.ToLower().Contains(searchLower))                )
                .OrderByDescending(c => c.RowId);
        }
    }

    public sealed class AssetControlLastCreatedSpec : Specification<AssetControl>
    {
        public AssetControlLastCreatedSpec()
        {
            Query
                .OrderByDescending(c => c.CreatedAt)
                .Take(1)
                .AsNoTracking()
                .EnableCache("AssetControlLastCreatedSpec");
        }
    }
    public sealed class AssetControlByIdSpec : Specification<AssetControl, AssetControlEntityDto>
    {
        public AssetControlByIdSpec(
        Guid assetId,
        Guid controlId
        )
        {

            _ = Guard.Against.NullOrEmpty(assetId, nameof(assetId));

            _ = Query.Where(x => x.AssetId == assetId);
            _ = Guard.Against.NullOrEmpty(controlId, nameof(controlId));

            _ = Query.Where(x => x.ControlId == controlId);

            _ = Query.Include(x => x.Asset);
            _ = Query.Include(x => x.Control);
            _ = Query.Select(x => new AssetControlEntityDto
            {
                RowId = x.RowId,
                AssetId = x.AssetId,
                ControlId = x.ControlId,
                TenantId = x.TenantId,
                IsApplicable = x.IsApplicable,
                CustomizedApproach = x.CustomizedApproach,
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
                Control = x.Control != null ? new ControlEntityDto
                {
                    ControlId = x.Control.ControlId,
                    TenantId = x.Control.TenantId,
                    ControlCode = x.Control.ControlCode,
                    RequirementNumber = x.Control.RequirementNumber,
                    ControlTitle = x.Control.ControlTitle,
                    ControlDescription = x.Control.ControlDescription,
                    TestingGuidance = x.Control.TestingGuidance,
                    FrequencyDays = x.Control.FrequencyDays,
                    IsMandatory = x.Control.IsMandatory,
                    EffectiveDate = x.Control.EffectiveDate,
                    CreatedAt = x.Control.CreatedAt,
                    CreatedBy = x.Control.CreatedBy,
                    UpdatedAt = x.Control.UpdatedAt,
                    UpdatedBy = x.Control.UpdatedBy,
                    IsDeleted = x.Control.IsDeleted,
                } : null,
            })
            .AsNoTracking()
            .AsSplitQuery()
            .EnableCache($"AssetControlByIdSpec--{assetId.ToString()}-{controlId.ToString()}");
        }
    }
}

    public sealed class AssetControlAdvancedFilterSpec : Specification<AssetControl>
    {
        public AssetControlAdvancedFilterSpec(
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
                        case "rowid":
                            if (int.TryParse(filter.Value, out int rowid))
                            {
                                Query.Where(c => c.RowId == rowid);
                            }
                            break;
                        case "isapplicable":
                            if (bool.TryParse(filter.Value, out bool isapplicable))
                            {
                                Query.Where(c => c.IsApplicable == isapplicable);
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
                Query.OrderByDescending(x => x.RowId);
            }

            Query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }

        private static IOrderedSpecificationBuilder<AssetControl> ApplySort(
            ISpecificationBuilder<AssetControl> query,
            Sort sort
        )
        {
            return sort.Direction == SortDirection.Descending
                ? query.OrderByDescending(GetSortProperty(sort.Field))
                : query.OrderBy(GetSortProperty(sort.Field));
        }

        private static IOrderedSpecificationBuilder<AssetControl> ApplyAdditionalSort(
            IOrderedSpecificationBuilder<AssetControl> query,
            Sort sort
        )
        {
            return sort.Direction == SortDirection.Descending
                ? query.ThenByDescending(GetSortProperty(sort.Field))
                : query.ThenBy(GetSortProperty(sort.Field));
        }

        private static Expression<Func<AssetControl, object>> GetSortProperty(
            string propertyName
        )
        {
            return propertyName.ToLower() switch
            {
                _ => c => c.RowId,
            };
        }
    }