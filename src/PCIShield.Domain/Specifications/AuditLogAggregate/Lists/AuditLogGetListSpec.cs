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
    public sealed class AuditLogListPagedSpec : PagedSpecification<AuditLog, AuditLogEntityDto>
    {
        public AuditLogListPagedSpec(int pageNumber, int pageSize)
        {
            _ = Guard.Against.NegativeOrZero(pageNumber, nameof(pageNumber));
            _ = Guard.Against.NegativeOrZero(pageSize, nameof(pageSize));

            _ = Query
                .OrderByDescending(i => i.AuditLogId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            _ = Query.Select(x => new AuditLogEntityDto
            {
                AuditLogId = x.AuditLogId,
                TenantId = x.TenantId,
                EntityType = x.EntityType,
                EntityId = x.EntityId,
                Action = x.Action,
                OldValues = x.OldValues,
                NewValues = x.NewValues,
                UserId = x.UserId,
                IPAddress = x.IPAddress,
            })
            .AsNoTracking()
            .AsSplitQuery()
            .EnableCache($"AuditLogListPagedSpec-{pageNumber}-{pageSize}");
        }
    }
    public sealed class AuditLogSearchSpec : Specification<AuditLog>
    {
        public AuditLogSearchSpec(string searchTerm)
        {
            string searchLower = searchTerm?.ToLower() ?? string.Empty;

            Query
                .Where(c =>
                        (c.EntityType != null && c.EntityType.ToLower().Contains(searchLower)) ||
                        (c.Action != null && c.Action.ToLower().Contains(searchLower)) ||
                        (c.IPAddress != null && c.IPAddress.ToLower().Contains(searchLower))                )
                .OrderByDescending(c => c.AuditLogId);
        }
    }

    public sealed class AuditLogLastCreatedSpec : Specification<AuditLog>
    {
        public AuditLogLastCreatedSpec()
        {
            Query
                .OrderByDescending(c => c.AuditLogId)
                .Take(1)
                .AsNoTracking()
                .EnableCache("AuditLogLastCreatedSpec");
        }
    }
    public sealed class AuditLogByIdSpec : Specification<AuditLog, AuditLogEntityDto>
    {
        public AuditLogByIdSpec(Guid id)
        {
            _ = Guard.Against.NullOrEmpty(id, nameof(id));

            _ = Query.Where(x => x.AuditLogId == id);

            _ = Query.Select(x => new AuditLogEntityDto
            {
                AuditLogId = x.AuditLogId,
                TenantId = x.TenantId,
                EntityType = x.EntityType,
                EntityId = x.EntityId,
                Action = x.Action,
                OldValues = x.OldValues,
                NewValues = x.NewValues,
                UserId = x.UserId,
                IPAddress = x.IPAddress,

            })
            .AsNoTracking()
            .AsSplitQuery()
            .EnableCache($"AuditLogByIdSpec-{id.ToString()}");
        }
    }
}

    public sealed class AuditLogAdvancedFilterSpec : Specification<AuditLog>
    {
        public AuditLogAdvancedFilterSpec(
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
                        case "entitytype":
                            Query.Where(c => c.EntityType.Contains(filter.Value));
                            break;
                        case "action":
                            Query.Where(c => c.Action.Contains(filter.Value));
                            break;
                        case "ipaddress":
                            Query.Where(c => c.IPAddress.Contains(filter.Value));
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
                Query.OrderByDescending(x => x.AuditLogId);
            }

            Query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }

        private static IOrderedSpecificationBuilder<AuditLog> ApplySort(
            ISpecificationBuilder<AuditLog> query,
            Sort sort
        )
        {
            return sort.Direction == SortDirection.Descending
                ? query.OrderByDescending(GetSortProperty(sort.Field))
                : query.OrderBy(GetSortProperty(sort.Field));
        }

        private static IOrderedSpecificationBuilder<AuditLog> ApplyAdditionalSort(
            IOrderedSpecificationBuilder<AuditLog> query,
            Sort sort
        )
        {
            return sort.Direction == SortDirection.Descending
                ? query.ThenByDescending(GetSortProperty(sort.Field))
                : query.ThenBy(GetSortProperty(sort.Field));
        }

        private static Expression<Func<AuditLog, object>> GetSortProperty(
            string propertyName
        )
        {
            return propertyName.ToLower() switch
            {
                "entitytype" => c => c.EntityType,
                "action" => c => c.Action,
                "ipaddress" => c => c.IPAddress,
                _ => c => c.AuditLogId,
            };
        }
    }