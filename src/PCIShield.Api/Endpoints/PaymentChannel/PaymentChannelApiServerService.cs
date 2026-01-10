using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

using Ardalis.GuardClauses;

using BlazorMauiShared.Models.PaymentChannel;

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

using BlazorMauiShared.Models.PaymentChannel;

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
using BlazorMauiShared.Models.PaymentPage;
using BlazorMauiShared.Models.ROCPackage;
using BlazorMauiShared.Models.ScanSchedule;
using BlazorMauiShared.Models.Vulnerability;
using BlazorMauiShared.Models.Merchant;

public class PaymentChannelService
{

    private readonly IMediator _mediator;
    private readonly IRepositoryFactory _repositoryFactory;
    private readonly IReadRedisRepositoryFactory _repositoryCacheFactory;
    private readonly IElasticsearchService<PaymentChannel> _paymentChannelIndexService;

    private readonly IRepository<PaymentPage> _paymentPageRepository;
    private readonly IReadRedisRepository<PaymentPage> _paymentPageCacheRepository;
    private readonly IRepository<Merchant> _merchantRepository;
    private readonly IReadRedisRepository<Merchant> _merchantCacheRepository;
    private readonly IReadRedisRepository<PaymentChannel> _paymentChannelCacheRepository;
    private readonly IRepository<PaymentChannel> _paymentChannelRepository;
    private readonly IKeyCloakTenantService _keyCloakTenantService;
    private readonly IAppLoggerService<PaymentChannelService> _logger;
    private readonly IRedisCacheService _redisCacheService;
    private readonly IValidator<PaymentChannelDto> _updatePaymentChannelValidator;
    private readonly IValidator<PaymentPageDto> _updatePaymentPageValidator;
    private readonly IValidator<MerchantDto> _updateMerchantValidator;

    public PaymentChannelService(

        IRepositoryFactory repositoryFactory,
        IReadRedisRepositoryFactory repositoryCacheFactory,
        IMediator mediator,
       
        IElasticsearchService<PaymentChannel> paymentChannelIndexService,

        IReadRedisRepository<PaymentChannel> paymentChannelCacheRepository,
        IRedisCacheService redisCacheService,
        IKeyCloakTenantService keyCloakTenantService,
        IRepository<PaymentChannel> paymentChannelRepository,
        IAppLoggerService<PaymentChannelService> logger,
        IRepository<PaymentPage> paymentPageRepository,
        IReadRedisRepository<PaymentPage> paymentPageCacheRepository,
        IRepository<Merchant> merchantRepository,
        IReadRedisRepository<Merchant> merchantCacheRepository
    )
    {

        _repositoryFactory = repositoryFactory;
        _repositoryCacheFactory = repositoryCacheFactory;
        _paymentChannelIndexService = paymentChannelIndexService;
        _mediator = mediator;
        _updatePaymentChannelValidator = new InlineValidator<PaymentChannelDto>();
        _updatePaymentPageValidator = new InlineValidator<PaymentPageDto>();
        _updateMerchantValidator = new InlineValidator<MerchantDto>();
        _paymentChannelCacheRepository = paymentChannelCacheRepository;
        _redisCacheService = redisCacheService;
        _keyCloakTenantService = keyCloakTenantService;
        _paymentChannelRepository = paymentChannelRepository;
        _logger = logger;
        _paymentPageRepository = paymentPageRepository;
        _paymentPageCacheRepository = paymentPageCacheRepository;
        _merchantRepository = merchantRepository;
        _merchantCacheRepository = merchantCacheRepository;
    }

