using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using FastEndpoints;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
namespace PCIShield.Api;
public class EnhancedRouteAnalyzer
{
    private readonly Dictionary<string, RouteInfo> _routes = new();
    private readonly ILogger<EnhancedRouteAnalyzer> _logger;
    private readonly IWebHostEnvironment _env;
    public EnhancedRouteAnalyzer(ILogger<EnhancedRouteAnalyzer> logger, IWebHostEnvironment env)
    {
        _logger = logger;
        _env = env;
    }
    public class RouteInfo
    {
        public string Route { get; set; }
        public string[] HttpMethods { get; set; }
        public string EndpointType { get; set; }
        public string[] ValidationRules { get; set; }
        public string[] RequiresAuthorization { get; set; }
        public string[] Tags { get; set; }
    }
    public List<(string Key, string ExistingType, string DuplicateType)> AnalyzeEndpoints(IEnumerable<EndpointDefinition> endpoints)
    {
        var duplicates = new List<(string Key, string ExistingType, string DuplicateType)>();
        foreach (var ep in endpoints)
        {
            foreach (var route in ep.Routes)
            {
                var key = $"{string.Join(",", ep.Verbs)}:{route}";
                if (_routes.ContainsKey(key))
                {
                    duplicates.Add((key, _routes[key].EndpointType, ep.EndpointType.FullName));
                    if (_env.IsDevelopment())
                    {
                        _logger.LogWarning($"Duplicate route found: {key}\nExisting: {_routes[key].EndpointType}\nDuplicate: {ep.EndpointType.FullName}");
                    }
                }
                else
                {
                    _routes[key] = new RouteInfo
                    {
                        Route = route,
                        HttpMethods = ep.Verbs.ToArray(),
                        EndpointType = ep.EndpointType.FullName,
                        ValidationRules = ep.ValidatorType?.GetMethods()
                            .Where(m => m.Name.Contains("Valid"))
                            .Select(m => m.Name)
                            .ToArray() ?? Array.Empty<string>(),
                        Tags = ep.EndpointTags?.ToArray() ?? Array.Empty<string>()
                    };
                }
            }
        }
        return duplicates;
    }
    public void WriteAnalysisToFile(string filePath)
    {
        var settings = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        var jsonRoutes = JsonSerializer.Serialize(_routes, settings);
        File.WriteAllText(filePath, jsonRoutes);
    }
}