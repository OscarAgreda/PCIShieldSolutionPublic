using System.Net.Http;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace PCIShield.BlazorAdmin.Client.Services
{
    public class HostHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HostHttpClient> _logger;
        
        public HostHttpClient(HttpClient httpClient, ILogger<HostHttpClient> logger, NavigationManager navigationManager)
        {
            _logger = logger;
            _httpClient = httpClient;
            if (_httpClient.BaseAddress == null)
            {
                var baseUri = new Uri(navigationManager.BaseUri);
                _httpClient.BaseAddress = baseUri;
                _logger.LogInformation($"HostHttpClient: Set BaseAddress to {baseUri}");
            }
            else
            {
                _logger.LogInformation($"HostHttpClient: Using existing BaseAddress {_httpClient.BaseAddress}");
            }
            _logger.LogInformation($"HostHttpClient: Default request headers count: {_httpClient.DefaultRequestHeaders.Count()}");
        }
        
        public HttpClient Client => _httpClient;
    }
}