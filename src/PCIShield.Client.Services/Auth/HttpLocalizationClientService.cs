using System;
using System.Threading.Tasks;
using Flurl.Http;
using PCIShield.Client.Services.Common;
using PCIShield.Client.Services.InvoiceSession;
using PCIShieldLib.SharedKernel.Interfaces;
using LanguageExt;
using System.Text.Json;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using System.Reactive;

using PCIShield.BlazorMauiShared.Models;

using Microsoft.Maui.Devices;
using static LanguageExt.Prelude;
using ReactiveUI;

using Unit = System.Reactive.Unit;
using Flurl.Http.Configuration;

namespace PCIShield.Client.Services.Auth
{
    public  interface IHttpAppLocalizationClientService
    {
        Task<Either<string, string>> GetStringAsync(string key, string culture = null);
        Task<Either<string, bool>> SetUserPreferredCultureAsync(string culture);
        Task<Either<string, string>> GetUserPreferredCultureAsync();
    }

    public  class HttpAppLocalizationClientService :  IHttpAppLocalizationClientService, IDisposable
    {
        private readonly CompositeDisposable _disposables = new();
        private readonly IAppLoggerService<HttpAppLocalizationClientService> _logger;
     
        private FlurlClient _flurlClient;
        private HttpClient _httpClient;

        public HttpAppLocalizationClientService(
            IAppLoggerService<HttpAppLocalizationClientService> logger
            )
        {
            _logger = logger;
         
            LoadInitializeHttp();
        }

        public async Task<Either<string, string>> GetStringAsync(string key, string culture = null)
        {
            string uri = "api/localization/string";
            try
            {
                string apiBaseUrl = await RetrieveApiBaseUrl();
                var request = _flurlClient.Request($"{apiBaseUrl}{uri}")
                    .AllowAnyHttpStatus()
                    .SetQueryParam("key", key);

                if (!string.IsNullOrEmpty(culture))
                {
                    request = request.SetQueryParam("culture", culture);
                }

                var response = await request.GetJsonAsync<GetLocalizationStringResponse>();

                if (!string.IsNullOrEmpty(response.ErrorMessage))
                {
                    return Left<string, string>(response.ErrorMessage);
                }

                return Right<string, string>(response.Value ?? $"[{key}]");
            }
            catch (FlurlHttpTimeoutException ex)
            {
                int? statusCode = ex.Call.Response?.StatusCode;
                string responseBody = await ex.GetResponseStringAsync();
                _logger.LogError(ex.Message, "Timeout occurred while getting localization string. Status code: {StatusCode}, Response: {Response}",
                    statusCode, responseBody);
                return Left<string, string>($"Request timed out. Status code: {statusCode}");
            }
            catch (FlurlHttpException ex)
            {
                string responseBody = await ex.GetResponseStringAsync();
                string errorMsg = BuildFlurlErrorMessage("An error occurred while getting localization string", ex);
                _logger.LogError(ex, responseBody);
                return Left<string, string>(responseBody);
            }
            catch (Exception ex)
            {
                _logger.LogError("An unexpected error occurred while getting localization string.", ex);
                return Left<string, string>($"An unexpected error occurred: {ex.Message}");
            }
        }

        public async Task<Either<string, bool>> SetUserPreferredCultureAsync(string culture)
        {
            string uri = "api/user/preferred-culture";
            try
            {
                var request = new SetUserPreferredCultureRequest
                {
                    Culture = culture
                };

                string apiBaseUrl = await RetrieveApiBaseUrl();
                var response = await _flurlClient.Request($"{apiBaseUrl}{uri}")
                    .AllowAnyHttpStatus()
                    .PostJsonAsync(request)
                    .ReceiveJson<SetUserPreferredCultureResponse>();

                if (!response.Success)
                {
                    return Left<string, bool>(response.ErrorMessage ?? "Unknown error setting culture");
                }

                return Right<string, bool>(true);
            }
            catch (FlurlHttpTimeoutException ex)
            {
                int? statusCode = ex.Call.Response?.StatusCode;
                string responseBody = await ex.GetResponseStringAsync();
                _logger.LogError(ex.Message, "Timeout occurred while setting culture. Status code: {StatusCode}, Response: {Response}",
                    statusCode, responseBody);
                return Left<string, bool>($"Request timed out. Status code: {statusCode}");
            }
            catch (FlurlHttpException ex)
            {
                string responseBody = await ex.GetResponseStringAsync();
                string errorMsg = BuildFlurlErrorMessage("An error occurred while setting culture", ex);
                _logger.LogError(ex, responseBody);
                return Left<string, bool>(responseBody);
            }
            catch (Exception ex)
            {
                _logger.LogError("An unexpected error occurred while setting culture.", ex);
                return Left<string, bool>($"An unexpected error occurred: {ex.Message}");
            }
        }

        public async Task<Either<string, string>> GetUserPreferredCultureAsync()
        {
            string uri = "api/user/preferred-culture";
            try
            {
                string apiBaseUrl = await RetrieveApiBaseUrl();
                var response = await _flurlClient.Request($"{apiBaseUrl}{uri}")
                    .AllowAnyHttpStatus()
                    .GetJsonAsync<GetUserPreferredCultureResponse>();

                if (!string.IsNullOrEmpty(response.ErrorMessage))
                {
                    return Left<string, string>(response.ErrorMessage);
                }

                return Right<string, string>(response.Culture ?? "en-US");
            }
            catch (FlurlHttpTimeoutException ex)
            {
                int? statusCode = ex.Call.Response?.StatusCode;
                string responseBody = await ex.GetResponseStringAsync();
                _logger.LogError(ex.Message, "Timeout occurred while getting user culture. Status code: {StatusCode}, Response: {Response}",
                    statusCode, responseBody);
                return Left<string, string>($"Request timed out. Status code: {statusCode}");
            }
            catch (FlurlHttpException ex)
            {
                string responseBody = await ex.GetResponseStringAsync();
                string errorMsg = BuildFlurlErrorMessage("An error occurred while getting user culture", ex);
                _logger.LogError(ex, responseBody);
                return Left<string, string>(responseBody);
            }
            catch (Exception ex)
            {
                _logger.LogError("An unexpected error occurred while getting user culture.", ex);
                return Left<string, string>($"An unexpected error occurred: {ex.Message}");
            }
        }
        private string BuildFlurlErrorMessage(string operation, FlurlHttpException ex)
        {
            FlurlCall? call = ex.Call;
            string? errorMsg =
                $"Error while {operation}. HTTP Status: {call.Response?.StatusCode}, Request: {call.ToString()}, Response: {call.Response}";
            return errorMsg;
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

        private async Task<string> RetrieveApiBaseUrl()
        {
            string thisBaseUrl = "https://localhost:52509/";
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                thisBaseUrl = thisBaseUrl.Replace("localhost", "10.0.2.2");
            }
            return thisBaseUrl;
        }

        public void Dispose()
        {
            _disposables.Dispose();
            _httpClient?.Dispose();
            _flurlClient?.Dispose();
        }

        private class GetLocalizationStringResponse
        {
            public string Value { get; set; }
            public string ErrorMessage { get; set; }
        }

        private class SetUserPreferredCultureRequest
        {
            public string Culture { get; set; }
        }

        private class SetUserPreferredCultureResponse
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public string ErrorMessage { get; set; }
        }

        private class GetUserPreferredCultureResponse
        {
            public string Culture { get; set; }
            public string ErrorMessage { get; set; }
        }
    }
}