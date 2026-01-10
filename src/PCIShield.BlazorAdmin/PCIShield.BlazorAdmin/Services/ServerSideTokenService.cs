using System.Threading.Tasks;
using PCIShield.Client.Services.InvoiceSession;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace PCIShield.BlazorAdmin.Services
{
    public class ServerSideTokenService : ITokenService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ServerSideTokenService> _logger;

        public ServerSideTokenService(IHttpContextAccessor httpContextAccessor, ILogger<ServerSideTokenService> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public Task<string?> GetTokenAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated == true)
            {
                var tokenClaim = httpContext.User.FindFirst("api_jwt_token");
                if (tokenClaim != null)
                {
                    _logger.LogInformation($"ServerSideTokenService: Retrieved token from claims (length: {tokenClaim.Value.Length})");
                    return Task.FromResult<string?>(tokenClaim.Value);
                }
                _logger.LogWarning("ServerSideTokenService: User authenticated but no api_jwt_token claim found");
            }
            else
            {
                _logger.LogInformation("ServerSideTokenService: User not authenticated or HttpContext not available");
            }
            return Task.FromResult<string?>(null);
        }

        public Task SetTokenAsync(string? token)
        {
            _logger.LogInformation("ServerSideTokenService: SetTokenAsync called (no-op on server)");
            return Task.CompletedTask;
        }

        public Task RemoveTokenAsync()
        {
            _logger.LogInformation("ServerSideTokenService: RemoveTokenAsync called (no-op on server)");
            return Task.CompletedTask;
        }

        public Task<string?> GetRefreshTokenAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated == true)
            {
                var refreshTokenClaim = httpContext.User.FindFirst("api_refresh_token");
                if (refreshTokenClaim != null)
                {
                    _logger.LogInformation("ServerSideTokenService: Retrieved refresh token from claims");
                    return Task.FromResult<string?>(refreshTokenClaim.Value);
                }
            }
            return Task.FromResult<string?>(null);
        }

        public Task SetRefreshTokenAsync(string? refreshToken)
        {
            return Task.CompletedTask;
        }

        public Task RemoveRefreshTokenAsync()
        {
            return Task.CompletedTask;
        }

        public Task<string?> GetUserIdAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null)
                {
                    return Task.FromResult<string?>(userIdClaim.Value);
                }
            }
            return Task.FromResult<string?>(null);
        }

        public Task SetUserIdAsync(string? userId)
        {
            return Task.CompletedTask;
        }

        public Task RemoveUserIdAsync()
        {
            return Task.CompletedTask;
        }
    }
}