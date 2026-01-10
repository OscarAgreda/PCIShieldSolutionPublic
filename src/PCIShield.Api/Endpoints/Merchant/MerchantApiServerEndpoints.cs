using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

using Amazon.Runtime.Internal;

using BlazorMauiShared.Models.Assessment;
using BlazorMauiShared.Models.Asset;
using BlazorMauiShared.Models.CompensatingControl;
using BlazorMauiShared.Models.ComplianceOfficer;
using BlazorMauiShared.Models.Control;
using BlazorMauiShared.Models.CryptographicInventory;
using BlazorMauiShared.Models.Evidence;
using BlazorMauiShared.Models.Merchant;
using BlazorMauiShared.Models.NetworkSegmentation;
using BlazorMauiShared.Models.PaymentChannel;
using BlazorMauiShared.Models.ServiceProvider;

using FastEndpoints;

using FluentValidation.Results;

using LanguageExt;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using PCIShield.BlazorMauiShared.Models;
using PCIShield.BlazorMauiShared.Models.Assessment;
using PCIShield.BlazorMauiShared.Models.Asset;
using PCIShield.BlazorMauiShared.Models.CompensatingControl;
using PCIShield.BlazorMauiShared.Models.ComplianceOfficer;
using PCIShield.BlazorMauiShared.Models.Control;
using PCIShield.BlazorMauiShared.Models.CryptographicInventory;
using PCIShield.BlazorMauiShared.Models.Evidence;
using PCIShield.BlazorMauiShared.Models.Merchant;
using PCIShield.BlazorMauiShared.Models.NetworkSegmentation;
using PCIShield.BlazorMauiShared.Models.PaymentChannel;
using PCIShield.BlazorMauiShared.Models.ServiceProvider;
using PCIShield.BlazorMauiShared.ModelsDto;
using PCIShield.Domain.ModelEntityDto;
using PCIShield.Domain.ModelsDto;
using PCIShield.Infrastructure.Services;

using PCIShieldLib.SharedKernel.Interfaces;

using Sort = PCIShieldLib.SharedKernel.Interfaces.Sort;
using UuidV7Generator = PCIShield.Domain.ModelsDto.UuidV7Generator;
namespace PCIShield.Api.Endpoints.Merchant;
public class GetLastCreatedMerchantEndpoint : EndpointWithoutRequest<GetByIdMerchantResponse>
{
    private readonly MerchantService _merchantService;
    private readonly IAppLoggerService<GetLastCreatedMerchantEndpoint> _logger;

    public GetLastCreatedMerchantEndpoint(MerchantService merchantService,
        IAppLoggerService<GetLastCreatedMerchantEndpoint> logger)
    {
        _merchantService = merchantService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("/merchants/last-created");
        AllowAnonymous();
        Description(d => d
            .Produces<GetByIdMerchantResponse>(200, "application/json")
            .Produces(404));
        Tags("Merchants");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await _merchantService.GetLastCreatedMerchantAsync();
        await result.Match(
            Right: async merchant => await SendOkAsync(new GetByIdMerchantResponse { Merchant = merchant }, ct),
            Left: async error => await SendErrorsAsync(404, ct)
        );
    }
}

public class CreateMerchantEndpoint : Endpoint<CreateMerchantRequest, CreateMerchantResponse>
{
    private readonly MerchantService _merchantService;
    private readonly IAppLoggerService<CreateMerchantEndpoint> _logger;

    public CreateMerchantEndpoint(MerchantService merchantService, IAppLoggerService<CreateMerchantEndpoint> logger)
    {
        _merchantService = merchantService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/merchants");
        AllowAnonymous();
        Description(d => d.Produces<CreateMerchantResponse>(201, "application/json").Produces(400));
        Tags("Merchants");
        Summary(s =>
        {
            s.Summary = "Create a new Merchant";
            s.Description = "Creates a new Merchant";
            s.ExampleRequest = new CreateMerchantRequest { };
        });
    }

    public override async Task HandleAsync(CreateMerchantRequest req, CancellationToken ct)
    {
        var response = await Observable.FromAsync(() => ProcessCreateMerchantRequest(req, ct)).Take(1).FirstOrDefaultAsync();
        if (response != null)
        {
            await SendCreatedAtAsync<CreateMerchantEndpoint>(new { merchantId = response.Merchant.MerchantId }, response, generateAbsoluteUrl: true, cancellation: ct);
        }
        else
        {
            await SendErrorsAsync(400, ct);
        }
    }

    private async Task<CreateMerchantResponse> ProcessCreateMerchantRequest(CreateMerchantRequest req, CancellationToken ct)
    {
        _logger.LogInformation("Creating new merchant");
        Either<string, MerchantDto> result = await _merchantService.CreateMerchantAsync(req.Merchant);
        return result.Match(Right: dto => new CreateMerchantResponse { Merchant = dto, ErrorMessage = null }, Left: err => new CreateMerchantResponse { Merchant = null, ErrorMessage = err });
    }
}

public class DeleteMerchantEndpoint : Endpoint<DeleteMerchantRequest, DeleteMerchantResponse>
{
    private readonly MerchantService _merchantService;
    private readonly IAppLoggerService<DeleteMerchantEndpoint> _logger;

    public DeleteMerchantEndpoint(MerchantService merchantService, IAppLoggerService<DeleteMerchantEndpoint> logger)
    {
        _merchantService = merchantService;
        _logger = logger;
    }

    public override void Configure()
    {
        Delete("/merchants/{merchantId}");
        AllowAnonymous();
        Description(d => d.Produces<DeleteMerchantResponse>(200, "application/json").Produces(404).Produces(400));
        Tags("Merchants");
        Summary(s =>
        {
            s.Summary = "Delete an existing Merchant";
            s.Description = "Deletes an existing Merchant";
            s.ExampleRequest = new DeleteMerchantRequest { MerchantId = Guid.NewGuid() };
        });
    }

    public override async Task HandleAsync(DeleteMerchantRequest req, CancellationToken ct)
    {
        var response = await Observable.FromAsync(() => ProcessDeleteMerchantRequest(req.MerchantId, ct)).Take(1).FirstOrDefaultAsync();
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

    private async Task<DeleteMerchantResponse> ProcessDeleteMerchantRequest(Guid merchantId, CancellationToken ct)
    {
        _logger.LogInformation($"Deleting merchant with ID: {merchantId}");
        Either<string, bool> result = await _merchantService.DeleteMerchantAsync(merchantId);
        return result.Match(
            Right: success => new DeleteMerchantResponse { IsSuccess = success, ErrorMessage = null },
            Left: err => new DeleteMerchantResponse { IsSuccess = false, ErrorMessage = err }
        );
    }
}

public class GetMerchantByIdWithIncludesEndpoint : Endpoint<GetByIdMerchantRequest, GetByIdMerchantResponse>
{
    private readonly MerchantService _merchantService;
    private readonly IAppLoggerService<GetMerchantByIdWithIncludesEndpoint> _logger;

    public GetMerchantByIdWithIncludesEndpoint(MerchantService merchantService, IAppLoggerService<GetMerchantByIdWithIncludesEndpoint> logger)
    {
        _merchantService = merchantService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/merchants/i/{merchantId}");
        AllowAnonymous();
        Description(d => d.Produces<GetByIdMerchantResponse>(200, "application/json").Produces(404).Produces(400));
        Tags("Merchants");
        Summary(s =>
        {
            s.Summary = "Get an Merchant by Id With Includes";
            s.Description = "Gets an Merchant by Id With Includes";
            s.ExampleRequest = new GetByIdMerchantRequest { MerchantId = Guid.NewGuid() };
        });
    }

    public override async Task HandleAsync(GetByIdMerchantRequest req, CancellationToken ct)
    {
        var response = await Observable.FromAsync(() => ProcessMerchantRequest(req.MerchantId, req.WithPostGraph, ct)).Take(1).FirstOrDefaultAsync();
        if (response != null)
        {
            await SendOkAsync(response, ct);
        }
        else
        {
            await SendErrorsAsync(400, ct);
        }
    }

