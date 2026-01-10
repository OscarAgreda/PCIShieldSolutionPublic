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
using BlazorMauiShared.Models.PaymentChannel;
using PCIShield.BlazorMauiShared.Models.PaymentChannel;
using BlazorMauiShared.Models.ROCPackage;
using PCIShield.BlazorMauiShared.Models.ROCPackage;
using BlazorMauiShared.Models.ScanSchedule;
using PCIShield.BlazorMauiShared.Models.ScanSchedule;
using BlazorMauiShared.Models.Vulnerability;
using PCIShield.BlazorMauiShared.Models.Vulnerability;
using BlazorMauiShared.Models.PaymentPage;
using PCIShield.BlazorMauiShared.Models.PaymentPage;
using BlazorMauiShared.Models.Merchant;
using PCIShield.BlazorMauiShared.Models.Merchant;
using PCIShield.BlazorMauiShared.ModelsDto;

using PCIShieldLib.SharedKernel.Interfaces;
using Microsoft.AspNetCore.Mvc;
using UuidV7Generator = PCIShield.Domain.ModelsDto.UuidV7Generator;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelEntityDto;
namespace PCIShield.Api.Endpoints.PaymentChannel;
public class GetLastCreatedPaymentChannelEndpoint : EndpointWithoutRequest<GetByIdPaymentChannelResponse>
{
    private readonly PaymentChannelService _paymentChannelService;
    private readonly IAppLoggerService<GetLastCreatedPaymentChannelEndpoint> _logger;

    public GetLastCreatedPaymentChannelEndpoint(PaymentChannelService paymentChannelService,
        IAppLoggerService<GetLastCreatedPaymentChannelEndpoint> logger)
    {
        _paymentChannelService = paymentChannelService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("/paymentChannels/last-created");
        AllowAnonymous();
        Description(d => d
            .Produces<GetByIdPaymentChannelResponse>(200, "application/json")
            .Produces(404));
        Tags("PaymentChannels");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await _paymentChannelService.GetLastCreatedPaymentChannelAsync();
        await result.Match(
            Right: async paymentChannel => await SendOkAsync(new GetByIdPaymentChannelResponse { PaymentChannel = paymentChannel }, ct),
            Left: async error => await SendErrorsAsync(404, ct)
        );
    }
}

public class ComparePaymentChannelSpecificationPerformanceEndpoint : Endpoint<GetByIdPaymentChannelRequest, SpecificationPerformanceResponse<PaymentChannelEntityDto>>
{
    private readonly PaymentChannelService _paymentChannelService;
    private readonly IAppLoggerService<ComparePaymentChannelSpecificationPerformanceEndpoint> _logger;

    public ComparePaymentChannelSpecificationPerformanceEndpoint(
        PaymentChannelService paymentChannelService,
        IAppLoggerService<ComparePaymentChannelSpecificationPerformanceEndpoint> logger)
    {
        _paymentChannelService = paymentChannelService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/paymentChannels/perf/{paymentChannelId}");
        AllowAnonymous();
        Description(d => d
            .Produces<SpecificationPerformanceResponse<PaymentChannelEntityDto>>(200, "application/json")
            .Produces(404)
            .Produces(400));
        Tags("PaymentChannels");
        Summary(s =>
        {
            s.Summary = "Compare PaymentChannel Specification Performance";
            s.Description = "Measures and compares the performance of different paymentChannel specification types";
            s.ExampleRequest = new GetByIdPaymentChannelRequest { PaymentChannelId = Guid.NewGuid() };
        });
    }

