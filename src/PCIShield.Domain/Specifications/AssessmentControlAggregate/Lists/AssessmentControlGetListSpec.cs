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
    public sealed class AssessmentControlListPagedSpec : PagedSpecification<AssessmentControl, AssessmentControlEntityDto>
    {
        public AssessmentControlListPagedSpec(int pageNumber, int pageSize)
        {
            _ = Guard.Against.NegativeOrZero(pageNumber, nameof(pageNumber));
            _ = Guard.Against.NegativeOrZero(pageSize, nameof(pageSize));

            _ = Query
                .OrderByDescending(i => i.RowId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            _ = Query.Include(x => x.Assessment);
            _ = Query.Include(x => x.Control);
            _ = Query.Select(x => new AssessmentControlEntityDto
            {
                RowId = x.RowId,
                AssessmentId = x.AssessmentId,
                ControlId = x.ControlId,
                TenantId = x.TenantId,
                TestResult = x.TestResult,
                TestDate = x.TestDate,
                TestedBy = x.TestedBy,
                Notes = x.Notes,
                CreatedAt = x.CreatedAt,
                CreatedBy = x.CreatedBy,
                UpdatedAt = x.UpdatedAt,
                UpdatedBy = x.UpdatedBy,
                IsDeleted = x.IsDeleted,
                Assessment = x.Assessment != null ? new AssessmentEntityDto
                {
                    AssessmentId = x.Assessment.AssessmentId,
                    TenantId = x.Assessment.TenantId,
                    MerchantId = x.Assessment.MerchantId,
                    AssessmentCode = x.Assessment.AssessmentCode,
                    AssessmentType = x.Assessment.AssessmentType,
                    AssessmentPeriod = x.Assessment.AssessmentPeriod,
                    StartDate = x.Assessment.StartDate,
                    EndDate = x.Assessment.EndDate,
                    CompletionDate = x.Assessment.CompletionDate,
                    Rank = x.Assessment.Rank,
                    ComplianceScore = x.Assessment.ComplianceScore,
                    QSAReviewRequired = x.Assessment.QSAReviewRequired,
                    CreatedAt = x.Assessment.CreatedAt,
                    CreatedBy = x.Assessment.CreatedBy,
                    UpdatedAt = x.Assessment.UpdatedAt,
                    UpdatedBy = x.Assessment.UpdatedBy,
                    IsDeleted = x.Assessment.IsDeleted,
                } : null,
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
            })
            .AsNoTracking()
            .AsSplitQuery()
            .EnableCache($"AssessmentControlListPagedSpec-{pageNumber}-{pageSize}");
        }
    }
    public sealed class AssessmentControlSearchSpec : Specification<AssessmentControl>
    {
        public AssessmentControlSearchSpec(string searchTerm)
        {
            string searchLower = searchTerm?.ToLower() ?? string.Empty;

            Query
                .Where(c =>
                        (c.Assessment.AssessmentCode != null && c.Assessment.AssessmentCode.ToLower().Contains(searchLower)) ||
                        (c.Assessment.AssessmentPeriod != null && c.Assessment.AssessmentPeriod.ToLower().Contains(searchLower)) ||
                        (c.Control.ControlCode != null && c.Control.ControlCode.ToLower().Contains(searchLower)) ||
                        (c.Control.RequirementNumber != null && c.Control.RequirementNumber.ToLower().Contains(searchLower))                )
                .OrderByDescending(c => c.RowId);
        }
    }

    public sealed class AssessmentControlLastCreatedSpec : Specification<AssessmentControl>
    {
        public AssessmentControlLastCreatedSpec()
        {
            Query
                .OrderByDescending(c => c.TestDate)
                .Take(1)
                .AsNoTracking()
                .EnableCache("AssessmentControlLastCreatedSpec");
        }
    }
    public sealed class AssessmentControlByIdSpec : Specification<AssessmentControl, AssessmentControlEntityDto>
    {
        public AssessmentControlByIdSpec(
        Guid assessmentId,
        Guid controlId
        )
        {

            _ = Guard.Against.NullOrEmpty(assessmentId, nameof(assessmentId));

            _ = Query.Where(x => x.AssessmentId == assessmentId);
            _ = Guard.Against.NullOrEmpty(controlId, nameof(controlId));

            _ = Query.Where(x => x.ControlId == controlId);

            _ = Query.Include(x => x.Assessment);
            _ = Query.Include(x => x.Control);
            _ = Query.Select(x => new AssessmentControlEntityDto
            {
                RowId = x.RowId,
                AssessmentId = x.AssessmentId,
                ControlId = x.ControlId,
                TenantId = x.TenantId,
                TestResult = x.TestResult,
                TestDate = x.TestDate,
                TestedBy = x.TestedBy,
                Notes = x.Notes,
                CreatedAt = x.CreatedAt,
                CreatedBy = x.CreatedBy,
                UpdatedAt = x.UpdatedAt,
                UpdatedBy = x.UpdatedBy,
                IsDeleted = x.IsDeleted,

                Assessment = x.Assessment != null ? new AssessmentEntityDto
                {
                    AssessmentId = x.Assessment.AssessmentId,
                    TenantId = x.Assessment.TenantId,
                    MerchantId = x.Assessment.MerchantId,
                    AssessmentCode = x.Assessment.AssessmentCode,
                    AssessmentType = x.Assessment.AssessmentType,
                    AssessmentPeriod = x.Assessment.AssessmentPeriod,
                    StartDate = x.Assessment.StartDate,
                    EndDate = x.Assessment.EndDate,
                    CompletionDate = x.Assessment.CompletionDate,
                    Rank = x.Assessment.Rank,
                    ComplianceScore = x.Assessment.ComplianceScore,
                    QSAReviewRequired = x.Assessment.QSAReviewRequired,
                    CreatedAt = x.Assessment.CreatedAt,
                    CreatedBy = x.Assessment.CreatedBy,
                    UpdatedAt = x.Assessment.UpdatedAt,
                    UpdatedBy = x.Assessment.UpdatedBy,
                    IsDeleted = x.Assessment.IsDeleted,
                } : null,
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
            })
            .AsNoTracking()
            .AsSplitQuery()
            .EnableCache($"AssessmentControlByIdSpec--{assessmentId.ToString()}-{controlId.ToString()}");
        }
    }
}

    public sealed class AssessmentControlAdvancedFilterSpec : Specification<AssessmentControl>
    {
        public AssessmentControlAdvancedFilterSpec(
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
                        case "rowid":
                            if (int.TryParse(filter.Value, out int rowid))
                            {
                                Query.Where(c => c.RowId == rowid);
                            }
                            break;
                        case "testresult":
                            if (int.TryParse(filter.Value, out int testresult))
                            {
                                Query.Where(c => c.TestResult == testresult);
                            }
                            break;
                        case "testdate":
                            if (DateTime.TryParse(filter.Value, out DateTime testdate))
                            {
                                Query.Where(c => c.TestDate >= testdate.AddHours(-6) && c.TestDate <= testdate.AddHours(6));
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
                Query.OrderByDescending(x => x.RowId);
            }

            Query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }

        private static IOrderedSpecificationBuilder<AssessmentControl> ApplySort(
            ISpecificationBuilder<AssessmentControl> query,
            Sort sort
        )
        {
            return sort.Direction == SortDirection.Descending
                ? query.OrderByDescending(GetSortProperty(sort.Field))
                : query.OrderBy(GetSortProperty(sort.Field));
        }

        private static IOrderedSpecificationBuilder<AssessmentControl> ApplyAdditionalSort(
            IOrderedSpecificationBuilder<AssessmentControl> query,
            Sort sort
        )
        {
            return sort.Direction == SortDirection.Descending
                ? query.ThenByDescending(GetSortProperty(sort.Field))
                : query.ThenBy(GetSortProperty(sort.Field));
        }

        private static Expression<Func<AssessmentControl, object>> GetSortProperty(
            string propertyName
        )
        {
            return propertyName.ToLower() switch
            {
                _ => c => c.RowId,
            };
        }
    }