using System.Net.Http.Headers;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text.Json;
using System.Text;
using System.Web;
using Ardalis.GuardClauses;

using BlazorMauiShared.Models.Merchant;
using BlazorMauiShared.Models.PaymentChannel;
using BlazorMauiShared.Models.PaymentPage;
using BlazorMauiShared.Models.ROCPackage;
using BlazorMauiShared.Models.ScanSchedule;
using BlazorMauiShared.Models.Vulnerability;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.BlazorMauiShared.ModelsDto;
using PCIShield.Client.Services.Common;

using PCIShield.Domain.ModelsDto;
using PCIShield.BlazorMauiShared.Models.PaymentPage;
using Flurl.Http;
using Flurl.Http.Configuration;
using Microsoft.Maui.Devices;

using PCIShield.BlazorMauiShared.Models.Merchant;
using PCIShield.BlazorMauiShared.Models.PaymentChannel;
using PCIShield.Client.Services.InvoiceSession;

using ReactiveUI;
using JsonSerializer = System.Text.Json.JsonSerializer;
using JsonSerializerOptions = System.Text.Json.JsonSerializerOptions;
using Unit = System.Reactive.Unit;
  	  
namespace PCIShield.Client.Services.PaymentChannel;
  	  
public class HttpPaymentChannelClientService : IDisposable, IHttpPaymentChannelClientService
{
    private readonly CompositeDisposable _disposables = new();
    private readonly IAppLoggerService<HttpPaymentChannelClientService> _logger;
    private readonly ITokenService _tokenService;
    private FlurlClient _flurlClient;
    private HttpClient _httpClient;
  	  
    public HttpPaymentChannelClientService(
        IAppLoggerService<HttpPaymentChannelClientService> logger,
        ITokenService tokenService
    )
    {
        _logger = logger;
        _tokenService = tokenService;
        LoadInitializeHttp();
    }
    private void HandleHttpServiceError(Exception ex, ErrorHandlingService.ClientOperationContext context)
    {
        ErrorHandlingService.GetContextualErrorMessage(ex, context);
    }

