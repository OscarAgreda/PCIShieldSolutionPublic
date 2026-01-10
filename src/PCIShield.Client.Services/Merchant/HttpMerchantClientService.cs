using System.Net.Http.Headers;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text.Json;
using System.Text;
using System.Web;
using Ardalis.GuardClauses;
using BlazorMauiShared.Models.Merchant;
using BlazorMauiShared.Models.Assessment;
using BlazorMauiShared.Models.Asset;
using BlazorMauiShared.Models.CompensatingControl;
using BlazorMauiShared.Models.ComplianceOfficer;
using BlazorMauiShared.Models.CryptographicInventory;
using BlazorMauiShared.Models.Evidence;
using BlazorMauiShared.Models.NetworkSegmentation;
using BlazorMauiShared.Models.PaymentChannel;
using BlazorMauiShared.Models.ServiceProvider;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.BlazorMauiShared.ModelsDto;
using PCIShield.Client.Services.Common;

using PCIShield.Domain.ModelsDto;
using PCIShield.BlazorMauiShared.Models.Assessment;
using PCIShield.BlazorMauiShared.Models.Asset;
using PCIShield.BlazorMauiShared.Models.CompensatingControl;
using PCIShield.BlazorMauiShared.Models.ComplianceOfficer;
using PCIShield.BlazorMauiShared.Models.CryptographicInventory;
using PCIShield.BlazorMauiShared.Models.Evidence;
using PCIShield.BlazorMauiShared.Models.NetworkSegmentation;
using PCIShield.BlazorMauiShared.Models.PaymentChannel;
using PCIShield.BlazorMauiShared.Models.ServiceProvider;
using Flurl.Http;
using Flurl.Http.Configuration;
using Microsoft.Maui.Devices;
using ReactiveUI;
using JsonSerializer = System.Text.Json.JsonSerializer;
using JsonSerializerOptions = System.Text.Json.JsonSerializerOptions;
using Unit = System.Reactive.Unit;
using PCIShield.Client.Services.InvoiceSession;
using PCIShield.BlazorMauiShared.Models.Merchant;
using BlazorMauiShared.Models.Control;

namespace PCIShield.Client.Services.Merchant;
  	  
public class HttpMerchantClientService : IDisposable, IHttpMerchantClientService
{
    private readonly CompositeDisposable _disposables = new();
    private readonly IAppLoggerService<HttpMerchantClientService> _logger;
    private readonly ITokenService _tokenService;
    private FlurlClient _flurlClient;
    private HttpClient _httpClient;
  	  
    public HttpMerchantClientService(
        IAppLoggerService<HttpMerchantClientService> logger,
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

    public async Task<UpdateMerchantResponse> UpdatePutMerchantAsync(UpdateMerchantRequest data)
    {
        string? uri = "api/merchants";
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
            UpdateMerchantResponse? responseDto =
                JsonSerializer.Deserialize<UpdateMerchantResponse>(responseString, settings);

            return responseDto;
        }
        catch (FlurlHttpTimeoutException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Timeout occurred while updating merchant. Status code: {StatusCode}, Response: {Response}",
                statusCode, responseBody);
            throw new FetchDataException($"Update request timed out. Status code: {statusCode}", ex);
        }
        catch (FlurlHttpException ex)
        {
            string responseBody = await ex.GetResponseStringAsync();
            string? errorMsg = BuildFlurlErrorMessage("An error occurred while updating merchant", ex);
            _logger.LogError(ex, responseBody);
            throw new FetchDataException(responseBody, ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error deserializing merchant update response");
            throw new FetchDataException("Error processing update response", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError("An unexpected error occurred while updating merchant.", ex);
            throw new FetchDataException("An unexpected error occurred while updating merchant.", ex);
        }
    }
    public async Task<CreateMerchantResponse> CreatePostMerchantAsync(CreateMerchantRequest data)
    {
        string? uri = "api/merchants";
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
            CreateMerchantResponse? responseDto =
                JsonSerializer.Deserialize<CreateMerchantResponse>(responseString, settings);

            return responseDto;
        }
        catch (FlurlHttpTimeoutException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Timeout occurred while creating merchant. Status code: {StatusCode}, Response: {Response}",
                statusCode, responseBody);
            HandleHttpServiceError(ex, ErrorHandlingService.ClientOperationContext.Create);
            throw new FetchDataException($"Create request timed out. Status code: {statusCode}", ex);
        }
        catch (FlurlHttpException ex)
        {
            string responseBody = await ex.GetResponseStringAsync();
            string? errorMsg = BuildFlurlErrorMessage("An error occurred while creating merchant", ex);
            _logger.LogError(ex, responseBody);
            throw new FetchDataException(responseBody, ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error deserializing merchant creation response");
            throw new FetchDataException("Error processing creation response", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError("An unexpected error occurred while creating merchant.", ex);
            throw new FetchDataException("An unexpected error occurred while creating merchant.", ex);
        }
    }
    public async Task<GetByIdMerchantResponse> GetLastCreatedMerchantAsync()
    {
        string uri = "api/merchants/last-created";
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
            return JsonSerializer.Deserialize<GetByIdMerchantResponse>(responseString, settings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving last created merchant");
            throw new FetchDataException("Error retrieving last created merchant", ex);
        }
    }
    public async Task<DeleteMerchantResponse> DeleteMerchantAsync(Guid merchantId)
    {
        string? uri = $"api/merchants/{merchantId}";
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
            DeleteMerchantResponse? responseDto =
                JsonSerializer.Deserialize<DeleteMerchantResponse>(responseString, settings);

            return responseDto;
        }
        catch (FlurlHttpTimeoutException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Timeout occurred while deleting merchant. Status code: {StatusCode}, Response: {Response}",
                statusCode, responseBody);
            throw new FetchDataException($"Delete request timed out. Status code: {statusCode}", ex);
        }
        catch (FlurlHttpException ex)
        {
            string responseBody = await ex.GetResponseStringAsync();
            string? errorMsg = BuildFlurlErrorMessage("An error occurred while deleting merchant", ex);
            _logger.LogError(ex, responseBody);
            throw new FetchDataException(responseBody, ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error deserializing merchant deletion response");
            throw new FetchDataException("Error processing deletion response", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError("An unexpected error occurred while deleting merchant.", ex);
            throw new FetchDataException("An unexpected error occurred while deleting merchant.", ex);
        }
    }
    public async Task<ListMerchantResponse?> GetFilteredMerchantListAsync(int pageNumber, int pageSize, Dictionary<string, string> filters = null, List<PCIShieldLib.SharedKernel.Interfaces.Sort> sorting = null)
    {
        try
        {
            if (sorting == null && filters == null)
            {
                return await GetPagedMerchantsListAsync(pageNumber, pageSize);
            }
           if (sorting == null && filters.Count == 1 && filters.TryGetValue("search", out string? filter))
            {
                var searchResults = await SearchMerchantByMerchantAsync(filter);
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
            IFlurlResponse? response = await _flurlClient.Request($"{apiBaseUrl}{uri}").AllowAnyHttpStatus().PostJsonAsync(request);
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
            GetMerchantListRequest request = new() { PageNumber = pageNumber, PageSize = pageSize };
            IFlurlRequest flurlRequest = await CreateAuthenticatedRequest(uri);
            IFlurlResponse? response = await flurlRequest
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
    public async Task<List<MerchantDto>> SearchMerchantByMerchantAsync(string searchTerm)
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

    public async Task<GetByIdMerchantResponse> GetOneFullMerchantByIdAsync(Guid merchantId, bool withPostGraph = false)
    {
        string? uri = $"api/merchants/i/{merchantId}";
        try
        {
            string? apiBaseUrl = await RetrieveApiBaseUrl();
            GetByIdMerchantRequest request = new() { MerchantId = merchantId, WithPostGraph = withPostGraph };
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
                    _logger.LogWarning($"Merchant with merchantId {merchantId} not found.");
                    throw new NotFoundException($"Merchant with merchantId {merchantId} not found.", merchantId.ToString());
                }
                throw new FetchDataException($"Server returned non-success status code: {response.StatusCode}");
            }

            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            GetByIdMerchantResponse? responseDto =
                JsonSerializer.Deserialize<GetByIdMerchantResponse>(responseString, settings);

            if (responseDto?.Merchant == null)
            {
                _logger.LogWarning($"Deserialized response contains no merchant data for ID {merchantId}");
                throw new NotFoundException($"No merchant data found for merchantId {merchantId}", merchantId.ToString());
            }

            return responseDto;
        }
        catch (FlurlHttpTimeoutException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Timeout occurred while fetching merchant. Status code: {StatusCode}, Response: {Response}",
                statusCode, responseBody);
            throw new FetchDataException($"Request timed out while fetching merchant {merchantId}. Status code: {statusCode}", ex);
        }
        catch (FlurlHttpException ex)
        {
            string responseBody = await ex.GetResponseStringAsync();
            string? errorMsg = BuildFlurlErrorMessage($"An error occurred while fetching merchant {merchantId}", ex);
            _logger.LogError(ex, responseBody);
            throw new FetchDataException(responseBody, ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex.Message, "Error deserializing merchant response for ID {MerchantId}", merchantId);
            throw new FetchDataException($"Error processing response for merchant {merchantId}", ex);
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "An unexpected error occurred while fetching merchant {MerchantId}", merchantId);
            throw new FetchDataException($"An unexpected error occurred while fetching merchant {merchantId}", ex);
        }
    }
    public async Task<SpecificationPerformanceResponse<MerchantDto>> RunMerchantPerformanceTests(Guid merchantId)
    {
        string? uri = $"api/merchants/perf/{merchantId}";
        try
        {
            string? apiBaseUrl = await RetrieveApiBaseUrl();
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
                    _logger.LogWarning($"Merchant with merchantId {merchantId} not found.");
                    throw new NotFoundException($"Merchant with merchantId {merchantId} not found.", merchantId.ToString());
                }
                throw new FetchDataException($"Server returned non-success status code: {response.StatusCode}");
            }

            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            SpecificationPerformanceResponse<MerchantDto>? responseDto =
                JsonSerializer.Deserialize<SpecificationPerformanceResponse<MerchantDto>>(responseString, settings);

