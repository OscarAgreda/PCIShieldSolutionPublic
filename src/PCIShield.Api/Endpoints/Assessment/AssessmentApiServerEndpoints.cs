using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using PCIShield.Domain.ModelsDto;
using PCIShield.Infrastructure.Services;
using LanguageExt;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;

using Amazon.Runtime.Internal;

using FluentValidation.Results;
using Sort = PCIShieldLib.SharedKernel.Interfaces.Sort;
using BlazorMauiShared.Models.Assessment;
using PCIShield.BlazorMauiShared.Models.Assessment;
using BlazorMauiShared.Models.AssessmentControl;
using PCIShield.BlazorMauiShared.Models.AssessmentControl;
using BlazorMauiShared.Models.Control;
using PCIShield.BlazorMauiShared.Models.Control;
using BlazorMauiShared.Models.ControlEvidence;
using PCIShield.BlazorMauiShared.Models.ControlEvidence;
using BlazorMauiShared.Models.Evidence;
using PCIShield.BlazorMauiShared.Models.Evidence;
using BlazorMauiShared.Models.ScanSchedule;
using PCIShield.BlazorMauiShared.Models.ScanSchedule;
using BlazorMauiShared.Models.Vulnerability;
using PCIShield.BlazorMauiShared.Models.Vulnerability;
using BlazorMauiShared.Models.PaymentPage;
using PCIShield.BlazorMauiShared.Models.PaymentPage;
using BlazorMauiShared.Models.Script;
using PCIShield.BlazorMauiShared.Models.Script;
using BlazorMauiShared.Models.ROCPackage;
using PCIShield.BlazorMauiShared.Models.ROCPackage;
using BlazorMauiShared.Models.Merchant;
using PCIShield.BlazorMauiShared.Models.Merchant;
using PCIShield.BlazorMauiShared.ModelsDto;

using PCIShieldLib.SharedKernel.Interfaces;
using Microsoft.AspNetCore.Mvc;
using UuidV7Generator = PCIShield.Domain.ModelsDto.UuidV7Generator;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelEntityDto;
namespace PCIShield.Api.Endpoints.Assessment;
public class GetLastCreatedAssessmentEndpoint : EndpointWithoutRequest<GetByIdAssessmentResponse>
{
    private readonly AssessmentService _assessmentService;
    private readonly IAppLoggerService<GetLastCreatedAssessmentEndpoint> _logger;

    public GetLastCreatedAssessmentEndpoint(AssessmentService assessmentService,
        IAppLoggerService<GetLastCreatedAssessmentEndpoint> logger)
    {
        _assessmentService = assessmentService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("/assessments/last-created");
        AllowAnonymous();
        Description(d => d
            .Produces<GetByIdAssessmentResponse>(200, "application/json")
            .Produces(404));
        Tags("Assessments");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await _assessmentService.GetLastCreatedAssessmentAsync();
        await result.Match(
            Right: async assessment => await SendOkAsync(new GetByIdAssessmentResponse { Assessment = assessment }, ct),
            Left: async error => await SendErrorsAsync(404, ct)
        );
    }
}

public class CreateAssessmentEndpoint : Endpoint<CreateAssessmentRequest, CreateAssessmentResponse>
{
    private readonly AssessmentService _assessmentService;
    private readonly IAppLoggerService<CreateAssessmentEndpoint> _logger;

    public CreateAssessmentEndpoint(AssessmentService assessmentService, IAppLoggerService<CreateAssessmentEndpoint> logger)
    {
        _assessmentService = assessmentService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/assessments");
        AllowAnonymous();
        Description(d => d.Produces<CreateAssessmentResponse>(201, "application/json").Produces(400));
        Tags("Assessments");
        Summary(s =>
        {
            s.Summary = "Create a new Assessment";
            s.Description = "Creates a new Assessment";
            s.ExampleRequest = new CreateAssessmentRequest { };
        });
    }

    public override async Task HandleAsync(CreateAssessmentRequest req, CancellationToken ct)
    {
        var response = await Observable.FromAsync(() => ProcessCreateAssessmentRequest(req, ct)).Take(1).FirstOrDefaultAsync();
        if (response != null)
        {
            await SendCreatedAtAsync<CreateAssessmentEndpoint>(new { assessmentId = response.Assessment.AssessmentId }, response, generateAbsoluteUrl: true, cancellation: ct);
        }
        else
        {
            await SendErrorsAsync(400, ct);
        }
    }

    private async Task<CreateAssessmentResponse> ProcessCreateAssessmentRequest(CreateAssessmentRequest req, CancellationToken ct)
    {
        _logger.LogInformation("Creating new assessment");
        Either<string, AssessmentDto> result = await _assessmentService.CreateAssessmentAsync(req.Assessment);
        return result.Match(Right: dto => new CreateAssessmentResponse { Assessment = dto, ErrorMessage = null }, Left: err => new CreateAssessmentResponse { Assessment = null, ErrorMessage = err });
    }
}

public class DeleteAssessmentEndpoint : Endpoint<DeleteAssessmentRequest, DeleteAssessmentResponse>
{
    private readonly AssessmentService _assessmentService;
    private readonly IAppLoggerService<DeleteAssessmentEndpoint> _logger;

    public DeleteAssessmentEndpoint(AssessmentService assessmentService, IAppLoggerService<DeleteAssessmentEndpoint> logger)
    {
        _assessmentService = assessmentService;
        _logger = logger;
    }

    public override void Configure()
    {
        Delete("/assessments/{assessmentId}");
        AllowAnonymous();
        Description(d => d.Produces<DeleteAssessmentResponse>(200, "application/json").Produces(404).Produces(400));
        Tags("Assessments");
        Summary(s =>
        {
            s.Summary = "Delete an existing Assessment";
            s.Description = "Deletes an existing Assessment";
            s.ExampleRequest = new DeleteAssessmentRequest { AssessmentId = Guid.NewGuid() };
        });
    }

    public override async Task HandleAsync(DeleteAssessmentRequest req, CancellationToken ct)
    {
        var response = await Observable.FromAsync(() => ProcessDeleteAssessmentRequest(req.AssessmentId, ct)).Take(1).FirstOrDefaultAsync();
        if (response != null)
        {
            if (response.IsSuccess)
            {
                await SendOkAsync(response, ct);
            }
            else
            {
                await SendNotFoundAsync(ct);
            }
        }
        else
        {
            await SendErrorsAsync(400, ct);
        }
    }

    private async Task<DeleteAssessmentResponse> ProcessDeleteAssessmentRequest(Guid assessmentId, CancellationToken ct)
    {
        _logger.LogInformation($"Deleting assessment with ID: {assessmentId}");
        Either<string, bool> result = await _assessmentService.DeleteAssessmentAsync(assessmentId);
        return result.Match(
            Right: success => new DeleteAssessmentResponse { IsSuccess = success, ErrorMessage = null },
            Left: err => new DeleteAssessmentResponse { IsSuccess = false, ErrorMessage = err }
        );
    }
}

public class GetAssessmentByIdWithIncludesEndpoint : Endpoint<GetByIdAssessmentRequest, GetByIdAssessmentResponse>
{
    private readonly AssessmentService _assessmentService;
    private readonly IAppLoggerService<GetAssessmentByIdWithIncludesEndpoint> _logger;

    public GetAssessmentByIdWithIncludesEndpoint(AssessmentService assessmentService, IAppLoggerService<GetAssessmentByIdWithIncludesEndpoint> logger)
    {
        _assessmentService = assessmentService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/assessments/i/{assessmentId}");
        AllowAnonymous();
        Description(d => d.Produces<GetByIdAssessmentResponse>(200, "application/json").Produces(404).Produces(400));
        Tags("Assessments");
        Summary(s =>
        {
            s.Summary = "Get an Assessment by Id With Includes";
            s.Description = "Gets an Assessment by Id With Includes";
            s.ExampleRequest = new GetByIdAssessmentRequest { AssessmentId = Guid.NewGuid() };
        });
    }

    public override async Task HandleAsync(GetByIdAssessmentRequest req, CancellationToken ct)
    {
        var response = await Observable.FromAsync(() => ProcessAssessmentRequest(req.AssessmentId, req.WithPostGraph, ct)).Take(1).FirstOrDefaultAsync();
        if (response != null)
        {
            await SendOkAsync(response, ct);
        }
        else
        {
            await SendErrorsAsync(400, ct);
        }
    }

    private async Task<GetByIdAssessmentResponse> ProcessAssessmentRequest(Guid assessmentId, bool withPostGraph, CancellationToken ct)
    {
        _logger.LogInformation($"Fetching assessment with ID: {assessmentId}");
        Either<string, AssessmentDto> result = await _assessmentService.GetOneFullAssessmentByIdAsync(assessmentId, withPostGraph);
        return result.Match(
            Right: dto => new GetByIdAssessmentResponse { Assessment = dto, ErrorMessage = null },
            Left: err => new GetByIdAssessmentResponse { Assessment = null, ErrorMessage = err }
        );
    }
}
public class CompareAssessmentSpecificationPerformanceEndpoint : Endpoint<GetByIdAssessmentRequest, SpecificationPerformanceResponse<AssessmentEntityDto>>
{
    private readonly AssessmentService _assessmentService;
    private readonly IAppLoggerService<CompareAssessmentSpecificationPerformanceEndpoint> _logger;

    public CompareAssessmentSpecificationPerformanceEndpoint(
        AssessmentService assessmentService,
        IAppLoggerService<CompareAssessmentSpecificationPerformanceEndpoint> logger)
    {
        _assessmentService = assessmentService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/assessments/perf/{assessmentId}");
        AllowAnonymous();
        Description(d => d
            .Produces<SpecificationPerformanceResponse<AssessmentEntityDto>>(200, "application/json")
            .Produces(404)
            .Produces(400));
        Tags("Assessments");
        Summary(s =>
        {
            s.Summary = "Compare Assessment Specification Performance";
            s.Description = "Measures and compares the performance of different assessment specification types";
            s.ExampleRequest = new GetByIdAssessmentRequest { AssessmentId = Guid.NewGuid() };
        });
    }

