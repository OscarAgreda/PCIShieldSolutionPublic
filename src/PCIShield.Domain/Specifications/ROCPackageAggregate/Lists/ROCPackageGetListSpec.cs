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
    public sealed class ROCPackageListPagedSpec : PagedSpecification<ROCPackage, ROCPackageEntityDto>
    {
        public ROCPackageListPagedSpec(int pageNumber, int pageSize)
        {
            _ = Guard.Against.NegativeOrZero(pageNumber, nameof(pageNumber));
            _ = Guard.Against.NegativeOrZero(pageSize, nameof(pageSize));

            _ = Query
                .OrderByDescending(i => i.ROCPackageId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            _ = Query.Include(x => x.Assessment);
            _ = Query.Select(x => new ROCPackageEntityDto
            {
                ROCPackageId = x.ROCPackageId,
                TenantId = x.TenantId,
                AssessmentId = x.AssessmentId,
                PackageVersion = x.PackageVersion,
                GeneratedDate = x.GeneratedDate,
                QSAName = x.QSAName,
                QSACompany = x.QSACompany,
                SignatureDate = x.SignatureDate,
                AOCNumber = x.AOCNumber,
                Rank = x.Rank,
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
            })
            .AsNoTracking()
            .AsSplitQuery()
            .EnableCache($"ROCPackageListPagedSpec-{pageNumber}-{pageSize}");
        }
    }
    public sealed class ROCPackageSearchSpec : Specification<ROCPackage>
    {
        public ROCPackageSearchSpec(string searchTerm)
        {
            string searchLower = searchTerm?.ToLower() ?? string.Empty;

            Query
                .Where(c =>
                        (c.AOCNumber != null && c.AOCNumber.ToLower().Contains(searchLower)) ||
                        (c.QSAName != null && c.QSAName.ToLower().Contains(searchLower)) ||
                        (c.PackageVersion != null && c.PackageVersion.ToLower().Contains(searchLower)) ||
                        (c.QSACompany != null && c.QSACompany.ToLower().Contains(searchLower))                )
                .OrderByDescending(c => c.ROCPackageId);
        }
    }

    public sealed class ROCPackageLastCreatedSpec : Specification<ROCPackage>
    {
        public ROCPackageLastCreatedSpec()
        {
            Query
                .OrderByDescending(c => c.GeneratedDate)
                .Take(1)
                .AsNoTracking()
                .EnableCache("ROCPackageLastCreatedSpec");
        }
    }
    public sealed class ROCPackageByIdSpec : Specification<ROCPackage, ROCPackageEntityDto>
    {
        public ROCPackageByIdSpec(Guid id)
        {
            _ = Guard.Against.NullOrEmpty(id, nameof(id));

            _ = Query.Where(x => x.ROCPackageId == id);

            _ = Query.Include(x => x.Assessment);
            _ = Query.Select(x => new ROCPackageEntityDto
            {
                ROCPackageId = x.ROCPackageId,
                TenantId = x.TenantId,
                AssessmentId = x.AssessmentId,
                PackageVersion = x.PackageVersion,
                GeneratedDate = x.GeneratedDate,
                QSAName = x.QSAName,
                QSACompany = x.QSACompany,
                SignatureDate = x.SignatureDate,
                AOCNumber = x.AOCNumber,
                Rank = x.Rank,
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
            })
            .AsNoTracking()
            .AsSplitQuery()
            .EnableCache($"ROCPackageByIdSpec-{id.ToString()}");
        }
    }
}

    public sealed class ROCPackageAdvancedGraphSpecV4 : Specification<ROCPackage, ROCPackageEntityDto>
    {
        public ROCPackageAdvancedGraphSpecV4(
            Guid rocpackageId,
            int? take = null,
            int? skip = null,
            DateTime? effectiveDate = null)
        {
            Guard.Against.Default(rocpackageId, nameof(rocpackageId));
            Query.Where(c => c.ROCPackageId == rocpackageId);

            if (take.HasValue && skip.HasValue)
            {
            }
            Query.Include(c => c.Assessment);

            Query.Select(c => new ROCPackageEntityDto
            {
                ROCPackageId = c.ROCPackageId,
                TenantId = c.TenantId,
                AssessmentId = c.AssessmentId,
                PackageVersion = c.PackageVersion,
                GeneratedDate = c.GeneratedDate,
                QSAName = c.QSAName,
                QSACompany = c.QSACompany,
                SignatureDate = c.SignatureDate,
                AOCNumber = c.AOCNumber,
                Rank = c.Rank,
                CreatedAt = c.CreatedAt,
                CreatedBy = c.CreatedBy,
                UpdatedAt = c.UpdatedAt,
                UpdatedBy = c.UpdatedBy,
                IsDeleted = c.IsDeleted,
                Assessment = c.Assessment != null ? new AssessmentEntityDto
                {
                    AssessmentId = c.Assessment.AssessmentId,
                    TenantId = c.Assessment.TenantId,
                    MerchantId = c.Assessment.MerchantId,
                    AssessmentCode = c.Assessment.AssessmentCode,
                    AssessmentType = c.Assessment.AssessmentType,
                    AssessmentPeriod = c.Assessment.AssessmentPeriod,
                    StartDate = c.Assessment.StartDate,
                    EndDate = c.Assessment.EndDate,
                    CompletionDate = c.Assessment.CompletionDate,
                    Rank = c.Assessment.Rank,
                    ComplianceScore = c.Assessment.ComplianceScore,
                    QSAReviewRequired = c.Assessment.QSAReviewRequired,
                    CreatedAt = c.Assessment.CreatedAt,
                    CreatedBy = c.Assessment.CreatedBy,
                    UpdatedAt = c.Assessment.UpdatedAt,
                    UpdatedBy = c.Assessment.UpdatedBy,
                    IsDeleted = c.Assessment.IsDeleted,
                } : null,
            })
            .AsNoTracking()
            .AsSplitQuery()
        .EnableCache($"ROCPackageAdvancedGraphSpec-{rocpackageId}");
        }
    }

    public sealed class ROCPackageAdvancedGraphSpecV6 : Specification<ROCPackage, ROCPackageEntityDto>
    {
        public ROCPackageAdvancedGraphSpecV6(
            Guid rocpackageId,
            bool enableIntelligentProjection = true,
            bool enableSemanticAnalysis = true,
            bool enableBlueprintStrategy = true,
            int? take = null,
            int? skip = null)
        {
            Guard.Against.Default(rocpackageId, nameof(rocpackageId));
            Query.Where(c => c.ROCPackageId == rocpackageId);

            if (take.HasValue && skip.HasValue)
            {
                Query.Skip(skip.Value).Take(take.Value);
            }
            Query.Include(c => c.Assessment);
            if (enableBlueprintStrategy)
            {
            Query.Include(c => c.Assessment);

            }

            Query.Select(c => new ROCPackageEntityDto
            {
                ROCPackageId = c.ROCPackageId,
                TenantId = c.TenantId,
                AssessmentId = c.AssessmentId,
                PackageVersion = c.PackageVersion,
                GeneratedDate = c.GeneratedDate,
                QSAName = c.QSAName,
                QSACompany = c.QSACompany,
                SignatureDate = c.SignatureDate,
                AOCNumber = c.AOCNumber,
                Rank = c.Rank,
                CreatedAt = c.CreatedAt,
                CreatedBy = c.CreatedBy,
                UpdatedAt = c.UpdatedAt,
                UpdatedBy = c.UpdatedBy,
                IsDeleted = c.IsDeleted,
                #region Intelligent Metrics
                IsRankCritical = c.Rank >= 7,
                DaysSinceGeneratedDate = (DateTime.UtcNow - c.GeneratedDate).Days,
                IsDeletedFlag = c.IsDeleted,
                #endregion
                #region Semantic Pattern Fields
                #endregion
                Assessment = c.Assessment != null ? new AssessmentEntityDto
                {
                    AssessmentId = c.Assessment.AssessmentId,
                    TenantId = c.Assessment.TenantId,
                    MerchantId = c.Assessment.MerchantId,
                    AssessmentCode = c.Assessment.AssessmentCode,
                    AssessmentType = c.Assessment.AssessmentType,
                    AssessmentPeriod = c.Assessment.AssessmentPeriod,
                    StartDate = c.Assessment.StartDate,
                    EndDate = c.Assessment.EndDate,
                    CompletionDate = c.Assessment.CompletionDate,
                    Rank = c.Assessment.Rank,
                    ComplianceScore = c.Assessment.ComplianceScore,
                    QSAReviewRequired = c.Assessment.QSAReviewRequired,
                    CreatedAt = c.Assessment.CreatedAt,
                    CreatedBy = c.Assessment.CreatedBy,
                    UpdatedAt = c.Assessment.UpdatedAt,
                    UpdatedBy = c.Assessment.UpdatedBy,
                    IsDeleted = c.Assessment.IsDeleted,
                } : null,
            })
            .AsNoTracking()
            .AsSplitQuery()
            .EnableCache($"ROCPackageAdvancedGraphSpecV6-{rocpackageId}");
        }
    }

    public sealed class ROCPackageAdvancedFilterSpec : Specification<ROCPackage>
    {
        public ROCPackageAdvancedFilterSpec(
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
                        case "aocnumber":
                            Query.Where(c => c.AOCNumber.Contains(filter.Value));
                            break;
                        case "qsaname":
                            Query.Where(c => c.QSAName.Contains(filter.Value));
                            break;
                        case "packageversion":
                            Query.Where(c => c.PackageVersion.Contains(filter.Value));
                            break;
                        case "qsacompany":
                            Query.Where(c => c.QSACompany.Contains(filter.Value));
                            break;
                        case "generateddate":
                            if (DateTime.TryParse(filter.Value, out DateTime generateddate))
                            {
                                Query.Where(c => c.GeneratedDate >= generateddate.AddHours(-6) && c.GeneratedDate <= generateddate.AddHours(6));
                            }
                            break;
                        case "signaturedate":
                            if (DateTime.TryParse(filter.Value, out DateTime signaturedate))
                            {
                                Query.Where(c => c.SignatureDate >= signaturedate.AddHours(-6) && c.SignatureDate <= signaturedate.AddHours(6));
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
                Query.OrderByDescending(x => x.ROCPackageId);
            }

            Query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }

        private static IOrderedSpecificationBuilder<ROCPackage> ApplySort(
            ISpecificationBuilder<ROCPackage> query,
            Sort sort
        )
        {
            return sort.Direction == SortDirection.Descending
                ? query.OrderByDescending(GetSortProperty(sort.Field))
                : query.OrderBy(GetSortProperty(sort.Field));
        }

        private static IOrderedSpecificationBuilder<ROCPackage> ApplyAdditionalSort(
            IOrderedSpecificationBuilder<ROCPackage> query,
            Sort sort
        )
        {
            return sort.Direction == SortDirection.Descending
                ? query.ThenByDescending(GetSortProperty(sort.Field))
                : query.ThenBy(GetSortProperty(sort.Field));
        }

        private static Expression<Func<ROCPackage, object>> GetSortProperty(
            string propertyName
        )
        {
            return propertyName.ToLower() switch
            {
                "aocnumber" => c => c.AOCNumber,
                "qsaname" => c => c.QSAName,
                "packageversion" => c => c.PackageVersion,
                "qsacompany" => c => c.QSACompany,
                _ => c => c.ROCPackageId,
            };
        }
    }