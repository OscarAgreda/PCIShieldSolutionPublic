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
    public sealed class EvidenceTypeListPagedSpec : PagedSpecification<EvidenceType, EvidenceTypeEntityDto>
    {
        public EvidenceTypeListPagedSpec(int pageNumber, int pageSize)
        {
            _ = Guard.Against.NegativeOrZero(pageNumber, nameof(pageNumber));
            _ = Guard.Against.NegativeOrZero(pageSize, nameof(pageSize));

            _ = Query
                .OrderByDescending(i => i.EvidenceTypeId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            _ = Query.Select(x => new EvidenceTypeEntityDto
            {
                EvidenceTypeId = x.EvidenceTypeId,
                EvidenceTypeCode = x.EvidenceTypeCode,
                EvidenceTypeName = x.EvidenceTypeName,
                FileExtensions = x.FileExtensions,
                MaxSizeMB = x.MaxSizeMB,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt,
                CreatedBy = x.CreatedBy,
                UpdatedAt = x.UpdatedAt,
                UpdatedBy = x.UpdatedBy,
            })
            .AsNoTracking()
            .AsSplitQuery()
            .EnableCache($"EvidenceTypeListPagedSpec-{pageNumber}-{pageSize}");
        }
    }
    public sealed class EvidenceTypeSearchSpec : Specification<EvidenceType>
    {
        public EvidenceTypeSearchSpec(string searchTerm)
        {
            string searchLower = searchTerm?.ToLower() ?? string.Empty;

            Query
                .Where(c =>
                        (c.EvidenceTypeName != null && c.EvidenceTypeName.ToLower().Contains(searchLower)) ||
                        (c.EvidenceTypeCode != null && c.EvidenceTypeCode.ToLower().Contains(searchLower)) ||
                        (c.FileExtensions != null && c.FileExtensions.ToLower().Contains(searchLower))                )
                .OrderByDescending(c => c.EvidenceTypeId);
        }
    }

    public sealed class EvidenceTypeLastCreatedSpec : Specification<EvidenceType>
    {
        public EvidenceTypeLastCreatedSpec()
        {
            Query
                .OrderByDescending(c => c.CreatedAt)
                .Take(1)
                .AsNoTracking()
                .EnableCache("EvidenceTypeLastCreatedSpec");
        }
    }
    public sealed class EvidenceTypeByIdSpec : Specification<EvidenceType, EvidenceTypeEntityDto>
    {
        public EvidenceTypeByIdSpec(Guid id)
        {
            _ = Guard.Against.NullOrEmpty(id, nameof(id));

            _ = Query.Where(x => x.EvidenceTypeId == id);

            _ = Query.Select(x => new EvidenceTypeEntityDto
            {
                EvidenceTypeId = x.EvidenceTypeId,
                EvidenceTypeCode = x.EvidenceTypeCode,
                EvidenceTypeName = x.EvidenceTypeName,
                FileExtensions = x.FileExtensions,
                MaxSizeMB = x.MaxSizeMB,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt,
                CreatedBy = x.CreatedBy,
                UpdatedAt = x.UpdatedAt,
                UpdatedBy = x.UpdatedBy,

            })
            .AsNoTracking()
            .AsSplitQuery()
            .EnableCache($"EvidenceTypeByIdSpec-{id.ToString()}");
        }
    }
}

    public sealed class EvidenceTypeAdvancedFilterSpec : Specification<EvidenceType>
    {
        public EvidenceTypeAdvancedFilterSpec(
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
                        case "evidencetypename":
                            Query.Where(c => c.EvidenceTypeName.Contains(filter.Value));
                            break;
                        case "evidencetypecode":
                            Query.Where(c => c.EvidenceTypeCode.Contains(filter.Value));
                            break;
                        case "fileextensions":
                            Query.Where(c => c.FileExtensions.Contains(filter.Value));
                            break;
                        case "maxsizemb":
                            if (int.TryParse(filter.Value, out int maxsizemb))
                            {
                                Query.Where(c => c.MaxSizeMB == maxsizemb);
                            }
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
                Query.OrderByDescending(x => x.EvidenceTypeId);
            }

            Query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }

        private static IOrderedSpecificationBuilder<EvidenceType> ApplySort(
            ISpecificationBuilder<EvidenceType> query,
            Sort sort
        )
        {
            return sort.Direction == SortDirection.Descending
                ? query.OrderByDescending(GetSortProperty(sort.Field))
                : query.OrderBy(GetSortProperty(sort.Field));
        }

        private static IOrderedSpecificationBuilder<EvidenceType> ApplyAdditionalSort(
            IOrderedSpecificationBuilder<EvidenceType> query,
            Sort sort
        )
        {
            return sort.Direction == SortDirection.Descending
                ? query.ThenByDescending(GetSortProperty(sort.Field))
                : query.ThenBy(GetSortProperty(sort.Field));
        }

        private static Expression<Func<EvidenceType, object>> GetSortProperty(
            string propertyName
        )
        {
            return propertyName.ToLower() switch
            {
                "evidencetypename" => c => c.EvidenceTypeName,
                "evidencetypecode" => c => c.EvidenceTypeCode,
                "fileextensions" => c => c.FileExtensions,
                _ => c => c.EvidenceTypeId,
            };
        }
    }