    public async Task<UpdatePaymentChannelResponse> UpdatePutPaymentChannelAsync(UpdatePaymentChannelRequest data)
    {
        string? uri = "api/paymentChannels";
        try
        {
            string? apiBaseUrl = await RetrieveApiBaseUrl();
            IFlurlResponse? response = await _flurlClient.Request($"{apiBaseUrl}{uri}")
                .AllowAnyHttpStatus()
                .PutJsonAsync(data);

            string responseString = await response.ResponseMessage.Content.ReadAsStringAsync();
            if (!response.ResponseMessage.IsSuccessStatusCode)
            {
                _logger.LogError(
                    $"Server returned non-success status code: {response.StatusCode}. Response content: {responseString}");
                throw new FetchDataException($"Server returned non-success status code: {response.StatusCode}");
            }

            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            UpdatePaymentChannelResponse? responseDto =
                JsonSerializer.Deserialize<UpdatePaymentChannelResponse>(responseString, settings);

            return responseDto;
        }
        catch (FlurlHttpTimeoutException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Timeout occurred while updating paymentChannel. Status code: {StatusCode}, Response: {Response}",
                statusCode, responseBody);
            throw new FetchDataException($"Update request timed out. Status code: {statusCode}", ex);
        }
        catch (FlurlHttpException ex)
        {
            string responseBody = await ex.GetResponseStringAsync();
            string? errorMsg = BuildFlurlErrorMessage("An error occurred while updating paymentChannel", ex);
            _logger.LogError(ex, responseBody);
            throw new FetchDataException(responseBody, ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error deserializing paymentChannel update response");
            throw new FetchDataException("Error processing update response", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError("An unexpected error occurred while updating paymentChannel.", ex);
            throw new FetchDataException("An unexpected error occurred while updating paymentChannel.", ex);
        }
    }
    public async Task<CreatePaymentChannelResponse> CreatePostPaymentChannelAsync(CreatePaymentChannelRequest data)
    {
        string? uri = "api/paymentChannels";
        try
        {
            string? apiBaseUrl = await RetrieveApiBaseUrl();
            IFlurlResponse? response = await _flurlClient.Request($"{apiBaseUrl}{uri}")
                .AllowAnyHttpStatus()
                .PostJsonAsync(data);

            string responseString = await response.ResponseMessage.Content.ReadAsStringAsync();
            if (!response.ResponseMessage.IsSuccessStatusCode)
            {
                _logger.LogError(
                    $"Server returned non-success status code: {response.StatusCode}. Response content: {responseString}");
                throw new FetchDataException($"Server returned non-success status code: {response.StatusCode}");
            }

            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            CreatePaymentChannelResponse? responseDto =
                JsonSerializer.Deserialize<CreatePaymentChannelResponse>(responseString, settings);

            return responseDto;
        }
        catch (FlurlHttpTimeoutException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Timeout occurred while creating paymentChannel. Status code: {StatusCode}, Response: {Response}",
                statusCode, responseBody);
            HandleHttpServiceError(ex, ErrorHandlingService.ClientOperationContext.Create);
            throw new FetchDataException($"Create request timed out. Status code: {statusCode}", ex);
        }
        catch (FlurlHttpException ex)
        {
            string responseBody = await ex.GetResponseStringAsync();
            string? errorMsg = BuildFlurlErrorMessage("An error occurred while creating paymentChannel", ex);
            _logger.LogError(ex, responseBody);
            throw new FetchDataException(responseBody, ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error deserializing paymentChannel creation response");
            throw new FetchDataException("Error processing creation response", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError("An unexpected error occurred while creating paymentChannel.", ex);
            throw new FetchDataException("An unexpected error occurred while creating paymentChannel.", ex);
        }
    }
    public async Task<GetByIdPaymentChannelResponse> GetLastCreatedPaymentChannelAsync()
    {
        string uri = "api/paymentChannels/last-created";
        try
        {
            string apiBaseUrl = await RetrieveApiBaseUrl();
            IFlurlResponse response = await _flurlClient.Request($"{apiBaseUrl}{uri}")
                .AllowAnyHttpStatus()
                .GetAsync();

            string responseString = await response.ResponseMessage.Content.ReadAsStringAsync();
            if (!response.ResponseMessage.IsSuccessStatusCode)
            {
                _logger.LogError($"Server returned non-success status code: {response.StatusCode}. Response content: {responseString}");
                throw new FetchDataException($"Server returned non-success status code: {response.StatusCode}");
            }

            var settings = GJset.GetSystemTextJsonSettings();
            return JsonSerializer.Deserialize<GetByIdPaymentChannelResponse>(responseString, settings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving last created paymentChannel");
            throw new FetchDataException("Error retrieving last created paymentChannel", ex);
        }
    }
    public async Task<DeletePaymentChannelResponse> DeletePaymentChannelAsync(Guid paymentChannelId)
    {
        string? uri = $"api/paymentChannels/{paymentChannelId}";
        try
        {
            string? apiBaseUrl = await RetrieveApiBaseUrl();
            IFlurlResponse? response = await _flurlClient.Request($"{apiBaseUrl}{uri}")
                .AllowAnyHttpStatus()
                .DeleteAsync();

            string responseString = await response.ResponseMessage.Content.ReadAsStringAsync();
            if (!response.ResponseMessage.IsSuccessStatusCode)
            {
                _logger.LogError(
                    $"Server returned non-success status code: {response.StatusCode}. Response content: {responseString}");
                throw new FetchDataException($"Server returned non-success status code: {response.StatusCode}");
            }

            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            DeletePaymentChannelResponse? responseDto =
                JsonSerializer.Deserialize<DeletePaymentChannelResponse>(responseString, settings);

            return responseDto;
        }
        catch (FlurlHttpTimeoutException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Timeout occurred while deleting paymentChannel. Status code: {StatusCode}, Response: {Response}",
                statusCode, responseBody);
            throw new FetchDataException($"Delete request timed out. Status code: {statusCode}", ex);
        }
        catch (FlurlHttpException ex)
        {
            string responseBody = await ex.GetResponseStringAsync();
            string? errorMsg = BuildFlurlErrorMessage("An error occurred while deleting paymentChannel", ex);
            _logger.LogError(ex, responseBody);
            throw new FetchDataException(responseBody, ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error deserializing paymentChannel deletion response");
            throw new FetchDataException("Error processing deletion response", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError("An unexpected error occurred while deleting paymentChannel.", ex);
            throw new FetchDataException("An unexpected error occurred while deleting paymentChannel.", ex);
        }
    }
    public async Task<ListPaymentChannelResponse?> GetFilteredPaymentChannelListAsync(int pageNumber, int pageSize, Dictionary<string, string> filters = null, List<PCIShieldLib.SharedKernel.Interfaces.Sort> sorting = null)
    {
        try
        {
            if (sorting == null && filters == null)
            {
                return await GetPagedPaymentChannelsListAsync(pageNumber, pageSize);
            }
           if (sorting == null && filters.Count == 1 && filters.TryGetValue("search", out string? filter))
            {
                var searchResults = await SearchPaymentChannelByPaymentChannelAsync(filter);
                return new ListPaymentChannelResponse { PaymentChannels = searchResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList(), Count = searchResults.Count };
            }
            string uri = $"api/paymentChannels/filtered_list";
            string apiBaseUrl = await RetrieveApiBaseUrl();
            var request = new FilteredPaymentChannelRequest
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Filters = filters,
                Sorting = sorting,
            };
            IFlurlResponse? response = await _flurlClient.Request($"{apiBaseUrl}{uri}").AllowAnyHttpStatus().PostJsonAsync(request);
            string responseString = await response.ResponseMessage.Content.ReadAsStringAsync();
            if (!response.ResponseMessage.IsSuccessStatusCode)
            {
                _logger.LogError($"Server returned non-success status code: {response.StatusCode}. Response content: {responseString}");
                throw new FetchDataException($"Server returned non-success status code: {response.StatusCode}");
            }
            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            return JsonSerializer.Deserialize<ListPaymentChannelResponse>(responseString, settings);
        }
        catch (FlurlHttpTimeoutException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Flurl HTTP error occurred. Status code: {StatusCode}, Response: {Response}", statusCode, responseBody);
            throw new FetchDataException($"An HTTP error occurred. Status code: {statusCode}", ex);
        }
        catch (FlurlHttpException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Flurl HTTP error occurred. Status code: {StatusCode}, Response: {Response}", statusCode, responseBody);
            throw new FetchDataException($"An HTTP error occurred. Status code: {statusCode}", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while fetching filtered paymentChannel list.");
            throw new FetchDataException("An unexpected error occurred while fetching filtered paymentChannel list.", ex);
        }
    }
    public async Task<ListPaymentChannelResponse?> GetPagedPaymentChannelsListAsync(int pageNumber, int pageSize)
    {
        string uri = $"api/paymentChannels/paged_list";
        try
        {
            string apiBaseUrl = await RetrieveApiBaseUrl();
            GetPaymentChannelListRequest request = new() { PageNumber = pageNumber, PageSize = pageSize };
            IFlurlResponse? response = await _flurlClient.Request($"{apiBaseUrl}{uri}").AllowAnyHttpStatus().PostJsonAsync(request);
            string responseString = await response.ResponseMessage.Content.ReadAsStringAsync();
            if (!response.ResponseMessage.IsSuccessStatusCode)
            {
                _logger.LogError($"Server returned non-success status code: {response.StatusCode}. Response content: {responseString}");
                throw new FetchDataException($"Server returned non-success status code: {response.StatusCode}");
            }
            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            ListPaymentChannelResponse? responseDto = JsonSerializer.Deserialize<ListPaymentChannelResponse>(responseString, settings);
            if (responseDto?.PaymentChannels != null) { }
            return responseDto;
        }
        catch (FlurlHttpTimeoutException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Flurl HTTP error occurred. Status code: {StatusCode}, Response: {Response}", statusCode, responseBody);
            throw new FetchDataException($"An HTTP error occurred. Status code: {statusCode}", ex);
        }
        catch (FlurlHttpException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Flurl HTTP error occurred. Status code: {StatusCode}, Response: {Response}", statusCode, responseBody);
            throw new FetchDataException($"An HTTP error occurred. Status code: {statusCode}", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while fetching paged paymentChannel list.");
            throw new FetchDataException("An unexpected error occurred while fetching paged paymentChannel list.", ex);
        }
    }
    public async Task<List<PaymentChannelDto>> SearchPaymentChannelByPaymentChannelAsync(string searchTerm)
    {
        string uri = $"api/paymentChannels/search";
        try
        {
            string apiBaseUrl = await RetrieveApiBaseUrl();
            var url = new StringBuilder($"{apiBaseUrl}{uri}").Append("?SearchTerm=").Append(HttpUtility.UrlEncode(searchTerm));
            IFlurlResponse response = await _flurlClient.Request(url.ToString()).WithTimeout(TimeSpan.FromSeconds(30)).AllowAnyHttpStatus().GetAsync();
            string responseString = await response.ResponseMessage.Content.ReadAsStringAsync();
            if (!response.ResponseMessage.IsSuccessStatusCode)
            {
                _logger.LogError($"Server returned non-success status code: {response.StatusCode}. Response content: {responseString}");
                if (response.StatusCode == 404)
                {
                    return new List<PaymentChannelDto>();
                }
                throw new FetchDataException($"Server returned non-success status code: {response.StatusCode}");
            }
            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            List<PaymentChannelDto>? paymentChannels = JsonSerializer.Deserialize<List<PaymentChannelDto>>(responseString, settings);
            return paymentChannels ?? new List<PaymentChannelDto>();
        }
        catch (FlurlHttpTimeoutException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Timeout occurred while searching paymentChannels. Status code: {StatusCode}, Response: {Response}", statusCode, responseBody);
            throw new FetchDataException($"Search request timed out. Status code: {statusCode}", ex);
        }
        catch (FlurlHttpException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "HTTP error occurred while searching paymentChannels. Status code: {StatusCode}, Response: {Response}", statusCode, responseBody);
            throw new FetchDataException($"An HTTP error occurred while searching. Status code: {statusCode}", ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error deserializing paymentChannel search response");
            throw new FetchDataException("Error processing search results", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while searching paymentChannels");
            throw new FetchDataException("An unexpected error occurred while searching paymentChannels", ex);
        }
    }

    public async Task<GetByIdPaymentChannelResponse> GetOneFullPaymentChannelByIdAsync(Guid paymentChannelId, bool withPostGraph = false)
    {
        string? uri = $"api/paymentChannels/i/{paymentChannelId}";
        try
        {
            string? apiBaseUrl = await RetrieveApiBaseUrl();
            GetByIdPaymentChannelRequest request = new() { PaymentChannelId = paymentChannelId, WithPostGraph = withPostGraph };
            IFlurlResponse? response = await _flurlClient.Request($"{apiBaseUrl}{uri}")
                .AllowAnyHttpStatus()
                .PostJsonAsync(request);

            string responseString = await response.ResponseMessage.Content.ReadAsStringAsync();
            if (!response.ResponseMessage.IsSuccessStatusCode)
            {
                _logger.LogError(
                    $"Server returned non-success status code: {response.StatusCode}. Response content: {responseString}");
                if (response.StatusCode == 404)
                {
                    _logger.LogWarning($"PaymentChannel with paymentChannelId {paymentChannelId} not found.");
                    throw new NotFoundException($"PaymentChannel with paymentChannelId {paymentChannelId} not found.", paymentChannelId.ToString());
                }
                throw new FetchDataException($"Server returned non-success status code: {response.StatusCode}");
            }

            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            GetByIdPaymentChannelResponse? responseDto =
                JsonSerializer.Deserialize<GetByIdPaymentChannelResponse>(responseString, settings);

            if (responseDto?.PaymentChannel == null)
            {
                _logger.LogWarning($"Deserialized response contains no paymentChannel data for ID {paymentChannelId}");
                throw new NotFoundException($"No paymentChannel data found for paymentChannelId {paymentChannelId}", paymentChannelId.ToString());
            }

            return responseDto;
        }
        catch (FlurlHttpTimeoutException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Timeout occurred while fetching paymentChannel. Status code: {StatusCode}, Response: {Response}",
                statusCode, responseBody);
            throw new FetchDataException($"Request timed out while fetching paymentChannel {paymentChannelId}. Status code: {statusCode}", ex);
        }
        catch (FlurlHttpException ex)
        {
            string responseBody = await ex.GetResponseStringAsync();
            string? errorMsg = BuildFlurlErrorMessage($"An error occurred while fetching paymentChannel {paymentChannelId}", ex);
            _logger.LogError(ex, responseBody);
            throw new FetchDataException(responseBody, ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex.Message, "Error deserializing paymentChannel response for ID {PaymentChannelId}", paymentChannelId);
            throw new FetchDataException($"Error processing response for paymentChannel {paymentChannelId}", ex);
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "An unexpected error occurred while fetching paymentChannel {PaymentChannelId}", paymentChannelId);
            throw new FetchDataException($"An unexpected error occurred while fetching paymentChannel {paymentChannelId}", ex);
        }
    }
    public async Task<SpecificationPerformanceResponse<PaymentChannelDto>> RunPaymentChannelPerformanceTests(Guid paymentChannelId)
    {
        string? uri = $"api/paymentChannels/perf/{paymentChannelId}";
        try
        {
            string? apiBaseUrl = await RetrieveApiBaseUrl();
            GetByIdPaymentChannelRequest request = new() { PaymentChannelId = paymentChannelId };
            IFlurlResponse? response = await _flurlClient.Request($"{apiBaseUrl}{uri}")
                .AllowAnyHttpStatus()
                .PostJsonAsync(request);

            string responseString = await response.ResponseMessage.Content.ReadAsStringAsync();
            if (!response.ResponseMessage.IsSuccessStatusCode)
            {
                _logger.LogError(
                    $"Server returned non-success status code: {response.StatusCode}. Response content: {responseString}");
                if (response.StatusCode == 404)
                {
                    _logger.LogWarning($"PaymentChannel with paymentChannelId {paymentChannelId} not found.");
                    throw new NotFoundException($"PaymentChannel with paymentChannelId {paymentChannelId} not found.", paymentChannelId.ToString());
                }
                throw new FetchDataException($"Server returned non-success status code: {response.StatusCode}");
            }

            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            SpecificationPerformanceResponse<PaymentChannelDto>? responseDto =
                JsonSerializer.Deserialize<SpecificationPerformanceResponse<PaymentChannelDto>>(responseString, settings);

            return responseDto;
        }
        catch (FlurlHttpTimeoutException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Timeout occurred while PerformanceTests paymentChannel. Status code: {StatusCode}, Response: {Response}",
                statusCode, responseBody);
            throw new FetchDataException($"Request timed out while PerformanceTests paymentChannel {paymentChannelId}. Status code: {statusCode}", ex);
        }
        catch (FlurlHttpException ex)
        {
            string responseBody = await ex.GetResponseStringAsync();
            string? errorMsg = BuildFlurlErrorMessage($"An error occurred while PerformanceTests paymentChannel {paymentChannelId}", ex);
            _logger.LogError(ex, responseBody);
            throw new FetchDataException(responseBody, ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex.Message, "Error deserializing paymentChannel response for ID {PaymentChannelId}", paymentChannelId);
            throw new FetchDataException($"Error processing response for paymentChannel {paymentChannelId}", ex);
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "An unexpected error occurred while fetching paymentChannel {PaymentChannelId}", paymentChannelId);
            throw new FetchDataException($"An unexpected error occurred while fetching paymentChannel {paymentChannelId}", ex);
        }
    }
    public async Task<CreatePaymentPageResponse> CreatePaymentPageAsync(PaymentPageDto data)
    {
        string uri = $"api/paymentPages";
        try
        {
            var request = new CreatePaymentPageRequest { PaymentPage = data };
            string apiBaseUrl = await RetrieveApiBaseUrl();
            CreatePaymentPageResponse response = await _flurlClient.Request($"{apiBaseUrl}{uri}")
                .AllowAnyHttpStatus()
                .PostJsonAsync(request).ReceiveJson<CreatePaymentPageResponse>();

            var aa = response.PaymentPage;
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while creating a paymentPage .", ex);
            throw new FetchDataException("An error occurred while creating data.", ex);
        }
    }

    public async Task<PaymentPageDto> UpdatePaymentPageAsync(PaymentPageDto data)
    {
        string uri = $"api/paymentPages";
        try
        {

            var request = new UpdatePaymentPageRequest() { PaymentPage = data };
            string apiBaseUrl = await RetrieveApiBaseUrl();
            PaymentPageDto response = await _flurlClient.Request($"{apiBaseUrl}{uri}")
                .AllowAnyHttpStatus()
                .PutJsonAsync(request)
                .ReceiveJson<PaymentPageDto>();
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while updating a paymentPage .", ex);
            throw new FetchDataException("An error occurred while updating data.", ex);
        }
    }

    public async Task DeletePaymentPageAsync(Guid paymentPageId)
    {
        string uri = $"api/paymentChannel/paymentPages/{paymentPageId}";
        try
        {
            string apiBaseUrl = await RetrieveApiBaseUrl();
            await _flurlClient.Request($"{apiBaseUrl}{uri}")
                .AllowAnyHttpStatus()
                .DeleteAsync()
                .ReceiveJson<DeletePaymentPageResponse>();
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while deleting a paymentPage .", ex);
            throw new FetchDataException("An error occurred while deleting data.", ex);
        }
    }

    public async Task<GetByIdPaymentPageResponse> GetOneFullPaymentPageByIdAsync(Guid paymentPageId)
    {
        try
        {
            string? apiBaseUrl = await RetrieveApiBaseUrl();
            string uri = $"api/paymentChannel/paymentPages/{paymentPageId}";
             GetByIdPaymentPageRequest request = new() { PaymentPageId = paymentPageId };
            IFlurlResponse? response = await _flurlClient.Request($"{apiBaseUrl}{uri}")
                .AllowAnyHttpStatus()
                .PostJsonAsync(request);

            string responseString = await response.ResponseMessage.Content.ReadAsStringAsync();
            if (!response.ResponseMessage.IsSuccessStatusCode)
            {
                _logger.LogError(
                    $"Server returned non-success status code: {response.StatusCode}. Response content: {responseString}");
                if (response.StatusCode == 404)
                {
                    _logger.LogWarning($"PaymentPage with paymentPage not found.");
                    throw new NotFoundException($"PaymentPage paymentPageId not found.", paymentPageId.ToString());
                }
                throw new FetchDataException($"Server returned non-success status code: {response.StatusCode}");
            }

            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            GetByIdPaymentPageResponse? responseDto =
                JsonSerializer.Deserialize<GetByIdPaymentPageResponse>(responseString, settings);

            if (responseDto?.PaymentPage == null)
            {
                _logger.LogWarning($"Deserialized response contains no paymentPage data for ID ");
                throw new NotFoundException($"No paymentPage data found for paymentPageId", paymentPageId.ToString());
            }

            return responseDto;
        }
        catch (FlurlHttpTimeoutException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Timeout occurred while fetching paymentPage. Status code: {StatusCode}, Response: {Response}",
                statusCode, responseBody);
            throw new FetchDataException($"Request timed out while fetching paymentPage data for ID ");
        }
        catch (FlurlHttpException ex)
        {
            string responseBody = await ex.GetResponseStringAsync();
            string? errorMsg = BuildFlurlErrorMessage($"An error occurred while fetching paymentPage ", ex);
            _logger.LogError(ex, responseBody);
            throw new FetchDataException(responseBody, ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex.Message, "Error deserializing paymentPage response for ID {PaymentPageId}", paymentPageId);
            throw new FetchDataException($"Error processing response for paymentPage {paymentPageId}", ex);
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "An unexpected error occurred while fetching paymentPage PaymentPage");
            throw new FetchDataException($"An unexpected error occurred while fetching paymentPage ", ex);
        }
    }