    private async Task<GetByIdMerchantResponse> ProcessMerchantRequest(Guid merchantId, bool withPostGraph, CancellationToken ct)
    {
        _logger.LogInformation($"Fetching merchant with ID: {merchantId}");
        Either<string, MerchantDto> result = await _merchantService.GetOneFullMerchantByIdAsync(merchantId, withPostGraph);
        return result.Match(
            Right: dto => new GetByIdMerchantResponse { Merchant = dto, ErrorMessage = null },
            Left: err => new GetByIdMerchantResponse { Merchant = null, ErrorMessage = err }
        );
    }
}
//public class CompareMerchantSpecificationPerformanceEndpoint : Endpoint<GetByIdMerchantRequest, SpecificationPerformanceResponse<MerchantEntityDto>>
//{
//    private readonly MerchantService _merchantService;
//    private readonly IAppLoggerService<CompareMerchantSpecificationPerformanceEndpoint> _logger;

//    public CompareMerchantSpecificationPerformanceEndpoint(
//        MerchantService merchantService,
//        IAppLoggerService<CompareMerchantSpecificationPerformanceEndpoint> logger)
//    {
//        _merchantService = merchantService;
//        _logger = logger;
//    }

//    public override void Configure()
//    {
//        Post("/merchants/perf/{merchantId}");
//        AllowAnonymous();
//        Description(d => d
//            .Produces<SpecificationPerformanceResponse<MerchantEntityDto>>(200, "application/json")
//            .Produces(404)
//            .Produces(400));
//        Tags("Merchants");
//        Summary(s =>
//        {
//            s.Summary = "Compare Merchant Specification Performance";
//            s.Description = "Measures and compares the performance of different merchant specification types";
//            s.ExampleRequest = new GetByIdMerchantRequest { MerchantId = Guid.NewGuid() };
//        });
//    }

//    public override async Task HandleAsync(GetByIdMerchantRequest req, CancellationToken ct)
//    {
//        _logger.LogInformation($"Starting performance test for merchant ID: {req.MerchantId}");
//        var response = await Observable
//            .FromAsync(() => ProcessMerchantSpecificationPerformance(req.MerchantId, ct))
//            .Take(1)
//            .FirstOrDefaultAsync();

//        if (response != null)
//        {
//            await SendOkAsync(response, ct);
//        }
//        else
//        {
//            await SendErrorsAsync(400, ct);
//        }
//    }

//    //private async Task<SpecificationPerformanceResponse<MerchantEntityDto>> ProcessMerchantSpecificationPerformance(
//    //    Guid merchantId,
//    //    CancellationToken ct)
//    //{
//    //    _logger.LogInformation($"Running performance tests for merchant: {merchantId}");
//    //    var response = new SpecificationPerformanceResponse<MerchantEntityDto>(Guid.NewGuid());
//    //    var result = await _merchantService.RunMerchantPerformanceTests(merchantId, disableCache: true);

//    //    return result.Match(
//    //        Right: perfData =>
//    //        {
//    //            response.TimingResults = perfData.TimingResults;
//    //            response.Statistics = perfData.Statistics;
//    //            response.ComplexityAnalysis = perfData.ComplexityAnalysis;
//    //            response.ErrorMessage = null;
//    //            response.FastestSpecification = perfData.FastestSpecification;
//    //            response.MostConsistentSpecification = perfData.MostConsistentSpecification;
//    //            response.RecommendedSpecification = perfData.RecommendedSpecification;
//    //            response.MaximumSpeedupFactor = perfData.MaximumSpeedupFactor;
//    //            return response;
//    //        },
//    //        Left: errorMsg =>
//    //        {
//    //            response.ErrorMessage = errorMsg;
//    //            return response;
//    //        }
//    //    );
//    //}
//}
public class SearchMerchantEndpoint
    : Endpoint<SearchMerchantRequest, List<MerchantDto>>
{
    private readonly MerchantService _merchantService;
    private readonly IAppLoggerService<SearchMerchantEndpoint> _logger;

    public SearchMerchantEndpoint(
        MerchantService merchantService,
        IAppLoggerService<SearchMerchantEndpoint> logger
    )
    {
        _merchantService = merchantService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("/merchants/search");
        AllowAnonymous();
        Description(d =>
            d.Produces<List<MerchantDto>>(200, "application/json").Produces(400)
        );
        Tags("Merchants");
        Summary(s =>
        {
            s.Summary = "Search merchants";
            s.Description = "Search merchants by various fields";
            s.ExampleRequest = new SearchMerchantRequest
            {
                SearchTerm = "example",
            };
        });
    }

    public override async Task HandleAsync(
         SearchMerchantRequest req,
        CancellationToken ct
    )
    {
        var result = await _merchantService.SearchMerchantsAsync(
            req.SearchTerm
        );
        List<ValidationFailure> validationFailures =
            new AutoConstructedList<ValidationFailure>();
        await result.Match(
            Right: async merchants => await SendOkAsync(merchants, ct),
            Left: async error => await SendErrorsAsync(400, ct)
        );
    }
}

public class FilterMerchantEndpoint
    : Endpoint<FilteredMerchantRequest, ListMerchantResponse>
{
    private readonly MerchantService _merchantService;
    private readonly IAppLoggerService<FilterMerchantEndpoint> _logger;

    public FilterMerchantEndpoint(
        MerchantService merchantService,
        IAppLoggerService<FilterMerchantEndpoint> logger
    )
    {
        _merchantService = merchantService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/merchants/filtered_list");
        AllowAnonymous();
        Description(d =>
            d.Produces<ListMerchantResponse>(200, "application/json")
                .Produces(400)
        );
        Tags("Merchants");
        Summary(s =>
        {
            s.Summary = "Filter merchants";
            s.Description = "Get a filtered and sorted list of merchants";
            s.ExampleRequest = new FilteredMerchantRequest
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
        FilteredMerchantRequest req,
        CancellationToken ct
    )
    {
        var result = await _merchantService.GetFilteredMerchantsAsync(req);
        await result.Match(
            Right: async response => await SendOkAsync(response, ct),
            Left: async error => await SendErrorsAsync(400, ct)
        );
    }
}
public class GetMerchantListEndpoint : Endpoint<GetMerchantListRequest, ListMerchantResponse>
{
    private readonly MerchantService _merchantService;
    private readonly IAppLoggerService<GetMerchantListEndpoint> _logger;

    public GetMerchantListEndpoint(
        MerchantService merchantService,
        IAppLoggerService<GetMerchantListEndpoint> logger)
    {
        _merchantService = merchantService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/merchants/paged_list");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        Description(d =>
            d.Produces<ListMerchantResponse>(200, "application/json")
                .Produces(400)
        );
        Tags("Merchants");
        Summary(s =>
        {
            s.Summary = "Get a paged list of Merchants";
            s.Description = "Gets a paginated list of Merchants with total count and pagination metadata";
            s.ExampleRequest = new GetMerchantListRequest
            {
                PageNumber = 1,
                PageSize = 10,
            };
        });
    }

    public override async Task HandleAsync(
        GetMerchantListRequest req,
        CancellationToken ct)
    {
        _logger.LogInformation($"Fetching merchant list. Page: {req.PageNumber}, Size: {req.PageSize}");
        var result = await _merchantService.GetMerchantPagedList(req.PageNumber, req.PageSize);
        await result.Match(
            async response =>
            {

                await SendOkAsync(response, ct);
            },
            async error =>
            {

                var errorResponse = new ListMerchantResponse
                {
                    ErrorMessage = error,
                    Merchants = null,
                    PageNumber = req.PageNumber,
                    PageSize = req.PageSize
                };

                await SendAsync(errorResponse, 400, ct);
            }
        );
    }
}

public class UpdateMerchantEndpoint
    : Endpoint<UpdateMerchantRequest, UpdateMerchantResponse>
{
    private readonly MerchantService _merchantService;
    private readonly IAppLoggerService<UpdateMerchantEndpoint> _logger;

    public UpdateMerchantEndpoint(
        MerchantService merchantService,
        IAppLoggerService<UpdateMerchantEndpoint> logger
    )
    {
        _merchantService = merchantService;
        _logger = logger;
    }

    public override void Configure()
    {
        Put("/merchants");
        AllowAnonymous();
        Description(d =>
            d.Produces<UpdateMerchantResponse>(200, "application/json")
                .Produces(404)
                .Produces(400)
        );
        Tags("Merchants");
        Summary(s =>
        {
            s.Summary = "Update an existing Merchant";
            s.Description = "Updates an existing Merchant's details";
            s.ExampleRequest = new UpdateMerchantRequest
            {
                Merchant = new MerchantDto
                {
                    MerchantId = Guid.NewGuid(),
                    TenantId = Guid.Empty,
                    MerchantCode = string.Empty,
                    MerchantName = string.Empty,
                    MerchantLevel = 0,
                    AcquirerName = string.Empty,
                    ProcessorMID = string.Empty,
                    AnnualCardVolume = 0m,
                    NextAssessmentDue = DateTime.UtcNow,
                    ComplianceRank = 0,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = Guid.Empty,
                 
                    IsDeleted = false,
                }
            };
        });
    }

    public override async Task HandleAsync(
        UpdateMerchantRequest req,
        CancellationToken ct
    )
    {
        var response = await Observable
            .FromAsync(() => ProcessUpdateMerchantRequest(req, ct))
            .Take(1)
            .FirstOrDefaultAsync();
        if (response != null)
        {
            if (response.Merchant != null)
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

    private async Task<UpdateMerchantResponse> ProcessUpdateMerchantRequest(UpdateMerchantRequest req, CancellationToken ct)
    {
        _logger.LogInformation($"Updating merchant with ID: {req.Merchant.MerchantId}");

        Either<string, MerchantDto> result = await _merchantService.UpdateMerchantAsync(req.Merchant.MerchantId, req.Merchant);

        return result.Match(
            Right: dto => new UpdateMerchantResponse
            {
                Merchant = dto,
                ErrorMessage = null,
            },
            Left: err => new UpdateMerchantResponse
            {
                Merchant = null,
                ErrorMessage = err,
            }
        );
    }
}
public class CreateAssetEndpoint : Endpoint<CreateAssetRequest, CreateAssetResponse>
{
    private readonly MerchantService _merchantService;
    private readonly IAppLoggerService<CreateAssetEndpoint> _logger;

    public CreateAssetEndpoint(MerchantService merchantService, IAppLoggerService<CreateAssetEndpoint> logger)
    {
        _merchantService = merchantService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/assets");
        AllowAnonymous();
        Description(d => d.Produces<CreateAssetResponse>(201, "application/json").Produces(400));
        Tags("Assets");
        Summary(s =>
        {
            s.Summary = "Create a new Asset";
            s.Description = "Creates a new Asset";
            s.ExampleRequest = new CreateAssetRequest { };
        });
    }

    public override async Task HandleAsync(CreateAssetRequest req, CancellationToken ct)
    {
        var response = await Observable.FromAsync(() => ProcessCreateAssetRequest(req, ct)).Take(1).FirstOrDefaultAsync();
        if (response != null)
        {
            await SendCreatedAtAsync<CreateAssetEndpoint>(new { assetId = response.Asset.AssetId }, response, generateAbsoluteUrl: true, cancellation: ct);
        }
        else
        {
            await SendErrorsAsync(400, ct);
        }
    }

    private async Task<CreateAssetResponse> ProcessCreateAssetRequest(CreateAssetRequest req, CancellationToken ct)
    {
        _logger.LogInformation("Creating new asset");
        Either<string, AssetDto> result = await _merchantService.CreateAssetAsync(req.Asset);
        return result.Match(Right: dto => new CreateAssetResponse { Asset = dto, ErrorMessage = null }, Left: err => new CreateAssetResponse { Asset = null, ErrorMessage = err });
    }
}

public class DeleteAssetEndpoint : Endpoint<DeleteAssetRequest, DeleteAssetResponse>
{
    private readonly MerchantService _merchantService;
    private readonly IAppLoggerService<DeleteAssetEndpoint> _logger;

    public DeleteAssetEndpoint(MerchantService merchantService, IAppLoggerService<DeleteAssetEndpoint> logger)
    {
        _merchantService = merchantService;
        _logger = logger;
    }

    public override void Configure()
    {
        Delete("/assets/{assetId}");
        AllowAnonymous();
        Description(d => d.Produces<DeleteAssetResponse>(200, "application/json").Produces(404).Produces(400));
        Tags("Assets");
        Summary(s =>
        {
            s.Summary = "Delete an existing Asset";
            s.Description = "Deletes an existing Asset";
            s.ExampleRequest = new DeleteAssetRequest { AssetId = Guid.NewGuid() };
        });
    }

    public override async Task HandleAsync(DeleteAssetRequest req, CancellationToken ct)
    {
        var response = await Observable.FromAsync(() => ProcessDeleteAssetRequest(req.AssetId, ct)).Take(1).FirstOrDefaultAsync();
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

    private async Task<DeleteAssetResponse> ProcessDeleteAssetRequest(Guid assetId, CancellationToken ct)
    {
        _logger.LogInformation($"Deleting asset with ID: {assetId}");
        Either<string, bool> result = await _merchantService.DeleteAssetAsync(assetId);
        return result.Match(
            Right: success => new DeleteAssetResponse { IsSuccess = success, ErrorMessage = null },
            Left: err => new DeleteAssetResponse { IsSuccess = false, ErrorMessage = err }
        );
    }
}
public class GetAssetByIdWithIncludesEndpoint : Endpoint<GetByIdAssetRequest, GetByIdAssetResponse>
{
    private readonly MerchantService _merchantService;
    private readonly IAppLoggerService<GetAssetByIdWithIncludesEndpoint> _logger;

    public GetAssetByIdWithIncludesEndpoint(MerchantService merchantService, IAppLoggerService<GetAssetByIdWithIncludesEndpoint> logger)
    {
        _merchantService = merchantService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/assets/i/{assetId}");
        AllowAnonymous();
        Description(d => d.Produces<GetByIdAssetResponse>(200, "application/json").Produces(404).Produces(400));
        Tags("Assets");
        Summary(s =>
        {
            s.Summary = "Get an Asset by Id With Includes";
            s.Description = "Gets an Asset by Id With Includes";
            s.ExampleRequest = new GetByIdAssetRequest { AssetId = Guid.NewGuid() };
        });
    }

    public override async Task HandleAsync(GetByIdAssetRequest req, CancellationToken ct)
    {
        var response = await Observable.FromAsync(() => ProcessAssetRequest(req.AssetId, ct)).Take(1).FirstOrDefaultAsync();
        if (response != null)
        {
            await SendOkAsync(response, ct);
        }
        else
        {
            await SendErrorsAsync(400, ct);
        }
    }

    private async Task<GetByIdAssetResponse> ProcessAssetRequest(Guid assetId, CancellationToken ct)
    {
        _logger.LogInformation($"Fetching asset with ID: {assetId}");
        Either<string, AssetDto> result = await _merchantService.GetOneFullAssetByIdAsync(assetId);
        return result.Match(
            Right: dto => new GetByIdAssetResponse { Asset = dto, ErrorMessage = null },
            Left: err => new GetByIdAssetResponse { Asset = null, ErrorMessage = err }
        );
    }
}

public class SearchAssetEndpoint
    : Endpoint<SearchAssetRequest, List<AssetDto>>
{
    private readonly MerchantService _merchantService;
    private readonly IAppLoggerService<SearchAssetEndpoint> _logger;

    public SearchAssetEndpoint(
        MerchantService merchantService,
        IAppLoggerService<SearchAssetEndpoint> logger
    )
    {
        _merchantService = merchantService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("/assets/search");
        AllowAnonymous();
        Description(d =>
            d.Produces<List<AssetDto>>(200, "application/json").Produces(400)
        );
        Tags("Assets");
        Summary(s =>
        {
            s.Summary = "Search assets";
            s.Description = "Search assets by various fields";
            s.ExampleRequest = new SearchAssetRequest
            {
                SearchTerm = "example",
            };
        });
    }

    public override async Task HandleAsync(
         SearchAssetRequest req,
        CancellationToken ct
    )
    {
        var result = await _merchantService.SearchAssetsAsync(
            req.SearchTerm
        );
        List<ValidationFailure> validationFailures =
            new AutoConstructedList<ValidationFailure>();
        await result.Match(
            Right: async assets => await SendOkAsync(assets, ct),
            Left: async error => await SendErrorsAsync(400, ct)
        );
    }
}

public class FilterAssetEndpoint
    : Endpoint<FilteredAssetRequest, ListAssetResponse>
{
    private readonly MerchantService _merchantService;
    private readonly IAppLoggerService<FilterAssetEndpoint> _logger;

    public FilterAssetEndpoint(
        MerchantService merchantService,
        IAppLoggerService<FilterAssetEndpoint> logger
    )
    {
        _merchantService = merchantService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/assets/filtered_list");
        AllowAnonymous();
        Description(d =>
            d.Produces<ListAssetResponse>(200, "application/json")
                .Produces(400)
        );
        Tags("Assets");
        Summary(s =>
        {
            s.Summary = "Filter assets";
            s.Description = "Get a filtered and sorted list of assets";
            s.ExampleRequest = new FilteredAssetRequest
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
        FilteredAssetRequest req,
        CancellationToken ct
    )
    {
        var result = await _merchantService.GetFilteredAssetsAsync(req);
        await result.Match(
            Right: async response => await SendOkAsync(response, ct),
            Left: async error => await SendErrorsAsync(400, ct)
        );
    }
}

public class GetAssetListEndpoint
    : Endpoint<GetAssetListRequest, ListAssetResponse>
{
    private readonly MerchantService _merchantService;
    private readonly IAppLoggerService<GetAssetListEndpoint> _logger;

    public GetAssetListEndpoint(
        MerchantService merchantService,
        IAppLoggerService<GetAssetListEndpoint> logger
    )
    {
        _merchantService = merchantService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/assets/paged_list");
        AllowAnonymous();
        Description(d =>
            d.Produces<ListAssetResponse>(200, "application/json")
                .Produces(400)
        );
        Tags("Assets");
        Summary(s =>
        {
            s.Summary = "Get a paged list of Assets";
            s.Description = "Gets a paginated list of Assets";
            s.ExampleRequest = new GetAssetListRequest
            {
                PageNumber = 1,
                PageSize = 10,
            };
        });
    }

    public override async Task HandleAsync(
        GetAssetListRequest req,
        CancellationToken ct
    )
    {
        ListAssetResponse? response = await Observable
            .FromAsync(
                () =>
                    ProcessAssetListRequest(req.PageNumber, req.PageSize, ct)
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

    private async Task<ListAssetResponse> ProcessAssetListRequest(
        int pageNumber,
        int pageSize,
        CancellationToken ct
    )
    {
        _logger.LogInformation(
            $"Fetching asset list. Page: {pageNumber}, Size: {pageSize}"
        );
        Either<string, List<AssetDto>> result =
            await _merchantService.GetAssetPagedList(pageNumber, pageSize);
        return result.Match(
            assets => new ListAssetResponse
            {
                Assets = assets,
                ErrorMessage = null,
            },
            err => new ListAssetResponse
            {
                Assets = null,
                ErrorMessage = err,
            }
        );
    }
}
public class UpdateAssetEndpoint
    : Endpoint<UpdateAssetRequest, UpdateAssetResponse>
{
    private readonly MerchantService _merchantService;
    private readonly IAppLoggerService<UpdateAssetEndpoint> _logger;

    public UpdateAssetEndpoint(
        MerchantService merchantService,
        IAppLoggerService<UpdateAssetEndpoint> logger
    )
    {
        _merchantService = merchantService;
        _logger = logger;
    }

    public override void Configure()
    {
        Put("/assets");
        AllowAnonymous();
        Description(d =>
            d.Produces<UpdateAssetResponse>(200, "application/json")
                .Produces(404)
                .Produces(400)
        );
        Tags("Assets");
        Summary(s =>
        {
            s.Summary = "Update an existing Asset";
            s.Description = "Updates an existing Asset's details";
            s.ExampleRequest = new UpdateAssetRequest
            {
                Asset = new AssetDto
                {
                    AssetId = Guid.NewGuid(),
                    TenantId = Guid.Empty,
                    MerchantId = Guid.Empty,
                    AssetCode = string.Empty,
                    AssetName = string.Empty,
                    AssetType = 0,
                    IsInCDE = false,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = Guid.Empty,
                    IsDeleted = false,
                }
            };
        });
    }

    public override async Task HandleAsync(
        UpdateAssetRequest req,
        CancellationToken ct
    )
    {
        var response = await Observable
            .FromAsync(() => ProcessUpdateAssetRequest(req, ct))
            .Take(1)
            .FirstOrDefaultAsync();
        if (response != null)
        {
            if (response.Asset != null)
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

    private async Task<UpdateAssetResponse> ProcessUpdateAssetRequest(UpdateAssetRequest req, CancellationToken ct)
    {
        _logger.LogInformation($"Updating asset with ID: {req.Asset.AssetId}");

        Either<string, AssetDto> result = await _merchantService.UpdateAssetAsync(req.Asset.AssetId, req.Asset);

        return result.Match(
            Right: dto => new UpdateAssetResponse
            {
                Asset = dto,
                ErrorMessage = null,
            },
            Left: err => new UpdateAssetResponse
            {
                Asset = null,
                ErrorMessage = err,
            }
        );
    }
}

public class CreateCompensatingControlEndpoint : Endpoint<CreateCompensatingControlRequest, CreateCompensatingControlResponse>
{
    private readonly MerchantService _merchantService;
    private readonly IAppLoggerService<CreateCompensatingControlEndpoint> _logger;

    public CreateCompensatingControlEndpoint(MerchantService merchantService, IAppLoggerService<CreateCompensatingControlEndpoint> logger)
    {
        _merchantService = merchantService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/compensatingControls");
        AllowAnonymous();
        Description(d => d.Produces<CreateCompensatingControlResponse>(201, "application/json").Produces(400));
        Tags("CompensatingControls");
        Summary(s =>
        {
            s.Summary = "Create a new CompensatingControl";
            s.Description = "Creates a new CompensatingControl";
            s.ExampleRequest = new CreateCompensatingControlRequest { };
        });
    }

    public override async Task HandleAsync(CreateCompensatingControlRequest req, CancellationToken ct)
    {
        var response = await Observable.FromAsync(() => ProcessCreateCompensatingControlRequest(req, ct)).Take(1).FirstOrDefaultAsync();
        if (response != null)
        {
            await SendCreatedAtAsync<CreateCompensatingControlEndpoint>(new { compensatingControlId = response.CompensatingControl.CompensatingControlId }, response, generateAbsoluteUrl: true, cancellation: ct);
        }
        else
        {
            await SendErrorsAsync(400, ct);
        }
    }

    private async Task<CreateCompensatingControlResponse> ProcessCreateCompensatingControlRequest(CreateCompensatingControlRequest req, CancellationToken ct)
    {
        _logger.LogInformation("Creating new compensatingControl");
        Either<string, CompensatingControlDto> result = await _merchantService.CreateCompensatingControlAsync(req.CompensatingControl);
        return result.Match(Right: dto => new CreateCompensatingControlResponse { CompensatingControl = dto, ErrorMessage = null }, Left: err => new CreateCompensatingControlResponse { CompensatingControl = null, ErrorMessage = err });
    }
}

public class DeleteCompensatingControlEndpoint : Endpoint<DeleteCompensatingControlRequest, DeleteCompensatingControlResponse>
{
    private readonly MerchantService _merchantService;
    private readonly IAppLoggerService<DeleteCompensatingControlEndpoint> _logger;

    public DeleteCompensatingControlEndpoint(MerchantService merchantService, IAppLoggerService<DeleteCompensatingControlEndpoint> logger)
    {
        _merchantService = merchantService;
        _logger = logger;
    }

    public override void Configure()
    {
        Delete("/compensatingControls/{compensatingControlId}");
        AllowAnonymous();
        Description(d => d.Produces<DeleteCompensatingControlResponse>(200, "application/json").Produces(404).Produces(400));
        Tags("CompensatingControls");
        Summary(s =>
        {
            s.Summary = "Delete an existing CompensatingControl";
            s.Description = "Deletes an existing CompensatingControl";
            s.ExampleRequest = new DeleteCompensatingControlRequest { CompensatingControlId = Guid.NewGuid() };
        });
    }

    public override async Task HandleAsync(DeleteCompensatingControlRequest req, CancellationToken ct)
    {
        var response = await Observable.FromAsync(() => ProcessDeleteCompensatingControlRequest(req.CompensatingControlId, ct)).Take(1).FirstOrDefaultAsync();
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

    private async Task<DeleteCompensatingControlResponse> ProcessDeleteCompensatingControlRequest(Guid compensatingControlId, CancellationToken ct)
    {
        _logger.LogInformation($"Deleting compensatingControl with ID: {compensatingControlId}");
        Either<string, bool> result = await _merchantService.DeleteCompensatingControlAsync(compensatingControlId);
        return result.Match(
            Right: success => new DeleteCompensatingControlResponse { IsSuccess = success, ErrorMessage = null },
            Left: err => new DeleteCompensatingControlResponse { IsSuccess = false, ErrorMessage = err }
        );
    }
}
public class GetCompensatingControlByIdWithIncludesEndpoint : Endpoint<GetByIdCompensatingControlRequest, GetByIdCompensatingControlResponse>
{
    private readonly MerchantService _merchantService;
    private readonly IAppLoggerService<GetCompensatingControlByIdWithIncludesEndpoint> _logger;

    public GetCompensatingControlByIdWithIncludesEndpoint(MerchantService merchantService, IAppLoggerService<GetCompensatingControlByIdWithIncludesEndpoint> logger)
    {
        _merchantService = merchantService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/compensatingControls/i/{compensatingControlId}");
        AllowAnonymous();
        Description(d => d.Produces<GetByIdCompensatingControlResponse>(200, "application/json").Produces(404).Produces(400));
        Tags("CompensatingControls");
        Summary(s =>
        {
            s.Summary = "Get an CompensatingControl by Id With Includes";
            s.Description = "Gets an CompensatingControl by Id With Includes";
            s.ExampleRequest = new GetByIdCompensatingControlRequest { CompensatingControlId = Guid.NewGuid() };
        });
    }

    public override async Task HandleAsync(GetByIdCompensatingControlRequest req, CancellationToken ct)
    {
        var response = await Observable.FromAsync(() => ProcessCompensatingControlRequest(req.CompensatingControlId, ct)).Take(1).FirstOrDefaultAsync();
        if (response != null)
        {
            await SendOkAsync(response, ct);
        }
        else
        {
            await SendErrorsAsync(400, ct);
        }
    }

    private async Task<GetByIdCompensatingControlResponse> ProcessCompensatingControlRequest(Guid compensatingControlId, CancellationToken ct)
    {
        _logger.LogInformation($"Fetching compensatingControl with ID: {compensatingControlId}");
        Either<string, CompensatingControlDto> result = await _merchantService.GetOneFullCompensatingControlByIdAsync(compensatingControlId);
        return result.Match(
            Right: dto => new GetByIdCompensatingControlResponse { CompensatingControl = dto, ErrorMessage = null },
            Left: err => new GetByIdCompensatingControlResponse { CompensatingControl = null, ErrorMessage = err }
        );
    }
}

public class SearchCompensatingControlEndpoint
    : Endpoint<SearchCompensatingControlRequest, List<CompensatingControlDto>>
{
    private readonly MerchantService _merchantService;
    private readonly IAppLoggerService<SearchCompensatingControlEndpoint> _logger;

    public SearchCompensatingControlEndpoint(
        MerchantService merchantService,
        IAppLoggerService<SearchCompensatingControlEndpoint> logger
    )
    {
        _merchantService = merchantService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("/compensatingControls/search");
        AllowAnonymous();
        Description(d =>
            d.Produces<List<CompensatingControlDto>>(200, "application/json").Produces(400)
        );
        Tags("CompensatingControls");
        Summary(s =>
        {
            s.Summary = "Search compensatingControls";
            s.Description = "Search compensatingControls by various fields";
            s.ExampleRequest = new SearchCompensatingControlRequest
            {
                SearchTerm = "example",
            };
        });
    }

    public override async Task HandleAsync(
         SearchCompensatingControlRequest req,
        CancellationToken ct
    )
    {
        var result = await _merchantService.SearchCompensatingControlsAsync(
            req.SearchTerm
        );
        List<ValidationFailure> validationFailures =
            new AutoConstructedList<ValidationFailure>();
        await result.Match(
            Right: async compensatingControls => await SendOkAsync(compensatingControls, ct),
            Left: async error => await SendErrorsAsync(400, ct)
        );
    }
}

public class FilterCompensatingControlEndpoint
    : Endpoint<FilteredCompensatingControlRequest, ListCompensatingControlResponse>
{
    private readonly MerchantService _merchantService;
    private readonly IAppLoggerService<FilterCompensatingControlEndpoint> _logger;

    public FilterCompensatingControlEndpoint(
        MerchantService merchantService,
        IAppLoggerService<FilterCompensatingControlEndpoint> logger
    )
    {
        _merchantService = merchantService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/compensatingControls/filtered_list");
        AllowAnonymous();
        Description(d =>
            d.Produces<ListCompensatingControlResponse>(200, "application/json")
                .Produces(400)
        );
        Tags("CompensatingControls");
        Summary(s =>
        {
            s.Summary = "Filter compensatingControls";
            s.Description = "Get a filtered and sorted list of compensatingControls";
            s.ExampleRequest = new FilteredCompensatingControlRequest
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
        FilteredCompensatingControlRequest req,
        CancellationToken ct
    )
    {
        var result = await _merchantService.GetFilteredCompensatingControlsAsync(req);
        await result.Match(
            Right: async response => await SendOkAsync(response, ct),
            Left: async error => await SendErrorsAsync(400, ct)
        );
    }
}

public class GetCompensatingControlListEndpoint
    : Endpoint<GetCompensatingControlListRequest, ListCompensatingControlResponse>
{
    private readonly MerchantService _merchantService;
    private readonly IAppLoggerService<GetCompensatingControlListEndpoint> _logger;

    public GetCompensatingControlListEndpoint(
        MerchantService merchantService,
        IAppLoggerService<GetCompensatingControlListEndpoint> logger
    )
    {
        _merchantService = merchantService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/compensatingControls/paged_list");
        AllowAnonymous();
        Description(d =>
            d.Produces<ListCompensatingControlResponse>(200, "application/json")
                .Produces(400)
        );
        Tags("CompensatingControls");
        Summary(s =>
        {
            s.Summary = "Get a paged list of CompensatingControls";
            s.Description = "Gets a paginated list of CompensatingControls";
            s.ExampleRequest = new GetCompensatingControlListRequest
            {
                PageNumber = 1,
                PageSize = 10,
            };
        });
    }

    public override async Task HandleAsync(
        GetCompensatingControlListRequest req,
        CancellationToken ct
    )
    {
        ListCompensatingControlResponse? response = await Observable
            .FromAsync(
                () =>
                    ProcessCompensatingControlListRequest(req.PageNumber, req.PageSize, ct)
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

    private async Task<ListCompensatingControlResponse> ProcessCompensatingControlListRequest(
        int pageNumber,
        int pageSize,
        CancellationToken ct
    )
    {
        _logger.LogInformation(
            $"Fetching compensatingControl list. Page: {pageNumber}, Size: {pageSize}"
        );
        Either<string, List<CompensatingControlDto>> result =
            await _merchantService.GetCompensatingControlPagedList(pageNumber, pageSize);
        return result.Match(
            compensatingControls => new ListCompensatingControlResponse
            {
                CompensatingControls = compensatingControls,
                ErrorMessage = null,
            },
            err => new ListCompensatingControlResponse
            {
                CompensatingControls = null,
                ErrorMessage = err,
            }
        );
    }
}
public class UpdateCompensatingControlEndpoint
    : Endpoint<UpdateCompensatingControlRequest, UpdateCompensatingControlResponse>
{
    private readonly MerchantService _merchantService;
    private readonly IAppLoggerService<UpdateCompensatingControlEndpoint> _logger;

    public UpdateCompensatingControlEndpoint(
        MerchantService merchantService,
        IAppLoggerService<UpdateCompensatingControlEndpoint> logger
    )
    {
        _merchantService = merchantService;
        _logger = logger;
    }

    public override void Configure()
    {
        Put("/compensatingControls");
        AllowAnonymous();
        Description(d =>
            d.Produces<UpdateCompensatingControlResponse>(200, "application/json")
                .Produces(404)
                .Produces(400)
        );
        Tags("CompensatingControls");
        Summary(s =>
        {
            s.Summary = "Update an existing CompensatingControl";
            s.Description = "Updates an existing CompensatingControl's details";
            s.ExampleRequest = new UpdateCompensatingControlRequest
            {
                CompensatingControl = new CompensatingControlDto
                {
                    CompensatingControlId = Guid.NewGuid(),
                    TenantId = Guid.Empty,
                    ControlId = Guid.Empty,
                    MerchantId = Guid.Empty,
                    Justification = string.Empty,
                    ImplementationDetails = string.Empty,
                    ExpiryDate = DateTime.UtcNow,
                    Rank = 0,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = Guid.Empty,
                    IsDeleted = false,
                }
            };
        });
    }

    public override async Task HandleAsync(
        UpdateCompensatingControlRequest req,
        CancellationToken ct
    )
    {
        var response = await Observable
            .FromAsync(() => ProcessUpdateCompensatingControlRequest(req, ct))
            .Take(1)
            .FirstOrDefaultAsync();
        if (response != null)
        {
            if (response.CompensatingControl != null)
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

    private async Task<UpdateCompensatingControlResponse> ProcessUpdateCompensatingControlRequest(UpdateCompensatingControlRequest req, CancellationToken ct)
    {
        _logger.LogInformation($"Updating compensatingControl with ID: {req.CompensatingControl.CompensatingControlId}");

        Either<string, CompensatingControlDto> result = await _merchantService.UpdateCompensatingControlAsync(req.CompensatingControl.CompensatingControlId, req.CompensatingControl);

        return result.Match(
            Right: dto => new UpdateCompensatingControlResponse
            {
                CompensatingControl = dto,
                ErrorMessage = null,
            },
            Left: err => new UpdateCompensatingControlResponse
            {
                CompensatingControl = null,
                ErrorMessage = err,
            }
        );
    }
}

public class CreateComplianceOfficerEndpoint : Endpoint<CreateComplianceOfficerRequest, CreateComplianceOfficerResponse>
{
    private readonly MerchantService _merchantService;
    private readonly IAppLoggerService<CreateComplianceOfficerEndpoint> _logger;

    public CreateComplianceOfficerEndpoint(MerchantService merchantService, IAppLoggerService<CreateComplianceOfficerEndpoint> logger)
    {
        _merchantService = merchantService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/complianceOfficers");
        AllowAnonymous();
        Description(d => d.Produces<CreateComplianceOfficerResponse>(201, "application/json").Produces(400));
        Tags("ComplianceOfficers");
        Summary(s =>
        {
            s.Summary = "Create a new ComplianceOfficer";
            s.Description = "Creates a new ComplianceOfficer";
            s.ExampleRequest = new CreateComplianceOfficerRequest { };
        });
    }

    public override async Task HandleAsync(CreateComplianceOfficerRequest req, CancellationToken ct)
    {
        var response = await Observable.FromAsync(() => ProcessCreateComplianceOfficerRequest(req, ct)).Take(1).FirstOrDefaultAsync();
        if (response != null)
        {
            await SendCreatedAtAsync<CreateComplianceOfficerEndpoint>(new { complianceOfficerId = response.ComplianceOfficer.ComplianceOfficerId }, response, generateAbsoluteUrl: true, cancellation: ct);
        }
        else
        {
            await SendErrorsAsync(400, ct);
        }
    }

    private async Task<CreateComplianceOfficerResponse> ProcessCreateComplianceOfficerRequest(CreateComplianceOfficerRequest req, CancellationToken ct)
    {
        _logger.LogInformation("Creating new complianceOfficer");
        Either<string, ComplianceOfficerDto> result = await _merchantService.CreateComplianceOfficerAsync(req.ComplianceOfficer);
        return result.Match(Right: dto => new CreateComplianceOfficerResponse { ComplianceOfficer = dto, ErrorMessage = null }, Left: err => new CreateComplianceOfficerResponse { ComplianceOfficer = null, ErrorMessage = err });
    }
}

public class DeleteComplianceOfficerEndpoint : Endpoint<DeleteComplianceOfficerRequest, DeleteComplianceOfficerResponse>
{
    private readonly MerchantService _merchantService;
    private readonly IAppLoggerService<DeleteComplianceOfficerEndpoint> _logger;

    public DeleteComplianceOfficerEndpoint(MerchantService merchantService, IAppLoggerService<DeleteComplianceOfficerEndpoint> logger)
    {
        _merchantService = merchantService;
        _logger = logger;
    }

    public override void Configure()
    {
        Delete("/complianceOfficers/{complianceOfficerId}");
        AllowAnonymous();
        Description(d => d.Produces<DeleteComplianceOfficerResponse>(200, "application/json").Produces(404).Produces(400));
        Tags("ComplianceOfficers");
        Summary(s =>
        {
            s.Summary = "Delete an existing ComplianceOfficer";
            s.Description = "Deletes an existing ComplianceOfficer";
            s.ExampleRequest = new DeleteComplianceOfficerRequest { ComplianceOfficerId = Guid.NewGuid() };
        });
    }

    public override async Task HandleAsync(DeleteComplianceOfficerRequest req, CancellationToken ct)
    {
        var response = await Observable.FromAsync(() => ProcessDeleteComplianceOfficerRequest(req.ComplianceOfficerId, ct)).Take(1).FirstOrDefaultAsync();
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

    private async Task<DeleteComplianceOfficerResponse> ProcessDeleteComplianceOfficerRequest(Guid complianceOfficerId, CancellationToken ct)
    {
        _logger.LogInformation($"Deleting complianceOfficer with ID: {complianceOfficerId}");
        Either<string, bool> result = await _merchantService.DeleteComplianceOfficerAsync(complianceOfficerId);
        return result.Match(
            Right: success => new DeleteComplianceOfficerResponse { IsSuccess = success, ErrorMessage = null },
            Left: err => new DeleteComplianceOfficerResponse { IsSuccess = false, ErrorMessage = err }
        );
    }
}
public class GetComplianceOfficerByIdWithIncludesEndpoint : Endpoint<GetByIdComplianceOfficerRequest, GetByIdComplianceOfficerResponse>
{
    private readonly MerchantService _merchantService;
    private readonly IAppLoggerService<GetComplianceOfficerByIdWithIncludesEndpoint> _logger;

    public GetComplianceOfficerByIdWithIncludesEndpoint(MerchantService merchantService, IAppLoggerService<GetComplianceOfficerByIdWithIncludesEndpoint> logger)
    {
        _merchantService = merchantService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/complianceOfficers/i/{complianceOfficerId}");
        AllowAnonymous();
        Description(d => d.Produces<GetByIdComplianceOfficerResponse>(200, "application/json").Produces(404).Produces(400));
        Tags("ComplianceOfficers");
        Summary(s =>
        {
            s.Summary = "Get an ComplianceOfficer by Id With Includes";
            s.Description = "Gets an ComplianceOfficer by Id With Includes";
            s.ExampleRequest = new GetByIdComplianceOfficerRequest { ComplianceOfficerId = Guid.NewGuid() };
        });
    }

    public override async Task HandleAsync(GetByIdComplianceOfficerRequest req, CancellationToken ct)
    {
        var response = await Observable.FromAsync(() => ProcessComplianceOfficerRequest(req.ComplianceOfficerId, ct)).Take(1).FirstOrDefaultAsync();
        if (response != null)
        {
            await SendOkAsync(response, ct);
        }
        else
        {
            await SendErrorsAsync(400, ct);
        }
    }

    private async Task<GetByIdComplianceOfficerResponse> ProcessComplianceOfficerRequest(Guid complianceOfficerId, CancellationToken ct)
    {
        _logger.LogInformation($"Fetching complianceOfficer with ID: {complianceOfficerId}");
        Either<string, ComplianceOfficerDto> result = await _merchantService.GetOneFullComplianceOfficerByIdAsync(complianceOfficerId);
        return result.Match(
            Right: dto => new GetByIdComplianceOfficerResponse { ComplianceOfficer = dto, ErrorMessage = null },
            Left: err => new GetByIdComplianceOfficerResponse { ComplianceOfficer = null, ErrorMessage = err }
        );
    }
}

public class SearchComplianceOfficerEndpoint
    : Endpoint<SearchComplianceOfficerRequest, List<ComplianceOfficerDto>>
{
    private readonly MerchantService _merchantService;
    private readonly IAppLoggerService<SearchComplianceOfficerEndpoint> _logger;

    public SearchComplianceOfficerEndpoint(
        MerchantService merchantService,
        IAppLoggerService<SearchComplianceOfficerEndpoint> logger
    )
    {
        _merchantService = merchantService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("/complianceOfficers/search");
        AllowAnonymous();
        Description(d =>
            d.Produces<List<ComplianceOfficerDto>>(200, "application/json").Produces(400)
        );
        Tags("ComplianceOfficers");
        Summary(s =>
        {
            s.Summary = "Search complianceOfficers";
            s.Description = "Search complianceOfficers by various fields";
            s.ExampleRequest = new SearchComplianceOfficerRequest
            {
                SearchTerm = "example",
            };
        });
    }

    public override async Task HandleAsync(
         SearchComplianceOfficerRequest req,
        CancellationToken ct
    )
    {
        var result = await _merchantService.SearchComplianceOfficersAsync(
            req.SearchTerm
        );
        List<ValidationFailure> validationFailures =
            new AutoConstructedList<ValidationFailure>();
        await result.Match(
            Right: async complianceOfficers => await SendOkAsync(complianceOfficers, ct),
            Left: async error => await SendErrorsAsync(400, ct)
        );
    }
}

public class FilterComplianceOfficerEndpoint
    : Endpoint<FilteredComplianceOfficerRequest, ListComplianceOfficerResponse>
{
    private readonly MerchantService _merchantService;
    private readonly IAppLoggerService<FilterComplianceOfficerEndpoint> _logger;

    public FilterComplianceOfficerEndpoint(
        MerchantService merchantService,
        IAppLoggerService<FilterComplianceOfficerEndpoint> logger
    )
    {
        _merchantService = merchantService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/complianceOfficers/filtered_list");
        AllowAnonymous();
        Description(d =>
            d.Produces<ListComplianceOfficerResponse>(200, "application/json")
                .Produces(400)
        );
        Tags("ComplianceOfficers");
        Summary(s =>
        {
            s.Summary = "Filter complianceOfficers";
            s.Description = "Get a filtered and sorted list of complianceOfficers";
            s.ExampleRequest = new FilteredComplianceOfficerRequest
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
        FilteredComplianceOfficerRequest req,
        CancellationToken ct
    )
    {
        var result = await _merchantService.GetFilteredComplianceOfficersAsync(req);
        await result.Match(
            Right: async response => await SendOkAsync(response, ct),
            Left: async error => await SendErrorsAsync(400, ct)
        );
    }
}

public class GetComplianceOfficerListEndpoint
    : Endpoint<GetComplianceOfficerListRequest, ListComplianceOfficerResponse>
{
    private readonly MerchantService _merchantService;
    private readonly IAppLoggerService<GetComplianceOfficerListEndpoint> _logger;

    public GetComplianceOfficerListEndpoint(
        MerchantService merchantService,
        IAppLoggerService<GetComplianceOfficerListEndpoint> logger
    )
    {
        _merchantService = merchantService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/complianceOfficers/paged_list");
        AllowAnonymous();
        Description(d =>
            d.Produces<ListComplianceOfficerResponse>(200, "application/json")
                .Produces(400)
        );
        Tags("ComplianceOfficers");
        Summary(s =>
        {
            s.Summary = "Get a paged list of ComplianceOfficers";
            s.Description = "Gets a paginated list of ComplianceOfficers";
            s.ExampleRequest = new GetComplianceOfficerListRequest
            {
                PageNumber = 1,
                PageSize = 10,
            };
        });
    }

    public override async Task HandleAsync(
        GetComplianceOfficerListRequest req,
        CancellationToken ct
    )
    {
        ListComplianceOfficerResponse? response = await Observable
            .FromAsync(
                () =>
                    ProcessComplianceOfficerListRequest(req.PageNumber, req.PageSize, ct)
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

    private async Task<ListComplianceOfficerResponse> ProcessComplianceOfficerListRequest(
        int pageNumber,
        int pageSize,
        CancellationToken ct
    )
    {
        _logger.LogInformation(
            $"Fetching complianceOfficer list. Page: {pageNumber}, Size: {pageSize}"
        );
        Either<string, List<ComplianceOfficerDto>> result =
            await _merchantService.GetComplianceOfficerPagedList(pageNumber, pageSize);
        return result.Match(
            complianceOfficers => new ListComplianceOfficerResponse
            {
                ComplianceOfficers = complianceOfficers,
                ErrorMessage = null,
            },
            err => new ListComplianceOfficerResponse
            {
                ComplianceOfficers = null,
                ErrorMessage = err,
            }
        );
    }
}
public class UpdateComplianceOfficerEndpoint
    : Endpoint<UpdateComplianceOfficerRequest, UpdateComplianceOfficerResponse>
{
    private readonly MerchantService _merchantService;
    private readonly IAppLoggerService<UpdateComplianceOfficerEndpoint> _logger;

    public UpdateComplianceOfficerEndpoint(
        MerchantService merchantService,
        IAppLoggerService<UpdateComplianceOfficerEndpoint> logger
    )
    {
        _merchantService = merchantService;
        _logger = logger;
    }

    public override void Configure()
    {
        Put("/complianceOfficers");
        AllowAnonymous();
        Description(d =>
            d.Produces<UpdateComplianceOfficerResponse>(200, "application/json")
                .Produces(404)
                .Produces(400)
        );
        Tags("ComplianceOfficers");
        Summary(s =>
        {
            s.Summary = "Update an existing ComplianceOfficer";
            s.Description = "Updates an existing ComplianceOfficer's details";
            s.ExampleRequest = new UpdateComplianceOfficerRequest
            {
                ComplianceOfficer = new ComplianceOfficerDto
                {
                    ComplianceOfficerId = Guid.NewGuid(),
                    TenantId = Guid.Empty,
                    MerchantId = Guid.Empty,
                    OfficerCode = string.Empty,
                    FirstName = string.Empty,
                    LastName = string.Empty,
                    Email = string.Empty,
                    IsActive = false,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = Guid.Empty,
                    IsDeleted = false,
                }
            };
        });
    }

    public override async Task HandleAsync(
        UpdateComplianceOfficerRequest req,
        CancellationToken ct
    )
    {
        var response = await Observable
            .FromAsync(() => ProcessUpdateComplianceOfficerRequest(req, ct))
            .Take(1)
            .FirstOrDefaultAsync();
        if (response != null)
        {
            if (response.ComplianceOfficer != null)
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

    private async Task<UpdateComplianceOfficerResponse> ProcessUpdateComplianceOfficerRequest(UpdateComplianceOfficerRequest req, CancellationToken ct)
    {
        _logger.LogInformation($"Updating complianceOfficer with ID: {req.ComplianceOfficer.ComplianceOfficerId}");

        Either<string, ComplianceOfficerDto> result = await _merchantService.UpdateComplianceOfficerAsync(req.ComplianceOfficer.ComplianceOfficerId, req.ComplianceOfficer);

        return result.Match(
            Right: dto => new UpdateComplianceOfficerResponse
            {
                ComplianceOfficer = dto,
                ErrorMessage = null,
            },
            Left: err => new UpdateComplianceOfficerResponse
            {
                ComplianceOfficer = null,
                ErrorMessage = err,
            }
        );
    }
}

public class CreateCryptographicInventoryEndpoint : Endpoint<CreateCryptographicInventoryRequest, CreateCryptographicInventoryResponse>
{
    private readonly MerchantService _merchantService;
    private readonly IAppLoggerService<CreateCryptographicInventoryEndpoint> _logger;

    public CreateCryptographicInventoryEndpoint(MerchantService merchantService, IAppLoggerService<CreateCryptographicInventoryEndpoint> logger)
    {
        _merchantService = merchantService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/cryptographicInventories");
        AllowAnonymous();
        Description(d => d.Produces<CreateCryptographicInventoryResponse>(201, "application/json").Produces(400));
        Tags("CryptographicInventories");
        Summary(s =>
        {
            s.Summary = "Create a new CryptographicInventory";
            s.Description = "Creates a new CryptographicInventory";
            s.ExampleRequest = new CreateCryptographicInventoryRequest { };
        });
    }

    public override async Task HandleAsync(CreateCryptographicInventoryRequest req, CancellationToken ct)
    {
        var response = await Observable.FromAsync(() => ProcessCreateCryptographicInventoryRequest(req, ct)).Take(1).FirstOrDefaultAsync();
        if (response != null)
        {
            await SendCreatedAtAsync<CreateCryptographicInventoryEndpoint>(new { cryptographicInventoryId = response.CryptographicInventory.CryptographicInventoryId }, response, generateAbsoluteUrl: true, cancellation: ct);
        }
        else
        {
            await SendErrorsAsync(400, ct);
        }
    }

    private async Task<CreateCryptographicInventoryResponse> ProcessCreateCryptographicInventoryRequest(CreateCryptographicInventoryRequest req, CancellationToken ct)
    {
        _logger.LogInformation("Creating new cryptographicInventory");
        Either<string, CryptographicInventoryDto> result = await _merchantService.CreateCryptographicInventoryAsync(req.CryptographicInventory);
        return result.Match(Right: dto => new CreateCryptographicInventoryResponse { CryptographicInventory = dto, ErrorMessage = null }, Left: err => new CreateCryptographicInventoryResponse { CryptographicInventory = null, ErrorMessage = err });
    }
}

public class DeleteCryptographicInventoryEndpoint : Endpoint<DeleteCryptographicInventoryRequest, DeleteCryptographicInventoryResponse>
{
    private readonly MerchantService _merchantService;
    private readonly IAppLoggerService<DeleteCryptographicInventoryEndpoint> _logger;

    public DeleteCryptographicInventoryEndpoint(MerchantService merchantService, IAppLoggerService<DeleteCryptographicInventoryEndpoint> logger)
    {
        _merchantService = merchantService;
        _logger = logger;
    }

    public override void Configure()
    {
        Delete("/cryptographicInventories/{cryptographicInventoryId}");
        AllowAnonymous();
        Description(d => d.Produces<DeleteCryptographicInventoryResponse>(200, "application/json").Produces(404).Produces(400));
        Tags("CryptographicInventories");
        Summary(s =>
        {
            s.Summary = "Delete an existing CryptographicInventory";
            s.Description = "Deletes an existing CryptographicInventory";
            s.ExampleRequest = new DeleteCryptographicInventoryRequest { CryptographicInventoryId = Guid.NewGuid() };
        });
    }

    public override async Task HandleAsync(DeleteCryptographicInventoryRequest req, CancellationToken ct)
    {
        var response = await Observable.FromAsync(() => ProcessDeleteCryptographicInventoryRequest(req.CryptographicInventoryId, ct)).Take(1).FirstOrDefaultAsync();
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

    private async Task<DeleteCryptographicInventoryResponse> ProcessDeleteCryptographicInventoryRequest(Guid cryptographicInventoryId, CancellationToken ct)
    {
        _logger.LogInformation($"Deleting cryptographicInventory with ID: {cryptographicInventoryId}");
        Either<string, bool> result = await _merchantService.DeleteCryptographicInventoryAsync(cryptographicInventoryId);
        return result.Match(
            Right: success => new DeleteCryptographicInventoryResponse { IsSuccess = success, ErrorMessage = null },
            Left: err => new DeleteCryptographicInventoryResponse { IsSuccess = false, ErrorMessage = err }
        );
    }
}
public class GetCryptographicInventoryByIdWithIncludesEndpoint : Endpoint<GetByIdCryptographicInventoryRequest, GetByIdCryptographicInventoryResponse>
{
    private readonly MerchantService _merchantService;
    private readonly IAppLoggerService<GetCryptographicInventoryByIdWithIncludesEndpoint> _logger;

    public GetCryptographicInventoryByIdWithIncludesEndpoint(MerchantService merchantService, IAppLoggerService<GetCryptographicInventoryByIdWithIncludesEndpoint> logger)
    {
        _merchantService = merchantService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/cryptographicInventories/i/{cryptographicInventoryId}");
        AllowAnonymous();
        Description(d => d.Produces<GetByIdCryptographicInventoryResponse>(200, "application/json").Produces(404).Produces(400));
        Tags("CryptographicInventories");
        Summary(s =>
        {
            s.Summary = "Get an CryptographicInventory by Id With Includes";
            s.Description = "Gets an CryptographicInventory by Id With Includes";
            s.ExampleRequest = new GetByIdCryptographicInventoryRequest { CryptographicInventoryId = Guid.NewGuid() };
        });
    }

    public override async Task HandleAsync(GetByIdCryptographicInventoryRequest req, CancellationToken ct)
    {
        var response = await Observable.FromAsync(() => ProcessCryptographicInventoryRequest(req.CryptographicInventoryId, ct)).Take(1).FirstOrDefaultAsync();
        if (response != null)
        {
            await SendOkAsync(response, ct);
        }
        else
        {
            await SendErrorsAsync(400, ct);
        }
    }

    private async Task<GetByIdCryptographicInventoryResponse> ProcessCryptographicInventoryRequest(Guid cryptographicInventoryId, CancellationToken ct)
    {
        _logger.LogInformation($"Fetching cryptographicInventory with ID: {cryptographicInventoryId}");
        Either<string, CryptographicInventoryDto> result = await _merchantService.GetOneFullCryptographicInventoryByIdAsync(cryptographicInventoryId);
        return result.Match(
            Right: dto => new GetByIdCryptographicInventoryResponse { CryptographicInventory = dto, ErrorMessage = null },
            Left: err => new GetByIdCryptographicInventoryResponse { CryptographicInventory = null, ErrorMessage = err }
        );
    }
}

public class SearchCryptographicInventoryEndpoint
    : Endpoint<SearchCryptographicInventoryRequest, List<CryptographicInventoryDto>>
{
    private readonly MerchantService _merchantService;
    private readonly IAppLoggerService<SearchCryptographicInventoryEndpoint> _logger;

    public SearchCryptographicInventoryEndpoint(
        MerchantService merchantService,
        IAppLoggerService<SearchCryptographicInventoryEndpoint> logger
    )
    {
        _merchantService = merchantService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("/cryptographicInventories/search");
        AllowAnonymous();
        Description(d =>
            d.Produces<List<CryptographicInventoryDto>>(200, "application/json").Produces(400)
        );
        Tags("CryptographicInventories");
        Summary(s =>
        {
            s.Summary = "Search cryptographicInventories";
            s.Description = "Search cryptographicInventories by various fields";
            s.ExampleRequest = new SearchCryptographicInventoryRequest
            {
                SearchTerm = "example",
            };
        });
    }

    public override async Task HandleAsync(
         SearchCryptographicInventoryRequest req,
        CancellationToken ct
    )
    {
        var result = await _merchantService.SearchCryptographicInventoriesAsync(
            req.SearchTerm
        );
        List<ValidationFailure> validationFailures =
            new AutoConstructedList<ValidationFailure>();
        await result.Match(
            Right: async cryptographicInventories => await SendOkAsync(cryptographicInventories, ct),
            Left: async error => await SendErrorsAsync(400, ct)
        );
    }
}

public class FilterCryptographicInventoryEndpoint
    : Endpoint<FilteredCryptographicInventoryRequest, ListCryptographicInventoryResponse>
{
    private readonly MerchantService _merchantService;
    private readonly IAppLoggerService<FilterCryptographicInventoryEndpoint> _logger;

    public FilterCryptographicInventoryEndpoint(
        MerchantService merchantService,
        IAppLoggerService<FilterCryptographicInventoryEndpoint> logger
    )
    {
        _merchantService = merchantService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/cryptographicInventories/filtered_list");
        AllowAnonymous();
        Description(d =>
            d.Produces<ListCryptographicInventoryResponse>(200, "application/json")
                .Produces(400)
        );
        Tags("CryptographicInventories");
        Summary(s =>
        {
            s.Summary = "Filter cryptographicInventories";
            s.Description = "Get a filtered and sorted list of cryptographicInventories";
            s.ExampleRequest = new FilteredCryptographicInventoryRequest
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
        FilteredCryptographicInventoryRequest req,
        CancellationToken ct
    )
    {
        var result = await _merchantService.GetFilteredCryptographicInventoriesAsync(req);
        await result.Match(
            Right: async response => await SendOkAsync(response, ct),
            Left: async error => await SendErrorsAsync(400, ct)
        );
    }
}

public class GetCryptographicInventoryListEndpoint
    : Endpoint<GetCryptographicInventoryListRequest, ListCryptographicInventoryResponse>
{
    private readonly MerchantService _merchantService;
    private readonly IAppLoggerService<GetCryptographicInventoryListEndpoint> _logger;

    public GetCryptographicInventoryListEndpoint(
        MerchantService merchantService,
        IAppLoggerService<GetCryptographicInventoryListEndpoint> logger
    )
    {
        _merchantService = merchantService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/cryptographicInventories/paged_list");
        AllowAnonymous();
        Description(d =>
            d.Produces<ListCryptographicInventoryResponse>(200, "application/json")
                .Produces(400)
        );
        Tags("CryptographicInventories");
        Summary(s =>
        {
            s.Summary = "Get a paged list of CryptographicInventories";
            s.Description = "Gets a paginated list of CryptographicInventories";
            s.ExampleRequest = new GetCryptographicInventoryListRequest
            {
                PageNumber = 1,
                PageSize = 10,
            };
        });
    }

    public override async Task HandleAsync(
        GetCryptographicInventoryListRequest req,
        CancellationToken ct
    )
    {
        ListCryptographicInventoryResponse? response = await Observable
            .FromAsync(
                () =>
                    ProcessCryptographicInventoryListRequest(req.PageNumber, req.PageSize, ct)
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

    private async Task<ListCryptographicInventoryResponse> ProcessCryptographicInventoryListRequest(
        int pageNumber,
        int pageSize,
        CancellationToken ct
    )
    {
        _logger.LogInformation(
            $"Fetching cryptographicInventory list. Page: {pageNumber}, Size: {pageSize}"
        );
        Either<string, List<CryptographicInventoryDto>> result =
            await _merchantService.GetCryptographicInventoryPagedList(pageNumber, pageSize);
        return result.Match(
            cryptographicInventories => new ListCryptographicInventoryResponse
            {
                CryptographicInventories = cryptographicInventories,
                ErrorMessage = null,
            },
            err => new ListCryptographicInventoryResponse
            {
                CryptographicInventories = null,
                ErrorMessage = err,
            }
        );
    }
}
public class UpdateCryptographicInventoryEndpoint
    : Endpoint<UpdateCryptographicInventoryRequest, UpdateCryptographicInventoryResponse>
{
    private readonly MerchantService _merchantService;
    private readonly IAppLoggerService<UpdateCryptographicInventoryEndpoint> _logger;

    public UpdateCryptographicInventoryEndpoint(
        MerchantService merchantService,
        IAppLoggerService<UpdateCryptographicInventoryEndpoint> logger
    )
    {
        _merchantService = merchantService;
        _logger = logger;
    }

    public override void Configure()
    {
        Put("/cryptographicInventories");
        AllowAnonymous();
        Description(d =>
            d.Produces<UpdateCryptographicInventoryResponse>(200, "application/json")
                .Produces(404)
                .Produces(400)
        );
        Tags("CryptographicInventories");
        Summary(s =>
        {
            s.Summary = "Update an existing CryptographicInventory";
            s.Description = "Updates an existing CryptographicInventory's details";
            s.ExampleRequest = new UpdateCryptographicInventoryRequest
            {
                CryptographicInventory = new CryptographicInventoryDto
                {
                    CryptographicInventoryId = Guid.NewGuid(),
                    TenantId = Guid.Empty,
                    MerchantId = Guid.Empty,
                    KeyName = string.Empty,
                    KeyType = string.Empty,
                    Algorithm = string.Empty,
                    KeyLength = 0,
                    KeyLocation = string.Empty,
                    CreationDate = DateTime.UtcNow,
                    NextRotationDue = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = Guid.Empty,
                    IsDeleted = false,
                }
            };
        });
    }

    public override async Task HandleAsync(
        UpdateCryptographicInventoryRequest req,
        CancellationToken ct
    )
    {
        var response = await Observable
            .FromAsync(() => ProcessUpdateCryptographicInventoryRequest(req, ct))
            .Take(1)
            .FirstOrDefaultAsync();
        if (response != null)
        {
            if (response.CryptographicInventory != null)
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

    private async Task<UpdateCryptographicInventoryResponse> ProcessUpdateCryptographicInventoryRequest(UpdateCryptographicInventoryRequest req, CancellationToken ct)
    {
        _logger.LogInformation($"Updating cryptographicInventory with ID: {req.CryptographicInventory.CryptographicInventoryId}");

        Either<string, CryptographicInventoryDto> result = await _merchantService.UpdateCryptographicInventoryAsync(req.CryptographicInventory.CryptographicInventoryId, req.CryptographicInventory);

        return result.Match(
            Right: dto => new UpdateCryptographicInventoryResponse
            {
                CryptographicInventory = dto,
                ErrorMessage = null,
            },
            Left: err => new UpdateCryptographicInventoryResponse
            {
                CryptographicInventory = null,
                ErrorMessage = err,
            }
        );
    }
}

public class CreateEvidenceEndpoint : Endpoint<CreateEvidenceRequest, CreateEvidenceResponse>
{
    private readonly MerchantService _merchantService;
    private readonly IAppLoggerService<CreateEvidenceEndpoint> _logger;

    public CreateEvidenceEndpoint(MerchantService merchantService, IAppLoggerService<CreateEvidenceEndpoint> logger)
    {
        _merchantService = merchantService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/evidences");
        AllowAnonymous();
        Description(d => d.Produces<CreateEvidenceResponse>(201, "application/json").Produces(400));
        Tags("Evidences");
        Summary(s =>
        {
            s.Summary = "Create a new Evidence";
            s.Description = "Creates a new Evidence";
            s.ExampleRequest = new CreateEvidenceRequest { };
        });
    }

    public override async Task HandleAsync(CreateEvidenceRequest req, CancellationToken ct)
    {
        var response = await Observable.FromAsync(() => ProcessCreateEvidenceRequest(req, ct)).Take(1).FirstOrDefaultAsync();
        if (response != null)
        {
            await SendCreatedAtAsync<CreateEvidenceEndpoint>(new { evidenceId = response.Evidence.EvidenceId }, response, generateAbsoluteUrl: true, cancellation: ct);
        }
        else
        {
            await SendErrorsAsync(400, ct);
        }
    }

    private async Task<CreateEvidenceResponse> ProcessCreateEvidenceRequest(CreateEvidenceRequest req, CancellationToken ct)
    {
        _logger.LogInformation("Creating new evidence");
        Either<string, EvidenceDto> result = await _merchantService.CreateEvidenceAsync(req.Evidence);
        return result.Match(Right: dto => new CreateEvidenceResponse { Evidence = dto, ErrorMessage = null }, Left: err => new CreateEvidenceResponse { Evidence = null, ErrorMessage = err });
    }
}

public class DeleteEvidenceEndpoint : Endpoint<DeleteEvidenceRequest, DeleteEvidenceResponse>
{
    private readonly MerchantService _merchantService;
    private readonly IAppLoggerService<DeleteEvidenceEndpoint> _logger;

    public DeleteEvidenceEndpoint(MerchantService merchantService, IAppLoggerService<DeleteEvidenceEndpoint> logger)
    {
        _merchantService = merchantService;
        _logger = logger;
    }

    public override void Configure()
    {
        Delete("/evidences/{evidenceId}");
        AllowAnonymous();
        Description(d => d.Produces<DeleteEvidenceResponse>(200, "application/json").Produces(404).Produces(400));
        Tags("Evidences");
        Summary(s =>
        {
            s.Summary = "Delete an existing Evidence";
            s.Description = "Deletes an existing Evidence";
            s.ExampleRequest = new DeleteEvidenceRequest { EvidenceId = Guid.NewGuid() };
        });
    }

    public override async Task HandleAsync(DeleteEvidenceRequest req, CancellationToken ct)
    {
        var response = await Observable.FromAsync(() => ProcessDeleteEvidenceRequest(req.EvidenceId, ct)).Take(1).FirstOrDefaultAsync();
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

    private async Task<DeleteEvidenceResponse> ProcessDeleteEvidenceRequest(Guid evidenceId, CancellationToken ct)
    {
        _logger.LogInformation($"Deleting evidence with ID: {evidenceId}");
        Either<string, bool> result = await _merchantService.DeleteEvidenceAsync(evidenceId);
        return result.Match(
            Right: success => new DeleteEvidenceResponse { IsSuccess = success, ErrorMessage = null },
            Left: err => new DeleteEvidenceResponse { IsSuccess = false, ErrorMessage = err }
        );
    }
}
public class GetEvidenceByIdWithIncludesEndpoint : Endpoint<GetByIdEvidenceRequest, GetByIdEvidenceResponse>
{
    private readonly MerchantService _merchantService;
    private readonly IAppLoggerService<GetEvidenceByIdWithIncludesEndpoint> _logger;

    public GetEvidenceByIdWithIncludesEndpoint(MerchantService merchantService, IAppLoggerService<GetEvidenceByIdWithIncludesEndpoint> logger)
    {
        _merchantService = merchantService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/evidences/i/{evidenceId}");
        AllowAnonymous();
        Description(d => d.Produces<GetByIdEvidenceResponse>(200, "application/json").Produces(404).Produces(400));
        Tags("Evidences");
        Summary(s =>
        {
            s.Summary = "Get an Evidence by Id With Includes";
            s.Description = "Gets an Evidence by Id With Includes";
            s.ExampleRequest = new GetByIdEvidenceRequest { EvidenceId = Guid.NewGuid() };
        });
    }

    public override async Task HandleAsync(GetByIdEvidenceRequest req, CancellationToken ct)
    {
        var response = await Observable.FromAsync(() => ProcessEvidenceRequest(req.EvidenceId, ct)).Take(1).FirstOrDefaultAsync();
        if (response != null)
        {
            await SendOkAsync(response, ct);
        }
        else
        {
            await SendErrorsAsync(400, ct);
        }
    }

    private async Task<GetByIdEvidenceResponse> ProcessEvidenceRequest(Guid evidenceId, CancellationToken ct)
    {
        _logger.LogInformation($"Fetching evidence with ID: {evidenceId}");
        Either<string, EvidenceDto> result = await _merchantService.GetOneFullEvidenceByIdAsync(evidenceId);
        return result.Match(
            Right: dto => new GetByIdEvidenceResponse { Evidence = dto, ErrorMessage = null },
            Left: err => new GetByIdEvidenceResponse { Evidence = null, ErrorMessage = err }
        );
    }
}

public class SearchEvidenceEndpoint
    : Endpoint<SearchEvidenceRequest, List<EvidenceDto>>
{
    private readonly MerchantService _merchantService;
    private readonly IAppLoggerService<SearchEvidenceEndpoint> _logger;

    public SearchEvidenceEndpoint(
        MerchantService merchantService,
        IAppLoggerService<SearchEvidenceEndpoint> logger
    )
    {
        _merchantService = merchantService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("/evidences/search");
        AllowAnonymous();
        Description(d =>
            d.Produces<List<EvidenceDto>>(200, "application/json").Produces(400)
        );
        Tags("Evidences");
        Summary(s =>
        {
            s.Summary = "Search evidences";
            s.Description = "Search evidences by various fields";
            s.ExampleRequest = new SearchEvidenceRequest
            {
                SearchTerm = "example",
            };
        });
    }

    public override async Task HandleAsync(
         SearchEvidenceRequest req,
        CancellationToken ct
    )
    {
        var result = await _merchantService.SearchEvidencesAsync(
            req.SearchTerm
        );
        List<ValidationFailure> validationFailures =
            new AutoConstructedList<ValidationFailure>();
        await result.Match(
            Right: async evidences => await SendOkAsync(evidences, ct),
            Left: async error => await SendErrorsAsync(400, ct)
        );
    }
}

public class FilterEvidenceEndpoint
    : Endpoint<FilteredEvidenceRequest, ListEvidenceResponse>
{
    private readonly MerchantService _merchantService;
    private readonly IAppLoggerService<FilterEvidenceEndpoint> _logger;

    public FilterEvidenceEndpoint(
        MerchantService merchantService,
        IAppLoggerService<FilterEvidenceEndpoint> logger
    )
    {
        _merchantService = merchantService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/evidences/filtered_list");
        AllowAnonymous();
        Description(d =>
            d.Produces<ListEvidenceResponse>(200, "application/json")
                .Produces(400)
        );
        Tags("Evidences");
        Summary(s =>
        {
            s.Summary = "Filter evidences";
            s.Description = "Get a filtered and sorted list of evidences";
            s.ExampleRequest = new FilteredEvidenceRequest
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
        FilteredEvidenceRequest req,
        CancellationToken ct
    )
    {
        var result = await _merchantService.GetFilteredEvidencesAsync(req);
        await result.Match(
            Right: async response => await SendOkAsync(response, ct),
            Left: async error => await SendErrorsAsync(400, ct)
        );
    }
}

public class GetEvidenceListEndpoint
    : Endpoint<GetEvidenceListRequest, ListEvidenceResponse>
{
    private readonly MerchantService _merchantService;
    private readonly IAppLoggerService<GetEvidenceListEndpoint> _logger;

    public GetEvidenceListEndpoint(
        MerchantService merchantService,
        IAppLoggerService<GetEvidenceListEndpoint> logger
    )
    {
        _merchantService = merchantService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/evidences/paged_list");
        AllowAnonymous();
        Description(d =>
            d.Produces<ListEvidenceResponse>(200, "application/json")
                .Produces(400)
        );
        Tags("Evidences");
        Summary(s =>
        {
            s.Summary = "Get a paged list of Evidences";
            s.Description = "Gets a paginated list of Evidences";
            s.ExampleRequest = new GetEvidenceListRequest
            {
                PageNumber = 1,
                PageSize = 10,
            };
        });
    }

    public override async Task HandleAsync(
        GetEvidenceListRequest req,
        CancellationToken ct
    )
    {
        ListEvidenceResponse? response = await Observable
            .FromAsync(
                () =>
                    ProcessEvidenceListRequest(req.PageNumber, req.PageSize, ct)
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

    private async Task<ListEvidenceResponse> ProcessEvidenceListRequest(
        int pageNumber,
        int pageSize,
        CancellationToken ct
    )
    {
        _logger.LogInformation(
            $"Fetching evidence list. Page: {pageNumber}, Size: {pageSize}"
        );
        Either<string, List<EvidenceDto>> result =
            await _merchantService.GetEvidencePagedList(pageNumber, pageSize);
        return result.Match(
            evidences => new ListEvidenceResponse
            {
                Evidences = evidences,
                ErrorMessage = null,
            },
            err => new ListEvidenceResponse
            {
                Evidences = null,
                ErrorMessage = err,
            }
        );
    }
}
public class UpdateEvidenceEndpoint
    : Endpoint<UpdateEvidenceRequest, UpdateEvidenceResponse>
{
    private readonly MerchantService _merchantService;
    private readonly IAppLoggerService<UpdateEvidenceEndpoint> _logger;

    public UpdateEvidenceEndpoint(
        MerchantService merchantService,
        IAppLoggerService<UpdateEvidenceEndpoint> logger
    )
    {
        _merchantService = merchantService;
        _logger = logger;
    }

    public override void Configure()
    {
        Put("/evidences");
        AllowAnonymous();
        Description(d =>
            d.Produces<UpdateEvidenceResponse>(200, "application/json")
                .Produces(404)
                .Produces(400)
        );
        Tags("Evidences");
        Summary(s =>
        {
            s.Summary = "Update an existing Evidence";
            s.Description = "Updates an existing Evidence's details";
            s.ExampleRequest = new UpdateEvidenceRequest
            {
                Evidence = new EvidenceDto
                {
                    EvidenceId = Guid.NewGuid(),
                    TenantId = Guid.Empty,
                    MerchantId = Guid.Empty,
                    EvidenceCode = string.Empty,
                    EvidenceTitle = string.Empty,
                    EvidenceType = 0,
                    CollectedDate = DateTime.UtcNow,
                    IsValid = false,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = Guid.Empty,
              
                    IsDeleted = false,
                }
            };
        });
    }

    public override async Task HandleAsync(
        UpdateEvidenceRequest req,
        CancellationToken ct
    )
    {
        var response = await Observable
            .FromAsync(() => ProcessUpdateEvidenceRequest(req, ct))
            .Take(1)
            .FirstOrDefaultAsync();
        if (response != null)
        {
            if (response.Evidence != null)
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

    private async Task<UpdateEvidenceResponse> ProcessUpdateEvidenceRequest(UpdateEvidenceRequest req, CancellationToken ct)
    {
        _logger.LogInformation($"Updating evidence with ID: {req.Evidence.EvidenceId}");

        Either<string, EvidenceDto> result = await _merchantService.UpdateEvidenceAsync(req.Evidence.EvidenceId, req.Evidence);

        return result.Match(
            Right: dto => new UpdateEvidenceResponse
            {
                Evidence = dto,
                ErrorMessage = null,
            },
            Left: err => new UpdateEvidenceResponse
            {
                Evidence = null,
                ErrorMessage = err,
            }
        );
    }
}

public class CreateNetworkSegmentationEndpoint : Endpoint<CreateNetworkSegmentationRequest, CreateNetworkSegmentationResponse>
{
    private readonly MerchantService _merchantService;
    private readonly IAppLoggerService<CreateNetworkSegmentationEndpoint> _logger;

    public CreateNetworkSegmentationEndpoint(MerchantService merchantService, IAppLoggerService<CreateNetworkSegmentationEndpoint> logger)
    {
        _merchantService = merchantService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/networkSegmentations");
        AllowAnonymous();
        Description(d => d.Produces<CreateNetworkSegmentationResponse>(201, "application/json").Produces(400));
        Tags("NetworkSegmentations");
        Summary(s =>
        {
            s.Summary = "Create a new NetworkSegmentation";
            s.Description = "Creates a new NetworkSegmentation";
            s.ExampleRequest = new CreateNetworkSegmentationRequest { };
        });
    }

    public override async Task HandleAsync(CreateNetworkSegmentationRequest req, CancellationToken ct)
    {
        var response = await Observable.FromAsync(() => ProcessCreateNetworkSegmentationRequest(req, ct)).Take(1).FirstOrDefaultAsync();
        if (response != null)
        {
            await SendCreatedAtAsync<CreateNetworkSegmentationEndpoint>(new { networkSegmentationId = response.NetworkSegmentation.NetworkSegmentationId }, response, generateAbsoluteUrl: true, cancellation: ct);
        }
        else
        {
            await SendErrorsAsync(400, ct);
        }
    }

    private async Task<CreateNetworkSegmentationResponse> ProcessCreateNetworkSegmentationRequest(CreateNetworkSegmentationRequest req, CancellationToken ct)
    {
        _logger.LogInformation("Creating new networkSegmentation");
        Either<string, NetworkSegmentationDto> result = await _merchantService.CreateNetworkSegmentationAsync(req.NetworkSegmentation);
        return result.Match(Right: dto => new CreateNetworkSegmentationResponse { NetworkSegmentation = dto, ErrorMessage = null }, Left: err => new CreateNetworkSegmentationResponse { NetworkSegmentation = null, ErrorMessage = err });
    }
}

public class DeleteNetworkSegmentationEndpoint : Endpoint<DeleteNetworkSegmentationRequest, DeleteNetworkSegmentationResponse>
{
    private readonly MerchantService _merchantService;
    private readonly IAppLoggerService<DeleteNetworkSegmentationEndpoint> _logger;

    public DeleteNetworkSegmentationEndpoint(MerchantService merchantService, IAppLoggerService<DeleteNetworkSegmentationEndpoint> logger)
    {
        _merchantService = merchantService;
        _logger = logger;
    }

    public override void Configure()
    {
        Delete("/networkSegmentations/{networkSegmentationId}");
        AllowAnonymous();
        Description(d => d.Produces<DeleteNetworkSegmentationResponse>(200, "application/json").Produces(404).Produces(400));
        Tags("NetworkSegmentations");
        Summary(s =>
        {
            s.Summary = "Delete an existing NetworkSegmentation";
            s.Description = "Deletes an existing NetworkSegmentation";
            s.ExampleRequest = new DeleteNetworkSegmentationRequest { NetworkSegmentationId = Guid.NewGuid() };
        });
    }

    public override async Task HandleAsync(DeleteNetworkSegmentationRequest req, CancellationToken ct)
    {
        var response = await Observable.FromAsync(() => ProcessDeleteNetworkSegmentationRequest(req.NetworkSegmentationId, ct)).Take(1).FirstOrDefaultAsync();
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

    private async Task<DeleteNetworkSegmentationResponse> ProcessDeleteNetworkSegmentationRequest(Guid networkSegmentationId, CancellationToken ct)
    {
        _logger.LogInformation($"Deleting networkSegmentation with ID: {networkSegmentationId}");
        Either<string, bool> result = await _merchantService.DeleteNetworkSegmentationAsync(networkSegmentationId);
        return result.Match(
            Right: success => new DeleteNetworkSegmentationResponse { IsSuccess = success, ErrorMessage = null },
            Left: err => new DeleteNetworkSegmentationResponse { IsSuccess = false, ErrorMessage = err }
        );
    }
}
public class GetNetworkSegmentationByIdWithIncludesEndpoint : Endpoint<GetByIdNetworkSegmentationRequest, GetByIdNetworkSegmentationResponse>
{
    private readonly MerchantService _merchantService;
    private readonly IAppLoggerService<GetNetworkSegmentationByIdWithIncludesEndpoint> _logger;

    public GetNetworkSegmentationByIdWithIncludesEndpoint(MerchantService merchantService, IAppLoggerService<GetNetworkSegmentationByIdWithIncludesEndpoint> logger)
    {
        _merchantService = merchantService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/networkSegmentations/i/{networkSegmentationId}");
        AllowAnonymous();
        Description(d => d.Produces<GetByIdNetworkSegmentationResponse>(200, "application/json").Produces(404).Produces(400));
        Tags("NetworkSegmentations");
        Summary(s =>
        {
            s.Summary = "Get an NetworkSegmentation by Id With Includes";
            s.Description = "Gets an NetworkSegmentation by Id With Includes";
            s.ExampleRequest = new GetByIdNetworkSegmentationRequest { NetworkSegmentationId = Guid.NewGuid() };
        });
    }

    public override async Task HandleAsync(GetByIdNetworkSegmentationRequest req, CancellationToken ct)
    {
        var response = await Observable.FromAsync(() => ProcessNetworkSegmentationRequest(req.NetworkSegmentationId, ct)).Take(1).FirstOrDefaultAsync();
        if (response != null)
        {
            await SendOkAsync(response, ct);
        }
        else
        {
            await SendErrorsAsync(400, ct);
        }
    }

    private async Task<GetByIdNetworkSegmentationResponse> ProcessNetworkSegmentationRequest(Guid networkSegmentationId, CancellationToken ct)
    {
        _logger.LogInformation($"Fetching networkSegmentation with ID: {networkSegmentationId}");
        Either<string, NetworkSegmentationDto> result = await _merchantService.GetOneFullNetworkSegmentationByIdAsync(networkSegmentationId);
        return result.Match(
            Right: dto => new GetByIdNetworkSegmentationResponse { NetworkSegmentation = dto, ErrorMessage = null },
            Left: err => new GetByIdNetworkSegmentationResponse { NetworkSegmentation = null, ErrorMessage = err }
        );
    }
}

public class SearchNetworkSegmentationEndpoint
    : Endpoint<SearchNetworkSegmentationRequest, List<NetworkSegmentationDto>>
{
    private readonly MerchantService _merchantService;
    private readonly IAppLoggerService<SearchNetworkSegmentationEndpoint> _logger;

    public SearchNetworkSegmentationEndpoint(
        MerchantService merchantService,
        IAppLoggerService<SearchNetworkSegmentationEndpoint> logger
    )
    {
        _merchantService = merchantService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("/networkSegmentations/search");
        AllowAnonymous();
        Description(d =>
            d.Produces<List<NetworkSegmentationDto>>(200, "application/json").Produces(400)
        );
        Tags("NetworkSegmentations");
        Summary(s =>
        {
            s.Summary = "Search networkSegmentations";
            s.Description = "Search networkSegmentations by various fields";
            s.ExampleRequest = new SearchNetworkSegmentationRequest
            {
                SearchTerm = "example",
            };
        });
    }

    public override async Task HandleAsync(
         SearchNetworkSegmentationRequest req,
        CancellationToken ct
    )
    {
        var result = await _merchantService.SearchNetworkSegmentationsAsync(
            req.SearchTerm
        );
        List<ValidationFailure> validationFailures =
            new AutoConstructedList<ValidationFailure>();
        await result.Match(
            Right: async networkSegmentations => await SendOkAsync(networkSegmentations, ct),
            Left: async error => await SendErrorsAsync(400, ct)
        );
    }
}

public class FilterNetworkSegmentationEndpoint
    : Endpoint<FilteredNetworkSegmentationRequest, ListNetworkSegmentationResponse>
{
    private readonly MerchantService _merchantService;
    private readonly IAppLoggerService<FilterNetworkSegmentationEndpoint> _logger;

    public FilterNetworkSegmentationEndpoint(
        MerchantService merchantService,
        IAppLoggerService<FilterNetworkSegmentationEndpoint> logger
    )
    {
        _merchantService = merchantService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/networkSegmentations/filtered_list");
        AllowAnonymous();
        Description(d =>
            d.Produces<ListNetworkSegmentationResponse>(200, "application/json")
                .Produces(400)
        );
        Tags("NetworkSegmentations");
        Summary(s =>
        {
            s.Summary = "Filter networkSegmentations";
            s.Description = "Get a filtered and sorted list of networkSegmentations";
            s.ExampleRequest = new FilteredNetworkSegmentationRequest
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
        FilteredNetworkSegmentationRequest req,
        CancellationToken ct
    )
    {
        var result = await _merchantService.GetFilteredNetworkSegmentationsAsync(req);
        await result.Match(
            Right: async response => await SendOkAsync(response, ct),
            Left: async error => await SendErrorsAsync(400, ct)
        );
    }
}

public class GetNetworkSegmentationListEndpoint
    : Endpoint<GetNetworkSegmentationListRequest, ListNetworkSegmentationResponse>
{
    private readonly MerchantService _merchantService;
    private readonly IAppLoggerService<GetNetworkSegmentationListEndpoint> _logger;

    public GetNetworkSegmentationListEndpoint(
        MerchantService merchantService,
        IAppLoggerService<GetNetworkSegmentationListEndpoint> logger
    )
    {
        _merchantService = merchantService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/networkSegmentations/paged_list");
        AllowAnonymous();
        Description(d =>
            d.Produces<ListNetworkSegmentationResponse>(200, "application/json")
                .Produces(400)
        );
        Tags("NetworkSegmentations");
        Summary(s =>
        {
            s.Summary = "Get a paged list of NetworkSegmentations";
            s.Description = "Gets a paginated list of NetworkSegmentations";
            s.ExampleRequest = new GetNetworkSegmentationListRequest
            {
                PageNumber = 1,
                PageSize = 10,
            };
        });
    }

    public override async Task HandleAsync(
        GetNetworkSegmentationListRequest req,
        CancellationToken ct
    )
    {
        ListNetworkSegmentationResponse? response = await Observable
            .FromAsync(
                () =>
                    ProcessNetworkSegmentationListRequest(req.PageNumber, req.PageSize, ct)
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

    private async Task<ListNetworkSegmentationResponse> ProcessNetworkSegmentationListRequest(
        int pageNumber,
        int pageSize,
        CancellationToken ct
    )
    {
        _logger.LogInformation(
            $"Fetching networkSegmentation list. Page: {pageNumber}, Size: {pageSize}"
        );
        Either<string, List<NetworkSegmentationDto>> result =
            await _merchantService.GetNetworkSegmentationPagedList(pageNumber, pageSize);
        return result.Match(
            networkSegmentations => new ListNetworkSegmentationResponse
            {
                NetworkSegmentations = networkSegmentations,
                ErrorMessage = null,
            },
            err => new ListNetworkSegmentationResponse
            {
                NetworkSegmentations = null,
                ErrorMessage = err,
            }
        );
    }
}
public class UpdateNetworkSegmentationEndpoint
    : Endpoint<UpdateNetworkSegmentationRequest, UpdateNetworkSegmentationResponse>
{
    private readonly MerchantService _merchantService;
    private readonly IAppLoggerService<UpdateNetworkSegmentationEndpoint> _logger;

    public UpdateNetworkSegmentationEndpoint(
        MerchantService merchantService,
        IAppLoggerService<UpdateNetworkSegmentationEndpoint> logger
    )
    {
        _merchantService = merchantService;
        _logger = logger;
    }

    public override void Configure()
    {
        Put("/networkSegmentations");
        AllowAnonymous();
        Description(d =>
            d.Produces<UpdateNetworkSegmentationResponse>(200, "application/json")
                .Produces(404)
                .Produces(400)
        );
        Tags("NetworkSegmentations");
        Summary(s =>
        {
            s.Summary = "Update an existing NetworkSegmentation";
            s.Description = "Updates an existing NetworkSegmentation's details";
            s.ExampleRequest = new UpdateNetworkSegmentationRequest
            {
                NetworkSegmentation = new NetworkSegmentationDto
                {
                    NetworkSegmentationId = Guid.NewGuid(),
                    TenantId = Guid.Empty,
                    MerchantId = Guid.Empty,
                    SegmentName = string.Empty,
                    IPRange = string.Empty,
                    IsInCDE = false,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = Guid.Empty,
                   
                    IsDeleted = false,
                }
            };
        });
    }

    public override async Task HandleAsync(
        UpdateNetworkSegmentationRequest req,
        CancellationToken ct
    )
    {
        var response = await Observable
            .FromAsync(() => ProcessUpdateNetworkSegmentationRequest(req, ct))
            .Take(1)
            .FirstOrDefaultAsync();
        if (response != null)
        {
            if (response.NetworkSegmentation != null)
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

    private async Task<UpdateNetworkSegmentationResponse> ProcessUpdateNetworkSegmentationRequest(UpdateNetworkSegmentationRequest req, CancellationToken ct)
    {
        _logger.LogInformation($"Updating networkSegmentation with ID: {req.NetworkSegmentation.NetworkSegmentationId}");

        Either<string, NetworkSegmentationDto> result = await _merchantService.UpdateNetworkSegmentationAsync(req.NetworkSegmentation.NetworkSegmentationId, req.NetworkSegmentation);

        return result.Match(
            Right: dto => new UpdateNetworkSegmentationResponse
            {
                NetworkSegmentation = dto,
                ErrorMessage = null,
            },
            Left: err => new UpdateNetworkSegmentationResponse
            {
                NetworkSegmentation = null,
                ErrorMessage = err,
            }
        );
    }
}

public class CreatePaymentChannelEndpoint : Endpoint<CreatePaymentChannelRequest, CreatePaymentChannelResponse>
{
    private readonly MerchantService _merchantService;
    private readonly IAppLoggerService<CreatePaymentChannelEndpoint> _logger;

    public CreatePaymentChannelEndpoint(MerchantService merchantService, IAppLoggerService<CreatePaymentChannelEndpoint> logger)
    {
        _merchantService = merchantService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/paymentChannels");
        AllowAnonymous();
        Description(d => d.Produces<CreatePaymentChannelResponse>(201, "application/json").Produces(400));
        Tags("PaymentChannels");
        Summary(s =>
        {
            s.Summary = "Create a new PaymentChannel";
            s.Description = "Creates a new PaymentChannel";
            s.ExampleRequest = new CreatePaymentChannelRequest { };
        });
    }

    public override async Task HandleAsync(CreatePaymentChannelRequest req, CancellationToken ct)
    {
        var response = await Observable.FromAsync(() => ProcessCreatePaymentChannelRequest(req, ct)).Take(1).FirstOrDefaultAsync();
        if (response != null)
        {
            await SendCreatedAtAsync<CreatePaymentChannelEndpoint>(new { paymentChannelId = response.PaymentChannel.PaymentChannelId }, response, generateAbsoluteUrl: true, cancellation: ct);
        }
        else
        {
            await SendErrorsAsync(400, ct);
        }
    }

    private async Task<CreatePaymentChannelResponse> ProcessCreatePaymentChannelRequest(CreatePaymentChannelRequest req, CancellationToken ct)
    {
        _logger.LogInformation("Creating new paymentChannel");
        Either<string, PaymentChannelDto> result = await _merchantService.CreatePaymentChannelAsync(req.PaymentChannel);
        return result.Match(Right: dto => new CreatePaymentChannelResponse { PaymentChannel = dto, ErrorMessage = null }, Left: err => new CreatePaymentChannelResponse { PaymentChannel = null, ErrorMessage = err });
    }
}

public class DeletePaymentChannelEndpoint : Endpoint<DeletePaymentChannelRequest, DeletePaymentChannelResponse>
{
    private readonly MerchantService _merchantService;
    private readonly IAppLoggerService<DeletePaymentChannelEndpoint> _logger;

    public DeletePaymentChannelEndpoint(MerchantService merchantService, IAppLoggerService<DeletePaymentChannelEndpoint> logger)
    {
        _merchantService = merchantService;
        _logger = logger;
    }

    public override void Configure()
    {
        Delete("/paymentChannels/{paymentChannelId}");
        AllowAnonymous();
        Description(d => d.Produces<DeletePaymentChannelResponse>(200, "application/json").Produces(404).Produces(400));
        Tags("PaymentChannels");
        Summary(s =>
        {
            s.Summary = "Delete an existing PaymentChannel";
            s.Description = "Deletes an existing PaymentChannel";
            s.ExampleRequest = new DeletePaymentChannelRequest { PaymentChannelId = Guid.NewGuid() };
        });
    }

    public override async Task HandleAsync(DeletePaymentChannelRequest req, CancellationToken ct)
    {
        var response = await Observable.FromAsync(() => ProcessDeletePaymentChannelRequest(req.PaymentChannelId, ct)).Take(1).FirstOrDefaultAsync();
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

    private async Task<DeletePaymentChannelResponse> ProcessDeletePaymentChannelRequest(Guid paymentChannelId, CancellationToken ct)
    {
        _logger.LogInformation($"Deleting paymentChannel with ID: {paymentChannelId}");
        Either<string, bool> result = await _merchantService.DeletePaymentChannelAsync(paymentChannelId);
        return result.Match(
            Right: success => new DeletePaymentChannelResponse { IsSuccess = success, ErrorMessage = null },
            Left: err => new DeletePaymentChannelResponse { IsSuccess = false, ErrorMessage = err }
        );
    }
}
public class GetPaymentChannelByIdWithIncludesEndpoint : Endpoint<GetByIdPaymentChannelRequest, GetByIdPaymentChannelResponse>
{
    private readonly MerchantService _merchantService;
    private readonly IAppLoggerService<GetPaymentChannelByIdWithIncludesEndpoint> _logger;

    public GetPaymentChannelByIdWithIncludesEndpoint(MerchantService merchantService, IAppLoggerService<GetPaymentChannelByIdWithIncludesEndpoint> logger)
    {
        _merchantService = merchantService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/paymentChannels/i/{paymentChannelId}");
        AllowAnonymous();
        Description(d => d.Produces<GetByIdPaymentChannelResponse>(200, "application/json").Produces(404).Produces(400));
        Tags("PaymentChannels");
        Summary(s =>
        {
            s.Summary = "Get an PaymentChannel by Id With Includes";
            s.Description = "Gets an PaymentChannel by Id With Includes";
            s.ExampleRequest = new GetByIdPaymentChannelRequest { PaymentChannelId = Guid.NewGuid() };
        });
    }

    public override async Task HandleAsync(GetByIdPaymentChannelRequest req, CancellationToken ct)
    {
        var response = await Observable.FromAsync(() => ProcessPaymentChannelRequest(req.PaymentChannelId, ct)).Take(1).FirstOrDefaultAsync();
        if (response != null)
        {
            await SendOkAsync(response, ct);
        }
        else
        {
            await SendErrorsAsync(400, ct);
        }
    }

    private async Task<GetByIdPaymentChannelResponse> ProcessPaymentChannelRequest(Guid paymentChannelId, CancellationToken ct)
    {
        _logger.LogInformation($"Fetching paymentChannel with ID: {paymentChannelId}");
        Either<string, PaymentChannelDto> result = await _merchantService.GetOneFullPaymentChannelByIdAsync(paymentChannelId);
        return result.Match(
            Right: dto => new GetByIdPaymentChannelResponse { PaymentChannel = dto, ErrorMessage = null },
            Left: err => new GetByIdPaymentChannelResponse { PaymentChannel = null, ErrorMessage = err }
        );
    }
}

public class SearchPaymentChannelEndpoint
    : Endpoint<SearchPaymentChannelRequest, List<PaymentChannelDto>>
{
    private readonly MerchantService _merchantService;
    private readonly IAppLoggerService<SearchPaymentChannelEndpoint> _logger;

    public SearchPaymentChannelEndpoint(
        MerchantService merchantService,
        IAppLoggerService<SearchPaymentChannelEndpoint> logger
    )
    {
        _merchantService = merchantService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("/paymentChannels/search");
        AllowAnonymous();
        Description(d =>
            d.Produces<List<PaymentChannelDto>>(200, "application/json").Produces(400)
        );
        Tags("PaymentChannels");
        Summary(s =>
        {
            s.Summary = "Search paymentChannels";
            s.Description = "Search paymentChannels by various fields";
            s.ExampleRequest = new SearchPaymentChannelRequest
            {
                SearchTerm = "example",
            };
        });
    }

    public override async Task HandleAsync(
         SearchPaymentChannelRequest req,
        CancellationToken ct
    )
    {
        var result = await _merchantService.SearchPaymentChannelsAsync(
            req.SearchTerm
        );
        List<ValidationFailure> validationFailures =
            new AutoConstructedList<ValidationFailure>();
        await result.Match(
            Right: async paymentChannels => await SendOkAsync(paymentChannels, ct),
            Left: async error => await SendErrorsAsync(400, ct)
        );
    }
}

public class FilterPaymentChannelEndpoint
    : Endpoint<FilteredPaymentChannelRequest, ListPaymentChannelResponse>
{
    private readonly MerchantService _merchantService;
    private readonly IAppLoggerService<FilterPaymentChannelEndpoint> _logger;

    public FilterPaymentChannelEndpoint(
        MerchantService merchantService,
        IAppLoggerService<FilterPaymentChannelEndpoint> logger
    )
    {
        _merchantService = merchantService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/paymentChannels/filtered_list");
        AllowAnonymous();
        Description(d =>
            d.Produces<ListPaymentChannelResponse>(200, "application/json")
                .Produces(400)
        );
        Tags("PaymentChannels");
        Summary(s =>
        {
            s.Summary = "Filter paymentChannels";
            s.Description = "Get a filtered and sorted list of paymentChannels";
            s.ExampleRequest = new FilteredPaymentChannelRequest
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
        FilteredPaymentChannelRequest req,
        CancellationToken ct
    )
    {
        var result = await _merchantService.GetFilteredPaymentChannelsAsync(req);
        await result.Match(
            Right: async response => await SendOkAsync(response, ct),
            Left: async error => await SendErrorsAsync(400, ct)
        );
    }
}

public class GetPaymentChannelListEndpoint
    : Endpoint<GetPaymentChannelListRequest, ListPaymentChannelResponse>
{
    private readonly MerchantService _merchantService;
    private readonly IAppLoggerService<GetPaymentChannelListEndpoint> _logger;

    public GetPaymentChannelListEndpoint(
        MerchantService merchantService,
        IAppLoggerService<GetPaymentChannelListEndpoint> logger
    )
    {
        _merchantService = merchantService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/paymentChannels/paged_list");
        AllowAnonymous();
        Description(d =>
            d.Produces<ListPaymentChannelResponse>(200, "application/json")
                .Produces(400)
        );
        Tags("PaymentChannels");
        Summary(s =>
        {
            s.Summary = "Get a paged list of PaymentChannels";
            s.Description = "Gets a paginated list of PaymentChannels";
            s.ExampleRequest = new GetPaymentChannelListRequest
            {
                PageNumber = 1,
                PageSize = 10,
            };
        });
    }

    public override async Task HandleAsync(
        GetPaymentChannelListRequest req,
        CancellationToken ct
    )
    {
        ListPaymentChannelResponse? response = await Observable
            .FromAsync(
                () =>
                    ProcessPaymentChannelListRequest(req.PageNumber, req.PageSize, ct)
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

    private async Task<ListPaymentChannelResponse> ProcessPaymentChannelListRequest(
        int pageNumber,
        int pageSize,
        CancellationToken ct
    )
    {
        _logger.LogInformation(
            $"Fetching paymentChannel list. Page: {pageNumber}, Size: {pageSize}"
        );
        Either<string, List<PaymentChannelDto>> result =
            await _merchantService.GetPaymentChannelPagedList(pageNumber, pageSize);
        return result.Match(
            paymentChannels => new ListPaymentChannelResponse
            {
                PaymentChannels = paymentChannels,
                ErrorMessage = null,
            },
            err => new ListPaymentChannelResponse
            {
                PaymentChannels = null,
                ErrorMessage = err,
            }
        );
    }
}
public class UpdatePaymentChannelEndpoint
    : Endpoint<UpdatePaymentChannelRequest, UpdatePaymentChannelResponse>
{
    private readonly MerchantService _merchantService;
    private readonly IAppLoggerService<UpdatePaymentChannelEndpoint> _logger;

    public UpdatePaymentChannelEndpoint(
        MerchantService merchantService,
        IAppLoggerService<UpdatePaymentChannelEndpoint> logger
    )
    {
        _merchantService = merchantService;
        _logger = logger;
    }

    public override void Configure()
    {
        Put("/paymentChannels");
        AllowAnonymous();
        Description(d =>
            d.Produces<UpdatePaymentChannelResponse>(200, "application/json")
                .Produces(404)
                .Produces(400)
        );
        Tags("PaymentChannels");
        Summary(s =>
        {
            s.Summary = "Update an existing PaymentChannel";
            s.Description = "Updates an existing PaymentChannel's details";
            s.ExampleRequest = new UpdatePaymentChannelRequest
            {
                PaymentChannel = new PaymentChannelDto
                {
                    PaymentChannelId = Guid.NewGuid(),
                    TenantId = Guid.Empty,
                    MerchantId = Guid.Empty,
                    ChannelCode = string.Empty,
                    ChannelName = string.Empty,
                    ChannelType = 0,
                    ProcessingVolume = 0m,
                    IsInScope = false,
                    TokenizationEnabled = false,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = Guid.Empty,
                    
                    IsDeleted = false,
                }
            };
        });
    }

    public override async Task HandleAsync(
        UpdatePaymentChannelRequest req,
        CancellationToken ct
    )
    {
        var response = await Observable
            .FromAsync(() => ProcessUpdatePaymentChannelRequest(req, ct))
            .Take(1)
            .FirstOrDefaultAsync();
        if (response != null)
        {
            if (response.PaymentChannel != null)
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

    private async Task<UpdatePaymentChannelResponse> ProcessUpdatePaymentChannelRequest(UpdatePaymentChannelRequest req, CancellationToken ct)
    {
        _logger.LogInformation($"Updating paymentChannel with ID: {req.PaymentChannel.PaymentChannelId}");

        Either<string, PaymentChannelDto> result = await _merchantService.UpdatePaymentChannelAsync(req.PaymentChannel.PaymentChannelId, req.PaymentChannel);

        return result.Match(
            Right: dto => new UpdatePaymentChannelResponse
            {
                PaymentChannel = dto,
                ErrorMessage = null,
            },
            Left: err => new UpdatePaymentChannelResponse
            {
                PaymentChannel = null,
                ErrorMessage = err,
            }
        );
    }
}

public class CreateServiceProviderEndpoint : Endpoint<CreateServiceProviderRequest, CreateServiceProviderResponse>
{
    private readonly MerchantService _merchantService;
    private readonly IAppLoggerService<CreateServiceProviderEndpoint> _logger;

    public CreateServiceProviderEndpoint(MerchantService merchantService, IAppLoggerService<CreateServiceProviderEndpoint> logger)
    {
        _merchantService = merchantService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/serviceProviders");
        AllowAnonymous();
        Description(d => d.Produces<CreateServiceProviderResponse>(201, "application/json").Produces(400));
        Tags("ServiceProviders");
        Summary(s =>
        {
            s.Summary = "Create a new ServiceProvider";
            s.Description = "Creates a new ServiceProvider";
            s.ExampleRequest = new CreateServiceProviderRequest { };
        });
    }

    public override async Task HandleAsync(CreateServiceProviderRequest req, CancellationToken ct)
    {
        var response = await Observable.FromAsync(() => ProcessCreateServiceProviderRequest(req, ct)).Take(1).FirstOrDefaultAsync();
        if (response != null)
        {
            await SendCreatedAtAsync<CreateServiceProviderEndpoint>(new { serviceProviderId = response.ServiceProvider.ServiceProviderId }, response, generateAbsoluteUrl: true, cancellation: ct);
        }
        else
        {
            await SendErrorsAsync(400, ct);
        }
    }

    private async Task<CreateServiceProviderResponse> ProcessCreateServiceProviderRequest(CreateServiceProviderRequest req, CancellationToken ct)
    {
        _logger.LogInformation("Creating new serviceProvider");
        Either<string, ServiceProviderDto> result = await _merchantService.CreateServiceProviderAsync(req.ServiceProvider);
        return result.Match(Right: dto => new CreateServiceProviderResponse { ServiceProvider = dto, ErrorMessage = null }, Left: err => new CreateServiceProviderResponse { ServiceProvider = null, ErrorMessage = err });
    }
}

public class DeleteServiceProviderEndpoint : Endpoint<DeleteServiceProviderRequest, DeleteServiceProviderResponse>
{
    private readonly MerchantService _merchantService;
    private readonly IAppLoggerService<DeleteServiceProviderEndpoint> _logger;

    public DeleteServiceProviderEndpoint(MerchantService merchantService, IAppLoggerService<DeleteServiceProviderEndpoint> logger)
    {
        _merchantService = merchantService;
        _logger = logger;
    }

    public override void Configure()
    {
        Delete("/serviceProviders/{serviceProviderId}");
        AllowAnonymous();
        Description(d => d.Produces<DeleteServiceProviderResponse>(200, "application/json").Produces(404).Produces(400));
        Tags("ServiceProviders");
        Summary(s =>
        {
            s.Summary = "Delete an existing ServiceProvider";
            s.Description = "Deletes an existing ServiceProvider";
            s.ExampleRequest = new DeleteServiceProviderRequest { ServiceProviderId = Guid.NewGuid() };
        });
    }

    public override async Task HandleAsync(DeleteServiceProviderRequest req, CancellationToken ct)
    {
        var response = await Observable.FromAsync(() => ProcessDeleteServiceProviderRequest(req.ServiceProviderId, ct)).Take(1).FirstOrDefaultAsync();
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

    private async Task<DeleteServiceProviderResponse> ProcessDeleteServiceProviderRequest(Guid serviceProviderId, CancellationToken ct)
    {
        _logger.LogInformation($"Deleting serviceProvider with ID: {serviceProviderId}");
        Either<string, bool> result = await _merchantService.DeleteServiceProviderAsync(serviceProviderId);
        return result.Match(
            Right: success => new DeleteServiceProviderResponse { IsSuccess = success, ErrorMessage = null },
            Left: err => new DeleteServiceProviderResponse { IsSuccess = false, ErrorMessage = err }
        );
    }
}
public class GetServiceProviderByIdWithIncludesEndpoint : Endpoint<GetByIdServiceProviderRequest, GetByIdServiceProviderResponse>
{
    private readonly MerchantService _merchantService;
    private readonly IAppLoggerService<GetServiceProviderByIdWithIncludesEndpoint> _logger;

    public GetServiceProviderByIdWithIncludesEndpoint(MerchantService merchantService, IAppLoggerService<GetServiceProviderByIdWithIncludesEndpoint> logger)
    {
        _merchantService = merchantService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/serviceProviders/i/{serviceProviderId}");
        AllowAnonymous();
        Description(d => d.Produces<GetByIdServiceProviderResponse>(200, "application/json").Produces(404).Produces(400));
        Tags("ServiceProviders");
        Summary(s =>
        {
            s.Summary = "Get an ServiceProvider by Id With Includes";
            s.Description = "Gets an ServiceProvider by Id With Includes";
            s.ExampleRequest = new GetByIdServiceProviderRequest { ServiceProviderId = Guid.NewGuid() };
        });
    }

    public override async Task HandleAsync(GetByIdServiceProviderRequest req, CancellationToken ct)
    {
        var response = await Observable.FromAsync(() => ProcessServiceProviderRequest(req.ServiceProviderId, ct)).Take(1).FirstOrDefaultAsync();
        if (response != null)
        {
            await SendOkAsync(response, ct);
        }
        else
        {
            await SendErrorsAsync(400, ct);
        }
    }

    private async Task<GetByIdServiceProviderResponse> ProcessServiceProviderRequest(Guid serviceProviderId, CancellationToken ct)
    {
        _logger.LogInformation($"Fetching serviceProvider with ID: {serviceProviderId}");
        Either<string, ServiceProviderDto> result = await _merchantService.GetOneFullServiceProviderByIdAsync(serviceProviderId);
        return result.Match(
            Right: dto => new GetByIdServiceProviderResponse { ServiceProvider = dto, ErrorMessage = null },
            Left: err => new GetByIdServiceProviderResponse { ServiceProvider = null, ErrorMessage = err }
        );
    }
}

public class SearchServiceProviderEndpoint
    : Endpoint<SearchServiceProviderRequest, List<ServiceProviderDto>>
{
    private readonly MerchantService _merchantService;
    private readonly IAppLoggerService<SearchServiceProviderEndpoint> _logger;

    public SearchServiceProviderEndpoint(
        MerchantService merchantService,
        IAppLoggerService<SearchServiceProviderEndpoint> logger
    )
    {
        _merchantService = merchantService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("/serviceProviders/search");
        AllowAnonymous();
        Description(d =>
            d.Produces<List<ServiceProviderDto>>(200, "application/json").Produces(400)
        );
        Tags("ServiceProviders");
        Summary(s =>
        {
            s.Summary = "Search serviceProviders";
            s.Description = "Search serviceProviders by various fields";
            s.ExampleRequest = new SearchServiceProviderRequest
            {
                SearchTerm = "example",
            };
        });
    }

    public override async Task HandleAsync(
         SearchServiceProviderRequest req,
        CancellationToken ct
    )
    {
        var result = await _merchantService.SearchServiceProvidersAsync(
            req.SearchTerm
        );
        List<ValidationFailure> validationFailures =
            new AutoConstructedList<ValidationFailure>();
        await result.Match(
            Right: async serviceProviders => await SendOkAsync(serviceProviders, ct),
            Left: async error => await SendErrorsAsync(400, ct)
        );
    }
}

public class FilterServiceProviderEndpoint
    : Endpoint<FilteredServiceProviderRequest, ListServiceProviderResponse>
{
    private readonly MerchantService _merchantService;
    private readonly IAppLoggerService<FilterServiceProviderEndpoint> _logger;

    public FilterServiceProviderEndpoint(
        MerchantService merchantService,
        IAppLoggerService<FilterServiceProviderEndpoint> logger
    )
    {
        _merchantService = merchantService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/serviceProviders/filtered_list");
        AllowAnonymous();
        Description(d =>
            d.Produces<ListServiceProviderResponse>(200, "application/json")
                .Produces(400)
        );
        Tags("ServiceProviders");
        Summary(s =>
        {
            s.Summary = "Filter serviceProviders";
            s.Description = "Get a filtered and sorted list of serviceProviders";
            s.ExampleRequest = new FilteredServiceProviderRequest
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
        FilteredServiceProviderRequest req,
        CancellationToken ct
    )
    {
        var result = await _merchantService.GetFilteredServiceProvidersAsync(req);
        await result.Match(
            Right: async response => await SendOkAsync(response, ct),
            Left: async error => await SendErrorsAsync(400, ct)
        );
    }
}

public class GetServiceProviderListEndpoint
    : Endpoint<GetServiceProviderListRequest, ListServiceProviderResponse>
{
    private readonly MerchantService _merchantService;
    private readonly IAppLoggerService<GetServiceProviderListEndpoint> _logger;

    public GetServiceProviderListEndpoint(
        MerchantService merchantService,
        IAppLoggerService<GetServiceProviderListEndpoint> logger
    )
    {
        _merchantService = merchantService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/serviceProviders/paged_list");
        AllowAnonymous();
        Description(d =>
            d.Produces<ListServiceProviderResponse>(200, "application/json")
                .Produces(400)
        );
        Tags("ServiceProviders");
        Summary(s =>
        {
            s.Summary = "Get a paged list of ServiceProviders";
            s.Description = "Gets a paginated list of ServiceProviders";
            s.ExampleRequest = new GetServiceProviderListRequest
            {
                PageNumber = 1,
                PageSize = 10,
            };
        });
    }

    public override async Task HandleAsync(
        GetServiceProviderListRequest req,
        CancellationToken ct
    )
    {
        ListServiceProviderResponse? response = await Observable
            .FromAsync(
                () =>
                    ProcessServiceProviderListRequest(req.PageNumber, req.PageSize, ct)
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

    private async Task<ListServiceProviderResponse> ProcessServiceProviderListRequest(
        int pageNumber,
        int pageSize,
        CancellationToken ct
    )
    {
        _logger.LogInformation(
            $"Fetching serviceProvider list. Page: {pageNumber}, Size: {pageSize}"
        );
        Either<string, List<ServiceProviderDto>> result =
            await _merchantService.GetServiceProviderPagedList(pageNumber, pageSize);
        return result.Match(
            serviceProviders => new ListServiceProviderResponse
            {
                ServiceProviders = serviceProviders,
                ErrorMessage = null,
            },
            err => new ListServiceProviderResponse
            {
                ServiceProviders = null,
                ErrorMessage = err,
            }
        );
    }
}
public class UpdateServiceProviderEndpoint
    : Endpoint<UpdateServiceProviderRequest, UpdateServiceProviderResponse>
{
    private readonly MerchantService _merchantService;
    private readonly IAppLoggerService<UpdateServiceProviderEndpoint> _logger;

    public UpdateServiceProviderEndpoint(
        MerchantService merchantService,
        IAppLoggerService<UpdateServiceProviderEndpoint> logger
    )
    {
        _merchantService = merchantService;
        _logger = logger;
    }

    public override void Configure()
    {
        Put("/serviceProviders");
        AllowAnonymous();
        Description(d =>
            d.Produces<UpdateServiceProviderResponse>(200, "application/json")
                .Produces(404)
                .Produces(400)
        );
        Tags("ServiceProviders");
        Summary(s =>
        {
            s.Summary = "Update an existing ServiceProvider";
            s.Description = "Updates an existing ServiceProvider's details";
            s.ExampleRequest = new UpdateServiceProviderRequest
            {
                ServiceProvider = new ServiceProviderDto
                {
                    ServiceProviderId = Guid.NewGuid(),
                    TenantId = Guid.Empty,
                    MerchantId = Guid.Empty,
                    ProviderName = string.Empty,
                    ServiceType = string.Empty,
                    IsPCICompliant = false,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = Guid.Empty,
                    
                    IsDeleted = false,
                }
            };
        });
    }

    public override async Task HandleAsync(
        UpdateServiceProviderRequest req,
        CancellationToken ct
    )
    {
        var response = await Observable
            .FromAsync(() => ProcessUpdateServiceProviderRequest(req, ct))
            .Take(1)
            .FirstOrDefaultAsync();
        if (response != null)
        {
            if (response.ServiceProvider != null)
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

    private async Task<UpdateServiceProviderResponse> ProcessUpdateServiceProviderRequest(UpdateServiceProviderRequest req, CancellationToken ct)
    {
        _logger.LogInformation($"Updating serviceProvider with ID: {req.ServiceProvider.ServiceProviderId}");

        Either<string, ServiceProviderDto> result = await _merchantService.UpdateServiceProviderAsync(req.ServiceProvider.ServiceProviderId, req.ServiceProvider);

        return result.Match(
            Right: dto => new UpdateServiceProviderResponse
            {
                ServiceProvider = dto,
                ErrorMessage = null,
            },
            Left: err => new UpdateServiceProviderResponse
            {
                ServiceProvider = null,
                ErrorMessage = err,
            }
        );
    }
}

