using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

using Ardalis.GuardClauses;

using BlazorMauiShared.Models.Assessment;

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

using BlazorMauiShared.Models.Assessment;

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
using BlazorMauiShared.Models.ROCPackage;
using BlazorMauiShared.Models.ScanSchedule;
using BlazorMauiShared.Models.Vulnerability;
using BlazorMauiShared.Models.PaymentPage;
using BlazorMauiShared.Models.Script;
using BlazorMauiShared.Models.Merchant;
using BlazorMauiShared.Models.AssessmentControl;
using BlazorMauiShared.Models.Control;
using BlazorMauiShared.Models.ControlEvidence;
using BlazorMauiShared.Models.Evidence;

public class AssessmentService
{

    private readonly IMediator _mediator;
    private readonly IRepositoryFactory _repositoryFactory;
    private readonly IReadRedisRepositoryFactory _repositoryCacheFactory;
    private readonly IElasticsearchService<Assessment> _assessmentIndexService;

    private readonly IRepository<ROCPackage> _rocpackageRepository;
    private readonly IReadRedisRepository<ROCPackage> _rocpackageCacheRepository;
    private readonly IRepository<Merchant> _merchantRepository;
    private readonly IReadRedisRepository<Merchant> _merchantCacheRepository;
    private readonly IRepository<AssessmentControl> _assessmentControlRepository;
    private readonly IReadRedisRepository<AssessmentControl> _assessmentControlCacheRepository;
    private readonly IRepository<Control> _controlRepository;
    private readonly IReadRedisRepository<Control> _controlCacheRepository;
    private readonly IRepository<ControlEvidence> _controlEvidenceRepository;
    private readonly IReadRedisRepository<ControlEvidence> _controlEvidenceCacheRepository;
    private readonly IRepository<Evidence> _evidenceRepository;
    private readonly IReadRedisRepository<Evidence> _evidenceCacheRepository;
    private readonly IReadRedisRepository<Assessment> _assessmentCacheRepository;
    private readonly IRepository<Assessment> _assessmentRepository;
    private readonly IKeyCloakTenantService _keyCloakTenantService;
    private readonly IAppLoggerService<AssessmentService> _logger;
    private readonly IRedisCacheService _redisCacheService;
    private readonly IValidator<AssessmentDto> _updateAssessmentValidator;
    private readonly IValidator<ROCPackageDto> _updateROCPackageValidator;
    private readonly IValidator<MerchantDto> _updateMerchantValidator;
    private readonly IValidator<AssessmentControlDto> _updateAssessmentControlValidator;
    private readonly IValidator<ControlEvidenceDto> _updateControlEvidenceValidator;

    public AssessmentService(

        IRepositoryFactory repositoryFactory,
        IReadRedisRepositoryFactory repositoryCacheFactory,
        IMediator mediator,
  
        IElasticsearchService<Assessment> assessmentIndexService,

        IReadRedisRepository<Assessment> assessmentCacheRepository,
        IRedisCacheService redisCacheService,
        IKeyCloakTenantService keyCloakTenantService,
        IRepository<Assessment> assessmentRepository,
        IAppLoggerService<AssessmentService> logger,
        IRepository<ROCPackage> rocpackageRepository,
        IReadRedisRepository<ROCPackage> rocpackageCacheRepository,
        IRepository<Merchant> merchantRepository,
        IReadRedisRepository<Merchant> merchantCacheRepository,
        IRepository<AssessmentControl> assessmentControlRepository,
        IReadRedisRepository<AssessmentControl> assessmentControlCacheRepository,
        IRepository<Control> controlRepository,
        IReadRedisRepository<Control> controlCacheRepository,
        IRepository<ControlEvidence> controlEvidenceRepository,
        IReadRedisRepository<ControlEvidence> controlEvidenceCacheRepository,
        IRepository<Evidence> evidenceRepository,
        IReadRedisRepository<Evidence> evidenceCacheRepository
    )
    {

        _repositoryFactory = repositoryFactory;
        _repositoryCacheFactory = repositoryCacheFactory;
        _assessmentIndexService = assessmentIndexService;
        _mediator = mediator;
        _updateAssessmentValidator = new InlineValidator<AssessmentDto>();
        _updateROCPackageValidator = new InlineValidator<ROCPackageDto>();
        _updateMerchantValidator = new InlineValidator<MerchantDto>();
        _updateAssessmentControlValidator = new InlineValidator<AssessmentControlDto>();
        _updateControlEvidenceValidator = new InlineValidator<ControlEvidenceDto>();
        _assessmentCacheRepository = assessmentCacheRepository;
        _redisCacheService = redisCacheService;
        _keyCloakTenantService = keyCloakTenantService;
        _assessmentRepository = assessmentRepository;
        _logger = logger;
        _rocpackageRepository = rocpackageRepository;
        _rocpackageCacheRepository = rocpackageCacheRepository;
        _merchantRepository = merchantRepository;
        _merchantCacheRepository = merchantCacheRepository;
        _assessmentControlRepository = assessmentControlRepository;
        _assessmentControlCacheRepository = assessmentControlCacheRepository;
        _controlRepository = controlRepository;
        _controlCacheRepository = controlCacheRepository;
        _controlEvidenceRepository = controlEvidenceRepository;
        _controlEvidenceCacheRepository = controlEvidenceCacheRepository;
        _evidenceRepository = evidenceRepository;
        _evidenceCacheRepository = evidenceCacheRepository;
    }

