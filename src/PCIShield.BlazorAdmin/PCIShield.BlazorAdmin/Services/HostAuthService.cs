using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

using Blazored.LocalStorage;

using Flurl;
using Flurl;
using Flurl.Http;
using Flurl.Http.Configuration;

using LanguageExt;
using LanguageExt.Pipes;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Devices;

using PCIShield.BlazorMauiShared.Auth;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.BlazorMauiShared.Models.Auth;
using PCIShield.Client.Services.Auth;
using PCIShield.Client.Services.Common;

using ReactiveUI;

using static LanguageExt.Prelude;

using static LanguageExt.Prelude;

using JsonSerializer = System.Text.Json.JsonSerializer;
using JsonSerializerOptions = System.Text.Json.JsonSerializerOptions;
using Unit = System.Reactive.Unit;

namespace PCIShield.BlazorAdmin.Services
{
    public class HostAuthService : IHttpAuthClientService
    {
        private  HttpClient _httpClient;
        private FlurlClient _flurlClient;
        private readonly CompositeDisposable _disposables = new();
        private readonly IHttpContextAccessor _http;
        private readonly ILogger<HostAuthService> _logger;
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public HostAuthService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, ILogger<HostAuthService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("PCIShieldAPI");
            _logger = logger;
            _http = httpContextAccessor;
            LoadInitializeHttp();

        }
        private void LoadInitializeHttp()
        {
            Observable
                .FromAsync(async () =>
                {
                    await InitializeHttp();
                    ConfigureFlurlHttp();
                })
                .Catch((Exception ex) =>
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
        private async Task<string> RetrieveApiBaseUrl()
        {
            string thisBaseUrl = "https://localhost:52509/";
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                thisBaseUrl = thisBaseUrl.Replace("localhost", "10.0.2.2");
            }
            return thisBaseUrl;
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

        private void LoadFromCookieClaims(out string? authToken, out string? userId, out string? role, out string? userType)
        {
            var user = _http.HttpContext?.User;
            authToken = user?.FindFirst("api_jwt_token")?.Value;            
            userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;  
            role = user?.FindFirst(ClaimTypes.Role)?.Value;
            userType = user?.FindFirst("UserType")?.Value;           
        }
        private string GetAuthTokenRequired()
        {
            LoadFromCookieClaims(out var authToken, out _, out _, out _);
            if (string.IsNullOrWhiteSpace(authToken))
                throw new HttpRequestException("Missing API token in cookie claims. User is not authenticated.");
            return authToken;
        }
        public async Task<Either<string, LoginResponse>> ProcessGoogleCallbackAsync(string code, string? state = null)
        {
            try
            {
                _logger.LogInformation("Processing Google callback with backend API");

                var query = $"?code={Uri.EscapeDataString(code)}";
                if (!string.IsNullOrWhiteSpace(state))
                {
                    query += $"&state={Uri.EscapeDataString(state)}";
                }

                return await Send<LoginResponse>(
                    HttpMethod.Get,
                    $"api/auth/v2/signin_google{query}",
                    body: null,
                    operationDescription: "process Google authentication callback");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Google callback");
                return Left<string, LoginResponse>($"Google authentication error: {ex.Message}");
            }
        }

        public Task<Either<string, LoginResponse>> LoginMerchantAsync(LoginRequest request) =>
            SendAnonymous<LoginResponse>(HttpMethod.Post, "api/auth/v2/login", request, "merchant login");

        public Task<Either<string, LoginResponse>> LoginComplianceOfficerAsync(LoginRequest request) =>
            SendAnonymous<LoginResponse>(HttpMethod.Post, "api/accountv2/login_complianceOfficer", request, "complianceOfficer login");

        public Task<Either<string, RegisterResponse>> RegisterMerchantAsync(RegisterRequest request) =>
            SendAnonymous<RegisterResponse>(HttpMethod.Post, "api/accountv2/register_merchant", request, "register merchant");

        public Task<Either<string, RegisterResponse>> RegisterComplianceOfficerAsync(RegisterRequest request) =>
            SendAnonymous<RegisterResponse>(HttpMethod.Post, "api/accountv2/register_complianceOfficer", request, "register complianceOfficer");

        public Task<Either<string, LogoutResponse>> LogoutMerchantAsync(LogoutRequest request) =>
            SendAnonymous<LogoutResponse>(HttpMethod.Post, "api/accountv2/logout_merchant", request, "logout merchant");

        public Task<Either<string, LogoutResponse>> LogoutComplianceOfficerAsync(LogoutRequest request) =>
            SendAnonymous<LogoutResponse>(HttpMethod.Post, "api/accountv2/logout_complianceOfficer", request, "logout complianceOfficer");

        public Task<Either<string, ValidateTokenResponse>> ValidateTokenAsync(ValidateTokenRequest request) =>
            Send<ValidateTokenResponse>(HttpMethod.Post, "api/accountv2/validate_merchant_token", request, "validate merchant token");

        public Task<Either<string, RefreshTokenResponse>> RefreshTokenAsync(RefreshTokenRequest request) =>
            Send<RefreshTokenResponse>(HttpMethod.Post, "api/auth/v2/refresh_token_v2", request, "refresh token");

        public Task<Either<string, UserDetailsDto>> GetUserByIdAsync(string userId) =>
            Send<UserDetailsDto>(HttpMethod.Get, $"api/admin/users/{Uri.EscapeDataString(userId)}", null, $"retrieve user {userId}");

        public Task<Either<string, AdminOperationResult>> CreateUserAsync(CreateUserRequest request) =>
            Send<AdminOperationResult>(HttpMethod.Post, "api/admin/users", request, "create user");

        public Task<Either<string, AdminOperationResult>> UpdateUserAsync(UpdateUserRequest request) =>
            Send<AdminOperationResult>(HttpMethod.Put, "api/admin/users", request, $"update user {request.Id}");

        public Task<Either<string, AdminOperationResult>> DeleteUserAsync(DeleteUserRequest request) =>
            Send<AdminOperationResult>(HttpMethod.Delete, "api/admin/users", request, $"delete user {request.UserId}");

        public Task<Either<string, AdminOperationResult>> LockUserAsync(LockUserRequest request) =>
            Send<AdminOperationResult>(HttpMethod.Post, "api/admin/users/lock", request,
                $"{(request.Lock ? "lock" : "unlock")} user {request.UserId}");

        public Task<Either<string, AdminOperationResult>> RefreshSecurityStampAsync(RefreshSecurityStampRequest request) =>
            Send<AdminOperationResult>(HttpMethod.Post, "api/admin/users/refresh-security-stamp", request,
                $"refresh security stamp for user {request.UserId}");

        public Task<Either<string, AdminOperationResult>> ChangeUserPasswordAsync(ChangeUserPasswordRequest request) =>
            Send<AdminOperationResult>(HttpMethod.Post, "api/admin/users/change-password", request,
                $"change password for user {request.UserId}");

        #region Role Management

        public Task<Either<string, List<RoleListItemDto>>> GetRolesAsync() =>
            Send<List<RoleListItemDto>>(HttpMethod.Get, "api/admin/roles", null, "retrieve role list");

        public Task<Either<string, RoleDetailsDto>> GetRoleByIdAsync(string roleId) =>
            Send<RoleDetailsDto>(HttpMethod.Get, $"api/admin/roles/{Uri.EscapeDataString(roleId)}", null, $"retrieve role {roleId}");

        public Task<Either<string, AdminOperationResult>> CreateRoleAsync(CreateRoleRequest request) =>
            Send<AdminOperationResult>(HttpMethod.Post, "api/admin/roles", request, "create role");

        public Task<Either<string, AdminOperationResult>> UpdateRoleAsync(UpdateRoleRequest request) =>
            Send<AdminOperationResult>(HttpMethod.Put, "api/admin/roles", request, $"update role {request.Id}");

        public Task<Either<string, AdminOperationResult>> DeleteRoleAsync(DeleteRoleRequest request) =>
            Send<AdminOperationResult>(HttpMethod.Delete, "api/admin/roles", request, $"delete role {request.RoleId}");

        #endregion

        #region Dashboard

        public Task<Either<string, AdminDashboardStatsDto>> GetDashboardStatsAsync() =>
            Send<AdminDashboardStatsDto>(HttpMethod.Get, "api/admin/dashboard/stats", null, "retrieve dashboard statistics");

        public Task<Either<string, BlazorMauiShared.Auth.PagedResult<UserListItemDto>>> GetUsersAsync() =>
            Send<BlazorMauiShared.Auth.PagedResult<UserListItemDto>>(HttpMethod.Get, "api/admin/users", null, "retrieve user list");

        #region Permission Management

        public async Task<Either<string, List<PermissionListItemDto>>> GetPermissionsAsync()
        {
            return await TryRequest(
                async () =>
                {
                    var token = GetAuthTokenRequired();
                    var result = await _httpClient.BaseAddress
                        .AppendPathSegment("api/admin/permissions")
                        .WithOAuthBearerToken(token)
                        .GetJsonAsync<List<PermissionListItemDto>>();
                    return result;
                },
                "retrieve permission list"
            );
        }

        public async Task<Either<string, List<PermissionCatalogDto>>> GetPermissionCatalogAsync()
        {
                    string apiBaseUrl = await RetrieveApiBaseUrl();
                    string uri = "api/admin/permissions/catalog";

                    var token = GetAuthTokenRequired();

                    IFlurlResponse response = await _flurlClient
                        .Request(uri.ToString())
                        .WithTimeout(TimeSpan.FromSeconds(30))
                        .AllowAnyHttpStatus()
                        .WithOAuthBearerToken(token)
                        .GetAsync();
                    string responseString = await response.ResponseMessage.Content.ReadAsStringAsync();
                   if (!response.ResponseMessage.IsSuccessStatusCode)
                    {
                        _logger.LogError($"Server returned non-success status code: {response.StatusCode}. Response content: {responseString}");
                        if (response.StatusCode == 404)
                        {
                            return new List<PermissionCatalogDto>();
                        }
                        throw new FetchDataException($"Server returned non-success status code: {response.StatusCode}");
                    }
                    JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
                    List<PermissionCatalogDto>? assets = JsonSerializer.Deserialize<List<PermissionCatalogDto>>(responseString, settings);
                    return assets ?? new List<PermissionCatalogDto>();
        }

        public async Task<Either<string, PermissionDetailsDto>> GetPermissionByIdAsync(Guid permissionId)
        {
            return await TryRequest(
                async () =>
                {
                    var token = GetAuthTokenRequired();
                    var result = await _httpClient.BaseAddress
                        .AppendPathSegments("api/admin/permissions", permissionId)
                        .WithOAuthBearerToken(token)
                        .GetJsonAsync<PermissionDetailsDto>();
                    return result;
                },
                $"retrieve permission {permissionId}"
            );
        }

        public async Task<Either<string, AdminOperationResult>> CreatePermissionAsync(CreatePermissionRequest request)
        {
            return await TryRequest(
                async () =>
                {
                    var token = GetAuthTokenRequired();
                    var result = await _httpClient.BaseAddress
                        .AppendPathSegment("api/admin/permissions")
                        .WithOAuthBearerToken(token)
                        .PostJsonAsync(request)
                        .ReceiveJson<AdminOperationResult>();
                    return result;
                },
                "create permission"
            );
        }

        public async Task<Either<string, AdminOperationResult>> UpdatePermissionAsync(UpdatePermissionRequest request)
        {
            return await TryRequest(
                async () =>
                {
                    var token = GetAuthTokenRequired();
                    var result = await _httpClient.BaseAddress
                        .AppendPathSegment("api/admin/permissions")
                        .WithOAuthBearerToken(token)
                        .PutJsonAsync(request)
                        .ReceiveJson<AdminOperationResult>();
                    return result;
                },
                $"update permission {request.Id}"
            );
        }

        public async Task<Either<string, AdminOperationResult>> DeletePermissionAsync(DeletePermissionRequest request)
        {
            return await TryRequest(
                async () =>
                {
                    var token = GetAuthTokenRequired();
                    var result = await _httpClient.BaseAddress
                        .AppendPathSegment("api/admin/permissions")
                        .WithOAuthBearerToken(token)
                        .SendJsonAsync(HttpMethod.Delete, request)
                        .ReceiveJson<AdminOperationResult>();
                    return result;
                },
                $"delete permission {request.PermissionId}"
            );
        }

        public async Task<Either<string, AdminOperationResult>> AssignPermissionsToRoleAsync(AssignRolePermissionsRequest request)
        {
            return await TryRequest(
                async () =>
                {
                    var token = GetAuthTokenRequired();
                    var result = await _httpClient.BaseAddress
                        .AppendPathSegment("api/admin/permissions/assign-to-role")
                        .WithOAuthBearerToken(token)
                        .PostJsonAsync(request)
                        .ReceiveJson<AdminOperationResult>();
                    return result;
                },
                $"assign permissions to role {request.RoleId}"
            );
        }

        public async Task<Either<string, AdminOperationResult>> GrantPermissionToUserAsync(GrantUserPermissionRequest request)
        {
            return await TryRequest(
                async () =>
                {
                    var token = GetAuthTokenRequired();
                    var result = await _httpClient.BaseAddress
                        .AppendPathSegment("api/admin/permissions/grant-to-user")
                        .WithOAuthBearerToken(token)
                        .PostJsonAsync(request)
                        .ReceiveJson<AdminOperationResult>();
                    return result;
                },
                $"grant permission to user {request.UserId}"
            );
        }

        public async Task<Either<string, UserEffectivePermissionsDto>> GetUserEffectivePermissionsAsync(string userId)
        {
            return await TryRequest(
                async () =>
                {
                    var token = GetAuthTokenRequired();
                    var result = await _httpClient.BaseAddress
                        .AppendPathSegments("api/admin/permissions/user", userId)
                        .WithOAuthBearerToken(token)
                        .GetJsonAsync<UserEffectivePermissionsDto>();
                    return result;
                },
                $"retrieve effective permissions for user {userId}"
            );
        }

        #endregion
        #endregion

        #region Helper Methods
        private Task<Either<string, T>> Send<T>(HttpMethod method, string relativeUri, object? body, string operationDescription) =>
            TryRequest(async () =>
            {
                using var request = new HttpRequestMessage(method, relativeUri);
       
                if (body is not null)
                {
                    request.Content = JsonContent.Create(body);
                }
                var token = GetAuthTokenRequired();
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException(
                        $"Status {(int)response.StatusCode} ({response.StatusCode}) while attempting to {operationDescription}. Response: {error}");
                }

                var payload = await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
                if (payload is null)
                {
                    throw new InvalidOperationException($"Empty or invalid {typeof(T).Name} payload when attempting to {operationDescription}.");
                }

                return payload;
            }, operationDescription);
        private Task<Either<string, T>> SendAnonymous<T>(HttpMethod method, string relativeUri, object? body, string operationDescription) =>
            TryRequest(async () =>
            {
                using var request = new HttpRequestMessage(method, relativeUri);

                if (body is not null)
                {
                    request.Content = JsonContent.Create(body);
                }

                using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException(
                        $"Status {(int)response.StatusCode} ({response.StatusCode}) while attempting to {operationDescription}. Response: {error}");
                }

                var payload = await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
                if (payload is null)
                {
                    throw new InvalidOperationException($"Empty or invalid {typeof(T).Name} payload when attempting to {operationDescription}.");
                }

                return payload;
            }, operationDescription);
        private async Task<Either<string, T>> TryRequest<T>(Func<Task<T>> requestFunc, string operationDescription)
        {
            try
            {
                _logger.LogInformation("Attempting to {Operation}", operationDescription);

                var result = await requestFunc();

                _logger.LogInformation("Successfully completed {Operation}", operationDescription);
                return result;
            }

            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "HTTP error during {Operation}", operationDescription);
                return $"Failed to {operationDescription}: {ex.Message}";
            }

            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error during {Operation}", operationDescription);
                return $"Failed to {operationDescription}: {ex.Message}";
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "HTTP error during {Operation}", operationDescription);
                return $"Failed to {operationDescription}: {ex.Message}";
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Timeout/cancellation during {Operation}", operationDescription);
                return $"Timed out while attempting to {operationDescription}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during {Operation}", operationDescription);
                return $"Unexpected error: {ex.Message}";
            }
        }

        #endregion
    }
}