    public override async Task HandleAsync(GetByIdAssessmentRequest req, CancellationToken ct)
    {
        _logger.LogInformation($"Starting performance test for assessment ID: {req.AssessmentId}");
        var response = await Observable
            .FromAsync(() => ProcessAssessmentSpecificationPerformance(req.AssessmentId, ct))
            .Take(1)
            .FirstOrDefaultAsync();

        if (response != null)
        {
            await SendOkAsync(response, ct);
        }
        else
        {
            await SendErrorsAsync(400, ct);
        }
    }

    private async Task<SpecificationPerformanceResponse<AssessmentEntityDto>> ProcessAssessmentSpecificationPerformance(
        Guid assessmentId,
        CancellationToken ct)
    {
        _logger.LogInformation($"Running performance tests for assessment: {assessmentId}");
        var response = new SpecificationPerformanceResponse<AssessmentEntityDto>(Guid.NewGuid());
        var result = await _assessmentService.RunAssessmentPerformanceTests(assessmentId, disableCache: true);

        return result.Match(
            Right: perfData =>
            {
                response.TimingResults = perfData.TimingResults;
                response.Statistics = perfData.Statistics;
                response.ComplexityAnalysis = perfData.ComplexityAnalysis;
                response.ErrorMessage = null;
                response.FastestSpecification = perfData.FastestSpecification;
                response.MostConsistentSpecification = perfData.MostConsistentSpecification;
                response.RecommendedSpecification = perfData.RecommendedSpecification;
                response.MaximumSpeedupFactor = perfData.MaximumSpeedupFactor;
                return response;
            },
            Left: errorMsg =>
            {
                response.ErrorMessage = errorMsg;
                return response;
            }
        );
    }
}
public class SearchAssessmentEndpoint
    : Endpoint<SearchAssessmentRequest, List<AssessmentDto>>
{
    private readonly AssessmentService _assessmentService;
    private readonly IAppLoggerService<SearchAssessmentEndpoint> _logger;

    public SearchAssessmentEndpoint(
        AssessmentService assessmentService,
        IAppLoggerService<SearchAssessmentEndpoint> logger
    )
    {
        _assessmentService = assessmentService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("/assessments/search");
        AllowAnonymous();
        Description(d =>
            d.Produces<List<AssessmentDto>>(200, "application/json").Produces(400)
        );
        Tags("Assessments");
        Summary(s =>
        {
            s.Summary = "Search assessments";
            s.Description = "Search assessments by various fields";
            s.ExampleRequest = new SearchAssessmentRequest
            {
                SearchTerm = "example",
            };
        });
    }

    public override async Task HandleAsync(
         SearchAssessmentRequest req,
        CancellationToken ct
    )
    {
        var result = await _assessmentService.SearchAssessmentsAsync(
            req.SearchTerm
        );
        List<ValidationFailure> validationFailures =
            new AutoConstructedList<ValidationFailure>();
        await result.Match(
            Right: async assessments => await SendOkAsync(assessments, ct),
            Left: async error => await SendErrorsAsync(400, ct)
        );
    }
}

public class FilterAssessmentEndpoint
    : Endpoint<FilteredAssessmentRequest, ListAssessmentResponse>
{
    private readonly AssessmentService _assessmentService;
    private readonly IAppLoggerService<FilterAssessmentEndpoint> _logger;

    public FilterAssessmentEndpoint(
        AssessmentService assessmentService,
        IAppLoggerService<FilterAssessmentEndpoint> logger
    )
    {
        _assessmentService = assessmentService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/assessments/filtered_list");
        AllowAnonymous();
        Description(d =>
            d.Produces<ListAssessmentResponse>(200, "application/json")
                .Produces(400)
        );
        Tags("Assessments");
        Summary(s =>
        {
            s.Summary = "Filter assessments";
            s.Description = "Get a filtered and sorted list of assessments";
            s.ExampleRequest = new FilteredAssessmentRequest
            {
                PageNumber = 1,
                PageSize = 10,
                Filters = new Dictionary<string, string> { { "status", "1" } },
                Sorting = new List<PCIShieldLib.SharedKernel.Interfaces.Sort>(new List<Sort>
                {
                    new()
                    {
                        Field = "CreatedDate",
                        Direction = SortDirection.Descending,
                    },
                }),
            };
        });
    }

    public override async Task HandleAsync(
        FilteredAssessmentRequest req,
        CancellationToken ct
    )
    {
        var result = await _assessmentService.GetFilteredAssessmentsAsync(req);
        await result.Match(
            Right: async response => await SendOkAsync(response, ct),
            Left: async error => await SendErrorsAsync(400, ct)
        );
    }
}
public class GetAssessmentListEndpoint : Endpoint<GetAssessmentListRequest, ListAssessmentResponse>
{
    private readonly AssessmentService _assessmentService;
    private readonly IAppLoggerService<GetAssessmentListEndpoint> _logger;

    public GetAssessmentListEndpoint(
        AssessmentService assessmentService,
        IAppLoggerService<GetAssessmentListEndpoint> logger)
    {
        _assessmentService = assessmentService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/assessments/paged_list");
        AllowAnonymous();
        Description(d =>
            d.Produces<ListAssessmentResponse>(200, "application/json")
                .Produces(400)
        );
        Tags("Assessments");
        Summary(s =>
        {
            s.Summary = "Get a paged list of Assessments";
            s.Description = "Gets a paginated list of Assessments with total count and pagination metadata";
            s.ExampleRequest = new GetAssessmentListRequest
            {
                PageNumber = 1,
                PageSize = 10,
            };
        });
    }

    public override async Task HandleAsync(
        GetAssessmentListRequest req,
        CancellationToken ct)
    {
        _logger.LogInformation($"Fetching assessment list. Page: {req.PageNumber}, Size: {req.PageSize}");
        var result = await _assessmentService.GetAssessmentPagedList(req.PageNumber, req.PageSize);
        await result.Match(
            async response =>
            {

                await SendOkAsync(response, ct);
            },
            async error =>
            {

                var errorResponse = new ListAssessmentResponse
                {
                    ErrorMessage = error,
                    Assessments = null,
                    PageNumber = req.PageNumber,
                    PageSize = req.PageSize
                };

                await SendAsync(errorResponse, 400, ct);
            }
        );
    }
}

public class UpdateAssessmentEndpoint
    : Endpoint<UpdateAssessmentRequest, UpdateAssessmentResponse>
{
    private readonly AssessmentService _assessmentService;
    private readonly IAppLoggerService<UpdateAssessmentEndpoint> _logger;

    public UpdateAssessmentEndpoint(
        AssessmentService assessmentService,
        IAppLoggerService<UpdateAssessmentEndpoint> logger
    )
    {
        _assessmentService = assessmentService;
        _logger = logger;
    }

    public override void Configure()
    {
        Put("/assessments");
        AllowAnonymous();
        Description(d =>
            d.Produces<UpdateAssessmentResponse>(200, "application/json")
                .Produces(404)
                .Produces(400)
        );
        Tags("Assessments");
        Summary(s =>
        {
            s.Summary = "Update an existing Assessment";
            s.Description = "Updates an existing Assessment's details";
            s.ExampleRequest = new UpdateAssessmentRequest
            {
                Assessment = new AssessmentDto
                {
                    AssessmentId = Guid.NewGuid(),
                    TenantId = Guid.Empty,
                    MerchantId = Guid.Empty,
                    AssessmentCode = string.Empty,
                    AssessmentType = 0,
                    AssessmentPeriod = string.Empty,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow,
                    Rank = 0,
                    QSAReviewRequired = false,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = Guid.Empty,
                    IsDeleted = false,
                }
            };
        });
    }

    public override async Task HandleAsync(
        UpdateAssessmentRequest req,
        CancellationToken ct
    )
    {
        var response = await Observable
            .FromAsync(() => ProcessUpdateAssessmentRequest(req, ct))
            .Take(1)
            .FirstOrDefaultAsync();
        if (response != null)
        {
            if (response.Assessment != null)
            {
                await SendOkAsync(response, ct);
            }
            else
            {
                if (response.ErrorMessage?.Contains("not found") == true)
                {
                    await SendNotFoundAsync(ct);
                }
                else
                {
                    await SendErrorsAsync(400, ct);
                }
            }
        }
        else
        {
            await SendErrorsAsync(500, ct);
        }
    }

    private async Task<UpdateAssessmentResponse> ProcessUpdateAssessmentRequest(UpdateAssessmentRequest req, CancellationToken ct)
    {
        _logger.LogInformation($"Updating assessment with ID: {req.Assessment.AssessmentId}");

        Either<string, AssessmentDto> result = await _assessmentService.UpdateAssessmentAsync(req.Assessment.AssessmentId, req.Assessment);

        return result.Match(
            Right: dto => new UpdateAssessmentResponse
            {
                Assessment = dto,
                ErrorMessage = null,
            },
            Left: err => new UpdateAssessmentResponse
            {
                Assessment = null,
                ErrorMessage = err,
            }
        );
    }
}
public class GetLastCreatedAssessmentControlEndpoint : EndpointWithoutRequest<GetByIdAssessmentControlResponse>
{
    private readonly AssessmentService _assessmentService;
    private readonly IAppLoggerService<GetLastCreatedAssessmentControlEndpoint> _logger;

    public GetLastCreatedAssessmentControlEndpoint(AssessmentService assessmentService,
        IAppLoggerService<GetLastCreatedAssessmentControlEndpoint> logger)
    {
        _assessmentService = assessmentService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("/assessmentControls/last-created");
        AllowAnonymous();
        Description(d => d
            .Produces<GetByIdAssessmentControlResponse>(200, "application/json")
            .Produces(404));
        Tags("AssessmentControls");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await _assessmentService.GetLastCreatedAssessmentControlAsync();
        await result.Match(
            Right: async assessmentControl => await SendOkAsync(new GetByIdAssessmentControlResponse { AssessmentControl = assessmentControl }, ct),
            Left: async error => await SendErrorsAsync(404, ct)
        );
    }
}

public class CreateAssessmentControlEndpoint : Endpoint<CreateAssessmentControlJoinRequest, CreateAssessmentControlResponse>
{
    private readonly AssessmentService _assessmentService;
    private readonly IAppLoggerService<CreateAssessmentControlEndpoint> _logger;