    private async Task InvalidatePaymentPageCacheAsync(Guid paymentPageId)
    {
        var tenantId = _keyCloakTenantService.GetPCIShieldSolutionuperUserId();
        await _redisCacheService.RemoveByPatternAsync($"PaymentPageListPagedSpec--{tenantId}-*");
        await _redisCacheService.RemoveByPatternAsync(
            $"PaymentPageByIdSpec-{paymentPageId.ToString()}--{tenantId}-FirstOrDefaultAsync-PaymentPageEntityDto"
        );
        await _redisCacheService.RemoveByPatternAsync(
            $"PaymentPageListPagedSpec-1-10--{tenantId}-ListAsync-PaymentPageEntityDto"
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

    public async Task<Either<string, PaymentChannelDto>> GetOneFullPaymentChannelByIdAsync(Guid paymentChannelId, bool withPostGraph = false)
    {
        try
        {
            PaymentChannelAdvancedGraphSpecV4 spec = new(paymentChannelId);
            PaymentChannelEntityDto? paymentChannel = await _repositoryCacheFactory.GetReadRedisRepository<PaymentChannel>().FirstOrDefaultAsync(spec);
            GetByIdPaymentChannelEntityDtoMapper mapper = new();
            if (paymentChannel != null)
            {
                if (withPostGraph)
                {
                }
                PaymentChannelDto? paymentChannelDto = mapper.FromEntityDto(paymentChannel);
                if (paymentChannelDto != null)
                {
                    return Right<string, PaymentChannelDto>(paymentChannelDto);
                }
                else
                {
                    return Left<string, PaymentChannelDto>($"Error retrieving paymentChannel");
                }
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError($"JsonException: {ex.Message}");
            _logger.LogError($"Path: {ex.Path}, LineNumber: {ex.LineNumber}, BytePositionInLine: {ex.BytePositionInLine}");
            _logger.LogError(ex.Message, "Error retrieving paymentChannel {PaymentChannelId} with includes", paymentChannelId);
            return Left<string, PaymentChannelDto>($"Error retrieving paymentChannel: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "Error retrieving paymentChannel {PaymentChannelId} with includes", paymentChannelId);
            return Left<string, PaymentChannelDto>($"Error retrieving paymentChannel: {ex.Message}");
        }
        return default;
    }
    public async Task<Either<string, SpecificationPerformanceResponse<PaymentChannelEntityDto>>> RunPaymentChannelPerformanceTests(
        Guid paymentChannelId,
        bool disableCache = false)
    {
        try
        {
            var perfLogger = _logger.ForType<SpecificationPerformanceTracker<PaymentChannel, PaymentChannelEntityDto>>();
            var tracker = new SpecificationPerformanceTracker<PaymentChannel, PaymentChannelEntityDto>(perfLogger);
            var complexityMetrics = new Dictionary<string, SpecificationPerformanceResponse<PaymentChannelEntityDto>.QueryComplexityAnalysis.SpecComplexityMetrics>
            {
                {"PaymentChannelByIdSpec", new SpecificationPerformanceResponse<PaymentChannelEntityDto>.QueryComplexityAnalysis.SpecComplexityMetrics
                    {
                        EstimatedJoinCount = 12,
                        IncludeCount = 12,
                        ProjectionDepth = 2,
                        UsesSplitQuery = true,
                        UsesNoTracking = true,
                        RelationshipDepth = 2,
                        ComplexityLevel = "Medium",
                        CalculatedPropertyCount = 0
                    }
                },
                { "PaymentChannelAdvancedGraphSpecV4",  new SpecificationPerformanceResponse<PaymentChannelEntityDto>.QueryComplexityAnalysis.SpecComplexityMetrics
                    {
                        EstimatedJoinCount = 18,
                        IncludeCount = 3,
                        ProjectionDepth = 4,
                        UsesSplitQuery = true,
                        UsesNoTracking = true,
                        RelationshipDepth = 4,
                        ComplexityLevel = "High",
                        CalculatedPropertyCount = 25
                    }
                },
            };

            tracker.ConfigureEntityMetrics(
                new Dictionary<string, int> { { "PaymentChannelByIdSpec", 150 }, { "PaymentChannelAdvancedGraphSpecV4", 250 } },
                new Dictionary<string, int> { { "PaymentChannelByIdSpec", 12 }, { "PaymentChannelAdvancedGraphSpecV4", 18 } },
                complexityMetrics
            );
            var testConfigurations = new List<(string Name, Func<Guid, ISpecification<PaymentChannel, PaymentChannelEntityDto>> SpecFactory)>
            {
                ("PaymentChannelAdvancedGraphSpecV4", id => (ISpecification<PaymentChannel, PaymentChannelEntityDto>)new PaymentChannelAdvancedGraphSpecV4(id)),
                ("PaymentChannelByIdSpec", id => (ISpecification<PaymentChannel, PaymentChannelEntityDto>)new PCIShield.Domain.Specifications.PaymentChannelByIdSpec(id))
            };
            foreach (var config in testConfigurations)
            {
                for (int i = 0; i < 2; i++)
                {
                    var stopwatch = Stopwatch.StartNew();
                    var spec = config.SpecFactory(paymentChannelId);
                    var includesProperty = spec.GetType().GetProperty("Includes");
                    if (includesProperty != null)
                    {
                        var includes = includesProperty.GetValue(spec) as System.Collections.IEnumerable;
                        if (includes != null)
                        {
                            int count = 0;
                            foreach (var _ in includes)
                                count++;

                            tracker.UpdateMetric(config.Name, "IncludeCount", count);
                        }
                    }
                   tracker.UpdateSpecificationComplexityMetrics<PaymentChannel, PaymentChannelEntityDto>(spec, config.Name, tracker);

                    PaymentChannelEntityDto? paymentChannel = null;

                    if (disableCache)
                    {
                        paymentChannel = await _repositoryFactory.GetRepository<PaymentChannel>().FirstOrDefaultAsync(spec);
                    }
                    else
                    {
                        paymentChannel = await _repositoryCacheFactory.GetReadRedisRepository<PaymentChannel>().FirstOrDefaultAsync(spec);
                    }
                    if (paymentChannel == null)
                    {
                        _logger.LogWarning($"No paymentChannel found for ID {paymentChannelId} using {config.Name}");
                    }

                    stopwatch.Stop();
                    tracker.AddTiming(config.Name, stopwatch.ElapsedMilliseconds);

                    if (paymentChannel != null)
                    {
                        var mapper = new GetByIdPaymentChannelEntityDtoMapper();
                        var paymentChannelDto = mapper.FromEntityDto(paymentChannel);
                    }
                    await Task.Delay(200);
                }
            }

            tracker.LogSummary();

            var response = new SpecificationPerformanceResponse<PaymentChannelEntityDto>
            {
                TimingResults = tracker.GetTimings(),
                Statistics = tracker.CalculateStatistics(),
                ComplexityAnalysis = tracker.GetComplexityAnalysis(),
                ErrorMessage = null
            };
            if (response.Statistics.Count > 0)
            {
                var sortedStats = response.Statistics.OrderBy(s => s.Value.AverageMs).ToList();
                response.FastestSpecification = sortedStats.First().Key;

                if (sortedStats.Count > 1)
                {
                    var mostConsistent = response.Statistics
                        .OrderBy(s => s.Value.VariabilityCoefficient)
                        .First().Key;
                    response.MostConsistentSpecification = mostConsistent;

                    var recommended = response.Statistics
                        .OrderBy(s => s.Value.Rank + s.Value.VariabilityCoefficient * 5)
                        .First().Key;
                    response.RecommendedSpecification = recommended;

                    var fastest = sortedStats.First().Value;
                    var slowest = sortedStats.Last().Value;
                    response.MaximumSpeedupFactor = Math.Round(slowest.AverageMs / fastest.AverageMs, 2);
                }
            }
            return Right<string, SpecificationPerformanceResponse<PaymentChannelEntityDto>>(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "Error running performance tests for paymentChannel {PaymentChannelId}", paymentChannelId);
            return Left<string, SpecificationPerformanceResponse<PaymentChannelEntityDto>>($"Error running performance tests: {ex.Message}");
        }
    }

    public async Task<Either<string, List<PaymentChannelDto>>> SearchPaymentChannelsAsync(string searchTerm)
    {
        try
        {
            Guard.Against.NullOrWhiteSpace(searchTerm, nameof(searchTerm));
            var spec = new PaymentChannelSearchSpec(searchTerm);
            var paymentChannels = await _paymentChannelCacheRepository.ListAsync(spec);
            if (!paymentChannels.Any())
            {
                return new List<PaymentChannelDto>();
            }
            var settings = GJset.GetSystemTextJsonSettings();
            var paymentChannelDtos = paymentChannels.Select(c => JsonSerializer.Deserialize<PaymentChannelDto>(JsonSerializer.Serialize(c, settings), settings)).Where(dto => dto != null).ToList();
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
                convertedSortings.Add(new PCIShieldLib.SharedKernel.Interfaces.Sort { Field = sort.Field, Direction = sort.Direction });
            }
            if (errors.Any())
            {
                return Left<string, ListPaymentChannelResponse>(string.Join("; ", errors));
            }
            var spec = new PaymentChannelAdvancedFilterSpec(req.PageNumber, req.PageSize, req.Filters, convertedSortings);
            var paymentChannels = await _paymentChannelCacheRepository.ListAsync(spec);
            if (!paymentChannels.Any())
            {
                return Right<string, ListPaymentChannelResponse>(new ListPaymentChannelResponse { PaymentChannels = null, Count = 0 });
            }
            var settings = GJset.GetSystemTextJsonSettings();
            var paymentChannelDtos = paymentChannels
                .Skip((req.PageNumber - 1) * req.PageSize)
                .Take(req.PageSize)
                .Select(c => JsonSerializer.Deserialize<PaymentChannelDto>(JsonSerializer.Serialize(c, settings), settings))
                .Where(dto => dto != null)
                .ToList();
            return Right<string, ListPaymentChannelResponse>(new ListPaymentChannelResponse { PaymentChannels = paymentChannelDtos, Count = paymentChannels.Count });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error filtering paymentChannels");
            return Left<string, ListPaymentChannelResponse>($"Error filtering paymentChannels: {ex.Message}");
        }
    }
    public async Task<Either<string, ListPaymentChannelResponse>> GetPaymentChannelPagedList(int pageNumber, int pageSize)
    {
        try
        {
            var spec = new PaymentChannelListPagedSpec(pageNumber, pageSize);

            var pagedResult = await _repositoryCacheFactory.GetReadRedisRepository<PaymentChannel>().GetPagedResultAsync(spec, pageNumber, pageSize);

            if (!pagedResult.Items.Any())
                return Left<string, ListPaymentChannelResponse>("No paymentChannels found.");

            var settings = GJset.GetSystemTextJsonSettings();
            var paymentChannelDtos = pagedResult.Items
                .Select(entityDto =>
                {
                    var jsonPaymentChannel = JsonSerializer.Serialize(entityDto, settings);
                    return JsonSerializer.Deserialize<PaymentChannelDto>(jsonPaymentChannel, settings);
                })
                .Where(dto => dto != null)
                .ToList();

            if (!paymentChannelDtos.Any())
                return Left<string, ListPaymentChannelResponse>("Error mapping paymentChannels.");

            var response = new ListPaymentChannelResponse
            {
                PaymentChannels = paymentChannelDtos,
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize,
                TotalPages = pagedResult.TotalPages
            };

            return Right<string, ListPaymentChannelResponse>(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "Error retrieving paymentChannel list for page {PageNumber} with size {PageSize}",
                pageNumber, pageSize);
            return Left<string, ListPaymentChannelResponse>($"Error retrieving paymentChannel list: {ex.Message}");
        }
    }

    public async Task<Either<string, bool>> DeletePaymentChannelAsync(Guid paymentChannelId)
    {
        try
        {
            Guard.Against.NullOrEmpty(paymentChannelId, nameof(paymentChannelId));
            var paymentChannel = await _paymentChannelRepository.GetByIdAsync(paymentChannelId);
            if (paymentChannel == null)
            {
                return Left<string, bool>("PaymentChannel not found.");
            }
            paymentChannel.SetIsDeleted(true);
            await _paymentChannelRepository.UpdateAsync(paymentChannel);
            await InvalidatePaymentChannelCacheAsync(paymentChannel.PaymentChannelId);
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
            var paymentChannelId = Guid.CreateVersion7();
            var createdDate = DateTime.UtcNow;
            var createdBy = paymentChannelDto.CreatedBy;
            var createPaymentChannelEff = PaymentChannel.Create(
                paymentChannelId: paymentChannelId,
                     merchantId:paymentChannelDto.MerchantId,
                     tenantId:paymentChannelDto.TenantId,
                     channelCode:paymentChannelDto.ChannelCode,
                     channelName:paymentChannelDto.ChannelName,
                     channelType:paymentChannelDto.ChannelType,
                     processingVolume:paymentChannelDto.ProcessingVolume,
                     isInScope:paymentChannelDto.IsInScope,
                     tokenizationEnabled:paymentChannelDto.TokenizationEnabled,
                     createdAt:paymentChannelDto.CreatedAt,
                     createdBy:paymentChannelDto.CreatedBy,
                     isDeleted:paymentChannelDto.IsDeleted
            );

            var validationResult = await _updatePaymentChannelValidator.ValidateAsync(paymentChannelDto);
            if (!validationResult.IsValid)
            {
                var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                return Left<string, PaymentChannelDto>($"Validation failed: {errors}");
            }
            var spec = new PaymentChannelLastCreatedSpec();

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

            var validation = createPaymentChannelEff.Run();
            if (validation.IsFail)
            {
                var errors = string.Join("; ", validation.ToList());
                return Left<string, PaymentChannelDto>($"Error creating paymentChannel: {errors}");
            }
             bool paymentChannel = validation.IsSucc;

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
                {
                    return Left<string, PaymentChannelDto>($"PaymentChannel with ID {paymentChannelDto.PaymentChannelId} not found.");
                }

                var updateResult = PaymentChannel
                    .Update(
                        paymentChannel,
                        paymentChannelDto.PaymentChannelId,
                        paymentChannelDto.TenantId,
                        paymentChannelDto.MerchantId,
                        paymentChannelDto.ChannelCode,
                        paymentChannelDto.ChannelName,
                        paymentChannelDto.ChannelType,
                        paymentChannelDto.ProcessingVolume,
                        paymentChannelDto.IsInScope,
                        paymentChannelDto.TokenizationEnabled,
                        paymentChannelDto.CreatedAt,
                        paymentChannelDto.CreatedBy,
                        paymentChannelDto.UpdatedAt,
                        paymentChannelDto.UpdatedBy,
                       
                        paymentChannelDto.IsDeleted                    )
                    .Run();
                if (updateResult.IsFail)
                {
                    Validation<string, PaymentChannel> validation = updateResult.ToList().FirstOrDefault();
                    return Left<string, PaymentChannelDto>("PaymentChannel update failed domain validation");
                }

                paymentChannel.SetIsDeleted(paymentChannelDto.IsDeleted);
                await _paymentChannelRepository.UpdateAsync(paymentChannel);

                await InvalidatePaymentChannelCacheAsync(paymentChannel.PaymentChannelId);
                _paymentChannelRepository.CommitTransaction();

                var mapper = new GetByIdPaymentChannelFromEntityMapper();
                var updatedPaymentChannelDto = mapper.FromEntity(paymentChannel);

                if (updatedPaymentChannelDto == null)
                {
                    return Left<string, PaymentChannelDto>("Error mapping updated paymentChannel to DTO");
                }

                return Right<string, PaymentChannelDto>(updatedPaymentChannelDto);
            }
            catch (Exception e)
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
        public  PaymentChannelDto? FromEntityDto(PaymentChannelEntityDto paymentChannel)
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
        public async Task<Either<string, PaymentPageDto>> GetOneFullPaymentPageByIdAsync(Guid paymentPageId)
        {
            try
            {
            PaymentPageByIdSpec spec = new(paymentPageId);
                PaymentPageEntityDto? paymentPage = await _paymentPageCacheRepository.FirstOrDefaultAsync(spec);
                GetByIdPaymentPageEntityDtoMapper mapper = new();

                if (paymentPage != null)
                {
                    PaymentPageDto? paymentPageDto = mapper.FromEntityDto(paymentPage);
                    return paymentPageDto != null
                        ? Right<string, PaymentPageDto>(paymentPageDto)
                        : Left<string, PaymentPageDto>("Error mapping paymentPage to DTO");
                }
                return Left<string, PaymentPageDto>("PaymentPage not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error retrieving paymentPage {PaymentPageId}", paymentPageId);
                return Left<string, PaymentPageDto>($"Error retrieving paymentPage: {ex.Message}");
            }
        }

        public async Task<Either<string, List<PaymentPageDto>>> SearchPaymentPagesAsync(string searchTerm)
        {
            try
            {
                Guard.Against.NullOrWhiteSpace(searchTerm, nameof(searchTerm));
                var spec = new PaymentPageSearchSpec(searchTerm);
                var paymentPages = await _paymentPageCacheRepository.ListAsync(spec);

                var settings = GJset.GetSystemTextJsonSettings();
                var paymentPageDtos = paymentPages
                    .Select(i => JsonSerializer.Deserialize<PaymentPageDto>(JsonSerializer.Serialize(i, settings), settings))
                    .Where(dto => dto != null)
                    .ToList();

                return Right<string, List<PaymentPageDto>>(paymentPageDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error searching paymentPages with term: {SearchTerm}", searchTerm);
                return Left<string, List<PaymentPageDto>>($"Error searching paymentPages: {ex.Message}");
            }
        }

        public async Task<Either<string, ListPaymentPageResponse>> GetFilteredPaymentPagesAsync(FilteredPaymentPageRequest req)
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
                    return Left<string, ListPaymentPageResponse>(string.Join("; ", errors));

                var spec = new PaymentPageAdvancedFilterSpec(req.PageNumber, req.PageSize, req.Filters, convertedSortings);
                var paymentPages = await _paymentPageCacheRepository.ListAsync(spec);

                if (!paymentPages.Any())
                    return Right<string, ListPaymentPageResponse>(new ListPaymentPageResponse { PaymentPages = null, Count = 0 });

                var settings = GJset.GetSystemTextJsonSettings();
                var paymentPageDtos = paymentPages
                    .Skip((req.PageNumber - 1) * req.PageSize)
                    .Take(req.PageSize)
                    .Select(i => JsonSerializer.Deserialize<PaymentPageDto>(JsonSerializer.Serialize(i, settings), settings))
                    .Where(dto => dto != null)
                    .ToList();

                return Right<string, ListPaymentPageResponse>(
                    new ListPaymentPageResponse { PaymentPages = paymentPageDtos, Count = paymentPages.Count }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error filtering paymentPages");
                return Left<string, ListPaymentPageResponse>($"Error filtering paymentPages: {ex.Message}");
            }
        }

        public async Task<Either<string, List<PaymentPageDto>>> GetPaymentPagePagedList(int pageNumber, int pageSize)
        {
            try
            {
                var spec = new PaymentPageListPagedSpec(pageNumber, pageSize);
                var paymentPageEntityDtos = await _paymentPageCacheRepository.ListAsync(spec);

                if (!paymentPageEntityDtos.Any())
                    return Left<string, List<PaymentPageDto>>("No paymentPages found.");

                var settings = GJset.GetSystemTextJsonSettings();
                var paymentPageDtos = paymentPageEntityDtos
                    .Select(entityDto =>
                    {
                        var jsonPaymentPage = JsonSerializer.Serialize(entityDto, settings);
                        return JsonSerializer.Deserialize<PaymentPageDto>(jsonPaymentPage, settings);
                    })
                    .Where(dto => dto != null)
                    .ToList();

                return paymentPageDtos.Any()
                    ? Right<string, List<PaymentPageDto>>(paymentPageDtos)
                    : Left<string, List<PaymentPageDto>>("Error mapping paymentPages.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error retrieving paymentPage list for page {PageNumber} with size {PageSize}",
                    pageNumber, pageSize);
                return Left<string, List<PaymentPageDto>>($"Error retrieving paymentPage list: {ex.Message}");
            }
        }

        public async Task<Either<string, bool>> DeletePaymentPageAsync(Guid paymentPageId)
        {
            try
            {
                Guard.Against.NullOrEmpty(paymentPageId, nameof(paymentPageId));

                var paymentPage = await _paymentPageRepository.GetByIdAsync(paymentPageId);

                if (paymentPage == null)
                    return Left<string, bool>("PaymentPage not found.");

                paymentPage.SetIsDeleted(true);
                await _paymentPageRepository.UpdateAsync(paymentPage);
                await InvalidatePaymentPageCacheAsync(paymentPageId);

                return Right<string, bool>(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting paymentPage");
                return Left<string, bool>($"Error deleting paymentPage: {ex.Message}");
            }
        }

        public async Task<Either<string, PaymentPageDto>> GetLastCreatedPaymentPageAsync()
        {
            try
            {
                var spec = new PaymentPageLastCreatedSpec();
                var paymentPage = await _paymentPageRepository.FirstOrDefaultAsync(spec);

                if (paymentPage == null)
                    return Left<string, PaymentPageDto>("No paymentPages found");

                var mapper = new GetByIdPaymentPageFromEntityMapper();
                var paymentPageDto = mapper.FromEntity(paymentPage);

                return paymentPageDto != null
                    ? Right<string, PaymentPageDto>(paymentPageDto)
                    : Left<string, PaymentPageDto>("Error mapping paymentPage to DTO");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving last created paymentPage");
                return Left<string, PaymentPageDto>($"Error retrieving last created paymentPage: {ex.Message}");
            }
        }

        public async Task<Either<string, PaymentPageDto>> CreatePaymentPageAsync(PaymentPageDto paymentPageDto)
        {
            try
            {
                Guard.Against.Null(paymentPageDto, nameof(paymentPageDto));

                var newPaymentPage = new PaymentPage(

                    paymentPageId: Guid.CreateVersion7(),
                      paymentPageDto.PaymentChannelId,
                      paymentPageDto.TenantId,
                      paymentPageDto.PageUrl,
                      paymentPageDto.PageName,
                      paymentPageDto.IsActive,
                      paymentPageDto.CreatedAt,
                      paymentPageDto.CreatedBy,
                      paymentPageDto.IsDeleted

                );

            newPaymentPage.SetLastScriptInventory(paymentPageDto.LastScriptInventory);
            newPaymentPage.SetScriptIntegrityHash(paymentPageDto.ScriptIntegrityHash);
            newPaymentPage.SetUpdatedAt(paymentPageDto.UpdatedAt);
            newPaymentPage.SetIsDeleted(false);

                _paymentPageRepository.BeginTransaction();
                await _paymentPageRepository.AddAsync(newPaymentPage);
                await InvalidatePaymentPageCacheAsync(newPaymentPage.PaymentPageId);
            _paymentPageRepository.CommitTransaction();

                var mapper = new GetByIdPaymentPageFromEntityMapper();
                var createdPaymentPageDto = mapper.FromEntity(newPaymentPage);

                return Right<string, PaymentPageDto>(createdPaymentPageDto);
            }
            catch (Exception ex)
            {
                _paymentPageRepository.RollbackTransaction();
                _logger.LogError(ex, "Error creating paymentPage");
                return Left<string, PaymentPageDto>($"Error creating paymentPage: {ex.Message}");
            }
        }

        public async Task<Either<string, PaymentPageDto>> UpdatePaymentPageAsync(Guid paymentPageId, PaymentPageDto paymentPageDto)
        {
            try
            {
                Guard.Against.NullOrEmpty(paymentPageId, nameof(paymentPageId));
                Guard.Against.Null(paymentPageDto, nameof(paymentPageDto));

                var validationResult = await _updatePaymentPageValidator.ValidateAsync(paymentPageDto);
                if (!validationResult.IsValid)
                {
                    var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                    return Left<string, PaymentPageDto>($"Validation failed: {errors}");
                }

                _paymentPageRepository.BeginTransaction();
                try
                {
                    PaymentPage? paymentPage = await _paymentPageRepository.GetByIdAsync(paymentPageId);

                    if (paymentPage == null)
                        return Left<string, PaymentPageDto>($"PaymentPage with ID {paymentPageDto.PaymentPageId} not found.");

                    var updateResult = new PaymentPage(

                        paymentPageId: paymentPage.PaymentPageId,
                      paymentPageDto.PaymentChannelId,
                      paymentPageDto.TenantId,
                      paymentPageDto.PageUrl,
                      paymentPageDto.PageName,
                      paymentPageDto.IsActive,
                      paymentPageDto.CreatedAt,
                      paymentPageDto.CreatedBy,
                      paymentPageDto.IsDeleted
                    );

                paymentPage.SetIsDeleted(paymentPageDto.IsDeleted);

                    await _paymentPageRepository.UpdateAsync(updateResult);
                    await InvalidatePaymentPageCacheAsync(paymentPageId);
                    _paymentPageRepository.CommitTransaction();

                    var mapper = new GetByIdPaymentPageFromEntityMapper();
                    var updatedPaymentPageDto = mapper.FromEntity(updateResult);

                    return updatedPaymentPageDto != null
                        ? Right<string, PaymentPageDto>(updatedPaymentPageDto)
                        : Left<string, PaymentPageDto>("Error mapping updated paymentPage to DTO");
                }
                catch
                {
                    _paymentPageRepository.RollbackTransaction();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error updating paymentPage {PaymentPageId}", paymentPageDto.PaymentPageId);
                return Left<string, PaymentPageDto>($"Error updating paymentPage: {ex.Message}");
            }
        }
    public class GetByIdPaymentPageEntityDtoMapper : Mapper<GetByIdPaymentPageRequest, GetByIdPaymentPageResponse, PaymentPageDto>
    {
        public   PaymentPageDto? FromEntityDto(PaymentPageEntityDto paymentPage)
        {
            Guard.Against.Null(paymentPage, nameof(paymentPage));
            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            string jsonPaymentPage = JsonSerializer.Serialize(paymentPage, settings);
            PaymentPageDto? paymentPageDto = JsonSerializer.Deserialize<PaymentPageDto>(jsonPaymentPage, settings);
            return paymentPageDto;
        }
    }

    public class GetByIdPaymentPageFromEntityMapper : Mapper<GetByIdPaymentPageRequest, GetByIdPaymentPageResponse, PaymentPageDto>
    {
        public   PaymentPageDto? FromEntity(PaymentPage paymentPage)
        {
            Guard.Against.Null(paymentPage, nameof(paymentPage));
            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            string jsonPaymentPage = JsonSerializer.Serialize(paymentPage, settings);
            PaymentPageDto? paymentPageDto = JsonSerializer.Deserialize<PaymentPageDto>(jsonPaymentPage, settings);
            return paymentPageDto;
        }
    }
    public async Task<Either<string, ListROCPackageResponse>> GetFilteredROCPackagesAsync(FilteredROCPackageRequest req)
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
                return Left<string, ListROCPackageResponse>(string.Join("; ", errors));

            var spec = new ROCPackageAdvancedFilterSpec(req.PageNumber, req.PageSize, req.Filters, convertedSortings);
            var rocpackages = await _repositoryCacheFactory.GetReadRedisRepository<ROCPackage>().ListAsync(spec);

            if (!rocpackages.Any())
                return Right<string, ListROCPackageResponse>(new ListROCPackageResponse { ROCPackages = null, Count = 0 });

            var settings = GJset.GetSystemTextJsonSettings();
            var rocpackageDtos = rocpackages
                .Skip((req.PageNumber - 1) * req.PageSize)
                .Take(req.PageSize)
                .Select(c => JsonSerializer.Deserialize<ROCPackageDto>(JsonSerializer.Serialize(c, settings), settings))
                .Where(dto => dto != null)
                .ToList();

            return Right<string, ListROCPackageResponse>(
                new ListROCPackageResponse { ROCPackages = rocpackageDtos, Count = rocpackages.Count }
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error filtering rocpackages");
            return Left<string, ListROCPackageResponse>($"Error filtering rocpackages: {ex.Message}");
        }
    }
    public async Task<Either<string, List<ROCPackageDto>>> GetROCPackagePagedList(int pageNumber,
        int pageSize,
        Guid? assessmentId
        )
    {
        try
        {
            var spec = new ROCPackageListPagedSpec(pageNumber,
        pageSize);
        if (assessmentId != null && assessmentId != Guid.Empty)
        {
        _ = spec.Query.Where(a => a.AssessmentId == assessmentId.Value);
        }
            var rocpackageEntityDtos = await _repositoryCacheFactory.GetReadRedisRepository<ROCPackage>().ListAsync(spec);

            if (!rocpackageEntityDtos.Any())
                return Left<string, List<ROCPackageDto>>("No rocpackages found.");

            var settings = GJset.GetSystemTextJsonSettings();
            var rocpackageDtos = rocpackageEntityDtos
                .Select(entityDto =>
                {
                    var jsonROCPackage = JsonSerializer.Serialize(entityDto, settings);
                    return JsonSerializer.Deserialize<ROCPackageDto>(jsonROCPackage, settings);
                })
                .Where(dto => dto != null)
                .ToList();

            return rocpackageDtos.Any()
                ? Right<string, List<ROCPackageDto>>(rocpackageDtos)
                : Left<string, List<ROCPackageDto>>("Error mapping rocpackages.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "Error retrieving rocpackage list for page {PageNumber} with size {PageSize}",
                pageNumber, pageSize);
            return Left<string, List<ROCPackageDto>>($"Error retrieving rocpackage list: {ex.Message}");
        }
    }
    public async Task<Either<string, List<ROCPackageDto>>> SearchROCPackagesAsync(string searchTerm)
    {
        try
        {
            Guard.Against.NullOrWhiteSpace(searchTerm, nameof(searchTerm));
            var spec = new ROCPackageSearchSpec(searchTerm);

            var rocpackages = await _repositoryCacheFactory.GetReadRedisRepository<ROCPackage>().ListAsync(spec);

            var settings = GJset.GetSystemTextJsonSettings();
            var rocpackageDtos = rocpackages
                .Select(c => JsonSerializer.Deserialize<ROCPackageDto>(JsonSerializer.Serialize(c, settings), settings))
                .Where(dto => dto != null)
                .ToList();

            return Right<string, List<ROCPackageDto>>(rocpackageDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "Error searching rocpackages with term: {SearchTerm}", searchTerm);
            return Left<string, List<ROCPackageDto>>($"Error searching rocpackages: {ex.Message}");
        }
    }
    public async Task<Either<string, ListScanScheduleResponse>> GetFilteredScanSchedulesAsync(FilteredScanScheduleRequest req)
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
                return Left<string, ListScanScheduleResponse>(string.Join("; ", errors));

            var spec = new ScanScheduleAdvancedFilterSpec(req.PageNumber, req.PageSize, req.Filters, convertedSortings);
            var scanSchedules = await _repositoryCacheFactory.GetReadRedisRepository<ScanSchedule>().ListAsync(spec);

            if (!scanSchedules.Any())
                return Right<string, ListScanScheduleResponse>(new ListScanScheduleResponse { ScanSchedules = null, Count = 0 });

            var settings = GJset.GetSystemTextJsonSettings();
            var scanScheduleDtos = scanSchedules
                .Skip((req.PageNumber - 1) * req.PageSize)
                .Take(req.PageSize)
                .Select(c => JsonSerializer.Deserialize<ScanScheduleDto>(JsonSerializer.Serialize(c, settings), settings))
                .Where(dto => dto != null)
                .ToList();

            return Right<string, ListScanScheduleResponse>(
                new ListScanScheduleResponse { ScanSchedules = scanScheduleDtos, Count = scanSchedules.Count }
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error filtering scanSchedules");
            return Left<string, ListScanScheduleResponse>($"Error filtering scanSchedules: {ex.Message}");
        }
    }
    public async Task<Either<string, List<ScanScheduleDto>>> GetScanSchedulePagedList(int pageNumber,
        int pageSize,
        Guid? assetId
        )
    {
        try
        {
            var spec = new ScanScheduleListPagedSpec(pageNumber,
        pageSize);
        if (assetId != null && assetId != Guid.Empty)
        {
        _ = spec.Query.Where(a => a.AssetId == assetId.Value);
        }
            var scanScheduleEntityDtos = await _repositoryCacheFactory.GetReadRedisRepository<ScanSchedule>().ListAsync(spec);

            if (!scanScheduleEntityDtos.Any())
                return Left<string, List<ScanScheduleDto>>("No scanSchedules found.");

            var settings = GJset.GetSystemTextJsonSettings();
            var scanScheduleDtos = scanScheduleEntityDtos
                .Select(entityDto =>
                {
                    var jsonScanSchedule = JsonSerializer.Serialize(entityDto, settings);
                    return JsonSerializer.Deserialize<ScanScheduleDto>(jsonScanSchedule, settings);
                })
                .Where(dto => dto != null)
                .ToList();

            return scanScheduleDtos.Any()
                ? Right<string, List<ScanScheduleDto>>(scanScheduleDtos)
                : Left<string, List<ScanScheduleDto>>("Error mapping scanSchedules.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "Error retrieving scanSchedule list for page {PageNumber} with size {PageSize}",
                pageNumber, pageSize);
            return Left<string, List<ScanScheduleDto>>($"Error retrieving scanSchedule list: {ex.Message}");
        }
    }
    public async Task<Either<string, List<ScanScheduleDto>>> SearchScanSchedulesAsync(string searchTerm)
    {
        try
        {
            Guard.Against.NullOrWhiteSpace(searchTerm, nameof(searchTerm));
            var spec = new ScanScheduleSearchSpec(searchTerm);

            var scanSchedules = await _repositoryCacheFactory.GetReadRedisRepository<ScanSchedule>().ListAsync(spec);

            var settings = GJset.GetSystemTextJsonSettings();
            var scanScheduleDtos = scanSchedules
                .Select(c => JsonSerializer.Deserialize<ScanScheduleDto>(JsonSerializer.Serialize(c, settings), settings))
                .Where(dto => dto != null)
                .ToList();

            return Right<string, List<ScanScheduleDto>>(scanScheduleDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "Error searching scanSchedules with term: {SearchTerm}", searchTerm);
            return Left<string, List<ScanScheduleDto>>($"Error searching scanSchedules: {ex.Message}");
        }
    }
    public async Task<Either<string, ListVulnerabilityResponse>> GetFilteredVulnerabilitiesAsync(FilteredVulnerabilityRequest req)
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
                return Left<string, ListVulnerabilityResponse>(string.Join("; ", errors));

            var spec = new VulnerabilityAdvancedFilterSpec(req.PageNumber, req.PageSize, req.Filters, convertedSortings);
            var vulnerabilities = await _repositoryCacheFactory.GetReadRedisRepository<Vulnerability>().ListAsync(spec);

            if (!vulnerabilities.Any())
                return Right<string, ListVulnerabilityResponse>(new ListVulnerabilityResponse { Vulnerabilities = null, Count = 0 });

            var settings = GJset.GetSystemTextJsonSettings();
            var vulnerabilityDtos = vulnerabilities
                .Skip((req.PageNumber - 1) * req.PageSize)
                .Take(req.PageSize)
                .Select(c => JsonSerializer.Deserialize<VulnerabilityDto>(JsonSerializer.Serialize(c, settings), settings))
                .Where(dto => dto != null)
                .ToList();

            return Right<string, ListVulnerabilityResponse>(
                new ListVulnerabilityResponse { Vulnerabilities = vulnerabilityDtos, Count = vulnerabilities.Count }
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error filtering vulnerabilities");
            return Left<string, ListVulnerabilityResponse>($"Error filtering vulnerabilities: {ex.Message}");
        }
    }
    public async Task<Either<string, List<VulnerabilityDto>>> GetVulnerabilityPagedList(int pageNumber,
        int pageSize,
        Guid? assetId
        )
    {
        try
        {
            var spec = new VulnerabilityListPagedSpec(pageNumber,
        pageSize);
        if (assetId != null && assetId != Guid.Empty)
        {
        _ = spec.Query.Where(a => a.AssetId == assetId.Value);
        }
            var vulnerabilityEntityDtos = await _repositoryCacheFactory.GetReadRedisRepository<Vulnerability>().ListAsync(spec);

            if (!vulnerabilityEntityDtos.Any())
                return Left<string, List<VulnerabilityDto>>("No vulnerabilities found.");

            var settings = GJset.GetSystemTextJsonSettings();
            var vulnerabilityDtos = vulnerabilityEntityDtos
                .Select(entityDto =>
                {
                    var jsonVulnerability = JsonSerializer.Serialize(entityDto, settings);
                    return JsonSerializer.Deserialize<VulnerabilityDto>(jsonVulnerability, settings);
                })
                .Where(dto => dto != null)
                .ToList();

            return vulnerabilityDtos.Any()
                ? Right<string, List<VulnerabilityDto>>(vulnerabilityDtos)
                : Left<string, List<VulnerabilityDto>>("Error mapping vulnerabilities.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "Error retrieving vulnerability list for page {PageNumber} with size {PageSize}",
                pageNumber, pageSize);
            return Left<string, List<VulnerabilityDto>>($"Error retrieving vulnerability list: {ex.Message}");
        }
    }
    public async Task<Either<string, List<VulnerabilityDto>>> SearchVulnerabilitiesAsync(string searchTerm)
    {
        try
        {
            Guard.Against.NullOrWhiteSpace(searchTerm, nameof(searchTerm));
            var spec = new VulnerabilitySearchSpec(searchTerm);

            var vulnerabilities = await _repositoryCacheFactory.GetReadRedisRepository<Vulnerability>().ListAsync(spec);

            var settings = GJset.GetSystemTextJsonSettings();
            var vulnerabilityDtos = vulnerabilities
                .Select(c => JsonSerializer.Deserialize<VulnerabilityDto>(JsonSerializer.Serialize(c, settings), settings))
                .Where(dto => dto != null)
                .ToList();

            return Right<string, List<VulnerabilityDto>>(vulnerabilityDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "Error searching vulnerabilities with term: {SearchTerm}", searchTerm);
            return Left<string, List<VulnerabilityDto>>($"Error searching vulnerabilities: {ex.Message}");
        }
    }

    public async Task<Either<string, MerchantDto>> GetOneFullMerchantByIdAsync(Guid merchantId)
    {
        try
        {
            MerchantByIdSpec spec = new(merchantId);
            MerchantEntityDto? merchant = await _merchantCacheRepository.FirstOrDefaultAsync(spec);
            GetByIdMerchantEntityDtoMapper mapper = new();

            if (merchant != null)
            {
                MerchantDto? merchantDto = mapper.FromEntityDto(merchant);
                return merchantDto != null
                    ? Right<string, MerchantDto>(merchantDto)
                    : Left<string, MerchantDto>("Error mapping merchant to DTO");
            }
            return Left<string, MerchantDto>("Merchant not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "Error retrieving merchant {MerchantId}", merchantId);
            return Left<string, MerchantDto>($"Error retrieving merchant: {ex.Message}");
        }
    }

    public async Task<Either<string, List<MerchantDto>>> SearchMerchantsAsync(string searchTerm)
    {
        try
        {
            Guard.Against.NullOrWhiteSpace(searchTerm, nameof(searchTerm));
            var spec = new MerchantSearchSpec(searchTerm);
            var merchants = await _merchantCacheRepository.ListAsync(spec);

            var settings = GJset.GetSystemTextJsonSettings();
            var merchantDtos = merchants
                .Select(c => JsonSerializer.Deserialize<MerchantDto>(JsonSerializer.Serialize(c, settings), settings))
                .Where(dto => dto != null)
                .ToList();

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
                convertedSortings.Add(
                    new PCIShieldLib.SharedKernel.Interfaces.Sort { Field = sort.Field, Direction = sort.Direction }
                );
            }

            if (errors.Any())
                return Left<string, ListMerchantResponse>(string.Join("; ", errors));

            var spec = new MerchantAdvancedFilterSpec(req.PageNumber, req.PageSize, req.Filters, convertedSortings);
            var merchants = await _merchantCacheRepository.ListAsync(spec);

            if (!merchants.Any())
                return Right<string, ListMerchantResponse>(new ListMerchantResponse { Merchants = null, Count = 0 });

            var settings = GJset.GetSystemTextJsonSettings();
            var merchantDtos = merchants
                .Skip((req.PageNumber - 1) * req.PageSize)
                .Take(req.PageSize)
                .Select(c => JsonSerializer.Deserialize<MerchantDto>(JsonSerializer.Serialize(c, settings), settings))
                .Where(dto => dto != null)
                .ToList();

            return Right<string, ListMerchantResponse>(
                new ListMerchantResponse { Merchants = merchantDtos, Count = merchants.Count }
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error filtering merchants");
            return Left<string, ListMerchantResponse>($"Error filtering merchants: {ex.Message}");
        }
    }
    public async Task<Either<string, List<MerchantDto>>> GetMerchantPagedList(int pageNumber, int pageSize)
    {
        try
        {
            var spec = new MerchantListPagedSpec(pageNumber, pageSize);
            var merchantEntityDtos = await _merchantCacheRepository.ListAsync(spec);

            if (!merchantEntityDtos.Any())
                return Left<string, List<MerchantDto>>("No merchants found.");

            var settings = GJset.GetSystemTextJsonSettings();
            var merchantDtos = merchantEntityDtos
                .Select(entityDto =>
                {
                    var jsonMerchant = JsonSerializer.Serialize(entityDto, settings);
                    return JsonSerializer.Deserialize<MerchantDto>(jsonMerchant, settings);
                })
                .Where(dto => dto != null)
                .ToList();

            return merchantDtos.Any()
                ? Right<string, List<MerchantDto>>(merchantDtos)
                : Left<string, List<MerchantDto>>("Error mapping merchants.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "Error retrieving merchant list for page {PageNumber} with size {PageSize}",
                pageNumber, pageSize);
            return Left<string, List<MerchantDto>>($"Error retrieving merchant list: {ex.Message}");
        }
    }

    public async Task<Either<string, bool>> DeleteMerchantAsync(Guid merchantId)
    {
        try
        {
            Guard.Against.NullOrEmpty(merchantId, nameof(merchantId));
            var merchant = await _merchantRepository.GetByIdAsync(merchantId);

            if (merchant == null)
                return Left<string, bool>("Merchant not found.");

            merchant.SetIsDeleted(true);
            await _merchantRepository.UpdateAsync(merchant);
            await InvalidateMerchantCacheAsync(merchantId);

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
            var merchant = await _merchantRepository.FirstOrDefaultAsync(spec);

            if (merchant == null)
                return Left<string, MerchantDto>("No merchants found");

            var mapper = new GetByIdMerchantFromEntityMapper();
            var merchantDto = mapper.FromEntity(merchant);

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
                    return Left<string, MerchantDto>($"Merchant with ID {merchantDto.MerchantId} not found.");

                var updateResult = new Merchant(
                    merchantId: merchant.MerchantId,
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
                      merchantDto.IsDeleted                );

                updateResult.SetIsDeleted(merchantDto.IsDeleted);

                await _merchantRepository.UpdateAsync(updateResult);
                await InvalidateMerchantCacheAsync(merchantId);
                _merchantRepository.CommitTransaction();

                var mapper = new GetByIdMerchantFromEntityMapper();
                var updatedMerchantDto = mapper.FromEntity(updateResult);

                return updatedMerchantDto != null
                    ? Right<string, MerchantDto>(updatedMerchantDto)
                    : Left<string, MerchantDto>("Error mapping updated merchant to DTO");
            }
            catch
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
        public   MerchantDto? FromEntityDto(MerchantEntityDto merchant)
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

  }
