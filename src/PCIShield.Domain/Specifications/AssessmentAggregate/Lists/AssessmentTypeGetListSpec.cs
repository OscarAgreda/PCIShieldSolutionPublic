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
    public sealed class AssessmentTypeListPagedSpec : PagedSpecification<AssessmentType, AssessmentTypeEntityDto>
    {
        public AssessmentTypeListPagedSpec(int pageNumber, int pageSize)
        {
            _ = Guard.Against.NegativeOrZero(pageNumber, nameof(pageNumber));
            _ = Guard.Against.NegativeOrZero(pageSize, nameof(pageSize));

            _ = Query
                .OrderByDescending(i => i.AssessmentTypeId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            _ = Query.Select(x => new AssessmentTypeEntityDto
            {
                AssessmentTypeId = x.AssessmentTypeId,
                AssessmentTypeCode = x.AssessmentTypeCode,
                AssessmentTypeName = x.AssessmentTypeName,
                Description = x.Description,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt,
                CreatedBy = x.CreatedBy,
                UpdatedAt = x.UpdatedAt,
                UpdatedBy = x.UpdatedBy,
            })
            .AsNoTracking()
            .AsSplitQuery()
            .EnableCache($"AssessmentTypeListPagedSpec-{pageNumber}-{pageSize}");
        }
    }
    public sealed class AssessmentTypeSearchSpec : Specification<AssessmentType>
    {
        public AssessmentTypeSearchSpec(string searchTerm)
        {
            string searchLower = searchTerm?.ToLower() ?? string.Empty;

            Query
                .Where(c =>
                        (c.AssessmentTypeName != null && c.AssessmentTypeName.ToLower().Contains(searchLower)) ||
                        (c.AssessmentTypeCode != null && c.AssessmentTypeCode.ToLower().Contains(searchLower))                )
                .OrderByDescending(c => c.AssessmentTypeId);
        }
    }

    public sealed class AssessmentTypeLastCreatedSpec : Specification<AssessmentType>
    {
        public AssessmentTypeLastCreatedSpec()
        {
            Query
                .OrderByDescending(c => c.CreatedAt)
                .Take(1)
                .AsNoTracking()
                .EnableCache("AssessmentTypeLastCreatedSpec");
        }
    }
    public sealed class AssessmentTypeByIdSpec : Specification<AssessmentType, AssessmentTypeEntityDto>
    {
        public AssessmentTypeByIdSpec(Guid id)
        {
            _ = Guard.Against.NullOrEmpty(id, nameof(id));

            _ = Query.Where(x => x.AssessmentTypeId == id);

            _ = Query.Select(x => new AssessmentTypeEntityDto
            {
                AssessmentTypeId = x.AssessmentTypeId,
                AssessmentTypeCode = x.AssessmentTypeCode,
                AssessmentTypeName = x.AssessmentTypeName,
                Description = x.Description,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt,
                CreatedBy = x.CreatedBy,
                UpdatedAt = x.UpdatedAt,
                UpdatedBy = x.UpdatedBy,

            })
            .AsNoTracking()
            .AsSplitQuery()
            .EnableCache($"AssessmentTypeByIdSpec-{id.ToString()}");
        }
    }
}

    public sealed class AssessmentTypeAdvancedFilterSpec : Specification<AssessmentType>
    {
        public AssessmentTypeAdvancedFilterSpec(
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
                        case "assessmenttypename":
                            Query.Where(c => c.AssessmentTypeName.Contains(filter.Value));
                            break;
                        case "assessmenttypecode":
                            Query.Where(c => c.AssessmentTypeCode.Contains(filter.Value));
                            break;
                        case "isactive":
                            if (bool.TryParse(filter.Value, out bool isactive))
                            {
                                Query.Where(c => c.IsActive == isactive);
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
                Query.OrderByDescending(x => x.AssessmentTypeId);
            }

            Query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }

        private static IOrderedSpecificationBuilder<AssessmentType> ApplySort(
            ISpecificationBuilder<AssessmentType> query,
            Sort sort
        )
        {
            return sort.Direction == SortDirection.Descending
                ? query.OrderByDescending(GetSortProperty(sort.Field))
                : query.OrderBy(GetSortProperty(sort.Field));
        }

        private static IOrderedSpecificationBuilder<AssessmentType> ApplyAdditionalSort(
            IOrderedSpecificationBuilder<AssessmentType> query,
            Sort sort
        )
        {
            return sort.Direction == SortDirection.Descending
                ? query.ThenByDescending(GetSortProperty(sort.Field))
                : query.ThenBy(GetSortProperty(sort.Field));
        }

        private static Expression<Func<AssessmentType, object>> GetSortProperty(
            string propertyName
        )
        {
            return propertyName.ToLower() switch
            {
                "assessmenttypename" => c => c.AssessmentTypeName,
                "assessmenttypecode" => c => c.AssessmentTypeCode,
                _ => c => c.AssessmentTypeId,
            };
        }
    }