    public CreateAssessmentControlEndpoint(AssessmentService assessmentService, IAppLoggerService<CreateAssessmentControlEndpoint> logger)
    {
        _assessmentService = assessmentService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/assessmentControls/assessment");
        AllowAnonymous();
        Description(d => d.Produces<CreateAssessmentControlResponse>(201, "application/json").Produces(400));
        Tags("AssessmentControls");
        Summary(s =>
        {
            s.Summary = "Create a new AssessmentControl";
            s.Description = "Creates a new AssessmentControl";
            s.ExampleRequest = new CreateAssessmentControlJoinRequest { };
        });
    }

    public override async Task HandleAsync(CreateAssessmentControlJoinRequest req, CancellationToken ct)
    {
        var response = await Observable.FromAsync(() => ProcessCreateAssessmentControlRequest(req, ct)).Take(1).FirstOrDefaultAsync();
        if (response != null)
        {
            await SendCreatedAtAsync<CreateAssessmentControlEndpoint>(new { assessmentId = response.AssessmentControl.AssessmentId }, response, generateAbsoluteUrl: true, cancellation: ct);
        }
        else
        {
            await SendErrorsAsync(400, ct);
        }
    }

    private async Task<CreateAssessmentControlResponse> ProcessCreateAssessmentControlRequest(CreateAssessmentControlJoinRequest req, CancellationToken ct)
    {
        _logger.LogInformation("Creating new assessmentControl");
        Either<string, AssessmentControlDto> result = await _assessmentService.CreateAssessmentControlAsync(req);
        return result.Match(Right: dto => new CreateAssessmentControlResponse { AssessmentControl = dto, ErrorMessage = null }, 
            Left: err => new CreateAssessmentControlResponse { AssessmentControl = null, ErrorMessage = err });
    }
}

public class DeleteAssessmentControlEndpoint : Endpoint<DeleteAssessmentControlRequest, DeleteAssessmentControlResponse>
{
    private readonly AssessmentService _assessmentService;
    private readonly IAppLoggerService<DeleteAssessmentControlEndpoint> _logger;

    public DeleteAssessmentControlEndpoint(AssessmentService assessmentService, IAppLoggerService<DeleteAssessmentControlEndpoint> logger)
    {
        _assessmentService = assessmentService;
        _logger = logger;
    }

    public override void Configure()
    {
        Delete("/assessment/assessmentControls/{assessmentId}/{controlId}");
        AllowAnonymous();
        Description(d => d.Produces<DeleteAssessmentControlResponse>(200, "application/json").Produces(404).Produces(400));
        Tags("AssessmentControls");
        Summary(s =>
        {
            s.Summary = "Delete an existing AssessmentControl";
            s.Description = "Deletes an existing AssessmentControl";
        });
    }

    public override async Task HandleAsync(DeleteAssessmentControlRequest req, CancellationToken ct)
    {
        var response = await Observable.FromAsync(() => ProcessDeleteAssessmentControlRequest(req, ct)).Take(1).FirstOrDefaultAsync();
        if (response != null)
        {
            if (response.IsSuccess)
            {
                await SendOkAsync(response, ct);
            }
            else
            {
                await SendNotFoundAsync(ct);
            }
        }
        else
        {
            await SendErrorsAsync(400, ct);
        }
    }

    private async Task<DeleteAssessmentControlResponse> ProcessDeleteAssessmentControlRequest(DeleteAssessmentControlRequest req, CancellationToken ct)
    {
        _logger.LogInformation($"Deleting assessmentControl with ID: {req.AssessmentId}");
        Either<string, bool> result = await _assessmentService.DeleteAssessmentControlAsync(req);
        return result.Match(
            Right: success => new DeleteAssessmentControlResponse { IsSuccess = success, ErrorMessage = null },
            Left: err => new DeleteAssessmentControlResponse { IsSuccess = false, ErrorMessage = err }
        );
    }
}
public class GetAssessmentControlByIdWithIncludesEndpoint : Endpoint<GetByIdAssessmentControlRequest, GetByIdAssessmentControlResponse>
{
    private readonly AssessmentService _assessmentService;
    private readonly IAppLoggerService<GetAssessmentControlByIdWithIncludesEndpoint> _logger;

    public GetAssessmentControlByIdWithIncludesEndpoint(AssessmentService assessmentService, IAppLoggerService<GetAssessmentControlByIdWithIncludesEndpoint> logger)
    {
        _assessmentService = assessmentService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/assessmentControls/i/{assessmentId}");
        AllowAnonymous();
        Description(d => d.Produces<GetByIdAssessmentControlResponse>(200, "application/json").Produces(404).Produces(400));
        Tags("AssessmentControls");
        Summary(s =>
        {
            s.Summary = "Get an AssessmentControl by Id With Includes";
            s.Description = "Gets an AssessmentControl by Id With Includes";
        });
    }

    public override async Task HandleAsync(GetByIdAssessmentControlRequest req, CancellationToken ct)
    {
        var response = await Observable.FromAsync(() => ProcessAssessmentControlRequest(req, ct)).Take(1).FirstOrDefaultAsync();
        if (response != null)
        {
            await SendOkAsync(response, ct);
        }
        else
        {
            await SendErrorsAsync(400, ct);
        }
    }

    private async Task<GetByIdAssessmentControlResponse> ProcessAssessmentControlRequest(GetByIdAssessmentControlRequest req, CancellationToken ct)
    {
        _logger.LogInformation($"Fetching assessmentControl with ID: {req.AssessmentId}");
        Either<string, AssessmentControlDto> result = await _assessmentService.GetOneFullAssessmentControlByIdAsync(req);
        return result.Match(
            Right: dto => new GetByIdAssessmentControlResponse { AssessmentControl = dto, ErrorMessage = null },
            Left: err => new GetByIdAssessmentControlResponse { AssessmentControl = null, ErrorMessage = err }
        );
    }
}

public class SearchAssessmentControlEndpoint
    : Endpoint<SearchAssessmentControlRequest, List<AssessmentControlDto>>
{
    private readonly AssessmentService _assessmentService;
    private readonly IAppLoggerService<SearchAssessmentControlEndpoint> _logger;

    public SearchAssessmentControlEndpoint(
        AssessmentService assessmentService,
        IAppLoggerService<SearchAssessmentControlEndpoint> logger
    )
    {
        _assessmentService = assessmentService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("/assessmentControls/search");
        AllowAnonymous();
        Description(d =>
            d.Produces<List<AssessmentControlDto>>(200, "application/json").Produces(400)
        );
        Tags("AssessmentControls");
        Summary(s =>
        {
            s.Summary = "Search assessmentControls";
            s.Description = "Search assessmentControls by various fields";
        });
    }

    public override async Task HandleAsync(
        SearchAssessmentControlRequest req,
        CancellationToken ct
    )
    {
        var result = await _assessmentService.SearchAssessmentControlsAsync(
            req.SearchTerm
        );
        List<ValidationFailure> validationFailures =
            new AutoConstructedList<ValidationFailure>();
        await result.Match(
            Right: async assessmentControls => await SendOkAsync(assessmentControls, ct),
            Left: async error => await SendErrorsAsync(400, ct)
        );
    }
}

public class FilterAssessmentControlEndpoint
    : Endpoint<FilteredAssessmentControlRequest, ListAssessmentControlResponse>
{
    private readonly AssessmentService _assessmentService;
    private readonly IAppLoggerService<FilterAssessmentControlEndpoint> _logger;

    public FilterAssessmentControlEndpoint(
        AssessmentService assessmentService,
        IAppLoggerService<FilterAssessmentControlEndpoint> logger
    )
    {
        _assessmentService = assessmentService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/assessmentControls/filtered_list");
        AllowAnonymous();
        Description(d =>
            d.Produces<ListAssessmentControlResponse>(200, "application/json")
                .Produces(400)
        );
        Tags("AssessmentControls");
        Summary(s =>
        {
            s.Summary = "Filter assessmentControls";
            s.Description = "Get a filtered and sorted list of assessmentControls";
            s.ExampleRequest = new FilteredAssessmentControlRequest
            {
                PageNumber = 1,
                PageSize = 10,
                Filters = new Dictionary<string, string> { { "status", "1" } },
            };
        });
    }

    public override async Task HandleAsync(
        FilteredAssessmentControlRequest req,
        CancellationToken ct
    )
    {
        var result = await _assessmentService.GetFilteredAssessmentControlsAsync(req);
        await result.Match(
            Right: async response => await SendOkAsync(response, ct),
            Left: async error => await SendErrorsAsync(400, ct)
        );
    }
}

public class GetAssessmentControlListEndpoint
    : Endpoint<GetAssessmentControlListRequest, ListAssessmentControlResponse>
{
    private readonly AssessmentService _assessmentService;
    private readonly IAppLoggerService<GetAssessmentControlListEndpoint> _logger;

    public GetAssessmentControlListEndpoint(
        AssessmentService assessmentService,
        IAppLoggerService<GetAssessmentControlListEndpoint> logger
    )
    {
        _assessmentService = assessmentService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/assessmentControls/paged_list");
        AllowAnonymous();
        Description(d =>
            d.Produces<ListAssessmentControlResponse>(200, "application/json")
                .Produces(400)
        );
        Tags("AssessmentControls");
        Summary(s =>
        {
            s.Summary = "Get a paged list of AssessmentControls";
            s.Description = "Gets a paginated list of AssessmentControls";
            s.ExampleRequest = new GetAssessmentControlListRequest
            {
                PageNumber = 1,
                PageSize = 10,
            };
        });
    }

    public override async Task HandleAsync(
        GetAssessmentControlListRequest req,
        CancellationToken ct
    )
    {
        ListAssessmentControlResponse? response = await Observable
            .FromAsync(
                () =>
                    ProcessAssessmentControlListRequest(req.PageNumber, req.PageSize, ct)
            )
            .Take(1)
            .FirstOrDefaultAsync();
        if (response != null)
        {
            await SendOkAsync(response, ct);
        }
        else
        {
            await SendErrorsAsync(400, ct);
        }
    }

    private async Task<ListAssessmentControlResponse> ProcessAssessmentControlListRequest(
        int pageNumber,
        int pageSize,
        CancellationToken ct
    )
    {
        _logger.LogInformation(
            $"Fetching assessmentControl list. Page: {pageNumber}, Size: {pageSize}"
        );
        Either<string, List<AssessmentControlDto>> result =
            await _assessmentService.GetAssessmentControlPagedList(pageNumber, pageSize);
        return result.Match(
            assessmentControls => new ListAssessmentControlResponse
            {
                AssessmentControls = assessmentControls,
                ErrorMessage = null,
            },
            err => new ListAssessmentControlResponse
            {
                AssessmentControls = null,
                ErrorMessage = err,
            }
        );
    }
}

