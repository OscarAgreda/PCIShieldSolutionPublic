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
    public sealed class LogsListPagedSpec : PagedSpecification<Logs, LogsEntityDto>
    {
        public LogsListPagedSpec(int pageNumber, int pageSize)
        {
            _ = Guard.Against.NegativeOrZero(pageNumber, nameof(pageNumber));
            _ = Guard.Against.NegativeOrZero(pageSize, nameof(pageSize));

            _ = Query
                .OrderByDescending(i => i.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            _ = Query.Select(x => new LogsEntityDto
            {
                Id = x.Id,
                Message = x.Message,
                MessageTemplate = x.MessageTemplate,
                Level = x.Level,
                Exception = x.Exception,
                Properties = x.Properties,
            })
            .AsNoTracking()
            .AsSplitQuery()
            .EnableCache($"LogsListPagedSpec-{pageNumber}-{pageSize}");
        }
    }
    public sealed class LogsSearchSpec : Specification<Logs>
    {
        public LogsSearchSpec(string searchTerm)
        {
            string searchLower = searchTerm?.ToLower() ?? string.Empty;

            Query
                .Where(c =>
                        (c.Id != null && c.Id.ToString().ToLower().Contains(searchLower))                )
                .OrderByDescending(c => c.Id);
        }
    }

    public sealed class LogsLastCreatedSpec : Specification<Logs>
    {
        public LogsLastCreatedSpec()
        {
            Query
                .OrderByDescending(c => c.Id)
                .Take(1)
                .AsNoTracking()
                .EnableCache("LogsLastCreatedSpec");
        }
    }
    public sealed class LogsByIdSpec : Specification<Logs, LogsEntityDto>
    {
        public LogsByIdSpec(int id)
        {
            _ = Guard.Against.Null(id, nameof(id));

            _ = Query.Where(x => x.Id == id);

            _ = Query.Select(x => new LogsEntityDto
            {
                Id = x.Id,
                Message = x.Message,
                MessageTemplate = x.MessageTemplate,
                Level = x.Level,
                Exception = x.Exception,
                Properties = x.Properties,

            })
            .AsNoTracking()
            .AsSplitQuery()
            .EnableCache($"LogsByIdSpec-{id.ToString()}");
        }
    }
}

    public sealed class LogsAdvancedFilterSpec : Specification<Logs>
    {
        public LogsAdvancedFilterSpec(
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
                        case "id":
                            if (int.TryParse(filter.Value, out int id))
                            {
                                Query.Where(c => c.Id == id);
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
                Query.OrderByDescending(x => x.Id);
            }

            Query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }

        private static IOrderedSpecificationBuilder<Logs> ApplySort(
            ISpecificationBuilder<Logs> query,
            Sort sort
        )
        {
            return sort.Direction == SortDirection.Descending
                ? query.OrderByDescending(GetSortProperty(sort.Field))
                : query.OrderBy(GetSortProperty(sort.Field));
        }

        private static IOrderedSpecificationBuilder<Logs> ApplyAdditionalSort(
            IOrderedSpecificationBuilder<Logs> query,
            Sort sort
        )
        {
            return sort.Direction == SortDirection.Descending
                ? query.ThenByDescending(GetSortProperty(sort.Field))
                : query.ThenBy(GetSortProperty(sort.Field));
        }

        private static Expression<Func<Logs, object>> GetSortProperty(
            string propertyName
        )
        {
            return propertyName.ToLower() switch
            {
                _ => c => c.Id,
            };
        }
    }