using Microsoft.AspNetCore.Components.Web;
namespace PCIShield.BlazorAdmin.Client;
public class ErrorBoundaryLogger : IErrorBoundaryLogger
{
    private readonly ILogger _logger;
    public ErrorBoundaryLogger(ILogger logger)
    {
        _logger = logger;
    }
    public ValueTask LogErrorAsync(Exception exception)
    {
        _logger.LogError(exception, "Error boundary caught an error");
        return ValueTask.CompletedTask;
    }
}