public class UpdateAssessmentControlEndpoint
    : Endpoint<UpdateAssessmentControlRequest, UpdateAssessmentControlResponse>
{
    private readonly AssessmentService _assessmentService;
    private readonly IAppLoggerService<UpdateAssessmentControlEndpoint> _logger;

    public UpdateAssessmentControlEndpoint(
        AssessmentService assessmentService,
        IAppLoggerService<UpdateAssessmentControlEndpoint> logger
    )
    {
        _assessmentService = assessmentService;
        _logger = logger;
    }

    public override void Configure()
    {
        Put("/assessmentControls");
        AllowAnonymous();
        Description(d =>
            d.Produces<UpdateAssessmentControlResponse>(200, "application/json")
                .Produces(404)
                .Produces(400)
        );
        Tags("AssessmentControls");
        Summary(s =>
        {
            s.Summary = "Update an existing AssessmentControl";
            s.Description = "Updates an existing AssessmentControl's details";
            s.ExampleRequest = new UpdateAssessmentControlRequest
            {
                AssessmentControl = new AssessmentControlDto
                {
                    ControlId = Guid.Empty,
                    AssessmentId = Guid.Empty,
                    IsDeleted = false,
                }
            };
        });
    }

    public override async Task HandleAsync(
        UpdateAssessmentControlRequest req,
        CancellationToken ct
    )
    {
        var response = await Observable
            .FromAsync(() => ProcessUpdateAssessmentControlRequest(req, ct))
            .Take(1)
            .FirstOrDefaultAsync();
        if (response != null)
        {
            if (response.AssessmentControl != null)
            {
                await SendOkAsync(response, ct);
            }
            else
            {
                if (response.ErrorMessage?.Contains("not found") == true)
                {
                    await SendNotFoundAsync(ct);
                }
                else
                {
                    await SendErrorsAsync(400, ct);
                }
            }
        }
        else
        {
            await SendErrorsAsync(500, ct);
        }
    }

    private async Task<UpdateAssessmentControlResponse> ProcessUpdateAssessmentControlRequest(UpdateAssessmentControlRequest req, CancellationToken ct)
    {
        _logger.LogInformation($"Updating assessmentControl with ID: {req.AssessmentControl.RowId}");

        Either<string, AssessmentControlDto> result = await _assessmentService.UpdateAssessmentControlAsync(req.AssessmentControl);

        return result.Match(
            Right: dto => new UpdateAssessmentControlResponse
            {
                AssessmentControl = dto,
                ErrorMessage = null,
            },
            Left: err => new UpdateAssessmentControlResponse
            {
                AssessmentControl = null,
                ErrorMessage = err,
            }
        );
    }
}
public class GetLastCreatedControlEvidenceEndpoint : EndpointWithoutRequest<GetByIdControlEvidenceResponse>
{
    private readonly AssessmentService _assessmentService;
    private readonly IAppLoggerService<GetLastCreatedControlEvidenceEndpoint> _logger;

    public GetLastCreatedControlEvidenceEndpoint(AssessmentService assessmentService,
        IAppLoggerService<GetLastCreatedControlEvidenceEndpoint> logger)
    {
        _assessmentService = assessmentService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("/controlEvidences/last-created");
        AllowAnonymous();
        Description(d => d
            .Produces<GetByIdControlEvidenceResponse>(200, "application/json")
            .Produces(404));
        Tags("ControlEvidences");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await _assessmentService.GetLastCreatedControlEvidenceAsync();
        await result.Match(
            Right: async controlEvidence => await SendOkAsync(new GetByIdControlEvidenceResponse { ControlEvidence = controlEvidence }, ct),
            Left: async error => await SendErrorsAsync(404, ct)
        );
    }
}

public class CreateControlEvidenceEndpoint : Endpoint<CreateControlEvidenceJoinRequest, CreateControlEvidenceResponse>
{
    private readonly AssessmentService _assessmentService;
    private readonly IAppLoggerService<CreateControlEvidenceEndpoint> _logger;

    public CreateControlEvidenceEndpoint(AssessmentService assessmentService, IAppLoggerService<CreateControlEvidenceEndpoint> logger)
    {
        _assessmentService = assessmentService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/controlEvidences/assessment");
        AllowAnonymous();
        Description(d => d.Produces<CreateControlEvidenceResponse>(201, "application/json").Produces(400));
        Tags("ControlEvidences");
        Summary(s =>
        {
            s.Summary = "Create a new ControlEvidence";
            s.Description = "Creates a new ControlEvidence";
            s.ExampleRequest = new CreateControlEvidenceJoinRequest { };
        });
    }

    public override async Task HandleAsync(CreateControlEvidenceJoinRequest req, CancellationToken ct)
    {
        var response = await Observable.FromAsync(() => ProcessCreateControlEvidenceRequest(req, ct)).Take(1).FirstOrDefaultAsync();
        if (response != null)
        {
            await SendCreatedAtAsync<CreateControlEvidenceEndpoint>(new { assessmentId = response.ControlEvidence.AssessmentId }, response, generateAbsoluteUrl: true, cancellation: ct);
        }
        else
        {
            await SendErrorsAsync(400, ct);
        }
    }

    private async Task<CreateControlEvidenceResponse> ProcessCreateControlEvidenceRequest(CreateControlEvidenceJoinRequest req, CancellationToken ct)
    {
        _logger.LogInformation("Creating new controlEvidence");
        Either<string, ControlEvidenceDto> result = await _assessmentService.CreateControlEvidenceAsync(req);
        return result.Match(Right: dto => new CreateControlEvidenceResponse { ControlEvidence = dto, ErrorMessage = null }, 
            Left: err => new CreateControlEvidenceResponse { ControlEvidence = null, ErrorMessage = err });
    }
}

public class GetControlEvidenceByIdWithIncludesEndpoint : Endpoint<GetByIdControlEvidenceRequest, GetByIdControlEvidenceResponse>
{
    private readonly AssessmentService _assessmentService;
    private readonly IAppLoggerService<GetControlEvidenceByIdWithIncludesEndpoint> _logger;

    public GetControlEvidenceByIdWithIncludesEndpoint(AssessmentService assessmentService, IAppLoggerService<GetControlEvidenceByIdWithIncludesEndpoint> logger)
    {
        _assessmentService = assessmentService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/controlEvidences/i/{assessmentId}");
        AllowAnonymous();
        Description(d => d.Produces<GetByIdControlEvidenceResponse>(200, "application/json").Produces(404).Produces(400));
        Tags("ControlEvidences");
        Summary(s =>
        {
            s.Summary = "Get an ControlEvidence by Id With Includes";
            s.Description = "Gets an ControlEvidence by Id With Includes";
        });
    }

    public override async Task HandleAsync(GetByIdControlEvidenceRequest req, CancellationToken ct)
    {
        var response = await Observable.FromAsync(() => ProcessControlEvidenceRequest(req, ct)).Take(1).FirstOrDefaultAsync();
        if (response != null)
        {
            await SendOkAsync(response, ct);
        }
        else
        {
            await SendErrorsAsync(400, ct);
        }
    }

    private async Task<GetByIdControlEvidenceResponse> ProcessControlEvidenceRequest(GetByIdControlEvidenceRequest req, CancellationToken ct)
    {
        _logger.LogInformation($"Fetching controlEvidence with ID: {req.AssessmentId}");
        Either<string, ControlEvidenceDto> result = await _assessmentService.GetOneFullControlEvidenceByIdAsync(req);
        return result.Match(
            Right: dto => new GetByIdControlEvidenceResponse { ControlEvidence = dto, ErrorMessage = null },
            Left: err => new GetByIdControlEvidenceResponse { ControlEvidence = null, ErrorMessage = err }
        );
    }
}

public class SearchControlEvidenceEndpoint
    : Endpoint<SearchControlEvidenceRequest, List<ControlEvidenceDto>>
{
    private readonly AssessmentService _assessmentService;
    private readonly IAppLoggerService<SearchControlEvidenceEndpoint> _logger;

    public SearchControlEvidenceEndpoint(
        AssessmentService assessmentService,
        IAppLoggerService<SearchControlEvidenceEndpoint> logger
    )
    {
        _assessmentService = assessmentService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("/controlEvidences/search");
        AllowAnonymous();
        Description(d =>
            d.Produces<List<ControlEvidenceDto>>(200, "application/json").Produces(400)
        );
        Tags("ControlEvidences");
        Summary(s =>
        {
            s.Summary = "Search controlEvidences";
            s.Description = "Search controlEvidences by various fields";
        });
    }

    public override async Task HandleAsync(
        SearchControlEvidenceRequest req,
        CancellationToken ct
    )
    {
        var result = await _assessmentService.SearchControlEvidencesAsync(
            req.SearchTerm
        );
        List<ValidationFailure> validationFailures =
            new AutoConstructedList<ValidationFailure>();
        await result.Match(
            Right: async controlEvidences => await SendOkAsync(controlEvidences, ct),
            Left: async error => await SendErrorsAsync(400, ct)
        );
    }
}

