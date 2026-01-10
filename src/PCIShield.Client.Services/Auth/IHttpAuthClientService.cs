// PCIShield.Client.Services/Auth/HttpAuthClientService.cs

using System;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

using Flurl;
using Flurl.Http;
using Flurl.Http.Configuration;

using LanguageExt;

using Microsoft.Extensions.Logging;
using Microsoft.Maui.Devices;

using PCIShield.BlazorMauiShared.Auth;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.BlazorMauiShared.Models.Auth;
using PCIShield.Client.Services.Common;
using PCIShield.Client.Services.InvoiceSession;
using PCIShield.Client.Services.Merchant;

using ReactiveUI;

using static LanguageExt.Prelude;

using JsonSerializer = System.Text.Json.JsonSerializer;
using JsonSerializerOptions = System.Text.Json.JsonSerializerOptions;
using Unit = System.Reactive.Unit;
namespace PCIShield.Client.Services.Auth
{
    public partial interface IHttpAuthClientService
    {
        Task<Either<string, LoginResponse>> LoginMerchantAsync(LoginRequest request);
        Task<Either<string, LoginResponse>> LoginComplianceOfficerAsync(LoginRequest request);
        Task<Either<string, RegisterResponse>> RegisterMerchantAsync(RegisterRequest request);
        Task<Either<string, RegisterResponse>> RegisterComplianceOfficerAsync(RegisterRequest request);
        Task<Either<string, LogoutResponse>> LogoutMerchantAsync(LogoutRequest request);
        Task<Either<string, LogoutResponse>> LogoutComplianceOfficerAsync(LogoutRequest request);
        Task<Either<string, ValidateTokenResponse>> ValidateTokenAsync(ValidateTokenRequest request);
        Task<Either<string, RefreshTokenResponse>> RefreshTokenAsync(RefreshTokenRequest request);

        Task<Either<string, LoginResponse>> ProcessGoogleCallbackAsync(string code, string? state = null);
    }

    public partial class HttpAuthClientService : IDisposable, IHttpAuthClientService
    {
        private readonly CompositeDisposable _disposables = new();
        private readonly IAppLoggerService<HttpAuthClientService> _logger;
        private readonly ITokenService _tokenService;
        private FlurlClient _flurlClient;
        private HttpClient _httpClient;
        public HttpAuthClientService( IAppLoggerService<HttpAuthClientService> logger, ITokenService tokenService)
        {
            _logger = logger;
            _tokenService = tokenService;
            LoadInitializeHttp();
        }

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
        public async Task<Either<string, LoginResponse>> ProcessGoogleCallbackAsync(string code, string? state = null)
        {
            try
            {
                string apiBaseUrl = await RetrieveApiBaseUrl();
                var queryParams = $"?code={Uri.EscapeDataString(code)}";
                if (!string.IsNullOrEmpty(state))
                {
                    queryParams += $"&state={Uri.EscapeDataString(state)}";
                }

                string uri = $"api/auth/v2/signin_google{queryParams}";

                _logger.LogInformation("Calling Google callback endpoint: {Uri}", uri);

                var response = await _flurlClient.Request($"{apiBaseUrl}{uri}")
                    .AllowAnyHttpStatus()
                    .GetAsync();

                if (!response.ResponseMessage.IsSuccessStatusCode)
                {
                    string errorContent = await response.ResponseMessage.Content.ReadAsStringAsync();
                    _logger.LogError("Google callback failed with status {StatusCode}. Details: {Error}",
                        response.StatusCode, errorContent);
                    return $"Google authentication failed: {response.StatusCode}";
                }

                var result = await response.GetJsonAsync<LoginResponse>();
                _logger.LogInformation("Google callback processed successfully");

                return result;
            }
            catch (FlurlHttpException ex)
            {
                _logger.LogError(ex, "HTTP error during Google callback processing");
                return $"Google authentication error: {ex.Message}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during Google callback processing");
                return $"Unexpected error: {ex.Message}";
            }
        }