    public override async Task HandleAsync(GetByIdPaymentChannelRequest req, CancellationToken ct)
    {
        _logger.LogInformation($"Starting performance test for paymentChannel ID: {req.PaymentChannelId}");
        var response = await Observable
            .FromAsync(() => ProcessPaymentChannelSpecificationPerformance(req.PaymentChannelId, ct))
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

    private async Task<SpecificationPerformanceResponse<PaymentChannelEntityDto>> ProcessPaymentChannelSpecificationPerformance(
        Guid paymentChannelId,
        CancellationToken ct)
    {
        _logger.LogInformation($"Running performance tests for paymentChannel: {paymentChannelId}");
        var response = new SpecificationPerformanceResponse<PaymentChannelEntityDto>(Guid.NewGuid());
        var result = await _paymentChannelService.RunPaymentChannelPerformanceTests(paymentChannelId, disableCache: true);

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
public class CreatePaymentPageEndpoint : Endpoint<CreatePaymentPageRequest, CreatePaymentPageResponse>
{
    private readonly PaymentChannelService _paymentChannelService;
    private readonly IAppLoggerService<CreatePaymentPageEndpoint> _logger;

    public CreatePaymentPageEndpoint(PaymentChannelService paymentChannelService, IAppLoggerService<CreatePaymentPageEndpoint> logger)
    {
        _paymentChannelService = paymentChannelService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/paymentPages");
        AllowAnonymous();
        Description(d => d.Produces<CreatePaymentPageResponse>(201, "application/json").Produces(400));
        Tags("PaymentPages");
        Summary(s =>
        {
            s.Summary = "Create a new PaymentPage";
            s.Description = "Creates a new PaymentPage";
            s.ExampleRequest = new CreatePaymentPageRequest { };
        });
    }

    public override async Task HandleAsync(CreatePaymentPageRequest req, CancellationToken ct)
    {
        var response = await Observable.FromAsync(() => ProcessCreatePaymentPageRequest(req, ct)).Take(1).FirstOrDefaultAsync();
        if (response != null)
        {
            await SendCreatedAtAsync<CreatePaymentPageEndpoint>(new { paymentPageId = response.PaymentPage.PaymentPageId }, response, generateAbsoluteUrl: true, cancellation: ct);
        }
        else
        {
            await SendErrorsAsync(400, ct);
        }
    }

    private async Task<CreatePaymentPageResponse> ProcessCreatePaymentPageRequest(CreatePaymentPageRequest req, CancellationToken ct)
    {
        _logger.LogInformation("Creating new paymentPage");
        Either<string, PaymentPageDto> result = await _paymentChannelService.CreatePaymentPageAsync(req.PaymentPage);
        return result.Match(Right: dto => new CreatePaymentPageResponse { PaymentPage = dto, ErrorMessage = null }, Left: err => new CreatePaymentPageResponse { PaymentPage = null, ErrorMessage = err });
    }
}

public class DeletePaymentPageEndpoint : Endpoint<DeletePaymentPageRequest, DeletePaymentPageResponse>
{
    private readonly PaymentChannelService _paymentChannelService;
    private readonly IAppLoggerService<DeletePaymentPageEndpoint> _logger;

    public DeletePaymentPageEndpoint(PaymentChannelService paymentChannelService, IAppLoggerService<DeletePaymentPageEndpoint> logger)
    {
        _paymentChannelService = paymentChannelService;
        _logger = logger;
    }

    public override void Configure()
    {
        Delete("/paymentPages/{paymentPageId}");
        AllowAnonymous();
        Description(d => d.Produces<DeletePaymentPageResponse>(200, "application/json").Produces(404).Produces(400));
        Tags("PaymentPages");
        Summary(s =>
        {
            s.Summary = "Delete an existing PaymentPage";
            s.Description = "Deletes an existing PaymentPage";
            s.ExampleRequest = new DeletePaymentPageRequest { PaymentPageId = Guid.NewGuid() };
        });
    }

    public override async Task HandleAsync(DeletePaymentPageRequest req, CancellationToken ct)
    {
        var response = await Observable.FromAsync(() => ProcessDeletePaymentPageRequest(req.PaymentPageId, ct)).Take(1).FirstOrDefaultAsync();
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

    private async Task<DeletePaymentPageResponse> ProcessDeletePaymentPageRequest(Guid paymentPageId, CancellationToken ct)
    {
        _logger.LogInformation($"Deleting paymentPage with ID: {paymentPageId}");
        Either<string, bool> result = await _paymentChannelService.DeletePaymentPageAsync(paymentPageId);
        return result.Match(
            Right: success => new DeletePaymentPageResponse { IsSuccess = success, ErrorMessage = null },
            Left: err => new DeletePaymentPageResponse { IsSuccess = false, ErrorMessage = err }
        );
    }
}
public class GetPaymentPageByIdWithIncludesEndpoint : Endpoint<GetByIdPaymentPageRequest, GetByIdPaymentPageResponse>
{
    private readonly PaymentChannelService _paymentChannelService;
    private readonly IAppLoggerService<GetPaymentPageByIdWithIncludesEndpoint> _logger;

    public GetPaymentPageByIdWithIncludesEndpoint(PaymentChannelService paymentChannelService, IAppLoggerService<GetPaymentPageByIdWithIncludesEndpoint> logger)
    {
        _paymentChannelService = paymentChannelService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/paymentPages/i/{paymentPageId}");
        AllowAnonymous();
        Description(d => d.Produces<GetByIdPaymentPageResponse>(200, "application/json").Produces(404).Produces(400));
        Tags("PaymentPages");
        Summary(s =>
        {
            s.Summary = "Get an PaymentPage by Id With Includes";
            s.Description = "Gets an PaymentPage by Id With Includes";
            s.ExampleRequest = new GetByIdPaymentPageRequest { PaymentPageId = Guid.NewGuid() };
        });
    }

    public override async Task HandleAsync(GetByIdPaymentPageRequest req, CancellationToken ct)
    {
        var response = await Observable.FromAsync(() => ProcessPaymentPageRequest(req.PaymentPageId, ct)).Take(1).FirstOrDefaultAsync();
        if (response != null)
        {
            await SendOkAsync(response, ct);
        }
        else
        {
            await SendErrorsAsync(400, ct);
        }
    }

    private async Task<GetByIdPaymentPageResponse> ProcessPaymentPageRequest(Guid paymentPageId, CancellationToken ct)
    {
        _logger.LogInformation($"Fetching paymentPage with ID: {paymentPageId}");
        Either<string, PaymentPageDto> result = await _paymentChannelService.GetOneFullPaymentPageByIdAsync(paymentPageId);
        return result.Match(
            Right: dto => new GetByIdPaymentPageResponse { PaymentPage = dto, ErrorMessage = null },
            Left: err => new GetByIdPaymentPageResponse { PaymentPage = null, ErrorMessage = err }
        );
    }
}

public class UpdatePaymentPageEndpoint
    : Endpoint<UpdatePaymentPageRequest, UpdatePaymentPageResponse>
{
    private readonly PaymentChannelService _paymentChannelService;
    private readonly IAppLoggerService<UpdatePaymentPageEndpoint> _logger;

    public UpdatePaymentPageEndpoint(
        PaymentChannelService paymentChannelService,
        IAppLoggerService<UpdatePaymentPageEndpoint> logger
    )
    {
        _paymentChannelService = paymentChannelService;
        _logger = logger;
    }

    public override void Configure()
    {
        Put("/paymentPages");
        AllowAnonymous();
        Description(d =>
            d.Produces<UpdatePaymentPageResponse>(200, "application/json")
                .Produces(404)
                .Produces(400)
        );
        Tags("PaymentPages");
        Summary(s =>
        {
            s.Summary = "Update an existing PaymentPage";
            s.Description = "Updates an existing PaymentPage's details";
            s.ExampleRequest = new UpdatePaymentPageRequest
            {
                PaymentPage = new PaymentPageDto
                {
                    PaymentPageId = Guid.NewGuid(),
                    TenantId = Guid.Empty,
                    PaymentChannelId = Guid.Empty,
                    PageUrl = string.Empty,
                    PageName = string.Empty,
                    IsActive = false,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = Guid.Empty,
                   
                    IsDeleted = false,
                }
            };
        });
    }

    public override async Task HandleAsync(
        UpdatePaymentPageRequest req,
        CancellationToken ct
    )
    {
        var response = await Observable
            .FromAsync(() => ProcessUpdatePaymentPageRequest(req, ct))
            .Take(1)
            .FirstOrDefaultAsync();
        if (response != null)
        {
            if (response.PaymentPage != null)
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

    private async Task<UpdatePaymentPageResponse> ProcessUpdatePaymentPageRequest(UpdatePaymentPageRequest req, CancellationToken ct)
    {
        _logger.LogInformation($"Updating paymentPage with ID: {req.PaymentPage.PaymentPageId}");

        Either<string, PaymentPageDto> result = await _paymentChannelService.UpdatePaymentPageAsync(req.PaymentPage.PaymentPageId, req.PaymentPage);

        return result.Match(
            Right: dto => new UpdatePaymentPageResponse
            {
                PaymentPage = dto,
                ErrorMessage = null,
            },
            Left: err => new UpdatePaymentPageResponse
            {
                PaymentPage = null,
                ErrorMessage = err,
            }
        );
    }
}

