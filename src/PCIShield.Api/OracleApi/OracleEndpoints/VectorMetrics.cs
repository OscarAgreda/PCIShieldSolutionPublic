using System.Threading;
using System.Threading.Tasks;

using FastEndpoints;

using Microsoft.AspNetCore.Http;

using PCIShield.Infrastructure.Services.OracleAi;

namespace PCIShield.Api.OracleApi.OracleEndpoints
{
    public sealed class GetVectorMetricsEndpoint
        : Endpoint<VectorMetricsQuery, VectorMetricsSeriesResponse>
    {
        private readonly IVectorMetricsService _svc;

        public GetVectorMetricsEndpoint(IVectorMetricsService svc)
        {
            _svc = svc;
        }

        public override void Configure()
        {
            Get("/api/vectors/metrics/series");
            AllowAnonymous();
            Summary(s =>
            {
                s.Summary = "Returns time-series metrics for vector health/quality/latency.";
                s.Description = "Prefers snapshot table when present; falls back to live computation using Oracle 23ai.";
                s.ExampleRequest = new VectorMetricsQuery { Entity = "PRODUCTS", Days = 14 };
                s.Response<VectorMetricsSeriesResponse>(200, "OK");
            });
        }

        public override async Task HandleAsync(VectorMetricsQuery req, CancellationToken ct)
        {
            var result = await _svc.GetSeriesAsync(req, ct);
            if (result.Success)
                await SendOkAsync(result, ct);
            else
                await SendAsync(result, StatusCodes.Status500InternalServerError, ct);
        }
    }
}
