using System.Net.Http.Headers;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text.Json;
using System.Text;
using System.Web;
using Ardalis.GuardClauses;

using PCIShield.BlazorMauiShared.Models;

using PCIShield.Client.Services.Common;
using Flurl.Http;
using Flurl.Http.Configuration;
using Microsoft.Maui.Devices;
using ReactiveUI;
using JsonSerializer = System.Text.Json.JsonSerializer;
using JsonSerializerOptions = System.Text.Json.JsonSerializerOptions;
using Unit = System.Reactive.Unit;

using PCIShield.BlazorMauiShared.ModelsDto.Responses.DashboardAggregate;
namespace PCIShield.Client.Services.DashBoard;
public class HttpDashBoardClientService : IDisposable, IHttpDashBoardClientService
{
    private readonly CompositeDisposable _disposables = new();
    private readonly IAppLoggerService<HttpDashBoardClientService> _logger;

    private FlurlClient _flurlClient;
    private HttpClient _httpClient;
    public HttpDashBoardClientService(
        IAppLoggerService<HttpDashBoardClientService> logger
       
    )
    {
        _logger = logger;
   
        LoadInitializeHttp();
    }

    private void HandleHttpServiceError(Exception ex, ErrorHandlingService.ClientOperationContext context)
    {
        ErrorHandlingService.GetContextualErrorMessage(ex, context);
    }

    private string BuildFlurlErrorMessage(string operation, FlurlHttpException ex)
    {
        FlurlCall? call = ex.Call;
        string? errorMsg =
            $"Error while {operation}. HTTP Status: {call.Response?.StatusCode}, Request: {call.ToString()}, Response: {call.Response}";
        return errorMsg;
    }
    private void ConfigureFlurlHttp()
    {
        FlurlHttp.Clients.WithDefaults(builder =>
        {
            builder
                .WithSettings(settings =>
                {
                    settings.Timeout = TimeSpan.FromSeconds(230);
                    settings.HttpVersion = "2.0";
                    settings.AllowedHttpStatusRange = "*";
                    settings.Redirects.Enabled = true;
                    settings.Redirects.AllowSecureToInsecure = false;
                    settings.Redirects.MaxAutoRedirects = 10;
                    settings.JsonSerializer = new DefaultJsonSerializer(
                        GJset.GetSystemTextJsonSettings()
                    );
                })
                .ConfigureInnerHandler(httpClientHandler =>
                {
                    httpClientHandler.ServerCertificateCustomValidationCallback = (
                        message,
                        cert,
                        chain,
                        errors
                    ) => true;
                    httpClientHandler.SslProtocols = System
                        .Security
                        .Authentication
                        .SslProtocols
                        .Tls12;
                    httpClientHandler.UseProxy = false;
                    httpClientHandler.Proxy = null;
                })
                .ConfigureHttpClient(httpClient =>
                {
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json")
                    );
                    httpClient.Timeout = TimeSpan.FromSeconds(230);
                    httpClient.DefaultRequestVersion = new Version(2, 0);
                    httpClient.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrHigher;
                })
                .BeforeCall(call =>
                {
                    foreach ((string Name, string Value) header in call.Request.Headers)
                    {
                        _logger.LogInformation(
                            $"Header: {header.Name}: {string.Join(", ", header.Value)}"
                        );
                    }
                })
                .AfterCall(call =>
                {
                    _logger.LogInformation($"Response: {call.Response?.StatusCode}");
                    if (call.Response != null)
                    {
                        foreach ((string Name, string Value) header in call.Response.Headers)
                        {
                            _logger.LogInformation(
                                $"Response Header: {header.Name}: {string.Join(", ", header.Value)}"
                            );
                        }
                    }
                })
                .OnError(call =>
                {
                    _logger.LogError($"Error: {call.Exception?.Message}");
                    if (call.Exception is FlurlHttpException flurlEx)
                    {
                        _logger.LogError(
                            $"Response status code: {flurlEx.Call.Response?.StatusCode}"
                        );
                        _logger.LogError(
                            $"Response content: {flurlEx.GetResponseStringAsync().Result}"
                        );
                    }
                });
        });
    }
    public void Dispose()
    { }
    private async Task InitializeHttp()
    {
        string? apiBaseUrl = await RetrieveApiBaseUrl();
        GiveMeMyHttpHandler myHttpHandler = new(apiBaseUrl);
        SocketsHttpHandler handler = myHttpHandler.CreateMessageHandler();
        _httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(apiBaseUrl),
            Timeout = TimeSpan.FromSeconds(230),
            DefaultRequestVersion = new Version(2, 0),
            DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrHigher,
        };
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json")
        );
        _flurlClient = new FlurlClient(_httpClient);
        _flurlClient.Settings.HttpVersion = "2.0";
    }
    private void LoadInitializeHttp()
    {
        Observable
            .FromAsync(async () =>
            {
                await InitializeHttp();
                ConfigureFlurlHttp();
            })
            .Catch(
                (Exception ex) =>
                {
                    _logger.LogError($"Exception in InitializeHttp: {ex.Message}");
                    return Observable.Return(Unit.Default);
                }
            )
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(
                _ => { },
                ex => _logger.LogError($"Failed after retrying: {ex.Message}"),
                () => { }
            )
            .DisposeWith(_disposables);
    }
    private async Task<string> RetrieveApiBaseUrl()
    {
        string thisBaseUrl = "https://localhost:52509/";
        if (DeviceInfo.Platform == DevicePlatform.Android)
        {
            thisBaseUrl = thisBaseUrl.Replace("localhost", "10.0.2.2");
        }
        return thisBaseUrl;
    }
}
