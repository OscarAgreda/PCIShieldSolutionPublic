using System.Reflection;
namespace PCIShield.BlazorAdmin.Client.Services
{
    public class LazyLoadingService
    {
        private readonly Dictionary<string, Assembly> _loadedAssemblies = new();
        private readonly ILogger<LazyLoadingService> _logger;
    }
}