            return responseDto;
        }
        catch (FlurlHttpTimeoutException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Timeout occurred while PerformanceTests merchant. Status code: {StatusCode}, Response: {Response}",
                statusCode, responseBody);
            throw new FetchDataException($"Request timed out while PerformanceTests merchant {merchantId}. Status code: {statusCode}", ex);
        }
        catch (FlurlHttpException ex)
        {
            string responseBody = await ex.GetResponseStringAsync();
            string? errorMsg = BuildFlurlErrorMessage($"An error occurred while PerformanceTests merchant {merchantId}", ex);
            _logger.LogError(ex, responseBody);
            throw new FetchDataException(responseBody, ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex.Message, "Error deserializing merchant response for ID {MerchantId}", merchantId);
            throw new FetchDataException($"Error processing response for merchant {merchantId}", ex);
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "An unexpected error occurred while fetching merchant {MerchantId}", merchantId);
            throw new FetchDataException($"An unexpected error occurred while fetching merchant {merchantId}", ex);
        }
    }
    public async Task<CreateAssessmentResponse> CreateAssessmentAsync(AssessmentDto data)
    {
        string uri = $"api/assessments";
        try
        {
            var request = new CreateAssessmentRequest { Assessment = data };
            string apiBaseUrl = await RetrieveApiBaseUrl();
            CreateAssessmentResponse response = await _flurlClient.Request($"{apiBaseUrl}{uri}")
                .AllowAnyHttpStatus()
                .PostJsonAsync(request).ReceiveJson<CreateAssessmentResponse>();

            var aa = response.Assessment;
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while creating a assessment .", ex);
            throw new FetchDataException("An error occurred while creating data.", ex);
        }
    }

    public async Task<AssessmentDto> UpdateAssessmentAsync(AssessmentDto data)
    {
        string uri = $"api/assessments";
        try
        {

            var request = new UpdateAssessmentRequest() { Assessment = data };
            string apiBaseUrl = await RetrieveApiBaseUrl();
            AssessmentDto response = await _flurlClient.Request($"{apiBaseUrl}{uri}")
                .AllowAnyHttpStatus()
                .PutJsonAsync(request)
                .ReceiveJson<AssessmentDto>();
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while updating a assessment .", ex);
            throw new FetchDataException("An error occurred while updating data.", ex);
        }
    }

    public async Task DeleteAssessmentAsync(Guid assessmentId)
    {
        string uri = $"api/merchant/assessments/{assessmentId}";
        try
        {
            string apiBaseUrl = await RetrieveApiBaseUrl();
            await _flurlClient.Request($"{apiBaseUrl}{uri}")
                .AllowAnyHttpStatus()
                .DeleteAsync()
                .ReceiveJson<DeleteAssessmentResponse>();
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while deleting a assessment .", ex);
            throw new FetchDataException("An error occurred while deleting data.", ex);
        }
    }

    public async Task<GetByIdAssessmentResponse> GetOneFullAssessmentByIdAsync(Guid assessmentId)
    {
        try
        {
            string? apiBaseUrl = await RetrieveApiBaseUrl();
            string uri = $"api/merchant/assessments/{assessmentId}";
             GetByIdAssessmentRequest request = new() { AssessmentId = assessmentId };
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
                    _logger.LogWarning($"Assessment with assessment not found.");
                    throw new NotFoundException($"Assessment assessmentId not found.", assessmentId.ToString());
                }
                throw new FetchDataException($"Server returned non-success status code: {response.StatusCode}");
            }

            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            GetByIdAssessmentResponse? responseDto =
                JsonSerializer.Deserialize<GetByIdAssessmentResponse>(responseString, settings);

            if (responseDto?.Assessment == null)
            {
                _logger.LogWarning($"Deserialized response contains no assessment data for ID ");
                throw new NotFoundException($"No assessment data found for assessmentId", assessmentId.ToString());
            }

            return responseDto;
        }
        catch (FlurlHttpTimeoutException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Timeout occurred while fetching assessment. Status code: {StatusCode}, Response: {Response}",
                statusCode, responseBody);
            throw new FetchDataException($"Request timed out while fetching assessment data for ID ");
        }
        catch (FlurlHttpException ex)
        {
            string responseBody = await ex.GetResponseStringAsync();
            string? errorMsg = BuildFlurlErrorMessage($"An error occurred while fetching assessment ", ex);
            _logger.LogError(ex, responseBody);
            throw new FetchDataException(responseBody, ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex.Message, "Error deserializing assessment response for ID {AssessmentId}", assessmentId);
            throw new FetchDataException($"Error processing response for assessment {assessmentId}", ex);
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "An unexpected error occurred while fetching assessment Assessment");
            throw new FetchDataException($"An unexpected error occurred while fetching assessment ", ex);
        }
    }

    public async Task<ListAssessmentResponse?> GetFilteredAssessmentsListAsync(int pageNumber, int pageSize, Dictionary<string, string> filters = null, List< PCIShieldLib.SharedKernel.Interfaces.Sort> sorting = null)
    {
        try
        {
            if (sorting == null && filters == null)
            {
                return await GetPagedAssessmentsListAsync(pageNumber, pageSize);
            }
           if (sorting == null && filters.Count == 1 && filters.TryGetValue("search", out string? filter))
            {
                var searchResults = await SearchAssessmentsAsync(filter);
                return new ListAssessmentResponse { Assessments = searchResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList(), Count = searchResults.Count };
            }
            string uri = $"api/assessments/filtered_list";
            string apiBaseUrl = await RetrieveApiBaseUrl();
            var request = new FilteredAssessmentRequest
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
            return JsonSerializer.Deserialize<ListAssessmentResponse>(responseString, settings);
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
            _logger.LogError(ex, "An unexpected error occurred while fetching filtered assessment list.");
            throw new FetchDataException("An unexpected error occurred while fetching filtered assessment list.", ex);
        }
    }
    public async Task<ListAssessmentResponse?> GetPagedAssessmentsListAsync(int pageNumber, int pageSize)
    {
        string uri = $"api/assessments/paged_list";
        try
        {
            string apiBaseUrl = await RetrieveApiBaseUrl();
            GetAssessmentListRequest request = new() { PageNumber = pageNumber, PageSize = pageSize };
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
            ListAssessmentResponse? responseDto = JsonSerializer.Deserialize<ListAssessmentResponse>(responseString, settings);
            if (responseDto?.Assessments != null) { }
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
            _logger.LogError(ex, "An unexpected error occurred while fetching paged assessment list.");
            throw new FetchDataException("An unexpected error occurred while fetching paged assessment list.", ex);
        }
    }

    public async Task<List<AssessmentDto>> SearchAssessmentsAsync(string searchTerm)
    {
        string uri = $"api/assessments/search";
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
                    return new List<AssessmentDto>();
                }
                throw new FetchDataException($"Server returned non-success status code: {response.StatusCode}");
            }
            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            List<AssessmentDto>? assessments = JsonSerializer.Deserialize<List<AssessmentDto>>(responseString, settings);
            return assessments ?? new List<AssessmentDto>();
        }
        catch (FlurlHttpTimeoutException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Timeout occurred while searching assessments. Status code: {StatusCode}, Response: {Response}", statusCode, responseBody);
            throw new FetchDataException($"Search request timed out. Status code: {statusCode}", ex);
        }
        catch (FlurlHttpException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "HTTP error occurred while searching assessments. Status code: {StatusCode}, Response: {Response}", statusCode, responseBody);
            throw new FetchDataException($"An HTTP error occurred while searching. Status code: {statusCode}", ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error deserializing assessment search response");
            throw new FetchDataException("Error processing search results", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while searching assessments");
            throw new FetchDataException("An unexpected error occurred while searching assessments", ex);
        }
    }
 
    public async Task<CreateAssetResponse> CreateAssetAsync(AssetDto data)
    {
        string uri = $"api/assets";
        try
        {
            var request = new CreateAssetRequest { Asset = data };
            string apiBaseUrl = await RetrieveApiBaseUrl();
            CreateAssetResponse response = await _flurlClient.Request($"{apiBaseUrl}{uri}")
                .AllowAnyHttpStatus()
                .PostJsonAsync(request).ReceiveJson<CreateAssetResponse>();

            var aa = response.Asset;
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while creating a asset .", ex);
            throw new FetchDataException("An error occurred while creating data.", ex);
        }
    }

    public async Task<AssetDto> UpdateAssetAsync(AssetDto data)
    {
        string uri = $"api/assets";
        try
        {

            var request = new UpdateAssetRequest() { Asset = data };
            string apiBaseUrl = await RetrieveApiBaseUrl();
            AssetDto response = await _flurlClient.Request($"{apiBaseUrl}{uri}")
                .AllowAnyHttpStatus()
                .PutJsonAsync(request)
                .ReceiveJson<AssetDto>();
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while updating a asset .", ex);
            throw new FetchDataException("An error occurred while updating data.", ex);
        }
    }

    public async Task DeleteAssetAsync(Guid assetId)
    {
        string uri = $"api/merchant/assets/{assetId}";
        try
        {
            string apiBaseUrl = await RetrieveApiBaseUrl();
            await _flurlClient.Request($"{apiBaseUrl}{uri}")
                .AllowAnyHttpStatus()
                .DeleteAsync()
                .ReceiveJson<DeleteAssetResponse>();
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while deleting a asset .", ex);
            throw new FetchDataException("An error occurred while deleting data.", ex);
        }
    }

    public async Task<GetByIdAssetResponse> GetOneFullAssetByIdAsync(Guid assetId)
    {
        try
        {
            string? apiBaseUrl = await RetrieveApiBaseUrl();
            string uri = $"api/merchant/assets/{assetId}";
             GetByIdAssetRequest request = new() { AssetId = assetId };
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
                    _logger.LogWarning($"Asset with asset not found.");
                    throw new NotFoundException($"Asset assetId not found.", assetId.ToString());
                }
                throw new FetchDataException($"Server returned non-success status code: {response.StatusCode}");
            }

            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            GetByIdAssetResponse? responseDto =
                JsonSerializer.Deserialize<GetByIdAssetResponse>(responseString, settings);

            if (responseDto?.Asset == null)
            {
                _logger.LogWarning($"Deserialized response contains no asset data for ID ");
                throw new NotFoundException($"No asset data found for assetId", assetId.ToString());
            }

            return responseDto;
        }
        catch (FlurlHttpTimeoutException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Timeout occurred while fetching asset. Status code: {StatusCode}, Response: {Response}",
                statusCode, responseBody);
            throw new FetchDataException($"Request timed out while fetching asset data for ID ");
        }
        catch (FlurlHttpException ex)
        {
            string responseBody = await ex.GetResponseStringAsync();
            string? errorMsg = BuildFlurlErrorMessage($"An error occurred while fetching asset ", ex);
            _logger.LogError(ex, responseBody);
            throw new FetchDataException(responseBody, ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex.Message, "Error deserializing asset response for ID {AssetId}", assetId);
            throw new FetchDataException($"Error processing response for asset {assetId}", ex);
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "An unexpected error occurred while fetching asset Asset");
            throw new FetchDataException($"An unexpected error occurred while fetching asset ", ex);
        }
    }

    public async Task<ListAssetResponse?> GetFilteredAssetsListAsync(int pageNumber, int pageSize, Dictionary<string, string> filters = null, List< PCIShieldLib.SharedKernel.Interfaces.Sort> sorting = null)
    {
        try
        {
            if (sorting == null && filters == null)
            {
                return await GetPagedAssetsListAsync(pageNumber, pageSize);
            }
           if (sorting == null && filters.Count == 1 && filters.TryGetValue("search", out string? filter))
            {
                var searchResults = await SearchAssetsAsync(filter);
                return new ListAssetResponse { Assets = searchResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList(), Count = searchResults.Count };
            }
            string uri = $"api/assets/filtered_list";
            string apiBaseUrl = await RetrieveApiBaseUrl();
            var request = new FilteredAssetRequest
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
            return JsonSerializer.Deserialize<ListAssetResponse>(responseString, settings);
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
            _logger.LogError(ex, "An unexpected error occurred while fetching filtered asset list.");
            throw new FetchDataException("An unexpected error occurred while fetching filtered asset list.", ex);
        }
    }
    public async Task<ListAssetResponse?> GetPagedAssetsListAsync(int pageNumber, int pageSize)
    {
        string uri = $"api/assets/paged_list";
        try
        {
            string apiBaseUrl = await RetrieveApiBaseUrl();
            GetAssetListRequest request = new() { PageNumber = pageNumber, PageSize = pageSize };
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
            ListAssetResponse? responseDto = JsonSerializer.Deserialize<ListAssetResponse>(responseString, settings);
            if (responseDto?.Assets != null) { }
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
            _logger.LogError(ex, "An unexpected error occurred while fetching paged asset list.");
            throw new FetchDataException("An unexpected error occurred while fetching paged asset list.", ex);
        }
    }

    public async Task<List<AssetDto>> SearchAssetsAsync(string searchTerm)
    {
        string uri = $"api/assets/search";
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
                    return new List<AssetDto>();
                }
                throw new FetchDataException($"Server returned non-success status code: {response.StatusCode}");
            }
            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            List<AssetDto>? assets = JsonSerializer.Deserialize<List<AssetDto>>(responseString, settings);
            return assets ?? new List<AssetDto>();
        }
        catch (FlurlHttpTimeoutException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Timeout occurred while searching assets. Status code: {StatusCode}, Response: {Response}", statusCode, responseBody);
            throw new FetchDataException($"Search request timed out. Status code: {statusCode}", ex);
        }
        catch (FlurlHttpException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "HTTP error occurred while searching assets. Status code: {StatusCode}, Response: {Response}", statusCode, responseBody);
            throw new FetchDataException($"An HTTP error occurred while searching. Status code: {statusCode}", ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error deserializing asset search response");
            throw new FetchDataException("Error processing search results", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while searching assets");
            throw new FetchDataException("An unexpected error occurred while searching assets", ex);
        }
    }
 
    public async Task<CreateCompensatingControlResponse> CreateCompensatingControlAsync(CompensatingControlDto data)
    {
        string uri = $"api/compensatingControls";
        try
        {
            var request = new CreateCompensatingControlRequest { CompensatingControl = data };
            string apiBaseUrl = await RetrieveApiBaseUrl();
            CreateCompensatingControlResponse response = await _flurlClient.Request($"{apiBaseUrl}{uri}")
                .AllowAnyHttpStatus()
                .PostJsonAsync(request).ReceiveJson<CreateCompensatingControlResponse>();

            var aa = response.CompensatingControl;
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while creating a compensatingControl .", ex);
            throw new FetchDataException("An error occurred while creating data.", ex);
        }
    }

    public async Task<CompensatingControlDto> UpdateCompensatingControlAsync(CompensatingControlDto data)
    {
        string uri = $"api/compensatingControls";
        try
        {

            var request = new UpdateCompensatingControlRequest() { CompensatingControl = data };
            string apiBaseUrl = await RetrieveApiBaseUrl();
            CompensatingControlDto response = await _flurlClient.Request($"{apiBaseUrl}{uri}")
                .AllowAnyHttpStatus()
                .PutJsonAsync(request)
                .ReceiveJson<CompensatingControlDto>();
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while updating a compensatingControl .", ex);
            throw new FetchDataException("An error occurred while updating data.", ex);
        }
    }

    public async Task DeleteCompensatingControlAsync(Guid compensatingControlId)
    {
        string uri = $"api/merchant/compensatingControls/{compensatingControlId}";
        try
        {
            string apiBaseUrl = await RetrieveApiBaseUrl();
            await _flurlClient.Request($"{apiBaseUrl}{uri}")
                .AllowAnyHttpStatus()
                .DeleteAsync()
                .ReceiveJson<DeleteCompensatingControlResponse>();
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while deleting a compensatingControl .", ex);
            throw new FetchDataException("An error occurred while deleting data.", ex);
        }
    }

    public async Task<GetByIdCompensatingControlResponse> GetOneFullCompensatingControlByIdAsync(Guid compensatingControlId)
    {
        try
        {
            string? apiBaseUrl = await RetrieveApiBaseUrl();
            string uri = $"api/merchant/compensatingControls/{compensatingControlId}";
             GetByIdCompensatingControlRequest request = new() { CompensatingControlId = compensatingControlId };
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
                    _logger.LogWarning($"CompensatingControl with compensatingControl not found.");
                    throw new NotFoundException($"CompensatingControl compensatingControlId not found.", compensatingControlId.ToString());
                }
                throw new FetchDataException($"Server returned non-success status code: {response.StatusCode}");
            }

            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            GetByIdCompensatingControlResponse? responseDto =
                JsonSerializer.Deserialize<GetByIdCompensatingControlResponse>(responseString, settings);

            if (responseDto?.CompensatingControl == null)
            {
                _logger.LogWarning($"Deserialized response contains no compensatingControl data for ID ");
                throw new NotFoundException($"No compensatingControl data found for compensatingControlId", compensatingControlId.ToString());
            }

            return responseDto;
        }
        catch (FlurlHttpTimeoutException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Timeout occurred while fetching compensatingControl. Status code: {StatusCode}, Response: {Response}",
                statusCode, responseBody);
            throw new FetchDataException($"Request timed out while fetching compensatingControl data for ID ");
        }
        catch (FlurlHttpException ex)
        {
            string responseBody = await ex.GetResponseStringAsync();
            string? errorMsg = BuildFlurlErrorMessage($"An error occurred while fetching compensatingControl ", ex);
            _logger.LogError(ex, responseBody);
            throw new FetchDataException(responseBody, ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex.Message, "Error deserializing compensatingControl response for ID {CompensatingControlId}", compensatingControlId);
            throw new FetchDataException($"Error processing response for compensatingControl {compensatingControlId}", ex);
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "An unexpected error occurred while fetching compensatingControl CompensatingControl");
            throw new FetchDataException($"An unexpected error occurred while fetching compensatingControl ", ex);
        }
    }

    public async Task<ListCompensatingControlResponse?> GetFilteredCompensatingControlsListAsync(int pageNumber, int pageSize, Dictionary<string, string> filters = null, List< PCIShieldLib.SharedKernel.Interfaces.Sort> sorting = null)
    {
        try
        {
            if (sorting == null && filters == null)
            {
                return await GetPagedCompensatingControlsListAsync(pageNumber, pageSize);
            }
           if (sorting == null && filters.Count == 1 && filters.TryGetValue("search", out string? filter))
            {
                var searchResults = await SearchCompensatingControlsAsync(filter);
                return new ListCompensatingControlResponse { CompensatingControls = searchResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList(), Count = searchResults.Count };
            }
            string uri = $"api/compensatingControls/filtered_list";
            string apiBaseUrl = await RetrieveApiBaseUrl();
            var request = new FilteredCompensatingControlRequest
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
            return JsonSerializer.Deserialize<ListCompensatingControlResponse>(responseString, settings);
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
            _logger.LogError(ex, "An unexpected error occurred while fetching filtered compensatingControl list.");
            throw new FetchDataException("An unexpected error occurred while fetching filtered compensatingControl list.", ex);
        }
    }
    public async Task<ListCompensatingControlResponse?> GetPagedCompensatingControlsListAsync(int pageNumber, int pageSize)
    {
        string uri = $"api/compensatingControls/paged_list";
        try
        {
            string apiBaseUrl = await RetrieveApiBaseUrl();
            GetCompensatingControlListRequest request = new() { PageNumber = pageNumber, PageSize = pageSize };
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
            ListCompensatingControlResponse? responseDto = JsonSerializer.Deserialize<ListCompensatingControlResponse>(responseString, settings);
            if (responseDto?.CompensatingControls != null) { }
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
            _logger.LogError(ex, "An unexpected error occurred while fetching paged compensatingControl list.");
            throw new FetchDataException("An unexpected error occurred while fetching paged compensatingControl list.", ex);
        }
    }

    public async Task<List<CompensatingControlDto>> SearchCompensatingControlsAsync(string searchTerm)
    {
        string uri = $"api/compensatingControls/search";
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
                    return new List<CompensatingControlDto>();
                }
                throw new FetchDataException($"Server returned non-success status code: {response.StatusCode}");
            }
            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            List<CompensatingControlDto>? compensatingControls = JsonSerializer.Deserialize<List<CompensatingControlDto>>(responseString, settings);
            return compensatingControls ?? new List<CompensatingControlDto>();
        }
        catch (FlurlHttpTimeoutException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Timeout occurred while searching compensatingControls. Status code: {StatusCode}, Response: {Response}", statusCode, responseBody);
            throw new FetchDataException($"Search request timed out. Status code: {statusCode}", ex);
        }
        catch (FlurlHttpException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "HTTP error occurred while searching compensatingControls. Status code: {StatusCode}, Response: {Response}", statusCode, responseBody);
            throw new FetchDataException($"An HTTP error occurred while searching. Status code: {statusCode}", ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error deserializing compensatingControl search response");
            throw new FetchDataException("Error processing search results", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while searching compensatingControls");
            throw new FetchDataException("An unexpected error occurred while searching compensatingControls", ex);
        }
    }
 
    public async Task<CreateComplianceOfficerResponse> CreateComplianceOfficerAsync(ComplianceOfficerDto data)
    {
        string uri = $"api/complianceOfficers";
        try
        {
            var request = new CreateComplianceOfficerRequest { ComplianceOfficer = data };
            string apiBaseUrl = await RetrieveApiBaseUrl();
            CreateComplianceOfficerResponse response = await _flurlClient.Request($"{apiBaseUrl}{uri}")
                .AllowAnyHttpStatus()
                .PostJsonAsync(request).ReceiveJson<CreateComplianceOfficerResponse>();

            var aa = response.ComplianceOfficer;
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while creating a complianceOfficer .", ex);
            throw new FetchDataException("An error occurred while creating data.", ex);
        }
    }

    public async Task<ComplianceOfficerDto> UpdateComplianceOfficerAsync(ComplianceOfficerDto data)
    {
        string uri = $"api/complianceOfficers";
        try
        {

            var request = new UpdateComplianceOfficerRequest() { ComplianceOfficer = data };
            string apiBaseUrl = await RetrieveApiBaseUrl();
            ComplianceOfficerDto response = await _flurlClient.Request($"{apiBaseUrl}{uri}")
                .AllowAnyHttpStatus()
                .PutJsonAsync(request)
                .ReceiveJson<ComplianceOfficerDto>();
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while updating a complianceOfficer .", ex);
            throw new FetchDataException("An error occurred while updating data.", ex);
        }
    }

    public async Task DeleteComplianceOfficerAsync(Guid complianceOfficerId)
    {
        string uri = $"api/merchant/complianceOfficers/{complianceOfficerId}";
        try
        {
            string apiBaseUrl = await RetrieveApiBaseUrl();
            await _flurlClient.Request($"{apiBaseUrl}{uri}")
                .AllowAnyHttpStatus()
                .DeleteAsync()
                .ReceiveJson<DeleteComplianceOfficerResponse>();
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while deleting a complianceOfficer .", ex);
            throw new FetchDataException("An error occurred while deleting data.", ex);
        }
    }

    public async Task<GetByIdComplianceOfficerResponse> GetOneFullComplianceOfficerByIdAsync(Guid complianceOfficerId)
    {
        try
        {
            string? apiBaseUrl = await RetrieveApiBaseUrl();
            string uri = $"api/merchant/complianceOfficers/{complianceOfficerId}";
             GetByIdComplianceOfficerRequest request = new() { ComplianceOfficerId = complianceOfficerId };
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
                    _logger.LogWarning($"ComplianceOfficer with complianceOfficer not found.");
                    throw new NotFoundException($"ComplianceOfficer complianceOfficerId not found.", complianceOfficerId.ToString());
                }
                throw new FetchDataException($"Server returned non-success status code: {response.StatusCode}");
            }

            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            GetByIdComplianceOfficerResponse? responseDto =
                JsonSerializer.Deserialize<GetByIdComplianceOfficerResponse>(responseString, settings);

            if (responseDto?.ComplianceOfficer == null)
            {
                _logger.LogWarning($"Deserialized response contains no complianceOfficer data for ID ");
                throw new NotFoundException($"No complianceOfficer data found for complianceOfficerId", complianceOfficerId.ToString());
            }

            return responseDto;
        }
        catch (FlurlHttpTimeoutException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Timeout occurred while fetching complianceOfficer. Status code: {StatusCode}, Response: {Response}",
                statusCode, responseBody);
            throw new FetchDataException($"Request timed out while fetching complianceOfficer data for ID ");
        }
        catch (FlurlHttpException ex)
        {
            string responseBody = await ex.GetResponseStringAsync();
            string? errorMsg = BuildFlurlErrorMessage($"An error occurred while fetching complianceOfficer ", ex);
            _logger.LogError(ex, responseBody);
            throw new FetchDataException(responseBody, ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex.Message, "Error deserializing complianceOfficer response for ID {ComplianceOfficerId}", complianceOfficerId);
            throw new FetchDataException($"Error processing response for complianceOfficer {complianceOfficerId}", ex);
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "An unexpected error occurred while fetching complianceOfficer ComplianceOfficer");
            throw new FetchDataException($"An unexpected error occurred while fetching complianceOfficer ", ex);
        }
    }

    public async Task<ListComplianceOfficerResponse?> GetFilteredComplianceOfficersListAsync(int pageNumber, int pageSize, Dictionary<string, string> filters = null, List< PCIShieldLib.SharedKernel.Interfaces.Sort> sorting = null)
    {
        try
        {
            if (sorting == null && filters == null)
            {
                return await GetPagedComplianceOfficersListAsync(pageNumber, pageSize);
            }
           if (sorting == null && filters.Count == 1 && filters.TryGetValue("search", out string? filter))
            {
                var searchResults = await SearchComplianceOfficersAsync(filter);
                return new ListComplianceOfficerResponse { ComplianceOfficers = searchResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList(), Count = searchResults.Count };
            }
            string uri = $"api/complianceOfficers/filtered_list";
            string apiBaseUrl = await RetrieveApiBaseUrl();
            var request = new FilteredComplianceOfficerRequest
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
            return JsonSerializer.Deserialize<ListComplianceOfficerResponse>(responseString, settings);
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
            _logger.LogError(ex, "An unexpected error occurred while fetching filtered complianceOfficer list.");
            throw new FetchDataException("An unexpected error occurred while fetching filtered complianceOfficer list.", ex);
        }
    }
    public async Task<ListComplianceOfficerResponse?> GetPagedComplianceOfficersListAsync(int pageNumber, int pageSize)
    {
        string uri = $"api/complianceOfficers/paged_list";
        try
        {
            string apiBaseUrl = await RetrieveApiBaseUrl();
            GetComplianceOfficerListRequest request = new() { PageNumber = pageNumber, PageSize = pageSize };
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
            ListComplianceOfficerResponse? responseDto = JsonSerializer.Deserialize<ListComplianceOfficerResponse>(responseString, settings);
            if (responseDto?.ComplianceOfficers != null) { }
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
            _logger.LogError(ex, "An unexpected error occurred while fetching paged complianceOfficer list.");
            throw new FetchDataException("An unexpected error occurred while fetching paged complianceOfficer list.", ex);
        }
    }

    public async Task<List<ComplianceOfficerDto>> SearchComplianceOfficersAsync(string searchTerm)
    {
        string uri = $"api/complianceOfficers/search";
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
                    return new List<ComplianceOfficerDto>();
                }
                throw new FetchDataException($"Server returned non-success status code: {response.StatusCode}");
            }
            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            List<ComplianceOfficerDto>? complianceOfficers = JsonSerializer.Deserialize<List<ComplianceOfficerDto>>(responseString, settings);
            return complianceOfficers ?? new List<ComplianceOfficerDto>();
        }
        catch (FlurlHttpTimeoutException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Timeout occurred while searching complianceOfficers. Status code: {StatusCode}, Response: {Response}", statusCode, responseBody);
            throw new FetchDataException($"Search request timed out. Status code: {statusCode}", ex);
        }
        catch (FlurlHttpException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "HTTP error occurred while searching complianceOfficers. Status code: {StatusCode}, Response: {Response}", statusCode, responseBody);
            throw new FetchDataException($"An HTTP error occurred while searching. Status code: {statusCode}", ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error deserializing complianceOfficer search response");
            throw new FetchDataException("Error processing search results", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while searching complianceOfficers");
            throw new FetchDataException("An unexpected error occurred while searching complianceOfficers", ex);
        }
    }
 
    public async Task<CreateCryptographicInventoryResponse> CreateCryptographicInventoryAsync(CryptographicInventoryDto data)
    {
        string uri = $"api/cryptographicInventories";
        try
        {
            var request = new CreateCryptographicInventoryRequest { CryptographicInventory = data };
            string apiBaseUrl = await RetrieveApiBaseUrl();
            CreateCryptographicInventoryResponse response = await _flurlClient.Request($"{apiBaseUrl}{uri}")
                .AllowAnyHttpStatus()
                .PostJsonAsync(request).ReceiveJson<CreateCryptographicInventoryResponse>();

            var aa = response.CryptographicInventory;
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while creating a cryptographicInventory .", ex);
            throw new FetchDataException("An error occurred while creating data.", ex);
        }
    }

    public async Task<CryptographicInventoryDto> UpdateCryptographicInventoryAsync(CryptographicInventoryDto data)
    {
        string uri = $"api/cryptographicInventories";
        try
        {

            var request = new UpdateCryptographicInventoryRequest() { CryptographicInventory = data };
            string apiBaseUrl = await RetrieveApiBaseUrl();
            CryptographicInventoryDto response = await _flurlClient.Request($"{apiBaseUrl}{uri}")
                .AllowAnyHttpStatus()
                .PutJsonAsync(request)
                .ReceiveJson<CryptographicInventoryDto>();
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while updating a cryptographicInventory .", ex);
            throw new FetchDataException("An error occurred while updating data.", ex);
        }
    }

    public async Task DeleteCryptographicInventoryAsync(Guid cryptographicInventoryId)
    {
        string uri = $"api/merchant/cryptographicInventories/{cryptographicInventoryId}";
        try
        {
            string apiBaseUrl = await RetrieveApiBaseUrl();
            await _flurlClient.Request($"{apiBaseUrl}{uri}")
                .AllowAnyHttpStatus()
                .DeleteAsync()
                .ReceiveJson<DeleteCryptographicInventoryResponse>();
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while deleting a cryptographicInventory .", ex);
            throw new FetchDataException("An error occurred while deleting data.", ex);
        }
    }

    public async Task<GetByIdCryptographicInventoryResponse> GetOneFullCryptographicInventoryByIdAsync(Guid cryptographicInventoryId)
    {
        try
        {
            string? apiBaseUrl = await RetrieveApiBaseUrl();
            string uri = $"api/merchant/cryptographicInventories/{cryptographicInventoryId}";
             GetByIdCryptographicInventoryRequest request = new() { CryptographicInventoryId = cryptographicInventoryId };
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
                    _logger.LogWarning($"CryptographicInventory with cryptographicInventory not found.");
                    throw new NotFoundException($"CryptographicInventory cryptographicInventoryId not found.", cryptographicInventoryId.ToString());
                }
                throw new FetchDataException($"Server returned non-success status code: {response.StatusCode}");
            }

            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            GetByIdCryptographicInventoryResponse? responseDto =
                JsonSerializer.Deserialize<GetByIdCryptographicInventoryResponse>(responseString, settings);

            if (responseDto?.CryptographicInventory == null)
            {
                _logger.LogWarning($"Deserialized response contains no cryptographicInventory data for ID ");
                throw new NotFoundException($"No cryptographicInventory data found for cryptographicInventoryId", cryptographicInventoryId.ToString());
            }

            return responseDto;
        }
        catch (FlurlHttpTimeoutException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Timeout occurred while fetching cryptographicInventory. Status code: {StatusCode}, Response: {Response}",
                statusCode, responseBody);
            throw new FetchDataException($"Request timed out while fetching cryptographicInventory data for ID ");
        }
        catch (FlurlHttpException ex)
        {
            string responseBody = await ex.GetResponseStringAsync();
            string? errorMsg = BuildFlurlErrorMessage($"An error occurred while fetching cryptographicInventory ", ex);
            _logger.LogError(ex, responseBody);
            throw new FetchDataException(responseBody, ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex.Message, "Error deserializing cryptographicInventory response for ID {CryptographicInventoryId}", cryptographicInventoryId);
            throw new FetchDataException($"Error processing response for cryptographicInventory {cryptographicInventoryId}", ex);
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "An unexpected error occurred while fetching cryptographicInventory CryptographicInventory");
            throw new FetchDataException($"An unexpected error occurred while fetching cryptographicInventory ", ex);
        }
    }

    public async Task<ListCryptographicInventoryResponse?> GetFilteredCryptographicInventoriesListAsync(int pageNumber, int pageSize, Dictionary<string, string> filters = null, List< PCIShieldLib.SharedKernel.Interfaces.Sort> sorting = null)
    {
        try
        {
            if (sorting == null && filters == null)
            {
                return await GetPagedCryptographicInventoriesListAsync(pageNumber, pageSize);
            }
           if (sorting == null && filters.Count == 1 && filters.TryGetValue("search", out string? filter))
            {
                var searchResults = await SearchCryptographicInventoriesAsync(filter);
                return new ListCryptographicInventoryResponse { CryptographicInventories = searchResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList(), Count = searchResults.Count };
            }
            string uri = $"api/cryptographicInventories/filtered_list";
            string apiBaseUrl = await RetrieveApiBaseUrl();
            var request = new FilteredCryptographicInventoryRequest
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
            return JsonSerializer.Deserialize<ListCryptographicInventoryResponse>(responseString, settings);
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
            _logger.LogError(ex, "An unexpected error occurred while fetching filtered cryptographicInventory list.");
            throw new FetchDataException("An unexpected error occurred while fetching filtered cryptographicInventory list.", ex);
        }
    }
    public async Task<ListCryptographicInventoryResponse?> GetPagedCryptographicInventoriesListAsync(int pageNumber, int pageSize)
    {
        string uri = $"api/cryptographicInventories/paged_list";
        try
        {
            string apiBaseUrl = await RetrieveApiBaseUrl();
            GetCryptographicInventoryListRequest request = new() { PageNumber = pageNumber, PageSize = pageSize };
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
            ListCryptographicInventoryResponse? responseDto = JsonSerializer.Deserialize<ListCryptographicInventoryResponse>(responseString, settings);
            if (responseDto?.CryptographicInventories != null) { }
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
            _logger.LogError(ex, "An unexpected error occurred while fetching paged cryptographicInventory list.");
            throw new FetchDataException("An unexpected error occurred while fetching paged cryptographicInventory list.", ex);
        }
    }

    public async Task<List<CryptographicInventoryDto>> SearchCryptographicInventoriesAsync(string searchTerm)
    {
        string uri = $"api/cryptographicInventories/search";
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
                    return new List<CryptographicInventoryDto>();
                }
                throw new FetchDataException($"Server returned non-success status code: {response.StatusCode}");
            }
            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            List<CryptographicInventoryDto>? cryptographicInventories = JsonSerializer.Deserialize<List<CryptographicInventoryDto>>(responseString, settings);
            return cryptographicInventories ?? new List<CryptographicInventoryDto>();
        }
        catch (FlurlHttpTimeoutException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Timeout occurred while searching cryptographicInventories. Status code: {StatusCode}, Response: {Response}", statusCode, responseBody);
            throw new FetchDataException($"Search request timed out. Status code: {statusCode}", ex);
        }
        catch (FlurlHttpException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "HTTP error occurred while searching cryptographicInventories. Status code: {StatusCode}, Response: {Response}", statusCode, responseBody);
            throw new FetchDataException($"An HTTP error occurred while searching. Status code: {statusCode}", ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error deserializing cryptographicInventory search response");
            throw new FetchDataException("Error processing search results", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while searching cryptographicInventories");
            throw new FetchDataException("An unexpected error occurred while searching cryptographicInventories", ex);
        }
    }
 
    public async Task<CreateEvidenceResponse> CreateEvidenceAsync(EvidenceDto data)
    {
        string uri = $"api/evidences";
        try
        {
            var request = new CreateEvidenceRequest { Evidence = data };
            string apiBaseUrl = await RetrieveApiBaseUrl();
            CreateEvidenceResponse response = await _flurlClient.Request($"{apiBaseUrl}{uri}")
                .AllowAnyHttpStatus()
                .PostJsonAsync(request).ReceiveJson<CreateEvidenceResponse>();

            var aa = response.Evidence;
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while creating a evidence .", ex);
            throw new FetchDataException("An error occurred while creating data.", ex);
        }
    }

    public async Task<EvidenceDto> UpdateEvidenceAsync(EvidenceDto data)
    {
        string uri = $"api/evidences";
        try
        {

            var request = new UpdateEvidenceRequest() { Evidence = data };
            string apiBaseUrl = await RetrieveApiBaseUrl();
            EvidenceDto response = await _flurlClient.Request($"{apiBaseUrl}{uri}")
                .AllowAnyHttpStatus()
                .PutJsonAsync(request)
                .ReceiveJson<EvidenceDto>();
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while updating a evidence .", ex);
            throw new FetchDataException("An error occurred while updating data.", ex);
        }
    }

    public async Task DeleteEvidenceAsync(Guid evidenceId)
    {
        string uri = $"api/merchant/evidences/{evidenceId}";
        try
        {
            string apiBaseUrl = await RetrieveApiBaseUrl();
            await _flurlClient.Request($"{apiBaseUrl}{uri}")
                .AllowAnyHttpStatus()
                .DeleteAsync()
                .ReceiveJson<DeleteEvidenceResponse>();
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while deleting a evidence .", ex);
            throw new FetchDataException("An error occurred while deleting data.", ex);
        }
    }

    public async Task<GetByIdEvidenceResponse> GetOneFullEvidenceByIdAsync(Guid evidenceId)
    {
        try
        {
            string? apiBaseUrl = await RetrieveApiBaseUrl();
            string uri = $"api/merchant/evidences/{evidenceId}";
             GetByIdEvidenceRequest request = new() { EvidenceId = evidenceId };
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
                    _logger.LogWarning($"Evidence with evidence not found.");
                    throw new NotFoundException($"Evidence evidenceId not found.", evidenceId.ToString());
                }
                throw new FetchDataException($"Server returned non-success status code: {response.StatusCode}");
            }

            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            GetByIdEvidenceResponse? responseDto =
                JsonSerializer.Deserialize<GetByIdEvidenceResponse>(responseString, settings);

            if (responseDto?.Evidence == null)
            {
                _logger.LogWarning($"Deserialized response contains no evidence data for ID ");
                throw new NotFoundException($"No evidence data found for evidenceId", evidenceId.ToString());
            }

            return responseDto;
        }
        catch (FlurlHttpTimeoutException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Timeout occurred while fetching evidence. Status code: {StatusCode}, Response: {Response}",
                statusCode, responseBody);
            throw new FetchDataException($"Request timed out while fetching evidence data for ID ");
        }
        catch (FlurlHttpException ex)
        {
            string responseBody = await ex.GetResponseStringAsync();
            string? errorMsg = BuildFlurlErrorMessage($"An error occurred while fetching evidence ", ex);
            _logger.LogError(ex, responseBody);
            throw new FetchDataException(responseBody, ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex.Message, "Error deserializing evidence response for ID {EvidenceId}", evidenceId);
            throw new FetchDataException($"Error processing response for evidence {evidenceId}", ex);
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "An unexpected error occurred while fetching evidence Evidence");
            throw new FetchDataException($"An unexpected error occurred while fetching evidence ", ex);
        }
    }

    public async Task<ListEvidenceResponse?> GetFilteredEvidencesListAsync(int pageNumber, int pageSize, Dictionary<string, string> filters = null, List< PCIShieldLib.SharedKernel.Interfaces.Sort> sorting = null)
    {
        try
        {
            if (sorting == null && filters == null)
            {
                return await GetPagedEvidencesListAsync(pageNumber, pageSize);
            }
           if (sorting == null && filters.Count == 1 && filters.TryGetValue("search", out string? filter))
            {
                var searchResults = await SearchEvidencesAsync(filter);
                return new ListEvidenceResponse { Evidences = searchResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList(), Count = searchResults.Count };
            }
            string uri = $"api/evidences/filtered_list";
            string apiBaseUrl = await RetrieveApiBaseUrl();
            var request = new FilteredEvidenceRequest
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
            return JsonSerializer.Deserialize<ListEvidenceResponse>(responseString, settings);
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
            _logger.LogError(ex, "An unexpected error occurred while fetching filtered evidence list.");
            throw new FetchDataException("An unexpected error occurred while fetching filtered evidence list.", ex);
        }
    }
    public async Task<ListEvidenceResponse?> GetPagedEvidencesListAsync(int pageNumber, int pageSize)
    {
        string uri = $"api/evidences/paged_list";
        try
        {
            string apiBaseUrl = await RetrieveApiBaseUrl();
            GetEvidenceListRequest request = new() { PageNumber = pageNumber, PageSize = pageSize };
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
            ListEvidenceResponse? responseDto = JsonSerializer.Deserialize<ListEvidenceResponse>(responseString, settings);
            if (responseDto?.Evidences != null) { }
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
            _logger.LogError(ex, "An unexpected error occurred while fetching paged evidence list.");
            throw new FetchDataException("An unexpected error occurred while fetching paged evidence list.", ex);
        }
    }

    public async Task<List<EvidenceDto>> SearchEvidencesAsync(string searchTerm)
    {
        string uri = $"api/evidences/search";
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
                    return new List<EvidenceDto>();
                }
                throw new FetchDataException($"Server returned non-success status code: {response.StatusCode}");
            }
            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            List<EvidenceDto>? evidences = JsonSerializer.Deserialize<List<EvidenceDto>>(responseString, settings);
            return evidences ?? new List<EvidenceDto>();
        }
        catch (FlurlHttpTimeoutException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Timeout occurred while searching evidences. Status code: {StatusCode}, Response: {Response}", statusCode, responseBody);
            throw new FetchDataException($"Search request timed out. Status code: {statusCode}", ex);
        }
        catch (FlurlHttpException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "HTTP error occurred while searching evidences. Status code: {StatusCode}, Response: {Response}", statusCode, responseBody);
            throw new FetchDataException($"An HTTP error occurred while searching. Status code: {statusCode}", ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error deserializing evidence search response");
            throw new FetchDataException("Error processing search results", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while searching evidences");
            throw new FetchDataException("An unexpected error occurred while searching evidences", ex);
        }
    }
 
    public async Task<CreateNetworkSegmentationResponse> CreateNetworkSegmentationAsync(NetworkSegmentationDto data)
    {
        string uri = $"api/networkSegmentations";
        try
        {
            var request = new CreateNetworkSegmentationRequest { NetworkSegmentation = data };
            string apiBaseUrl = await RetrieveApiBaseUrl();
            CreateNetworkSegmentationResponse response = await _flurlClient.Request($"{apiBaseUrl}{uri}")
                .AllowAnyHttpStatus()
                .PostJsonAsync(request).ReceiveJson<CreateNetworkSegmentationResponse>();

            var aa = response.NetworkSegmentation;
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while creating a networkSegmentation .", ex);
            throw new FetchDataException("An error occurred while creating data.", ex);
        }
    }

    public async Task<NetworkSegmentationDto> UpdateNetworkSegmentationAsync(NetworkSegmentationDto data)
    {
        string uri = $"api/networkSegmentations";
        try
        {

            var request = new UpdateNetworkSegmentationRequest() { NetworkSegmentation = data };
            string apiBaseUrl = await RetrieveApiBaseUrl();
            NetworkSegmentationDto response = await _flurlClient.Request($"{apiBaseUrl}{uri}")
                .AllowAnyHttpStatus()
                .PutJsonAsync(request)
                .ReceiveJson<NetworkSegmentationDto>();
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while updating a networkSegmentation .", ex);
            throw new FetchDataException("An error occurred while updating data.", ex);
        }
    }

    public async Task DeleteNetworkSegmentationAsync(Guid networkSegmentationId)
    {
        string uri = $"api/merchant/networkSegmentations/{networkSegmentationId}";
        try
        {
            string apiBaseUrl = await RetrieveApiBaseUrl();
            await _flurlClient.Request($"{apiBaseUrl}{uri}")
                .AllowAnyHttpStatus()
                .DeleteAsync()
                .ReceiveJson<DeleteNetworkSegmentationResponse>();
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while deleting a networkSegmentation .", ex);
            throw new FetchDataException("An error occurred while deleting data.", ex);
        }
    }

    public async Task<GetByIdNetworkSegmentationResponse> GetOneFullNetworkSegmentationByIdAsync(Guid networkSegmentationId)
    {
        try
        {
            string? apiBaseUrl = await RetrieveApiBaseUrl();
            string uri = $"api/merchant/networkSegmentations/{networkSegmentationId}";
             GetByIdNetworkSegmentationRequest request = new() { NetworkSegmentationId = networkSegmentationId };
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
                    _logger.LogWarning($"NetworkSegmentation with networkSegmentation not found.");
                    throw new NotFoundException($"NetworkSegmentation networkSegmentationId not found.", networkSegmentationId.ToString());
                }
                throw new FetchDataException($"Server returned non-success status code: {response.StatusCode}");
            }

            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            GetByIdNetworkSegmentationResponse? responseDto =
                JsonSerializer.Deserialize<GetByIdNetworkSegmentationResponse>(responseString, settings);

            if (responseDto?.NetworkSegmentation == null)
            {
                _logger.LogWarning($"Deserialized response contains no networkSegmentation data for ID ");
                throw new NotFoundException($"No networkSegmentation data found for networkSegmentationId", networkSegmentationId.ToString());
            }

            return responseDto;
        }
        catch (FlurlHttpTimeoutException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Timeout occurred while fetching networkSegmentation. Status code: {StatusCode}, Response: {Response}",
                statusCode, responseBody);
            throw new FetchDataException($"Request timed out while fetching networkSegmentation data for ID ");
        }
        catch (FlurlHttpException ex)
        {
            string responseBody = await ex.GetResponseStringAsync();
            string? errorMsg = BuildFlurlErrorMessage($"An error occurred while fetching networkSegmentation ", ex);
            _logger.LogError(ex, responseBody);
            throw new FetchDataException(responseBody, ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex.Message, "Error deserializing networkSegmentation response for ID {NetworkSegmentationId}", networkSegmentationId);
            throw new FetchDataException($"Error processing response for networkSegmentation {networkSegmentationId}", ex);
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "An unexpected error occurred while fetching networkSegmentation NetworkSegmentation");
            throw new FetchDataException($"An unexpected error occurred while fetching networkSegmentation ", ex);
        }
    }

    public async Task<ListNetworkSegmentationResponse?> GetFilteredNetworkSegmentationsListAsync(int pageNumber, int pageSize, Dictionary<string, string> filters = null, List< PCIShieldLib.SharedKernel.Interfaces.Sort> sorting = null)
    {
        try
        {
            if (sorting == null && filters == null)
            {
                return await GetPagedNetworkSegmentationsListAsync(pageNumber, pageSize);
            }
           if (sorting == null && filters.Count == 1 && filters.TryGetValue("search", out string? filter))
            {
                var searchResults = await SearchNetworkSegmentationsAsync(filter);
                return new ListNetworkSegmentationResponse { NetworkSegmentations = searchResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList(), Count = searchResults.Count };
            }
            string uri = $"api/networkSegmentations/filtered_list";
            string apiBaseUrl = await RetrieveApiBaseUrl();
            var request = new FilteredNetworkSegmentationRequest
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
            return JsonSerializer.Deserialize<ListNetworkSegmentationResponse>(responseString, settings);
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
            _logger.LogError(ex, "An unexpected error occurred while fetching filtered networkSegmentation list.");
            throw new FetchDataException("An unexpected error occurred while fetching filtered networkSegmentation list.", ex);
        }
    }
    public async Task<ListNetworkSegmentationResponse?> GetPagedNetworkSegmentationsListAsync(int pageNumber, int pageSize)
    {
        string uri = $"api/networkSegmentations/paged_list";
        try
        {
            string apiBaseUrl = await RetrieveApiBaseUrl();
            GetNetworkSegmentationListRequest request = new() { PageNumber = pageNumber, PageSize = pageSize };
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
            ListNetworkSegmentationResponse? responseDto = JsonSerializer.Deserialize<ListNetworkSegmentationResponse>(responseString, settings);
            if (responseDto?.NetworkSegmentations != null) { }
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
            _logger.LogError(ex, "An unexpected error occurred while fetching paged networkSegmentation list.");
            throw new FetchDataException("An unexpected error occurred while fetching paged networkSegmentation list.", ex);
        }
    }

    public async Task<List<NetworkSegmentationDto>> SearchNetworkSegmentationsAsync(string searchTerm)
    {
        string uri = $"api/networkSegmentations/search";
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
                    return new List<NetworkSegmentationDto>();
                }
                throw new FetchDataException($"Server returned non-success status code: {response.StatusCode}");
            }
            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            List<NetworkSegmentationDto>? networkSegmentations = JsonSerializer.Deserialize<List<NetworkSegmentationDto>>(responseString, settings);
            return networkSegmentations ?? new List<NetworkSegmentationDto>();
        }
        catch (FlurlHttpTimeoutException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Timeout occurred while searching networkSegmentations. Status code: {StatusCode}, Response: {Response}", statusCode, responseBody);
            throw new FetchDataException($"Search request timed out. Status code: {statusCode}", ex);
        }
        catch (FlurlHttpException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "HTTP error occurred while searching networkSegmentations. Status code: {StatusCode}, Response: {Response}", statusCode, responseBody);
            throw new FetchDataException($"An HTTP error occurred while searching. Status code: {statusCode}", ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error deserializing networkSegmentation search response");
            throw new FetchDataException("Error processing search results", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while searching networkSegmentations");
            throw new FetchDataException("An unexpected error occurred while searching networkSegmentations", ex);
        }
    }
 
    public async Task<CreatePaymentChannelResponse> CreatePaymentChannelAsync(PaymentChannelDto data)
    {
        string uri = $"api/paymentChannels";
        try
        {
            var request = new CreatePaymentChannelRequest { PaymentChannel = data };
            string apiBaseUrl = await RetrieveApiBaseUrl();
            CreatePaymentChannelResponse response = await _flurlClient.Request($"{apiBaseUrl}{uri}")
                .AllowAnyHttpStatus()
                .PostJsonAsync(request).ReceiveJson<CreatePaymentChannelResponse>();

            var aa = response.PaymentChannel;
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while creating a paymentChannel .", ex);
            throw new FetchDataException("An error occurred while creating data.", ex);
        }
    }

    public async Task<PaymentChannelDto> UpdatePaymentChannelAsync(PaymentChannelDto data)
    {
        string uri = $"api/paymentChannels";
        try
        {

            var request = new UpdatePaymentChannelRequest() { PaymentChannel = data };
            string apiBaseUrl = await RetrieveApiBaseUrl();
            PaymentChannelDto response = await _flurlClient.Request($"{apiBaseUrl}{uri}")
                .AllowAnyHttpStatus()
                .PutJsonAsync(request)
                .ReceiveJson<PaymentChannelDto>();
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while updating a paymentChannel .", ex);
            throw new FetchDataException("An error occurred while updating data.", ex);
        }
    }

    public async Task DeletePaymentChannelAsync(Guid paymentChannelId)
    {
        string uri = $"api/merchant/paymentChannels/{paymentChannelId}";
        try
        {
            string apiBaseUrl = await RetrieveApiBaseUrl();
            await _flurlClient.Request($"{apiBaseUrl}{uri}")
                .AllowAnyHttpStatus()
                .DeleteAsync()
                .ReceiveJson<DeletePaymentChannelResponse>();
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while deleting a paymentChannel .", ex);
            throw new FetchDataException("An error occurred while deleting data.", ex);
        }
    }

    public async Task<GetByIdPaymentChannelResponse> GetOneFullPaymentChannelByIdAsync(Guid paymentChannelId)
    {
        try
        {
            string? apiBaseUrl = await RetrieveApiBaseUrl();
            string uri = $"api/merchant/paymentChannels/{paymentChannelId}";
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
                    _logger.LogWarning($"PaymentChannel with paymentChannel not found.");
                    throw new NotFoundException($"PaymentChannel paymentChannelId not found.", paymentChannelId.ToString());
                }
                throw new FetchDataException($"Server returned non-success status code: {response.StatusCode}");
            }

            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            GetByIdPaymentChannelResponse? responseDto =
                JsonSerializer.Deserialize<GetByIdPaymentChannelResponse>(responseString, settings);

            if (responseDto?.PaymentChannel == null)
            {
                _logger.LogWarning($"Deserialized response contains no paymentChannel data for ID ");
                throw new NotFoundException($"No paymentChannel data found for paymentChannelId", paymentChannelId.ToString());
            }

            return responseDto;
        }
        catch (FlurlHttpTimeoutException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Timeout occurred while fetching paymentChannel. Status code: {StatusCode}, Response: {Response}",
                statusCode, responseBody);
            throw new FetchDataException($"Request timed out while fetching paymentChannel data for ID ");
        }
        catch (FlurlHttpException ex)
        {
            string responseBody = await ex.GetResponseStringAsync();
            string? errorMsg = BuildFlurlErrorMessage($"An error occurred while fetching paymentChannel ", ex);
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
            _logger.LogError(ex.Message, "An unexpected error occurred while fetching paymentChannel PaymentChannel");
            throw new FetchDataException($"An unexpected error occurred while fetching paymentChannel ", ex);
        }
    }

    public async Task<ListPaymentChannelResponse?> GetFilteredPaymentChannelsListAsync(int pageNumber, int pageSize, Dictionary<string, string> filters = null, List< PCIShieldLib.SharedKernel.Interfaces.Sort> sorting = null)
    {
        try
        {
            if (sorting == null && filters == null)
            {
                return await GetPagedPaymentChannelsListAsync(pageNumber, pageSize);
            }
           if (sorting == null && filters.Count == 1 && filters.TryGetValue("search", out string? filter))
            {
                var searchResults = await SearchPaymentChannelsAsync(filter);
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

    public async Task<List<PaymentChannelDto>> SearchPaymentChannelsAsync(string searchTerm)
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
 
    public async Task<CreateServiceProviderResponse> CreateServiceProviderAsync(ServiceProviderDto data)
    {
        string uri = $"api/serviceProviders";
        try
        {
            var request = new CreateServiceProviderRequest { ServiceProvider = data };
            string apiBaseUrl = await RetrieveApiBaseUrl();
            CreateServiceProviderResponse response = await _flurlClient.Request($"{apiBaseUrl}{uri}")
                .AllowAnyHttpStatus()
                .PostJsonAsync(request).ReceiveJson<CreateServiceProviderResponse>();

            var aa = response.ServiceProvider;
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while creating a serviceProvider .", ex);
            throw new FetchDataException("An error occurred while creating data.", ex);
        }
    }

    public async Task<ServiceProviderDto> UpdateServiceProviderAsync(ServiceProviderDto data)
    {
        string uri = $"api/serviceProviders";
        try
        {

            var request = new UpdateServiceProviderRequest() { ServiceProvider = data };
            string apiBaseUrl = await RetrieveApiBaseUrl();
            ServiceProviderDto response = await _flurlClient.Request($"{apiBaseUrl}{uri}")
                .AllowAnyHttpStatus()
                .PutJsonAsync(request)
                .ReceiveJson<ServiceProviderDto>();
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while updating a serviceProvider .", ex);
            throw new FetchDataException("An error occurred while updating data.", ex);
        }
    }

    public async Task DeleteServiceProviderAsync(Guid serviceProviderId)
    {
        string uri = $"api/merchant/serviceProviders/{serviceProviderId}";
        try
        {
            string apiBaseUrl = await RetrieveApiBaseUrl();
            await _flurlClient.Request($"{apiBaseUrl}{uri}")
                .AllowAnyHttpStatus()
                .DeleteAsync()
                .ReceiveJson<DeleteServiceProviderResponse>();
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while deleting a serviceProvider .", ex);
            throw new FetchDataException("An error occurred while deleting data.", ex);
        }
    }

    public async Task<GetByIdServiceProviderResponse> GetOneFullServiceProviderByIdAsync(Guid serviceProviderId)
    {
        try
        {
            string? apiBaseUrl = await RetrieveApiBaseUrl();
            string uri = $"api/merchant/serviceProviders/{serviceProviderId}";
             GetByIdServiceProviderRequest request = new() { ServiceProviderId = serviceProviderId };
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
                    _logger.LogWarning($"ServiceProvider with serviceProvider not found.");
                    throw new NotFoundException($"ServiceProvider serviceProviderId not found.", serviceProviderId.ToString());
                }
                throw new FetchDataException($"Server returned non-success status code: {response.StatusCode}");
            }

            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            GetByIdServiceProviderResponse? responseDto =
                JsonSerializer.Deserialize<GetByIdServiceProviderResponse>(responseString, settings);

            if (responseDto?.ServiceProvider == null)
            {
                _logger.LogWarning($"Deserialized response contains no serviceProvider data for ID ");
                throw new NotFoundException($"No serviceProvider data found for serviceProviderId", serviceProviderId.ToString());
            }

            return responseDto;
        }
        catch (FlurlHttpTimeoutException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Timeout occurred while fetching serviceProvider. Status code: {StatusCode}, Response: {Response}",
                statusCode, responseBody);
            throw new FetchDataException($"Request timed out while fetching serviceProvider data for ID ");
        }
        catch (FlurlHttpException ex)
        {
            string responseBody = await ex.GetResponseStringAsync();
            string? errorMsg = BuildFlurlErrorMessage($"An error occurred while fetching serviceProvider ", ex);
            _logger.LogError(ex, responseBody);
            throw new FetchDataException(responseBody, ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex.Message, "Error deserializing serviceProvider response for ID {ServiceProviderId}", serviceProviderId);
            throw new FetchDataException($"Error processing response for serviceProvider {serviceProviderId}", ex);
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "An unexpected error occurred while fetching serviceProvider ServiceProvider");
            throw new FetchDataException($"An unexpected error occurred while fetching serviceProvider ", ex);
        }
    }

    public async Task<ListServiceProviderResponse?> GetFilteredServiceProvidersListAsync(int pageNumber, int pageSize, Dictionary<string, string> filters = null, List< PCIShieldLib.SharedKernel.Interfaces.Sort> sorting = null)
    {
        try
        {
            if (sorting == null && filters == null)
            {
                return await GetPagedServiceProvidersListAsync(pageNumber, pageSize);
            }
           if (sorting == null && filters.Count == 1 && filters.TryGetValue("search", out string? filter))
            {
                var searchResults = await SearchServiceProvidersAsync(filter);
                return new ListServiceProviderResponse { ServiceProviders = searchResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList(), Count = searchResults.Count };
            }
            string uri = $"api/serviceProviders/filtered_list";
            string apiBaseUrl = await RetrieveApiBaseUrl();
            var request = new FilteredServiceProviderRequest
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
            return JsonSerializer.Deserialize<ListServiceProviderResponse>(responseString, settings);
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
            _logger.LogError(ex, "An unexpected error occurred while fetching filtered serviceProvider list.");
            throw new FetchDataException("An unexpected error occurred while fetching filtered serviceProvider list.", ex);
        }
    }
    public async Task<ListServiceProviderResponse?> GetPagedServiceProvidersListAsync(int pageNumber, int pageSize)
    {
        string uri = $"api/serviceProviders/paged_list";
        try
        {
            string apiBaseUrl = await RetrieveApiBaseUrl();
            GetServiceProviderListRequest request = new() { PageNumber = pageNumber, PageSize = pageSize };
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
            ListServiceProviderResponse? responseDto = JsonSerializer.Deserialize<ListServiceProviderResponse>(responseString, settings);
            if (responseDto?.ServiceProviders != null) { }
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
            _logger.LogError(ex, "An unexpected error occurred while fetching paged serviceProvider list.");
            throw new FetchDataException("An unexpected error occurred while fetching paged serviceProvider list.", ex);
        }
    }

    public async Task<List<ServiceProviderDto>> SearchServiceProvidersAsync(string searchTerm)
    {
        string uri = $"api/serviceProviders/search";
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
                    return new List<ServiceProviderDto>();
                }
                throw new FetchDataException($"Server returned non-success status code: {response.StatusCode}");
            }
            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            List<ServiceProviderDto>? serviceProviders = JsonSerializer.Deserialize<List<ServiceProviderDto>>(responseString, settings);
            return serviceProviders ?? new List<ServiceProviderDto>();
        }
        catch (FlurlHttpTimeoutException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Timeout occurred while searching serviceProviders. Status code: {StatusCode}, Response: {Response}", statusCode, responseBody);
            throw new FetchDataException($"Search request timed out. Status code: {statusCode}", ex);
        }
        catch (FlurlHttpException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "HTTP error occurred while searching serviceProviders. Status code: {StatusCode}, Response: {Response}", statusCode, responseBody);
            throw new FetchDataException($"An HTTP error occurred while searching. Status code: {statusCode}", ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error deserializing serviceProvider search response");
            throw new FetchDataException("Error processing search results", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while searching serviceProviders");
            throw new FetchDataException("An unexpected error occurred while searching serviceProviders", ex);
        }
    }
    public async Task<ListControlResponse?> GetFilteredControlsListAsync(int pageNumber, int pageSize, Dictionary<string, string> filters = null, List< PCIShieldLib.SharedKernel.Interfaces.Sort> sorting = null)
    {
        try
        {
            if (sorting == null && filters == null)
            {
                return await GetPagedControlsListAsync(pageNumber, pageSize);
            }
           if (sorting == null && filters.Count == 1 && filters.TryGetValue("search", out string? filter))
            {
                var searchResults = await SearchControlsAsync(filter);
                return new ListControlResponse { Controls = searchResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList(), Count = searchResults.Count };
            }
            string uri = $"api/controls/filtered_list";
            string apiBaseUrl = await RetrieveApiBaseUrl();
            var request = new FilteredControlRequest
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
            return JsonSerializer.Deserialize<ListControlResponse>(responseString, settings);
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
            _logger.LogError(ex, "An unexpected error occurred while fetching filtered control list.");
            throw new FetchDataException("An unexpected error occurred while fetching filtered control list.", ex);
        }
    }
    public async Task<ListControlResponse?> GetPagedControlsListAsync(int pageNumber, int pageSize)
    {
        string uri = $"api/controls/paged_list";
        try
        {
            string apiBaseUrl = await RetrieveApiBaseUrl();
            GetControlListRequest request = new() { PageNumber = pageNumber, PageSize = pageSize };
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
            ListControlResponse? responseDto = JsonSerializer.Deserialize<ListControlResponse>(responseString, settings);
            if (responseDto?.Controls != null) { }
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
            _logger.LogError(ex, "An unexpected error occurred while fetching paged control list.");
            throw new FetchDataException("An unexpected error occurred while fetching paged control list.", ex);
        }
    }

    public async Task<List<ControlDto>> SearchControlsAsync(string searchTerm)
    {
        string uri = $"api/controls/search";
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
                    return new List<ControlDto>();
                }
                throw new FetchDataException($"Server returned non-success status code: {response.StatusCode}");
            }
            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            List<ControlDto>? controls = JsonSerializer.Deserialize<List<ControlDto>>(responseString, settings);
            return controls ?? new List<ControlDto>();
        }
        catch (FlurlHttpTimeoutException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Timeout occurred while searching controls. Status code: {StatusCode}, Response: {Response}", statusCode, responseBody);
            throw new FetchDataException($"Search request timed out. Status code: {statusCode}", ex);
        }
        catch (FlurlHttpException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "HTTP error occurred while searching controls. Status code: {StatusCode}, Response: {Response}", statusCode, responseBody);
            throw new FetchDataException($"An HTTP error occurred while searching. Status code: {statusCode}", ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error deserializing control search response");
            throw new FetchDataException("Error processing search results", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while searching controls");
            throw new FetchDataException("An unexpected error occurred while searching controls", ex);
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
    private async Task<IFlurlRequest> CreateAuthenticatedRequest(string uri)
    {
        string apiBaseUrl = await RetrieveApiBaseUrl();
        string? token = await _tokenService.GetTokenAsync();

        IFlurlRequest request = _flurlClient.Request($"{apiBaseUrl}{uri}");

        if (!string.IsNullOrEmpty(token))
        {
            request = request.WithOAuthBearerToken(token);
        }
        else
        {
            _logger.LogWarning("No authentication token available for request to {Uri}", uri);
        }

        return request;
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

