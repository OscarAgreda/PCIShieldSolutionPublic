using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
namespace PCIShield.Api;
public class DuplicateRouteCheckerMiddleware : IMiddleware
{
    private static readonly ConcurrentDictionary<string, string> _routes = new();
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        IEndpoint endpoint = context.GetEndpoint() as IEndpoint;
        if (endpoint != null)
        {
            var routePattern = context.Request.Path.Value?.ToLowerInvariant();
            var httpMethod = context.Request.Method;
            var key = $"{httpMethod}:{routePattern}";
            if (!_routes.TryAdd(key, endpoint.GetType().FullName))
            {
                throw new InvalidOperationException(
                    $"Duplicate route detected! Route: {key}, Endpoint: {endpoint.GetType().FullName}, Existing: {_routes[key]}");
            }
        }
        await next(context);
    }
}