    private async Task InvalidateROCPackageCacheAsync(Guid rocpackageId)
    {
        var tenantId = _keyCloakTenantService.GetPCIShieldSolutionuperUserId();
        await _redisCacheService.RemoveByPatternAsync($"ROCPackageListPagedSpec--{tenantId}-*");
        await _redisCacheService.RemoveByPatternAsync(
            $"ROCPackageByIdSpec-{rocpackageId.ToString()}--{tenantId}-FirstOrDefaultAsync-ROCPackageEntityDto"
        );
        await _redisCacheService.RemoveByPatternAsync(
            $"ROCPackageListPagedSpec-1-10--{tenantId}-ListAsync-ROCPackageEntityDto"
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

    private async Task InvalidateAssessmentControlCacheAsync(Guid assessmentId, Guid controlId)
    {
        var tenantId = _keyCloakTenantService.GetPCIShieldSolutionuperUserId();
        await _redisCacheService.RemoveByPatternAsync($"AssessmentControlListPagedSpec--{tenantId}-*");
        await _redisCacheService.RemoveByPatternAsync(
            $"AssessmentControlByIdSpec-{assessmentId}-{controlId}--{tenantId}-FirstOrDefaultAsync-AssessmentControlEntityDto"
        );
        await _redisCacheService.RemoveByPatternAsync(
            $"AssessmentControlListPagedSpec-1-10--{tenantId}-ListAsync-AssessmentControlEntityDto"
        );
    }

    private async Task InvalidateControlEvidenceCacheAsync(Guid assessmentId, Guid evidenceId)
    {
        var tenantId = _keyCloakTenantService.GetPCIShieldSolutionuperUserId();
        await _redisCacheService.RemoveByPatternAsync($"ControlEvidenceListPagedSpec--{tenantId}-*");
        await _redisCacheService.RemoveByPatternAsync(
            $"ControlEvidenceByIdSpec-{assessmentId}-{evidenceId}--{tenantId}-FirstOrDefaultAsync-ControlEvidenceEntityDto"
        );
        await _redisCacheService.RemoveByPatternAsync(
            $"ControlEvidenceListPagedSpec-1-10--{tenantId}-ListAsync-ControlEvidenceEntityDto"
        );
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

    public async Task<Either<string, AssessmentDto>> GetOneFullAssessmentByIdAsync(Guid assessmentId, bool withPostGraph = false)
    {
        try
        {
            AssessmentAdvancedGraphSpecV4 spec = new(assessmentId);
            AssessmentEntityDto? assessment = await _repositoryCacheFactory.GetReadRedisRepository<Assessment>().FirstOrDefaultAsync(spec);
            GetByIdAssessmentEntityDtoMapper mapper = new();
            if (assessment != null)
            {
                if (withPostGraph)
                {
                }
                AssessmentDto? assessmentDto = mapper.FromEntityDto(assessment);
                if (assessmentDto != null)
                {
                    return Right<string, AssessmentDto>(assessmentDto);
                }
                else
                {
                    return Left<string, AssessmentDto>($"Error retrieving assessment");
                }
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError($"JsonException: {ex.Message}");
            _logger.LogError($"Path: {ex.Path}, LineNumber: {ex.LineNumber}, BytePositionInLine: {ex.BytePositionInLine}");
            _logger.LogError(ex.Message, "Error retrieving assessment {AssessmentId} with includes", assessmentId);
            return Left<string, AssessmentDto>($"Error retrieving assessment: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "Error retrieving assessment {AssessmentId} with includes", assessmentId);
            return Left<string, AssessmentDto>($"Error retrieving assessment: {ex.Message}");
        }
        return default;
    }
    public async Task<Either<string, SpecificationPerformanceResponse<AssessmentEntityDto>>> RunAssessmentPerformanceTests(
        Guid assessmentId,
        bool disableCache = false)
    {
        try
        {
            var perfLogger = _logger.ForType<SpecificationPerformanceTracker<Assessment, AssessmentEntityDto>>();
            var tracker = new SpecificationPerformanceTracker<Assessment, AssessmentEntityDto>(perfLogger);
            var complexityMetrics = new Dictionary<string, SpecificationPerformanceResponse<AssessmentEntityDto>.QueryComplexityAnalysis.SpecComplexityMetrics>
            {
                {"AssessmentByIdSpec", new SpecificationPerformanceResponse<AssessmentEntityDto>.QueryComplexityAnalysis.SpecComplexityMetrics
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
                { "AssessmentAdvancedGraphSpecV4",  new SpecificationPerformanceResponse<AssessmentEntityDto>.QueryComplexityAnalysis.SpecComplexityMetrics
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
                new Dictionary<string, int> { { "AssessmentByIdSpec", 150 }, { "AssessmentAdvancedGraphSpecV4", 250 } },
                new Dictionary<string, int> { { "AssessmentByIdSpec", 12 }, { "AssessmentAdvancedGraphSpecV4", 18 } },
                complexityMetrics
            );
            var testConfigurations = new List<(string Name, Func<Guid, ISpecification<Assessment, AssessmentEntityDto>> SpecFactory)>
            {
                ("AssessmentAdvancedGraphSpecV4", id => (ISpecification<Assessment, AssessmentEntityDto>)new AssessmentAdvancedGraphSpecV4(id)),
                ("AssessmentByIdSpec", id => (ISpecification<Assessment, AssessmentEntityDto>)new PCIShield.Domain.Specifications.AssessmentByIdSpec(id))
            };
            foreach (var config in testConfigurations)
            {
                for (int i = 0; i < 2; i++)
                {
                    var stopwatch = Stopwatch.StartNew();
                    var spec = config.SpecFactory(assessmentId);
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
                   tracker.UpdateSpecificationComplexityMetrics<Assessment, AssessmentEntityDto>(spec, config.Name, tracker);

                    AssessmentEntityDto? assessment = null;

                    if (disableCache)
                    {
                        assessment = await _repositoryFactory.GetRepository<Assessment>().FirstOrDefaultAsync(spec);
                    }
                    else
                    {
                        assessment = await _repositoryCacheFactory.GetReadRedisRepository<Assessment>().FirstOrDefaultAsync(spec);
                    }
                    if (assessment == null)
                    {
                        _logger.LogWarning($"No assessment found for ID {assessmentId} using {config.Name}");
                    }

                    stopwatch.Stop();
                    tracker.AddTiming(config.Name, stopwatch.ElapsedMilliseconds);

                    if (assessment != null)
                    {
                        var mapper = new GetByIdAssessmentEntityDtoMapper();
                        var assessmentDto = mapper.FromEntityDto(assessment);
                    }
                    await Task.Delay(200);
                }
            }

            tracker.LogSummary();

            var response = new SpecificationPerformanceResponse<AssessmentEntityDto>
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
            return Right<string, SpecificationPerformanceResponse<AssessmentEntityDto>>(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "Error running performance tests for assessment {AssessmentId}", assessmentId);
            return Left<string, SpecificationPerformanceResponse<AssessmentEntityDto>>($"Error running performance tests: {ex.Message}");
        }
    }

    public async Task<Either<string, List<AssessmentDto>>> SearchAssessmentsAsync(string searchTerm)
    {
        try
        {
            Guard.Against.NullOrWhiteSpace(searchTerm, nameof(searchTerm));
            var spec = new AssessmentSearchSpec(searchTerm);
            var assessments = await _assessmentCacheRepository.ListAsync(spec);
            if (!assessments.Any())
            {
                return new List<AssessmentDto>();
            }
            var settings = GJset.GetSystemTextJsonSettings();
            var assessmentDtos = assessments.Select(c => JsonSerializer.Deserialize<AssessmentDto>(JsonSerializer.Serialize(c, settings), settings)).Where(dto => dto != null).ToList();
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
                convertedSortings.Add(new PCIShieldLib.SharedKernel.Interfaces.Sort { Field = sort.Field, Direction = sort.Direction });
            }
            if (errors.Any())
            {
                return Left<string, ListAssessmentResponse>(string.Join("; ", errors));
            }
            var spec = new AssessmentAdvancedFilterSpec(req.PageNumber, req.PageSize, req.Filters, convertedSortings);
            var assessments = await _assessmentCacheRepository.ListAsync(spec);
            if (!assessments.Any())
            {
                return Right<string, ListAssessmentResponse>(new ListAssessmentResponse { Assessments = null, Count = 0 });
            }
            var settings = GJset.GetSystemTextJsonSettings();
            var assessmentDtos = assessments
                .Skip((req.PageNumber - 1) * req.PageSize)
                .Take(req.PageSize)
                .Select(c => JsonSerializer.Deserialize<AssessmentDto>(JsonSerializer.Serialize(c, settings), settings))
                .Where(dto => dto != null)
                .ToList();
            return Right<string, ListAssessmentResponse>(new ListAssessmentResponse { Assessments = assessmentDtos, Count = assessments.Count });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error filtering assessments");
            return Left<string, ListAssessmentResponse>($"Error filtering assessments: {ex.Message}");
        }
    }
    public async Task<Either<string, ListAssessmentResponse>> GetAssessmentPagedList(int pageNumber, int pageSize)
    {
        try
        {
            var spec = new AssessmentListPagedSpec(pageNumber, pageSize);

            var pagedResult = await _repositoryCacheFactory.GetReadRedisRepository<Assessment>().GetPagedResultAsync(spec, pageNumber, pageSize);

            if (!pagedResult.Items.Any())
                return Left<string, ListAssessmentResponse>("No assessments found.");

            var settings = GJset.GetSystemTextJsonSettings();
            var assessmentDtos = pagedResult.Items
                .Select(entityDto =>
                {
                    var jsonAssessment = JsonSerializer.Serialize(entityDto, settings);
                    return JsonSerializer.Deserialize<AssessmentDto>(jsonAssessment, settings);
                })
                .Where(dto => dto != null)
                .ToList();

            if (!assessmentDtos.Any())
                return Left<string, ListAssessmentResponse>("Error mapping assessments.");

            var response = new ListAssessmentResponse
            {
                Assessments = assessmentDtos,
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize,
                TotalPages = pagedResult.TotalPages
            };

            return Right<string, ListAssessmentResponse>(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "Error retrieving assessment list for page {PageNumber} with size {PageSize}",
                pageNumber, pageSize);
            return Left<string, ListAssessmentResponse>($"Error retrieving assessment list: {ex.Message}");
        }
    }

    public async Task<Either<string, bool>> DeleteAssessmentAsync(Guid assessmentId)
    {
        try
        {
            Guard.Against.NullOrEmpty(assessmentId, nameof(assessmentId));
            var assessment = await _assessmentRepository.GetByIdAsync(assessmentId);
            if (assessment == null)
            {
                return Left<string, bool>("Assessment not found.");
            }
            assessment.SetIsDeleted(true);
            await _assessmentRepository.UpdateAsync(assessment);
            await InvalidateAssessmentCacheAsync(assessment.AssessmentId);
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
            var assessmentId = Guid.CreateVersion7();
            var createdDate = DateTime.UtcNow;
            var createdBy = assessmentDto.CreatedBy;
            var createAssessmentEff = Assessment.Create(
                assessmentId: assessmentId,
                     merchantId:assessmentDto.MerchantId,
                     tenantId:assessmentDto.TenantId,
                     assessmentCode:assessmentDto.AssessmentCode,
                     assessmentType:assessmentDto.AssessmentType,
                     assessmentPeriod:assessmentDto.AssessmentPeriod,
                     startDate:assessmentDto.StartDate,
                     endDate:assessmentDto.EndDate,
                     rank:assessmentDto.Rank,
                     qsareviewRequired:assessmentDto.QSAReviewRequired,
                     createdAt:assessmentDto.CreatedAt,
                     createdBy:assessmentDto.CreatedBy,
                     isDeleted:assessmentDto.IsDeleted
            );

            var validationResult = await _updateAssessmentValidator.ValidateAsync(assessmentDto);
            if (!validationResult.IsValid)
            {
                var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                return Left<string, AssessmentDto>($"Validation failed: {errors}");
            }
            var spec = new AssessmentLastCreatedSpec();

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

            var validation = createAssessmentEff.Run();
            if (validation.IsFail)
            {
                var errors = string.Join("; ", validation.ToList());
                return Left<string, AssessmentDto>($"Error creating assessment: {errors}");
            }
             bool assessment = validation.IsSucc;

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
                {
                    return Left<string, AssessmentDto>($"Assessment with ID {assessmentDto.AssessmentId} not found.");
                }

                var updateResult = Assessment
                    .Update(
                        assessment,
                        assessmentDto.AssessmentId,
                        assessmentDto.TenantId,
                        assessmentDto.MerchantId,
                        assessmentDto.AssessmentCode,
                        assessmentDto.AssessmentType,
                        assessmentDto.AssessmentPeriod,
                        assessmentDto.StartDate,
                        assessmentDto.EndDate,
                        assessmentDto.CompletionDate,
                        assessmentDto.Rank,
                        assessmentDto.ComplianceScore,
                        assessmentDto.QSAReviewRequired,
                        assessmentDto.CreatedAt,
                        assessmentDto.CreatedBy,
                        assessmentDto.UpdatedAt,
                        assessmentDto.UpdatedBy,
                        assessmentDto.IsDeleted                    )
                    .Run();
                if (updateResult.IsFail)
                {
                    Validation<string, Assessment> validation = updateResult.ToList().FirstOrDefault();
                    return Left<string, AssessmentDto>("Assessment update failed domain validation");
                }

                assessment.SetIsDeleted(assessmentDto.IsDeleted);
                await _assessmentRepository.UpdateAsync(assessment);

                await InvalidateAssessmentCacheAsync(assessment.AssessmentId);
                _assessmentRepository.CommitTransaction();

                var mapper = new GetByIdAssessmentFromEntityMapper();
                var updatedAssessmentDto = mapper.FromEntity(assessment);

                if (updatedAssessmentDto == null)
                {
                    return Left<string, AssessmentDto>("Error mapping updated assessment to DTO");
                }

                return Right<string, AssessmentDto>(updatedAssessmentDto);
            }
            catch (Exception e)
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
        public  AssessmentDto? FromEntityDto(AssessmentEntityDto assessment)
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

    public async Task<Either<string, bool>> DeleteAssessmentControlAsync(DeleteAssessmentControlRequest req)
    {
        try
        {
            Guard.Against.NullOrEmpty(req.AssessmentId, nameof(req.AssessmentId));
            Guard.Against.NullOrEmpty(req.ControlId, nameof(req.ControlId));

            var spec = new AssessmentControlByRelIdsSpec(
            req.AssessmentId, 
            req.ControlId            );
            var assessmentControl = await _assessmentControlRepository.FirstOrDefaultAsync(spec);

            if (assessmentControl == null)
                return Left<string, bool>("AssessmentControl not found.");

            assessmentControl.SetIsDeleted(true);
            await _assessmentControlRepository.UpdateAsync(assessmentControl);
            await InvalidateAssessmentControlCacheAsync(req.AssessmentId, req.ControlId);

            return Right<string, bool>(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting assessmentControl");
            return Left<string, bool>($"Error deleting assessmentControl: {ex.Message}");
        }
    }

    public async Task<Either<string, AssessmentControlDto>> CreateAssessmentControlAsync(CreateAssessmentControlJoinRequest req)
    {
        try
        {
            Guard.Against.Null(req, nameof(req));
            var rowId = -1;
            var createdDate = DateTime.UtcNow;

            if (req?.Control?.ControlId == Guid.Empty || req?.Control?.ControlId is null)
            {
                var newControlId = Guid.CreateVersion7();
                var newControl = new Control(
                    controlId: newControlId,
                    tenantId: req.Control.TenantId,
                    controlCode: req.Control.ControlCode,
                    requirementNumber: req.Control.RequirementNumber,
                    controlTitle: req.Control.ControlTitle,
                    controlDescription: req.Control.ControlDescription,
                    frequencyDays: req.Control.FrequencyDays,
                    isMandatory: req.Control.IsMandatory,
                    effectiveDate: req.Control.EffectiveDate,
                    createdAt: req.Control.CreatedAt,
                    createdBy: req.Control.CreatedBy,
                    isDeleted: req.Control.IsDeleted
                );

                 newControl.SetTestingGuidance(req.Control.TestingGuidance);
                 newControl.SetUpdatedAt(req.Control.UpdatedAt);
                 newControl.SetUpdatedBy(req.Control.UpdatedBy);
                await _repositoryFactory.GetRepository<Control>().AddAsync(newControl);
                req.Control.ControlId = newControlId;
                req.AssessmentControl.ControlId = newControlId;

            }
            var newAssessmentControl = new AssessmentControl(
                req.AssessmentId,
                req.AssessmentControl.ControlId,
                req.AssessmentControl.TenantId,
                req.AssessmentControl.TestResult,
                req.AssessmentControl.CreatedAt,
                req.AssessmentControl.CreatedBy,
                req.AssessmentControl.IsDeleted
            );

            newAssessmentControl.SetAssessmentId(req.AssessmentId);
            newAssessmentControl.SetControlId(req.AssessmentControl.ControlId);
            newAssessmentControl.SetTenantId(req.AssessmentControl.TenantId);
            newAssessmentControl.SetTestResult(req.AssessmentControl.TestResult);
            newAssessmentControl.SetTestDate(req.AssessmentControl.TestDate);
            newAssessmentControl.SetTestedBy(req.AssessmentControl.TestedBy);
            newAssessmentControl.SetNotes(req.AssessmentControl.Notes);
            newAssessmentControl.SetCreatedAt(req.AssessmentControl.CreatedAt);
            newAssessmentControl.SetUpdatedAt(req.AssessmentControl.UpdatedAt);
          
            newAssessmentControl.SetIsDeleted(req.AssessmentControl.IsDeleted);
            newAssessmentControl.SetIsDeleted(false);
            _repositoryFactory.GetRepository<AssessmentControl>().BeginTransaction();

            await _repositoryFactory.GetRepository<AssessmentControl>().AddAsync(newAssessmentControl);
            await InvalidateAssessmentControlCacheAsync(req.AssessmentControl.AssessmentId, req.AssessmentControl.ControlId );
            _repositoryFactory.GetRepository<AssessmentControl>().CommitTransaction();

            var mapper = new GetByIdAssessmentControlFromEntityMapper();
            var createdAssessmentControlDto = mapper.FromEntity(newAssessmentControl);
            return Right<string, AssessmentControlDto>(createdAssessmentControlDto);
        }
        catch (Exception ex)
        {
            _repositoryFactory.GetRepository<AssessmentControl>().RollbackTransaction();
            _logger.LogError(ex, "Error creating assessmentControl");
            return Left<string, AssessmentControlDto>($"Error creating assessmentControl: {ex.Message}");
        }
    }
    public async Task<Either<string, AssessmentControlDto>> UpdateAssessmentControlAsync(AssessmentControlDto assessmentControlDto)
    {
        try
        {
            Guard.Against.Null(assessmentControlDto, nameof(assessmentControlDto));

            _assessmentControlRepository.BeginTransaction();
            try
            {
            var spec = new AssessmentControlByRelIdsSpec(
            assessmentControlDto.AssessmentId, 
            assessmentControlDto.ControlId            );

                AssessmentControl? assessmentControl = await _assessmentControlRepository.FirstOrDefaultAsync(spec);
                if (assessmentControl == null)
                {
                    return Left<string, AssessmentControlDto>(
                        $"AssessmentControl with AssessmentId {assessmentControlDto.AssessmentId} and ControlId {assessmentControlDto.ControlId} not found."
                    );
                }

                var updateResult = new AssessmentControl(
                      assessmentControlDto.AssessmentId,
                      assessmentControlDto.ControlId,
                      assessmentControlDto.TenantId,
                      assessmentControlDto.TestResult,
                      assessmentControlDto.CreatedAt,
                      assessmentControlDto.CreatedBy,
                      assessmentControlDto.IsDeleted                );

                updateResult.SetIsDeleted(assessmentControlDto.IsDeleted);
                updateResult.SetRowId(assessmentControl.RowId);

                await _assessmentControlRepository.UpdateAsync(updateResult);
                await InvalidateAssessmentControlCacheAsync(assessmentControlDto.AssessmentId, assessmentControlDto.ControlId);
                _assessmentControlRepository.CommitTransaction();

                var mapper = new GetByIdAssessmentControlFromEntityMapper();
                var updatedAssessmentControlDto = mapper.FromEntity(updateResult);

                if (updatedAssessmentControlDto == null)
                    return Left<string, AssessmentControlDto>("Error mapping updated assessmentControl to DTO");

                return Right<string, AssessmentControlDto>(updatedAssessmentControlDto);
            }
            catch
            {
                _assessmentControlRepository.RollbackTransaction();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "Error updating assessmentControl for AssessmentId {AssessmentId}, ControlId {ControlId}",
                assessmentControlDto.AssessmentId, assessmentControlDto.ControlId);
            return Left<string, AssessmentControlDto>($"Error updating assessmentControl: {ex.Message}");
        }
    }
    public async Task<Either<string, List<AssessmentControlDto>>> GetAssessmentControlPagedList(int pageNumber, int pageSize)
    {
        try
        {
            AssessmentControlListPagedSpec spec = new(pageNumber, pageSize);
            var assessmentControls = await _assessmentControlCacheRepository.ListAsync(spec);
            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();

            var assessmentDtos = assessmentControls
                .Select(entityDto =>
                {
                    var jsonAssessment = JsonSerializer.Serialize(entityDto, settings);
                    return JsonSerializer.Deserialize<AssessmentControlDto>(jsonAssessment, settings);
                })
                .Where(dto => dto != null)
                .ToList();

            return assessmentDtos.Any()
                ? Right<string, List<AssessmentControlDto>>(assessmentDtos)
                : Left<string, List<AssessmentControlDto>>("Error mapping assessments.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving assessmentControl list");
            return Left<string, List<AssessmentControlDto>>($"Error retrieving assessmentControl list: {ex.Message}");
        }
    }
    public async Task<Either<string, AssessmentControlDto>> GetOneFullAssessmentControlByIdAsync(GetByIdAssessmentControlRequest req)
    {
        try
        {
            AssessmentControlByIdSpec spec = new(
            req.AssessmentId,
            req.ControlId 
            );
            AssessmentControlEntityDto? assessmentControl = await _assessmentControlRepository.FirstOrDefaultAsync(spec);

            if (assessmentControl == null)
                return Left<string, AssessmentControlDto>($"AssessmentControl not found for AssessmentId: {req.AssessmentId} and ControlId: {req.ControlId}");

            var mapper = new GetByIdAssessmentControlEntityDtoMapper();
            var assessmentControlDto = mapper.FromEntityDto(assessmentControl);

            return assessmentControlDto != null
                ? Right<string, AssessmentControlDto>(assessmentControlDto)
                : Left<string, AssessmentControlDto>("Error mapping assessmentControl to DTO");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "Error retrieving assessmentControl for AssessmentId: {AssessmentId}, ControlId: {ControlId}", req.AssessmentId, req.ControlId);
            return Left<string, AssessmentControlDto>($"Error retrieving assessmentControl: {ex.Message}");
        }
    }

    public async Task<Either<string, List<AssessmentControlDto>>> SearchAssessmentControlsAsync(string searchTerm)
    {
        try
        {
            Guard.Against.NullOrWhiteSpace(searchTerm, nameof(searchTerm));
            var spec = new AssessmentControlSearchSpec(searchTerm);
            var assessmentControls = await _assessmentControlCacheRepository.ListAsync(spec);

            if (!assessmentControls.Any())
                return new List<AssessmentControlDto>();

            var settings = GJset.GetSystemTextJsonSettings();
            var assessmentControlDtos = assessmentControls
                .Select(ca => JsonSerializer.Deserialize<AssessmentControlDto>(
                    JsonSerializer.Serialize(ca, settings), settings))
                .Where(dto => dto != null)
                .ToList();

            return Right<string, List<AssessmentControlDto>>(assessmentControlDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "Error searching assessmentControls with term: {SearchTerm}", searchTerm);
            return Left<string, List<AssessmentControlDto>>($"Error searching assessmentControls: {ex.Message}");
        }
    }

    public async Task<Either<string, ListAssessmentControlResponse>> GetFilteredAssessmentControlsAsync(FilteredAssessmentControlRequest req)
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
                return Left<string, ListAssessmentControlResponse>(string.Join("; ", errors));

            var spec = new AssessmentControlAdvancedFilterSpec(req.PageNumber, req.PageSize, req.Filters, convertedSortings);
            var assessmentControls = await _assessmentControlCacheRepository.ListAsync(spec);

            if (!assessmentControls.Any())
                return Right<string, ListAssessmentControlResponse>(
                    new ListAssessmentControlResponse { AssessmentControls = null, Count = 0 });

            var settings = GJset.GetSystemTextJsonSettings();
            var assessmentControlDtos = assessmentControls
                .Skip((req.PageNumber - 1) * req.PageSize)
                .Take(req.PageSize)
                .Select(ca => JsonSerializer.Deserialize<AssessmentControlDto>(
                    JsonSerializer.Serialize(ca, settings), settings))
                .Where(dto => dto != null)
                .ToList();

            return Right<string, ListAssessmentControlResponse>(
                new ListAssessmentControlResponse { AssessmentControls = assessmentControlDtos, Count = assessmentControls.Count }
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error filtering assessmentControls");
            return Left<string, ListAssessmentControlResponse>($"Error filtering assessmentControls: {ex.Message}");
        }
    }

    public async Task<Either<string, AssessmentControlDto>> GetLastCreatedAssessmentControlAsync()
    {
        try
        {
            var spec = new AssessmentControlLastCreatedSpec();
            var assessmentControl = await _assessmentControlRepository.FirstOrDefaultAsync(spec);

            if (assessmentControl == null)
                return Left<string, AssessmentControlDto>("No assessmentControls found");

            var mapper = new GetByIdAssessmentControlFromEntityMapper();
            var assessmentControlDto = mapper.FromEntity(assessmentControl);

            return assessmentControlDto != null
                ? Right<string, AssessmentControlDto>(assessmentControlDto)
                : Left<string, AssessmentControlDto>("Error mapping assessmentControl to DTO");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving last created assessmentControl");
            return Left<string, AssessmentControlDto>($"Error retrieving last created assessmentControl: {ex.Message}");
        }
    }

    public class GetByIdAssessmentControlEntityDtoMapper : Mapper<GetByIdAssessmentControlRequest, GetByIdAssessmentControlResponse, AssessmentControlDto>
    {
        public   AssessmentControlDto? FromEntityDto(AssessmentControlEntityDto assessmentControl)
        {
            Guard.Against.Null(assessmentControl, nameof(assessmentControl));
            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            string jsonAssessmentControl = JsonSerializer.Serialize(assessmentControl, settings);
            AssessmentControlDto? assessmentControlDto = JsonSerializer.Deserialize<AssessmentControlDto>(
                jsonAssessmentControl,
                settings
            );
            return assessmentControlDto;
        }
    }

    public class GetByIdAssessmentControlFromEntityMapper : Mapper<GetByIdAssessmentControlRequest, GetByIdAssessmentControlResponse, AssessmentControlDto>
    {
        public   AssessmentControlDto? FromEntity(AssessmentControl assessmentControl)
        {
            Guard.Against.Null(assessmentControl, nameof(assessmentControl));
            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            string jsonAssessmentControl = JsonSerializer.Serialize(assessmentControl, settings);
            AssessmentControlDto? assessmentControlDto = JsonSerializer.Deserialize<AssessmentControlDto>(
                jsonAssessmentControl,
                settings
            );
            return assessmentControlDto;
        }
    }

    public async Task<Either<string, bool>> DeleteControlEvidenceAsync(DeleteControlEvidenceRequest req)
    {
        try
        {
            Guard.Against.NullOrEmpty(req.AssessmentId, nameof(req.AssessmentId));
            Guard.Against.NullOrEmpty(req.EvidenceId, nameof(req.EvidenceId));

            var spec = new ControlEvidenceByRelIdsSpec(
            req.ControlId, 
            req.EvidenceId, 
            req.AssessmentId            );
            var controlEvidence = await _controlEvidenceRepository.FirstOrDefaultAsync(spec);

            if (controlEvidence == null)
                return Left<string, bool>("ControlEvidence not found.");

            controlEvidence.SetIsDeleted(true);
            await _controlEvidenceRepository.UpdateAsync(controlEvidence);
            await InvalidateControlEvidenceCacheAsync(req.AssessmentId, req.EvidenceId);

            return Right<string, bool>(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting controlEvidence");
            return Left<string, bool>($"Error deleting controlEvidence: {ex.Message}");
        }
    }

    public async Task<Either<string, ControlEvidenceDto>> CreateControlEvidenceAsync(CreateControlEvidenceJoinRequest req)
    {
        try
        {
            Guard.Against.Null(req, nameof(req));
            var rowId = -1;
            var createdDate = DateTime.UtcNow;

            if (req?.Evidence?.EvidenceId == Guid.Empty || req?.Evidence?.EvidenceId is null)
            {
                var newEvidenceId = Guid.CreateVersion7();
                var newEvidence = new Evidence(
                    evidenceId: newEvidenceId,
                    merchantId: req.Evidence.MerchantId,
                    tenantId: req.Evidence.TenantId,
                    evidenceCode: req.Evidence.EvidenceCode,
                    evidenceTitle: req.Evidence.EvidenceTitle,
                    evidenceType: req.Evidence.EvidenceType,
                    collectedDate: req.Evidence.CollectedDate,
                    isValid: req.Evidence.IsValid,
                    createdAt: req.Evidence.CreatedAt,
                    createdBy: req.Evidence.CreatedBy,
                    isDeleted: req.Evidence.IsDeleted
                );

                 newEvidence.SetFileHash(req.Evidence.FileHash);
                 newEvidence.SetStorageUri(req.Evidence.StorageUri);
                 newEvidence.SetUpdatedAt(req.Evidence.UpdatedAt);
                 newEvidence.SetUpdatedBy(req.Evidence.UpdatedBy);
                await _repositoryFactory.GetRepository<Evidence>().AddAsync(newEvidence);
                req.Evidence.EvidenceId = newEvidenceId;
                req.ControlEvidence.EvidenceId = newEvidenceId;

            }
            var newControlEvidence = new ControlEvidence(
                req.AssessmentId,
                req.ControlEvidence.ControlId,
                req.ControlEvidence.EvidenceId,
                req.ControlEvidence.TenantId,
                req.ControlEvidence.IsPrimary,
                req.ControlEvidence.CreatedAt,
                req.ControlEvidence.CreatedBy,
                req.ControlEvidence.IsDeleted
            );

            newControlEvidence.SetControlId(req.ControlEvidence.ControlId);
            newControlEvidence.SetEvidenceId(req.ControlEvidence.EvidenceId);
            newControlEvidence.SetAssessmentId(req.AssessmentId);
            newControlEvidence.SetTenantId(req.ControlEvidence.TenantId);
            newControlEvidence.SetIsPrimary(req.ControlEvidence.IsPrimary);
            newControlEvidence.SetCreatedAt(req.ControlEvidence.CreatedAt);
            newControlEvidence.SetUpdatedAt(req.ControlEvidence.UpdatedAt);
            newControlEvidence.SetIsDeleted(req.ControlEvidence.IsDeleted);
            newControlEvidence.SetIsDeleted(false);
            _repositoryFactory.GetRepository<ControlEvidence>().BeginTransaction();

            await _repositoryFactory.GetRepository<ControlEvidence>().AddAsync(newControlEvidence);
            await InvalidateControlEvidenceCacheAsync(req.ControlEvidence.AssessmentId, req.ControlEvidence.EvidenceId );
            _repositoryFactory.GetRepository<ControlEvidence>().CommitTransaction();

            var mapper = new GetByIdControlEvidenceFromEntityMapper();
            var createdControlEvidenceDto = mapper.FromEntity(newControlEvidence);
            return Right<string, ControlEvidenceDto>(createdControlEvidenceDto);
        }
        catch (Exception ex)
        {
            _repositoryFactory.GetRepository<ControlEvidence>().RollbackTransaction();
            _logger.LogError(ex, "Error creating controlEvidence");
            return Left<string, ControlEvidenceDto>($"Error creating controlEvidence: {ex.Message}");
        }
    }
    public async Task<Either<string, ControlEvidenceDto>> UpdateControlEvidenceAsync(ControlEvidenceDto controlEvidenceDto)
    {
        try
        {
            Guard.Against.Null(controlEvidenceDto, nameof(controlEvidenceDto));

            _controlEvidenceRepository.BeginTransaction();
            try
            {
            var spec = new ControlEvidenceByRelIdsSpec(
            controlEvidenceDto.ControlId, 
            controlEvidenceDto.EvidenceId, 
            controlEvidenceDto.AssessmentId            );

                ControlEvidence? controlEvidence = await _controlEvidenceRepository.FirstOrDefaultAsync(spec);
                if (controlEvidence == null)
                {
                    return Left<string, ControlEvidenceDto>(
                        $"ControlEvidence with AssessmentId {controlEvidenceDto.AssessmentId} and EvidenceId {controlEvidenceDto.EvidenceId} not found."
                    );
                }

                var updateResult = new ControlEvidence(
                      controlEvidenceDto.AssessmentId,
                      controlEvidenceDto.ControlId,
                      controlEvidenceDto.EvidenceId,
                      controlEvidenceDto.TenantId,
                      controlEvidenceDto.IsPrimary,
                      controlEvidenceDto.CreatedAt,
                      controlEvidenceDto.CreatedBy,
                      controlEvidenceDto.IsDeleted                );

                updateResult.SetIsDeleted(controlEvidenceDto.IsDeleted);
                updateResult.SetRowId(controlEvidence.RowId);

                await _controlEvidenceRepository.UpdateAsync(updateResult);
                await InvalidateControlEvidenceCacheAsync(controlEvidenceDto.AssessmentId, controlEvidenceDto.EvidenceId);
                _controlEvidenceRepository.CommitTransaction();

                var mapper = new GetByIdControlEvidenceFromEntityMapper();
                var updatedControlEvidenceDto = mapper.FromEntity(updateResult);

                if (updatedControlEvidenceDto == null)
                    return Left<string, ControlEvidenceDto>("Error mapping updated controlEvidence to DTO");

                return Right<string, ControlEvidenceDto>(updatedControlEvidenceDto);
            }
            catch
            {
                _controlEvidenceRepository.RollbackTransaction();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "Error updating controlEvidence for AssessmentId {AssessmentId}, EvidenceId {EvidenceId}",
                controlEvidenceDto.AssessmentId, controlEvidenceDto.EvidenceId);
            return Left<string, ControlEvidenceDto>($"Error updating controlEvidence: {ex.Message}");
        }
    }
    public async Task<Either<string, List<ControlEvidenceDto>>> GetControlEvidencePagedList(int pageNumber, int pageSize)
    {
        try
        {
            ControlEvidenceListPagedSpec spec = new(pageNumber, pageSize);
            var controlEvidences = await _controlEvidenceCacheRepository.ListAsync(spec);
            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();

            var assessmentDtos = controlEvidences
                .Select(entityDto =>
                {
                    var jsonAssessment = JsonSerializer.Serialize(entityDto, settings);
                    return JsonSerializer.Deserialize<ControlEvidenceDto>(jsonAssessment, settings);
                })
                .Where(dto => dto != null)
                .ToList();

            return assessmentDtos.Any()
                ? Right<string, List<ControlEvidenceDto>>(assessmentDtos)
                : Left<string, List<ControlEvidenceDto>>("Error mapping assessments.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving controlEvidence list");
            return Left<string, List<ControlEvidenceDto>>($"Error retrieving controlEvidence list: {ex.Message}");
        }
    }
    public async Task<Either<string, ControlEvidenceDto>> GetOneFullControlEvidenceByIdAsync(GetByIdControlEvidenceRequest req)
    {
        try
        {
            ControlEvidenceByIdSpec spec = new(
            req.ControlId,
            req.EvidenceId,
            req.AssessmentId 
            );
            ControlEvidenceEntityDto? controlEvidence = await _controlEvidenceRepository.FirstOrDefaultAsync(spec);

            if (controlEvidence == null)
                return Left<string, ControlEvidenceDto>($"ControlEvidence not found for AssessmentId: {req.AssessmentId} and EvidenceId: {req.EvidenceId}");

            var mapper = new GetByIdControlEvidenceEntityDtoMapper();
            var controlEvidenceDto = mapper.FromEntityDto(controlEvidence);

            return controlEvidenceDto != null
                ? Right<string, ControlEvidenceDto>(controlEvidenceDto)
                : Left<string, ControlEvidenceDto>("Error mapping controlEvidence to DTO");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "Error retrieving controlEvidence for AssessmentId: {AssessmentId}, EvidenceId: {EvidenceId}", req.AssessmentId, req.EvidenceId);
            return Left<string, ControlEvidenceDto>($"Error retrieving controlEvidence: {ex.Message}");
        }
    }

    public async Task<Either<string, List<ControlEvidenceDto>>> SearchControlEvidencesAsync(string searchTerm)
    {
        try
        {
            Guard.Against.NullOrWhiteSpace(searchTerm, nameof(searchTerm));
            var spec = new ControlEvidenceSearchSpec(searchTerm);
            var controlEvidences = await _controlEvidenceCacheRepository.ListAsync(spec);

            if (!controlEvidences.Any())
                return new List<ControlEvidenceDto>();

            var settings = GJset.GetSystemTextJsonSettings();
            var controlEvidenceDtos = controlEvidences
                .Select(ca => JsonSerializer.Deserialize<ControlEvidenceDto>(
                    JsonSerializer.Serialize(ca, settings), settings))
                .Where(dto => dto != null)
                .ToList();

            return Right<string, List<ControlEvidenceDto>>(controlEvidenceDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "Error searching controlEvidences with term: {SearchTerm}", searchTerm);
            return Left<string, List<ControlEvidenceDto>>($"Error searching controlEvidences: {ex.Message}");
        }
    }

    public async Task<Either<string, ListControlEvidenceResponse>> GetFilteredControlEvidencesAsync(FilteredControlEvidenceRequest req)
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
                return Left<string, ListControlEvidenceResponse>(string.Join("; ", errors));

            var spec = new ControlEvidenceAdvancedFilterSpec(req.PageNumber, req.PageSize, req.Filters, convertedSortings);
            var controlEvidences = await _controlEvidenceCacheRepository.ListAsync(spec);

            if (!controlEvidences.Any())
                return Right<string, ListControlEvidenceResponse>(
                    new ListControlEvidenceResponse { ControlEvidences = null, Count = 0 });

            var settings = GJset.GetSystemTextJsonSettings();
            var controlEvidenceDtos = controlEvidences
                .Skip((req.PageNumber - 1) * req.PageSize)
                .Take(req.PageSize)
                .Select(ca => JsonSerializer.Deserialize<ControlEvidenceDto>(
                    JsonSerializer.Serialize(ca, settings), settings))
                .Where(dto => dto != null)
                .ToList();

            return Right<string, ListControlEvidenceResponse>(
                new ListControlEvidenceResponse { ControlEvidences = controlEvidenceDtos, Count = controlEvidences.Count }
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error filtering controlEvidences");
            return Left<string, ListControlEvidenceResponse>($"Error filtering controlEvidences: {ex.Message}");
        }
    }

    public async Task<Either<string, ControlEvidenceDto>> GetLastCreatedControlEvidenceAsync()
    {
        try
        {
            var spec = new ControlEvidenceLastCreatedSpec();
            var controlEvidence = await _controlEvidenceRepository.FirstOrDefaultAsync(spec);

            if (controlEvidence == null)
                return Left<string, ControlEvidenceDto>("No controlEvidences found");

            var mapper = new GetByIdControlEvidenceFromEntityMapper();
            var controlEvidenceDto = mapper.FromEntity(controlEvidence);

            return controlEvidenceDto != null
                ? Right<string, ControlEvidenceDto>(controlEvidenceDto)
                : Left<string, ControlEvidenceDto>("Error mapping controlEvidence to DTO");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving last created controlEvidence");
            return Left<string, ControlEvidenceDto>($"Error retrieving last created controlEvidence: {ex.Message}");
        }
    }

    public class GetByIdControlEvidenceEntityDtoMapper : Mapper<GetByIdControlEvidenceRequest, GetByIdControlEvidenceResponse, ControlEvidenceDto>
    {
        public   ControlEvidenceDto? FromEntityDto(ControlEvidenceEntityDto controlEvidence)
        {
            Guard.Against.Null(controlEvidence, nameof(controlEvidence));
            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            string jsonControlEvidence = JsonSerializer.Serialize(controlEvidence, settings);
            ControlEvidenceDto? controlEvidenceDto = JsonSerializer.Deserialize<ControlEvidenceDto>(
                jsonControlEvidence,
                settings
            );
            return controlEvidenceDto;
        }
    }

    public class GetByIdControlEvidenceFromEntityMapper : Mapper<GetByIdControlEvidenceRequest, GetByIdControlEvidenceResponse, ControlEvidenceDto>
    {
        public   ControlEvidenceDto? FromEntity(ControlEvidence controlEvidence)
        {
            Guard.Against.Null(controlEvidence, nameof(controlEvidence));
            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            string jsonControlEvidence = JsonSerializer.Serialize(controlEvidence, settings);
            ControlEvidenceDto? controlEvidenceDto = JsonSerializer.Deserialize<ControlEvidenceDto>(
                jsonControlEvidence,
                settings
            );
            return controlEvidenceDto;
        }
    }
        public async Task<Either<string, ROCPackageDto>> GetOneFullROCPackageByIdAsync(Guid rocpackageId)
        {
            try
            {
            ROCPackageByIdSpec spec = new(rocpackageId);
                ROCPackageEntityDto? rocpackage = await _rocpackageCacheRepository.FirstOrDefaultAsync(spec);
                GetByIdROCPackageEntityDtoMapper mapper = new();

                if (rocpackage != null)
                {
                    ROCPackageDto? rocpackageDto = mapper.FromEntityDto(rocpackage);
                    return rocpackageDto != null
                        ? Right<string, ROCPackageDto>(rocpackageDto)
                        : Left<string, ROCPackageDto>("Error mapping rocpackage to DTO");
                }
                return Left<string, ROCPackageDto>("ROCPackage not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error retrieving rocpackage {ROCPackageId}", rocpackageId);
                return Left<string, ROCPackageDto>($"Error retrieving rocpackage: {ex.Message}");
            }
        }

        public async Task<Either<string, List<ROCPackageDto>>> SearchROCPackagesAsync(string searchTerm)
        {
            try
            {
                Guard.Against.NullOrWhiteSpace(searchTerm, nameof(searchTerm));
                var spec = new ROCPackageSearchSpec(searchTerm);
                var rocpackages = await _rocpackageCacheRepository.ListAsync(spec);

                var settings = GJset.GetSystemTextJsonSettings();
                var rocpackageDtos = rocpackages
                    .Select(i => JsonSerializer.Deserialize<ROCPackageDto>(JsonSerializer.Serialize(i, settings), settings))
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
                var rocpackages = await _rocpackageCacheRepository.ListAsync(spec);

                if (!rocpackages.Any())
                    return Right<string, ListROCPackageResponse>(new ListROCPackageResponse { ROCPackages = null, Count = 0 });

                var settings = GJset.GetSystemTextJsonSettings();
                var rocpackageDtos = rocpackages
                    .Skip((req.PageNumber - 1) * req.PageSize)
                    .Take(req.PageSize)
                    .Select(i => JsonSerializer.Deserialize<ROCPackageDto>(JsonSerializer.Serialize(i, settings), settings))
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

        public async Task<Either<string, List<ROCPackageDto>>> GetROCPackagePagedList(int pageNumber, int pageSize)
        {
            try
            {
                var spec = new ROCPackageListPagedSpec(pageNumber, pageSize);
                var rocpackageEntityDtos = await _rocpackageCacheRepository.ListAsync(spec);

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

        public async Task<Either<string, bool>> DeleteROCPackageAsync(Guid rocpackageId)
        {
            try
            {
                Guard.Against.NullOrEmpty(rocpackageId, nameof(rocpackageId));

                var rocpackage = await _rocpackageRepository.GetByIdAsync(rocpackageId);

                if (rocpackage == null)
                    return Left<string, bool>("ROCPackage not found.");

                rocpackage.SetIsDeleted(true);
                await _rocpackageRepository.UpdateAsync(rocpackage);
                await InvalidateROCPackageCacheAsync(rocpackageId);

                return Right<string, bool>(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting rocpackage");
                return Left<string, bool>($"Error deleting rocpackage: {ex.Message}");
            }
        }

        public async Task<Either<string, ROCPackageDto>> GetLastCreatedROCPackageAsync()
        {
            try
            {
                var spec = new ROCPackageLastCreatedSpec();
                var rocpackage = await _rocpackageRepository.FirstOrDefaultAsync(spec);

                if (rocpackage == null)
                    return Left<string, ROCPackageDto>("No rocpackages found");

                var mapper = new GetByIdROCPackageFromEntityMapper();
                var rocpackageDto = mapper.FromEntity(rocpackage);

                return rocpackageDto != null
                    ? Right<string, ROCPackageDto>(rocpackageDto)
                    : Left<string, ROCPackageDto>("Error mapping rocpackage to DTO");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving last created rocpackage");
                return Left<string, ROCPackageDto>($"Error retrieving last created rocpackage: {ex.Message}");
            }
        }

        public async Task<Either<string, ROCPackageDto>> CreateROCPackageAsync(ROCPackageDto rocpackageDto)
        {
            try
            {
                Guard.Against.Null(rocpackageDto, nameof(rocpackageDto));

                var newROCPackage = new ROCPackage(

                    rocpackageId: Guid.CreateVersion7(),
                      rocpackageDto.AssessmentId,
                      rocpackageDto.TenantId,
                      rocpackageDto.PackageVersion,
                      rocpackageDto.GeneratedDate,
                      rocpackageDto.Rank,
                      rocpackageDto.CreatedAt,
                      rocpackageDto.CreatedBy,
                      rocpackageDto.IsDeleted

                );

            newROCPackage.SetQSAName(rocpackageDto.QSAName);
            newROCPackage.SetQSACompany(rocpackageDto.QSACompany);
            newROCPackage.SetSignatureDate(rocpackageDto.SignatureDate);
            newROCPackage.SetAOCNumber(rocpackageDto.AOCNumber);
            newROCPackage.SetUpdatedAt(rocpackageDto.UpdatedAt);
            newROCPackage.SetIsDeleted(false);

                _rocpackageRepository.BeginTransaction();
                await _rocpackageRepository.AddAsync(newROCPackage);
                await InvalidateROCPackageCacheAsync(newROCPackage.ROCPackageId);
            _rocpackageRepository.CommitTransaction();

                var mapper = new GetByIdROCPackageFromEntityMapper();
                var createdROCPackageDto = mapper.FromEntity(newROCPackage);

                return Right<string, ROCPackageDto>(createdROCPackageDto);
            }
            catch (Exception ex)
            {
                _rocpackageRepository.RollbackTransaction();
                _logger.LogError(ex, "Error creating rocpackage");
                return Left<string, ROCPackageDto>($"Error creating rocpackage: {ex.Message}");
            }
        }

        public async Task<Either<string, ROCPackageDto>> UpdateROCPackageAsync(Guid rocpackageId, ROCPackageDto rocpackageDto)
        {
            try
            {
                Guard.Against.NullOrEmpty(rocpackageId, nameof(rocpackageId));
                Guard.Against.Null(rocpackageDto, nameof(rocpackageDto));

                var validationResult = await _updateROCPackageValidator.ValidateAsync(rocpackageDto);
                if (!validationResult.IsValid)
                {
                    var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                    return Left<string, ROCPackageDto>($"Validation failed: {errors}");
                }

                _rocpackageRepository.BeginTransaction();
                try
                {
                    ROCPackage? rocpackage = await _rocpackageRepository.GetByIdAsync(rocpackageId);

                    if (rocpackage == null)
                        return Left<string, ROCPackageDto>($"ROCPackage with ID {rocpackageDto.ROCPackageId} not found.");

                    var updateResult = new ROCPackage(

                        rocpackageId: rocpackage.ROCPackageId,
                      rocpackageDto.AssessmentId,
                      rocpackageDto.TenantId,
                      rocpackageDto.PackageVersion,
                      rocpackageDto.GeneratedDate,
                      rocpackageDto.Rank,
                      rocpackageDto.CreatedAt,
                      rocpackageDto.CreatedBy,
                      rocpackageDto.IsDeleted
                    );

                rocpackage.SetIsDeleted(rocpackageDto.IsDeleted);

                    await _rocpackageRepository.UpdateAsync(updateResult);
                    await InvalidateROCPackageCacheAsync(rocpackageId);
                    _rocpackageRepository.CommitTransaction();

                    var mapper = new GetByIdROCPackageFromEntityMapper();
                    var updatedROCPackageDto = mapper.FromEntity(updateResult);

                    return updatedROCPackageDto != null
                        ? Right<string, ROCPackageDto>(updatedROCPackageDto)
                        : Left<string, ROCPackageDto>("Error mapping updated rocpackage to DTO");
                }
                catch
                {
                    _rocpackageRepository.RollbackTransaction();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error updating rocpackage {ROCPackageId}", rocpackageDto.ROCPackageId);
                return Left<string, ROCPackageDto>($"Error updating rocpackage: {ex.Message}");
            }
        }
    public class GetByIdROCPackageEntityDtoMapper : Mapper<GetByIdROCPackageRequest, GetByIdROCPackageResponse, ROCPackageDto>
    {
        public   ROCPackageDto? FromEntityDto(ROCPackageEntityDto rocpackage)
        {
            Guard.Against.Null(rocpackage, nameof(rocpackage));
            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            string jsonROCPackage = JsonSerializer.Serialize(rocpackage, settings);
            ROCPackageDto? rocpackageDto = JsonSerializer.Deserialize<ROCPackageDto>(jsonROCPackage, settings);
            return rocpackageDto;
        }
    }

    public class GetByIdROCPackageFromEntityMapper : Mapper<GetByIdROCPackageRequest, GetByIdROCPackageResponse, ROCPackageDto>
    {
        public   ROCPackageDto? FromEntity(ROCPackage rocpackage)
        {
            Guard.Against.Null(rocpackage, nameof(rocpackage));
            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            string jsonROCPackage = JsonSerializer.Serialize(rocpackage, settings);
            ROCPackageDto? rocpackageDto = JsonSerializer.Deserialize<ROCPackageDto>(jsonROCPackage, settings);
            return rocpackageDto;
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
            var paymentPages = await _repositoryCacheFactory.GetReadRedisRepository<PaymentPage>().ListAsync(spec);

            if (!paymentPages.Any())
                return Right<string, ListPaymentPageResponse>(new ListPaymentPageResponse { PaymentPages = null, Count = 0 });

            var settings = GJset.GetSystemTextJsonSettings();
            var paymentPageDtos = paymentPages
                .Skip((req.PageNumber - 1) * req.PageSize)
                .Take(req.PageSize)
                .Select(c => JsonSerializer.Deserialize<PaymentPageDto>(JsonSerializer.Serialize(c, settings), settings))
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
    public async Task<Either<string, List<PaymentPageDto>>> GetPaymentPagePagedList(int pageNumber,
        int pageSize,
        Guid? paymentChannelId
        )
    {
        try
        {
            var spec = new PaymentPageListPagedSpec(pageNumber,
        pageSize);
        if (paymentChannelId != null && paymentChannelId != Guid.Empty)
        {
        _ = spec.Query.Where(a => a.PaymentChannelId == paymentChannelId.Value);
        }
            var paymentPageEntityDtos = await _repositoryCacheFactory.GetReadRedisRepository<PaymentPage>().ListAsync(spec);

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
    public async Task<Either<string, List<PaymentPageDto>>> SearchPaymentPagesAsync(string searchTerm)
    {
        try
        {
            Guard.Against.NullOrWhiteSpace(searchTerm, nameof(searchTerm));
            var spec = new PaymentPageSearchSpec(searchTerm);

            var paymentPages = await _repositoryCacheFactory.GetReadRedisRepository<PaymentPage>().ListAsync(spec);

            var settings = GJset.GetSystemTextJsonSettings();
            var paymentPageDtos = paymentPages
                .Select(c => JsonSerializer.Deserialize<PaymentPageDto>(JsonSerializer.Serialize(c, settings), settings))
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
    public async Task<Either<string, ListScriptResponse>> GetFilteredScriptsAsync(FilteredScriptRequest req)
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
                return Left<string, ListScriptResponse>(string.Join("; ", errors));

            var spec = new ScriptAdvancedFilterSpec(req.PageNumber, req.PageSize, req.Filters, convertedSortings);
            var scripts = await _repositoryCacheFactory.GetReadRedisRepository<Script>().ListAsync(spec);

            if (!scripts.Any())
                return Right<string, ListScriptResponse>(new ListScriptResponse { Scripts = null, Count = 0 });

            var settings = GJset.GetSystemTextJsonSettings();
            var scriptDtos = scripts
                .Skip((req.PageNumber - 1) * req.PageSize)
                .Take(req.PageSize)
                .Select(c => JsonSerializer.Deserialize<ScriptDto>(JsonSerializer.Serialize(c, settings), settings))
                .Where(dto => dto != null)
                .ToList();

            return Right<string, ListScriptResponse>(
                new ListScriptResponse { Scripts = scriptDtos, Count = scripts.Count }
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error filtering scripts");
            return Left<string, ListScriptResponse>($"Error filtering scripts: {ex.Message}");
        }
    }
    public async Task<Either<string, List<ScriptDto>>> GetScriptPagedList(int pageNumber,
        int pageSize,
        Guid? paymentPageId
        )
    {
        try
        {
            var spec = new ScriptListPagedSpec(pageNumber,
        pageSize);
        if (paymentPageId != null && paymentPageId != Guid.Empty)
        {
        _ = spec.Query.Where(a => a.PaymentPageId == paymentPageId.Value);
        }
            var scriptEntityDtos = await _repositoryCacheFactory.GetReadRedisRepository<Script>().ListAsync(spec);

            if (!scriptEntityDtos.Any())
                return Left<string, List<ScriptDto>>("No scripts found.");

            var settings = GJset.GetSystemTextJsonSettings();
            var scriptDtos = scriptEntityDtos
                .Select(entityDto =>
                {
                    var jsonScript = JsonSerializer.Serialize(entityDto, settings);
                    return JsonSerializer.Deserialize<ScriptDto>(jsonScript, settings);
                })
                .Where(dto => dto != null)
                .ToList();

            return scriptDtos.Any()
                ? Right<string, List<ScriptDto>>(scriptDtos)
                : Left<string, List<ScriptDto>>("Error mapping scripts.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "Error retrieving script list for page {PageNumber} with size {PageSize}",
                pageNumber, pageSize);
            return Left<string, List<ScriptDto>>($"Error retrieving script list: {ex.Message}");
        }
    }
    public async Task<Either<string, List<ScriptDto>>> SearchScriptsAsync(string searchTerm)
    {
        try
        {
            Guard.Against.NullOrWhiteSpace(searchTerm, nameof(searchTerm));
            var spec = new ScriptSearchSpec(searchTerm);

            var scripts = await _repositoryCacheFactory.GetReadRedisRepository<Script>().ListAsync(spec);

            var settings = GJset.GetSystemTextJsonSettings();
            var scriptDtos = scripts
                .Select(c => JsonSerializer.Deserialize<ScriptDto>(JsonSerializer.Serialize(c, settings), settings))
                .Where(dto => dto != null)
                .ToList();

            return Right<string, List<ScriptDto>>(scriptDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "Error searching scripts with term: {SearchTerm}", searchTerm);
            return Left<string, List<ScriptDto>>($"Error searching scripts: {ex.Message}");
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
