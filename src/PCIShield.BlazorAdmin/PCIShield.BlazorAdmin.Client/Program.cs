using System.Globalization;
using System.Net.Http.Json;
using System.Reactive;
using System.Reactive.Linq;

using ApexCharts;

using Blazored.LocalStorage;

using FluentValidation;

using Magic.IndexedDb.Extensions;
using Magic.IndexedDb.Helpers;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;

using MudBlazor;
using MudBlazor.Services;

using PCIShield.BlazorAdmin.Client.Auth;
using PCIShield.BlazorAdmin.Client.Services;
using PCIShield.BlazorAdmin.Client.Shared.State;
using PCIShield.BlazorAdmin.Client.SignalR;
using PCIShield.BlazorMauiShared.Models.Merchant;
using PCIShield.Client.Services.Assessment;
using PCIShield.Client.Services.Auth;
using PCIShield.Client.Services.Common;
using PCIShield.Client.Services.Common.Cache;

using PCIShield.Client.Services.DashBoard;
using PCIShield.Client.Services.InvoiceSession;
using PCIShield.Client.Services.Merchant;
using PCIShield.Client.Services.PaymentChannel;
using PCIShield.Client.Services.Agui;

namespace PCIShield.BlazorAdmin.Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var builder = WebAssemblyHostBuilder.CreateDefault(args);
                builder.Services.AddApexCharts(e =>
                {
                    e.GlobalOptions = new ApexChartBaseOptions
                    {
                        Debug = true,
                        Theme = new Theme { Palette = PaletteType.Palette6 }
                    };
                });
                builder.Logging.ClearProviders();
                builder.Logging.SetMinimumLevel(LogLevel.Trace);
                builder.Logging.AddProvider(new SimpleConsoleLoggerProvider());
                builder.Services.AddSmartSignalR(options =>
                {
                    options.HubUrl = "/genericEventHub";
                    options.MaxRetryAttempts = 3;
                });
                var descriptor = builder.Services.FirstOrDefault(d => d.ServiceType == typeof(ISignalRNotificationStrategy));
                if (descriptor != null)
                {
                }
                builder.Services.AddScoped(sp =>
                {
                    var baseAddress = new Uri(builder.HostEnvironment.BaseAddress);
                    var client = new HttpClient { BaseAddress = baseAddress };
                    return client;
                });
                builder.Services.AddBlazorDB(options =>
                {
                    options.Name = "BlazorCache";
                    options.Version = "1";
                    options.EncryptionKey = "{secure_key_76765}";
                    options.StoreSchemas = SchemaHelper.GetAllSchemas("BlazorCache");
                });
                builder.Services.AddLocalization();

                builder.Services.AddMemoryCache();
                builder.Services.AddScoped<IClientLocalizationService, ClientLocalizationService>();
                builder.Services.AddScoped<IHttpAppLocalizationClientService, HttpAppLocalizationClientService>();
                builder.Services.AddScoped<MudLocalizer, MudLocalizerService>();
                builder.Services.AddScoped<MudLocalizer, SpanishMudLocalizer>();
                builder.Services.AddScoped<IDirtyStateService, ResilientDirtyStateService>();
                builder.Services.AddScoped<MagicCacheService>();
                builder.Services.AddScoped<IErrorBoundaryLogger>(sp =>
                {
                    var logger = sp.GetRequiredService<ILogger<Program>>();
                    return new ErrorBoundaryLogger(logger);
                });

                builder.Services.AddMudServices(config =>
                {
                    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
                    config.SnackbarConfiguration.PreventDuplicates = false;
                    config.SnackbarConfiguration.NewestOnTop = false;
                    config.SnackbarConfiguration.ShowCloseIcon = true;
                    config.SnackbarConfiguration.VisibleStateDuration = 3000;
                    config.SnackbarConfiguration.HideTransitionDuration = 500;
                    config.SnackbarConfiguration.ShowTransitionDuration = 500;
                });
                builder.Services.AddSingleton<MerchantState>();
                builder.Services.AddBlazoredLocalStorage();
                builder.Services.AddScoped<ITokenService, TokenService>();
                builder.Services.AddScoped<HostHttpClient>(serviceProvider =>
                {
                    var httpClient = serviceProvider.GetRequiredService<HttpClient>();
                    var logger = serviceProvider.GetRequiredService<ILogger<HostHttpClient>>();
                    var navigationManager = serviceProvider.GetRequiredService<NavigationManager>();
                    return new HostHttpClient(httpClient, logger, navigationManager);
                });
                builder.Services.AddScoped(typeof(IAppLoggerService<>), typeof(AppLoggerService<>));
                builder.Services.AddScoped<AuthTokenDelegatingHandler>();
                builder.Services.AddScoped<IHttpAuthClientService, HttpAuthClientService>();
                builder.Services.AddScoped<WasmAuthStateProvider>(serviceProvider =>
                {
                    var tokenService = serviceProvider.GetRequiredService<ITokenService>();
                    var hostHttpClient = serviceProvider.GetRequiredService<HostHttpClient>();
                    var wasmLogger = serviceProvider.GetRequiredService<ILogger<WasmAuthStateProvider>>();

                    return new WasmAuthStateProvider(tokenService, hostHttpClient, wasmLogger);
                });

                builder.Services.AddScoped<AuthenticationStateProvider>(serviceProvider =>
                    serviceProvider.GetRequiredService<WasmAuthStateProvider>());

                builder.Services.AddAuthorizationCore();

                builder.Services.AddScoped<IHttpAssessmentClientService, HttpAssessmentClientService>();
                builder.Services.AddScoped<IHttpPaymentChannelClientService, HttpPaymentChannelClientService>();

                builder.Services.AddScoped<IHttpMerchantClientService, HttpMerchantClientService>();
                builder.Services.AddScoped<IHttpAguiClientService, HttpAguiClientService>();
                builder.Services.AddScoped<IHttpDashBoardClientService, HttpDashBoardClientService>();
                builder.Services.AddValidatorsFromAssemblyContaining<UpdateMerchantValidator>();
                var host = builder.Build();

                using (var scop1 = host.Services.CreateScope())
                {
                    var signalR = scop1.ServiceProvider.GetService<ISignalRNotificationStrategy>();
                }
                AppDomain.CurrentDomain.UnhandledException += (sender, error) =>
                {
                    var logger = host.Services.GetService<ILogger<Program>>();
                    logger?.LogError(error.ExceptionObject as Exception, "Unhandled application error");
                };
                await using var scope = host.Services.CreateAsyncScope();
                var errorBoundaryLogger = scope.ServiceProvider.GetRequiredService<IErrorBoundaryLogger>();

                Observable
                    .StartAsync(async () =>
                    {
                        await SetInitialCultureAsync(host);
                    })
                    .Timeout(TimeSpan.FromSeconds(1))
                    .Catch<Unit, TimeoutException>(ex =>
                    {
                        return Observable.Return(Unit.Default);
                    })
                    .Subscribe(onNext: _ => { });
                const bool CLEAR_AUTH_ON_STARTUP = false;
                if (CLEAR_AUTH_ON_STARTUP)
                {
                    await using (var clearScope = host.Services.CreateAsyncScope())
                    {
                        try
                        {
                            var js = clearScope.ServiceProvider.GetRequiredService<IJSRuntime>();
                            await js.InvokeVoidAsync("localStorage.clear");
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
                await using (var startupScope = host.Services.CreateAsyncScope())
                {
                    try
                    {
                        var authStateProvider = startupScope.ServiceProvider.GetRequiredService<AuthenticationStateProvider>();

                        if (authStateProvider is WasmAuthStateProvider wasmProvider)
                        {
                            var authState = await wasmProvider.GetAuthenticationStateAsync();
                            var tokenService = startupScope.ServiceProvider.GetRequiredService<ITokenService>();
                            var token = await tokenService.GetTokenAsync();
                        }
                        else
                        {
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }

                await host.RunAsync();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Application startup failed: {ex}");
                throw;
            }
        }

        private static async Task SetInitialCultureAsync(WebAssemblyHost host)
        {
            var js = host.Services.GetRequiredService<IJSRuntime>();
            var savedCulture = await js.InvokeAsync<string>("localStorage.getItem", "preferredLanguage");
            Console.WriteLine($"Found culture in localStorage: {savedCulture}");

            CultureInfo culture;
            if (!string.IsNullOrEmpty(savedCulture))
            {
                culture = new CultureInfo(savedCulture);
            }
            else
            {
                culture = new CultureInfo("en-US");
            }
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            Console.WriteLine($"Culture initialized to: {CultureInfo.CurrentUICulture.Name}");
        }
    }

    class GetUserPreferredCultureResponse
    {
        public string? Culture { get; set; }
        public string? ErrorMessage { get; set; }
    }
    public class SimpleConsoleLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
        {
            return new SimpleConsoleLogger(categoryName);
        }

        public void Dispose() { }
    }

    public class SimpleConsoleLogger : ILogger
    {
        private readonly string _categoryName;

        public SimpleConsoleLogger(string categoryName)
        {
            _categoryName = categoryName;
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            var message = formatter(state, exception);
            var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            var logLevelString = logLevel.ToString().ToUpper().PadRight(5);

            var formattedMessage = $"[{timestamp}] [{logLevelString}] {_categoryName}: {message}";

            switch (logLevel)
            {
                case LogLevel.Error:
                case LogLevel.Critical:
                    Console.Error.WriteLine(formattedMessage);
                    if (exception != null)
                    {
                        Console.Error.WriteLine($"Exception: {exception}");
                        Console.Error.WriteLine($"StackTrace: {exception.StackTrace}");
                    }
                    break;
                case LogLevel.Warning:
                    break;
                case LogLevel.Information:
                    break;
                default:
                    break;
            }
        }
    }
}
