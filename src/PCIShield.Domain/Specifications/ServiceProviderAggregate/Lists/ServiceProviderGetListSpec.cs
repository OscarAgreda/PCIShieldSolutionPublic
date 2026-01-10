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
    public sealed class ServiceProviderListPagedSpec : PagedSpecification<ServiceProvider, ServiceProviderEntityDto>
    {
        public ServiceProviderListPagedSpec(int pageNumber, int pageSize)
        {
            _ = Guard.Against.NegativeOrZero(pageNumber, nameof(pageNumber));
            _ = Guard.Against.NegativeOrZero(pageSize, nameof(pageSize));

            _ = Query
                .OrderByDescending(i => i.ServiceProviderId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            _ = Query.Include(x => x.Merchant);
            _ = Query.Select(x => new ServiceProviderEntityDto
            {
                ServiceProviderId = x.ServiceProviderId,
                TenantId = x.TenantId,
                MerchantId = x.MerchantId,
                ProviderName = x.ProviderName,
                ServiceType = x.ServiceType,
                IsPCICompliant = x.IsPCICompliant,
                AOCExpiryDate = x.AOCExpiryDate,
                ResponsibilityMatrix = x.ResponsibilityMatrix,
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
            .EnableCache($"ServiceProviderListPagedSpec-{pageNumber}-{pageSize}");
        }
    }
    public sealed class ServiceProviderSearchSpec : Specification<ServiceProvider>
    {
        public ServiceProviderSearchSpec(string searchTerm)
        {
            string searchLower = searchTerm?.ToLower() ?? string.Empty;

            Query
                .Where(c =>
                        (c.ServiceType != null && c.ServiceType.ToLower().Contains(searchLower)) ||
                        (c.ProviderName != null && c.ProviderName.ToLower().Contains(searchLower))                )
                .OrderByDescending(c => c.ServiceProviderId);
        }
    }

    public sealed class ServiceProviderLastCreatedSpec : Specification<ServiceProvider>
    {
        public ServiceProviderLastCreatedSpec()
        {
            Query
                .OrderByDescending(c => c.AOCExpiryDate)
                .Take(1)
                .AsNoTracking()
                .EnableCache("ServiceProviderLastCreatedSpec");
        }
    }
    public sealed class ServiceProviderByIdSpec : Specification<ServiceProvider, ServiceProviderEntityDto>
    {
        public ServiceProviderByIdSpec(Guid id)
        {
            _ = Guard.Against.NullOrEmpty(id, nameof(id));

            _ = Query.Where(x => x.ServiceProviderId == id);

            _ = Query.Include(x => x.Merchant);
            _ = Query.Select(x => new ServiceProviderEntityDto
            {
                ServiceProviderId = x.ServiceProviderId,
                TenantId = x.TenantId,
                MerchantId = x.MerchantId,
                ProviderName = x.ProviderName,
                ServiceType = x.ServiceType,
                IsPCICompliant = x.IsPCICompliant,
                AOCExpiryDate = x.AOCExpiryDate,
                ResponsibilityMatrix = x.ResponsibilityMatrix,
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
            .EnableCache($"ServiceProviderByIdSpec-{id.ToString()}");
        }
    }
}

    public sealed class ServiceProviderAdvancedFilterSpec : Specification<ServiceProvider>
    {
        public ServiceProviderAdvancedFilterSpec(
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
                        case "servicetype":
                            Query.Where(c => c.ServiceType.Contains(filter.Value));
                            break;
                        case "providername":
                            Query.Where(c => c.ProviderName.Contains(filter.Value));
                            break;
                        case "ispcicompliant":
                            if (bool.TryParse(filter.Value, out bool ispcicompliant))
                            {
                                Query.Where(c => c.IsPCICompliant == ispcicompliant);
                            }
                            break;
                        case "aocexpirydate":
                            if (DateTime.TryParse(filter.Value, out DateTime aocexpirydate))
                            {
                                Query.Where(c => c.AOCExpiryDate >= aocexpirydate.AddHours(-6) && c.AOCExpiryDate <= aocexpirydate.AddHours(6));
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
                Query.OrderByDescending(x => x.ServiceProviderId);
            }

            Query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }

        private static IOrderedSpecificationBuilder<ServiceProvider> ApplySort(
            ISpecificationBuilder<ServiceProvider> query,
            Sort sort
        )
        {
            return sort.Direction == SortDirection.Descending
                ? query.OrderByDescending(GetSortProperty(sort.Field))
                : query.OrderBy(GetSortProperty(sort.Field));
        }

        private static IOrderedSpecificationBuilder<ServiceProvider> ApplyAdditionalSort(
            IOrderedSpecificationBuilder<ServiceProvider> query,
            Sort sort
        )
        {
            return sort.Direction == SortDirection.Descending
                ? query.ThenByDescending(GetSortProperty(sort.Field))
                : query.ThenBy(GetSortProperty(sort.Field));
        }

        private static Expression<Func<ServiceProvider, object>> GetSortProperty(
            string propertyName
        )
        {
            return propertyName.ToLower() switch
            {
                "servicetype" => c => c.ServiceType,
                "providername" => c => c.ProviderName,
                _ => c => c.ServiceProviderId,
            };
        }
    }