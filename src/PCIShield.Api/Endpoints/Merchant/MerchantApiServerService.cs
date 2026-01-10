using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

using Ardalis.GuardClauses;

using BlazorMauiShared.Models.Merchant;

using FastEndpoints;
using System.Diagnostics;
using FluentValidation;
using System.Reactive.Linq;
using System.Reactive;

using PCIShield.BlazorMauiShared.Models;
using PCIShield.BlazorMauiShared.ModelsDto;
using PCIShield.Domain.Entities;
using PCIShield.Domain.ModelEntityDto;
using PCIShield.Domain.ModelsDto;
using PCIShield.Domain.Specifications;
using PCIShield.Infrastructure.Services;

using PCIShieldLib.SharedKernel.Interfaces;

using LanguageExt;

using ModelingEvolution.Plumberd.RelationDataModeling;

using static LanguageExt.Prelude;

using UuidV7Generator = PCIShield.Domain.ModelsDto.UuidV7Generator;
using PCIShield.Api.CQRS;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

using Ardalis.GuardClauses;

using BlazorMauiShared.Models.Merchant;

using FastEndpoints;

using FluentValidation;

using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.Entities;
using PCIShield.Domain.ModelEntityDto;
using PCIShield.Domain.ModelsDto;
using PCIShield.Infrastructure.Services;
using PCIShield.Infrastructure.Services.Redis;

using PCIShieldLib.SharedKernel.Interfaces;

using LanguageExt;

using static LanguageExt.Prelude;

using Sort = PCIShieldLib.SharedKernel.Interfaces.Sort;
using MediatR;
using static MudBlazor.Icons;
using PCIShield.Infrastructure.Services.Elasticsearch;
using PCIShield.Infrastructure.Helpers;
using PCIShield.Infrastructure.Data;
using System.Threading;
using Ardalis.Specification;
using BlazorMauiShared.Models.Assessment;
using BlazorMauiShared.Models.Asset;
using BlazorMauiShared.Models.CompensatingControl;
using BlazorMauiShared.Models.ComplianceOfficer;
using BlazorMauiShared.Models.CryptographicInventory;
using BlazorMauiShared.Models.Evidence;
using BlazorMauiShared.Models.NetworkSegmentation;
using BlazorMauiShared.Models.PaymentChannel;
using BlazorMauiShared.Models.ServiceProvider;
using BlazorMauiShared.Models.Control;

public class MerchantService
{
    private readonly IRepositoryFactory _repositoryFactory;
    private readonly IReadRedisRepositoryFactory _repositoryCacheFactory;
    private readonly IElasticsearchService<Merchant> _merchantIndexService;

    private readonly IRepository<Assessment> _assessmentRepository;
    private readonly IReadRedisRepository<Assessment> _assessmentCacheRepository;
    private readonly IRepository<Asset> _assetRepository;
    private readonly IReadRedisRepository<Asset> _assetCacheRepository;
    private readonly IRepository<CompensatingControl> _compensatingControlRepository;
    private readonly IReadRedisRepository<CompensatingControl> _compensatingControlCacheRepository;
    private readonly IRepository<ComplianceOfficer> _complianceOfficerRepository;
    private readonly IReadRedisRepository<ComplianceOfficer> _complianceOfficerCacheRepository;
    private readonly IRepository<CryptographicInventory> _cryptographicInventoryRepository;
    private readonly IReadRedisRepository<CryptographicInventory> _cryptographicInventoryCacheRepository;
    private readonly IRepository<Evidence> _evidenceRepository;
    private readonly IReadRedisRepository<Evidence> _evidenceCacheRepository;
    private readonly IRepository<NetworkSegmentation> _networkSegmentationRepository;
    private readonly IReadRedisRepository<NetworkSegmentation> _networkSegmentationCacheRepository;
    private readonly IRepository<PaymentChannel> _paymentChannelRepository;
    private readonly IReadRedisRepository<PaymentChannel> _paymentChannelCacheRepository;
    private readonly IRepository<PCIShield.Domain.Entities.ServiceProvider> _serviceProviderRepository;
    private readonly IReadRedisRepository<PCIShield.Domain.Entities.ServiceProvider> _serviceProviderCacheRepository;
    private readonly IReadRedisRepository<Merchant> _merchantCacheRepository;
    private readonly IRepository<Merchant> _merchantRepository;
    private readonly IKeyCloakTenantService _keyCloakTenantService;
    private readonly IAppLoggerService<MerchantService> _logger;
    private readonly IRedisCacheService _redisCacheService;
    private readonly IValidator<MerchantDto> _updateMerchantValidator;
    private readonly IValidator<AssessmentDto> _updateAssessmentValidator;
    private readonly IValidator<AssetDto> _updateAssetValidator;
    private readonly IValidator<CompensatingControlDto> _updateCompensatingControlValidator;
    private readonly IValidator<ComplianceOfficerDto> _updateComplianceOfficerValidator;
    private readonly IValidator<CryptographicInventoryDto> _updateCryptographicInventoryValidator;
    private readonly IValidator<EvidenceDto> _updateEvidenceValidator;
    private readonly IValidator<NetworkSegmentationDto> _updateNetworkSegmentationValidator;
    private readonly IValidator<PaymentChannelDto> _updatePaymentChannelValidator;
    private readonly IValidator<ServiceProviderDto> _updateServiceProviderValidator;

    public MerchantService(

        IRepositoryFactory repositoryFactory,
        IReadRedisRepositoryFactory repositoryCacheFactory,
 
        IElasticsearchService<Merchant> merchantIndexService,

        IReadRedisRepository<Merchant> merchantCacheRepository,
        IRedisCacheService redisCacheService,
        IKeyCloakTenantService keyCloakTenantService,
        IRepository<Merchant> merchantRepository,
        IAppLoggerService<MerchantService> logger,
        IRepository<Assessment> assessmentRepository,
        IReadRedisRepository<Assessment> assessmentCacheRepository,
        IRepository<Asset> assetRepository,
        IReadRedisRepository<Asset> assetCacheRepository,
        IRepository<CompensatingControl> compensatingControlRepository,
        IReadRedisRepository<CompensatingControl> compensatingControlCacheRepository,
        IRepository<ComplianceOfficer> complianceOfficerRepository,
        IReadRedisRepository<ComplianceOfficer> complianceOfficerCacheRepository,
        IRepository<CryptographicInventory> cryptographicInventoryRepository,
        IReadRedisRepository<CryptographicInventory> cryptographicInventoryCacheRepository,
        IRepository<Evidence> evidenceRepository,
        IReadRedisRepository<Evidence> evidenceCacheRepository,
        IRepository<NetworkSegmentation> networkSegmentationRepository,
        IReadRedisRepository<NetworkSegmentation> networkSegmentationCacheRepository,
        IRepository<PaymentChannel> paymentChannelRepository,
        IReadRedisRepository<PaymentChannel> paymentChannelCacheRepository,
        IRepository<PCIShield.Domain.Entities.ServiceProvider> serviceProviderRepository,
        IReadRedisRepository<PCIShield.Domain.Entities.ServiceProvider> serviceProviderCacheRepository
    )
    {

        _repositoryFactory = repositoryFactory;
        _repositoryCacheFactory = repositoryCacheFactory;
        _merchantIndexService = merchantIndexService;
        _updateMerchantValidator = new InlineValidator<MerchantDto>();
        _updateAssessmentValidator = new InlineValidator<AssessmentDto>();
        _updateAssetValidator = new InlineValidator<AssetDto>();
        _updateCompensatingControlValidator = new InlineValidator<CompensatingControlDto>();
        _updateComplianceOfficerValidator = new InlineValidator<ComplianceOfficerDto>();
        _updateCryptographicInventoryValidator = new InlineValidator<CryptographicInventoryDto>();
        _updateEvidenceValidator = new InlineValidator<EvidenceDto>();
        _updateNetworkSegmentationValidator = new InlineValidator<NetworkSegmentationDto>();
        _updatePaymentChannelValidator = new InlineValidator<PaymentChannelDto>();
        _updateServiceProviderValidator = new InlineValidator<ServiceProviderDto>();
        _merchantCacheRepository = merchantCacheRepository;
        _redisCacheService = redisCacheService;
        _keyCloakTenantService = keyCloakTenantService;
        _merchantRepository = merchantRepository;
        _logger = logger;
        _assessmentRepository = assessmentRepository;
        _assessmentCacheRepository = assessmentCacheRepository;
        _assetRepository = assetRepository;
        _assetCacheRepository = assetCacheRepository;
        _compensatingControlRepository = compensatingControlRepository;
        _compensatingControlCacheRepository = compensatingControlCacheRepository;
        _complianceOfficerRepository = complianceOfficerRepository;
        _complianceOfficerCacheRepository = complianceOfficerCacheRepository;
        _cryptographicInventoryRepository = cryptographicInventoryRepository;
        _cryptographicInventoryCacheRepository = cryptographicInventoryCacheRepository;
        _evidenceRepository = evidenceRepository;
        _evidenceCacheRepository = evidenceCacheRepository;
        _networkSegmentationRepository = networkSegmentationRepository;
        _networkSegmentationCacheRepository = networkSegmentationCacheRepository;
        _paymentChannelRepository = paymentChannelRepository;
        _paymentChannelCacheRepository = paymentChannelCacheRepository;
        _serviceProviderRepository = serviceProviderRepository;
        _serviceProviderCacheRepository = serviceProviderCacheRepository;
    }

    private async Task InvalidateAssessmentCacheAsync(Guid assessmentId)
    {
        var tenantId = _keyCloakTenantService.GetPCIShieldSolutionuperUserId();
        await _redisCacheService.RemoveByPatternAsync($"AssessmentListPagedSpec--{tenantId}-*");
        await _redisCacheService.RemoveByPatternAsync(
            $"AssessmentByIdSpec-{assessmentId.ToString()}--{tenantId}-FirstOrDefaultAsync-AssessmentEntityDto"
        );
        await _redisCacheService.RemoveByPatternAsync(
            $"AssessmentListPagedSpec-1-10--{tenantId}-ListAsync-AssessmentEntityDto"
        );
    }

    private async Task InvalidateAssetCacheAsync(Guid assetId)
    {
        var tenantId = _keyCloakTenantService.GetPCIShieldSolutionuperUserId();
        await _redisCacheService.RemoveByPatternAsync($"AssetListPagedSpec--{tenantId}-*");
        await _redisCacheService.RemoveByPatternAsync(
            $"AssetByIdSpec-{assetId.ToString()}--{tenantId}-FirstOrDefaultAsync-AssetEntityDto"
        );
        await _redisCacheService.RemoveByPatternAsync(
            $"AssetListPagedSpec-1-10--{tenantId}-ListAsync-AssetEntityDto"
        );
    }

    private async Task InvalidateCompensatingControlCacheAsync(Guid compensatingControlId)
    {
        var tenantId = _keyCloakTenantService.GetPCIShieldSolutionuperUserId();
        await _redisCacheService.RemoveByPatternAsync($"CompensatingControlListPagedSpec--{tenantId}-*");
        await _redisCacheService.RemoveByPatternAsync(
            $"CompensatingControlByIdSpec-{compensatingControlId.ToString()}--{tenantId}-FirstOrDefaultAsync-CompensatingControlEntityDto"
        );
        await _redisCacheService.RemoveByPatternAsync(
            $"CompensatingControlListPagedSpec-1-10--{tenantId}-ListAsync-CompensatingControlEntityDto"
        );
    }

    private async Task InvalidateComplianceOfficerCacheAsync(Guid complianceOfficerId)
    {
        var tenantId = _keyCloakTenantService.GetPCIShieldSolutionuperUserId();
        await _redisCacheService.RemoveByPatternAsync($"ComplianceOfficerListPagedSpec--{tenantId}-*");
        await _redisCacheService.RemoveByPatternAsync(
            $"ComplianceOfficerByIdSpec-{complianceOfficerId.ToString()}--{tenantId}-FirstOrDefaultAsync-ComplianceOfficerEntityDto"
        );
        await _redisCacheService.RemoveByPatternAsync(
            $"ComplianceOfficerListPagedSpec-1-10--{tenantId}-ListAsync-ComplianceOfficerEntityDto"
        );
    }

    private async Task InvalidateCryptographicInventoryCacheAsync(Guid cryptographicInventoryId)
    {
        var tenantId = _keyCloakTenantService.GetPCIShieldSolutionuperUserId();
        await _redisCacheService.RemoveByPatternAsync($"CryptographicInventoryListPagedSpec--{tenantId}-*");
        await _redisCacheService.RemoveByPatternAsync(
            $"CryptographicInventoryByIdSpec-{cryptographicInventoryId.ToString()}--{tenantId}-FirstOrDefaultAsync-CryptographicInventoryEntityDto"
        );
        await _redisCacheService.RemoveByPatternAsync(
            $"CryptographicInventoryListPagedSpec-1-10--{tenantId}-ListAsync-CryptographicInventoryEntityDto"
        );
    }

    private async Task InvalidateEvidenceCacheAsync(Guid evidenceId)
    {
        var tenantId = _keyCloakTenantService.GetPCIShieldSolutionuperUserId();
        await _redisCacheService.RemoveByPatternAsync($"EvidenceListPagedSpec--{tenantId}-*");
        await _redisCacheService.RemoveByPatternAsync(
            $"EvidenceByIdSpec-{evidenceId.ToString()}--{tenantId}-FirstOrDefaultAsync-EvidenceEntityDto"
        );
        await _redisCacheService.RemoveByPatternAsync(
            $"EvidenceListPagedSpec-1-10--{tenantId}-ListAsync-EvidenceEntityDto"
        );
    }

    private async Task InvalidateNetworkSegmentationCacheAsync(Guid networkSegmentationId)
    {
        var tenantId = _keyCloakTenantService.GetPCIShieldSolutionuperUserId();
        await _redisCacheService.RemoveByPatternAsync($"NetworkSegmentationListPagedSpec--{tenantId}-*");
        await _redisCacheService.RemoveByPatternAsync(
            $"NetworkSegmentationByIdSpec-{networkSegmentationId.ToString()}--{tenantId}-FirstOrDefaultAsync-NetworkSegmentationEntityDto"
        );
        await _redisCacheService.RemoveByPatternAsync(
            $"NetworkSegmentationListPagedSpec-1-10--{tenantId}-ListAsync-NetworkSegmentationEntityDto"
        );
    }

    private async Task InvalidatePaymentChannelCacheAsync(Guid paymentChannelId)
    {
        var tenantId = _keyCloakTenantService.GetPCIShieldSolutionuperUserId();
        await _redisCacheService.RemoveByPatternAsync($"PaymentChannelListPagedSpec--{tenantId}-*");
        await _redisCacheService.RemoveByPatternAsync(
            $"PaymentChannelByIdSpec-{paymentChannelId.ToString()}--{tenantId}-FirstOrDefaultAsync-PaymentChannelEntityDto"
        );
        await _redisCacheService.RemoveByPatternAsync(
            $"PaymentChannelListPagedSpec-1-10--{tenantId}-ListAsync-PaymentChannelEntityDto"
        );
    }

    private async Task InvalidateServiceProviderCacheAsync(Guid serviceProviderId)
    {
        var tenantId = _keyCloakTenantService.GetPCIShieldSolutionuperUserId();
        await _redisCacheService.RemoveByPatternAsync($"ServiceProviderListPagedSpec--{tenantId}-*");
        await _redisCacheService.RemoveByPatternAsync(
            $"ServiceProviderByIdSpec-{serviceProviderId.ToString()}--{tenantId}-FirstOrDefaultAsync-ServiceProviderEntityDto"
        );
        await _redisCacheService.RemoveByPatternAsync(
            $"ServiceProviderListPagedSpec-1-10--{tenantId}-ListAsync-ServiceProviderEntityDto"
        );
    }

    private async Task InvalidateControlCacheAsync(Guid controlId)
    {
        var tenantId = _keyCloakTenantService.GetPCIShieldSolutionuperUserId();
        await _redisCacheService.RemoveByPatternAsync($"ControlListPagedSpec--{tenantId}-*");
        await _redisCacheService.RemoveByPatternAsync(
            $"ControlByIdSpec-{controlId.ToString()}--{tenantId}-FirstOrDefaultAsync-ControlEntityDto"
        );
        await _redisCacheService.RemoveByPatternAsync(
            $"ControlListPagedSpec-1-10--{tenantId}-ListAsync-ControlEntityDto"
        );
    }

    private async Task InvalidateMerchantCacheAsync(Guid merchantId)
    {

        var tenantId = _keyCloakTenantService.GetPCIShieldSolutionuperUserId();
        await _redisCacheService.RemoveByPatternAsync($"MerchantListPagedSpec--{tenantId}-*");
        await _redisCacheService.RemoveByPatternAsync(
            $"MerchantByIdSpec-{merchantId.ToString()}--{tenantId}-FirstOrDefaultAsync-MerchantEntityDto"
        );
        await _redisCacheService.RemoveByPatternAsync(
            $"MerchantListPagedSpec-1-10--{tenantId}-ListAsync-MerchantEntityDto"
        );
    }

