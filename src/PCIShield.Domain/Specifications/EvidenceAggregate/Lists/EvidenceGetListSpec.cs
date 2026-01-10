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
    public sealed class EvidenceListPagedSpec : PagedSpecification<Evidence, EvidenceEntityDto>
    {
        public EvidenceListPagedSpec(int pageNumber, int pageSize)
        {
            _ = Guard.Against.NegativeOrZero(pageNumber, nameof(pageNumber));
            _ = Guard.Against.NegativeOrZero(pageSize, nameof(pageSize));

            _ = Query
                .OrderByDescending(i => i.EvidenceId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            _ = Query.Include(x => x.Merchant);
            _ = Query.Select(x => new EvidenceEntityDto
            {
                EvidenceId = x.EvidenceId,
                TenantId = x.TenantId,
                MerchantId = x.MerchantId,
                EvidenceCode = x.EvidenceCode,
                EvidenceTitle = x.EvidenceTitle,
                EvidenceType = x.EvidenceType,
                CollectedDate = x.CollectedDate,
                FileHash = x.FileHash,
                StorageUri = x.StorageUri,
                IsValid = x.IsValid,
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
            .EnableCache($"EvidenceListPagedSpec-{pageNumber}-{pageSize}");
        }
    }
    public sealed class EvidenceSearchSpec : Specification<Evidence>
    {
        public EvidenceSearchSpec(string searchTerm)
        {
            string searchLower = searchTerm?.ToLower() ?? string.Empty;

            Query
                .Where(c =>
                        (c.EvidenceCode != null && c.EvidenceCode.ToLower().Contains(searchLower)) ||
                        (c.EvidenceTitle != null && c.EvidenceTitle.ToLower().Contains(searchLower)) ||
                        (c.FileHash != null && c.FileHash.ToLower().Contains(searchLower))                )
                .OrderByDescending(c => c.EvidenceId);
        }
    }

    public sealed class EvidenceLastCreatedSpec : Specification<Evidence>
    {
        public EvidenceLastCreatedSpec()
        {
            Query
                .OrderByDescending(c => c.CollectedDate)
                .Take(1)
                .AsNoTracking()
                .EnableCache("EvidenceLastCreatedSpec");
        }
    }
    public sealed class EvidenceByIdSpec : Specification<Evidence, EvidenceEntityDto>
    {
        public EvidenceByIdSpec(Guid id)
        {
            _ = Guard.Against.NullOrEmpty(id, nameof(id));

            _ = Query.Where(x => x.EvidenceId == id);

            _ = Query.Include(x => x.Merchant);
            _ = Query.Include(x => x.ControlEvidences);
            _ = Query.Select(x => new EvidenceEntityDto
            {
                EvidenceId = x.EvidenceId,
                TenantId = x.TenantId,
                MerchantId = x.MerchantId,
                EvidenceCode = x.EvidenceCode,
                EvidenceTitle = x.EvidenceTitle,
                EvidenceType = x.EvidenceType,
                CollectedDate = x.CollectedDate,
                FileHash = x.FileHash,
                StorageUri = x.StorageUri,
                IsValid = x.IsValid,
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
                ControlEvidences = x.ControlEvidences.Select(controlEvidence => new ControlEvidenceEntityDto
                {
                    RowId = controlEvidence.RowId,
                    ControlId = controlEvidence.ControlId,
                    EvidenceId = controlEvidence.EvidenceId,
                    AssessmentId = controlEvidence.AssessmentId,
                    TenantId = controlEvidence.TenantId,
                    IsPrimary = controlEvidence.IsPrimary,
                    CreatedAt = controlEvidence.CreatedAt,
                    CreatedBy = controlEvidence.CreatedBy,
                    UpdatedAt = controlEvidence.UpdatedAt,
                    UpdatedBy = controlEvidence.UpdatedBy,
                    IsDeleted = controlEvidence.IsDeleted,
                    Evidence = new EvidenceEntityDto
                    {
                        EvidenceId = controlEvidence.Evidence.EvidenceId,
                        TenantId = controlEvidence.Evidence.TenantId,
                        MerchantId = controlEvidence.Evidence.MerchantId,
                        EvidenceCode = controlEvidence.Evidence.EvidenceCode,
                        EvidenceTitle = controlEvidence.Evidence.EvidenceTitle,
                        EvidenceType = controlEvidence.Evidence.EvidenceType,
                        CollectedDate = controlEvidence.Evidence.CollectedDate,
                        FileHash = controlEvidence.Evidence.FileHash,
                        StorageUri = controlEvidence.Evidence.StorageUri,
                        IsValid = controlEvidence.Evidence.IsValid,
                        CreatedAt = controlEvidence.Evidence.CreatedAt,
                        CreatedBy = controlEvidence.Evidence.CreatedBy,
                        UpdatedAt = controlEvidence.Evidence.UpdatedAt,
                        UpdatedBy = controlEvidence.Evidence.UpdatedBy,
                        IsDeleted = controlEvidence.Evidence.IsDeleted,
                    },
                    Control = new ControlEntityDto
                    {
                        ControlId = controlEvidence.Control.ControlId,
                        TenantId = controlEvidence.Control.TenantId,
                        ControlCode = controlEvidence.Control.ControlCode,
                        RequirementNumber = controlEvidence.Control.RequirementNumber,
                        ControlTitle = controlEvidence.Control.ControlTitle,
                        ControlDescription = controlEvidence.Control.ControlDescription,
                        TestingGuidance = controlEvidence.Control.TestingGuidance,
                        FrequencyDays = controlEvidence.Control.FrequencyDays,
                        IsMandatory = controlEvidence.Control.IsMandatory,
                        EffectiveDate = controlEvidence.Control.EffectiveDate,
                        CreatedAt = controlEvidence.Control.CreatedAt,
                        CreatedBy = controlEvidence.Control.CreatedBy,
                        UpdatedAt = controlEvidence.Control.UpdatedAt,
                        UpdatedBy = controlEvidence.Control.UpdatedBy,
                        IsDeleted = controlEvidence.Control.IsDeleted,
                    },
                }).ToList(),
            })
            .AsNoTracking()
            .AsSplitQuery()
            .EnableCache($"EvidenceByIdSpec-{id.ToString()}");
        }
    }
}

    public sealed class EvidenceAdvancedGraphSpecV4 : Specification<Evidence, EvidenceEntityDto>
    {
        public EvidenceAdvancedGraphSpecV4(
            Guid evidenceId,
            bool includeReferenceData = true,
            bool includeHierarchicalData = true,
            int? take = null,
            int? skip = null,
            DateTime? effectiveDate = null)
        {
            Guard.Against.Default(evidenceId, nameof(evidenceId));
            Query.Where(c => c.EvidenceId == evidenceId);

            if (take.HasValue && skip.HasValue)
            {
            }
            Query.Include(c => c.Merchant);

            if (includeReferenceData)
            {
            }

            if (includeHierarchicalData)
            {
            }

            Query.Select(c => new EvidenceEntityDto
            {
                EvidenceId = c.EvidenceId,
                TenantId = c.TenantId,
                MerchantId = c.MerchantId,
                EvidenceCode = c.EvidenceCode,
                EvidenceTitle = c.EvidenceTitle,
                EvidenceType = c.EvidenceType,
                CollectedDate = c.CollectedDate,
                FileHash = c.FileHash,
                StorageUri = c.StorageUri,
                IsValid = c.IsValid,
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
                ControlEvidences = c.ControlEvidences.Select(controlEvidence => new ControlEvidenceEntityDto
                {
                    RowId = controlEvidence.RowId,
                    ControlId = controlEvidence.ControlId,
                    EvidenceId = controlEvidence.EvidenceId,
                    AssessmentId = controlEvidence.AssessmentId,
                    TenantId = controlEvidence.TenantId,
                    IsPrimary = controlEvidence.IsPrimary,
                    CreatedAt = controlEvidence.CreatedAt,
                    CreatedBy = controlEvidence.CreatedBy,
                    UpdatedAt = controlEvidence.UpdatedAt,
                    UpdatedBy = controlEvidence.UpdatedBy,
                    IsDeleted = controlEvidence.IsDeleted,
                    Evidence = new EvidenceEntityDto
                    {
                        EvidenceId = controlEvidence.Evidence.EvidenceId,
                        TenantId = controlEvidence.Evidence.TenantId,
                        MerchantId = controlEvidence.Evidence.MerchantId,
                        EvidenceCode = controlEvidence.Evidence.EvidenceCode,
                        EvidenceTitle = controlEvidence.Evidence.EvidenceTitle,
                        EvidenceType = controlEvidence.Evidence.EvidenceType,
                        CollectedDate = controlEvidence.Evidence.CollectedDate,
                        FileHash = controlEvidence.Evidence.FileHash,
                        StorageUri = controlEvidence.Evidence.StorageUri,
                        IsValid = controlEvidence.Evidence.IsValid,
                        CreatedAt = controlEvidence.Evidence.CreatedAt,
                        CreatedBy = controlEvidence.Evidence.CreatedBy,
                        UpdatedAt = controlEvidence.Evidence.UpdatedAt,
                        UpdatedBy = controlEvidence.Evidence.UpdatedBy,
                        IsDeleted = controlEvidence.Evidence.IsDeleted,
                    },
                    Control = new ControlEntityDto
                    {
                        ControlId = controlEvidence.Control.ControlId,
                        TenantId = controlEvidence.Control.TenantId,
                        ControlCode = controlEvidence.Control.ControlCode,
                        RequirementNumber = controlEvidence.Control.RequirementNumber,
                        ControlTitle = controlEvidence.Control.ControlTitle,
                        ControlDescription = controlEvidence.Control.ControlDescription,
                        TestingGuidance = controlEvidence.Control.TestingGuidance,
                        FrequencyDays = controlEvidence.Control.FrequencyDays,
                        IsMandatory = controlEvidence.Control.IsMandatory,
                        EffectiveDate = controlEvidence.Control.EffectiveDate,
                        CreatedAt = controlEvidence.Control.CreatedAt,
                        CreatedBy = controlEvidence.Control.CreatedBy,
                        UpdatedAt = controlEvidence.Control.UpdatedAt,
                        UpdatedBy = controlEvidence.Control.UpdatedBy,
                        IsDeleted = controlEvidence.Control.IsDeleted,
                    },
                }).ToList(),
            })
            .AsNoTracking()
            .AsSplitQuery()
        .EnableCache($"EvidenceAdvancedGraphSpec-{evidenceId}");
        }
    }

    public sealed class EvidenceAdvancedGraphSpecV6 : Specification<Evidence, EvidenceEntityDto>
    {
        public EvidenceAdvancedGraphSpecV6(
            Guid evidenceId,
            bool enableIntelligentProjection = true,
            bool enableSemanticAnalysis = true,
            bool enableBlueprintStrategy = true,
            int? take = null,
            int? skip = null)
        {
            Guard.Against.Default(evidenceId, nameof(evidenceId));
            Query.Where(c => c.EvidenceId == evidenceId);

            if (take.HasValue && skip.HasValue)
            {
                Query.Skip(skip.Value).Take(take.Value);
            }
            Query.Include(c => c.Merchant);
            if (enableBlueprintStrategy)
            {
            Query.Include(c => c.Merchant);

            }

            Query.Select(c => new EvidenceEntityDto
            {
                EvidenceId = c.EvidenceId,
                TenantId = c.TenantId,
                MerchantId = c.MerchantId,
                EvidenceCode = c.EvidenceCode,
                EvidenceTitle = c.EvidenceTitle,
                EvidenceType = c.EvidenceType,
                CollectedDate = c.CollectedDate,
                FileHash = c.FileHash,
                StorageUri = c.StorageUri,
                IsValid = c.IsValid,
                CreatedAt = c.CreatedAt,
                CreatedBy = c.CreatedBy,
                UpdatedAt = c.UpdatedAt,
                UpdatedBy = c.UpdatedBy,
                IsDeleted = c.IsDeleted,
                #region Intelligent Metrics
                IsEvidenceTypeCritical = c.EvidenceType >= 7,
                DaysSinceCollectedDate = (DateTime.UtcNow - c.CollectedDate).Days,
                IsValidFlag = c.IsValid,
                TotalControlEvidenceCount = c.ControlEvidences.Count(),
                ActiveControlEvidenceCount = c.ControlEvidences.Count(x => !x.IsDeleted),
                LatestControlEvidenceUpdatedAt = c.ControlEvidences.Max(x => (DateTime?)x.UpdatedAt),
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
                ControlEvidences = c.ControlEvidences.Select(controlEvidence => new ControlEvidenceEntityDto
                {
                    RowId = controlEvidence.RowId,
                    ControlId = controlEvidence.ControlId,
                    EvidenceId = controlEvidence.EvidenceId,
                    AssessmentId = controlEvidence.AssessmentId,
                    TenantId = controlEvidence.TenantId,
                    IsPrimary = controlEvidence.IsPrimary,
                    CreatedAt = controlEvidence.CreatedAt,
                    CreatedBy = controlEvidence.CreatedBy,
                    UpdatedAt = controlEvidence.UpdatedAt,
                    UpdatedBy = controlEvidence.UpdatedBy,
                    IsDeleted = controlEvidence.IsDeleted,
                    Evidence = new EvidenceEntityDto
                    {
                        EvidenceId = controlEvidence.Evidence.EvidenceId,
                        TenantId = controlEvidence.Evidence.TenantId,
                        MerchantId = controlEvidence.Evidence.MerchantId,
                        EvidenceCode = controlEvidence.Evidence.EvidenceCode,
                        EvidenceTitle = controlEvidence.Evidence.EvidenceTitle,
                        EvidenceType = controlEvidence.Evidence.EvidenceType,
                        CollectedDate = controlEvidence.Evidence.CollectedDate,
                        FileHash = controlEvidence.Evidence.FileHash,
                        StorageUri = controlEvidence.Evidence.StorageUri,
                        IsValid = controlEvidence.Evidence.IsValid,
                        CreatedAt = controlEvidence.Evidence.CreatedAt,
                        CreatedBy = controlEvidence.Evidence.CreatedBy,
                        UpdatedAt = controlEvidence.Evidence.UpdatedAt,
                        UpdatedBy = controlEvidence.Evidence.UpdatedBy,
                        IsDeleted = controlEvidence.Evidence.IsDeleted,
                    },
                    Control = new ControlEntityDto
                    {
                        ControlId = controlEvidence.Control.ControlId,
                        TenantId = controlEvidence.Control.TenantId,
                        ControlCode = controlEvidence.Control.ControlCode,
                        RequirementNumber = controlEvidence.Control.RequirementNumber,
                        ControlTitle = controlEvidence.Control.ControlTitle,
                        ControlDescription = controlEvidence.Control.ControlDescription,
                        TestingGuidance = controlEvidence.Control.TestingGuidance,
                        FrequencyDays = controlEvidence.Control.FrequencyDays,
                        IsMandatory = controlEvidence.Control.IsMandatory,
                        EffectiveDate = controlEvidence.Control.EffectiveDate,
                        CreatedAt = controlEvidence.Control.CreatedAt,
                        CreatedBy = controlEvidence.Control.CreatedBy,
                        UpdatedAt = controlEvidence.Control.UpdatedAt,
                        UpdatedBy = controlEvidence.Control.UpdatedBy,
                        IsDeleted = controlEvidence.Control.IsDeleted,
                    },
                }).ToList(),
            })
            .AsNoTracking()
            .AsSplitQuery()
            .EnableCache($"EvidenceAdvancedGraphSpecV6-{evidenceId}");
        }
    }

    public sealed class EvidenceAdvancedFilterSpec : Specification<Evidence>
    {
        public EvidenceAdvancedFilterSpec(
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
                        case "evidencecode":
                            Query.Where(c => c.EvidenceCode.Contains(filter.Value));
                            break;
                        case "evidencetitle":
                            Query.Where(c => c.EvidenceTitle.Contains(filter.Value));
                            break;
                        case "filehash":
                            Query.Where(c => c.FileHash.Contains(filter.Value));
                            break;
                        case "evidencetype":
                            if (int.TryParse(filter.Value, out int evidencetype))
                            {
                                Query.Where(c => c.EvidenceType == evidencetype);
                            }
                            break;
                        case "collecteddate":
                            if (DateTime.TryParse(filter.Value, out DateTime collecteddate))
                            {
                                Query.Where(c => c.CollectedDate >= collecteddate.AddHours(-6) && c.CollectedDate <= collecteddate.AddHours(6));
                            }
                            break;
                        case "isvalid":
                            if (bool.TryParse(filter.Value, out bool isvalid))
                            {
                                Query.Where(c => c.IsValid == isvalid);
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
                Query.OrderByDescending(x => x.EvidenceId);
            }

            Query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }

        private static IOrderedSpecificationBuilder<Evidence> ApplySort(
            ISpecificationBuilder<Evidence> query,
            Sort sort
        )
        {
            return sort.Direction == SortDirection.Descending
                ? query.OrderByDescending(GetSortProperty(sort.Field))
                : query.OrderBy(GetSortProperty(sort.Field));
        }

        private static IOrderedSpecificationBuilder<Evidence> ApplyAdditionalSort(
            IOrderedSpecificationBuilder<Evidence> query,
            Sort sort
        )
        {
            return sort.Direction == SortDirection.Descending
                ? query.ThenByDescending(GetSortProperty(sort.Field))
                : query.ThenBy(GetSortProperty(sort.Field));
        }

        private static Expression<Func<Evidence, object>> GetSortProperty(
            string propertyName
        )
        {
            return propertyName.ToLower() switch
            {
                "evidencecode" => c => c.EvidenceCode,
                "evidencetitle" => c => c.EvidenceTitle,
                "filehash" => c => c.FileHash,
                _ => c => c.EvidenceId,
            };
        }
    }