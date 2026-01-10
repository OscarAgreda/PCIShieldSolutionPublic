using System.Threading.Tasks;

using Blazored.LocalStorage;

using LanguageExt;

using Microsoft.Extensions.Logging;

namespace PCIShield.Client.Services.InvoiceSession
{
    public interface ITokenService
    {
        Task<string?> GetTokenAsync();
        Task SetTokenAsync(string? token);
        Task RemoveTokenAsync();
        Task<string?> GetRefreshTokenAsync();
        Task SetRefreshTokenAsync(string? refreshToken);
        Task RemoveRefreshTokenAsync();
        Task<string?> GetUserIdAsync();
        Task SetUserIdAsync(string? userId);
        Task RemoveUserIdAsync();
    }

    public class TokenService : ITokenService
    {
        private readonly ILocalStorageService _localStorage;
        private readonly ILogger<TokenService> _logger;
        private const string AuthTokenStorageKey = "authToken";
        private const string RefreshTokenStorageKey = "refreshToken";
        private const string UserIdStorageKey = "userId";
        public TokenService(ILocalStorageService localStorage, ILogger<TokenService> logger)
        {
            _localStorage = localStorage;
            _logger = logger;
        }

        public async Task<string?> GetTokenAsync()
        {
            var token = await _localStorage.GetItemAsync<string>(AuthTokenStorageKey);
            _logger.LogInformation($"TokenService.GetTokenAsync: Retrieved token from localStorage: {(string.IsNullOrEmpty(token) ? "NULL/EMPTY" : "Present (length: " + token.Length + ")")}");
            return token;
        }

        public async Task SetTokenAsync(string? token)
        {
            _logger.LogInformation($"TokenService.SetTokenAsync called with token: {(string.IsNullOrWhiteSpace(token) ? "NULL/EMPTY" : $"length {token.Length}")}");
            
            if (string.IsNullOrWhiteSpace(token))
            {
                _logger.LogInformation("TokenService.SetTokenAsync: Removing token from localStorage (null/empty provided)");
                await _localStorage.RemoveItemAsync(AuthTokenStorageKey);
            }
            else
            {
                _logger.LogInformation($"TokenService.SetTokenAsync: Storing token in localStorage (length: {token.Length})");
                await _localStorage.SetItemAsync(AuthTokenStorageKey, token);
                var verifyToken = await _localStorage.GetItemAsync<string>(AuthTokenStorageKey);
                _logger.LogInformation($"TokenService.SetTokenAsync: Verification - token {(string.IsNullOrEmpty(verifyToken) ? "NOT" : "successfully")} stored");
            }
        }
        public async Task RemoveTokenAsync()
        {
            await _localStorage.RemoveItemAsync(AuthTokenStorageKey);
        }
        public async Task<string?> GetRefreshTokenAsync()
        {
            return await _localStorage.GetItemAsync<string>(RefreshTokenStorageKey);
        }

        public async Task SetRefreshTokenAsync(string? refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                await _localStorage.RemoveItemAsync(RefreshTokenStorageKey);
            }
            else
            {
                await _localStorage.SetItemAsync(RefreshTokenStorageKey, refreshToken);
            }
        }
        public async Task RemoveRefreshTokenAsync()
        {
            await _localStorage.RemoveItemAsync(RefreshTokenStorageKey);
        }
        public async Task<string?> GetUserIdAsync()
        {
            return await _localStorage.GetItemAsync<string>(UserIdStorageKey);
        }

        public async Task SetUserIdAsync(string? userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                await _localStorage.RemoveItemAsync(UserIdStorageKey);
            }
            else
            {
                await _localStorage.SetItemAsync(UserIdStorageKey, userId);
            }
        }
        public async Task RemoveUserIdAsync()
        {
            await _localStorage.RemoveItemAsync(UserIdStorageKey);
        }
    }
}