using Microsoft.Extensions.DependencyInjection;
using Blazored.LocalStorage;
using PCIShield.Client.Services.Common;
using PCIShield.Client.Services.InvoiceSession;
using PCIShield.Client.Services.Merchant;

namespace PCIShield.PerformanceTests.Infrastructure;

public static class ClientFactory
{
    private static readonly IServiceProvider _serviceProvider;
    private static readonly object _lock = new();

    static ClientFactory()
    {
        var services = new ServiceCollection();

        // Register core services
        services.AddSingleton(typeof(IAppLoggerService<>), typeof(StubLogger<>));
        services.AddSingleton<ILocalStorageService, InMemoryLocalStorageService>();
        services.AddSingleton<ITokenService, TokenService>();

        // Register HTTP clients with proper lifecycle
        services.AddTransient<IHttpMerchantClientService, HttpMerchantClientService>();

        // Add HTTP client factory
        services.AddHttpClient();

        _serviceProvider = services.BuildServiceProvider();
    }

    public static IHttpMerchantClientService CreateMerchantClient()
    {
        lock (_lock)
        {
            return _serviceProvider.GetRequiredService<IHttpMerchantClientService>();
        }
    }
}
