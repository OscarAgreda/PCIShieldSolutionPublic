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
    public sealed class PaymentChannelListPagedSpec : PagedSpecification<PaymentChannel, PaymentChannelEntityDto>
    {
        public PaymentChannelListPagedSpec(int pageNumber, int pageSize)
        {
            _ = Guard.Against.NegativeOrZero(pageNumber, nameof(pageNumber));
            _ = Guard.Against.NegativeOrZero(pageSize, nameof(pageSize));

            _ = Query
                .OrderByDescending(i => i.PaymentChannelId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            _ = Query.Include(x => x.Merchant);
            _ = Query.Select(x => new PaymentChannelEntityDto
            {
                PaymentChannelId = x.PaymentChannelId,
                TenantId = x.TenantId,
                MerchantId = x.MerchantId,
                ChannelCode = x.ChannelCode,
                ChannelName = x.ChannelName,
                ChannelType = x.ChannelType,
                ProcessingVolume = x.ProcessingVolume,
                IsInScope = x.IsInScope,
                TokenizationEnabled = x.TokenizationEnabled,
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
            .EnableCache($"PaymentChannelListPagedSpec-{pageNumber}-{pageSize}");
        }
    }
    public sealed class PaymentChannelSearchSpec : Specification<PaymentChannel>
    {
        public PaymentChannelSearchSpec(string searchTerm)
        {
            string searchLower = searchTerm?.ToLower() ?? string.Empty;

            Query
                .Where(c =>
                        (c.ChannelName != null && c.ChannelName.ToLower().Contains(searchLower)) ||
                        (c.ChannelCode != null && c.ChannelCode.ToLower().Contains(searchLower))                )
                .OrderByDescending(c => c.PaymentChannelId);
        }
    }

    public sealed class PaymentChannelLastCreatedSpec : Specification<PaymentChannel>
    {
        public PaymentChannelLastCreatedSpec()
        {
            Query
                .OrderByDescending(c => c.CreatedAt)
                .Take(1)
                .AsNoTracking()
                .EnableCache("PaymentChannelLastCreatedSpec");
        }
    }
    public sealed class PaymentChannelByIdSpec : Specification<PaymentChannel, PaymentChannelEntityDto>
    {
        public PaymentChannelByIdSpec(Guid id)
        {
            _ = Guard.Against.NullOrEmpty(id, nameof(id));

            _ = Query.Where(x => x.PaymentChannelId == id);

            _ = Query.Include(x => x.Merchant);
            _ = Query.Include(x => x.PaymentPages);
            _ = Query.Select(x => new PaymentChannelEntityDto
            {
                PaymentChannelId = x.PaymentChannelId,
                TenantId = x.TenantId,
                MerchantId = x.MerchantId,
                ChannelCode = x.ChannelCode,
                ChannelName = x.ChannelName,
                ChannelType = x.ChannelType,
                ProcessingVolume = x.ProcessingVolume,
                IsInScope = x.IsInScope,
                TokenizationEnabled = x.TokenizationEnabled,
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
                PaymentPages = x.PaymentPages.Select(paymentPage => new PaymentPageEntityDto
                {
                    PaymentPageId = paymentPage.PaymentPageId,
                    TenantId = paymentPage.TenantId,
                    PaymentChannelId = paymentPage.PaymentChannelId,
                    PageUrl = paymentPage.PageUrl,
                    PageName = paymentPage.PageName,
                    IsActive = paymentPage.IsActive,
                    LastScriptInventory = paymentPage.LastScriptInventory,
                    ScriptIntegrityHash = paymentPage.ScriptIntegrityHash,
                    CreatedAt = paymentPage.CreatedAt,
                    CreatedBy = paymentPage.CreatedBy,
                    UpdatedAt = paymentPage.UpdatedAt,
                    UpdatedBy = paymentPage.UpdatedBy,
                    IsDeleted = paymentPage.IsDeleted,
                }).ToList(),
            })
            .AsNoTracking()
            .AsSplitQuery()
            .EnableCache($"PaymentChannelByIdSpec-{id.ToString()}");
        }
    }
}

    public sealed class PaymentChannelAdvancedGraphSpecV4 : Specification<PaymentChannel, PaymentChannelEntityDto>
    {
        public PaymentChannelAdvancedGraphSpecV4(
            Guid paymentChannelId,
            bool includeTransactionData = true,
            bool includeTransitiveData = true,
            bool includeHierarchicalData = true,
            int? take = null,
            int? skip = null,
            DateTime? effectiveDate = null)
        {
            Guard.Against.Default(paymentChannelId, nameof(paymentChannelId));
            Query.Where(c => c.PaymentChannelId == paymentChannelId);

            if (take.HasValue && skip.HasValue)
            {
            }
            Query.Include(c => c.Merchant);

            if (includeTransactionData)
            {
            }

            if (includeTransitiveData)
            {

            }

            Query.Select(c => new PaymentChannelEntityDto
            {
                PaymentChannelId = c.PaymentChannelId,
                TenantId = c.TenantId,
                MerchantId = c.MerchantId,
                ChannelCode = c.ChannelCode,
                ChannelName = c.ChannelName,
                ChannelType = c.ChannelType,
                ProcessingVolume = c.ProcessingVolume,
                IsInScope = c.IsInScope,
                TokenizationEnabled = c.TokenizationEnabled,
                CreatedAt = c.CreatedAt,
                CreatedBy = c.CreatedBy,
                UpdatedAt = c.UpdatedAt,
                UpdatedBy = c.UpdatedBy,
                IsDeleted = c.IsDeleted,

                Scripts = c.PaymentPages
                    .SelectMany(i => i.Scripts)
                    .Select(t => new ScriptEntityDto
                    {
                        ScriptId = t.ScriptId,
                        TenantId = t.TenantId,
                        PaymentPageId = t.PaymentPageId,
                        ScriptUrl = t.ScriptUrl,
                        ScriptHash = t.ScriptHash,
                        ScriptType = t.ScriptType,
                        IsAuthorized = t.IsAuthorized,
                        FirstSeen = t.FirstSeen,
                        LastSeen = t.LastSeen,
                        IsDeleted = t.IsDeleted,
                    })
                    .Take(2).ToList(),
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
                PaymentPages = c.PaymentPages.Select(paymentPage => new PaymentPageEntityDto
                {
                    PaymentPageId = paymentPage.PaymentPageId,
                    TenantId = paymentPage.TenantId,
                    PaymentChannelId = paymentPage.PaymentChannelId,
                    PageUrl = paymentPage.PageUrl,
                    PageName = paymentPage.PageName,
                    IsActive = paymentPage.IsActive,
                    LastScriptInventory = paymentPage.LastScriptInventory,
                    ScriptIntegrityHash = paymentPage.ScriptIntegrityHash,
                    CreatedAt = paymentPage.CreatedAt,
                    CreatedBy = paymentPage.CreatedBy,
                    UpdatedAt = paymentPage.UpdatedAt,
                    UpdatedBy = paymentPage.UpdatedBy,
                    IsDeleted = paymentPage.IsDeleted,
                }).ToList(),
            })
            .AsNoTracking()
            .AsSplitQuery()
        .EnableCache($"PaymentChannelAdvancedGraphSpec-{paymentChannelId}");
        }
    }

    public sealed class PaymentChannelAdvancedGraphSpecV6 : Specification<PaymentChannel, PaymentChannelEntityDto>
    {
        public PaymentChannelAdvancedGraphSpecV6(
            Guid paymentChannelId,
            bool enableIntelligentProjection = true,
            bool enableSemanticAnalysis = true,
            bool enableBlueprintStrategy = true,
            int? take = null,
            int? skip = null)
        {
            Guard.Against.Default(paymentChannelId, nameof(paymentChannelId));
            Query.Where(c => c.PaymentChannelId == paymentChannelId);

            if (take.HasValue && skip.HasValue)
            {
                Query.Skip(skip.Value).Take(take.Value);
            }
            Query.Include(c => c.Merchant);
            if (enableBlueprintStrategy)
            {
            Query.Include(c => c.Merchant);

            }

            Query.Select(c => new PaymentChannelEntityDto
            {
                PaymentChannelId = c.PaymentChannelId,
                TenantId = c.TenantId,
                MerchantId = c.MerchantId,
                ChannelCode = c.ChannelCode,
                ChannelName = c.ChannelName,
                ChannelType = c.ChannelType,
                ProcessingVolume = c.ProcessingVolume,
                IsInScope = c.IsInScope,
                TokenizationEnabled = c.TokenizationEnabled,
                CreatedAt = c.CreatedAt,
                CreatedBy = c.CreatedBy,
                UpdatedAt = c.UpdatedAt,
                UpdatedBy = c.UpdatedBy,
                IsDeleted = c.IsDeleted,
                #region Intelligent Metrics
                IsChannelTypeCritical = c.ChannelType >= 7,
                DaysSinceUpdatedAt = (DateTime.UtcNow - c.UpdatedAt).Value.Days,
                IsInScopeFlag = c.IsInScope,
                TotalPaymentPageCount = c.PaymentPages.Count(),
                ActivePaymentPageCount = c.PaymentPages.Count(x => !x.IsDeleted),
                LatestPaymentPageUpdatedAt = c.PaymentPages.Max(x => (DateTime?)x.UpdatedAt),
                #endregion
                #region Semantic Pattern Fields
                #endregion

                Scripts = c.PaymentPages
                    .SelectMany(i => i.Scripts)
                    .Select(t => new ScriptEntityDto
                    {
                        ScriptId = t.ScriptId,
                        TenantId = t.TenantId,
                        PaymentPageId = t.PaymentPageId,
                        ScriptUrl = t.ScriptUrl,
                        ScriptHash = t.ScriptHash,
                        ScriptType = t.ScriptType,
                        IsAuthorized = t.IsAuthorized,
                        FirstSeen = t.FirstSeen,
                        LastSeen = t.LastSeen,
                        IsDeleted = t.IsDeleted,
                    })
                    .Take(2).ToList(),
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
                PaymentPages = c.PaymentPages.Select(paymentPage => new PaymentPageEntityDto
                {
                    PaymentPageId = paymentPage.PaymentPageId,
                    TenantId = paymentPage.TenantId,
                    PaymentChannelId = paymentPage.PaymentChannelId,
                    PageUrl = paymentPage.PageUrl,
                    PageName = paymentPage.PageName,
                    IsActive = paymentPage.IsActive,
                    LastScriptInventory = paymentPage.LastScriptInventory,
                    ScriptIntegrityHash = paymentPage.ScriptIntegrityHash,
                    CreatedAt = paymentPage.CreatedAt,
                    CreatedBy = paymentPage.CreatedBy,
                    UpdatedAt = paymentPage.UpdatedAt,
                    UpdatedBy = paymentPage.UpdatedBy,
                    IsDeleted = paymentPage.IsDeleted,
                }).ToList(),
            })
            .AsNoTracking()
            .AsSplitQuery()
            .EnableCache($"PaymentChannelAdvancedGraphSpecV6-{paymentChannelId}");
        }
    }

    public sealed class PaymentChannelAdvancedFilterSpec : Specification<PaymentChannel>
    {
        public PaymentChannelAdvancedFilterSpec(
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
                        case "channelname":
                            Query.Where(c => c.ChannelName.Contains(filter.Value));
                            break;
                        case "channelcode":
                            Query.Where(c => c.ChannelCode.Contains(filter.Value));
                            break;
                        case "channeltype":
                            if (int.TryParse(filter.Value, out int channeltype))
                            {
                                Query.Where(c => c.ChannelType == channeltype);
                            }
                            break;
                        case "processingvolume":
                            if (decimal.TryParse(filter.Value, out decimal processingvolume))
                            {
                               Query.Where(c => c.ProcessingVolume >= processingvolume - 0.1m && c.ProcessingVolume <= processingvolume + 0.1m);
                            }
                            break;
                        case "isinscope":
                            if (bool.TryParse(filter.Value, out bool isinscope))
                            {
                                Query.Where(c => c.IsInScope == isinscope);
                            }
                            break;
                        case "tokenizationenabled":
                            if (bool.TryParse(filter.Value, out bool tokenizationenabled))
                            {
                                Query.Where(c => c.TokenizationEnabled == tokenizationenabled);
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
                Query.OrderByDescending(x => x.PaymentChannelId);
            }

            Query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }

        private static IOrderedSpecificationBuilder<PaymentChannel> ApplySort(
            ISpecificationBuilder<PaymentChannel> query,
            Sort sort
        )
        {
            return sort.Direction == SortDirection.Descending
                ? query.OrderByDescending(GetSortProperty(sort.Field))
                : query.OrderBy(GetSortProperty(sort.Field));
        }

        private static IOrderedSpecificationBuilder<PaymentChannel> ApplyAdditionalSort(
            IOrderedSpecificationBuilder<PaymentChannel> query,
            Sort sort
        )
        {
            return sort.Direction == SortDirection.Descending
                ? query.ThenByDescending(GetSortProperty(sort.Field))
                : query.ThenBy(GetSortProperty(sort.Field));
        }

        private static Expression<Func<PaymentChannel, object>> GetSortProperty(
            string propertyName
        )
        {
            return propertyName.ToLower() switch
            {
                "channelname" => c => c.ChannelName,
                "channelcode" => c => c.ChannelCode,
                _ => c => c.PaymentChannelId,
            };
        }
    }