        public async Task<Either<string, LoginResponse>> LoginMerchantAsync(LoginRequest request)
        {
            try
            {
                string apiBaseUrl = await RetrieveApiBaseUrl();
                string uri = "api/auth/v2/login";

                var response = await _flurlClient
                    .Request($"{apiBaseUrl}{uri}")
                    .AllowAnyHttpStatus()
                    .PostJsonAsync(request);

                if (!response.ResponseMessage.IsSuccessStatusCode)
                {
                    string errorContent = await response.ResponseMessage.Content.ReadAsStringAsync();
                    _logger.LogError($"Login merchant failed with status {response.StatusCode}. Details: {errorContent}");
                     ProblemDetails problemDetails = null;
                    if (response.ResponseMessage.Content.Headers.ContentType?.MediaType == "application/problem+json")
                    {
                        try
                        {
                            problemDetails = await response.GetJsonAsync<ProblemDetails>();
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to deserialize problem details from 400 response.");
                        }
                    }

                    string detailedErrorMessage = $"Authentication failed. Status: {response.StatusCode}.";
                    if (problemDetails != null && problemDetails.Errors.Any())
                    {
                        detailedErrorMessage += " Validation Errors: " +
                                                string.Join("; ", problemDetails.Errors.Select(e => $"{e.Key}: {string.Join(", ", e.Value)}"));
                    }
                    else if (!string.IsNullOrWhiteSpace(errorContent))
                    {
                        detailedErrorMessage += $" Raw content: {errorContent}";
                    }
                    return detailedErrorMessage;
                }

                var result = await response.GetJsonAsync<LoginResponse>();
                return result;
            }
            catch (FlurlHttpException ex)
            {
                _logger.LogError(ex, "Error during merchant login");
                return $"Authentication error: {ex.Message}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during merchant login");
                return $"Unexpected error: {ex.Message}";
            }
        }

        public async Task<Either<string, LoginResponse>> LoginComplianceOfficerAsync(LoginRequest request)
        {
            try
            {
                string apiBaseUrl = await RetrieveApiBaseUrl();
                string uri = "api/accountv2/login_complianceOfficer";

                var response = await _flurlClient
                    .Request($"{apiBaseUrl}{uri}")
                    .AllowAnyHttpStatus()
                    .PostJsonAsync(request);

                if (!response.ResponseMessage.IsSuccessStatusCode)
                {
                    string errorContent = await response.ResponseMessage.Content.ReadAsStringAsync();
                    _logger.LogError($"Login complianceOfficer failed with status {response.StatusCode}. Details: {errorContent}");
                    return "Authentication failed";
                }

                var result = await response.GetJsonAsync<LoginResponse>();
                return result;
            }
            catch (FlurlHttpException ex)
            {
                _logger.LogError(ex, "Error during complianceOfficer login");
                return $"Authentication error: {ex.Message}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during complianceOfficer login");
                return $"Unexpected error: {ex.Message}";
            }
        }

        public async Task<Either<string, RegisterResponse>> RegisterMerchantAsync(RegisterRequest request)
        {
            try
            {
                string apiBaseUrl = await RetrieveApiBaseUrl();
                string uri = "api/accountv2/register_merchant";

                var response = await _flurlClient
                    .Request($"{apiBaseUrl}{uri}")
                    .AllowAnyHttpStatus()
                    .PostJsonAsync(request);

                if (!response.ResponseMessage.IsSuccessStatusCode)
                {
                    string errorContent = await response.ResponseMessage.Content.ReadAsStringAsync();
                    _logger.LogError($"Register merchant failed with status {response.StatusCode}. Details: {errorContent}");
                    return "Registration failed";
                }

                var result = await response.GetJsonAsync<RegisterResponse>();
                return result;
            }
            catch (FlurlHttpException ex)
            {
                _logger.LogError(ex, "Error during merchant registration");
                return $"Registration error: {ex.Message}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during merchant registration");
                return $"Unexpected error: {ex.Message}";
            }
        }

        public async Task<Either<string, RegisterResponse>> RegisterComplianceOfficerAsync(RegisterRequest request)
        {
            try
            {
                string apiBaseUrl = await RetrieveApiBaseUrl();
                string uri = "api/accountv2/register_complianceOfficer";

                var response = await _flurlClient
                    .Request($"{apiBaseUrl}{uri}")
                    .AllowAnyHttpStatus()
                    .PostJsonAsync(request);

                if (!response.ResponseMessage.IsSuccessStatusCode)
                {
                    string errorContent = await response.ResponseMessage.Content.ReadAsStringAsync();
                    _logger.LogError($"Register complianceOfficer failed with status {response.StatusCode}. Details: {errorContent}");
                    return "Registration failed";
                }

                var result = await response.GetJsonAsync<RegisterResponse>();
                return result;
            }
            catch (FlurlHttpException ex)
            {
                _logger.LogError(ex, "Error during complianceOfficer registration");
                return $"Registration error: {ex.Message}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during complianceOfficer registration");
                return $"Unexpected error: {ex.Message}";
            }
        }

        public async Task<Either<string, LogoutResponse>> LogoutMerchantAsync(LogoutRequest request)
        {
            try
            {
                string apiBaseUrl = await RetrieveApiBaseUrl();
                string uri = "api/accountv2/logout_merchant";

                var response = await _flurlClient
                    .Request($"{apiBaseUrl}{uri}")
                    .AllowAnyHttpStatus()
                    .PostJsonAsync(request);

                if (!response.ResponseMessage.IsSuccessStatusCode)
                {
                    string errorContent = await response.ResponseMessage.Content.ReadAsStringAsync();
                    _logger.LogError($"Logout merchant failed with status {response.StatusCode}. Details: {errorContent}");
                    return "Logout failed";
                }

                var result = await response.GetJsonAsync<LogoutResponse>();
                return result;
            }
            catch (FlurlHttpException ex)
            {
                _logger.LogError(ex, "Error during merchant logout");
                return $"Logout error: {ex.Message}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during merchant logout");
                return $"Unexpected error: {ex.Message}";
            }
        }

        public async Task<Either<string, LogoutResponse>> LogoutComplianceOfficerAsync(LogoutRequest request)
        {
            try
            {
                string apiBaseUrl = await RetrieveApiBaseUrl();
                string uri = "api/accountv2/logout_complianceOfficer";

                var response = await _flurlClient
                    .Request($"{apiBaseUrl}{uri}")
                    .AllowAnyHttpStatus()
                    .PostJsonAsync(request);

                if (!response.ResponseMessage.IsSuccessStatusCode)
                {
                    string errorContent = await response.ResponseMessage.Content.ReadAsStringAsync();
                    _logger.LogError($"Logout complianceOfficer failed with status {response.StatusCode}. Details: {errorContent}");
                    return "Logout failed";
                }

                var result = await response.GetJsonAsync<LogoutResponse>();
                return result;
            }
            catch (FlurlHttpException ex)
            {
                _logger.LogError(ex, "Error during complianceOfficer logout");
                return $"Logout error: {ex.Message}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during complianceOfficer logout");
                return $"Unexpected error: {ex.Message}";
            }
        }

        public async Task<Either<string, ValidateTokenResponse>> ValidateTokenAsync(ValidateTokenRequest request)
        {
            try
            {
                string apiBaseUrl = await RetrieveApiBaseUrl();
                string uri = "api/accountv2/validate_merchant_token";

                var response = await _flurlClient
                    .Request($"{apiBaseUrl}{uri}")
                    .AllowAnyHttpStatus()
                    .PostJsonAsync(request);

                if (!response.ResponseMessage.IsSuccessStatusCode)
                {
                    string errorContent = await response.ResponseMessage.Content.ReadAsStringAsync();
                    _logger.LogError($"Token validation failed with status {response.StatusCode}. Details: {errorContent}");
                    return "Token validation failed";
                }

                var result = await response.GetJsonAsync<ValidateTokenResponse>();
                return result;
            }
            catch (FlurlHttpException ex)
            {
                _logger.LogError(ex, "Error during token validation");
                return $"Token validation error: {ex.Message}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during token validation");
                return $"Unexpected error: {ex.Message}";
            }
        }

        public async Task<Either<string, RefreshTokenResponse>> RefreshTokenAsync(RefreshTokenRequest request)
        {
            try
            {
                string apiBaseUrl = await RetrieveApiBaseUrl();
                string uri = "api/auth/v2/refresh_token_v2";

                var response = await _flurlClient
                    .Request($"{apiBaseUrl}{uri}")
                    .AllowAnyHttpStatus()
                    .PostJsonAsync(request);

                if (!response.ResponseMessage.IsSuccessStatusCode)
                {
                    string errorContent = await response.ResponseMessage.Content.ReadAsStringAsync();
                    _logger.LogError($"Token refresh failed with status {response.StatusCode}. Details: {errorContent}");
                    return "Token refresh failed";
                }

                var result = await response.GetJsonAsync<RefreshTokenResponse>();
                return result;
            }
            catch (FlurlHttpException ex)
            {
                _logger.LogError(ex, "Error during token refresh");
                return $"Token refresh error: {ex.Message}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during token refresh");
                return $"Unexpected error: {ex.Message}";
            }
        }

    }

    public class ProblemDetails
    {
        public string Type { get; set; }
        public string Title { get; set; }
        public int? Status { get; set; }
        public string Detail { get; set; }
        public string Instance { get; set; }
        public Dictionary<string, string[]> Errors { get; set; } = new Dictionary<string, string[]>();
    }
}