    public async Task<ListPaymentPageResponse?> GetFilteredPaymentPagesListAsync(int pageNumber, int pageSize, Dictionary<string, string> filters = null, List< PCIShieldLib.SharedKernel.Interfaces.Sort> sorting = null)
    {
        try
        {
            if (sorting == null && filters == null)
            {
                return await GetPagedPaymentPagesListAsync(pageNumber, pageSize);
            }
           if (sorting == null && filters.Count == 1 && filters.TryGetValue("search", out string? filter))
            {
                var searchResults = await SearchPaymentPagesAsync(filter);
                return new ListPaymentPageResponse { PaymentPages = searchResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList(), Count = searchResults.Count };
            }
            string uri = $"api/paymentPages/filtered_list";
            string apiBaseUrl = await RetrieveApiBaseUrl();
            var request = new FilteredPaymentPageRequest
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Filters = filters,
                Sorting = sorting,
            };
            IFlurlResponse? response = await _flurlClient
                .Request($"{apiBaseUrl}{uri}")
                .AllowAnyHttpStatus()
                .PostJsonAsync(request);
            string responseString = await response.ResponseMessage.Content.ReadAsStringAsync();
            if (!response.ResponseMessage.IsSuccessStatusCode)
            {
                _logger.LogError($"Server returned non-success status code: {response.StatusCode}. Response content: {responseString}");
                throw new FetchDataException($"Server returned non-success status code: {response.StatusCode}");
            }
            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            return JsonSerializer.Deserialize<ListPaymentPageResponse>(responseString, settings);
        }
        catch (FlurlHttpTimeoutException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Flurl HTTP error occurred. Status code: {StatusCode}, Response: {Response}", statusCode, responseBody);
            throw new FetchDataException($"An HTTP error occurred. Status code: {statusCode}", ex);
        }
        catch (FlurlHttpException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Flurl HTTP error occurred. Status code: {StatusCode}, Response: {Response}", statusCode, responseBody);
            throw new FetchDataException($"An HTTP error occurred. Status code: {statusCode}", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while fetching filtered paymentPage list.");
            throw new FetchDataException("An unexpected error occurred while fetching filtered paymentPage list.", ex);
        }
    }
    public async Task<ListPaymentPageResponse?> GetPagedPaymentPagesListAsync(int pageNumber, int pageSize)
    {
        string uri = $"api/paymentPages/paged_list";
        try
        {
            string apiBaseUrl = await RetrieveApiBaseUrl();
            GetPaymentPageListRequest request = new() { PageNumber = pageNumber, PageSize = pageSize };
            IFlurlResponse? response = await _flurlClient
                .Request($"{apiBaseUrl}{uri}")
                .AllowAnyHttpStatus()
                .PostJsonAsync(request);
            string responseString = await response.ResponseMessage.Content.ReadAsStringAsync();
            if (!response.ResponseMessage.IsSuccessStatusCode)
            {
                _logger.LogError($"Server returned non-success status code: {response.StatusCode}. Response content: {responseString}");
                throw new FetchDataException($"Server returned non-success status code: {response.StatusCode}");
            }
            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            ListPaymentPageResponse? responseDto = JsonSerializer.Deserialize<ListPaymentPageResponse>(responseString, settings);
            if (responseDto?.PaymentPages != null) { }
            return responseDto;
        }
        catch (FlurlHttpTimeoutException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Flurl HTTP error occurred. Status code: {StatusCode}, Response: {Response}", statusCode, responseBody);
            throw new FetchDataException($"An HTTP error occurred. Status code: {statusCode}", ex);
        }
        catch (FlurlHttpException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Flurl HTTP error occurred. Status code: {StatusCode}, Response: {Response}", statusCode, responseBody);
            throw new FetchDataException($"An HTTP error occurred. Status code: {statusCode}", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while fetching paged paymentPage list.");
            throw new FetchDataException("An unexpected error occurred while fetching paged paymentPage list.", ex);
        }
    }

    public async Task<List<PaymentPageDto>> SearchPaymentPagesAsync(string searchTerm)
    {
        string uri = $"api/paymentPages/search";
        try
        {
            string apiBaseUrl = await RetrieveApiBaseUrl();
            var url = new StringBuilder($"{apiBaseUrl}{uri}").Append("?SearchTerm=").Append(HttpUtility.UrlEncode(searchTerm));
            IFlurlResponse response = await _flurlClient.Request(url.ToString()).WithTimeout(TimeSpan.FromSeconds(30)).AllowAnyHttpStatus().GetAsync();
            string responseString = await response.ResponseMessage.Content.ReadAsStringAsync();
            if (!response.ResponseMessage.IsSuccessStatusCode)
            {
                _logger.LogError($"Server returned non-success status code: {response.StatusCode}. Response content: {responseString}");
                if (response.StatusCode == 404)
                {
                    return new List<PaymentPageDto>();
                }
                throw new FetchDataException($"Server returned non-success status code: {response.StatusCode}");
            }
            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            List<PaymentPageDto>? paymentPages = JsonSerializer.Deserialize<List<PaymentPageDto>>(responseString, settings);
            return paymentPages ?? new List<PaymentPageDto>();
        }
        catch (FlurlHttpTimeoutException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Timeout occurred while searching paymentPages. Status code: {StatusCode}, Response: {Response}", statusCode, responseBody);
            throw new FetchDataException($"Search request timed out. Status code: {statusCode}", ex);
        }
        catch (FlurlHttpException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "HTTP error occurred while searching paymentPages. Status code: {StatusCode}, Response: {Response}", statusCode, responseBody);
            throw new FetchDataException($"An HTTP error occurred while searching. Status code: {statusCode}", ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error deserializing paymentPage search response");
            throw new FetchDataException("Error processing search results", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while searching paymentPages");
            throw new FetchDataException("An unexpected error occurred while searching paymentPages", ex);
        }
    }
    public async Task<ListROCPackageResponse?> GetFilteredROCPackagesListAsync(
        Guid assessmentId,
        int pageNumber,
        int pageSize,
        Dictionary<string, string> filters = null,
        List<PCIShieldLib.SharedKernel.Interfaces.Sort> sorting = null)
    {
        try
        {
            if (sorting == null && filters == null)
            {
                return await GetPagedROCPackagesListAsync(
                    pageNumber, 
                    pageSize, 
                    assessmentId
                    );
            }
           if (sorting == null && filters.Count == 1 && filters.TryGetValue("search", out string? filter))
            {
                var searchResults = await SearchROCPackagesAsync(filter);
                return new ListROCPackageResponse { ROCPackages = searchResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList(), Count = searchResults.Count };
            }
            string uri = $"api/rocpackages/filtered_list";
            string apiBaseUrl = await RetrieveApiBaseUrl();
            var request = new FilteredROCPackageRequest
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Filters = filters,
                Sorting = sorting,
            };
            IFlurlResponse? response = await _flurlClient
                .Request($"{apiBaseUrl}{uri}")
                .AllowAnyHttpStatus()
                .PostJsonAsync(request);
            string responseString = await response.ResponseMessage.Content.ReadAsStringAsync();
            if (!response.ResponseMessage.IsSuccessStatusCode)
            {
                _logger.LogError($"Server returned non-success status code: {response.StatusCode}. Response content: {responseString}");
                throw new FetchDataException($"Server returned non-success status code: {response.StatusCode}");
            }
            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            return JsonSerializer.Deserialize<ListROCPackageResponse>(responseString, settings);
        }
        catch (FlurlHttpTimeoutException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Flurl HTTP error occurred. Status code: {StatusCode}, Response: {Response}", statusCode, responseBody);
            throw new FetchDataException($"An HTTP error occurred. Status code: {statusCode}", ex);
        }
        catch (FlurlHttpException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Flurl HTTP error occurred. Status code: {StatusCode}, Response: {Response}", statusCode, responseBody);
            throw new FetchDataException($"An HTTP error occurred. Status code: {statusCode}", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while fetching filtered rocpackage list.");
            throw new FetchDataException("An unexpected error occurred while fetching filtered rocpackage list.", ex);
        }
    }
    public async Task<ListROCPackageResponse?> GetPagedROCPackagesListAsync(

        int pageNumber, 
        int pageSize,
        Guid assessmentId

        )
    {
        string uri = $"api/rocpackages/paged_list";
        try
        {
            string apiBaseUrl = await RetrieveApiBaseUrl();
            GetROCPackageListRequest request = new() {

                PageNumber = pageNumber,
                PageSize = pageSize,
                AssessmentId = assessmentId

            };
            IFlurlResponse? response = await _flurlClient
                .Request($"{apiBaseUrl}{uri}")
                .AllowAnyHttpStatus()
                .PostJsonAsync(request);
            string responseString = await response.ResponseMessage.Content.ReadAsStringAsync();
            if (!response.ResponseMessage.IsSuccessStatusCode)
            {
                _logger.LogError($"Server returned non-success status code: {response.StatusCode}. Response content: {responseString}");
                throw new FetchDataException($"Server returned non-success status code: {response.StatusCode}");
            }
            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            ListROCPackageResponse? responseDto = JsonSerializer.Deserialize<ListROCPackageResponse>(responseString, settings);
            if (responseDto?.ROCPackages != null) { }
            return responseDto;
        }
        catch (FlurlHttpTimeoutException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Flurl HTTP error occurred. Status code: {StatusCode}, Response: {Response}", statusCode, responseBody);
            throw new FetchDataException($"An HTTP error occurred. Status code: {statusCode}", ex);
        }
        catch (FlurlHttpException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Flurl HTTP error occurred. Status code: {StatusCode}, Response: {Response}", statusCode, responseBody);
            throw new FetchDataException($"An HTTP error occurred. Status code: {statusCode}", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while fetching paged rocpackage list.");
            throw new FetchDataException("An unexpected error occurred while fetching paged rocpackage list.", ex);
        }
    }
    public async Task<List<ROCPackageDto>> SearchROCPackagesAsync(string searchTerm)
    {
        string uri = $"api/rocpackages/search";
        try
        {
            string apiBaseUrl = await RetrieveApiBaseUrl();
            var url = new StringBuilder($"{apiBaseUrl}{uri}").Append("?SearchTerm=").Append(HttpUtility.UrlEncode(searchTerm));
            IFlurlResponse response = await _flurlClient.Request(url.ToString()).WithTimeout(TimeSpan.FromSeconds(30)).AllowAnyHttpStatus().GetAsync();
            string responseString = await response.ResponseMessage.Content.ReadAsStringAsync();
            if (!response.ResponseMessage.IsSuccessStatusCode)
            {
                _logger.LogError($"Server returned non-success status code: {response.StatusCode}. Response content: {responseString}");
                if (response.StatusCode == 404)
                {
                    return new List<ROCPackageDto>();
                }
                throw new FetchDataException($"Server returned non-success status code: {response.StatusCode}");
            }
            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            List<ROCPackageDto>? rocpackages = JsonSerializer.Deserialize<List<ROCPackageDto>>(responseString, settings);
            return rocpackages ?? new List<ROCPackageDto>();
        }
        catch (FlurlHttpTimeoutException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Timeout occurred while searching rocpackages. Status code: {StatusCode}, Response: {Response}", statusCode, responseBody);
            throw new FetchDataException($"Search request timed out. Status code: {statusCode}", ex);
        }
        catch (FlurlHttpException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "HTTP error occurred while searching rocpackages. Status code: {StatusCode}, Response: {Response}", statusCode, responseBody);
            throw new FetchDataException($"An HTTP error occurred while searching. Status code: {statusCode}", ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error deserializing rocpackage search response");
            throw new FetchDataException("Error processing search results", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while searching rocpackages");
            throw new FetchDataException("An unexpected error occurred while searching rocpackages", ex);
        }
    }
    public async Task<ListScanScheduleResponse?> GetFilteredScanSchedulesListAsync(
        Guid assetId,
        int pageNumber,
        int pageSize,
        Dictionary<string, string> filters = null,
        List<PCIShieldLib.SharedKernel.Interfaces.Sort> sorting = null)
    {
        try
        {
            if (sorting == null && filters == null)
            {
                return await GetPagedScanSchedulesListAsync(
                    pageNumber, 
                    pageSize, 
                    assetId
                    );
            }
           if (sorting == null && filters.Count == 1 && filters.TryGetValue("search", out string? filter))
            {
                var searchResults = await SearchScanSchedulesAsync(filter);
                return new ListScanScheduleResponse { ScanSchedules = searchResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList(), Count = searchResults.Count };
            }
            string uri = $"api/scanSchedules/filtered_list";
            string apiBaseUrl = await RetrieveApiBaseUrl();
            var request = new FilteredScanScheduleRequest
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Filters = filters,
                Sorting = sorting,
            };
            IFlurlResponse? response = await _flurlClient
                .Request($"{apiBaseUrl}{uri}")
                .AllowAnyHttpStatus()
                .PostJsonAsync(request);
            string responseString = await response.ResponseMessage.Content.ReadAsStringAsync();
            if (!response.ResponseMessage.IsSuccessStatusCode)
            {
                _logger.LogError($"Server returned non-success status code: {response.StatusCode}. Response content: {responseString}");
                throw new FetchDataException($"Server returned non-success status code: {response.StatusCode}");
            }
            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            return JsonSerializer.Deserialize<ListScanScheduleResponse>(responseString, settings);
        }
        catch (FlurlHttpTimeoutException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Flurl HTTP error occurred. Status code: {StatusCode}, Response: {Response}", statusCode, responseBody);
            throw new FetchDataException($"An HTTP error occurred. Status code: {statusCode}", ex);
        }
        catch (FlurlHttpException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Flurl HTTP error occurred. Status code: {StatusCode}, Response: {Response}", statusCode, responseBody);
            throw new FetchDataException($"An HTTP error occurred. Status code: {statusCode}", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while fetching filtered scanSchedule list.");
            throw new FetchDataException("An unexpected error occurred while fetching filtered scanSchedule list.", ex);
        }
    }
    public async Task<ListScanScheduleResponse?> GetPagedScanSchedulesListAsync(

        int pageNumber, 
        int pageSize,
        Guid assetId

        )
    {
        string uri = $"api/scanSchedules/paged_list";
        try
        {
            string apiBaseUrl = await RetrieveApiBaseUrl();
            GetScanScheduleListRequest request = new() {

                PageNumber = pageNumber,
                PageSize = pageSize,
                AssetId = assetId

            };
            IFlurlResponse? response = await _flurlClient
                .Request($"{apiBaseUrl}{uri}")
                .AllowAnyHttpStatus()
                .PostJsonAsync(request);
            string responseString = await response.ResponseMessage.Content.ReadAsStringAsync();
            if (!response.ResponseMessage.IsSuccessStatusCode)
            {
                _logger.LogError($"Server returned non-success status code: {response.StatusCode}. Response content: {responseString}");
                throw new FetchDataException($"Server returned non-success status code: {response.StatusCode}");
            }
            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            ListScanScheduleResponse? responseDto = JsonSerializer.Deserialize<ListScanScheduleResponse>(responseString, settings);
            if (responseDto?.ScanSchedules != null) { }
            return responseDto;
        }
        catch (FlurlHttpTimeoutException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Flurl HTTP error occurred. Status code: {StatusCode}, Response: {Response}", statusCode, responseBody);
            throw new FetchDataException($"An HTTP error occurred. Status code: {statusCode}", ex);
        }
        catch (FlurlHttpException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Flurl HTTP error occurred. Status code: {StatusCode}, Response: {Response}", statusCode, responseBody);
            throw new FetchDataException($"An HTTP error occurred. Status code: {statusCode}", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while fetching paged scanSchedule list.");
            throw new FetchDataException("An unexpected error occurred while fetching paged scanSchedule list.", ex);
        }
    }
    public async Task<List<ScanScheduleDto>> SearchScanSchedulesAsync(string searchTerm)
    {
        string uri = $"api/scanSchedules/search";
        try
        {
            string apiBaseUrl = await RetrieveApiBaseUrl();
            var url = new StringBuilder($"{apiBaseUrl}{uri}").Append("?SearchTerm=").Append(HttpUtility.UrlEncode(searchTerm));
            IFlurlResponse response = await _flurlClient.Request(url.ToString()).WithTimeout(TimeSpan.FromSeconds(30)).AllowAnyHttpStatus().GetAsync();
            string responseString = await response.ResponseMessage.Content.ReadAsStringAsync();
            if (!response.ResponseMessage.IsSuccessStatusCode)
            {
                _logger.LogError($"Server returned non-success status code: {response.StatusCode}. Response content: {responseString}");
                if (response.StatusCode == 404)
                {
                    return new List<ScanScheduleDto>();
                }
                throw new FetchDataException($"Server returned non-success status code: {response.StatusCode}");
            }
            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            List<ScanScheduleDto>? scanSchedules = JsonSerializer.Deserialize<List<ScanScheduleDto>>(responseString, settings);
            return scanSchedules ?? new List<ScanScheduleDto>();
        }
        catch (FlurlHttpTimeoutException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Timeout occurred while searching scanSchedules. Status code: {StatusCode}, Response: {Response}", statusCode, responseBody);
            throw new FetchDataException($"Search request timed out. Status code: {statusCode}", ex);
        }
        catch (FlurlHttpException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "HTTP error occurred while searching scanSchedules. Status code: {StatusCode}, Response: {Response}", statusCode, responseBody);
            throw new FetchDataException($"An HTTP error occurred while searching. Status code: {statusCode}", ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error deserializing scanSchedule search response");
            throw new FetchDataException("Error processing search results", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while searching scanSchedules");
            throw new FetchDataException("An unexpected error occurred while searching scanSchedules", ex);
        }
    }
    public async Task<ListVulnerabilityResponse?> GetFilteredVulnerabilitiesListAsync(
        Guid assetId,
        int pageNumber,
        int pageSize,
        Dictionary<string, string> filters = null,
        List<PCIShieldLib.SharedKernel.Interfaces.Sort> sorting = null)
    {
        try
        {
            if (sorting == null && filters == null)
            {
                return await GetPagedVulnerabilitiesListAsync(
                    pageNumber, 
                    pageSize, 
                    assetId
                    );
            }
           if (sorting == null && filters.Count == 1 && filters.TryGetValue("search", out string? filter))
            {
                var searchResults = await SearchVulnerabilitiesAsync(filter);
                return new ListVulnerabilityResponse { Vulnerabilities = searchResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList(), Count = searchResults.Count };
            }
            string uri = $"api/vulnerabilities/filtered_list";
            string apiBaseUrl = await RetrieveApiBaseUrl();
            var request = new FilteredVulnerabilityRequest
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Filters = filters,
                Sorting = sorting,
            };
            IFlurlResponse? response = await _flurlClient
                .Request($"{apiBaseUrl}{uri}")
                .AllowAnyHttpStatus()
                .PostJsonAsync(request);
            string responseString = await response.ResponseMessage.Content.ReadAsStringAsync();
            if (!response.ResponseMessage.IsSuccessStatusCode)
            {
                _logger.LogError($"Server returned non-success status code: {response.StatusCode}. Response content: {responseString}");
                throw new FetchDataException($"Server returned non-success status code: {response.StatusCode}");
            }
            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            return JsonSerializer.Deserialize<ListVulnerabilityResponse>(responseString, settings);
        }
        catch (FlurlHttpTimeoutException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Flurl HTTP error occurred. Status code: {StatusCode}, Response: {Response}", statusCode, responseBody);
            throw new FetchDataException($"An HTTP error occurred. Status code: {statusCode}", ex);
        }
        catch (FlurlHttpException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Flurl HTTP error occurred. Status code: {StatusCode}, Response: {Response}", statusCode, responseBody);
            throw new FetchDataException($"An HTTP error occurred. Status code: {statusCode}", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while fetching filtered vulnerability list.");
            throw new FetchDataException("An unexpected error occurred while fetching filtered vulnerability list.", ex);
        }
    }
    public async Task<ListVulnerabilityResponse?> GetPagedVulnerabilitiesListAsync(

        int pageNumber, 
        int pageSize,
        Guid assetId

        )
    {
        string uri = $"api/vulnerabilities/paged_list";
        try
        {
            string apiBaseUrl = await RetrieveApiBaseUrl();
            GetVulnerabilityListRequest request = new() {

                PageNumber = pageNumber,
                PageSize = pageSize,
                AssetId = assetId

            };
            IFlurlResponse? response = await _flurlClient
                .Request($"{apiBaseUrl}{uri}")
                .AllowAnyHttpStatus()
                .PostJsonAsync(request);
            string responseString = await response.ResponseMessage.Content.ReadAsStringAsync();
            if (!response.ResponseMessage.IsSuccessStatusCode)
            {
                _logger.LogError($"Server returned non-success status code: {response.StatusCode}. Response content: {responseString}");
                throw new FetchDataException($"Server returned non-success status code: {response.StatusCode}");
            }
            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            ListVulnerabilityResponse? responseDto = JsonSerializer.Deserialize<ListVulnerabilityResponse>(responseString, settings);
            if (responseDto?.Vulnerabilities != null) { }
            return responseDto;
        }
        catch (FlurlHttpTimeoutException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Flurl HTTP error occurred. Status code: {StatusCode}, Response: {Response}", statusCode, responseBody);
            throw new FetchDataException($"An HTTP error occurred. Status code: {statusCode}", ex);
        }
        catch (FlurlHttpException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Flurl HTTP error occurred. Status code: {StatusCode}, Response: {Response}", statusCode, responseBody);
            throw new FetchDataException($"An HTTP error occurred. Status code: {statusCode}", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while fetching paged vulnerability list.");
            throw new FetchDataException("An unexpected error occurred while fetching paged vulnerability list.", ex);
        }
    }
    public async Task<List<VulnerabilityDto>> SearchVulnerabilitiesAsync(string searchTerm)
    {
        string uri = $"api/vulnerabilities/search";
        try
        {
            string apiBaseUrl = await RetrieveApiBaseUrl();
            var url = new StringBuilder($"{apiBaseUrl}{uri}").Append("?SearchTerm=").Append(HttpUtility.UrlEncode(searchTerm));
            IFlurlResponse response = await _flurlClient.Request(url.ToString()).WithTimeout(TimeSpan.FromSeconds(30)).AllowAnyHttpStatus().GetAsync();
            string responseString = await response.ResponseMessage.Content.ReadAsStringAsync();
            if (!response.ResponseMessage.IsSuccessStatusCode)
            {
                _logger.LogError($"Server returned non-success status code: {response.StatusCode}. Response content: {responseString}");
                if (response.StatusCode == 404)
                {
                    return new List<VulnerabilityDto>();
                }
                throw new FetchDataException($"Server returned non-success status code: {response.StatusCode}");
            }
            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            List<VulnerabilityDto>? vulnerabilities = JsonSerializer.Deserialize<List<VulnerabilityDto>>(responseString, settings);
            return vulnerabilities ?? new List<VulnerabilityDto>();
        }
        catch (FlurlHttpTimeoutException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Timeout occurred while searching vulnerabilities. Status code: {StatusCode}, Response: {Response}", statusCode, responseBody);
            throw new FetchDataException($"Search request timed out. Status code: {statusCode}", ex);
        }
        catch (FlurlHttpException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "HTTP error occurred while searching vulnerabilities. Status code: {StatusCode}, Response: {Response}", statusCode, responseBody);
            throw new FetchDataException($"An HTTP error occurred while searching. Status code: {statusCode}", ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error deserializing vulnerability search response");
            throw new FetchDataException("Error processing search results", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while searching vulnerabilities");
            throw new FetchDataException("An unexpected error occurred while searching vulnerabilities", ex);
        }
    }
    public async Task<CreateMerchantResponse> CreateMerchantAsync(MerchantDto data)
    {
        string uri = $"api/merchants";
        try
        {
            var request = new CreateMerchantRequest { Merchant = data };
            string apiBaseUrl = await RetrieveApiBaseUrl();
            CreateMerchantResponse response = await _flurlClient.Request($"{apiBaseUrl}{uri}")
                .AllowAnyHttpStatus()
                .PostJsonAsync(request).ReceiveJson<CreateMerchantResponse>();

            var aa = response.Merchant;
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while creating a merchant .", ex);
            throw new FetchDataException("An error occurred while creating data.", ex);
        }
    }

    public async Task<MerchantDto> UpdateMerchantAsync(MerchantDto data)
    {
        string uri = $"api/merchants";
        try
        {

            var request = new UpdateMerchantRequest() { Merchant = data };
            string apiBaseUrl = await RetrieveApiBaseUrl();
            MerchantDto response = await _flurlClient.Request($"{apiBaseUrl}{uri}")
                .AllowAnyHttpStatus()
                .PutJsonAsync(request)
                .ReceiveJson<MerchantDto>();
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while updating a merchant .", ex);
            throw new FetchDataException("An error occurred while updating data.", ex);
        }
    }

    public async Task DeleteMerchantAsync(Guid merchantId)
    {
        string uri = $"api/paymentChannel/merchants/{merchantId}";
        try
        {
            string apiBaseUrl = await RetrieveApiBaseUrl();
            await _flurlClient.Request($"{apiBaseUrl}{uri}")
                .AllowAnyHttpStatus()
                .DeleteAsync()
                .ReceiveJson<DeleteMerchantResponse>();
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while deleting a merchant .", ex);
            throw new FetchDataException("An error occurred while deleting data.", ex);
        }
    }

    public async Task<GetByIdMerchantResponse> GetOneFullMerchantByIdAsync(Guid merchantId)
    {
        try
        {
            string? apiBaseUrl = await RetrieveApiBaseUrl();
            string uri = $"api/merchants/{merchantId}";
            GetByIdMerchantRequest request = new() { MerchantId = merchantId };
            IFlurlResponse? response = await _flurlClient.Request($"{apiBaseUrl}{uri}")
                .AllowAnyHttpStatus()
                .PostJsonAsync(request);

            string responseString = await response.ResponseMessage.Content.ReadAsStringAsync();
            if (!response.ResponseMessage.IsSuccessStatusCode)
            {
                _logger.LogError(
                    $"Server returned non-success status code: {response.StatusCode}. Response content: {responseString}");
                if (response.StatusCode == 404)
                {
                    _logger.LogWarning($"Merchant with {merchantId} not found.");
                    throw new NotFoundException($"Merchant with merchantId  not found.", merchantId.ToString());
                }
                throw new FetchDataException($"Server returned non-success status code: {response.StatusCode}");
            }

            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            GetByIdMerchantResponse? responseDto =
                JsonSerializer.Deserialize<GetByIdMerchantResponse>(responseString, settings);

            if (responseDto?.Merchant == null)
            {
                _logger.LogWarning($"Deserialized response contains no merchant data for ID ");
                throw new NotFoundException($"No merchant data found for merchantId", merchantId.ToString());
            }

            return responseDto;
        }
        catch (FlurlHttpTimeoutException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Timeout occurred while fetching merchant. Status code: {StatusCode}, Response: {Response}",
                statusCode, responseBody);
            throw new FetchDataException($"Request timed out while fetching merchant data for ID ");
        }
        catch (FlurlHttpException ex)
        {
            string responseBody = await ex.GetResponseStringAsync();
            string? errorMsg = BuildFlurlErrorMessage($"An error occurred while fetching merchant ", ex);
            _logger.LogError(ex, responseBody);
            throw new FetchDataException(responseBody, ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex.Message, "Error deserializing merchant response for ID {Merchant)");
            throw new FetchDataException($"Error processing response for merchant ", ex);
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "An unexpected error occurred while fetching merchant {merchantId}");
            throw new FetchDataException($"An unexpected error occurred while fetching merchant ", ex);
        }
    }

    public async Task<ListMerchantResponse?> GetFilteredMerchantsListAsync(int pageNumber, int pageSize, Dictionary<string, string> filters = null, List< PCIShieldLib.SharedKernel.Interfaces.Sort> sorting = null)
    {
        try
        {
            if (sorting == null && filters == null)
            {
                return await GetPagedMerchantsListAsync(pageNumber, pageSize);
            }
           if (sorting == null && filters.Count == 1 && filters.TryGetValue("search", out string? filter))
            {
                var searchResults = await SearchMerchantsAsync(filter);
                return new ListMerchantResponse { Merchants = searchResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList(), Count = searchResults.Count };
            }
            string uri = $"api/merchants/filtered_list";
            string apiBaseUrl = await RetrieveApiBaseUrl();
            var request = new FilteredMerchantRequest
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Filters = filters,
                Sorting = sorting,
            };
            IFlurlResponse? response = await _flurlClient
                .Request($"{apiBaseUrl}{uri}")
                .AllowAnyHttpStatus()
                .PostJsonAsync(request);
            string responseString = await response.ResponseMessage.Content.ReadAsStringAsync();
            if (!response.ResponseMessage.IsSuccessStatusCode)
            {
                _logger.LogError($"Server returned non-success status code: {response.StatusCode}. Response content: {responseString}");
                throw new FetchDataException($"Server returned non-success status code: {response.StatusCode}");
            }
            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            return JsonSerializer.Deserialize<ListMerchantResponse>(responseString, settings);
        }
        catch (FlurlHttpTimeoutException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Flurl HTTP error occurred. Status code: {StatusCode}, Response: {Response}", statusCode, responseBody);
            throw new FetchDataException($"An HTTP error occurred. Status code: {statusCode}", ex);
        }
        catch (FlurlHttpException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Flurl HTTP error occurred. Status code: {StatusCode}, Response: {Response}", statusCode, responseBody);
            throw new FetchDataException($"An HTTP error occurred. Status code: {statusCode}", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while fetching filtered merchant list.");
            throw new FetchDataException("An unexpected error occurred while fetching filtered merchant list.", ex);
        }
    }
    public async Task<ListMerchantResponse?> GetPagedMerchantsListAsync(int pageNumber, int pageSize)
    {
        string uri = $"api/merchants/paged_list";
        try
        {
            string apiBaseUrl = await RetrieveApiBaseUrl();
            GetMerchantListRequest request = new() { PageNumber = pageNumber, PageSize = pageSize };
            IFlurlResponse? response = await _flurlClient
                .Request($"{apiBaseUrl}{uri}")
                .AllowAnyHttpStatus()
                .PostJsonAsync(request);
            string responseString = await response.ResponseMessage.Content.ReadAsStringAsync();
            if (!response.ResponseMessage.IsSuccessStatusCode)
            {
                _logger.LogError($"Server returned non-success status code: {response.StatusCode}. Response content: {responseString}");
                throw new FetchDataException($"Server returned non-success status code: {response.StatusCode}");
            }
            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            ListMerchantResponse? responseDto = JsonSerializer.Deserialize<ListMerchantResponse>(responseString, settings);
            if (responseDto?.Merchants != null) { }
            return responseDto;
        }
        catch (FlurlHttpTimeoutException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Flurl HTTP error occurred. Status code: {StatusCode}, Response: {Response}", statusCode, responseBody);
            throw new FetchDataException($"An HTTP error occurred. Status code: {statusCode}", ex);
        }
        catch (FlurlHttpException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Flurl HTTP error occurred. Status code: {StatusCode}, Response: {Response}", statusCode, responseBody);
            throw new FetchDataException($"An HTTP error occurred. Status code: {statusCode}", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while fetching paged merchant list.");
            throw new FetchDataException("An unexpected error occurred while fetching paged merchant list.", ex);
        }
    }

    public async Task<List<MerchantDto>> SearchMerchantsAsync(string searchTerm)
    {
        string uri = $"api/merchants/search";
        try
        {
            string apiBaseUrl = await RetrieveApiBaseUrl();
            var url = new StringBuilder($"{apiBaseUrl}{uri}").Append("?SearchTerm=").Append(HttpUtility.UrlEncode(searchTerm));
            IFlurlResponse response = await _flurlClient.Request(url.ToString()).WithTimeout(TimeSpan.FromSeconds(30)).AllowAnyHttpStatus().GetAsync();
            string responseString = await response.ResponseMessage.Content.ReadAsStringAsync();
            if (!response.ResponseMessage.IsSuccessStatusCode)
            {
                _logger.LogError($"Server returned non-success status code: {response.StatusCode}. Response content: {responseString}");
                if (response.StatusCode == 404)
                {
                    return new List<MerchantDto>();
                }
                throw new FetchDataException($"Server returned non-success status code: {response.StatusCode}");
            }
            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            List<MerchantDto>? merchants = JsonSerializer.Deserialize<List<MerchantDto>>(responseString, settings);
            return merchants ?? new List<MerchantDto>();
        }
        catch (FlurlHttpTimeoutException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Timeout occurred while searching merchants. Status code: {StatusCode}, Response: {Response}", statusCode, responseBody);
            throw new FetchDataException($"Search request timed out. Status code: {statusCode}", ex);
        }
        catch (FlurlHttpException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "HTTP error occurred while searching merchants. Status code: {StatusCode}, Response: {Response}", statusCode, responseBody);
            throw new FetchDataException($"An HTTP error occurred while searching. Status code: {statusCode}", ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error deserializing merchant search response");
            throw new FetchDataException("Error processing search results", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while searching merchants");
            throw new FetchDataException("An unexpected error occurred while searching merchants", ex);
        }
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
  	  
    private async Task<string> RetrieveApiBaseUrl()
    {
        string thisBaseUrl = "https://localhost:52509/";
        if (DeviceInfo.Platform == DevicePlatform.Android)
        {
            thisBaseUrl = thisBaseUrl.Replace("localhost", "10.0.2.2");
        }
  	  
        return thisBaseUrl;
    }
}

