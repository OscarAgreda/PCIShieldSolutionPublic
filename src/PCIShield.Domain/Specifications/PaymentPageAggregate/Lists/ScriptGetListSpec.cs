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
    public sealed class ScriptListPagedSpec : PagedSpecification<Script, ScriptEntityDto>
    {
        public ScriptListPagedSpec(int pageNumber, int pageSize)
        {
            _ = Guard.Against.NegativeOrZero(pageNumber, nameof(pageNumber));
            _ = Guard.Against.NegativeOrZero(pageSize, nameof(pageSize));

            _ = Query
                .OrderByDescending(i => i.ScriptId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            _ = Query.Include(x => x.PaymentPage);
            _ = Query.Select(x => new ScriptEntityDto
            {
                ScriptId = x.ScriptId,
                TenantId = x.TenantId,
                PaymentPageId = x.PaymentPageId,
                ScriptUrl = x.ScriptUrl,
                ScriptHash = x.ScriptHash,
                ScriptType = x.ScriptType,
                IsAuthorized = x.IsAuthorized,
                FirstSeen = x.FirstSeen,
                LastSeen = x.LastSeen,
                CreatedAt = x.CreatedAt,
                CreatedBy = x.CreatedBy,
                UpdatedAt = x.UpdatedAt,
                UpdatedBy = x.UpdatedBy,
                IsDeleted = x.IsDeleted,
                PaymentPage = x.PaymentPage != null ? new PaymentPageEntityDto
                {
                    PaymentPageId = x.PaymentPage.PaymentPageId,
                    TenantId = x.PaymentPage.TenantId,
                    PaymentChannelId = x.PaymentPage.PaymentChannelId,
                    PageUrl = x.PaymentPage.PageUrl,
                    PageName = x.PaymentPage.PageName,
                    IsActive = x.PaymentPage.IsActive,
                    LastScriptInventory = x.PaymentPage.LastScriptInventory,
                    ScriptIntegrityHash = x.PaymentPage.ScriptIntegrityHash,
                    CreatedAt = x.PaymentPage.CreatedAt,
                    CreatedBy = x.PaymentPage.CreatedBy,
                    UpdatedAt = x.PaymentPage.UpdatedAt,
                    UpdatedBy = x.PaymentPage.UpdatedBy,
                    IsDeleted = x.PaymentPage.IsDeleted,
                } : null,
            })
            .AsNoTracking()
            .AsSplitQuery()
            .EnableCache($"ScriptListPagedSpec-{pageNumber}-{pageSize}");
        }
    }
    public sealed class ScriptSearchSpec : Specification<Script>
    {
        public ScriptSearchSpec(string searchTerm)
        {
            string searchLower = searchTerm?.ToLower() ?? string.Empty;

            Query
                .Where(c =>
                        (c.ScriptHash != null && c.ScriptHash.ToLower().Contains(searchLower)) ||
                        (c.ScriptType != null && c.ScriptType.ToLower().Contains(searchLower))                )
                .OrderByDescending(c => c.ScriptId);
        }
    }

    public sealed class ScriptLastCreatedSpec : Specification<Script>
    {
        public ScriptLastCreatedSpec()
        {
            Query
                .OrderByDescending(c => c.FirstSeen)
                .Take(1)
                .AsNoTracking()
                .EnableCache("ScriptLastCreatedSpec");
        }
    }
    public sealed class ScriptByIdSpec : Specification<Script, ScriptEntityDto>
    {
        public ScriptByIdSpec(Guid id)
        {
            _ = Guard.Against.NullOrEmpty(id, nameof(id));

            _ = Query.Where(x => x.ScriptId == id);

            _ = Query.Include(x => x.PaymentPage);
            _ = Query.Select(x => new ScriptEntityDto
            {
                ScriptId = x.ScriptId,
                TenantId = x.TenantId,
                PaymentPageId = x.PaymentPageId,
                ScriptUrl = x.ScriptUrl,
                ScriptHash = x.ScriptHash,
                ScriptType = x.ScriptType,
                IsAuthorized = x.IsAuthorized,
                FirstSeen = x.FirstSeen,
                LastSeen = x.LastSeen,
                CreatedAt = x.CreatedAt,
                CreatedBy = x.CreatedBy,
                UpdatedAt = x.UpdatedAt,
                UpdatedBy = x.UpdatedBy,
                IsDeleted = x.IsDeleted,

                PaymentPage = x.PaymentPage != null ? new PaymentPageEntityDto
                {
                    PaymentPageId = x.PaymentPage.PaymentPageId,
                    TenantId = x.PaymentPage.TenantId,
                    PaymentChannelId = x.PaymentPage.PaymentChannelId,
                    PageUrl = x.PaymentPage.PageUrl,
                    PageName = x.PaymentPage.PageName,
                    IsActive = x.PaymentPage.IsActive,
                    LastScriptInventory = x.PaymentPage.LastScriptInventory,
                    ScriptIntegrityHash = x.PaymentPage.ScriptIntegrityHash,
                    CreatedAt = x.PaymentPage.CreatedAt,
                    CreatedBy = x.PaymentPage.CreatedBy,
                    UpdatedAt = x.PaymentPage.UpdatedAt,
                    UpdatedBy = x.PaymentPage.UpdatedBy,
                    IsDeleted = x.PaymentPage.IsDeleted,
                } : null,
            })
            .AsNoTracking()
            .AsSplitQuery()
            .EnableCache($"ScriptByIdSpec-{id.ToString()}");
        }
    }
}

    public sealed class ScriptAdvancedFilterSpec : Specification<Script>
    {
        public ScriptAdvancedFilterSpec(
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
                        case "scripthash":
                            Query.Where(c => c.ScriptHash.Contains(filter.Value));
                            break;
                        case "scripttype":
                            Query.Where(c => c.ScriptType.Contains(filter.Value));
                            break;
                        case "isauthorized":
                            if (bool.TryParse(filter.Value, out bool isauthorized))
                            {
                                Query.Where(c => c.IsAuthorized == isauthorized);
                            }
                            break;
                        case "firstseen":
                            if (DateTime.TryParse(filter.Value, out DateTime firstseen))
                            {
                                Query.Where(c => c.FirstSeen >= firstseen.AddHours(-6) && c.FirstSeen <= firstseen.AddHours(6));
                            }
                            break;
                        case "lastseen":
                            if (DateTime.TryParse(filter.Value, out DateTime lastseen))
                            {
                                Query.Where(c => c.LastSeen >= lastseen.AddHours(-6) && c.LastSeen <= lastseen.AddHours(6));
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
                Query.OrderByDescending(x => x.ScriptId);
            }

            Query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }

        private static IOrderedSpecificationBuilder<Script> ApplySort(
            ISpecificationBuilder<Script> query,
            Sort sort
        )
        {
            return sort.Direction == SortDirection.Descending
                ? query.OrderByDescending(GetSortProperty(sort.Field))
                : query.OrderBy(GetSortProperty(sort.Field));
        }

        private static IOrderedSpecificationBuilder<Script> ApplyAdditionalSort(
            IOrderedSpecificationBuilder<Script> query,
            Sort sort
        )
        {
            return sort.Direction == SortDirection.Descending
                ? query.ThenByDescending(GetSortProperty(sort.Field))
                : query.ThenBy(GetSortProperty(sort.Field));
        }

        private static Expression<Func<Script, object>> GetSortProperty(
            string propertyName
        )
        {
            return propertyName.ToLower() switch
            {
                "scripthash" => c => c.ScriptHash,
                "scripttype" => c => c.ScriptType,
                _ => c => c.ScriptId,
            };
        }
    }