public class FilterControlEvidenceEndpoint
    : Endpoint<FilteredControlEvidenceRequest, ListControlEvidenceResponse>
{
    private readonly AssessmentService _assessmentService;
    private readonly IAppLoggerService<FilterControlEvidenceEndpoint> _logger;

    public FilterControlEvidenceEndpoint(
        AssessmentService assessmentService,
        IAppLoggerService<FilterControlEvidenceEndpoint> logger
    )
    {
        _assessmentService = assessmentService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/controlEvidences/filtered_list");
        AllowAnonymous();
        Description(d =>
            d.Produces<ListControlEvidenceResponse>(200, "application/json")
                .Produces(400)
        );
        Tags("ControlEvidences");
        Summary(s =>
        {
            s.Summary = "Filter controlEvidences";
            s.Description = "Get a filtered and sorted list of controlEvidences";
            s.ExampleRequest = new FilteredControlEvidenceRequest
            {
                PageNumber = 1,
                PageSize = 10,
                Filters = new Dictionary<string, string> { { "status", "1" } },
            };
        });
    }

    public override async Task HandleAsync(
        FilteredControlEvidenceRequest req,
        CancellationToken ct
    )
    {
        var result = await _assessmentService.GetFilteredControlEvidencesAsync(req);
        await result.Match(
            Right: async response => await SendOkAsync(response, ct),
            Left: async error => await SendErrorsAsync(400, ct)
        );
    }
}

public class GetControlEvidenceListEndpoint
    : Endpoint<GetControlEvidenceListRequest, ListControlEvidenceResponse>
{
    private readonly AssessmentService _assessmentService;
    private readonly IAppLoggerService<GetControlEvidenceListEndpoint> _logger;

    public GetControlEvidenceListEndpoint(
        AssessmentService assessmentService,
        IAppLoggerService<GetControlEvidenceListEndpoint> logger
    )
    {
        _assessmentService = assessmentService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/controlEvidences/paged_list");
        AllowAnonymous();
        Description(d =>
            d.Produces<ListControlEvidenceResponse>(200, "application/json")
                .Produces(400)
        );
        Tags("ControlEvidences");
        Summary(s =>
        {
            s.Summary = "Get a paged list of ControlEvidences";
            s.Description = "Gets a paginated list of ControlEvidences";
            s.ExampleRequest = new GetControlEvidenceListRequest
            {
                PageNumber = 1,
                PageSize = 10,
            };
        });
    }

    public override async Task HandleAsync(
        GetControlEvidenceListRequest req,
        CancellationToken ct
    )
    {
        ListControlEvidenceResponse? response = await Observable
            .FromAsync(
                () =>
                    ProcessControlEvidenceListRequest(req.PageNumber, req.PageSize, ct)
            )
            .Take(1)
            .FirstOrDefaultAsync();
        if (response != null)
        {
            await SendOkAsync(response, ct);
        }
        else
        {
            await SendErrorsAsync(400, ct);
        }
    }

    private async Task<ListControlEvidenceResponse> ProcessControlEvidenceListRequest(
        int pageNumber,
        int pageSize,
        CancellationToken ct
    )
    {
        _logger.LogInformation(
            $"Fetching controlEvidence list. Page: {pageNumber}, Size: {pageSize}"
        );
        Either<string, List<ControlEvidenceDto>> result =
            await _assessmentService.GetControlEvidencePagedList(pageNumber, pageSize);
        return result.Match(
            controlEvidences => new ListControlEvidenceResponse
            {
                ControlEvidences = controlEvidences,
                ErrorMessage = null,
            },
            err => new ListControlEvidenceResponse
            {
                ControlEvidences = null,
                ErrorMessage = err,
            }
        );
    }
}

public class UpdateControlEvidenceEndpoint
    : Endpoint<UpdateControlEvidenceRequest, UpdateControlEvidenceResponse>
{
    private readonly AssessmentService _assessmentService;
    private readonly IAppLoggerService<UpdateControlEvidenceEndpoint> _logger;

    public UpdateControlEvidenceEndpoint(
        AssessmentService assessmentService,
        IAppLoggerService<UpdateControlEvidenceEndpoint> logger
    )
    {
        _assessmentService = assessmentService;
        _logger = logger;
    }

    public override void Configure()
    {
        Put("/controlEvidences");
        AllowAnonymous();
        Description(d =>
            d.Produces<UpdateControlEvidenceResponse>(200, "application/json")
                .Produces(404)
                .Produces(400)
        );
        Tags("ControlEvidences");
        Summary(s =>
        {
            s.Summary = "Update an existing ControlEvidence";
            s.Description = "Updates an existing ControlEvidence's details";
            s.ExampleRequest = new UpdateControlEvidenceRequest
            {
                ControlEvidence = new ControlEvidenceDto
                {
                    EvidenceId = Guid.Empty,
                    AssessmentId = Guid.Empty,
                    IsPrimary = false,
                    IsDeleted = false,
                }
            };
        });
    }

    public override async Task HandleAsync(
        UpdateControlEvidenceRequest req,
        CancellationToken ct
    )
    {
        var response = await Observable
            .FromAsync(() => ProcessUpdateControlEvidenceRequest(req, ct))
            .Take(1)
            .FirstOrDefaultAsync();
        if (response != null)
        {
            if (response.ControlEvidence != null)
            {
                await SendOkAsync(response, ct);
            }
            else
            {
                if (response.ErrorMessage?.Contains("not found") == true)
                {
                    await SendNotFoundAsync(ct);
                }
                else
                {
                    await SendErrorsAsync(400, ct);
                }
            }
        }
        else
        {
            await SendErrorsAsync(500, ct);
        }
    }

    private async Task<UpdateControlEvidenceResponse> ProcessUpdateControlEvidenceRequest(UpdateControlEvidenceRequest req, CancellationToken ct)
    {
        _logger.LogInformation($"Updating controlEvidence with ID: {req.ControlEvidence.RowId}");

        Either<string, ControlEvidenceDto> result = await _assessmentService.UpdateControlEvidenceAsync(req.ControlEvidence);

        return result.Match(
            Right: dto => new UpdateControlEvidenceResponse
            {
                ControlEvidence = dto,
                ErrorMessage = null,
            },
            Left: err => new UpdateControlEvidenceResponse
            {
                ControlEvidence = null,
                ErrorMessage = err,
            }
        );
    }
}
public class CreateROCPackageEndpoint : Endpoint<CreateROCPackageRequest, CreateROCPackageResponse>
{
    private readonly AssessmentService _assessmentService;
    private readonly IAppLoggerService<CreateROCPackageEndpoint> _logger;

    public CreateROCPackageEndpoint(AssessmentService assessmentService, IAppLoggerService<CreateROCPackageEndpoint> logger)
    {
        _assessmentService = assessmentService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/rocpackages");
        AllowAnonymous();
        Description(d => d.Produces<CreateROCPackageResponse>(201, "application/json").Produces(400));
        Tags("ROCPackages");
        Summary(s =>
        {
            s.Summary = "Create a new ROCPackage";
            s.Description = "Creates a new ROCPackage";
            s.ExampleRequest = new CreateROCPackageRequest { };
        });
    }

    public override async Task HandleAsync(CreateROCPackageRequest req, CancellationToken ct)
    {
        var response = await Observable.FromAsync(() => ProcessCreateROCPackageRequest(req, ct)).Take(1).FirstOrDefaultAsync();
        if (response != null)
        {
            await SendCreatedAtAsync<CreateROCPackageEndpoint>(new { rocpackageId = response.ROCPackage.ROCPackageId }, response, generateAbsoluteUrl: true, cancellation: ct);
        }
        else
        {
            await SendErrorsAsync(400, ct);
        }
    }

    private async Task<CreateROCPackageResponse> ProcessCreateROCPackageRequest(CreateROCPackageRequest req, CancellationToken ct)
    {
        _logger.LogInformation("Creating new rocpackage");
        Either<string, ROCPackageDto> result = await _assessmentService.CreateROCPackageAsync(req.ROCPackage);
        return result.Match(Right: dto => new CreateROCPackageResponse { ROCPackage = dto, ErrorMessage = null }, Left: err => new CreateROCPackageResponse { ROCPackage = null, ErrorMessage = err });
    }
}

public class DeleteROCPackageEndpoint : Endpoint<DeleteROCPackageRequest, DeleteROCPackageResponse>
{
    private readonly AssessmentService _assessmentService;
    private readonly IAppLoggerService<DeleteROCPackageEndpoint> _logger;

    public DeleteROCPackageEndpoint(AssessmentService assessmentService, IAppLoggerService<DeleteROCPackageEndpoint> logger)
    {
        _assessmentService = assessmentService;
        _logger = logger;
    }

    public override void Configure()
    {
        Delete("/rocpackages/{rocpackageId}");
        AllowAnonymous();
        Description(d => d.Produces<DeleteROCPackageResponse>(200, "application/json").Produces(404).Produces(400));
        Tags("ROCPackages");
        Summary(s =>
        {
            s.Summary = "Delete an existing ROCPackage";
            s.Description = "Deletes an existing ROCPackage";
            s.ExampleRequest = new DeleteROCPackageRequest { ROCPackageId = Guid.NewGuid() };
        });
    }

    public override async Task HandleAsync(DeleteROCPackageRequest req, CancellationToken ct)
    {
        var response = await Observable.FromAsync(() => ProcessDeleteROCPackageRequest(req.ROCPackageId, ct)).Take(1).FirstOrDefaultAsync();
        if (response != null)
        {
            if (response.IsSuccess)
            {
                await SendOkAsync(response, ct);
            }
            else
            {
                await SendNotFoundAsync(ct);
            }
        }
        else
        {
            await SendErrorsAsync(400, ct);
        }
    }

    private async Task<DeleteROCPackageResponse> ProcessDeleteROCPackageRequest(Guid rocpackageId, CancellationToken ct)
    {
        _logger.LogInformation($"Deleting rocpackage with ID: {rocpackageId}");
        Either<string, bool> result = await _assessmentService.DeleteROCPackageAsync(rocpackageId);
        return result.Match(
            Right: success => new DeleteROCPackageResponse { IsSuccess = success, ErrorMessage = null },
            Left: err => new DeleteROCPackageResponse { IsSuccess = false, ErrorMessage = err }
        );
    }
}
public class GetROCPackageByIdWithIncludesEndpoint : Endpoint<GetByIdROCPackageRequest, GetByIdROCPackageResponse>
{
    private readonly AssessmentService _assessmentService;
    private readonly IAppLoggerService<GetROCPackageByIdWithIncludesEndpoint> _logger;

