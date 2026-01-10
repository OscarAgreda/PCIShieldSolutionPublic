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
    public sealed class ComplianceOfficerListPagedSpec : PagedSpecification<ComplianceOfficer, ComplianceOfficerEntityDto>
    {
        public ComplianceOfficerListPagedSpec(int pageNumber, int pageSize)
        {
            _ = Guard.Against.NegativeOrZero(pageNumber, nameof(pageNumber));
            _ = Guard.Against.NegativeOrZero(pageSize, nameof(pageSize));

            _ = Query
                .OrderByDescending(i => i.ComplianceOfficerId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            _ = Query.Include(x => x.Merchant);
            _ = Query.Select(x => new ComplianceOfficerEntityDto
            {
                ComplianceOfficerId = x.ComplianceOfficerId,
                TenantId = x.TenantId,
                MerchantId = x.MerchantId,
                OfficerCode = x.OfficerCode,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                Phone = x.Phone,
                CertificationLevel = x.CertificationLevel,
                IsActive = x.IsActive,
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
            .EnableCache($"ComplianceOfficerListPagedSpec-{pageNumber}-{pageSize}");
        }
    }
    public sealed class ComplianceOfficerSearchSpec : Specification<ComplianceOfficer>
    {
        public ComplianceOfficerSearchSpec(string searchTerm)
        {
            string searchLower = searchTerm?.ToLower() ?? string.Empty;

            Query
                .Where(c =>
                        (c.FirstName != null && c.FirstName.ToLower().Contains(searchLower)) ||
                        (c.LastName != null && c.LastName.ToLower().Contains(searchLower)) ||
                        (c.OfficerCode != null && c.OfficerCode.ToLower().Contains(searchLower)) ||
                        (c.Phone != null && c.Phone.ToLower().Contains(searchLower))                )
                .OrderByDescending(c => c.ComplianceOfficerId);
        }
    }

    public sealed class ComplianceOfficerLastCreatedSpec : Specification<ComplianceOfficer>
    {
        public ComplianceOfficerLastCreatedSpec()
        {
            Query
                .OrderByDescending(c => c.CreatedAt)
                .Take(1)
                .AsNoTracking()
                .EnableCache("ComplianceOfficerLastCreatedSpec");
        }
    }
    public sealed class ComplianceOfficerByIdSpec : Specification<ComplianceOfficer, ComplianceOfficerEntityDto>
    {
        public ComplianceOfficerByIdSpec(Guid id)
        {
            _ = Guard.Against.NullOrEmpty(id, nameof(id));

            _ = Query.Where(x => x.ComplianceOfficerId == id);

            _ = Query.Include(x => x.Merchant);
            _ = Query.Select(x => new ComplianceOfficerEntityDto
            {
                ComplianceOfficerId = x.ComplianceOfficerId,
                TenantId = x.TenantId,
                MerchantId = x.MerchantId,
                OfficerCode = x.OfficerCode,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                Phone = x.Phone,
                CertificationLevel = x.CertificationLevel,
                IsActive = x.IsActive,
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
            .EnableCache($"ComplianceOfficerByIdSpec-{id.ToString()}");
        }
    }
}

    public sealed class ComplianceOfficerAdvancedGraphSpecV4 : Specification<ComplianceOfficer, ComplianceOfficerEntityDto>
    {
        public ComplianceOfficerAdvancedGraphSpecV4(
            Guid complianceOfficerId,
            bool includeHierarchicalData = true,
            int? take = null,
            int? skip = null,
            DateTime? effectiveDate = null)
        {
            Guard.Against.Default(complianceOfficerId, nameof(complianceOfficerId));
            Query.Where(c => c.ComplianceOfficerId == complianceOfficerId);

            if (take.HasValue && skip.HasValue)
            {
            }
            Query.Include(c => c.Merchant);

            Query.Select(c => new ComplianceOfficerEntityDto
            {
                ComplianceOfficerId = c.ComplianceOfficerId,
                TenantId = c.TenantId,
                MerchantId = c.MerchantId,
                OfficerCode = c.OfficerCode,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Email = c.Email,
                Phone = c.Phone,
                CertificationLevel = c.CertificationLevel,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt,
                CreatedBy = c.CreatedBy,
                UpdatedAt = c.UpdatedAt,
                UpdatedBy = c.UpdatedBy,
                IsDeleted = c.IsDeleted,
                Merchant = c.Merchant != null ? new MerchantEntityDto
                {
                    MerchantId = c.Merchant.MerchantId,
                    TenantId = c.Merchant.TenantId,
                    MerchantCode = c.Merchant.MerchantCode,
                    MerchantName = c.Merchant.MerchantName,
                    MerchantLevel = c.Merchant.MerchantLevel,
                    AcquirerName = c.Merchant.AcquirerName,
                    ProcessorMID = c.Merchant.ProcessorMID,
                    AnnualCardVolume = c.Merchant.AnnualCardVolume,
                    LastAssessmentDate = c.Merchant.LastAssessmentDate,
                    NextAssessmentDue = c.Merchant.NextAssessmentDue,
                    ComplianceRank = c.Merchant.ComplianceRank,
                    CreatedAt = c.Merchant.CreatedAt,
                    CreatedBy = c.Merchant.CreatedBy,
                    UpdatedAt = c.Merchant.UpdatedAt,
                    UpdatedBy = c.Merchant.UpdatedBy,
                    IsDeleted = c.Merchant.IsDeleted,
                } : null,
            })
            .AsNoTracking()
            .AsSplitQuery()
        .EnableCache($"ComplianceOfficerAdvancedGraphSpec-{complianceOfficerId}");
        }
    }

    public sealed class ComplianceOfficerAdvancedGraphSpecV6 : Specification<ComplianceOfficer, ComplianceOfficerEntityDto>
    {
        public ComplianceOfficerAdvancedGraphSpecV6(
            Guid complianceOfficerId,
            bool enableIntelligentProjection = true,
            bool enableSemanticAnalysis = true,
            bool enableBlueprintStrategy = true,
            int? take = null,
            int? skip = null)
        {
            Guard.Against.Default(complianceOfficerId, nameof(complianceOfficerId));
            Query.Where(c => c.ComplianceOfficerId == complianceOfficerId);

            if (take.HasValue && skip.HasValue)
            {
                Query.Skip(skip.Value).Take(take.Value);
            }
            Query.Include(c => c.Merchant);
            if (enableBlueprintStrategy)
            {
            Query.Include(c => c.Merchant);

            }

            Query.Select(c => new ComplianceOfficerEntityDto
            {
                ComplianceOfficerId = c.ComplianceOfficerId,
                TenantId = c.TenantId,
                MerchantId = c.MerchantId,
                OfficerCode = c.OfficerCode,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Email = c.Email,
                Phone = c.Phone,
                CertificationLevel = c.CertificationLevel,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt,
                CreatedBy = c.CreatedBy,
                UpdatedAt = c.UpdatedAt,
                UpdatedBy = c.UpdatedBy,
                IsDeleted = c.IsDeleted,
                #region Intelligent Metrics
                DaysSinceUpdatedAt = (DateTime.UtcNow - c.UpdatedAt).Value.Days,
                IsActiveFlag = c.IsActive,
                #endregion
                #region Semantic Pattern Fields
                #endregion
                Merchant = c.Merchant != null ? new MerchantEntityDto
                {
                    MerchantId = c.Merchant.MerchantId,
                    TenantId = c.Merchant.TenantId,
                    MerchantCode = c.Merchant.MerchantCode,
                    MerchantName = c.Merchant.MerchantName,
                    MerchantLevel = c.Merchant.MerchantLevel,
                    AcquirerName = c.Merchant.AcquirerName,
                    ProcessorMID = c.Merchant.ProcessorMID,
                    AnnualCardVolume = c.Merchant.AnnualCardVolume,
                    LastAssessmentDate = c.Merchant.LastAssessmentDate,
                    NextAssessmentDue = c.Merchant.NextAssessmentDue,
                    ComplianceRank = c.Merchant.ComplianceRank,
                    CreatedAt = c.Merchant.CreatedAt,
                    CreatedBy = c.Merchant.CreatedBy,
                    UpdatedAt = c.Merchant.UpdatedAt,
                    UpdatedBy = c.Merchant.UpdatedBy,
                    IsDeleted = c.Merchant.IsDeleted,
                } : null,
            })
            .AsNoTracking()
            .AsSplitQuery()
            .EnableCache($"ComplianceOfficerAdvancedGraphSpecV6-{complianceOfficerId}");
        }
    }

    public sealed class ComplianceOfficerAdvancedFilterSpec : Specification<ComplianceOfficer>
    {
        public ComplianceOfficerAdvancedFilterSpec(
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
                        case "firstname":
                            Query.Where(c => c.FirstName.Contains(filter.Value));
                            break;
                        case "lastname":
                            Query.Where(c => c.LastName.Contains(filter.Value));
                            break;
                        case "officercode":
                            Query.Where(c => c.OfficerCode.Contains(filter.Value));
                            break;
                        case "phone":
                            Query.Where(c => c.Phone.Contains(filter.Value));
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
                Query.OrderByDescending(x => x.ComplianceOfficerId);
            }

            Query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }

        private static IOrderedSpecificationBuilder<ComplianceOfficer> ApplySort(
            ISpecificationBuilder<ComplianceOfficer> query,
            Sort sort
        )
        {
            return sort.Direction == SortDirection.Descending
                ? query.OrderByDescending(GetSortProperty(sort.Field))
                : query.OrderBy(GetSortProperty(sort.Field));
        }

        private static IOrderedSpecificationBuilder<ComplianceOfficer> ApplyAdditionalSort(
            IOrderedSpecificationBuilder<ComplianceOfficer> query,
            Sort sort
        )
        {
            return sort.Direction == SortDirection.Descending
                ? query.ThenByDescending(GetSortProperty(sort.Field))
                : query.ThenBy(GetSortProperty(sort.Field));
        }

        private static Expression<Func<ComplianceOfficer, object>> GetSortProperty(
            string propertyName
        )
        {
            return propertyName.ToLower() switch
            {
                "firstname" => c => c.FirstName,
                "lastname" => c => c.LastName,
                "officercode" => c => c.OfficerCode,
                "phone" => c => c.Phone,
                _ => c => c.ComplianceOfficerId,
            };
        }
    }