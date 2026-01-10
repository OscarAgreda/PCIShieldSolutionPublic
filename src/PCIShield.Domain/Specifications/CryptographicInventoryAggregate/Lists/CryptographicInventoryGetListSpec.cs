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
    public sealed class CryptographicInventoryListPagedSpec : PagedSpecification<CryptographicInventory, CryptographicInventoryEntityDto>
    {
        public CryptographicInventoryListPagedSpec(int pageNumber, int pageSize)
        {
            _ = Guard.Against.NegativeOrZero(pageNumber, nameof(pageNumber));
            _ = Guard.Against.NegativeOrZero(pageSize, nameof(pageSize));

            _ = Query
                .OrderByDescending(i => i.CryptographicInventoryId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            _ = Query.Include(x => x.Merchant);
            _ = Query.Select(x => new CryptographicInventoryEntityDto
            {
                CryptographicInventoryId = x.CryptographicInventoryId,
                TenantId = x.TenantId,
                MerchantId = x.MerchantId,
                KeyName = x.KeyName,
                KeyType = x.KeyType,
                Algorithm = x.Algorithm,
                KeyLength = x.KeyLength,
                KeyLocation = x.KeyLocation,
                CreationDate = x.CreationDate,
                LastRotationDate = x.LastRotationDate,
                NextRotationDue = x.NextRotationDue,
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
            .EnableCache($"CryptographicInventoryListPagedSpec-{pageNumber}-{pageSize}");
        }
    }
    public sealed class CryptographicInventorySearchSpec : Specification<CryptographicInventory>
    {
        public CryptographicInventorySearchSpec(string searchTerm)
        {
            string searchLower = searchTerm?.ToLower() ?? string.Empty;

            Query
                .Where(c =>
                        (c.KeyType != null && c.KeyType.ToLower().Contains(searchLower)) ||
                        (c.KeyName != null && c.KeyName.ToLower().Contains(searchLower)) ||
                        (c.Algorithm != null && c.Algorithm.ToLower().Contains(searchLower)) ||
                        (c.KeyLocation != null && c.KeyLocation.ToLower().Contains(searchLower))                )
                .OrderByDescending(c => c.CryptographicInventoryId);
        }
    }

    public sealed class CryptographicInventoryLastCreatedSpec : Specification<CryptographicInventory>
    {
        public CryptographicInventoryLastCreatedSpec()
        {
            Query
                .OrderByDescending(c => c.CreationDate)
                .Take(1)
                .AsNoTracking()
                .EnableCache("CryptographicInventoryLastCreatedSpec");
        }
    }
    public sealed class CryptographicInventoryByIdSpec : Specification<CryptographicInventory, CryptographicInventoryEntityDto>
    {
        public CryptographicInventoryByIdSpec(Guid id)
        {
            _ = Guard.Against.NullOrEmpty(id, nameof(id));

            _ = Query.Where(x => x.CryptographicInventoryId == id);

            _ = Query.Include(x => x.Merchant);
            _ = Query.Select(x => new CryptographicInventoryEntityDto
            {
                CryptographicInventoryId = x.CryptographicInventoryId,
                TenantId = x.TenantId,
                MerchantId = x.MerchantId,
                KeyName = x.KeyName,
                KeyType = x.KeyType,
                Algorithm = x.Algorithm,
                KeyLength = x.KeyLength,
                KeyLocation = x.KeyLocation,
                CreationDate = x.CreationDate,
                LastRotationDate = x.LastRotationDate,
                NextRotationDue = x.NextRotationDue,
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
            .EnableCache($"CryptographicInventoryByIdSpec-{id.ToString()}");
        }
    }
}

    public sealed class CryptographicInventoryAdvancedFilterSpec : Specification<CryptographicInventory>
    {
        public CryptographicInventoryAdvancedFilterSpec(
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
                        case "keytype":
                            Query.Where(c => c.KeyType.Contains(filter.Value));
                            break;
                        case "keyname":
                            Query.Where(c => c.KeyName.Contains(filter.Value));
                            break;
                        case "algorithm":
                            Query.Where(c => c.Algorithm.Contains(filter.Value));
                            break;
                        case "keylocation":
                            Query.Where(c => c.KeyLocation.Contains(filter.Value));
                            break;
                        case "keylength":
                            if (int.TryParse(filter.Value, out int keylength))
                            {
                                Query.Where(c => c.KeyLength == keylength);
                            }
                            break;
                        case "creationdate":
                            if (DateTime.TryParse(filter.Value, out DateTime creationdate))
                            {
                                Query.Where(c => c.CreationDate >= creationdate.AddHours(-6) && c.CreationDate <= creationdate.AddHours(6));
                            }
                            break;
                        case "lastrotationdate":
                            if (DateTime.TryParse(filter.Value, out DateTime lastrotationdate))
                            {
                                Query.Where(c => c.LastRotationDate >= lastrotationdate.AddHours(-6) && c.LastRotationDate <= lastrotationdate.AddHours(6));
                            }
                            break;
                        case "nextrotationdue":
                            if (DateTime.TryParse(filter.Value, out DateTime nextrotationdue))
                            {
                                Query.Where(c => c.NextRotationDue >= nextrotationdue.AddHours(-6) && c.NextRotationDue <= nextrotationdue.AddHours(6));
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
                Query.OrderByDescending(x => x.CryptographicInventoryId);
            }

            Query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }

        private static IOrderedSpecificationBuilder<CryptographicInventory> ApplySort(
            ISpecificationBuilder<CryptographicInventory> query,
            Sort sort
        )
        {
            return sort.Direction == SortDirection.Descending
                ? query.OrderByDescending(GetSortProperty(sort.Field))
                : query.OrderBy(GetSortProperty(sort.Field));
        }

        private static IOrderedSpecificationBuilder<CryptographicInventory> ApplyAdditionalSort(
            IOrderedSpecificationBuilder<CryptographicInventory> query,
            Sort sort
        )
        {
            return sort.Direction == SortDirection.Descending
                ? query.ThenByDescending(GetSortProperty(sort.Field))
                : query.ThenBy(GetSortProperty(sort.Field));
        }

        private static Expression<Func<CryptographicInventory, object>> GetSortProperty(
            string propertyName
        )
        {
            return propertyName.ToLower() switch
            {
                "keytype" => c => c.KeyType,
                "keyname" => c => c.KeyName,
                "algorithm" => c => c.Algorithm,
                "keylocation" => c => c.KeyLocation,
                _ => c => c.CryptographicInventoryId,
            };
        }
    }