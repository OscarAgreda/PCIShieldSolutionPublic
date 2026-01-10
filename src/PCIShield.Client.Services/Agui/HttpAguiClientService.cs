using System.Net.Http.Headers;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Text.Json;
using System.Web;

using Ardalis.GuardClauses;

using Flurl.Http;
using Flurl.Http.Configuration;

using Microsoft.Maui.Devices;

using PCIShield.BlazorMauiShared.Models;
using PCIShield.BlazorMauiShared.Models.Agui;
using PCIShield.Client.Services.Auth;
using PCIShield.Client.Services.Common;
using PCIShield.Client.Services.InvoiceSession;

using ReactiveUI;

using Unit = System.Reactive.Unit;

namespace PCIShield.Client.Services.Agui;

public class HttpAguiClientService : IDisposable, IHttpAguiClientService
{
    public string EndpointUrl => "agui/merchant-copilot"; // Or retrieve from base address
    private readonly CompositeDisposable _disposables = new();
    private readonly IAppLoggerService<HttpAguiClientService> _logger;
    private readonly ITokenService _tokenService;
    private HttpClient _httpClient = null!;
    private FlurlClient _flurlClient = null!;

    public HttpAguiClientService(
        IAppLoggerService<HttpAguiClientService> logger,
        ITokenService tokenService)
    {
        _logger = logger;
        _tokenService = tokenService;
        LoadInitializeHttp();
    }

    public void Dispose()
    {
        _disposables.Dispose();
        // HttpClient and FlurlClient are usually managed by DI or singleton in some patterns, 
        // but here they are created in InitializeHttp, so we should dispose them if we own them.
        _httpClient?.Dispose();
        _flurlClient?.Dispose();
    }

    public async IAsyncEnumerable<AguiMessageEvent> SendMessageAsync(
        string message,
        IReadOnlyList<AguiChatMessage>? conversationHistory = null,
        Dictionary<string, object?>? sharedState = null,
        [System.Runtime.CompilerServices.EnumeratorCancellation] System.Threading.CancellationToken ct = default)
    {
        // Ensure initialization is complete or wait for it? 
        // The Rx pattern loads it async. Ideally we check if _httpClient is ready.
        if (_httpClient == null)
        {
            await InitializeHttp();
            if (_httpClient == null) throw new InvalidOperationException("Failed to initialize HttpClient");
        }

        var uri = "agui/merchant-copilot";
        var apiBaseUrl = await RetrieveApiBaseUrl();
        var fullUrl = $"{apiBaseUrl}{uri}";

        var request = new HttpRequestMessage(HttpMethod.Post, fullUrl);
        var token = await _tokenService.GetTokenAsync();

        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        var payload = new
        {
            message,
            history = conversationHistory,
            sharedState
        };

        var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var jsonContent = JsonSerializer.Serialize(payload, jsonOptions);
        request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        // Use ResponseHeadersRead for streaming
        using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError($"AG-UI Error: {response.StatusCode} - {errorContent}");
            throw new Exception($"AG-UI request failed: {response.StatusCode}");
        }

        using var stream = await response.Content.ReadAsStreamAsync();
        using var reader = new StreamReader(stream);

        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(line)) continue;

            if (line.StartsWith("data: "))
            {
                var data = line["data: ".Length..];
                if (data == "[DONE]") break;

                AguiMessageEvent? evt = null;
                try
                {
                    evt = JsonSerializer.Deserialize<AguiMessageEvent>(data, jsonOptions);
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, "Failed to parse AG-UI event");
                }

                if (evt != null)
                {
                    yield return evt;
                }
            }
        }
    }

    private async Task InitializeHttp()
    {
        string apiBaseUrl = await RetrieveApiBaseUrl();

        // This pattern relies on GiveMeMyHttpHandler existing in Common namespace
        GiveMeMyHttpHandler myHttpHandler = new(apiBaseUrl);
        SocketsHttpHandler handler = myHttpHandler.CreateMessageHandler();

        _httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(apiBaseUrl),
            Timeout = TimeSpan.FromSeconds(230), // Extended timeout for AI streaming
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
                // ConfigureFlurlHttp(); // If there is extra config needed
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
        // Point to API project (where AG-UI agents are hosted in Startup.cs - commit b80f38e)
        string thisBaseUrl = "https://localhost:52509/"; // PCIShield.Api port
        if (DeviceInfo.Platform == DevicePlatform.Android)
        {
            thisBaseUrl = thisBaseUrl.Replace("localhost", "10.0.2.2");
        }
        return await Task.FromResult(thisBaseUrl);
    }
}