    public GetROCPackageByIdWithIncludesEndpoint(AssessmentService assessmentService, IAppLoggerService<GetROCPackageByIdWithIncludesEndpoint> logger)
    {
        _assessmentService = assessmentService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/rocpackages/i/{rocpackageId}");
        AllowAnonymous();
        Description(d => d.Produces<GetByIdROCPackageResponse>(200, "application/json").Produces(404).Produces(400));
        Tags("ROCPackages");
        Summary(s =>
        {
            s.Summary = "Get an ROCPackage by Id With Includes";
            s.Description = "Gets an ROCPackage by Id With Includes";
            s.ExampleRequest = new GetByIdROCPackageRequest { ROCPackageId = Guid.NewGuid() };
        });
    }

    public override async Task HandleAsync(GetByIdROCPackageRequest req, CancellationToken ct)
    {
        var response = await Observable.FromAsync(() => ProcessROCPackageRequest(req.ROCPackageId, ct)).Take(1).FirstOrDefaultAsync();
        if (response != null)
        {
            await SendOkAsync(response, ct);
        }
        else
        {
            await SendErrorsAsync(400, ct);
        }
    }

    private async Task<GetByIdROCPackageResponse> ProcessROCPackageRequest(Guid rocpackageId, CancellationToken ct)
    {
        _logger.LogInformation($"Fetching rocpackage with ID: {rocpackageId}");
        Either<string, ROCPackageDto> result = await _assessmentService.GetOneFullROCPackageByIdAsync(rocpackageId);
        return result.Match(
            Right: dto => new GetByIdROCPackageResponse { ROCPackage = dto, ErrorMessage = null },
            Left: err => new GetByIdROCPackageResponse { ROCPackage = null, ErrorMessage = err }
        );
    }
}

public class SearchROCPackageEndpoint
    : Endpoint<SearchROCPackageRequest, List<ROCPackageDto>>
{
    private readonly AssessmentService _assessmentService;
    private readonly IAppLoggerService<SearchROCPackageEndpoint> _logger;

    public SearchROCPackageEndpoint(
        AssessmentService assessmentService,
        IAppLoggerService<SearchROCPackageEndpoint> logger
    )
    {
        _assessmentService = assessmentService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("/rocpackages/search");
        AllowAnonymous();
        Description(d =>
            d.Produces<List<ROCPackageDto>>(200, "application/json").Produces(400)
        );
        Tags("ROCPackages");
        Summary(s =>
        {
            s.Summary = "Search rocpackages";
            s.Description = "Search rocpackages by various fields";
            s.ExampleRequest = new SearchROCPackageRequest
            {
                SearchTerm = "example",
            };
        });
    }

    public override async Task HandleAsync(
         SearchROCPackageRequest req,
        CancellationToken ct
    )
    {
        var result = await _assessmentService.SearchROCPackagesAsync(
            req.SearchTerm
        );
        List<ValidationFailure> validationFailures =
            new AutoConstructedList<ValidationFailure>();
        await result.Match(
            Right: async rocpackages => await SendOkAsync(rocpackages, ct),
            Left: async error => await SendErrorsAsync(400, ct)
        );
    }
}

public class FilterROCPackageEndpoint
    : Endpoint<FilteredROCPackageRequest, ListROCPackageResponse>
{
    private readonly AssessmentService _assessmentService;
    private readonly IAppLoggerService<FilterROCPackageEndpoint> _logger;

    public FilterROCPackageEndpoint(
        AssessmentService assessmentService,
        IAppLoggerService<FilterROCPackageEndpoint> logger
    )
    {
        _assessmentService = assessmentService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/rocpackages/filtered_list");
        AllowAnonymous();
        Description(d =>
            d.Produces<ListROCPackageResponse>(200, "application/json")
                .Produces(400)
        );
        Tags("ROCPackages");
        Summary(s =>
        {
            s.Summary = "Filter rocpackages";
            s.Description = "Get a filtered and sorted list of rocpackages";
            s.ExampleRequest = new FilteredROCPackageRequest
            {
                PageNumber = 1,
                PageSize = 10,
                Filters = new Dictionary<string, string> { { "status", "1" } },
                Sorting = new List<PCIShieldLib.SharedKernel.Interfaces.Sort>(new List<Sort>
                {
                    new()
                    {
                        Field = "CreatedDate",
                        Direction = SortDirection.Descending,
                    },
                }),
            };
        });
    }

    public override async Task HandleAsync(
        FilteredROCPackageRequest req,
        CancellationToken ct
    )
    {
        var result = await _assessmentService.GetFilteredROCPackagesAsync(req);
        await result.Match(
            Right: async response => await SendOkAsync(response, ct),
            Left: async error => await SendErrorsAsync(400, ct)
        );
    }
}

public class GetROCPackageListEndpoint
    : Endpoint<GetROCPackageListRequest, ListROCPackageResponse>
{
    private readonly AssessmentService _assessmentService;
    private readonly IAppLoggerService<GetROCPackageListEndpoint> _logger;

    public GetROCPackageListEndpoint(
        AssessmentService assessmentService,
        IAppLoggerService<GetROCPackageListEndpoint> logger
    )
    {
        _assessmentService = assessmentService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/rocpackages/paged_list");
        AllowAnonymous();
        Description(d =>
            d.Produces<ListROCPackageResponse>(200, "application/json")
                .Produces(400)
        );
        Tags("ROCPackages");
        Summary(s =>
        {
            s.Summary = "Get a paged list of ROCPackages";
            s.Description = "Gets a paginated list of ROCPackages";
            s.ExampleRequest = new GetROCPackageListRequest
            {
                PageNumber = 1,
                PageSize = 10,
            };
        });
    }

    public override async Task HandleAsync(
        GetROCPackageListRequest req,
        CancellationToken ct
    )
    {
        ListROCPackageResponse? response = await Observable
            .FromAsync(
                () =>
                    ProcessROCPackageListRequest(req.PageNumber, req.PageSize, ct)
            )
            .Take(1)
            .FirstOrDefaultAsync();
        if (response != null)
        {
            await SendOkAsync(response, ct);
        }
        else
        {
            await SendErrorsAsync(400, ct);
        }
    }

    private async Task<ListROCPackageResponse> ProcessROCPackageListRequest(
        int pageNumber,
        int pageSize,
        CancellationToken ct
    )
    {
        _logger.LogInformation(
            $"Fetching rocpackage list. Page: {pageNumber}, Size: {pageSize}"
        );
        Either<string, List<ROCPackageDto>> result =
            await _assessmentService.GetROCPackagePagedList(pageNumber, pageSize);
        return result.Match(
            rocpackages => new ListROCPackageResponse
            {
                ROCPackages = rocpackages,
                ErrorMessage = null,
            },
            err => new ListROCPackageResponse
            {
                ROCPackages = null,
                ErrorMessage = err,
            }
        );
    }
}
public class UpdateROCPackageEndpoint
    : Endpoint<UpdateROCPackageRequest, UpdateROCPackageResponse>
{
    private readonly AssessmentService _assessmentService;
    private readonly IAppLoggerService<UpdateROCPackageEndpoint> _logger;

    public UpdateROCPackageEndpoint(
        AssessmentService assessmentService,
        IAppLoggerService<UpdateROCPackageEndpoint> logger
    )
    {
        _assessmentService = assessmentService;
        _logger = logger;
    }

    public override void Configure()
    {
        Put("/rocpackages");
        AllowAnonymous();
        Description(d =>
            d.Produces<UpdateROCPackageResponse>(200, "application/json")
                .Produces(404)
                .Produces(400)
        );
        Tags("ROCPackages");
        Summary(s =>
        {
            s.Summary = "Update an existing ROCPackage";
            s.Description = "Updates an existing ROCPackage's details";
            s.ExampleRequest = new UpdateROCPackageRequest
            {
                ROCPackage = new ROCPackageDto
                {
                    ROCPackageId = Guid.NewGuid(),
                    TenantId = Guid.Empty,
                    AssessmentId = Guid.Empty,
                    PackageVersion = string.Empty,
                    GeneratedDate = DateTime.UtcNow,
                    Rank = 0,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = Guid.Empty,
              
                    IsDeleted = false,
                }
            };
        });
    }

    public override async Task HandleAsync(
        UpdateROCPackageRequest req,
        CancellationToken ct
    )
    {
        var response = await Observable
            .FromAsync(() => ProcessUpdateROCPackageRequest(req, ct))
            .Take(1)
            .FirstOrDefaultAsync();
        if (response != null)
        {
            if (response.ROCPackage != null)
            {
                await SendOkAsync(response, ct);
            }
            else
            {
                if (response.ErrorMessage?.Contains("not found") == true)
                {
                    await SendNotFoundAsync(ct);
                }
                else
                {
                    await SendErrorsAsync(400, ct);
                }
            }
        }
        else
        {
            await SendErrorsAsync(500, ct);
        }
    }

    private async Task<UpdateROCPackageResponse> ProcessUpdateROCPackageRequest(UpdateROCPackageRequest req, CancellationToken ct)
    {
        _logger.LogInformation($"Updating rocpackage with ID: {req.ROCPackage.ROCPackageId}");

        Either<string, ROCPackageDto> result = await _assessmentService.UpdateROCPackageAsync(req.ROCPackage.ROCPackageId, req.ROCPackage);

        return result.Match(
            Right: dto => new UpdateROCPackageResponse
            {
                ROCPackage = dto,
                ErrorMessage = null,
            },
            Left: err => new UpdateROCPackageResponse
            {
                ROCPackage = null,
                ErrorMessage = err,
            }
        );
    }
}

