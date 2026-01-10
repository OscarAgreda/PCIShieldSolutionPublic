// PCIShield.BlazorAdmin.Client/Auth/AuthOrchestrator.cs

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reactive.Subjects;
using System.Security.Claims;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using LanguageExt;

using System.Reactive;
using PCIShield.BlazorMauiShared.Models.Auth;
using PCIShield.Client.Services.Auth;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

using Unit = System.Reactive.Unit;

namespace PCIShield.BlazorAdmin.Client.Auth
{
    public class AuthOrchestrator : IDisposable
    {
        private readonly IHttpAuthClientService _authService;
        private readonly ILocalStorageService _localStorage;
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly ILogger<AuthOrchestrator> _logger;
        private const string AuthTokenKey = "authToken";
        private const string RefreshTokenKey = "refreshToken";
        private const string UserIdKey = "userId";
        private const string UserRoleKey = "userRole";
        private const string PowerUserIdKey = "powerUserId";
        private const string UserTypeKey = "userType";
        public bool IsLoading { get; private set; }
        public bool IsAuthenticated { get; private set; }
        public string CurrentUserId { get; private set; }
        public string CurrentUserRole { get; private set; }
        public string AuthToken { get; private set; }
        public string UserType { get; private set; }
        public Subject<Unit> RefreshRequested { get; } = new Subject<Unit>();

