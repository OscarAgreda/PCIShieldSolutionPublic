using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using PCIShield.Client.Services.InvoiceSession;
using Microsoft.Extensions.Logging;

namespace PCIShield.BlazorAdmin.Client.Services
{
    public class AuthTokenDelegatingHandler : DelegatingHandler
    {
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthTokenDelegatingHandler> _logger;

        public AuthTokenDelegatingHandler(ITokenService tokenService, ILogger<AuthTokenDelegatingHandler> logger)
        {
            _tokenService = tokenService;
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _tokenService.GetTokenAsync();
            
            if (!string.IsNullOrWhiteSpace(token))
            {
                _logger.LogDebug("Adding authorization header to request for {Uri}", request.RequestUri);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                _logger.LogDebug("No token available for request to {Uri}", request.RequestUri);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}