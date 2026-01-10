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
    public sealed class CompensatingControlListPagedSpec : PagedSpecification<CompensatingControl, CompensatingControlEntityDto>
    {
        public CompensatingControlListPagedSpec(int pageNumber, int pageSize)
        {
            _ = Guard.Against.NegativeOrZero(pageNumber, nameof(pageNumber));
            _ = Guard.Against.NegativeOrZero(pageSize, nameof(pageSize));

            _ = Query
                .OrderByDescending(i => i.CompensatingControlId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            _ = Query.Include(x => x.Control);
            _ = Query.Include(x => x.Merchant);
            _ = Query.Select(x => new CompensatingControlEntityDto
            {
                CompensatingControlId = x.CompensatingControlId,
                TenantId = x.TenantId,
                ControlId = x.ControlId,
                MerchantId = x.MerchantId,
                Justification = x.Justification,
                ImplementationDetails = x.ImplementationDetails,
                ApprovedBy = x.ApprovedBy,
                ApprovalDate = x.ApprovalDate,
                ExpiryDate = x.ExpiryDate,
                Rank = x.Rank,
                CreatedAt = x.CreatedAt,
                CreatedBy = x.CreatedBy,
                UpdatedAt = x.UpdatedAt,
                UpdatedBy = x.UpdatedBy,
                IsDeleted = x.IsDeleted,
                Control = x.Control != null ? new ControlEntityDto
                {
                    ControlId = x.Control.ControlId,
                    TenantId = x.Control.TenantId,
                    ControlCode = x.Control.ControlCode,
                    RequirementNumber = x.Control.RequirementNumber,
                    ControlTitle = x.Control.ControlTitle,
                    ControlDescription = x.Control.ControlDescription,
                    TestingGuidance = x.Control.TestingGuidance,
                    FrequencyDays = x.Control.FrequencyDays,
                    IsMandatory = x.Control.IsMandatory,
                    EffectiveDate = x.Control.EffectiveDate,
                    CreatedAt = x.Control.CreatedAt,
                    CreatedBy = x.Control.CreatedBy,
                    UpdatedAt = x.Control.UpdatedAt,
                    UpdatedBy = x.Control.UpdatedBy,
                    IsDeleted = x.Control.IsDeleted,
                } : null,
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
            .EnableCache($"CompensatingControlListPagedSpec-{pageNumber}-{pageSize}");
        }
    }
    public sealed class CompensatingControlSearchSpec : Specification<CompensatingControl>
    {
        public CompensatingControlSearchSpec(string searchTerm)
        {
            string searchLower = searchTerm?.ToLower() ?? string.Empty;

            Query
                .Where(c =>
                        (c.CompensatingControlId != null && c.CompensatingControlId.ToString().ToLower().Contains(searchLower))                )
                .OrderByDescending(c => c.CompensatingControlId);
        }
    }

    public sealed class CompensatingControlLastCreatedSpec : Specification<CompensatingControl>
    {
        public CompensatingControlLastCreatedSpec()
        {
            Query
                .OrderByDescending(c => c.ApprovalDate)
                .Take(1)
                .AsNoTracking()
                .EnableCache("CompensatingControlLastCreatedSpec");
        }
    }
    public sealed class CompensatingControlByIdSpec : Specification<CompensatingControl, CompensatingControlEntityDto>
    {
        public CompensatingControlByIdSpec(Guid id)
        {
            _ = Guard.Against.NullOrEmpty(id, nameof(id));

            _ = Query.Where(x => x.CompensatingControlId == id);

            _ = Query.Include(x => x.Control);
            _ = Query.Include(x => x.Merchant);
            _ = Query.Select(x => new CompensatingControlEntityDto
            {
                CompensatingControlId = x.CompensatingControlId,
                TenantId = x.TenantId,
                ControlId = x.ControlId,
                MerchantId = x.MerchantId,
                Justification = x.Justification,
                ImplementationDetails = x.ImplementationDetails,
                ApprovedBy = x.ApprovedBy,
                ApprovalDate = x.ApprovalDate,
                ExpiryDate = x.ExpiryDate,
                Rank = x.Rank,
                CreatedAt = x.CreatedAt,
                CreatedBy = x.CreatedBy,
                UpdatedAt = x.UpdatedAt,
                UpdatedBy = x.UpdatedBy,
                IsDeleted = x.IsDeleted,

                Control = x.Control != null ? new ControlEntityDto
                {
                    ControlId = x.Control.ControlId,
                    TenantId = x.Control.TenantId,
                    ControlCode = x.Control.ControlCode,
                    RequirementNumber = x.Control.RequirementNumber,
                    ControlTitle = x.Control.ControlTitle,
                    ControlDescription = x.Control.ControlDescription,
                    TestingGuidance = x.Control.TestingGuidance,
                    FrequencyDays = x.Control.FrequencyDays,
                    IsMandatory = x.Control.IsMandatory,
                    EffectiveDate = x.Control.EffectiveDate,
                    CreatedAt = x.Control.CreatedAt,
                    CreatedBy = x.Control.CreatedBy,
                    UpdatedAt = x.Control.UpdatedAt,
                    UpdatedBy = x.Control.UpdatedBy,
                    IsDeleted = x.Control.IsDeleted,
                } : null,
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
            .EnableCache($"CompensatingControlByIdSpec-{id.ToString()}");
        }
    }
}

    public sealed class CompensatingControlAdvancedFilterSpec : Specification<CompensatingControl>
    {
        public CompensatingControlAdvancedFilterSpec(
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
                        case "approvaldate":
                            if (DateTime.TryParse(filter.Value, out DateTime approvaldate))
                            {
                                Query.Where(c => c.ApprovalDate >= approvaldate.AddHours(-6) && c.ApprovalDate <= approvaldate.AddHours(6));
                            }
                            break;
                        case "expirydate":
                            if (DateTime.TryParse(filter.Value, out DateTime expirydate))
                            {
                                Query.Where(c => c.ExpiryDate >= expirydate.AddHours(-6) && c.ExpiryDate <= expirydate.AddHours(6));
                            }
                            break;
                        case "rank":
                            if (int.TryParse(filter.Value, out int rank))
                            {
                                Query.Where(c => c.Rank == rank);
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
                Query.OrderByDescending(x => x.CompensatingControlId);
            }

            Query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }

        private static IOrderedSpecificationBuilder<CompensatingControl> ApplySort(
            ISpecificationBuilder<CompensatingControl> query,
            Sort sort
        )
        {
            return sort.Direction == SortDirection.Descending
                ? query.OrderByDescending(GetSortProperty(sort.Field))
                : query.OrderBy(GetSortProperty(sort.Field));
        }

        private static IOrderedSpecificationBuilder<CompensatingControl> ApplyAdditionalSort(
            IOrderedSpecificationBuilder<CompensatingControl> query,
            Sort sort
        )
        {
            return sort.Direction == SortDirection.Descending
                ? query.ThenByDescending(GetSortProperty(sort.Field))
                : query.ThenBy(GetSortProperty(sort.Field));
        }

        private static Expression<Func<CompensatingControl, object>> GetSortProperty(
            string propertyName
        )
        {
            return propertyName.ToLower() switch
            {
                _ => c => c.CompensatingControlId,
            };
        }
    }