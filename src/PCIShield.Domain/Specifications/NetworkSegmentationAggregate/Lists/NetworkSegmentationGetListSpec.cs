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
    public sealed class NetworkSegmentationListPagedSpec : PagedSpecification<NetworkSegmentation, NetworkSegmentationEntityDto>
    {
        public NetworkSegmentationListPagedSpec(int pageNumber, int pageSize)
        {
            _ = Guard.Against.NegativeOrZero(pageNumber, nameof(pageNumber));
            _ = Guard.Against.NegativeOrZero(pageSize, nameof(pageSize));

            _ = Query
                .OrderByDescending(i => i.NetworkSegmentationId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            _ = Query.Include(x => x.Merchant);
            _ = Query.Select(x => new NetworkSegmentationEntityDto
            {
                NetworkSegmentationId = x.NetworkSegmentationId,
                TenantId = x.TenantId,
                MerchantId = x.MerchantId,
                SegmentName = x.SegmentName,
                VLANId = x.VLANId,
                IPRange = x.IPRange,
                FirewallRules = x.FirewallRules,
                IsInCDE = x.IsInCDE,
                LastValidated = x.LastValidated,
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
            .EnableCache($"NetworkSegmentationListPagedSpec-{pageNumber}-{pageSize}");
        }
    }
    public sealed class NetworkSegmentationSearchSpec : Specification<NetworkSegmentation>
    {
        public NetworkSegmentationSearchSpec(string searchTerm)
        {
            string searchLower = searchTerm?.ToLower() ?? string.Empty;

            Query
                .Where(c =>
                        (c.SegmentName != null && c.SegmentName.ToLower().Contains(searchLower)) ||
                        (c.IPRange != null && c.IPRange.ToLower().Contains(searchLower))                )
                .OrderByDescending(c => c.NetworkSegmentationId);
        }
    }

    public sealed class NetworkSegmentationLastCreatedSpec : Specification<NetworkSegmentation>
    {
        public NetworkSegmentationLastCreatedSpec()
        {
            Query
                .OrderByDescending(c => c.LastValidated)
                .Take(1)
                .AsNoTracking()
                .EnableCache("NetworkSegmentationLastCreatedSpec");
        }
    }
    public sealed class NetworkSegmentationByIdSpec : Specification<NetworkSegmentation, NetworkSegmentationEntityDto>
    {
        public NetworkSegmentationByIdSpec(Guid id)
        {
            _ = Guard.Against.NullOrEmpty(id, nameof(id));

            _ = Query.Where(x => x.NetworkSegmentationId == id);

            _ = Query.Include(x => x.Merchant);
            _ = Query.Select(x => new NetworkSegmentationEntityDto
            {
                NetworkSegmentationId = x.NetworkSegmentationId,
                TenantId = x.TenantId,
                MerchantId = x.MerchantId,
                SegmentName = x.SegmentName,
                VLANId = x.VLANId,
                IPRange = x.IPRange,
                FirewallRules = x.FirewallRules,
                IsInCDE = x.IsInCDE,
                LastValidated = x.LastValidated,
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
            .EnableCache($"NetworkSegmentationByIdSpec-{id.ToString()}");
        }
    }
}

    public sealed class NetworkSegmentationAdvancedFilterSpec : Specification<NetworkSegmentation>
    {
        public NetworkSegmentationAdvancedFilterSpec(
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
                        case "segmentname":
                            Query.Where(c => c.SegmentName.Contains(filter.Value));
                            break;
                        case "iprange":
                            Query.Where(c => c.IPRange.Contains(filter.Value));
                            break;
                        case "vlanid":
                            if (int.TryParse(filter.Value, out int vlanid))
                            {
                                Query.Where(c => c.VLANId == vlanid);
                            }
                            break;
                        case "isincde":
                            if (bool.TryParse(filter.Value, out bool isincde))
                            {
                                Query.Where(c => c.IsInCDE == isincde);
                            }
                            break;
                        case "lastvalidated":
                            if (DateTime.TryParse(filter.Value, out DateTime lastvalidated))
                            {
                                Query.Where(c => c.LastValidated >= lastvalidated.AddHours(-6) && c.LastValidated <= lastvalidated.AddHours(6));
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
                Query.OrderByDescending(x => x.NetworkSegmentationId);
            }

            Query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }

        private static IOrderedSpecificationBuilder<NetworkSegmentation> ApplySort(
            ISpecificationBuilder<NetworkSegmentation> query,
            Sort sort
        )
        {
            return sort.Direction == SortDirection.Descending
                ? query.OrderByDescending(GetSortProperty(sort.Field))
                : query.OrderBy(GetSortProperty(sort.Field));
        }

        private static IOrderedSpecificationBuilder<NetworkSegmentation> ApplyAdditionalSort(
            IOrderedSpecificationBuilder<NetworkSegmentation> query,
            Sort sort
        )
        {
            return sort.Direction == SortDirection.Descending
                ? query.ThenByDescending(GetSortProperty(sort.Field))
                : query.ThenBy(GetSortProperty(sort.Field));
        }

        private static Expression<Func<NetworkSegmentation, object>> GetSortProperty(
            string propertyName
        )
        {
            return propertyName.ToLower() switch
            {
                "segmentname" => c => c.SegmentName,
                "iprange" => c => c.IPRange,
                _ => c => c.NetworkSegmentationId,
            };
        }
    }