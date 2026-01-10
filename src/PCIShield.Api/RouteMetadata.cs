using System.Collections.Generic;
namespace PCIShield.Api;
public record RouteMetadata
{
    public string Route { get; init; }
    public string[] HttpMethods { get; init; }
    public string EndpointType { get; init; }
    public string[] ValidationRules { get; init; }
    public string[] SecuritySchemes { get; init; }
    public List<string>? RequiresAuthorization { get; init; }
    public string[] Tags { get; init; }
}