public class FilterScanScheduleEndpoint
    : Endpoint<FilteredScanScheduleRequest, ListScanScheduleResponse>
{
    private readonly AssessmentService _assessmentService;
    private readonly IAppLoggerService<FilterScanScheduleEndpoint> _logger;

    public FilterScanScheduleEndpoint(
        AssessmentService assessmentService,
        IAppLoggerService<FilterScanScheduleEndpoint> logger
    )
    {
        _assessmentService = assessmentService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/scanSchedules/filtered_list");
        AllowAnonymous();
        Description(d =>
            d.Produces<ListScanScheduleResponse>(200, "application/json")
                .Produces(400)
        );
        Tags("ScanSchedules");
        Summary(s =>
        {
            s.Summary = "Filter scanSchedules";
            s.Description = "Get a filtered and sorted list of scanSchedules";
            s.ExampleRequest = new FilteredScanScheduleRequest
            {
                PageNumber = 1,
                PageSize = 10,
                Filters = new Dictionary<string, string> { { "status", "1" } },
                Sorting = new List<PCIShieldLib.SharedKernel.Interfaces.Sort>(new List<Sort>
                {
                    new()
                    {
                        Direction = SortDirection.Descending,
                    },
                }),
            };
        });
    }

    public override async Task HandleAsync(
        FilteredScanScheduleRequest req,
        CancellationToken ct
    )
    {
        var result = await _assessmentService.GetFilteredScanSchedulesAsync(req);
        await result.Match(
            Right: async response => await SendOkAsync(response, ct),
            Left: async error => await SendErrorsAsync(400, ct)
        );
    }
}
public class GetScanScheduleListEndpoint
    : Endpoint<GetScanScheduleListRequest, ListScanScheduleResponse>
{
    private readonly AssessmentService _assessmentService;
    private readonly IAppLoggerService<GetScanScheduleListEndpoint> _logger;

    public GetScanScheduleListEndpoint(
        AssessmentService assessmentService,
        IAppLoggerService<GetScanScheduleListEndpoint> logger
    )
    {
        _assessmentService = assessmentService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/scanSchedules/paged_list");
        AllowAnonymous();
        Description(d =>
            d.Produces<ListScanScheduleResponse>(200, "application/json")
                .Produces(400)
        );
        Tags("ScanSchedules");
        Summary(s =>
        {
            s.Summary = "Get a paged list of ScanSchedules";
            s.Description = "Gets a paginated list of ScanSchedules";
            s.ExampleRequest = new GetScanScheduleListRequest
            {
                PageNumber = 1,
                PageSize = 10,
            };
        });
    }

    public override async Task HandleAsync(
        GetScanScheduleListRequest req,
        CancellationToken ct
    )
    {
        ListScanScheduleResponse? response = await Observable
            .FromAsync(
                () =>
                    ProcessScanScheduleListRequest(

                        req.PageNumber,
                        req.PageSize,
                        req.AssetId,

                        ct)
            )
            .Take(1)
            .FirstOrDefaultAsync();
        if (response != null)
        {
            await SendOkAsync(response, ct);
        }
        else
        {
            await SendErrorsAsync(400, ct);
        }
    }

    private async Task<ListScanScheduleResponse> ProcessScanScheduleListRequest(

        int pageNumber,
        int pageSize,
        Guid? assetId,

        CancellationToken ct)
    {
        _logger.LogInformation(
            $"Fetching scanSchedule list. Page: {pageNumber}, Size: {pageSize}"
        );
        Either<string, List<ScanScheduleDto>> result =
            await _assessmentService.GetScanSchedulePagedList(

                pageNumber,
                pageSize,
                assetId
                );

        return result.Match(
            scanSchedules => new ListScanScheduleResponse
            {
                ScanSchedules = scanSchedules,
                ErrorMessage = null,
            },
            err => new ListScanScheduleResponse
            {
                ScanSchedules = null,
                ErrorMessage = err,
            }
        );
    }
}
public class SearchScanScheduleEndpoint
    : Endpoint<SearchScanScheduleRequest, List<ScanScheduleDto>>
{
    private readonly AssessmentService _assessmentService;
    private readonly IAppLoggerService<SearchScanScheduleEndpoint> _logger;

    public SearchScanScheduleEndpoint(
        AssessmentService assessmentService,
        IAppLoggerService<SearchScanScheduleEndpoint> logger
    )
    {
        _assessmentService = assessmentService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("/scanSchedules/search");
        AllowAnonymous();
        Description(d =>
            d.Produces<List<ScanScheduleDto>>(200, "application/json").Produces(400)
        );
        Tags("ScanSchedules");
        Summary(s =>
        {
            s.Summary = "Search scanSchedules";
            s.Description = "Search scanSchedules by various fields";
            s.ExampleRequest = new SearchScanScheduleRequest
            {
                SearchTerm = "example",
            };
        });
    }

    public override async Task HandleAsync(
         SearchScanScheduleRequest req,
        CancellationToken ct
    )
    {
        var result = await _assessmentService.SearchScanSchedulesAsync(
            req.SearchTerm
        );
        List<ValidationFailure> validationFailures =
            new AutoConstructedList<ValidationFailure>();
        await result.Match(
            Right: async scanSchedules => await SendOkAsync(scanSchedules, ct),
            Left: async error => await SendErrorsAsync(400, ct)
        );
    }
}
public class FilterVulnerabilityEndpoint
    : Endpoint<FilteredVulnerabilityRequest, ListVulnerabilityResponse>
{
    private readonly AssessmentService _assessmentService;
    private readonly IAppLoggerService<FilterVulnerabilityEndpoint> _logger;

    public FilterVulnerabilityEndpoint(
        AssessmentService assessmentService,
        IAppLoggerService<FilterVulnerabilityEndpoint> logger
    )
    {
        _assessmentService = assessmentService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/vulnerabilities/filtered_list");
        AllowAnonymous();
        Description(d =>
            d.Produces<ListVulnerabilityResponse>(200, "application/json")
                .Produces(400)
        );
        Tags("Vulnerabilities");
        Summary(s =>
        {
            s.Summary = "Filter vulnerabilities";
            s.Description = "Get a filtered and sorted list of vulnerabilities";
            s.ExampleRequest = new FilteredVulnerabilityRequest
            {
                PageNumber = 1,
                PageSize = 10,
                Filters = new Dictionary<string, string> { { "status", "1" } },
                Sorting = new List<PCIShieldLib.SharedKernel.Interfaces.Sort>(new List<Sort>
                {
                    new()
                    {
                        Direction = SortDirection.Descending,
                    },
                }),
            };
        });
    }

    public override async Task HandleAsync(
        FilteredVulnerabilityRequest req,
        CancellationToken ct
    )
    {
        var result = await _assessmentService.GetFilteredVulnerabilitiesAsync(req);
        await result.Match(
            Right: async response => await SendOkAsync(response, ct),
            Left: async error => await SendErrorsAsync(400, ct)
        );
    }
}
public class GetVulnerabilityListEndpoint
    : Endpoint<GetVulnerabilityListRequest, ListVulnerabilityResponse>
{
    private readonly AssessmentService _assessmentService;
    private readonly IAppLoggerService<GetVulnerabilityListEndpoint> _logger;

    public GetVulnerabilityListEndpoint(
        AssessmentService assessmentService,
        IAppLoggerService<GetVulnerabilityListEndpoint> logger
    )
    {
        _assessmentService = assessmentService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/vulnerabilities/paged_list");
        AllowAnonymous();
        Description(d =>
            d.Produces<ListVulnerabilityResponse>(200, "application/json")
                .Produces(400)
        );
        Tags("Vulnerabilities");
        Summary(s =>
        {
            s.Summary = "Get a paged list of Vulnerabilities";
            s.Description = "Gets a paginated list of Vulnerabilities";
            s.ExampleRequest = new GetVulnerabilityListRequest
            {
                PageNumber = 1,
                PageSize = 10,
            };
        });
    }

    public override async Task HandleAsync(
        GetVulnerabilityListRequest req,
        CancellationToken ct
    )
    {
        ListVulnerabilityResponse? response = await Observable
            .FromAsync(
                () =>
                    ProcessVulnerabilityListRequest(

                        req.PageNumber,
                        req.PageSize,
                        req.AssetId,

                        ct)
            )
            .Take(1)
            .FirstOrDefaultAsync();
        if (response != null)
        {
            await SendOkAsync(response, ct);
        }
        else
        {
            await SendErrorsAsync(400, ct);
        }
    }

    private async Task<ListVulnerabilityResponse> ProcessVulnerabilityListRequest(

        int pageNumber,
        int pageSize,
        Guid? assetId,

        CancellationToken ct)
    {
        _logger.LogInformation(
            $"Fetching vulnerability list. Page: {pageNumber}, Size: {pageSize}"
        );
        Either<string, List<VulnerabilityDto>> result =
            await _assessmentService.GetVulnerabilityPagedList(

                pageNumber,
                pageSize,
                assetId
                );

        return result.Match(
            vulnerabilities => new ListVulnerabilityResponse
            {
                Vulnerabilities = vulnerabilities,
                ErrorMessage = null,
            },
            err => new ListVulnerabilityResponse
            {
                Vulnerabilities = null,
                ErrorMessage = err,
            }
        );
    }
}
public class SearchVulnerabilityEndpoint
    : Endpoint<SearchVulnerabilityRequest, List<VulnerabilityDto>>
{
    private readonly AssessmentService _assessmentService;
    private readonly IAppLoggerService<SearchVulnerabilityEndpoint> _logger;

    public SearchVulnerabilityEndpoint(
        AssessmentService assessmentService,
        IAppLoggerService<SearchVulnerabilityEndpoint> logger
    )
    {
        _assessmentService = assessmentService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("/vulnerabilities/search");
        AllowAnonymous();
        Description(d =>
            d.Produces<List<VulnerabilityDto>>(200, "application/json").Produces(400)
        );
        Tags("Vulnerabilities");
        Summary(s =>
        {
            s.Summary = "Search vulnerabilities";
            s.Description = "Search vulnerabilities by various fields";
            s.ExampleRequest = new SearchVulnerabilityRequest
            {
                SearchTerm = "example",
            };
        });
    }

    public override async Task HandleAsync(
         SearchVulnerabilityRequest req,
        CancellationToken ct
    )
    {
        var result = await _assessmentService.SearchVulnerabilitiesAsync(
            req.SearchTerm
        );
        List<ValidationFailure> validationFailures =
            new AutoConstructedList<ValidationFailure>();
        await result.Match(
            Right: async vulnerabilities => await SendOkAsync(vulnerabilities, ct),
            Left: async error => await SendErrorsAsync(400, ct)
        );
    }
}
public class FilterPaymentPageEndpoint
    : Endpoint<FilteredPaymentPageRequest, ListPaymentPageResponse>
{
    private readonly AssessmentService _assessmentService;
    private readonly IAppLoggerService<FilterPaymentPageEndpoint> _logger;

    public FilterPaymentPageEndpoint(
        AssessmentService assessmentService,
        IAppLoggerService<FilterPaymentPageEndpoint> logger
    )
    {
        _assessmentService = assessmentService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/paymentPages/filtered_list");
        AllowAnonymous();
        Description(d =>
            d.Produces<ListPaymentPageResponse>(200, "application/json")
                .Produces(400)
        );
        Tags("PaymentPages");
        Summary(s =>
        {
            s.Summary = "Filter paymentPages";
            s.Description = "Get a filtered and sorted list of paymentPages";
            s.ExampleRequest = new FilteredPaymentPageRequest
            {
                PageNumber = 1,
                PageSize = 10,
                Filters = new Dictionary<string, string> { { "status", "1" } },
                Sorting = new List<PCIShieldLib.SharedKernel.Interfaces.Sort>(new List<Sort>
                {
                    new()
                    {
                        Direction = SortDirection.Descending,
                    },
                }),
            };
        });
    }

    public override async Task HandleAsync(
        FilteredPaymentPageRequest req,
        CancellationToken ct
    )
    {
        var result = await _assessmentService.GetFilteredPaymentPagesAsync(req);
        await result.Match(
            Right: async response => await SendOkAsync(response, ct),
            Left: async error => await SendErrorsAsync(400, ct)
        );
    }
}
public class GetPaymentPageListEndpoint
    : Endpoint<GetPaymentPageListRequest, ListPaymentPageResponse>
{
    private readonly AssessmentService _assessmentService;
    private readonly IAppLoggerService<GetPaymentPageListEndpoint> _logger;

    public GetPaymentPageListEndpoint(
        AssessmentService assessmentService,
        IAppLoggerService<GetPaymentPageListEndpoint> logger
    )
    {
        _assessmentService = assessmentService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/paymentPages/paged_list");
        AllowAnonymous();
        Description(d =>
            d.Produces<ListPaymentPageResponse>(200, "application/json")
                .Produces(400)
        );
        Tags("PaymentPages");
        Summary(s =>
        {
            s.Summary = "Get a paged list of PaymentPages";
            s.Description = "Gets a paginated list of PaymentPages";
            s.ExampleRequest = new GetPaymentPageListRequest
            {
                PageNumber = 1,
                PageSize = 10,
            };
        });
    }

    public override async Task HandleAsync(
        GetPaymentPageListRequest req,
        CancellationToken ct
    )
    {
        ListPaymentPageResponse? response = await Observable
            .FromAsync(
                () =>
                    ProcessPaymentPageListRequest(

                        req.PageNumber,
                        req.PageSize,
                        req.PaymentChannelId,

                        ct)
            )
            .Take(1)
            .FirstOrDefaultAsync();
        if (response != null)
        {
            await SendOkAsync(response, ct);
        }
        else
        {
            await SendErrorsAsync(400, ct);
        }
    }

    private async Task<ListPaymentPageResponse> ProcessPaymentPageListRequest(

        int pageNumber,
        int pageSize,
        Guid? paymentChannelId,

        CancellationToken ct)
    {
        _logger.LogInformation(
            $"Fetching paymentPage list. Page: {pageNumber}, Size: {pageSize}"
        );
        Either<string, List<PaymentPageDto>> result =
            await _assessmentService.GetPaymentPagePagedList(

                pageNumber,
                pageSize,
                paymentChannelId
                );

        return result.Match(
            paymentPages => new ListPaymentPageResponse
            {
                PaymentPages = paymentPages,
                ErrorMessage = null,
            },
            err => new ListPaymentPageResponse
            {
                PaymentPages = null,
                ErrorMessage = err,
            }
        );
    }
}
public class SearchPaymentPageEndpoint
    : Endpoint<SearchPaymentPageRequest, List<PaymentPageDto>>
{
    private readonly AssessmentService _assessmentService;
    private readonly IAppLoggerService<SearchPaymentPageEndpoint> _logger;

    public SearchPaymentPageEndpoint(
        AssessmentService assessmentService,
        IAppLoggerService<SearchPaymentPageEndpoint> logger
    )
    {
        _assessmentService = assessmentService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("/paymentPages/search");
        AllowAnonymous();
        Description(d =>
            d.Produces<List<PaymentPageDto>>(200, "application/json").Produces(400)
        );
        Tags("PaymentPages");
        Summary(s =>
        {
            s.Summary = "Search paymentPages";
            s.Description = "Search paymentPages by various fields";
            s.ExampleRequest = new SearchPaymentPageRequest
            {
                SearchTerm = "example",
            };
        });
    }

    public override async Task HandleAsync(
         SearchPaymentPageRequest req,
        CancellationToken ct
    )
    {
        var result = await _assessmentService.SearchPaymentPagesAsync(
            req.SearchTerm
        );
        List<ValidationFailure> validationFailures =
            new AutoConstructedList<ValidationFailure>();
        await result.Match(
            Right: async paymentPages => await SendOkAsync(paymentPages, ct),
            Left: async error => await SendErrorsAsync(400, ct)
        );
    }
}
public class FilterScriptEndpoint
    : Endpoint<FilteredScriptRequest, ListScriptResponse>
{
    private readonly AssessmentService _assessmentService;
    private readonly IAppLoggerService<FilterScriptEndpoint> _logger;

    public FilterScriptEndpoint(
        AssessmentService assessmentService,
        IAppLoggerService<FilterScriptEndpoint> logger
    )
    {
        _assessmentService = assessmentService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/scripts/filtered_list");
        AllowAnonymous();
        Description(d =>
            d.Produces<ListScriptResponse>(200, "application/json")
                .Produces(400)
        );
        Tags("Scripts");
        Summary(s =>
        {
            s.Summary = "Filter scripts";
            s.Description = "Get a filtered and sorted list of scripts";
            s.ExampleRequest = new FilteredScriptRequest
            {
                PageNumber = 1,
                PageSize = 10,
                Filters = new Dictionary<string, string> { { "status", "1" } },
                Sorting = new List<PCIShieldLib.SharedKernel.Interfaces.Sort>(new List<Sort>
                {
                    new()
                    {
                        Direction = SortDirection.Descending,
                    },
                }),
            };
        });
    }

    public override async Task HandleAsync(
        FilteredScriptRequest req,
        CancellationToken ct
    )
    {
        var result = await _assessmentService.GetFilteredScriptsAsync(req);
        await result.Match(
            Right: async response => await SendOkAsync(response, ct),
            Left: async error => await SendErrorsAsync(400, ct)
        );
    }
}
public class GetScriptListEndpoint
    : Endpoint<GetScriptListRequest, ListScriptResponse>
{
    private readonly AssessmentService _assessmentService;
    private readonly IAppLoggerService<GetScriptListEndpoint> _logger;

    public GetScriptListEndpoint(
        AssessmentService assessmentService,
        IAppLoggerService<GetScriptListEndpoint> logger
    )
    {
        _assessmentService = assessmentService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/scripts/paged_list");
        AllowAnonymous();
        Description(d =>
            d.Produces<ListScriptResponse>(200, "application/json")
                .Produces(400)
        );
        Tags("Scripts");
        Summary(s =>
        {
            s.Summary = "Get a paged list of Scripts";
            s.Description = "Gets a paginated list of Scripts";
            s.ExampleRequest = new GetScriptListRequest
            {
                PageNumber = 1,
                PageSize = 10,
            };
        });
    }

    public override async Task HandleAsync(
        GetScriptListRequest req,
        CancellationToken ct
    )
    {
        ListScriptResponse? response = await Observable
            .FromAsync(
                () =>
                    ProcessScriptListRequest(

                        req.PageNumber,
                        req.PageSize,
                        req.PaymentPageId,

                        ct)
            )
            .Take(1)
            .FirstOrDefaultAsync();
        if (response != null)
        {
            await SendOkAsync(response, ct);
        }
        else
        {
            await SendErrorsAsync(400, ct);
        }
    }

    private async Task<ListScriptResponse> ProcessScriptListRequest(

        int pageNumber,
        int pageSize,
        Guid? paymentPageId,

        CancellationToken ct)
    {
        _logger.LogInformation(
            $"Fetching script list. Page: {pageNumber}, Size: {pageSize}"
        );
        Either<string, List<ScriptDto>> result =
            await _assessmentService.GetScriptPagedList(

                pageNumber,
                pageSize,
                paymentPageId
                );

        return result.Match(
            scripts => new ListScriptResponse
            {
                Scripts = scripts,
                ErrorMessage = null,
            },
            err => new ListScriptResponse
            {
                Scripts = null,
                ErrorMessage = err,
            }
        );
    }
}
public class SearchScriptEndpoint
    : Endpoint<SearchScriptRequest, List<ScriptDto>>
{
    private readonly AssessmentService _assessmentService;
    private readonly IAppLoggerService<SearchScriptEndpoint> _logger;

    public SearchScriptEndpoint(
        AssessmentService assessmentService,
        IAppLoggerService<SearchScriptEndpoint> logger
    )
    {
        _assessmentService = assessmentService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("/scripts/search");
        AllowAnonymous();
        Description(d =>
            d.Produces<List<ScriptDto>>(200, "application/json").Produces(400)
        );
        Tags("Scripts");
        Summary(s =>
        {
            s.Summary = "Search scripts";
            s.Description = "Search scripts by various fields";
            s.ExampleRequest = new SearchScriptRequest
            {
                SearchTerm = "example",
            };
        });
    }

    public override async Task HandleAsync(
         SearchScriptRequest req,
        CancellationToken ct
    )
    {
        var result = await _assessmentService.SearchScriptsAsync(
            req.SearchTerm
        );
        List<ValidationFailure> validationFailures =
            new AutoConstructedList<ValidationFailure>();
        await result.Match(
            Right: async scripts => await SendOkAsync(scripts, ct),
            Left: async error => await SendErrorsAsync(400, ct)
        );
    }
}