    public async Task<Either<string, MerchantDto>> GetOneFullMerchantByIdAsync(Guid merchantId, bool withPostGraph = false)
    {
        try
        {
            MerchantAdvancedGraphSpecV4 spec = new(merchantId);
            MerchantEntityDto? merchant = await _repositoryCacheFactory.GetReadRedisRepository<Merchant>().FirstOrDefaultAsync(spec);
            GetByIdMerchantEntityDtoMapper mapper = new();
            if (merchant != null)
            {
                if (withPostGraph)
                {
                }
                MerchantDto? merchantDto = mapper.FromEntityDto(merchant);
                if (merchantDto != null)
                {
                    return Right<string, MerchantDto>(merchantDto);
                }
                else
                {
                    return Left<string, MerchantDto>($"Error retrieving merchant");
                }
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError($"JsonException: {ex.Message}");
            _logger.LogError($"Path: {ex.Path}, LineNumber: {ex.LineNumber}, BytePositionInLine: {ex.BytePositionInLine}");
            _logger.LogError(ex.Message, "Error retrieving merchant {MerchantId} with includes", merchantId);
            return Left<string, MerchantDto>($"Error retrieving merchant: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "Error retrieving merchant {MerchantId} with includes", merchantId);
            return Left<string, MerchantDto>($"Error retrieving merchant: {ex.Message}");
        }
        return default;
    }
    //public async Task<Either<string, SpecificationPerformanceResponse<MerchantEntityDto>>> RunMerchantPerformanceTests(
    //    Guid merchantId,
    //    bool disableCache = false)
    //{
    //    try
    //    {
    //        var perfLogger = _logger.ForType<SpecificationPerformanceTracker<Merchant, MerchantEntityDto>>();
    //        var tracker = new SpecificationPerformanceTracker<Merchant, MerchantEntityDto>(perfLogger);
    //        var complexityMetrics = new Dictionary<string, SpecificationPerformanceResponse<MerchantEntityDto>.QueryComplexityAnalysis.SpecComplexityMetrics>
    //        {
    //            {"MerchantByIdSpec", new SpecificationPerformanceResponse<MerchantEntityDto>.QueryComplexityAnalysis.SpecComplexityMetrics
    //                {
    //                    EstimatedJoinCount = 12,
    //                    IncludeCount = 12,
    //                    ProjectionDepth = 2,
    //                    UsesSplitQuery = true,
    //                    UsesNoTracking = true,
    //                    RelationshipDepth = 2,
    //                    ComplexityLevel = "Medium",
    //                    CalculatedPropertyCount = 0
    //                }
    //            },
    //            { "MerchantAdvancedGraphSpecV4",  new SpecificationPerformanceResponse<MerchantEntityDto>.QueryComplexityAnalysis.SpecComplexityMetrics
    //                {
    //                    EstimatedJoinCount = 18,
    //                    IncludeCount = 3,
    //                    ProjectionDepth = 4,
    //                    UsesSplitQuery = true,
    //                    UsesNoTracking = true,
    //                    RelationshipDepth = 4,
    //                    ComplexityLevel = "High",
    //                    CalculatedPropertyCount = 25
    //                }
    //            },
    //        };

    //        tracker.ConfigureEntityMetrics(
    //            new Dictionary<string, int> { { "MerchantByIdSpec", 150 }, { "MerchantAdvancedGraphSpecV4", 250 } },
    //            new Dictionary<string, int> { { "MerchantByIdSpec", 12 }, { "MerchantAdvancedGraphSpecV4", 18 } },
    //            complexityMetrics
    //        );
    //        var testConfigurations = new List<(string Name, Func<Guid, ISpecification<Merchant, MerchantEntityDto>> SpecFactory)>
    //        {
    //            ("MerchantAdvancedGraphSpecV4", id => (ISpecification<Merchant, MerchantEntityDto>)new MerchantAdvancedGraphSpecV4(id)),
    //            ("MerchantByIdSpec", id => (ISpecification<Merchant, MerchantEntityDto>)new PCIShield.Domain.Specifications.MerchantByIdSpec(id))
    //        };
    //        foreach (var config in testConfigurations)
    //        {
    //            for (int i = 0; i < 2; i++)
    //            {
    //                var stopwatch = Stopwatch.StartNew();
    //                var spec = config.SpecFactory(merchantId);
    //                var includesProperty = spec.GetType().GetProperty("Includes");
    //                if (includesProperty != null)
    //                {
    //                    var includes = includesProperty.GetValue(spec) as System.Collections.IEnumerable;
    //                    if (includes != null)
    //                    {
    //                        int count = 0;
    //                        foreach (var _ in includes)
    //                            count++;

    //                        tracker.UpdateMetric(config.Name, "IncludeCount", count);
    //                    }
    //                }
    //               tracker.UpdateSpecificationComplexityMetrics<Merchant, MerchantEntityDto>(spec, config.Name, tracker);

    //                MerchantEntityDto? merchant = null;

    //                if (disableCache)
    //                {
    //                    merchant = await _repositoryFactory.GetRepository<Merchant>().FirstOrDefaultAsync(spec);
    //                }
    //                else
    //                {
    //                    merchant = await _repositoryCacheFactory.GetReadRedisRepository<Merchant>().FirstOrDefaultAsync(spec);
    //                }
    //                if (merchant == null)
    //                {
    //                    _logger.LogWarning($"No merchant found for ID {merchantId} using {config.Name}");
    //                }

    //                stopwatch.Stop();
    //                tracker.AddTiming(config.Name, stopwatch.ElapsedMilliseconds);

    //                if (merchant != null)
    //                {
    //                    var mapper = new GetByIdMerchantEntityDtoMapper();
    //                    var merchantDto = mapper.FromEntityDto(merchant);
    //                }
    //                await Task.Delay(200);
    //            }
    //        }

    //        tracker.LogSummary();

    //        var response = new SpecificationPerformanceResponse<MerchantEntityDto>
    //        {
    //            TimingResults = tracker.GetTimings(),
    //            Statistics = tracker.CalculateStatistics(),
    //            ComplexityAnalysis = tracker.GetComplexityAnalysis(),
    //            ErrorMessage = null
    //        };
    //        if (response.Statistics.Count > 0)
    //        {
    //            var sortedStats = response.Statistics.OrderBy(s => s.Value.AverageMs).ToList();
    //            response.FastestSpecification = sortedStats.First().Key;

    //            if (sortedStats.Count > 1)
    //            {
    //                var mostConsistent = response.Statistics
    //                    .OrderBy(s => s.Value.VariabilityCoefficient)
    //                    .First().Key;
    //                response.MostConsistentSpecification = mostConsistent;

    //                var recommended = response.Statistics
    //                    .OrderBy(s => s.Value.Rank + s.Value.VariabilityCoefficient * 5)
    //                    .First().Key;
    //                response.RecommendedSpecification = recommended;

    //                var fastest = sortedStats.First().Value;
    //                var slowest = sortedStats.Last().Value;
    //                response.MaximumSpeedupFactor = Math.Round(slowest.AverageMs / fastest.AverageMs, 2);
    //            }
    //        }
    //        return Right<string, SpecificationPerformanceResponse<MerchantEntityDto>>(response);
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex.Message, "Error running performance tests for merchant {MerchantId}", merchantId);
    //        return Left<string, SpecificationPerformanceResponse<MerchantEntityDto>>($"Error running performance tests: {ex.Message}");
    //    }
    //}

    public async Task<Either<string, List<MerchantDto>>> SearchMerchantsAsync(string searchTerm)
    {
        try
        {
            Guard.Against.NullOrWhiteSpace(searchTerm, nameof(searchTerm));
            var spec = new MerchantSearchSpec(searchTerm);
            var merchants = await _merchantCacheRepository.ListAsync(spec);
            if (!merchants.Any())
            {
                return new List<MerchantDto>();
            }
            var settings = GJset.GetSystemTextJsonSettings();
            var merchantDtos = merchants.Select(c => JsonSerializer.Deserialize<MerchantDto>(JsonSerializer.Serialize(c, settings), settings)).Where(dto => dto != null).ToList();
            return Right<string, List<MerchantDto>>(merchantDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "Error searching merchants with term: {SearchTerm}", searchTerm);
            return Left<string, List<MerchantDto>>($"Error searching merchants: {ex.Message}");
        }
    }
    public async Task<Either<string, ListMerchantResponse>> GetFilteredMerchantsAsync(FilteredMerchantRequest req)
    {
        try
        {
            Guard.Against.NegativeOrZero(req.PageNumber, nameof(req.PageNumber));
            Guard.Against.NegativeOrZero(req.PageSize, nameof(req.PageSize));
            List<PCIShieldLib.SharedKernel.Interfaces.Sort> convertedSortings = new();
            var errors = new List<string>();
            if (req.Sorting == null)
            {
                req.Sorting = new List<Sort>();
            }
            foreach (var sort1 in req.Sorting)
            {
                var sort = (Sort)sort1;
                if (string.IsNullOrWhiteSpace(sort.Field))
                {
                    errors.Add("Sort field cannot be empty");
                    continue;
                }
                convertedSortings.Add(new PCIShieldLib.SharedKernel.Interfaces.Sort { Field = sort.Field, Direction = sort.Direction });
            }
            if (errors.Any())
            {
                return Left<string, ListMerchantResponse>(string.Join("; ", errors));
            }
            var spec = new MerchantAdvancedFilterSpec(req.PageNumber, req.PageSize, req.Filters, convertedSortings);
            var merchants = await _merchantCacheRepository.ListAsync(spec);
            if (!merchants.Any())
            {
                return Right<string, ListMerchantResponse>(new ListMerchantResponse { Merchants = null, Count = 0 });
            }
            var settings = GJset.GetSystemTextJsonSettings();
            var merchantDtos = merchants
                .Skip((req.PageNumber - 1) * req.PageSize)
                .Take(req.PageSize)
                .Select(c => JsonSerializer.Deserialize<MerchantDto>(JsonSerializer.Serialize(c, settings), settings))
                .Where(dto => dto != null)
                .ToList();
            return Right<string, ListMerchantResponse>(new ListMerchantResponse { Merchants = merchantDtos, Count = merchants.Count });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error filtering merchants");
            return Left<string, ListMerchantResponse>($"Error filtering merchants: {ex.Message}");
        }
    }
    public async Task<Either<string, ListMerchantResponse>> GetMerchantPagedList(int pageNumber, int pageSize)
    {
        try
        {
            MerchantListPagedSpec spec = new MerchantListPagedSpec(pageNumber, pageSize);

            PagedResult<MerchantEntityDto> pagedResult = await _repositoryCacheFactory.GetReadRedisRepository<Merchant>().GetPagedResultAsync(spec, pageNumber, pageSize);

            if (!pagedResult.Items.Any())
                return Left<string, ListMerchantResponse>("No merchants found.");

            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            List<MerchantDto?> merchantDtos = pagedResult.Items
                .Select(entityDto =>
                {
                    var jsonMerchant = JsonSerializer.Serialize(entityDto, settings);
                    return JsonSerializer.Deserialize<MerchantDto>(jsonMerchant, settings);
                })
                .Where(dto => dto != null)
                .ToList();

            if (!merchantDtos.Any())
                return Left<string, ListMerchantResponse>("Error mapping merchants.");

            var response = new ListMerchantResponse
            {
                Merchants = merchantDtos,
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize,
                TotalPages = pagedResult.TotalPages
            };

            return Right<string, ListMerchantResponse>(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "Error retrieving merchant list for page {PageNumber} with size {PageSize}",
                pageNumber, pageSize);
            return Left<string, ListMerchantResponse>($"Error retrieving merchant list: {ex.Message}");
        }
    }

    public async Task<Either<string, bool>> DeleteMerchantAsync(Guid merchantId)
    {
        try
        {
            Guard.Against.NullOrEmpty(merchantId, nameof(merchantId));
            var merchant = await _merchantRepository.GetByIdAsync(merchantId);
            if (merchant == null)
            {
                return Left<string, bool>("Merchant not found.");
            }
            merchant.SetIsDeleted(true);
            await _merchantRepository.UpdateAsync(merchant);
            await InvalidateMerchantCacheAsync(merchant.MerchantId);
            return Right<string, bool>(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting merchant");
            return Left<string, bool>($"Error deleting merchant: {ex.Message}");
        }
    }
    public async Task<Either<string, MerchantDto>> GetLastCreatedMerchantAsync()
    {
        try
        {
            var spec = new MerchantLastCreatedSpec();
            Merchant? merchant = await _merchantRepository.FirstOrDefaultAsync(spec);

            if (merchant == null)
                return Left<string, MerchantDto>("No merchants found");

            var mapper = new GetByIdMerchantFromEntityMapper();
            MerchantDto? merchantDto = mapper.FromEntity(merchant);

            return merchantDto != null
                ? Right<string, MerchantDto>(merchantDto)
                : Left<string, MerchantDto>("Error mapping merchant to DTO");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving last created merchant");
            return Left<string, MerchantDto>($"Error retrieving last created merchant: {ex.Message}");
        }
    }

    public async Task<Either<string, MerchantDto>> CreateMerchantAsync(MerchantDto merchantDto)
    {
        try
        {
            Guard.Against.Null(merchantDto, nameof(merchantDto));
            var merchantId = Guid.CreateVersion7();
            var createdDate = DateTime.UtcNow;
            var createdBy = merchantDto.CreatedBy;
            var createMerchantEff = Merchant.Create(
                merchantId: merchantId,
                     tenantId:merchantDto.TenantId,
                     merchantCode:merchantDto.MerchantCode,
                     merchantName:merchantDto.MerchantName,
                     merchantLevel:merchantDto.MerchantLevel,
                     acquirerName:merchantDto.AcquirerName,
                     processorMID:merchantDto.ProcessorMID,
                     annualCardVolume:merchantDto.AnnualCardVolume,
                     nextAssessmentDue:merchantDto.NextAssessmentDue,
                     complianceRank:merchantDto.ComplianceRank,
                     createdAt:merchantDto.CreatedAt,
                     createdBy:merchantDto.CreatedBy,
                     isDeleted:merchantDto.IsDeleted
            );

            var validationResult = await _updateMerchantValidator.ValidateAsync(merchantDto);
            if (!validationResult.IsValid)
            {
                var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                return Left<string, MerchantDto>($"Validation failed: {errors}");
            }
            var spec = new MerchantLastCreatedSpec();

            var newMerchant = new Merchant(
                merchantId: Guid.CreateVersion7(),
                      merchantDto.TenantId,
                      merchantDto.MerchantCode,
                      merchantDto.MerchantName,
                      merchantDto.MerchantLevel,
                      merchantDto.AcquirerName,
                      merchantDto.ProcessorMID,
                      merchantDto.AnnualCardVolume,
                      merchantDto.NextAssessmentDue,
                      merchantDto.ComplianceRank,
                      merchantDto.CreatedAt,
                      merchantDto.CreatedBy,
                      merchantDto.IsDeleted
            );
            newMerchant.SetLastAssessmentDate(merchantDto.LastAssessmentDate);
            newMerchant.SetUpdatedAt(merchantDto.UpdatedAt);
            newMerchant.SetIsDeleted(false);

            var validation = createMerchantEff.Run();
            if (validation.IsFail)
            {
                var errors = string.Join("; ", validation.ToList());
                return Left<string, MerchantDto>($"Error creating merchant: {errors}");
            }
             bool merchant = validation.IsSucc;

            _merchantRepository.BeginTransaction();

            await _merchantRepository.AddAsync(newMerchant);
            await InvalidateMerchantCacheAsync(newMerchant.MerchantId);

            _merchantRepository.CommitTransaction();

            var mapper = new GetByIdMerchantFromEntityMapper();
            var createdMerchantDto = mapper.FromEntity(newMerchant);
            return Right<string, MerchantDto>(createdMerchantDto);
        }
        catch (Exception ex)
        {
            _merchantRepository.RollbackTransaction();
            _logger.LogError(ex, "Error creating merchant");
            return Left<string, MerchantDto>($"Error creating merchant: {ex.Message}");
        }
    }

    public async Task<Either<string, MerchantDto>> UpdateMerchantAsync(Guid merchantId, MerchantDto merchantDto)
    {
        try
        {
            Guard.Against.NullOrEmpty(merchantId, nameof(merchantId));
            Guard.Against.Null(merchantDto, nameof(merchantDto));
            var validationResult = await _updateMerchantValidator.ValidateAsync(merchantDto);
            if (!validationResult.IsValid)
            {
                var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                return Left<string, MerchantDto>($"Validation failed: {errors}");
            }
            _merchantRepository.BeginTransaction();
            try
            {
                Merchant? merchant = await _merchantRepository.GetByIdAsync(merchantId);
                if (merchant == null)
                {
                    return Left<string, MerchantDto>($"Merchant with ID {merchantDto.MerchantId} not found.");
                }

                var updateResult = Merchant
                    .Update(
                        merchant,
                        merchantDto.MerchantId,
                        merchantDto.TenantId,
                        merchantDto.MerchantCode,
                        merchantDto.MerchantName,
                        merchantDto.MerchantLevel,
                        merchantDto.AcquirerName,
                        merchantDto.ProcessorMID,
                        merchantDto.AnnualCardVolume,
                        merchantDto.LastAssessmentDate,
                        merchantDto.NextAssessmentDue,
                        merchantDto.ComplianceRank,
                        merchantDto.CreatedAt,
                        merchantDto.CreatedBy,
                        merchantDto.UpdatedAt,
                        merchantDto.UpdatedBy,
                      
                        merchantDto.IsDeleted                    )
                    .Run();
                if (updateResult.IsFail)
                {
                    Validation<string, Merchant> validation = updateResult.ToList().FirstOrDefault();
                    return Left<string, MerchantDto>("Merchant update failed domain validation");
                }

                merchant.SetIsDeleted(merchantDto.IsDeleted);
                await _merchantRepository.UpdateAsync(merchant);

                await InvalidateMerchantCacheAsync(merchant.MerchantId);
                _merchantRepository.CommitTransaction();

                var mapper = new GetByIdMerchantFromEntityMapper();
                var updatedMerchantDto = mapper.FromEntity(merchant);

                if (updatedMerchantDto == null)
                {
                    return Left<string, MerchantDto>("Error mapping updated merchant to DTO");
                }

                return Right<string, MerchantDto>(updatedMerchantDto);
            }
            catch (Exception e)
            {
                _merchantRepository.RollbackTransaction();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "Error updating merchant {MerchantId}", merchantDto.MerchantId);
            return Left<string, MerchantDto>($"Error updating merchant: {ex.Message}");
        }
    }

    public class GetByIdMerchantEntityDtoMapper : Mapper<GetByIdMerchantRequest, GetByIdMerchantResponse, MerchantDto>
    {
        public  MerchantDto? FromEntityDto(MerchantEntityDto merchant)
        {
            Guard.Against.Null(merchant, nameof(merchant));
            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            string jsonMerchant = JsonSerializer.Serialize(merchant, settings);
            MerchantDto? merchantDto = JsonSerializer.Deserialize<MerchantDto>(jsonMerchant, settings);
            return merchantDto;
        }
    }

    public class GetByIdMerchantFromEntityMapper : Mapper<GetByIdMerchantRequest, GetByIdMerchantResponse, MerchantDto>
    {
        public   MerchantDto? FromEntity(Merchant merchant)
        {
            Guard.Against.Null(merchant, nameof(merchant));
            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            string jsonMerchant = JsonSerializer.Serialize(merchant, settings);
            MerchantDto? merchantDto = JsonSerializer.Deserialize<MerchantDto>(jsonMerchant, settings);
            return merchantDto;
        }
    }
        public async Task<Either<string, AssessmentDto>> GetOneFullAssessmentByIdAsync(Guid assessmentId)
        {
            try
            {
            AssessmentByIdSpec spec = new(assessmentId);
                AssessmentEntityDto? assessment = await _assessmentCacheRepository.FirstOrDefaultAsync(spec);
                GetByIdAssessmentEntityDtoMapper mapper = new();

                if (assessment != null)
                {
                    AssessmentDto? assessmentDto = mapper.FromEntityDto(assessment);
                    return assessmentDto != null
                        ? Right<string, AssessmentDto>(assessmentDto)
                        : Left<string, AssessmentDto>("Error mapping assessment to DTO");
                }
                return Left<string, AssessmentDto>("Assessment not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error retrieving assessment {AssessmentId}", assessmentId);
                return Left<string, AssessmentDto>($"Error retrieving assessment: {ex.Message}");
            }
        }

        public async Task<Either<string, List<AssessmentDto>>> SearchAssessmentsAsync(string searchTerm)
        {
            try
            {
                Guard.Against.NullOrWhiteSpace(searchTerm, nameof(searchTerm));
                var spec = new AssessmentSearchSpec(searchTerm);
                var assessments = await _assessmentCacheRepository.ListAsync(spec);

                var settings = GJset.GetSystemTextJsonSettings();
                var assessmentDtos = assessments
                    .Select(i => JsonSerializer.Deserialize<AssessmentDto>(JsonSerializer.Serialize(i, settings), settings))
                    .Where(dto => dto != null)
                    .ToList();

                return Right<string, List<AssessmentDto>>(assessmentDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error searching assessments with term: {SearchTerm}", searchTerm);
                return Left<string, List<AssessmentDto>>($"Error searching assessments: {ex.Message}");
            }
        }

        public async Task<Either<string, ListAssessmentResponse>> GetFilteredAssessmentsAsync(FilteredAssessmentRequest req)
        {
            try
            {
                Guard.Against.NegativeOrZero(req.PageNumber, nameof(req.PageNumber));
                Guard.Against.NegativeOrZero(req.PageSize, nameof(req.PageSize));

                List<PCIShieldLib.SharedKernel.Interfaces.Sort> convertedSortings = new();
                var errors = new List<string>();

            if (req.Sorting == null)
            {
                req.Sorting = new List<Sort>();
            }
            foreach (var sort1 in req.Sorting)
            {
                    var sort = (Sort)sort1;
                    if (string.IsNullOrWhiteSpace(sort.Field))
                    {
                        errors.Add("Sort field cannot be empty");
                        continue;
                    }
                    convertedSortings.Add(
                        new PCIShieldLib.SharedKernel.Interfaces.Sort { Field = sort.Field, Direction = sort.Direction }
                    );
                }

                if (errors.Any())
                    return Left<string, ListAssessmentResponse>(string.Join("; ", errors));

                var spec = new AssessmentAdvancedFilterSpec(req.PageNumber, req.PageSize, req.Filters, convertedSortings);
                var assessments = await _assessmentCacheRepository.ListAsync(spec);

                if (!assessments.Any())
                    return Right<string, ListAssessmentResponse>(new ListAssessmentResponse { Assessments = null, Count = 0 });

                var settings = GJset.GetSystemTextJsonSettings();
                var assessmentDtos = assessments
                    .Skip((req.PageNumber - 1) * req.PageSize)
                    .Take(req.PageSize)
                    .Select(i => JsonSerializer.Deserialize<AssessmentDto>(JsonSerializer.Serialize(i, settings), settings))
                    .Where(dto => dto != null)
                    .ToList();

                return Right<string, ListAssessmentResponse>(
                    new ListAssessmentResponse { Assessments = assessmentDtos, Count = assessments.Count }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error filtering assessments");
                return Left<string, ListAssessmentResponse>($"Error filtering assessments: {ex.Message}");
            }
        }

        public async Task<Either<string, List<AssessmentDto>>> GetAssessmentPagedList(int pageNumber, int pageSize)
        {
            try
            {
                var spec = new AssessmentListPagedSpec(pageNumber, pageSize);
                var assessmentEntityDtos = await _assessmentCacheRepository.ListAsync(spec);

                if (!assessmentEntityDtos.Any())
                    return Left<string, List<AssessmentDto>>("No assessments found.");

                var settings = GJset.GetSystemTextJsonSettings();
                var assessmentDtos = assessmentEntityDtos
                    .Select(entityDto =>
                    {
                        var jsonAssessment = JsonSerializer.Serialize(entityDto, settings);
                        return JsonSerializer.Deserialize<AssessmentDto>(jsonAssessment, settings);
                    })
                    .Where(dto => dto != null)
                    .ToList();

                return assessmentDtos.Any()
                    ? Right<string, List<AssessmentDto>>(assessmentDtos)
                    : Left<string, List<AssessmentDto>>("Error mapping assessments.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error retrieving assessment list for page {PageNumber} with size {PageSize}",
                    pageNumber, pageSize);
                return Left<string, List<AssessmentDto>>($"Error retrieving assessment list: {ex.Message}");
            }
        }

        public async Task<Either<string, bool>> DeleteAssessmentAsync(Guid assessmentId)
        {
            try
            {
                Guard.Against.NullOrEmpty(assessmentId, nameof(assessmentId));

                var assessment = await _assessmentRepository.GetByIdAsync(assessmentId);

                if (assessment == null)
                    return Left<string, bool>("Assessment not found.");

                assessment.SetIsDeleted(true);
                await _assessmentRepository.UpdateAsync(assessment);
                await InvalidateAssessmentCacheAsync(assessmentId);

                return Right<string, bool>(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting assessment");
                return Left<string, bool>($"Error deleting assessment: {ex.Message}");
            }
        }

        public async Task<Either<string, AssessmentDto>> GetLastCreatedAssessmentAsync()
        {
            try
            {
                var spec = new AssessmentLastCreatedSpec();
                var assessment = await _assessmentRepository.FirstOrDefaultAsync(spec);

                if (assessment == null)
                    return Left<string, AssessmentDto>("No assessments found");

                var mapper = new GetByIdAssessmentFromEntityMapper();
                var assessmentDto = mapper.FromEntity(assessment);

                return assessmentDto != null
                    ? Right<string, AssessmentDto>(assessmentDto)
                    : Left<string, AssessmentDto>("Error mapping assessment to DTO");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving last created assessment");
                return Left<string, AssessmentDto>($"Error retrieving last created assessment: {ex.Message}");
            }
        }

        public async Task<Either<string, AssessmentDto>> CreateAssessmentAsync(AssessmentDto assessmentDto)
        {
            try
            {
                Guard.Against.Null(assessmentDto, nameof(assessmentDto));

                var newAssessment = new Assessment(

                    assessmentId: Guid.CreateVersion7(),
                      assessmentDto.MerchantId,
                      assessmentDto.TenantId,
                      assessmentDto.AssessmentCode,
                      assessmentDto.AssessmentType,
                      assessmentDto.AssessmentPeriod,
                      assessmentDto.StartDate,
                      assessmentDto.EndDate,
                      assessmentDto.Rank,
                      assessmentDto.QSAReviewRequired,
                      assessmentDto.CreatedAt,
                      assessmentDto.CreatedBy,
                      assessmentDto.IsDeleted

                );

            newAssessment.SetCompletionDate(assessmentDto.CompletionDate);
            newAssessment.SetComplianceScore(assessmentDto.ComplianceScore);
            newAssessment.SetUpdatedAt(assessmentDto.UpdatedAt);
            newAssessment.SetIsDeleted(false);

                _assessmentRepository.BeginTransaction();
                await _assessmentRepository.AddAsync(newAssessment);
                await InvalidateAssessmentCacheAsync(newAssessment.AssessmentId);
            _assessmentRepository.CommitTransaction();

                var mapper = new GetByIdAssessmentFromEntityMapper();
                var createdAssessmentDto = mapper.FromEntity(newAssessment);

                return Right<string, AssessmentDto>(createdAssessmentDto);
            }
            catch (Exception ex)
            {
                _assessmentRepository.RollbackTransaction();
                _logger.LogError(ex, "Error creating assessment");
                return Left<string, AssessmentDto>($"Error creating assessment: {ex.Message}");
            }
        }

        public async Task<Either<string, AssessmentDto>> UpdateAssessmentAsync(Guid assessmentId, AssessmentDto assessmentDto)
        {
            try
            {
                Guard.Against.NullOrEmpty(assessmentId, nameof(assessmentId));
                Guard.Against.Null(assessmentDto, nameof(assessmentDto));

                var validationResult = await _updateAssessmentValidator.ValidateAsync(assessmentDto);
                if (!validationResult.IsValid)
                {
                    var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                    return Left<string, AssessmentDto>($"Validation failed: {errors}");
                }

                _assessmentRepository.BeginTransaction();
                try
                {
                    Assessment? assessment = await _assessmentRepository.GetByIdAsync(assessmentId);

                    if (assessment == null)
                        return Left<string, AssessmentDto>($"Assessment with ID {assessmentDto.AssessmentId} not found.");

                    var updateResult = new Assessment(

                        assessmentId: assessment.AssessmentId,
                      assessmentDto.MerchantId,
                      assessmentDto.TenantId,
                      assessmentDto.AssessmentCode,
                      assessmentDto.AssessmentType,
                      assessmentDto.AssessmentPeriod,
                      assessmentDto.StartDate,
                      assessmentDto.EndDate,
                      assessmentDto.Rank,
                      assessmentDto.QSAReviewRequired,
                      assessmentDto.CreatedAt,
                      assessmentDto.CreatedBy,
                      assessmentDto.IsDeleted
                    );

                assessment.SetIsDeleted(assessmentDto.IsDeleted);

                    await _assessmentRepository.UpdateAsync(updateResult);
                    await InvalidateAssessmentCacheAsync(assessmentId);
                    _assessmentRepository.CommitTransaction();

                    var mapper = new GetByIdAssessmentFromEntityMapper();
                    var updatedAssessmentDto = mapper.FromEntity(updateResult);

                    return updatedAssessmentDto != null
                        ? Right<string, AssessmentDto>(updatedAssessmentDto)
                        : Left<string, AssessmentDto>("Error mapping updated assessment to DTO");
                }
                catch
                {
                    _assessmentRepository.RollbackTransaction();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error updating assessment {AssessmentId}", assessmentDto.AssessmentId);
                return Left<string, AssessmentDto>($"Error updating assessment: {ex.Message}");
            }
        }
    public class GetByIdAssessmentEntityDtoMapper : Mapper<GetByIdAssessmentRequest, GetByIdAssessmentResponse, AssessmentDto>
    {
        public   AssessmentDto? FromEntityDto(AssessmentEntityDto assessment)
        {
            Guard.Against.Null(assessment, nameof(assessment));
            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            string jsonAssessment = JsonSerializer.Serialize(assessment, settings);
            AssessmentDto? assessmentDto = JsonSerializer.Deserialize<AssessmentDto>(jsonAssessment, settings);
            return assessmentDto;
        }
    }

    public class GetByIdAssessmentFromEntityMapper : Mapper<GetByIdAssessmentRequest, GetByIdAssessmentResponse, AssessmentDto>
    {
        public   AssessmentDto? FromEntity(Assessment assessment)
        {
            Guard.Against.Null(assessment, nameof(assessment));
            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            string jsonAssessment = JsonSerializer.Serialize(assessment, settings);
            AssessmentDto? assessmentDto = JsonSerializer.Deserialize<AssessmentDto>(jsonAssessment, settings);
            return assessmentDto;
        }
    }

        public async Task<Either<string, AssetDto>> GetOneFullAssetByIdAsync(Guid assetId)
        {
            try
            {
            AssetByIdSpec spec = new(assetId);
                AssetEntityDto? asset = await _assetCacheRepository.FirstOrDefaultAsync(spec);
                GetByIdAssetEntityDtoMapper mapper = new();

                if (asset != null)
                {
                    AssetDto? assetDto = mapper.FromEntityDto(asset);
                    return assetDto != null
                        ? Right<string, AssetDto>(assetDto)
                        : Left<string, AssetDto>("Error mapping asset to DTO");
                }
                return Left<string, AssetDto>("Asset not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error retrieving asset {AssetId}", assetId);
                return Left<string, AssetDto>($"Error retrieving asset: {ex.Message}");
            }
        }

        public async Task<Either<string, List<AssetDto>>> SearchAssetsAsync(string searchTerm)
        {
            try
            {
                Guard.Against.NullOrWhiteSpace(searchTerm, nameof(searchTerm));
                var spec = new AssetSearchSpec(searchTerm);
                var assets = await _assetCacheRepository.ListAsync(spec);

                var settings = GJset.GetSystemTextJsonSettings();
                var assetDtos = assets
                    .Select(i => JsonSerializer.Deserialize<AssetDto>(JsonSerializer.Serialize(i, settings), settings))
                    .Where(dto => dto != null)
                    .ToList();

                return Right<string, List<AssetDto>>(assetDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error searching assets with term: {SearchTerm}", searchTerm);
                return Left<string, List<AssetDto>>($"Error searching assets: {ex.Message}");
            }
        }

        public async Task<Either<string, ListAssetResponse>> GetFilteredAssetsAsync(FilteredAssetRequest req)
        {
            try
            {
                Guard.Against.NegativeOrZero(req.PageNumber, nameof(req.PageNumber));
                Guard.Against.NegativeOrZero(req.PageSize, nameof(req.PageSize));

                List<PCIShieldLib.SharedKernel.Interfaces.Sort> convertedSortings = new();
                var errors = new List<string>();

            if (req.Sorting == null)
            {
                req.Sorting = new List<Sort>();
            }
            foreach (var sort1 in req.Sorting)
                {
                    var sort = (Sort)sort1;
                    if (string.IsNullOrWhiteSpace(sort.Field))
                    {
                        errors.Add("Sort field cannot be empty");
                        continue;
                    }
                    convertedSortings.Add(
                        new PCIShieldLib.SharedKernel.Interfaces.Sort { Field = sort.Field, Direction = sort.Direction }
                    );
                }

                if (errors.Any())
                    return Left<string, ListAssetResponse>(string.Join("; ", errors));

                var spec = new AssetAdvancedFilterSpec(req.PageNumber, req.PageSize, req.Filters, convertedSortings);
                var assets = await _assetCacheRepository.ListAsync(spec);

                if (!assets.Any())
                    return Right<string, ListAssetResponse>(new ListAssetResponse { Assets = null, Count = 0 });

                var settings = GJset.GetSystemTextJsonSettings();
                var assetDtos = assets
                    .Skip((req.PageNumber - 1) * req.PageSize)
                    .Take(req.PageSize)
                    .Select(i => JsonSerializer.Deserialize<AssetDto>(JsonSerializer.Serialize(i, settings), settings))
                    .Where(dto => dto != null)
                    .ToList();

                return Right<string, ListAssetResponse>(
                    new ListAssetResponse { Assets = assetDtos, Count = assets.Count }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error filtering assets");
                return Left<string, ListAssetResponse>($"Error filtering assets: {ex.Message}");
            }
        }

        public async Task<Either<string, List<AssetDto>>> GetAssetPagedList(int pageNumber, int pageSize)
        {
            try
            {
                var spec = new AssetListPagedSpec(pageNumber, pageSize);
                var assetEntityDtos = await _assetCacheRepository.ListAsync(spec);

                if (!assetEntityDtos.Any())
                    return Left<string, List<AssetDto>>("No assets found.");

                var settings = GJset.GetSystemTextJsonSettings();
                var assetDtos = assetEntityDtos
                    .Select(entityDto =>
                    {
                        var jsonAsset = JsonSerializer.Serialize(entityDto, settings);
                        return JsonSerializer.Deserialize<AssetDto>(jsonAsset, settings);
                    })
                    .Where(dto => dto != null)
                    .ToList();

                return assetDtos.Any()
                    ? Right<string, List<AssetDto>>(assetDtos)
                    : Left<string, List<AssetDto>>("Error mapping assets.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error retrieving asset list for page {PageNumber} with size {PageSize}",
                    pageNumber, pageSize);
                return Left<string, List<AssetDto>>($"Error retrieving asset list: {ex.Message}");
            }
        }

        public async Task<Either<string, bool>> DeleteAssetAsync(Guid assetId)
        {
            try
            {
                Guard.Against.NullOrEmpty(assetId, nameof(assetId));

                var asset = await _assetRepository.GetByIdAsync(assetId);

                if (asset == null)
                    return Left<string, bool>("Asset not found.");

                asset.SetIsDeleted(true);
                await _assetRepository.UpdateAsync(asset);
                await InvalidateAssetCacheAsync(assetId);

                return Right<string, bool>(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting asset");
                return Left<string, bool>($"Error deleting asset: {ex.Message}");
            }
        }

        public async Task<Either<string, AssetDto>> GetLastCreatedAssetAsync()
        {
            try
            {
                var spec = new AssetLastCreatedSpec();
                var asset = await _assetRepository.FirstOrDefaultAsync(spec);

                if (asset == null)
                    return Left<string, AssetDto>("No assets found");

                var mapper = new GetByIdAssetFromEntityMapper();
                var assetDto = mapper.FromEntity(asset);

                return assetDto != null
                    ? Right<string, AssetDto>(assetDto)
                    : Left<string, AssetDto>("Error mapping asset to DTO");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving last created asset");
                return Left<string, AssetDto>($"Error retrieving last created asset: {ex.Message}");
            }
        }

        public async Task<Either<string, AssetDto>> CreateAssetAsync(AssetDto assetDto)
        {
            try
            {
                Guard.Against.Null(assetDto, nameof(assetDto));

                var newAsset = new Asset(

                    assetId: Guid.CreateVersion7(),
                      assetDto.MerchantId,
                      assetDto.TenantId,
                      assetDto.AssetCode,
                      assetDto.AssetName,
                      assetDto.AssetType,
                      assetDto.IsInCDE,
                      assetDto.CreatedAt,
                      assetDto.CreatedBy,
                      assetDto.IsDeleted

                );

            newAsset.SetIPAddress(assetDto.IPAddress);
            newAsset.SetHostname(assetDto.Hostname);
            newAsset.SetNetworkZone(assetDto.NetworkZone);
            newAsset.SetLastScanDate(assetDto.LastScanDate);
            newAsset.SetUpdatedAt(assetDto.UpdatedAt);
            newAsset.SetIsDeleted(false);

                _assetRepository.BeginTransaction();
                await _assetRepository.AddAsync(newAsset);
                await InvalidateAssetCacheAsync(newAsset.AssetId);
            _assetRepository.CommitTransaction();

                var mapper = new GetByIdAssetFromEntityMapper();
                var createdAssetDto = mapper.FromEntity(newAsset);

                return Right<string, AssetDto>(createdAssetDto);
            }
            catch (Exception ex)
            {
                _assetRepository.RollbackTransaction();
                _logger.LogError(ex, "Error creating asset");
                return Left<string, AssetDto>($"Error creating asset: {ex.Message}");
            }
        }

        public async Task<Either<string, AssetDto>> UpdateAssetAsync(Guid assetId, AssetDto assetDto)
        {
            try
            {
                Guard.Against.NullOrEmpty(assetId, nameof(assetId));
                Guard.Against.Null(assetDto, nameof(assetDto));

                var validationResult = await _updateAssetValidator.ValidateAsync(assetDto);
                if (!validationResult.IsValid)
                {
                    var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                    return Left<string, AssetDto>($"Validation failed: {errors}");
                }

                _assetRepository.BeginTransaction();
                try
                {
                    Asset? asset = await _assetRepository.GetByIdAsync(assetId);

                    if (asset == null)
                        return Left<string, AssetDto>($"Asset with ID {assetDto.AssetId} not found.");

                    var updateResult = new Asset(

                        assetId: asset.AssetId,
                      assetDto.MerchantId,
                      assetDto.TenantId,
                      assetDto.AssetCode,
                      assetDto.AssetName,
                      assetDto.AssetType,
                      assetDto.IsInCDE,
                      assetDto.CreatedAt,
                      assetDto.CreatedBy,
                      assetDto.IsDeleted
                    );

                asset.SetIsDeleted(assetDto.IsDeleted);

                    await _assetRepository.UpdateAsync(updateResult);
                    await InvalidateAssetCacheAsync(assetId);
                    _assetRepository.CommitTransaction();

                    var mapper = new GetByIdAssetFromEntityMapper();
                    var updatedAssetDto = mapper.FromEntity(updateResult);

                    return updatedAssetDto != null
                        ? Right<string, AssetDto>(updatedAssetDto)
                        : Left<string, AssetDto>("Error mapping updated asset to DTO");
                }
                catch
                {
                    _assetRepository.RollbackTransaction();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error updating asset {AssetId}", assetDto.AssetId);
                return Left<string, AssetDto>($"Error updating asset: {ex.Message}");
            }
        }
    public class GetByIdAssetEntityDtoMapper : Mapper<GetByIdAssetRequest, GetByIdAssetResponse, AssetDto>
    {
        public   AssetDto? FromEntityDto(AssetEntityDto asset)
        {
            Guard.Against.Null(asset, nameof(asset));
            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            string jsonAsset = JsonSerializer.Serialize(asset, settings);
            AssetDto? assetDto = JsonSerializer.Deserialize<AssetDto>(jsonAsset, settings);
            return assetDto;
        }
    }

    public class GetByIdAssetFromEntityMapper : Mapper<GetByIdAssetRequest, GetByIdAssetResponse, AssetDto>
    {
        public   AssetDto? FromEntity(Asset asset)
        {
            Guard.Against.Null(asset, nameof(asset));
            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            string jsonAsset = JsonSerializer.Serialize(asset, settings);
            AssetDto? assetDto = JsonSerializer.Deserialize<AssetDto>(jsonAsset, settings);
            return assetDto;
        }
    }

        public async Task<Either<string, CompensatingControlDto>> GetOneFullCompensatingControlByIdAsync(Guid compensatingControlId)
        {
            try
            {
            CompensatingControlByIdSpec spec = new(compensatingControlId);
                CompensatingControlEntityDto? compensatingControl = await _compensatingControlCacheRepository.FirstOrDefaultAsync(spec);
                GetByIdCompensatingControlEntityDtoMapper mapper = new();

                if (compensatingControl != null)
                {
                    CompensatingControlDto? compensatingControlDto = mapper.FromEntityDto(compensatingControl);
                    return compensatingControlDto != null
                        ? Right<string, CompensatingControlDto>(compensatingControlDto)
                        : Left<string, CompensatingControlDto>("Error mapping compensatingControl to DTO");
                }
                return Left<string, CompensatingControlDto>("CompensatingControl not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error retrieving compensatingControl {CompensatingControlId}", compensatingControlId);
                return Left<string, CompensatingControlDto>($"Error retrieving compensatingControl: {ex.Message}");
            }
        }

        public async Task<Either<string, List<CompensatingControlDto>>> SearchCompensatingControlsAsync(string searchTerm)
        {
            try
            {
                Guard.Against.NullOrWhiteSpace(searchTerm, nameof(searchTerm));
                var spec = new CompensatingControlSearchSpec(searchTerm);
                var compensatingControls = await _compensatingControlCacheRepository.ListAsync(spec);

                var settings = GJset.GetSystemTextJsonSettings();
                var compensatingControlDtos = compensatingControls
                    .Select(i => JsonSerializer.Deserialize<CompensatingControlDto>(JsonSerializer.Serialize(i, settings), settings))
                    .Where(dto => dto != null)
                    .ToList();

                return Right<string, List<CompensatingControlDto>>(compensatingControlDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error searching compensatingControls with term: {SearchTerm}", searchTerm);
                return Left<string, List<CompensatingControlDto>>($"Error searching compensatingControls: {ex.Message}");
            }
        }

        public async Task<Either<string, ListCompensatingControlResponse>> GetFilteredCompensatingControlsAsync(FilteredCompensatingControlRequest req)
        {
            try
            {
                Guard.Against.NegativeOrZero(req.PageNumber, nameof(req.PageNumber));
                Guard.Against.NegativeOrZero(req.PageSize, nameof(req.PageSize));

                List<PCIShieldLib.SharedKernel.Interfaces.Sort> convertedSortings = new();
                var errors = new List<string>();

            if (req.Sorting == null)
            {
                req.Sorting = new List<Sort>();
            }
            foreach (var sort1 in req.Sorting)
            {
                    var sort = (Sort)sort1;
                    if (string.IsNullOrWhiteSpace(sort.Field))
                    {
                        errors.Add("Sort field cannot be empty");
                        continue;
                    }
                    convertedSortings.Add(
                        new PCIShieldLib.SharedKernel.Interfaces.Sort { Field = sort.Field, Direction = sort.Direction }
                    );
                }

                if (errors.Any())
                    return Left<string, ListCompensatingControlResponse>(string.Join("; ", errors));

                var spec = new CompensatingControlAdvancedFilterSpec(req.PageNumber, req.PageSize, req.Filters, convertedSortings);
                var compensatingControls = await _compensatingControlCacheRepository.ListAsync(spec);

                if (!compensatingControls.Any())
                    return Right<string, ListCompensatingControlResponse>(new ListCompensatingControlResponse { CompensatingControls = null, Count = 0 });

                var settings = GJset.GetSystemTextJsonSettings();
                var compensatingControlDtos = compensatingControls
                    .Skip((req.PageNumber - 1) * req.PageSize)
                    .Take(req.PageSize)
                    .Select(i => JsonSerializer.Deserialize<CompensatingControlDto>(JsonSerializer.Serialize(i, settings), settings))
                    .Where(dto => dto != null)
                    .ToList();

                return Right<string, ListCompensatingControlResponse>(
                    new ListCompensatingControlResponse { CompensatingControls = compensatingControlDtos, Count = compensatingControls.Count }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error filtering compensatingControls");
                return Left<string, ListCompensatingControlResponse>($"Error filtering compensatingControls: {ex.Message}");
            }
        }

        public async Task<Either<string, List<CompensatingControlDto>>> GetCompensatingControlPagedList(int pageNumber, int pageSize)
        {
            try
            {
                var spec = new CompensatingControlListPagedSpec(pageNumber, pageSize);
                var compensatingControlEntityDtos = await _compensatingControlCacheRepository.ListAsync(spec);

                if (!compensatingControlEntityDtos.Any())
                    return Left<string, List<CompensatingControlDto>>("No compensatingControls found.");

                var settings = GJset.GetSystemTextJsonSettings();
                var compensatingControlDtos = compensatingControlEntityDtos
                    .Select(entityDto =>
                    {
                        var jsonCompensatingControl = JsonSerializer.Serialize(entityDto, settings);
                        return JsonSerializer.Deserialize<CompensatingControlDto>(jsonCompensatingControl, settings);
                    })
                    .Where(dto => dto != null)
                    .ToList();

                return compensatingControlDtos.Any()
                    ? Right<string, List<CompensatingControlDto>>(compensatingControlDtos)
                    : Left<string, List<CompensatingControlDto>>("Error mapping compensatingControls.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error retrieving compensatingControl list for page {PageNumber} with size {PageSize}",
                    pageNumber, pageSize);
                return Left<string, List<CompensatingControlDto>>($"Error retrieving compensatingControl list: {ex.Message}");
            }
        }

        public async Task<Either<string, bool>> DeleteCompensatingControlAsync(Guid compensatingControlId)
        {
            try
            {
                Guard.Against.NullOrEmpty(compensatingControlId, nameof(compensatingControlId));

                var compensatingControl = await _compensatingControlRepository.GetByIdAsync(compensatingControlId);

                if (compensatingControl == null)
                    return Left<string, bool>("CompensatingControl not found.");

                compensatingControl.SetIsDeleted(true);
                await _compensatingControlRepository.UpdateAsync(compensatingControl);
                await InvalidateCompensatingControlCacheAsync(compensatingControlId);

                return Right<string, bool>(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting compensatingControl");
                return Left<string, bool>($"Error deleting compensatingControl: {ex.Message}");
            }
        }

        public async Task<Either<string, CompensatingControlDto>> GetLastCreatedCompensatingControlAsync()
        {
            try
            {
                var spec = new CompensatingControlLastCreatedSpec();
                var compensatingControl = await _compensatingControlRepository.FirstOrDefaultAsync(spec);

                if (compensatingControl == null)
                    return Left<string, CompensatingControlDto>("No compensatingControls found");

                var mapper = new GetByIdCompensatingControlFromEntityMapper();
                var compensatingControlDto = mapper.FromEntity(compensatingControl);

                return compensatingControlDto != null
                    ? Right<string, CompensatingControlDto>(compensatingControlDto)
                    : Left<string, CompensatingControlDto>("Error mapping compensatingControl to DTO");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving last created compensatingControl");
                return Left<string, CompensatingControlDto>($"Error retrieving last created compensatingControl: {ex.Message}");
            }
        }

        public async Task<Either<string, CompensatingControlDto>> CreateCompensatingControlAsync(CompensatingControlDto compensatingControlDto)
        {
            try
            {
                Guard.Against.Null(compensatingControlDto, nameof(compensatingControlDto));

                var newCompensatingControl = new CompensatingControl(

                    compensatingControlId: Guid.CreateVersion7(),
                      compensatingControlDto.ControlId,
                      compensatingControlDto.MerchantId,
                      compensatingControlDto.TenantId,
                      compensatingControlDto.Justification,
                      compensatingControlDto.ImplementationDetails,
                      compensatingControlDto.ExpiryDate,
                      compensatingControlDto.Rank,
                      compensatingControlDto.CreatedAt,
                      compensatingControlDto.CreatedBy,
                      compensatingControlDto.IsDeleted

                );

            newCompensatingControl.SetApprovedBy(compensatingControlDto.ApprovedBy);
            newCompensatingControl.SetApprovalDate(compensatingControlDto.ApprovalDate);
            newCompensatingControl.SetUpdatedAt(compensatingControlDto.UpdatedAt);
            newCompensatingControl.SetIsDeleted(false);

                _compensatingControlRepository.BeginTransaction();
                await _compensatingControlRepository.AddAsync(newCompensatingControl);
                await InvalidateCompensatingControlCacheAsync(newCompensatingControl.CompensatingControlId);
            _compensatingControlRepository.CommitTransaction();

                var mapper = new GetByIdCompensatingControlFromEntityMapper();
                var createdCompensatingControlDto = mapper.FromEntity(newCompensatingControl);

                return Right<string, CompensatingControlDto>(createdCompensatingControlDto);
            }
            catch (Exception ex)
            {
                _compensatingControlRepository.RollbackTransaction();
                _logger.LogError(ex, "Error creating compensatingControl");
                return Left<string, CompensatingControlDto>($"Error creating compensatingControl: {ex.Message}");
            }
        }

        public async Task<Either<string, CompensatingControlDto>> UpdateCompensatingControlAsync(Guid compensatingControlId, CompensatingControlDto compensatingControlDto)
        {
            try
            {
                Guard.Against.NullOrEmpty(compensatingControlId, nameof(compensatingControlId));
                Guard.Against.Null(compensatingControlDto, nameof(compensatingControlDto));

                var validationResult = await _updateCompensatingControlValidator.ValidateAsync(compensatingControlDto);
                if (!validationResult.IsValid)
                {
                    var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                    return Left<string, CompensatingControlDto>($"Validation failed: {errors}");
                }

                _compensatingControlRepository.BeginTransaction();
                try
                {
                    CompensatingControl? compensatingControl = await _compensatingControlRepository.GetByIdAsync(compensatingControlId);

                    if (compensatingControl == null)
                        return Left<string, CompensatingControlDto>($"CompensatingControl with ID {compensatingControlDto.CompensatingControlId} not found.");

                    var updateResult = new CompensatingControl(

                        compensatingControlId: compensatingControl.CompensatingControlId,
                      compensatingControlDto.ControlId,
                      compensatingControlDto.MerchantId,
                      compensatingControlDto.TenantId,
                      compensatingControlDto.Justification,
                      compensatingControlDto.ImplementationDetails,
                      compensatingControlDto.ExpiryDate,
                      compensatingControlDto.Rank,
                      compensatingControlDto.CreatedAt,
                      compensatingControlDto.CreatedBy,
                      compensatingControlDto.IsDeleted
                    );

                compensatingControl.SetIsDeleted(compensatingControlDto.IsDeleted);

                    await _compensatingControlRepository.UpdateAsync(updateResult);
                    await InvalidateCompensatingControlCacheAsync(compensatingControlId);
                    _compensatingControlRepository.CommitTransaction();

                    var mapper = new GetByIdCompensatingControlFromEntityMapper();
                    var updatedCompensatingControlDto = mapper.FromEntity(updateResult);

                    return updatedCompensatingControlDto != null
                        ? Right<string, CompensatingControlDto>(updatedCompensatingControlDto)
                        : Left<string, CompensatingControlDto>("Error mapping updated compensatingControl to DTO");
                }
                catch
                {
                    _compensatingControlRepository.RollbackTransaction();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error updating compensatingControl {CompensatingControlId}", compensatingControlDto.CompensatingControlId);
                return Left<string, CompensatingControlDto>($"Error updating compensatingControl: {ex.Message}");
            }
        }
    public class GetByIdCompensatingControlEntityDtoMapper : Mapper<GetByIdCompensatingControlRequest, GetByIdCompensatingControlResponse, CompensatingControlDto>
    {
        public   CompensatingControlDto? FromEntityDto(CompensatingControlEntityDto compensatingControl)
        {
            Guard.Against.Null(compensatingControl, nameof(compensatingControl));
            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            string jsonCompensatingControl = JsonSerializer.Serialize(compensatingControl, settings);
            CompensatingControlDto? compensatingControlDto = JsonSerializer.Deserialize<CompensatingControlDto>(jsonCompensatingControl, settings);
            return compensatingControlDto;
        }
    }

    public class GetByIdCompensatingControlFromEntityMapper : Mapper<GetByIdCompensatingControlRequest, GetByIdCompensatingControlResponse, CompensatingControlDto>
    {
        public   CompensatingControlDto? FromEntity(CompensatingControl compensatingControl)
        {
            Guard.Against.Null(compensatingControl, nameof(compensatingControl));
            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            string jsonCompensatingControl = JsonSerializer.Serialize(compensatingControl, settings);
            CompensatingControlDto? compensatingControlDto = JsonSerializer.Deserialize<CompensatingControlDto>(jsonCompensatingControl, settings);
            return compensatingControlDto;
        }
    }

        public async Task<Either<string, ComplianceOfficerDto>> GetOneFullComplianceOfficerByIdAsync(Guid complianceOfficerId)
        {
            try
            {
            ComplianceOfficerByIdSpec spec = new(complianceOfficerId);
                ComplianceOfficerEntityDto? complianceOfficer = await _complianceOfficerCacheRepository.FirstOrDefaultAsync(spec);
                GetByIdComplianceOfficerEntityDtoMapper mapper = new();

                if (complianceOfficer != null)
                {
                    ComplianceOfficerDto? complianceOfficerDto = mapper.FromEntityDto(complianceOfficer);
                    return complianceOfficerDto != null
                        ? Right<string, ComplianceOfficerDto>(complianceOfficerDto)
                        : Left<string, ComplianceOfficerDto>("Error mapping complianceOfficer to DTO");
                }
                return Left<string, ComplianceOfficerDto>("ComplianceOfficer not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error retrieving complianceOfficer {ComplianceOfficerId}", complianceOfficerId);
                return Left<string, ComplianceOfficerDto>($"Error retrieving complianceOfficer: {ex.Message}");
            }
        }

        public async Task<Either<string, List<ComplianceOfficerDto>>> SearchComplianceOfficersAsync(string searchTerm)
        {
            try
            {
                Guard.Against.NullOrWhiteSpace(searchTerm, nameof(searchTerm));
                var spec = new ComplianceOfficerSearchSpec(searchTerm);
                var complianceOfficers = await _complianceOfficerCacheRepository.ListAsync(spec);

                var settings = GJset.GetSystemTextJsonSettings();
                var complianceOfficerDtos = complianceOfficers
                    .Select(i => JsonSerializer.Deserialize<ComplianceOfficerDto>(JsonSerializer.Serialize(i, settings), settings))
                    .Where(dto => dto != null)
                    .ToList();

                return Right<string, List<ComplianceOfficerDto>>(complianceOfficerDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error searching complianceOfficers with term: {SearchTerm}", searchTerm);
                return Left<string, List<ComplianceOfficerDto>>($"Error searching complianceOfficers: {ex.Message}");
            }
        }

        public async Task<Either<string, ListComplianceOfficerResponse>> GetFilteredComplianceOfficersAsync(FilteredComplianceOfficerRequest req)
        {
            try
            {
                Guard.Against.NegativeOrZero(req.PageNumber, nameof(req.PageNumber));
                Guard.Against.NegativeOrZero(req.PageSize, nameof(req.PageSize));

                List<PCIShieldLib.SharedKernel.Interfaces.Sort> convertedSortings = new();
                var errors = new List<string>();

            if (req.Sorting == null)
            {
                req.Sorting = new List<Sort>();
            }
            foreach (var sort1 in req.Sorting)
            {
                    var sort = (Sort)sort1;
                    if (string.IsNullOrWhiteSpace(sort.Field))
                    {
                        errors.Add("Sort field cannot be empty");
                        continue;
                    }
                    convertedSortings.Add(
                        new PCIShieldLib.SharedKernel.Interfaces.Sort { Field = sort.Field, Direction = sort.Direction }
                    );
                }

                if (errors.Any())
                    return Left<string, ListComplianceOfficerResponse>(string.Join("; ", errors));

                var spec = new ComplianceOfficerAdvancedFilterSpec(req.PageNumber, req.PageSize, req.Filters, convertedSortings);
                var complianceOfficers = await _complianceOfficerCacheRepository.ListAsync(spec);

                if (!complianceOfficers.Any())
                    return Right<string, ListComplianceOfficerResponse>(new ListComplianceOfficerResponse { ComplianceOfficers = null, Count = 0 });

                var settings = GJset.GetSystemTextJsonSettings();
                var complianceOfficerDtos = complianceOfficers
                    .Skip((req.PageNumber - 1) * req.PageSize)
                    .Take(req.PageSize)
                    .Select(i => JsonSerializer.Deserialize<ComplianceOfficerDto>(JsonSerializer.Serialize(i, settings), settings))
                    .Where(dto => dto != null)
                    .ToList();

                return Right<string, ListComplianceOfficerResponse>(
                    new ListComplianceOfficerResponse { ComplianceOfficers = complianceOfficerDtos, Count = complianceOfficers.Count }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error filtering complianceOfficers");
                return Left<string, ListComplianceOfficerResponse>($"Error filtering complianceOfficers: {ex.Message}");
            }
        }

        public async Task<Either<string, List<ComplianceOfficerDto>>> GetComplianceOfficerPagedList(int pageNumber, int pageSize)
        {
            try
            {
                var spec = new ComplianceOfficerListPagedSpec(pageNumber, pageSize);
                var complianceOfficerEntityDtos = await _complianceOfficerCacheRepository.ListAsync(spec);

                if (!complianceOfficerEntityDtos.Any())
                    return Left<string, List<ComplianceOfficerDto>>("No complianceOfficers found.");

                var settings = GJset.GetSystemTextJsonSettings();
                var complianceOfficerDtos = complianceOfficerEntityDtos
                    .Select(entityDto =>
                    {
                        var jsonComplianceOfficer = JsonSerializer.Serialize(entityDto, settings);
                        return JsonSerializer.Deserialize<ComplianceOfficerDto>(jsonComplianceOfficer, settings);
                    })
                    .Where(dto => dto != null)
                    .ToList();

                return complianceOfficerDtos.Any()
                    ? Right<string, List<ComplianceOfficerDto>>(complianceOfficerDtos)
                    : Left<string, List<ComplianceOfficerDto>>("Error mapping complianceOfficers.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error retrieving complianceOfficer list for page {PageNumber} with size {PageSize}",
                    pageNumber, pageSize);
                return Left<string, List<ComplianceOfficerDto>>($"Error retrieving complianceOfficer list: {ex.Message}");
            }
        }

        public async Task<Either<string, bool>> DeleteComplianceOfficerAsync(Guid complianceOfficerId)
        {
            try
            {
                Guard.Against.NullOrEmpty(complianceOfficerId, nameof(complianceOfficerId));

                var complianceOfficer = await _complianceOfficerRepository.GetByIdAsync(complianceOfficerId);

                if (complianceOfficer == null)
                    return Left<string, bool>("ComplianceOfficer not found.");

                complianceOfficer.SetIsDeleted(true);
                await _complianceOfficerRepository.UpdateAsync(complianceOfficer);
                await InvalidateComplianceOfficerCacheAsync(complianceOfficerId);

                return Right<string, bool>(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting complianceOfficer");
                return Left<string, bool>($"Error deleting complianceOfficer: {ex.Message}");
            }
        }

        public async Task<Either<string, ComplianceOfficerDto>> GetLastCreatedComplianceOfficerAsync()
        {
            try
            {
                var spec = new ComplianceOfficerLastCreatedSpec();
                var complianceOfficer = await _complianceOfficerRepository.FirstOrDefaultAsync(spec);

                if (complianceOfficer == null)
                    return Left<string, ComplianceOfficerDto>("No complianceOfficers found");

                var mapper = new GetByIdComplianceOfficerFromEntityMapper();
                var complianceOfficerDto = mapper.FromEntity(complianceOfficer);

                return complianceOfficerDto != null
                    ? Right<string, ComplianceOfficerDto>(complianceOfficerDto)
                    : Left<string, ComplianceOfficerDto>("Error mapping complianceOfficer to DTO");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving last created complianceOfficer");
                return Left<string, ComplianceOfficerDto>($"Error retrieving last created complianceOfficer: {ex.Message}");
            }
        }

        public async Task<Either<string, ComplianceOfficerDto>> CreateComplianceOfficerAsync(ComplianceOfficerDto complianceOfficerDto)
        {
            try
            {
                Guard.Against.Null(complianceOfficerDto, nameof(complianceOfficerDto));

                var newComplianceOfficer = new ComplianceOfficer(

                    complianceOfficerId: Guid.CreateVersion7(),
                      complianceOfficerDto.MerchantId,
                      complianceOfficerDto.TenantId,
                      complianceOfficerDto.OfficerCode,
                      complianceOfficerDto.FirstName,
                      complianceOfficerDto.LastName,
                      complianceOfficerDto.Email,
                      complianceOfficerDto.IsActive,
                      complianceOfficerDto.CreatedAt,
                      complianceOfficerDto.CreatedBy,
                      complianceOfficerDto.IsDeleted

                );

            newComplianceOfficer.SetPhone(complianceOfficerDto.Phone);
            newComplianceOfficer.SetCertificationLevel(complianceOfficerDto.CertificationLevel);
            newComplianceOfficer.SetUpdatedAt(complianceOfficerDto.UpdatedAt);
            newComplianceOfficer.SetIsDeleted(false);

                _complianceOfficerRepository.BeginTransaction();
                await _complianceOfficerRepository.AddAsync(newComplianceOfficer);
                await InvalidateComplianceOfficerCacheAsync(newComplianceOfficer.ComplianceOfficerId);
            _complianceOfficerRepository.CommitTransaction();

                var mapper = new GetByIdComplianceOfficerFromEntityMapper();
                var createdComplianceOfficerDto = mapper.FromEntity(newComplianceOfficer);

                return Right<string, ComplianceOfficerDto>(createdComplianceOfficerDto);
            }
            catch (Exception ex)
            {
                _complianceOfficerRepository.RollbackTransaction();
                _logger.LogError(ex, "Error creating complianceOfficer");
                return Left<string, ComplianceOfficerDto>($"Error creating complianceOfficer: {ex.Message}");
            }
        }

        public async Task<Either<string, ComplianceOfficerDto>> UpdateComplianceOfficerAsync(Guid complianceOfficerId, ComplianceOfficerDto complianceOfficerDto)
        {
            try
            {
                Guard.Against.NullOrEmpty(complianceOfficerId, nameof(complianceOfficerId));
                Guard.Against.Null(complianceOfficerDto, nameof(complianceOfficerDto));

                var validationResult = await _updateComplianceOfficerValidator.ValidateAsync(complianceOfficerDto);
                if (!validationResult.IsValid)
                {
                    var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                    return Left<string, ComplianceOfficerDto>($"Validation failed: {errors}");
                }

                _complianceOfficerRepository.BeginTransaction();
                try
                {
                    ComplianceOfficer? complianceOfficer = await _complianceOfficerRepository.GetByIdAsync(complianceOfficerId);

                    if (complianceOfficer == null)
                        return Left<string, ComplianceOfficerDto>($"ComplianceOfficer with ID {complianceOfficerDto.ComplianceOfficerId} not found.");

                    var updateResult = new ComplianceOfficer(

                        complianceOfficerId: complianceOfficer.ComplianceOfficerId,
                      complianceOfficerDto.MerchantId,
                      complianceOfficerDto.TenantId,
                      complianceOfficerDto.OfficerCode,
                      complianceOfficerDto.FirstName,
                      complianceOfficerDto.LastName,
                      complianceOfficerDto.Email,
                      complianceOfficerDto.IsActive,
                      complianceOfficerDto.CreatedAt,
                      complianceOfficerDto.CreatedBy,
                      complianceOfficerDto.IsDeleted
                    );

                complianceOfficer.SetIsDeleted(complianceOfficerDto.IsDeleted);

                    await _complianceOfficerRepository.UpdateAsync(updateResult);
                    await InvalidateComplianceOfficerCacheAsync(complianceOfficerId);
                    _complianceOfficerRepository.CommitTransaction();

                    var mapper = new GetByIdComplianceOfficerFromEntityMapper();
                    var updatedComplianceOfficerDto = mapper.FromEntity(updateResult);

                    return updatedComplianceOfficerDto != null
                        ? Right<string, ComplianceOfficerDto>(updatedComplianceOfficerDto)
                        : Left<string, ComplianceOfficerDto>("Error mapping updated complianceOfficer to DTO");
                }
                catch
                {
                    _complianceOfficerRepository.RollbackTransaction();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error updating complianceOfficer {ComplianceOfficerId}", complianceOfficerDto.ComplianceOfficerId);
                return Left<string, ComplianceOfficerDto>($"Error updating complianceOfficer: {ex.Message}");
            }
        }
    public class GetByIdComplianceOfficerEntityDtoMapper : Mapper<GetByIdComplianceOfficerRequest, GetByIdComplianceOfficerResponse, ComplianceOfficerDto>
    {
        public   ComplianceOfficerDto? FromEntityDto(ComplianceOfficerEntityDto complianceOfficer)
        {
            Guard.Against.Null(complianceOfficer, nameof(complianceOfficer));
            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            string jsonComplianceOfficer = JsonSerializer.Serialize(complianceOfficer, settings);
            ComplianceOfficerDto? complianceOfficerDto = JsonSerializer.Deserialize<ComplianceOfficerDto>(jsonComplianceOfficer, settings);
            return complianceOfficerDto;
        }
    }

    public class GetByIdComplianceOfficerFromEntityMapper : Mapper<GetByIdComplianceOfficerRequest, GetByIdComplianceOfficerResponse, ComplianceOfficerDto>
    {
        public   ComplianceOfficerDto? FromEntity(ComplianceOfficer complianceOfficer)
        {
            Guard.Against.Null(complianceOfficer, nameof(complianceOfficer));
            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            string jsonComplianceOfficer = JsonSerializer.Serialize(complianceOfficer, settings);
            ComplianceOfficerDto? complianceOfficerDto = JsonSerializer.Deserialize<ComplianceOfficerDto>(jsonComplianceOfficer, settings);
            return complianceOfficerDto;
        }
    }

        public async Task<Either<string, CryptographicInventoryDto>> GetOneFullCryptographicInventoryByIdAsync(Guid cryptographicInventoryId)
        {
            try
            {
            CryptographicInventoryByIdSpec spec = new(cryptographicInventoryId);
                CryptographicInventoryEntityDto? cryptographicInventory = await _cryptographicInventoryCacheRepository.FirstOrDefaultAsync(spec);
                GetByIdCryptographicInventoryEntityDtoMapper mapper = new();

                if (cryptographicInventory != null)
                {
                    CryptographicInventoryDto? cryptographicInventoryDto = mapper.FromEntityDto(cryptographicInventory);
                    return cryptographicInventoryDto != null
                        ? Right<string, CryptographicInventoryDto>(cryptographicInventoryDto)
                        : Left<string, CryptographicInventoryDto>("Error mapping cryptographicInventory to DTO");
                }
                return Left<string, CryptographicInventoryDto>("CryptographicInventory not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error retrieving cryptographicInventory {CryptographicInventoryId}", cryptographicInventoryId);
                return Left<string, CryptographicInventoryDto>($"Error retrieving cryptographicInventory: {ex.Message}");
            }
        }

        public async Task<Either<string, List<CryptographicInventoryDto>>> SearchCryptographicInventoriesAsync(string searchTerm)
        {
            try
            {
                Guard.Against.NullOrWhiteSpace(searchTerm, nameof(searchTerm));
                var spec = new CryptographicInventorySearchSpec(searchTerm);
                var cryptographicInventories = await _cryptographicInventoryCacheRepository.ListAsync(spec);

                var settings = GJset.GetSystemTextJsonSettings();
                var cryptographicInventoryDtos = cryptographicInventories
                    .Select(i => JsonSerializer.Deserialize<CryptographicInventoryDto>(JsonSerializer.Serialize(i, settings), settings))
                    .Where(dto => dto != null)
                    .ToList();

                return Right<string, List<CryptographicInventoryDto>>(cryptographicInventoryDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error searching cryptographicInventories with term: {SearchTerm}", searchTerm);
                return Left<string, List<CryptographicInventoryDto>>($"Error searching cryptographicInventories: {ex.Message}");
            }
        }

        public async Task<Either<string, ListCryptographicInventoryResponse>> GetFilteredCryptographicInventoriesAsync(FilteredCryptographicInventoryRequest req)
        {
            try
            {
                Guard.Against.NegativeOrZero(req.PageNumber, nameof(req.PageNumber));
                Guard.Against.NegativeOrZero(req.PageSize, nameof(req.PageSize));

                List<PCIShieldLib.SharedKernel.Interfaces.Sort> convertedSortings = new();
                var errors = new List<string>();

            if (req.Sorting == null)
            {
                req.Sorting = new List<Sort>();
            }
            foreach (var sort1 in req.Sorting)
            {
                    var sort = (Sort)sort1;
                    if (string.IsNullOrWhiteSpace(sort.Field))
                    {
                        errors.Add("Sort field cannot be empty");
                        continue;
                    }
                    convertedSortings.Add(
                        new PCIShieldLib.SharedKernel.Interfaces.Sort { Field = sort.Field, Direction = sort.Direction }
                    );
                }

                if (errors.Any())
                    return Left<string, ListCryptographicInventoryResponse>(string.Join("; ", errors));

                var spec = new CryptographicInventoryAdvancedFilterSpec(req.PageNumber, req.PageSize, req.Filters, convertedSortings);
                var cryptographicInventories = await _cryptographicInventoryCacheRepository.ListAsync(spec);

                if (!cryptographicInventories.Any())
                    return Right<string, ListCryptographicInventoryResponse>(new ListCryptographicInventoryResponse { CryptographicInventories = null, Count = 0 });

                var settings = GJset.GetSystemTextJsonSettings();
                var cryptographicInventoryDtos = cryptographicInventories
                    .Skip((req.PageNumber - 1) * req.PageSize)
                    .Take(req.PageSize)
                    .Select(i => JsonSerializer.Deserialize<CryptographicInventoryDto>(JsonSerializer.Serialize(i, settings), settings))
                    .Where(dto => dto != null)
                    .ToList();

                return Right<string, ListCryptographicInventoryResponse>(
                    new ListCryptographicInventoryResponse { CryptographicInventories = cryptographicInventoryDtos, Count = cryptographicInventories.Count }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error filtering cryptographicInventories");
                return Left<string, ListCryptographicInventoryResponse>($"Error filtering cryptographicInventories: {ex.Message}");
            }
        }

        public async Task<Either<string, List<CryptographicInventoryDto>>> GetCryptographicInventoryPagedList(int pageNumber, int pageSize)
        {
            try
            {
                var spec = new CryptographicInventoryListPagedSpec(pageNumber, pageSize);
                var cryptographicInventoryEntityDtos = await _cryptographicInventoryCacheRepository.ListAsync(spec);

                if (!cryptographicInventoryEntityDtos.Any())
                    return Left<string, List<CryptographicInventoryDto>>("No cryptographicInventories found.");

                var settings = GJset.GetSystemTextJsonSettings();
                var cryptographicInventoryDtos = cryptographicInventoryEntityDtos
                    .Select(entityDto =>
                    {
                        var jsonCryptographicInventory = JsonSerializer.Serialize(entityDto, settings);
                        return JsonSerializer.Deserialize<CryptographicInventoryDto>(jsonCryptographicInventory, settings);
                    })
                    .Where(dto => dto != null)
                    .ToList();

                return cryptographicInventoryDtos.Any()
                    ? Right<string, List<CryptographicInventoryDto>>(cryptographicInventoryDtos)
                    : Left<string, List<CryptographicInventoryDto>>("Error mapping cryptographicInventories.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error retrieving cryptographicInventory list for page {PageNumber} with size {PageSize}",
                    pageNumber, pageSize);
                return Left<string, List<CryptographicInventoryDto>>($"Error retrieving cryptographicInventory list: {ex.Message}");
            }
        }

        public async Task<Either<string, bool>> DeleteCryptographicInventoryAsync(Guid cryptographicInventoryId)
        {
            try
            {
                Guard.Against.NullOrEmpty(cryptographicInventoryId, nameof(cryptographicInventoryId));

                var cryptographicInventory = await _cryptographicInventoryRepository.GetByIdAsync(cryptographicInventoryId);

                if (cryptographicInventory == null)
                    return Left<string, bool>("CryptographicInventory not found.");

                cryptographicInventory.SetIsDeleted(true);
                await _cryptographicInventoryRepository.UpdateAsync(cryptographicInventory);
                await InvalidateCryptographicInventoryCacheAsync(cryptographicInventoryId);

                return Right<string, bool>(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting cryptographicInventory");
                return Left<string, bool>($"Error deleting cryptographicInventory: {ex.Message}");
            }
        }

        public async Task<Either<string, CryptographicInventoryDto>> GetLastCreatedCryptographicInventoryAsync()
        {
            try
            {
                var spec = new CryptographicInventoryLastCreatedSpec();
                var cryptographicInventory = await _cryptographicInventoryRepository.FirstOrDefaultAsync(spec);

                if (cryptographicInventory == null)
                    return Left<string, CryptographicInventoryDto>("No cryptographicInventories found");

                var mapper = new GetByIdCryptographicInventoryFromEntityMapper();
                var cryptographicInventoryDto = mapper.FromEntity(cryptographicInventory);

                return cryptographicInventoryDto != null
                    ? Right<string, CryptographicInventoryDto>(cryptographicInventoryDto)
                    : Left<string, CryptographicInventoryDto>("Error mapping cryptographicInventory to DTO");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving last created cryptographicInventory");
                return Left<string, CryptographicInventoryDto>($"Error retrieving last created cryptographicInventory: {ex.Message}");
            }
        }

        public async Task<Either<string, CryptographicInventoryDto>> CreateCryptographicInventoryAsync(CryptographicInventoryDto cryptographicInventoryDto)
        {
            try
            {
                Guard.Against.Null(cryptographicInventoryDto, nameof(cryptographicInventoryDto));

                var newCryptographicInventory = new CryptographicInventory(

                    cryptographicInventoryId: Guid.CreateVersion7(),
                      cryptographicInventoryDto.MerchantId,
                      cryptographicInventoryDto.TenantId,
                      cryptographicInventoryDto.KeyName,
                      cryptographicInventoryDto.KeyType,
                      cryptographicInventoryDto.Algorithm,
                      cryptographicInventoryDto.KeyLength,
                      cryptographicInventoryDto.KeyLocation,
                      cryptographicInventoryDto.CreationDate,
                      cryptographicInventoryDto.NextRotationDue,
                      cryptographicInventoryDto.CreatedAt,
                      cryptographicInventoryDto.CreatedBy,
                      cryptographicInventoryDto.IsDeleted

                );

            newCryptographicInventory.SetLastRotationDate(cryptographicInventoryDto.LastRotationDate);
            newCryptographicInventory.SetUpdatedAt(cryptographicInventoryDto.UpdatedAt);
            newCryptographicInventory.SetIsDeleted(false);

                _cryptographicInventoryRepository.BeginTransaction();
                await _cryptographicInventoryRepository.AddAsync(newCryptographicInventory);
                await InvalidateCryptographicInventoryCacheAsync(newCryptographicInventory.CryptographicInventoryId);
            _cryptographicInventoryRepository.CommitTransaction();

                var mapper = new GetByIdCryptographicInventoryFromEntityMapper();
                var createdCryptographicInventoryDto = mapper.FromEntity(newCryptographicInventory);

                return Right<string, CryptographicInventoryDto>(createdCryptographicInventoryDto);
            }
            catch (Exception ex)
            {
                _cryptographicInventoryRepository.RollbackTransaction();
                _logger.LogError(ex, "Error creating cryptographicInventory");
                return Left<string, CryptographicInventoryDto>($"Error creating cryptographicInventory: {ex.Message}");
            }
        }

        public async Task<Either<string, CryptographicInventoryDto>> UpdateCryptographicInventoryAsync(Guid cryptographicInventoryId, CryptographicInventoryDto cryptographicInventoryDto)
        {
            try
            {
                Guard.Against.NullOrEmpty(cryptographicInventoryId, nameof(cryptographicInventoryId));
                Guard.Against.Null(cryptographicInventoryDto, nameof(cryptographicInventoryDto));

                var validationResult = await _updateCryptographicInventoryValidator.ValidateAsync(cryptographicInventoryDto);
                if (!validationResult.IsValid)
                {
                    var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                    return Left<string, CryptographicInventoryDto>($"Validation failed: {errors}");
                }

                _cryptographicInventoryRepository.BeginTransaction();
                try
                {
                    CryptographicInventory? cryptographicInventory = await _cryptographicInventoryRepository.GetByIdAsync(cryptographicInventoryId);

                    if (cryptographicInventory == null)
                        return Left<string, CryptographicInventoryDto>($"CryptographicInventory with ID {cryptographicInventoryDto.CryptographicInventoryId} not found.");

                    var updateResult = new CryptographicInventory(

                        cryptographicInventoryId: cryptographicInventory.CryptographicInventoryId,
                      cryptographicInventoryDto.MerchantId,
                      cryptographicInventoryDto.TenantId,
                      cryptographicInventoryDto.KeyName,
                      cryptographicInventoryDto.KeyType,
                      cryptographicInventoryDto.Algorithm,
                      cryptographicInventoryDto.KeyLength,
                      cryptographicInventoryDto.KeyLocation,
                      cryptographicInventoryDto.CreationDate,
                      cryptographicInventoryDto.NextRotationDue,
                      cryptographicInventoryDto.CreatedAt,
                      cryptographicInventoryDto.CreatedBy,
                      cryptographicInventoryDto.IsDeleted
                    );

                cryptographicInventory.SetIsDeleted(cryptographicInventoryDto.IsDeleted);

                    await _cryptographicInventoryRepository.UpdateAsync(updateResult);
                    await InvalidateCryptographicInventoryCacheAsync(cryptographicInventoryId);
                    _cryptographicInventoryRepository.CommitTransaction();

                    var mapper = new GetByIdCryptographicInventoryFromEntityMapper();
                    var updatedCryptographicInventoryDto = mapper.FromEntity(updateResult);

                    return updatedCryptographicInventoryDto != null
                        ? Right<string, CryptographicInventoryDto>(updatedCryptographicInventoryDto)
                        : Left<string, CryptographicInventoryDto>("Error mapping updated cryptographicInventory to DTO");
                }
                catch
                {
                    _cryptographicInventoryRepository.RollbackTransaction();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error updating cryptographicInventory {CryptographicInventoryId}", cryptographicInventoryDto.CryptographicInventoryId);
                return Left<string, CryptographicInventoryDto>($"Error updating cryptographicInventory: {ex.Message}");
            }
        }
    public class GetByIdCryptographicInventoryEntityDtoMapper : Mapper<GetByIdCryptographicInventoryRequest, GetByIdCryptographicInventoryResponse, CryptographicInventoryDto>
    {
        public   CryptographicInventoryDto? FromEntityDto(CryptographicInventoryEntityDto cryptographicInventory)
        {
            Guard.Against.Null(cryptographicInventory, nameof(cryptographicInventory));
            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            string jsonCryptographicInventory = JsonSerializer.Serialize(cryptographicInventory, settings);
            CryptographicInventoryDto? cryptographicInventoryDto = JsonSerializer.Deserialize<CryptographicInventoryDto>(jsonCryptographicInventory, settings);
            return cryptographicInventoryDto;
        }
    }

    public class GetByIdCryptographicInventoryFromEntityMapper : Mapper<GetByIdCryptographicInventoryRequest, GetByIdCryptographicInventoryResponse, CryptographicInventoryDto>
    {
        public   CryptographicInventoryDto? FromEntity(CryptographicInventory cryptographicInventory)
        {
            Guard.Against.Null(cryptographicInventory, nameof(cryptographicInventory));
            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            string jsonCryptographicInventory = JsonSerializer.Serialize(cryptographicInventory, settings);
            CryptographicInventoryDto? cryptographicInventoryDto = JsonSerializer.Deserialize<CryptographicInventoryDto>(jsonCryptographicInventory, settings);
            return cryptographicInventoryDto;
        }
    }

        public async Task<Either<string, EvidenceDto>> GetOneFullEvidenceByIdAsync(Guid evidenceId)
        {
            try
            {
            EvidenceByIdSpec spec = new(evidenceId);
                EvidenceEntityDto? evidence = await _evidenceCacheRepository.FirstOrDefaultAsync(spec);
                GetByIdEvidenceEntityDtoMapper mapper = new();

                if (evidence != null)
                {
                    EvidenceDto? evidenceDto = mapper.FromEntityDto(evidence);
                    return evidenceDto != null
                        ? Right<string, EvidenceDto>(evidenceDto)
                        : Left<string, EvidenceDto>("Error mapping evidence to DTO");
                }
                return Left<string, EvidenceDto>("Evidence not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error retrieving evidence {EvidenceId}", evidenceId);
                return Left<string, EvidenceDto>($"Error retrieving evidence: {ex.Message}");
            }
        }

        public async Task<Either<string, List<EvidenceDto>>> SearchEvidencesAsync(string searchTerm)
        {
            try
            {
                Guard.Against.NullOrWhiteSpace(searchTerm, nameof(searchTerm));
                var spec = new EvidenceSearchSpec(searchTerm);
                var evidences = await _evidenceCacheRepository.ListAsync(spec);

                var settings = GJset.GetSystemTextJsonSettings();
                var evidenceDtos = evidences
                    .Select(i => JsonSerializer.Deserialize<EvidenceDto>(JsonSerializer.Serialize(i, settings), settings))
                    .Where(dto => dto != null)
                    .ToList();

                return Right<string, List<EvidenceDto>>(evidenceDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error searching evidences with term: {SearchTerm}", searchTerm);
                return Left<string, List<EvidenceDto>>($"Error searching evidences: {ex.Message}");
            }
        }

        public async Task<Either<string, ListEvidenceResponse>> GetFilteredEvidencesAsync(FilteredEvidenceRequest req)
        {
            try
            {
                Guard.Against.NegativeOrZero(req.PageNumber, nameof(req.PageNumber));
                Guard.Against.NegativeOrZero(req.PageSize, nameof(req.PageSize));

                List<PCIShieldLib.SharedKernel.Interfaces.Sort> convertedSortings = new();
                var errors = new List<string>();

            if (req.Sorting == null)
            {
                req.Sorting = new List<Sort>();
            }
            foreach (var sort1 in req.Sorting)
            {
                    var sort = (Sort)sort1;
                    if (string.IsNullOrWhiteSpace(sort.Field))
                    {
                        errors.Add("Sort field cannot be empty");
                        continue;
                    }
                    convertedSortings.Add(
                        new PCIShieldLib.SharedKernel.Interfaces.Sort { Field = sort.Field, Direction = sort.Direction }
                    );
                }

                if (errors.Any())
                    return Left<string, ListEvidenceResponse>(string.Join("; ", errors));

                var spec = new EvidenceAdvancedFilterSpec(req.PageNumber, req.PageSize, req.Filters, convertedSortings);
                var evidences = await _evidenceCacheRepository.ListAsync(spec);

                if (!evidences.Any())
                    return Right<string, ListEvidenceResponse>(new ListEvidenceResponse { Evidences = null, Count = 0 });

                var settings = GJset.GetSystemTextJsonSettings();
                var evidenceDtos = evidences
                    .Skip((req.PageNumber - 1) * req.PageSize)
                    .Take(req.PageSize)
                    .Select(i => JsonSerializer.Deserialize<EvidenceDto>(JsonSerializer.Serialize(i, settings), settings))
                    .Where(dto => dto != null)
                    .ToList();

                return Right<string, ListEvidenceResponse>(
                    new ListEvidenceResponse { Evidences = evidenceDtos, Count = evidences.Count }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error filtering evidences");
                return Left<string, ListEvidenceResponse>($"Error filtering evidences: {ex.Message}");
            }
        }

        public async Task<Either<string, List<EvidenceDto>>> GetEvidencePagedList(int pageNumber, int pageSize)
        {
            try
            {
                var spec = new EvidenceListPagedSpec(pageNumber, pageSize);
                var evidenceEntityDtos = await _evidenceCacheRepository.ListAsync(spec);

                if (!evidenceEntityDtos.Any())
                    return Left<string, List<EvidenceDto>>("No evidences found.");

                var settings = GJset.GetSystemTextJsonSettings();
                var evidenceDtos = evidenceEntityDtos
                    .Select(entityDto =>
                    {
                        var jsonEvidence = JsonSerializer.Serialize(entityDto, settings);
                        return JsonSerializer.Deserialize<EvidenceDto>(jsonEvidence, settings);
                    })
                    .Where(dto => dto != null)
                    .ToList();

                return evidenceDtos.Any()
                    ? Right<string, List<EvidenceDto>>(evidenceDtos)
                    : Left<string, List<EvidenceDto>>("Error mapping evidences.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error retrieving evidence list for page {PageNumber} with size {PageSize}",
                    pageNumber, pageSize);
                return Left<string, List<EvidenceDto>>($"Error retrieving evidence list: {ex.Message}");
            }
        }

        public async Task<Either<string, bool>> DeleteEvidenceAsync(Guid evidenceId)
        {
            try
            {
                Guard.Against.NullOrEmpty(evidenceId, nameof(evidenceId));

                var evidence = await _evidenceRepository.GetByIdAsync(evidenceId);

                if (evidence == null)
                    return Left<string, bool>("Evidence not found.");

                evidence.SetIsDeleted(true);
                await _evidenceRepository.UpdateAsync(evidence);
                await InvalidateEvidenceCacheAsync(evidenceId);

                return Right<string, bool>(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting evidence");
                return Left<string, bool>($"Error deleting evidence: {ex.Message}");
            }
        }

        public async Task<Either<string, EvidenceDto>> GetLastCreatedEvidenceAsync()
        {
            try
            {
                var spec = new EvidenceLastCreatedSpec();
                var evidence = await _evidenceRepository.FirstOrDefaultAsync(spec);

                if (evidence == null)
                    return Left<string, EvidenceDto>("No evidences found");

                var mapper = new GetByIdEvidenceFromEntityMapper();
                var evidenceDto = mapper.FromEntity(evidence);

                return evidenceDto != null
                    ? Right<string, EvidenceDto>(evidenceDto)
                    : Left<string, EvidenceDto>("Error mapping evidence to DTO");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving last created evidence");
                return Left<string, EvidenceDto>($"Error retrieving last created evidence: {ex.Message}");
            }
        }

        public async Task<Either<string, EvidenceDto>> CreateEvidenceAsync(EvidenceDto evidenceDto)
        {
            try
            {
                Guard.Against.Null(evidenceDto, nameof(evidenceDto));

                var newEvidence = new Evidence(

                    evidenceId: Guid.CreateVersion7(),
                      evidenceDto.MerchantId,
                      evidenceDto.TenantId,
                      evidenceDto.EvidenceCode,
                      evidenceDto.EvidenceTitle,
                      evidenceDto.EvidenceType,
                      evidenceDto.CollectedDate,
                      evidenceDto.IsValid,
                      evidenceDto.CreatedAt,
                      evidenceDto.CreatedBy,
                      evidenceDto.IsDeleted

                );

            newEvidence.SetFileHash(evidenceDto.FileHash);
            newEvidence.SetStorageUri(evidenceDto.StorageUri);
            newEvidence.SetUpdatedAt(evidenceDto.UpdatedAt);
            newEvidence.SetIsDeleted(false);

                _evidenceRepository.BeginTransaction();
                await _evidenceRepository.AddAsync(newEvidence);
                await InvalidateEvidenceCacheAsync(newEvidence.EvidenceId);
            _evidenceRepository.CommitTransaction();

                var mapper = new GetByIdEvidenceFromEntityMapper();
                var createdEvidenceDto = mapper.FromEntity(newEvidence);

                return Right<string, EvidenceDto>(createdEvidenceDto);
            }
            catch (Exception ex)
            {
                _evidenceRepository.RollbackTransaction();
                _logger.LogError(ex, "Error creating evidence");
                return Left<string, EvidenceDto>($"Error creating evidence: {ex.Message}");
            }
        }

        public async Task<Either<string, EvidenceDto>> UpdateEvidenceAsync(Guid evidenceId, EvidenceDto evidenceDto)
        {
            try
            {
                Guard.Against.NullOrEmpty(evidenceId, nameof(evidenceId));
                Guard.Against.Null(evidenceDto, nameof(evidenceDto));

                var validationResult = await _updateEvidenceValidator.ValidateAsync(evidenceDto);
                if (!validationResult.IsValid)
                {
                    var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                    return Left<string, EvidenceDto>($"Validation failed: {errors}");
                }

                _evidenceRepository.BeginTransaction();
                try
                {
                    Evidence? evidence = await _evidenceRepository.GetByIdAsync(evidenceId);

                    if (evidence == null)
                        return Left<string, EvidenceDto>($"Evidence with ID {evidenceDto.EvidenceId} not found.");

                    var updateResult = new Evidence(

                        evidenceId: evidence.EvidenceId,
                      evidenceDto.MerchantId,
                      evidenceDto.TenantId,
                      evidenceDto.EvidenceCode,
                      evidenceDto.EvidenceTitle,
                      evidenceDto.EvidenceType,
                      evidenceDto.CollectedDate,
                      evidenceDto.IsValid,
                      evidenceDto.CreatedAt,
                      evidenceDto.CreatedBy,
                      evidenceDto.IsDeleted
                    );

                evidence.SetIsDeleted(evidenceDto.IsDeleted);

                    await _evidenceRepository.UpdateAsync(updateResult);
                    await InvalidateEvidenceCacheAsync(evidenceId);
                    _evidenceRepository.CommitTransaction();

                    var mapper = new GetByIdEvidenceFromEntityMapper();
                    var updatedEvidenceDto = mapper.FromEntity(updateResult);

                    return updatedEvidenceDto != null
                        ? Right<string, EvidenceDto>(updatedEvidenceDto)
                        : Left<string, EvidenceDto>("Error mapping updated evidence to DTO");
                }
                catch
                {
                    _evidenceRepository.RollbackTransaction();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error updating evidence {EvidenceId}", evidenceDto.EvidenceId);
                return Left<string, EvidenceDto>($"Error updating evidence: {ex.Message}");
            }
        }
    public class GetByIdEvidenceEntityDtoMapper : Mapper<GetByIdEvidenceRequest, GetByIdEvidenceResponse, EvidenceDto>
    {
        public   EvidenceDto? FromEntityDto(EvidenceEntityDto evidence)
        {
            Guard.Against.Null(evidence, nameof(evidence));
            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            string jsonEvidence = JsonSerializer.Serialize(evidence, settings);
            EvidenceDto? evidenceDto = JsonSerializer.Deserialize<EvidenceDto>(jsonEvidence, settings);
            return evidenceDto;
        }
    }

    public class GetByIdEvidenceFromEntityMapper : Mapper<GetByIdEvidenceRequest, GetByIdEvidenceResponse, EvidenceDto>
    {
        public   EvidenceDto? FromEntity(Evidence evidence)
        {
            Guard.Against.Null(evidence, nameof(evidence));
            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            string jsonEvidence = JsonSerializer.Serialize(evidence, settings);
            EvidenceDto? evidenceDto = JsonSerializer.Deserialize<EvidenceDto>(jsonEvidence, settings);
            return evidenceDto;
        }
    }

        public async Task<Either<string, NetworkSegmentationDto>> GetOneFullNetworkSegmentationByIdAsync(Guid networkSegmentationId)
        {
            try
            {
            NetworkSegmentationByIdSpec spec = new(networkSegmentationId);
                NetworkSegmentationEntityDto? networkSegmentation = await _networkSegmentationCacheRepository.FirstOrDefaultAsync(spec);
                GetByIdNetworkSegmentationEntityDtoMapper mapper = new();

                if (networkSegmentation != null)
                {
                    NetworkSegmentationDto? networkSegmentationDto = mapper.FromEntityDto(networkSegmentation);
                    return networkSegmentationDto != null
                        ? Right<string, NetworkSegmentationDto>(networkSegmentationDto)
                        : Left<string, NetworkSegmentationDto>("Error mapping networkSegmentation to DTO");
                }
                return Left<string, NetworkSegmentationDto>("NetworkSegmentation not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error retrieving networkSegmentation {NetworkSegmentationId}", networkSegmentationId);
                return Left<string, NetworkSegmentationDto>($"Error retrieving networkSegmentation: {ex.Message}");
            }
        }

        public async Task<Either<string, List<NetworkSegmentationDto>>> SearchNetworkSegmentationsAsync(string searchTerm)
        {
            try
            {
                Guard.Against.NullOrWhiteSpace(searchTerm, nameof(searchTerm));
                var spec = new NetworkSegmentationSearchSpec(searchTerm);
                var networkSegmentations = await _networkSegmentationCacheRepository.ListAsync(spec);

                var settings = GJset.GetSystemTextJsonSettings();
                var networkSegmentationDtos = networkSegmentations
                    .Select(i => JsonSerializer.Deserialize<NetworkSegmentationDto>(JsonSerializer.Serialize(i, settings), settings))
                    .Where(dto => dto != null)
                    .ToList();

                return Right<string, List<NetworkSegmentationDto>>(networkSegmentationDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error searching networkSegmentations with term: {SearchTerm}", searchTerm);
                return Left<string, List<NetworkSegmentationDto>>($"Error searching networkSegmentations: {ex.Message}");
            }
        }

        public async Task<Either<string, ListNetworkSegmentationResponse>> GetFilteredNetworkSegmentationsAsync(FilteredNetworkSegmentationRequest req)
        {
            try
            {
                Guard.Against.NegativeOrZero(req.PageNumber, nameof(req.PageNumber));
                Guard.Against.NegativeOrZero(req.PageSize, nameof(req.PageSize));

                List<PCIShieldLib.SharedKernel.Interfaces.Sort> convertedSortings = new();
                var errors = new List<string>();

            if (req.Sorting == null)
            {
                req.Sorting = new List<Sort>();
            }
            foreach (var sort1 in req.Sorting)
            {
                    var sort = (Sort)sort1;
                    if (string.IsNullOrWhiteSpace(sort.Field))
                    {
                        errors.Add("Sort field cannot be empty");
                        continue;
                    }
                    convertedSortings.Add(
                        new PCIShieldLib.SharedKernel.Interfaces.Sort { Field = sort.Field, Direction = sort.Direction }
                    );
                }

                if (errors.Any())
                    return Left<string, ListNetworkSegmentationResponse>(string.Join("; ", errors));

                var spec = new NetworkSegmentationAdvancedFilterSpec(req.PageNumber, req.PageSize, req.Filters, convertedSortings);
                var networkSegmentations = await _networkSegmentationCacheRepository.ListAsync(spec);

                if (!networkSegmentations.Any())
                    return Right<string, ListNetworkSegmentationResponse>(new ListNetworkSegmentationResponse { NetworkSegmentations = null, Count = 0 });

                var settings = GJset.GetSystemTextJsonSettings();
                var networkSegmentationDtos = networkSegmentations
                    .Skip((req.PageNumber - 1) * req.PageSize)
                    .Take(req.PageSize)
                    .Select(i => JsonSerializer.Deserialize<NetworkSegmentationDto>(JsonSerializer.Serialize(i, settings), settings))
                    .Where(dto => dto != null)
                    .ToList();

                return Right<string, ListNetworkSegmentationResponse>(
                    new ListNetworkSegmentationResponse { NetworkSegmentations = networkSegmentationDtos, Count = networkSegmentations.Count }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error filtering networkSegmentations");
                return Left<string, ListNetworkSegmentationResponse>($"Error filtering networkSegmentations: {ex.Message}");
            }
        }

        public async Task<Either<string, List<NetworkSegmentationDto>>> GetNetworkSegmentationPagedList(int pageNumber, int pageSize)
        {
            try
            {
                var spec = new NetworkSegmentationListPagedSpec(pageNumber, pageSize);
                var networkSegmentationEntityDtos = await _networkSegmentationCacheRepository.ListAsync(spec);

                if (!networkSegmentationEntityDtos.Any())
                    return Left<string, List<NetworkSegmentationDto>>("No networkSegmentations found.");

                var settings = GJset.GetSystemTextJsonSettings();
                var networkSegmentationDtos = networkSegmentationEntityDtos
                    .Select(entityDto =>
                    {
                        var jsonNetworkSegmentation = JsonSerializer.Serialize(entityDto, settings);
                        return JsonSerializer.Deserialize<NetworkSegmentationDto>(jsonNetworkSegmentation, settings);
                    })
                    .Where(dto => dto != null)
                    .ToList();

                return networkSegmentationDtos.Any()
                    ? Right<string, List<NetworkSegmentationDto>>(networkSegmentationDtos)
                    : Left<string, List<NetworkSegmentationDto>>("Error mapping networkSegmentations.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error retrieving networkSegmentation list for page {PageNumber} with size {PageSize}",
                    pageNumber, pageSize);
                return Left<string, List<NetworkSegmentationDto>>($"Error retrieving networkSegmentation list: {ex.Message}");
            }
        }

        public async Task<Either<string, bool>> DeleteNetworkSegmentationAsync(Guid networkSegmentationId)
        {
            try
            {
                Guard.Against.NullOrEmpty(networkSegmentationId, nameof(networkSegmentationId));

                var networkSegmentation = await _networkSegmentationRepository.GetByIdAsync(networkSegmentationId);

                if (networkSegmentation == null)
                    return Left<string, bool>("NetworkSegmentation not found.");

                networkSegmentation.SetIsDeleted(true);
                await _networkSegmentationRepository.UpdateAsync(networkSegmentation);
                await InvalidateNetworkSegmentationCacheAsync(networkSegmentationId);

                return Right<string, bool>(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting networkSegmentation");
                return Left<string, bool>($"Error deleting networkSegmentation: {ex.Message}");
            }
        }

        public async Task<Either<string, NetworkSegmentationDto>> GetLastCreatedNetworkSegmentationAsync()
        {
            try
            {
                var spec = new NetworkSegmentationLastCreatedSpec();
                var networkSegmentation = await _networkSegmentationRepository.FirstOrDefaultAsync(spec);

                if (networkSegmentation == null)
                    return Left<string, NetworkSegmentationDto>("No networkSegmentations found");

                var mapper = new GetByIdNetworkSegmentationFromEntityMapper();
                var networkSegmentationDto = mapper.FromEntity(networkSegmentation);

                return networkSegmentationDto != null
                    ? Right<string, NetworkSegmentationDto>(networkSegmentationDto)
                    : Left<string, NetworkSegmentationDto>("Error mapping networkSegmentation to DTO");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving last created networkSegmentation");
                return Left<string, NetworkSegmentationDto>($"Error retrieving last created networkSegmentation: {ex.Message}");
            }
        }

        public async Task<Either<string, NetworkSegmentationDto>> CreateNetworkSegmentationAsync(NetworkSegmentationDto networkSegmentationDto)
        {
            try
            {
                Guard.Against.Null(networkSegmentationDto, nameof(networkSegmentationDto));

                var newNetworkSegmentation = new NetworkSegmentation(

                    networkSegmentationId: Guid.CreateVersion7(),
                      networkSegmentationDto.MerchantId,
                      networkSegmentationDto.TenantId,
                      networkSegmentationDto.SegmentName,
                      networkSegmentationDto.IPRange,
                      networkSegmentationDto.IsInCDE,
                      networkSegmentationDto.CreatedAt,
                      networkSegmentationDto.CreatedBy,
                      networkSegmentationDto.IsDeleted

                );

            newNetworkSegmentation.SetVLANId(networkSegmentationDto.VLANId);
            newNetworkSegmentation.SetFirewallRules(networkSegmentationDto.FirewallRules);
            newNetworkSegmentation.SetLastValidated(networkSegmentationDto.LastValidated);
            newNetworkSegmentation.SetUpdatedAt(networkSegmentationDto.UpdatedAt);
            newNetworkSegmentation.SetIsDeleted(false);

                _networkSegmentationRepository.BeginTransaction();
                await _networkSegmentationRepository.AddAsync(newNetworkSegmentation);
                await InvalidateNetworkSegmentationCacheAsync(newNetworkSegmentation.NetworkSegmentationId);
            _networkSegmentationRepository.CommitTransaction();

                var mapper = new GetByIdNetworkSegmentationFromEntityMapper();
                var createdNetworkSegmentationDto = mapper.FromEntity(newNetworkSegmentation);

                return Right<string, NetworkSegmentationDto>(createdNetworkSegmentationDto);
            }
            catch (Exception ex)
            {
                _networkSegmentationRepository.RollbackTransaction();
                _logger.LogError(ex, "Error creating networkSegmentation");
                return Left<string, NetworkSegmentationDto>($"Error creating networkSegmentation: {ex.Message}");
            }
        }

        public async Task<Either<string, NetworkSegmentationDto>> UpdateNetworkSegmentationAsync(Guid networkSegmentationId, NetworkSegmentationDto networkSegmentationDto)
        {
            try
            {
                Guard.Against.NullOrEmpty(networkSegmentationId, nameof(networkSegmentationId));
                Guard.Against.Null(networkSegmentationDto, nameof(networkSegmentationDto));

                var validationResult = await _updateNetworkSegmentationValidator.ValidateAsync(networkSegmentationDto);
                if (!validationResult.IsValid)
                {
                    var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                    return Left<string, NetworkSegmentationDto>($"Validation failed: {errors}");
                }

                _networkSegmentationRepository.BeginTransaction();
                try
                {
                    NetworkSegmentation? networkSegmentation = await _networkSegmentationRepository.GetByIdAsync(networkSegmentationId);

                    if (networkSegmentation == null)
                        return Left<string, NetworkSegmentationDto>($"NetworkSegmentation with ID {networkSegmentationDto.NetworkSegmentationId} not found.");

                    var updateResult = new NetworkSegmentation(

                        networkSegmentationId: networkSegmentation.NetworkSegmentationId,
                      networkSegmentationDto.MerchantId,
                      networkSegmentationDto.TenantId,
                      networkSegmentationDto.SegmentName,
                      networkSegmentationDto.IPRange,
                      networkSegmentationDto.IsInCDE,
                      networkSegmentationDto.CreatedAt,
                      networkSegmentationDto.CreatedBy,
                      networkSegmentationDto.IsDeleted
                    );

                networkSegmentation.SetIsDeleted(networkSegmentationDto.IsDeleted);

                    await _networkSegmentationRepository.UpdateAsync(updateResult);
                    await InvalidateNetworkSegmentationCacheAsync(networkSegmentationId);
                    _networkSegmentationRepository.CommitTransaction();

                    var mapper = new GetByIdNetworkSegmentationFromEntityMapper();
                    var updatedNetworkSegmentationDto = mapper.FromEntity(updateResult);

                    return updatedNetworkSegmentationDto != null
                        ? Right<string, NetworkSegmentationDto>(updatedNetworkSegmentationDto)
                        : Left<string, NetworkSegmentationDto>("Error mapping updated networkSegmentation to DTO");
                }
                catch
                {
                    _networkSegmentationRepository.RollbackTransaction();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error updating networkSegmentation {NetworkSegmentationId}", networkSegmentationDto.NetworkSegmentationId);
                return Left<string, NetworkSegmentationDto>($"Error updating networkSegmentation: {ex.Message}");
            }
        }
    public class GetByIdNetworkSegmentationEntityDtoMapper : Mapper<GetByIdNetworkSegmentationRequest, GetByIdNetworkSegmentationResponse, NetworkSegmentationDto>
    {
        public   NetworkSegmentationDto? FromEntityDto(NetworkSegmentationEntityDto networkSegmentation)
        {
            Guard.Against.Null(networkSegmentation, nameof(networkSegmentation));
            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            string jsonNetworkSegmentation = JsonSerializer.Serialize(networkSegmentation, settings);
            NetworkSegmentationDto? networkSegmentationDto = JsonSerializer.Deserialize<NetworkSegmentationDto>(jsonNetworkSegmentation, settings);
            return networkSegmentationDto;
        }
    }

    public class GetByIdNetworkSegmentationFromEntityMapper : Mapper<GetByIdNetworkSegmentationRequest, GetByIdNetworkSegmentationResponse, NetworkSegmentationDto>
    {
        public   NetworkSegmentationDto? FromEntity(NetworkSegmentation networkSegmentation)
        {
            Guard.Against.Null(networkSegmentation, nameof(networkSegmentation));
            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            string jsonNetworkSegmentation = JsonSerializer.Serialize(networkSegmentation, settings);
            NetworkSegmentationDto? networkSegmentationDto = JsonSerializer.Deserialize<NetworkSegmentationDto>(jsonNetworkSegmentation, settings);
            return networkSegmentationDto;
        }
    }

        public async Task<Either<string, PaymentChannelDto>> GetOneFullPaymentChannelByIdAsync(Guid paymentChannelId)
        {
            try
            {
            PaymentChannelByIdSpec spec = new(paymentChannelId);
                PaymentChannelEntityDto? paymentChannel = await _paymentChannelCacheRepository.FirstOrDefaultAsync(spec);
                GetByIdPaymentChannelEntityDtoMapper mapper = new();

                if (paymentChannel != null)
                {
                    PaymentChannelDto? paymentChannelDto = mapper.FromEntityDto(paymentChannel);
                    return paymentChannelDto != null
                        ? Right<string, PaymentChannelDto>(paymentChannelDto)
                        : Left<string, PaymentChannelDto>("Error mapping paymentChannel to DTO");
                }
                return Left<string, PaymentChannelDto>("PaymentChannel not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error retrieving paymentChannel {PaymentChannelId}", paymentChannelId);
                return Left<string, PaymentChannelDto>($"Error retrieving paymentChannel: {ex.Message}");
            }
        }

        public async Task<Either<string, List<PaymentChannelDto>>> SearchPaymentChannelsAsync(string searchTerm)
        {
            try
            {
                Guard.Against.NullOrWhiteSpace(searchTerm, nameof(searchTerm));
                var spec = new PaymentChannelSearchSpec(searchTerm);
                var paymentChannels = await _paymentChannelCacheRepository.ListAsync(spec);

                var settings = GJset.GetSystemTextJsonSettings();
                var paymentChannelDtos = paymentChannels
                    .Select(i => JsonSerializer.Deserialize<PaymentChannelDto>(JsonSerializer.Serialize(i, settings), settings))
                    .Where(dto => dto != null)
                    .ToList();

                return Right<string, List<PaymentChannelDto>>(paymentChannelDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error searching paymentChannels with term: {SearchTerm}", searchTerm);
                return Left<string, List<PaymentChannelDto>>($"Error searching paymentChannels: {ex.Message}");
            }
        }

        public async Task<Either<string, ListPaymentChannelResponse>> GetFilteredPaymentChannelsAsync(FilteredPaymentChannelRequest req)
        {
            try
            {
                Guard.Against.NegativeOrZero(req.PageNumber, nameof(req.PageNumber));
                Guard.Against.NegativeOrZero(req.PageSize, nameof(req.PageSize));

                List<PCIShieldLib.SharedKernel.Interfaces.Sort> convertedSortings = new();
                var errors = new List<string>();

            if (req.Sorting == null)
                {
                    req.Sorting = new List<Sort>();
            }

            foreach (var sort1 in req.Sorting)
                {
                    var sort = (Sort)sort1;
                    if (string.IsNullOrWhiteSpace(sort.Field))
                    {
                        errors.Add("Sort field cannot be empty");
                        continue;
                    }
                    convertedSortings.Add(
                        new PCIShieldLib.SharedKernel.Interfaces.Sort { Field = sort.Field, Direction = sort.Direction }
                    );
                }

                if (errors.Any())
                    return Left<string, ListPaymentChannelResponse>(string.Join("; ", errors));

                var spec = new PaymentChannelAdvancedFilterSpec(req.PageNumber, req.PageSize, req.Filters, convertedSortings);
                var paymentChannels = await _paymentChannelCacheRepository.ListAsync(spec);

                if (!paymentChannels.Any())
                    return Right<string, ListPaymentChannelResponse>(new ListPaymentChannelResponse { PaymentChannels = null, Count = 0 });

                var settings = GJset.GetSystemTextJsonSettings();
                var paymentChannelDtos = paymentChannels
                    .Skip((req.PageNumber - 1) * req.PageSize)
                    .Take(req.PageSize)
                    .Select(i => JsonSerializer.Deserialize<PaymentChannelDto>(JsonSerializer.Serialize(i, settings), settings))
                    .Where(dto => dto != null)
                    .ToList();

                return Right<string, ListPaymentChannelResponse>(
                    new ListPaymentChannelResponse { PaymentChannels = paymentChannelDtos, Count = paymentChannels.Count }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error filtering paymentChannels");
                return Left<string, ListPaymentChannelResponse>($"Error filtering paymentChannels: {ex.Message}");
            }
        }

        public async Task<Either<string, List<PaymentChannelDto>>> GetPaymentChannelPagedList(int pageNumber, int pageSize)
        {
            try
            {
                var spec = new PaymentChannelListPagedSpec(pageNumber, pageSize);
                var paymentChannelEntityDtos = await _paymentChannelCacheRepository.ListAsync(spec);

                if (!paymentChannelEntityDtos.Any())
                    return Left<string, List<PaymentChannelDto>>("No paymentChannels found.");

                var settings = GJset.GetSystemTextJsonSettings();
                var paymentChannelDtos = paymentChannelEntityDtos
                    .Select(entityDto =>
                    {
                        var jsonPaymentChannel = JsonSerializer.Serialize(entityDto, settings);
                        return JsonSerializer.Deserialize<PaymentChannelDto>(jsonPaymentChannel, settings);
                    })
                    .Where(dto => dto != null)
                    .ToList();

                return paymentChannelDtos.Any()
                    ? Right<string, List<PaymentChannelDto>>(paymentChannelDtos)
                    : Left<string, List<PaymentChannelDto>>("Error mapping paymentChannels.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error retrieving paymentChannel list for page {PageNumber} with size {PageSize}",
                    pageNumber, pageSize);
                return Left<string, List<PaymentChannelDto>>($"Error retrieving paymentChannel list: {ex.Message}");
            }
        }

        public async Task<Either<string, bool>> DeletePaymentChannelAsync(Guid paymentChannelId)
        {
            try
            {
                Guard.Against.NullOrEmpty(paymentChannelId, nameof(paymentChannelId));

                var paymentChannel = await _paymentChannelRepository.GetByIdAsync(paymentChannelId);

                if (paymentChannel == null)
                    return Left<string, bool>("PaymentChannel not found.");

                paymentChannel.SetIsDeleted(true);
                await _paymentChannelRepository.UpdateAsync(paymentChannel);
                await InvalidatePaymentChannelCacheAsync(paymentChannelId);

                return Right<string, bool>(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting paymentChannel");
                return Left<string, bool>($"Error deleting paymentChannel: {ex.Message}");
            }
        }

        public async Task<Either<string, PaymentChannelDto>> GetLastCreatedPaymentChannelAsync()
        {
            try
            {
                var spec = new PaymentChannelLastCreatedSpec();
                var paymentChannel = await _paymentChannelRepository.FirstOrDefaultAsync(spec);

                if (paymentChannel == null)
                    return Left<string, PaymentChannelDto>("No paymentChannels found");

                var mapper = new GetByIdPaymentChannelFromEntityMapper();
                var paymentChannelDto = mapper.FromEntity(paymentChannel);

                return paymentChannelDto != null
                    ? Right<string, PaymentChannelDto>(paymentChannelDto)
                    : Left<string, PaymentChannelDto>("Error mapping paymentChannel to DTO");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving last created paymentChannel");
                return Left<string, PaymentChannelDto>($"Error retrieving last created paymentChannel: {ex.Message}");
            }
        }

        public async Task<Either<string, PaymentChannelDto>> CreatePaymentChannelAsync(PaymentChannelDto paymentChannelDto)
        {
            try
            {
                Guard.Against.Null(paymentChannelDto, nameof(paymentChannelDto));

                var newPaymentChannel = new PaymentChannel(

                    paymentChannelId: Guid.CreateVersion7(),
                      paymentChannelDto.MerchantId,
                      paymentChannelDto.TenantId,
                      paymentChannelDto.ChannelCode,
                      paymentChannelDto.ChannelName,
                      paymentChannelDto.ChannelType,
                      paymentChannelDto.ProcessingVolume,
                      paymentChannelDto.IsInScope,
                      paymentChannelDto.TokenizationEnabled,
                      paymentChannelDto.CreatedAt,
                      paymentChannelDto.CreatedBy,
                      paymentChannelDto.IsDeleted

                );

            newPaymentChannel.SetUpdatedAt(paymentChannelDto.UpdatedAt);
            newPaymentChannel.SetIsDeleted(false);

                _paymentChannelRepository.BeginTransaction();
                await _paymentChannelRepository.AddAsync(newPaymentChannel);
                await InvalidatePaymentChannelCacheAsync(newPaymentChannel.PaymentChannelId);
            _paymentChannelRepository.CommitTransaction();

                var mapper = new GetByIdPaymentChannelFromEntityMapper();
                var createdPaymentChannelDto = mapper.FromEntity(newPaymentChannel);

                return Right<string, PaymentChannelDto>(createdPaymentChannelDto);
            }
            catch (Exception ex)
            {
                _paymentChannelRepository.RollbackTransaction();
                _logger.LogError(ex, "Error creating paymentChannel");
                return Left<string, PaymentChannelDto>($"Error creating paymentChannel: {ex.Message}");
            }
        }

        public async Task<Either<string, PaymentChannelDto>> UpdatePaymentChannelAsync(Guid paymentChannelId, PaymentChannelDto paymentChannelDto)
        {
            try
            {
                Guard.Against.NullOrEmpty(paymentChannelId, nameof(paymentChannelId));
                Guard.Against.Null(paymentChannelDto, nameof(paymentChannelDto));

                var validationResult = await _updatePaymentChannelValidator.ValidateAsync(paymentChannelDto);
                if (!validationResult.IsValid)
                {
                    var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                    return Left<string, PaymentChannelDto>($"Validation failed: {errors}");
                }

                _paymentChannelRepository.BeginTransaction();
                try
                {
                    PaymentChannel? paymentChannel = await _paymentChannelRepository.GetByIdAsync(paymentChannelId);

                    if (paymentChannel == null)
                        return Left<string, PaymentChannelDto>($"PaymentChannel with ID {paymentChannelDto.PaymentChannelId} not found.");

                    var updateResult = new PaymentChannel(

                        paymentChannelId: paymentChannel.PaymentChannelId,
                      paymentChannelDto.MerchantId,
                      paymentChannelDto.TenantId,
                      paymentChannelDto.ChannelCode,
                      paymentChannelDto.ChannelName,
                      paymentChannelDto.ChannelType,
                      paymentChannelDto.ProcessingVolume,
                      paymentChannelDto.IsInScope,
                      paymentChannelDto.TokenizationEnabled,
                      paymentChannelDto.CreatedAt,
                      paymentChannelDto.CreatedBy,
                      paymentChannelDto.IsDeleted
                    );

                paymentChannel.SetIsDeleted(paymentChannelDto.IsDeleted);

                    await _paymentChannelRepository.UpdateAsync(updateResult);
                    await InvalidatePaymentChannelCacheAsync(paymentChannelId);
                    _paymentChannelRepository.CommitTransaction();

                    var mapper = new GetByIdPaymentChannelFromEntityMapper();
                    var updatedPaymentChannelDto = mapper.FromEntity(updateResult);

                    return updatedPaymentChannelDto != null
                        ? Right<string, PaymentChannelDto>(updatedPaymentChannelDto)
                        : Left<string, PaymentChannelDto>("Error mapping updated paymentChannel to DTO");
                }
                catch
                {
                    _paymentChannelRepository.RollbackTransaction();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error updating paymentChannel {PaymentChannelId}", paymentChannelDto.PaymentChannelId);
                return Left<string, PaymentChannelDto>($"Error updating paymentChannel: {ex.Message}");
            }
        }
    public class GetByIdPaymentChannelEntityDtoMapper : Mapper<GetByIdPaymentChannelRequest, GetByIdPaymentChannelResponse, PaymentChannelDto>
    {
        public   PaymentChannelDto? FromEntityDto(PaymentChannelEntityDto paymentChannel)
        {
            Guard.Against.Null(paymentChannel, nameof(paymentChannel));
            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            string jsonPaymentChannel = JsonSerializer.Serialize(paymentChannel, settings);
            PaymentChannelDto? paymentChannelDto = JsonSerializer.Deserialize<PaymentChannelDto>(jsonPaymentChannel, settings);
            return paymentChannelDto;
        }
    }

    public class GetByIdPaymentChannelFromEntityMapper : Mapper<GetByIdPaymentChannelRequest, GetByIdPaymentChannelResponse, PaymentChannelDto>
    {
        public   PaymentChannelDto? FromEntity(PaymentChannel paymentChannel)
        {
            Guard.Against.Null(paymentChannel, nameof(paymentChannel));
            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            string jsonPaymentChannel = JsonSerializer.Serialize(paymentChannel, settings);
            PaymentChannelDto? paymentChannelDto = JsonSerializer.Deserialize<PaymentChannelDto>(jsonPaymentChannel, settings);
            return paymentChannelDto;
        }
    }

        public async Task<Either<string, ServiceProviderDto>> GetOneFullServiceProviderByIdAsync(Guid serviceProviderId)
        {
            try
            {
            ServiceProviderByIdSpec spec = new(serviceProviderId);
                ServiceProviderEntityDto? serviceProvider = await _serviceProviderCacheRepository.FirstOrDefaultAsync(spec);
                GetByIdServiceProviderEntityDtoMapper mapper = new();

                if (serviceProvider != null)
                {
                    ServiceProviderDto? serviceProviderDto = mapper.FromEntityDto(serviceProvider);
                    return serviceProviderDto != null
                        ? Right<string, ServiceProviderDto>(serviceProviderDto)
                        : Left<string, ServiceProviderDto>("Error mapping serviceProvider to DTO");
                }
                return Left<string, ServiceProviderDto>("ServiceProvider not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error retrieving serviceProvider {ServiceProviderId}", serviceProviderId);
                return Left<string, ServiceProviderDto>($"Error retrieving serviceProvider: {ex.Message}");
            }
        }

        public async Task<Either<string, List<ServiceProviderDto>>> SearchServiceProvidersAsync(string searchTerm)
        {
            try
            {
                Guard.Against.NullOrWhiteSpace(searchTerm, nameof(searchTerm));
                var spec = new ServiceProviderSearchSpec(searchTerm);
                var serviceProviders = await _serviceProviderCacheRepository.ListAsync(spec);

                var settings = GJset.GetSystemTextJsonSettings();
                var serviceProviderDtos = serviceProviders
                    .Select(i => JsonSerializer.Deserialize<ServiceProviderDto>(JsonSerializer.Serialize(i, settings), settings))
                    .Where(dto => dto != null)
                    .ToList();

                return Right<string, List<ServiceProviderDto>>(serviceProviderDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error searching serviceProviders with term: {SearchTerm}", searchTerm);
                return Left<string, List<ServiceProviderDto>>($"Error searching serviceProviders: {ex.Message}");
            }
        }

        public async Task<Either<string, ListServiceProviderResponse>> GetFilteredServiceProvidersAsync(FilteredServiceProviderRequest req)
        {
            try
            {
                Guard.Against.NegativeOrZero(req.PageNumber, nameof(req.PageNumber));
                Guard.Against.NegativeOrZero(req.PageSize, nameof(req.PageSize));

                List<PCIShieldLib.SharedKernel.Interfaces.Sort> convertedSortings = new();
                var errors = new List<string>();

            if (req.Sorting == null)
            {
                req.Sorting = new List<Sort>();
            }
            foreach (var sort1 in req.Sorting)
            {
                    var sort = (Sort)sort1;
                    if (string.IsNullOrWhiteSpace(sort.Field))
                    {
                        errors.Add("Sort field cannot be empty");
                        continue;
                    }
                    convertedSortings.Add(
                        new PCIShieldLib.SharedKernel.Interfaces.Sort { Field = sort.Field, Direction = sort.Direction }
                    );
                }

                if (errors.Any())
                    return Left<string, ListServiceProviderResponse>(string.Join("; ", errors));

                var spec = new ServiceProviderAdvancedFilterSpec(req.PageNumber, req.PageSize, req.Filters, convertedSortings);
                var serviceProviders = await _serviceProviderCacheRepository.ListAsync(spec);

                if (!serviceProviders.Any())
                    return Right<string, ListServiceProviderResponse>(new ListServiceProviderResponse { ServiceProviders = null, Count = 0 });

                var settings = GJset.GetSystemTextJsonSettings();
                var serviceProviderDtos = serviceProviders
                    .Skip((req.PageNumber - 1) * req.PageSize)
                    .Take(req.PageSize)
                    .Select(i => JsonSerializer.Deserialize<ServiceProviderDto>(JsonSerializer.Serialize(i, settings), settings))
                    .Where(dto => dto != null)
                    .ToList();

                return Right<string, ListServiceProviderResponse>(
                    new ListServiceProviderResponse { ServiceProviders = serviceProviderDtos, Count = serviceProviders.Count }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error filtering serviceProviders");
                return Left<string, ListServiceProviderResponse>($"Error filtering serviceProviders: {ex.Message}");
            }
        }

        public async Task<Either<string, List<ServiceProviderDto>>> GetServiceProviderPagedList(int pageNumber, int pageSize)
        {
            try
            {
                var spec = new ServiceProviderListPagedSpec(pageNumber, pageSize);
                var serviceProviderEntityDtos = await _serviceProviderCacheRepository.ListAsync(spec);

                if (!serviceProviderEntityDtos.Any())
                    return Left<string, List<ServiceProviderDto>>("No serviceProviders found.");

                var settings = GJset.GetSystemTextJsonSettings();
                var serviceProviderDtos = serviceProviderEntityDtos
                    .Select(entityDto =>
                    {
                        var jsonServiceProvider = JsonSerializer.Serialize(entityDto, settings);
                        return JsonSerializer.Deserialize<ServiceProviderDto>(jsonServiceProvider, settings);
                    })
                    .Where(dto => dto != null)
                    .ToList();

                return serviceProviderDtos.Any()
                    ? Right<string, List<ServiceProviderDto>>(serviceProviderDtos)
                    : Left<string, List<ServiceProviderDto>>("Error mapping serviceProviders.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error retrieving serviceProvider list for page {PageNumber} with size {PageSize}",
                    pageNumber, pageSize);
                return Left<string, List<ServiceProviderDto>>($"Error retrieving serviceProvider list: {ex.Message}");
            }
        }

        public async Task<Either<string, bool>> DeleteServiceProviderAsync(Guid serviceProviderId)
        {
            try
            {
                Guard.Against.NullOrEmpty(serviceProviderId, nameof(serviceProviderId));

                var serviceProvider = await _serviceProviderRepository.GetByIdAsync(serviceProviderId);

                if (serviceProvider == null)
                    return Left<string, bool>("ServiceProvider not found.");

                serviceProvider.SetIsDeleted(true);
                await _serviceProviderRepository.UpdateAsync(serviceProvider);
                await InvalidateServiceProviderCacheAsync(serviceProviderId);

                return Right<string, bool>(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting serviceProvider");
                return Left<string, bool>($"Error deleting serviceProvider: {ex.Message}");
            }
        }

        public async Task<Either<string, ServiceProviderDto>> GetLastCreatedServiceProviderAsync()
        {
            try
            {
                var spec = new ServiceProviderLastCreatedSpec();
                var serviceProvider = await _serviceProviderRepository.FirstOrDefaultAsync(spec);

                if (serviceProvider == null)
                    return Left<string, ServiceProviderDto>("No serviceProviders found");

                var mapper = new GetByIdServiceProviderFromEntityMapper();
                var serviceProviderDto = mapper.FromEntity(serviceProvider);

                return serviceProviderDto != null
                    ? Right<string, ServiceProviderDto>(serviceProviderDto)
                    : Left<string, ServiceProviderDto>("Error mapping serviceProvider to DTO");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving last created serviceProvider");
                return Left<string, ServiceProviderDto>($"Error retrieving last created serviceProvider: {ex.Message}");
            }
        }

        public async Task<Either<string, ServiceProviderDto>> CreateServiceProviderAsync(ServiceProviderDto serviceProviderDto)
        {
            try
            {
                Guard.Against.Null(serviceProviderDto, nameof(serviceProviderDto));

                var newServiceProvider = new PCIShield.Domain.Entities.ServiceProvider(

                    serviceProviderId: Guid.CreateVersion7(),
                      serviceProviderDto.MerchantId,
                      serviceProviderDto.TenantId,
                      serviceProviderDto.ProviderName,
                      serviceProviderDto.ServiceType,
                      serviceProviderDto.IsPCICompliant,
                      serviceProviderDto.CreatedAt,
                      serviceProviderDto.CreatedBy,
                      serviceProviderDto.IsDeleted

                );

            newServiceProvider.SetAOCExpiryDate(serviceProviderDto.AOCExpiryDate);
            newServiceProvider.SetResponsibilityMatrix(serviceProviderDto.ResponsibilityMatrix);
            newServiceProvider.SetUpdatedAt(serviceProviderDto.UpdatedAt);
            newServiceProvider.SetIsDeleted(false);

                _serviceProviderRepository.BeginTransaction();
                await _serviceProviderRepository.AddAsync(newServiceProvider);
                await InvalidateServiceProviderCacheAsync(newServiceProvider.ServiceProviderId);
            _serviceProviderRepository.CommitTransaction();

                var mapper = new GetByIdServiceProviderFromEntityMapper();
                var createdServiceProviderDto = mapper.FromEntity(newServiceProvider);

                return Right<string, ServiceProviderDto>(createdServiceProviderDto);
            }
            catch (Exception ex)
            {
                _serviceProviderRepository.RollbackTransaction();
                _logger.LogError(ex, "Error creating serviceProvider");
                return Left<string, ServiceProviderDto>($"Error creating serviceProvider: {ex.Message}");
            }
        }

        public async Task<Either<string, ServiceProviderDto>> UpdateServiceProviderAsync(Guid serviceProviderId, ServiceProviderDto serviceProviderDto)
        {
            try
            {
                Guard.Against.NullOrEmpty(serviceProviderId, nameof(serviceProviderId));
                Guard.Against.Null(serviceProviderDto, nameof(serviceProviderDto));

                var validationResult = await _updateServiceProviderValidator.ValidateAsync(serviceProviderDto);
                if (!validationResult.IsValid)
                {
                    var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                    return Left<string, ServiceProviderDto>($"Validation failed: {errors}");
                }

                _serviceProviderRepository.BeginTransaction();
                try
                {
                PCIShield.Domain.Entities.ServiceProvider? serviceProvider = await _serviceProviderRepository.GetByIdAsync(serviceProviderId);

                    if (serviceProvider == null)
                        return Left<string, ServiceProviderDto>($"ServiceProvider with ID {serviceProviderDto.ServiceProviderId} not found.");

                    var updateResult = new PCIShield.Domain.Entities.ServiceProvider(

                        serviceProviderId: serviceProvider.ServiceProviderId,
                      serviceProviderDto.MerchantId,
                      serviceProviderDto.TenantId,
                      serviceProviderDto.ProviderName,
                      serviceProviderDto.ServiceType,
                      serviceProviderDto.IsPCICompliant,
                      serviceProviderDto.CreatedAt,
                      serviceProviderDto.CreatedBy,
                      serviceProviderDto.IsDeleted
                    );

                serviceProvider.SetIsDeleted(serviceProviderDto.IsDeleted);

                    await _serviceProviderRepository.UpdateAsync(updateResult);
                    await InvalidateServiceProviderCacheAsync(serviceProviderId);
                    _serviceProviderRepository.CommitTransaction();

                    var mapper = new GetByIdServiceProviderFromEntityMapper();
                    var updatedServiceProviderDto = mapper.FromEntity(updateResult);

                    return updatedServiceProviderDto != null
                        ? Right<string, ServiceProviderDto>(updatedServiceProviderDto)
                        : Left<string, ServiceProviderDto>("Error mapping updated serviceProvider to DTO");
                }
                catch
                {
                    _serviceProviderRepository.RollbackTransaction();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error updating serviceProvider {ServiceProviderId}", serviceProviderDto.ServiceProviderId);
                return Left<string, ServiceProviderDto>($"Error updating serviceProvider: {ex.Message}");
            }
        }
    public class GetByIdServiceProviderEntityDtoMapper : Mapper<GetByIdServiceProviderRequest, GetByIdServiceProviderResponse, ServiceProviderDto>
    {
        public   ServiceProviderDto? FromEntityDto(ServiceProviderEntityDto serviceProvider)
        {
            Guard.Against.Null(serviceProvider, nameof(serviceProvider));
            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            string jsonServiceProvider = JsonSerializer.Serialize(serviceProvider, settings);
            ServiceProviderDto? serviceProviderDto = JsonSerializer.Deserialize<ServiceProviderDto>(jsonServiceProvider, settings);
            return serviceProviderDto;
        }
    }

    public class GetByIdServiceProviderFromEntityMapper : Mapper<GetByIdServiceProviderRequest, GetByIdServiceProviderResponse, ServiceProviderDto>
    {
        public   ServiceProviderDto? FromEntity(PCIShield.Domain.Entities.ServiceProvider serviceProvider)
        {
            Guard.Against.Null(serviceProvider, nameof(serviceProvider));
            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            string jsonServiceProvider = JsonSerializer.Serialize(serviceProvider, settings);
            ServiceProviderDto? serviceProviderDto = JsonSerializer.Deserialize<ServiceProviderDto>(jsonServiceProvider, settings);
            return serviceProviderDto;
        }
    }
    public class GetByIdControlEntityDtoMapper : Mapper<GetByIdControlRequest, GetByIdControlResponse, ControlDto>
    {
        public   ControlDto? FromEntityDto(ControlEntityDto control)
        {
            Guard.Against.Null(control, nameof(control));
            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            string jsonControl = JsonSerializer.Serialize(control, settings);
            ControlDto? controlDto = JsonSerializer.Deserialize<ControlDto>(jsonControl, settings);
            return controlDto;
        }
    }

    public class GetByIdControlFromEntityMapper : Mapper<GetByIdControlRequest, GetByIdControlResponse, ControlDto>
    {
        public   ControlDto? FromEntity(Control control)
        {
            Guard.Against.Null(control, nameof(control));
            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            string jsonControl = JsonSerializer.Serialize(control, settings);
            ControlDto? controlDto = JsonSerializer.Deserialize<ControlDto>(jsonControl, settings);
            return controlDto;
        }
    }

  }
