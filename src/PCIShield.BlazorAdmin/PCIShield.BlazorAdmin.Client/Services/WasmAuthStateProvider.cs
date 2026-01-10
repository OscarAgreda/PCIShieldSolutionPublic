// In PCIShield.BlazorAdmin.Client/Services/WasmAuthStateProvider.cs (WASM Project)
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;

using PCIShield.Client.Services.InvoiceSession;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;

namespace PCIShield.BlazorAdmin.Client.Services
{
    public class WasmAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ITokenService _tokenService;
        private readonly HttpClient _httpClientForHost;
        private readonly JwtSecurityTokenHandler _jwtHandler = new JwtSecurityTokenHandler();
        private readonly ILogger<WasmAuthStateProvider> _logger;

        public WasmAuthStateProvider(ITokenService tokenService,
                                     HostHttpClient hostHttpClient,
                                     ILogger<WasmAuthStateProvider> logger)
        {
            _tokenService = tokenService;
            _httpClientForHost = hostHttpClient.Client;
            _logger = logger;
            _logger.LogInformation($"WasmAuthStateProvider initialized. HttpClient BaseAddress: {_httpClientForHost.BaseAddress}");
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            _logger.LogInformation("WasmAuthStateProvider: GetAuthenticationStateAsync called.");
            string? token = await _tokenService.GetTokenAsync();
            var anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

            if (string.IsNullOrWhiteSpace(token))
            {
                _logger.LogInformation("WasmAuthStateProvider: No token in local storage. Attempting to fetch from host.");
                token = await FetchTokenFromHostAsync();
                if (!string.IsNullOrWhiteSpace(token))
                {
                    _logger.LogInformation($"WasmAuthStateProvider: Storing fetched token in localStorage. Token length: {token.Length}");
                    await _tokenService.SetTokenAsync(token);
                    var verifyToken = await _tokenService.GetTokenAsync();
                    _logger.LogInformation($"WasmAuthStateProvider: Token storage verification: {(string.IsNullOrEmpty(verifyToken) ? "FAILED" : "SUCCESS")}");
                }
                else
                {
                    _logger.LogWarning("WasmAuthStateProvider: Failed to fetch token from host");
                }
            }

            if (string.IsNullOrWhiteSpace(token))
            {
                _logger.LogInformation("WasmAuthStateProvider: Still no token after attempting fetch. Returning anonymous.");
                return anonymous;
            }

            try
            {
                var tokenContent = _jwtHandler.ReadJwtToken(token);
                if (tokenContent.ValidTo < DateTime.UtcNow)
                {
                    _logger.LogWarning("WasmAuthStateProvider: Token expired. Clearing token.");
                    await _tokenService.RemoveTokenAsync();
                    await _tokenService.RemoveRefreshTokenAsync();
                    NotifyAuthenticationStateChanged(Task.FromResult(anonymous));
                    return anonymous;
                }

                _logger.LogInformation("WasmAuthStateProvider: Token is valid.");
                var claims = ParseClaimsFromJwt(token);
                var identity = new ClaimsIdentity(claims, "jwt");
                return new AuthenticationState(new ClaimsPrincipal(identity));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "WasmAuthStateProvider: Error processing token. Clearing token.");
                await _tokenService.RemoveTokenAsync();
                await _tokenService.RemoveRefreshTokenAsync();
                NotifyAuthenticationStateChanged(Task.FromResult(anonymous));
                return anonymous;
            }
        }

        private async Task<string?> FetchTokenFromHostAsync()
        {
            _logger.LogInformation("WasmAuthStateProvider: Fetching token from host endpoint /auth/get-client-tokens");
            try
            {
                _logger.LogInformation($"WasmAuthStateProvider: HTTP client base address: {_httpClientForHost.BaseAddress}");
                string requestUrl = "/auth/get-client-tokens";
                if (_httpClientForHost.BaseAddress == null)
                {
                    _logger.LogError("WasmAuthStateProvider: HttpClient BaseAddress is null. Cannot fetch tokens.");
                    return null;
                }
                var httpResponse = await _httpClientForHost.GetAsync(requestUrl);
                _logger.LogInformation($"WasmAuthStateProvider: HTTP response status: {httpResponse.StatusCode}");
                _logger.LogInformation($"WasmAuthStateProvider: Content-Type: {httpResponse.Content.Headers.ContentType?.MediaType}");
                
                var responseContent = await httpResponse.Content.ReadAsStringAsync();
                
                if (httpResponse.IsSuccessStatusCode)
                {
                    if (httpResponse.Content.Headers.ContentType?.MediaType?.Contains("application/json") == true)
                    {
                        _logger.LogInformation($"WasmAuthStateProvider: Response content (first 100 chars): {responseContent.Substring(0, Math.Min(100, responseContent.Length))}");
                        
                        var response = System.Text.Json.JsonSerializer.Deserialize<ClientTokenResponse>(responseContent, 
                            new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        
                        if (response != null && !string.IsNullOrEmpty(response.AccessToken))
                        {
                            _logger.LogInformation($"WasmAuthStateProvider: Successfully fetched token from host. Token length: {response.AccessToken.Length}");
                            if (!string.IsNullOrEmpty(response.RefreshToken))
                            {
                                await _tokenService.SetRefreshTokenAsync(response.RefreshToken);
                            }
                            return response.AccessToken;
                        }
                        _logger.LogWarning("WasmAuthStateProvider: Response parsed but token was null or empty.");
                    }
                    else
                    {
                        _logger.LogWarning($"WasmAuthStateProvider: Expected JSON but got {httpResponse.Content.Headers.ContentType?.MediaType}. Content: {responseContent.Substring(0, Math.Min(200, responseContent.Length))}");
                    }
                }
                else
                {
                    _logger.LogWarning($"WasmAuthStateProvider: Failed to fetch token. Status: {httpResponse.StatusCode}");
                    _logger.LogWarning($"WasmAuthStateProvider: Error content (first 200 chars): {responseContent.Substring(0, Math.Min(200, responseContent.Length))}");
                    if (httpResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        _logger.LogWarning("WasmAuthStateProvider: User is not authenticated on the Host. Cookies may not be sent properly.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "WasmAuthStateProvider: Error fetching token from host.");
            }
            return null;
        }

        public void NotifyStateChanged()
        {
            _logger.LogInformation("WasmAuthStateProvider: External notification to re-evaluate auth state.");
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            keyValuePairs.TryGetValue(ClaimTypes.Role, out object roles);

            if (roles != null)
            {
                if (roles.ToString().Trim().StartsWith("["))
                {
                    var parsedRoles = System.Text.Json.JsonSerializer.Deserialize<string[]>(roles.ToString());
                    foreach (var parsedRole in parsedRoles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, parsedRole));
                    }
                }
                else
                {
                    claims.Add(new Claim(ClaimTypes.Role, roles.ToString()));
                }
                keyValuePairs.Remove(ClaimTypes.Role);
            }

            claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString())));
            return claims;
        }

        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }

        private class ClientTokenResponse
        {
            public string? AccessToken { get; set; }
            public string? RefreshToken { get; set; }
        }
    }
}