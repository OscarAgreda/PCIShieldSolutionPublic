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
    public sealed class ControlCategoryListPagedSpec : PagedSpecification<ControlCategory, ControlCategoryEntityDto>
    {
        public ControlCategoryListPagedSpec(int pageNumber, int pageSize)
        {
            _ = Guard.Against.NegativeOrZero(pageNumber, nameof(pageNumber));
            _ = Guard.Against.NegativeOrZero(pageSize, nameof(pageSize));

            _ = Query
                .OrderByDescending(i => i.ControlCategoryId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            _ = Query.Select(x => new ControlCategoryEntityDto
            {
                ControlCategoryId = x.ControlCategoryId,
                ControlCategoryCode = x.ControlCategoryCode,
                ControlCategoryName = x.ControlCategoryName,
                RequirementSection = x.RequirementSection,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt,
                CreatedBy = x.CreatedBy,
                UpdatedAt = x.UpdatedAt,
                UpdatedBy = x.UpdatedBy,
            })
            .AsNoTracking()
            .AsSplitQuery()
            .EnableCache($"ControlCategoryListPagedSpec-{pageNumber}-{pageSize}");
        }
    }
    public sealed class ControlCategorySearchSpec : Specification<ControlCategory>
    {
        public ControlCategorySearchSpec(string searchTerm)
        {
            string searchLower = searchTerm?.ToLower() ?? string.Empty;

            Query
                .Where(c =>
                        (c.ControlCategoryName != null && c.ControlCategoryName.ToLower().Contains(searchLower)) ||
                        (c.ControlCategoryCode != null && c.ControlCategoryCode.ToLower().Contains(searchLower)) ||
                        (c.RequirementSection != null && c.RequirementSection.ToLower().Contains(searchLower))                )
                .OrderByDescending(c => c.ControlCategoryId);
        }
    }

    public sealed class ControlCategoryLastCreatedSpec : Specification<ControlCategory>
    {
        public ControlCategoryLastCreatedSpec()
        {
            Query
                .OrderByDescending(c => c.CreatedAt)
                .Take(1)
                .AsNoTracking()
                .EnableCache("ControlCategoryLastCreatedSpec");
        }
    }
    public sealed class ControlCategoryByIdSpec : Specification<ControlCategory, ControlCategoryEntityDto>
    {
        public ControlCategoryByIdSpec(Guid id)
        {
            _ = Guard.Against.NullOrEmpty(id, nameof(id));

            _ = Query.Where(x => x.ControlCategoryId == id);

            _ = Query.Select(x => new ControlCategoryEntityDto
            {
                ControlCategoryId = x.ControlCategoryId,
                ControlCategoryCode = x.ControlCategoryCode,
                ControlCategoryName = x.ControlCategoryName,
                RequirementSection = x.RequirementSection,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt,
                CreatedBy = x.CreatedBy,
                UpdatedAt = x.UpdatedAt,
                UpdatedBy = x.UpdatedBy,

            })
            .AsNoTracking()
            .AsSplitQuery()
            .EnableCache($"ControlCategoryByIdSpec-{id.ToString()}");
        }
    }
}

    public sealed class ControlCategoryAdvancedFilterSpec : Specification<ControlCategory>
    {
        public ControlCategoryAdvancedFilterSpec(
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
                        case "controlcategoryname":
                            Query.Where(c => c.ControlCategoryName.Contains(filter.Value));
                            break;
                        case "controlcategorycode":
                            Query.Where(c => c.ControlCategoryCode.Contains(filter.Value));
                            break;
                        case "requirementsection":
                            Query.Where(c => c.RequirementSection.Contains(filter.Value));
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
                Query.OrderByDescending(x => x.ControlCategoryId);
            }

            Query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }

        private static IOrderedSpecificationBuilder<ControlCategory> ApplySort(
            ISpecificationBuilder<ControlCategory> query,
            Sort sort
        )
        {
            return sort.Direction == SortDirection.Descending
                ? query.OrderByDescending(GetSortProperty(sort.Field))
                : query.OrderBy(GetSortProperty(sort.Field));
        }

        private static IOrderedSpecificationBuilder<ControlCategory> ApplyAdditionalSort(
            IOrderedSpecificationBuilder<ControlCategory> query,
            Sort sort
        )
        {
            return sort.Direction == SortDirection.Descending
                ? query.ThenByDescending(GetSortProperty(sort.Field))
                : query.ThenBy(GetSortProperty(sort.Field));
        }

        private static Expression<Func<ControlCategory, object>> GetSortProperty(
            string propertyName
        )
        {
            return propertyName.ToLower() switch
            {
                "controlcategoryname" => c => c.ControlCategoryName,
                "controlcategorycode" => c => c.ControlCategoryCode,
                "requirementsection" => c => c.RequirementSection,
                _ => c => c.ControlCategoryId,
            };
        }
    }