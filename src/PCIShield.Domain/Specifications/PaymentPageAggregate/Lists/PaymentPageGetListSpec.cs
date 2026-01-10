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
    public sealed class PaymentPageListPagedSpec : PagedSpecification<PaymentPage, PaymentPageEntityDto>
    {
        public PaymentPageListPagedSpec(int pageNumber, int pageSize)
        {
            _ = Guard.Against.NegativeOrZero(pageNumber, nameof(pageNumber));
            _ = Guard.Against.NegativeOrZero(pageSize, nameof(pageSize));

            _ = Query
                .OrderByDescending(i => i.PaymentPageId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            _ = Query.Include(x => x.PaymentChannel);
            _ = Query.Select(x => new PaymentPageEntityDto
            {
                PaymentPageId = x.PaymentPageId,
                TenantId = x.TenantId,
                PaymentChannelId = x.PaymentChannelId,
                PageUrl = x.PageUrl,
                PageName = x.PageName,
                IsActive = x.IsActive,
                LastScriptInventory = x.LastScriptInventory,
                ScriptIntegrityHash = x.ScriptIntegrityHash,
                CreatedAt = x.CreatedAt,
                CreatedBy = x.CreatedBy,
                UpdatedAt = x.UpdatedAt,
                UpdatedBy = x.UpdatedBy,
                IsDeleted = x.IsDeleted,
                PaymentChannel = x.PaymentChannel != null ? new PaymentChannelEntityDto
                {
                    PaymentChannelId = x.PaymentChannel.PaymentChannelId,
                    TenantId = x.PaymentChannel.TenantId,
                    MerchantId = x.PaymentChannel.MerchantId,
                    ChannelCode = x.PaymentChannel.ChannelCode,
                    ChannelName = x.PaymentChannel.ChannelName,
                    ChannelType = x.PaymentChannel.ChannelType,
                    ProcessingVolume = x.PaymentChannel.ProcessingVolume,
                    IsInScope = x.PaymentChannel.IsInScope,
                    TokenizationEnabled = x.PaymentChannel.TokenizationEnabled,
                    CreatedAt = x.PaymentChannel.CreatedAt,
                    CreatedBy = x.PaymentChannel.CreatedBy,
                    UpdatedAt = x.PaymentChannel.UpdatedAt,
                    UpdatedBy = x.PaymentChannel.UpdatedBy,
                    IsDeleted = x.PaymentChannel.IsDeleted,
                } : null,
            })
            .AsNoTracking()
            .AsSplitQuery()
            .EnableCache($"PaymentPageListPagedSpec-{pageNumber}-{pageSize}");
        }
    }
    public sealed class PaymentPageSearchSpec : Specification<PaymentPage>
    {
        public PaymentPageSearchSpec(string searchTerm)
        {
            string searchLower = searchTerm?.ToLower() ?? string.Empty;

            Query
                .Where(c =>
                        (c.ScriptIntegrityHash != null && c.ScriptIntegrityHash.ToLower().Contains(searchLower)) ||
                        (c.PageName != null && c.PageName.ToLower().Contains(searchLower))                )
                .OrderByDescending(c => c.PaymentPageId);
        }
    }

    public sealed class PaymentPageLastCreatedSpec : Specification<PaymentPage>
    {
        public PaymentPageLastCreatedSpec()
        {
            Query
                .OrderByDescending(c => c.LastScriptInventory)
                .Take(1)
                .AsNoTracking()
                .EnableCache("PaymentPageLastCreatedSpec");
        }
    }
    public sealed class PaymentPageByIdSpec : Specification<PaymentPage, PaymentPageEntityDto>
    {
        public PaymentPageByIdSpec(Guid id)
        {
            _ = Guard.Against.NullOrEmpty(id, nameof(id));

            _ = Query.Where(x => x.PaymentPageId == id);

            _ = Query.Include(x => x.PaymentChannel);
            _ = Query.Include(x => x.Scripts);
            _ = Query.Select(x => new PaymentPageEntityDto
            {
                PaymentPageId = x.PaymentPageId,
                TenantId = x.TenantId,
                PaymentChannelId = x.PaymentChannelId,
                PageUrl = x.PageUrl,
                PageName = x.PageName,
                IsActive = x.IsActive,
                LastScriptInventory = x.LastScriptInventory,
                ScriptIntegrityHash = x.ScriptIntegrityHash,
                CreatedAt = x.CreatedAt,
                CreatedBy = x.CreatedBy,
                UpdatedAt = x.UpdatedAt,
                UpdatedBy = x.UpdatedBy,
                IsDeleted = x.IsDeleted,

                PaymentChannel = x.PaymentChannel != null ? new PaymentChannelEntityDto
                {
                    PaymentChannelId = x.PaymentChannel.PaymentChannelId,
                    TenantId = x.PaymentChannel.TenantId,
                    MerchantId = x.PaymentChannel.MerchantId,
                    ChannelCode = x.PaymentChannel.ChannelCode,
                    ChannelName = x.PaymentChannel.ChannelName,
                    ChannelType = x.PaymentChannel.ChannelType,
                    ProcessingVolume = x.PaymentChannel.ProcessingVolume,
                    IsInScope = x.PaymentChannel.IsInScope,
                    TokenizationEnabled = x.PaymentChannel.TokenizationEnabled,
                    CreatedAt = x.PaymentChannel.CreatedAt,
                    CreatedBy = x.PaymentChannel.CreatedBy,
                    UpdatedAt = x.PaymentChannel.UpdatedAt,
                    UpdatedBy = x.PaymentChannel.UpdatedBy,
                    IsDeleted = x.PaymentChannel.IsDeleted,
                } : null,
                Scripts = x.Scripts.Select(script => new ScriptEntityDto
                {
                    ScriptId = script.ScriptId,
                    TenantId = script.TenantId,
                    PaymentPageId = script.PaymentPageId,
                    ScriptUrl = script.ScriptUrl,
                    ScriptHash = script.ScriptHash,
                    ScriptType = script.ScriptType,
                    IsAuthorized = script.IsAuthorized,
                    FirstSeen = script.FirstSeen,
                    LastSeen = script.LastSeen,
                    CreatedAt = script.CreatedAt,
                    CreatedBy = script.CreatedBy,
                    UpdatedAt = script.UpdatedAt,
                    UpdatedBy = script.UpdatedBy,
                    IsDeleted = script.IsDeleted,
                }).ToList(),
            })
            .AsNoTracking()
            .AsSplitQuery()
            .EnableCache($"PaymentPageByIdSpec-{id.ToString()}");
        }
    }
}

    public sealed class PaymentPageAdvancedFilterSpec : Specification<PaymentPage>
    {
        public PaymentPageAdvancedFilterSpec(
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
                        case "scriptintegrityhash":
                            Query.Where(c => c.ScriptIntegrityHash.Contains(filter.Value));
                            break;
                        case "pagename":
                            Query.Where(c => c.PageName.Contains(filter.Value));
                            break;
                        case "isactive":
                            if (bool.TryParse(filter.Value, out bool isactive))
                            {
                                Query.Where(c => c.IsActive == isactive);
                            }
                            break;
                        case "lastscriptinventory":
                            if (DateTime.TryParse(filter.Value, out DateTime lastscriptinventory))
                            {
                                Query.Where(c => c.LastScriptInventory >= lastscriptinventory.AddHours(-6) && c.LastScriptInventory <= lastscriptinventory.AddHours(6));
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
                Query.OrderByDescending(x => x.PaymentPageId);
            }

            Query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }

        private static IOrderedSpecificationBuilder<PaymentPage> ApplySort(
            ISpecificationBuilder<PaymentPage> query,
            Sort sort
        )
        {
            return sort.Direction == SortDirection.Descending
                ? query.OrderByDescending(GetSortProperty(sort.Field))
                : query.OrderBy(GetSortProperty(sort.Field));
        }

        private static IOrderedSpecificationBuilder<PaymentPage> ApplyAdditionalSort(
            IOrderedSpecificationBuilder<PaymentPage> query,
            Sort sort
        )
        {
            return sort.Direction == SortDirection.Descending
                ? query.ThenByDescending(GetSortProperty(sort.Field))
                : query.ThenBy(GetSortProperty(sort.Field));
        }

        private static Expression<Func<PaymentPage, object>> GetSortProperty(
            string propertyName
        )
        {
            return propertyName.ToLower() switch
            {
                "scriptintegrityhash" => c => c.ScriptIntegrityHash,
                "pagename" => c => c.PageName,
                _ => c => c.PaymentPageId,
            };
        }
    }