        public AuthOrchestrator(
            IHttpAuthClientService authService,
            ILocalStorageService localStorage,
            HttpClient httpClient,
            AuthenticationStateProvider authStateProvider,
            ILogger<AuthOrchestrator> logger)
        {
            _authService = authService;
            _localStorage = localStorage;
            _httpClient = httpClient;
            _authStateProvider = authStateProvider;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            IsLoading = true;
            RefreshRequested.OnNext(Unit.Default);

            try
            {
                AuthToken = await _localStorage.GetItemAsync<string>(AuthTokenKey);
                CurrentUserId = await _localStorage.GetItemAsync<string>(UserIdKey);
                CurrentUserRole = await _localStorage.GetItemAsync<string>(UserRoleKey);
                UserType = await _localStorage.GetItemAsync<string>(UserTypeKey);

                if (!string.IsNullOrEmpty(AuthToken))
                {
                    var validateRequest = new ValidateTokenRequest { Token = AuthToken };
                    Either<string, ValidateTokenResponse> result = await _authService.ValidateTokenAsync(validateRequest);

                    await result.Match(
                        Right: async response =>
                        {
                            if (response.IsValidToken)
                            {
                                IsAuthenticated = true;
                                _httpClient.DefaultRequestHeaders.Authorization =
                                    new AuthenticationHeaderValue("Bearer", AuthToken);
                                ((PCIShieldAuthStateProvider)_authStateProvider).NotifyUserAuthentication(AuthToken);
                            }
                            else
                            {
                                await ClearAuthDataAsync();
                            }
                        },
                        Left: async error =>
                        {
                            _logger.LogWarning($"Token validation failed: {error}");
                            await ClearAuthDataAsync();
                        }
                    );
                }
                else
                {
                    await ClearAuthDataAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing auth state");
                await ClearAuthDataAsync();
            }
            finally
            {
                IsLoading = false;
                RefreshRequested.OnNext(Unit.Default);
            }
        }

        public async Task<Either<string, Unit>> LoginMerchantAsync(LoginRequest request)
        {
            IsLoading = true;
            RefreshRequested.OnNext(Unit.Default);

            try
            {
                Either<string, LoginResponse> result = await _authService.LoginMerchantAsync(request);

                return await result.Match(
                    Right: async response =>
                    {
                        if (response.IsValidToken && !string.IsNullOrEmpty(response.Token))
                        {
                            await StoreAuthDataAsync(
                                response.Token,
                                response.RefreshToken,
                                response.ApplicationUser?.ToString(),
                                "Merchant",
                                response.MerchantId,
                                response.PCIShieldAppPowerUserId
                            );

                            _httpClient.DefaultRequestHeaders.Authorization =
                                new AuthenticationHeaderValue("Bearer", response.Token);

                            IsAuthenticated = true;
                            UserType = "Merchant";
                            ((PCIShieldAuthStateProvider)_authStateProvider).NotifyUserAuthentication(response.Token);

                            return Unit.Default;
                        }

                        return Left<string, Unit>(response.ErrorMessage ?? "Login failed");
                    },
                    Left: error => Task.FromResult(Left<string, Unit>(error))
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during merchant login");
                return Left<string, Unit>($"Login error: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
                RefreshRequested.OnNext(Unit.Default);
            }
        }

        public async Task<Either<string, Unit>> LoginComplianceOfficerAsync(LoginRequest request)
        {
            IsLoading = true;
            RefreshRequested.OnNext(Unit.Default);

            try
            {
                Either<string, LoginResponse> result = await _authService.LoginComplianceOfficerAsync(request);

                return await result.Match(
                    Right: async response =>
                    {
                        if (response.IsValidToken && !string.IsNullOrEmpty(response.Token))
                        {
                            await StoreAuthDataAsync(
                                response.Token,
                                response.RefreshToken,
                                response.ApplicationUser?.ToString(),
                                "ComplianceOfficer",
                                response.ComplianceOfficerId,
                                response.PCIShieldAppPowerUserId
                            );

                            _httpClient.DefaultRequestHeaders.Authorization =
                                new AuthenticationHeaderValue("Bearer", response.Token);

                            IsAuthenticated = true;
                            UserType = "ComplianceOfficer";
                            ((PCIShieldAuthStateProvider)_authStateProvider).NotifyUserAuthentication(response.Token);

                            return Unit.Default;
                        }

                        return Left<string, Unit>(response.ErrorMessage ?? "Login failed");
                    },
                    Left: error => Task.FromResult(Left<string, Unit>(error))
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during complianceOfficer login");
                return Left<string, Unit>($"Login error: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
                RefreshRequested.OnNext(Unit.Default);
            }
        }

        public async Task<Either<string, Unit>> RegisterMerchantAsync(RegisterRequest request)
        {
            IsLoading = true;
            RefreshRequested.OnNext(Unit.Default);

            try
            {
                Either<string, RegisterResponse> result = await _authService.RegisterMerchantAsync(request);

                return await result.Match(
                    Right: async response =>
                    {
                        if (response.Message?.Contains("success") == true)
                        {
                            return Unit.Default;
                        }

                        return Left<string, Unit>(response.ErrorMessage ?? "Registration failed");
                    },
                    Left: error => Task.FromResult(Left<string, Unit>(error))
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during merchant registration");
                return Left<string, Unit>($"Registration error: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
                RefreshRequested.OnNext(Unit.Default);
            }
        }

        public async Task<Either<string, Unit>> RegisterComplianceOfficerAsync(RegisterRequest request)
        {
            IsLoading = true;
            RefreshRequested.OnNext(Unit.Default);

            try
            {
                Either<string, RegisterResponse> result = await _authService.RegisterComplianceOfficerAsync(request);

                return await result.Match(
                    Right: async response =>
                    {
                        if (response.Message?.Contains("success") == true)
                        {
                            return Unit.Default;
                        }

                        return Left<string, Unit>(response.ErrorMessage ?? "Registration failed");
                    },
                    Left: error => Task.FromResult(Left<string, Unit>(error))
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during complianceOfficer registration");
                return Left<string, Unit>($"Registration error: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
                RefreshRequested.OnNext(Unit.Default);
            }
        }

        public async Task<Either<string, Unit>> LogoutAsync()
        {
            IsLoading = true;
            RefreshRequested.OnNext(Unit.Default);

            try
            {
                string userId = await _localStorage.GetItemAsync<string>(UserIdKey);
                string userType = await _localStorage.GetItemAsync<string>(UserTypeKey);

                if (string.IsNullOrEmpty(userId))
                {
                    await ClearAuthDataAsync();
                    return Unit.Default;
                }

                var request = new LogoutRequest { UserId = userId };
                Either<string, LogoutResponse> result;

                if (userType == "Merchant")
                {
                    result = await _authService.LogoutMerchantAsync(request);
                }
                else
                {
                    result = await _authService.LogoutComplianceOfficerAsync(request);
                }

                await ClearAuthDataAsync();

                return await result.Match(
                    Right: async response =>
                    {
                        if (response.IsSuccess)
                        {
                            return Unit.Default;
                        }

                        return Left<string, Unit>(response.Message ?? "Logout failed");
                    },
                    Left: error => Task.FromResult(Left<string, Unit>(error))
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                await ClearAuthDataAsync();
                return Left<string, Unit>($"Logout error: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
                RefreshRequested.OnNext(Unit.Default);
            }
        }

        private async Task StoreAuthDataAsync(
            string token,
            string refreshToken,
            string userId,
            string role,
            string entityId,
            string powerUserId)
        {
            await _localStorage.SetItemAsync(AuthTokenKey, token);
            await _localStorage.SetItemAsync(RefreshTokenKey, refreshToken);
            await _localStorage.SetItemAsync(UserIdKey, userId);
            await _localStorage.SetItemAsync(UserRoleKey, role);
            await _localStorage.SetItemAsync(PowerUserIdKey, powerUserId);
            await _localStorage.SetItemAsync(UserTypeKey, role);

            AuthToken = token;
            CurrentUserId = userId;
            CurrentUserRole = role;
            UserType = role;
        }

        public async Task<Either<string, Unit>> RefreshTokenAsync()
        {
            try
            {
                string refreshToken = await _localStorage.GetItemAsync<string>(RefreshTokenKey);
                if (string.IsNullOrEmpty(refreshToken))
                {
                    return Left<string, Unit>("No refresh token available");
                }

                var request = new RefreshTokenRequest { RefreshToken = refreshToken };
                Either<string, RefreshTokenResponse> result = await _authService.RefreshTokenAsync(request);

                return await result.Match(
                    Right: async response =>
                    {
                        if (!string.IsNullOrEmpty(response.AccessToken))
                        {
                            await _localStorage.SetItemAsync(AuthTokenKey, response.AccessToken);
                            await _localStorage.SetItemAsync(RefreshTokenKey, response.RefreshToken);

                            AuthToken = response.AccessToken;
                            _httpClient.DefaultRequestHeaders.Authorization =
                                new AuthenticationHeaderValue("Bearer", response.AccessToken);
                            ((PCIShieldAuthStateProvider)_authStateProvider).NotifyUserAuthentication(response.AccessToken);

                            return Unit.Default;
                        }

                        return Left<string, Unit>(response.Error ?? "Token refresh failed");
                    },
                    Left: error => Task.FromResult(Left<string, Unit>(error))
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing token");
                return Left<string, Unit>($"Token refresh error: {ex.Message}");
            }
        }

        private async Task ClearAuthDataAsync()
        {
            await _localStorage.RemoveItemAsync(AuthTokenKey);
            await _localStorage.RemoveItemAsync(RefreshTokenKey);
            await _localStorage.RemoveItemAsync(UserIdKey);
            await _localStorage.RemoveItemAsync(UserRoleKey);
            await _localStorage.RemoveItemAsync(PowerUserIdKey);
            await _localStorage.RemoveItemAsync(UserTypeKey);

            _httpClient.DefaultRequestHeaders.Authorization = null;

            AuthToken = null;
            CurrentUserId = null;
            CurrentUserRole = null;
            UserType = null;
            IsAuthenticated = false;
            ((PCIShieldAuthStateProvider)_authStateProvider).NotifyUserLogout();
        }

        public void Dispose()
        {
            RefreshRequested.Dispose();
        }
    }
}