using System.Net.Http.Headers;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text.Json;
using System.Text;
using System.Web;
using Ardalis.GuardClauses;
using BlazorMauiShared.Models.Assessment;
using BlazorMauiShared.Models.ROCPackage;
using BlazorMauiShared.Models.ScanSchedule;
using BlazorMauiShared.Models.Vulnerability;
using BlazorMauiShared.Models.PaymentPage;
using BlazorMauiShared.Models.Script;
using BlazorMauiShared.Models.AssessmentControl;
using BlazorMauiShared.Models.Control;
using BlazorMauiShared.Models.ControlEvidence;
using BlazorMauiShared.Models.Evidence;
using BlazorMauiShared.Models.Merchant;

using PCIShield.BlazorMauiShared.Models;
using PCIShield.BlazorMauiShared.ModelsDto;
using PCIShield.Client.Services.Common;
using PCIShield.Domain.ModelsDto;
using PCIShield.BlazorMauiShared.Models.ROCPackage;
using PCIShield.BlazorMauiShared.Models.Control;
using PCIShield.BlazorMauiShared.Models.AssessmentControl;
using PCIShield.BlazorMauiShared.Models.Evidence;
using PCIShield.BlazorMauiShared.Models.ControlEvidence;
using Flurl.Http;
using Flurl.Http.Configuration;
using Microsoft.Maui.Devices;

using PCIShield.BlazorMauiShared.Models.Assessment;
using PCIShield.BlazorMauiShared.Models.Merchant;
using PCIShield.Client.Services.InvoiceSession;

using ReactiveUI;
using JsonSerializer = System.Text.Json.JsonSerializer;
using JsonSerializerOptions = System.Text.Json.JsonSerializerOptions;
using Unit = System.Reactive.Unit;
  	  
namespace PCIShield.Client.Services.Assessment;
  	  
public class HttpAssessmentClientService : IDisposable, IHttpAssessmentClientService
{
    private readonly CompositeDisposable _disposables = new();
    private readonly IAppLoggerService<HttpAssessmentClientService> _logger;
    private readonly ITokenService _tokenService;
    private FlurlClient _flurlClient;
    private HttpClient _httpClient;
  	  
    public HttpAssessmentClientService(
        IAppLoggerService<HttpAssessmentClientService> logger,
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

    public async Task<UpdateAssessmentResponse> UpdatePutAssessmentAsync(UpdateAssessmentRequest data)
    {
        string? uri = "api/assessments";
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
            UpdateAssessmentResponse? responseDto =
                JsonSerializer.Deserialize<UpdateAssessmentResponse>(responseString, settings);

            return responseDto;
        }
        catch (FlurlHttpTimeoutException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Timeout occurred while updating assessment. Status code: {StatusCode}, Response: {Response}",
                statusCode, responseBody);
            throw new FetchDataException($"Update request timed out. Status code: {statusCode}", ex);
        }
        catch (FlurlHttpException ex)
        {
            string responseBody = await ex.GetResponseStringAsync();
            string? errorMsg = BuildFlurlErrorMessage("An error occurred while updating assessment", ex);
            _logger.LogError(ex, responseBody);
            throw new FetchDataException(responseBody, ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error deserializing assessment update response");
            throw new FetchDataException("Error processing update response", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError("An unexpected error occurred while updating assessment.", ex);
            throw new FetchDataException("An unexpected error occurred while updating assessment.", ex);
        }
    }
    public async Task<CreateAssessmentResponse> CreatePostAssessmentAsync(CreateAssessmentRequest data)
    {
        string? uri = "api/assessments";
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
            CreateAssessmentResponse? responseDto =
                JsonSerializer.Deserialize<CreateAssessmentResponse>(responseString, settings);

            return responseDto;
        }
        catch (FlurlHttpTimeoutException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Timeout occurred while creating assessment. Status code: {StatusCode}, Response: {Response}",
                statusCode, responseBody);
            HandleHttpServiceError(ex, ErrorHandlingService.ClientOperationContext.Create);
            throw new FetchDataException($"Create request timed out. Status code: {statusCode}", ex);
        }
        catch (FlurlHttpException ex)
        {
            string responseBody = await ex.GetResponseStringAsync();
            string? errorMsg = BuildFlurlErrorMessage("An error occurred while creating assessment", ex);
            _logger.LogError(ex, responseBody);
            throw new FetchDataException(responseBody, ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error deserializing assessment creation response");
            throw new FetchDataException("Error processing creation response", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError("An unexpected error occurred while creating assessment.", ex);
            throw new FetchDataException("An unexpected error occurred while creating assessment.", ex);
        }
    }
    public async Task<GetByIdAssessmentResponse> GetLastCreatedAssessmentAsync()
    {
        string uri = "api/assessments/last-created";
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
            return JsonSerializer.Deserialize<GetByIdAssessmentResponse>(responseString, settings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving last created assessment");
            throw new FetchDataException("Error retrieving last created assessment", ex);
        }
    }
    public async Task<DeleteAssessmentResponse> DeleteAssessmentAsync(Guid assessmentId)
    {
        string? uri = $"api/assessments/{assessmentId}";
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
            DeleteAssessmentResponse? responseDto =
                JsonSerializer.Deserialize<DeleteAssessmentResponse>(responseString, settings);

            return responseDto;
        }
        catch (FlurlHttpTimeoutException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Timeout occurred while deleting assessment. Status code: {StatusCode}, Response: {Response}",
                statusCode, responseBody);
            throw new FetchDataException($"Delete request timed out. Status code: {statusCode}", ex);
        }
        catch (FlurlHttpException ex)
        {
            string responseBody = await ex.GetResponseStringAsync();
            string? errorMsg = BuildFlurlErrorMessage("An error occurred while deleting assessment", ex);
            _logger.LogError(ex, responseBody);
            throw new FetchDataException(responseBody, ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error deserializing assessment deletion response");
            throw new FetchDataException("Error processing deletion response", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError("An unexpected error occurred while deleting assessment.", ex);
            throw new FetchDataException("An unexpected error occurred while deleting assessment.", ex);
        }
    }
    public async Task<ListAssessmentResponse?> GetFilteredAssessmentListAsync(int pageNumber, int pageSize, Dictionary<string, string> filters = null, List<PCIShieldLib.SharedKernel.Interfaces.Sort> sorting = null)
    {
        try
        {
            if (sorting == null && filters == null)
            {
                return await GetPagedAssessmentsListAsync(pageNumber, pageSize);
            }
           if (sorting == null && filters.Count == 1 && filters.TryGetValue("search", out string? filter))
            {
                var searchResults = await SearchAssessmentByAssessmentAsync(filter);
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
            IFlurlResponse? response = await _flurlClient.Request($"{apiBaseUrl}{uri}").AllowAnyHttpStatus().PostJsonAsync(request);
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
            GetAssessmentListRequest request = new() { PageNumber = pageNumber, PageSize = pageSize };
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
    public async Task<List<AssessmentDto>> SearchAssessmentByAssessmentAsync(string searchTerm)
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

    public async Task<GetByIdAssessmentResponse> GetOneFullAssessmentByIdAsync(Guid assessmentId, bool withPostGraph = false)
    {
        string? uri = $"api/assessments/i/{assessmentId}";
        try
        {
            string? apiBaseUrl = await RetrieveApiBaseUrl();
            GetByIdAssessmentRequest request = new() { AssessmentId = assessmentId, WithPostGraph = withPostGraph };
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
                    _logger.LogWarning($"Assessment with assessmentId {assessmentId} not found.");
                    throw new NotFoundException($"Assessment with assessmentId {assessmentId} not found.", assessmentId.ToString());
                }
                throw new FetchDataException($"Server returned non-success status code: {response.StatusCode}");
            }

            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            GetByIdAssessmentResponse? responseDto =
                JsonSerializer.Deserialize<GetByIdAssessmentResponse>(responseString, settings);

            if (responseDto?.Assessment == null)
            {
                _logger.LogWarning($"Deserialized response contains no assessment data for ID {assessmentId}");
                throw new NotFoundException($"No assessment data found for assessmentId {assessmentId}", assessmentId.ToString());
            }

            return responseDto;
        }
        catch (FlurlHttpTimeoutException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Timeout occurred while fetching assessment. Status code: {StatusCode}, Response: {Response}",
                statusCode, responseBody);
            throw new FetchDataException($"Request timed out while fetching assessment {assessmentId}. Status code: {statusCode}", ex);
        }
        catch (FlurlHttpException ex)
        {
            string responseBody = await ex.GetResponseStringAsync();
            string? errorMsg = BuildFlurlErrorMessage($"An error occurred while fetching assessment {assessmentId}", ex);
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
            _logger.LogError(ex.Message, "An unexpected error occurred while fetching assessment {AssessmentId}", assessmentId);
            throw new FetchDataException($"An unexpected error occurred while fetching assessment {assessmentId}", ex);
        }
    }
    public async Task<SpecificationPerformanceResponse<AssessmentDto>> RunAssessmentPerformanceTests(Guid assessmentId)
    {
        string? uri = $"api/assessments/perf/{assessmentId}";
        try
        {
            string? apiBaseUrl = await RetrieveApiBaseUrl();
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
                    _logger.LogWarning($"Assessment with assessmentId {assessmentId} not found.");
                    throw new NotFoundException($"Assessment with assessmentId {assessmentId} not found.", assessmentId.ToString());
                }
                throw new FetchDataException($"Server returned non-success status code: {response.StatusCode}");
            }

            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            SpecificationPerformanceResponse<AssessmentDto>? responseDto =
                JsonSerializer.Deserialize<SpecificationPerformanceResponse<AssessmentDto>>(responseString, settings);

            return responseDto;
        }
        catch (FlurlHttpTimeoutException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Timeout occurred while PerformanceTests assessment. Status code: {StatusCode}, Response: {Response}",
                statusCode, responseBody);
            throw new FetchDataException($"Request timed out while PerformanceTests assessment {assessmentId}. Status code: {statusCode}", ex);
        }
        catch (FlurlHttpException ex)
        {
            string responseBody = await ex.GetResponseStringAsync();
            string? errorMsg = BuildFlurlErrorMessage($"An error occurred while PerformanceTests assessment {assessmentId}", ex);
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
            _logger.LogError(ex.Message, "An unexpected error occurred while fetching assessment {AssessmentId}", assessmentId);
            throw new FetchDataException($"An unexpected error occurred while fetching assessment {assessmentId}", ex);
        }
    }
    public async Task<CreateAssessmentControlResponse> CreateAssessmentControlAsync(CreateAssessmentControlJoinRequest data)
    {
        string uri = $"api/assessmentControls/assessment";
        try
        {
            string apiBaseUrl = await RetrieveApiBaseUrl();
            CreateAssessmentControlResponse response = await _flurlClient.Request($"{apiBaseUrl}{uri}")
                .AllowAnyHttpStatus()
                .PostJsonAsync(data).ReceiveJson<CreateAssessmentControlResponse>();

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while creating a assessmentControl .", ex);
            throw new FetchDataException("An error occurred while creating data.", ex);
        }
    }

    public async Task<AssessmentControlDto> UpdateAssessmentControlAsync(AssessmentControlDto data)
    {
        string uri = $"api/assessmentControls";
        try
        {

            var request = new UpdateAssessmentControlRequest() { AssessmentControl = data };
            string apiBaseUrl = await RetrieveApiBaseUrl();
            AssessmentControlDto response = await _flurlClient.Request($"{apiBaseUrl}{uri}")
                .AllowAnyHttpStatus()
                .PutJsonAsync(request)
                .ReceiveJson<AssessmentControlDto>();
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while updating a assessmentControl .", ex);
            throw new FetchDataException("An error occurred while updating data.", ex);
        }
    }

    public async Task DeleteAssessmentControlAsync(
    Guid assessmentId, 
    Guid controlId 
    )
    {
        string uri = $"api/assessment/assessmentControls/{assessmentId}/{controlId}";
        try
        {
            string apiBaseUrl = await RetrieveApiBaseUrl();
            await _flurlClient.Request($"{apiBaseUrl}{uri}")
                .AllowAnyHttpStatus()
                .DeleteAsync()
                .ReceiveJson<DeleteAssessmentControlResponse>();
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while deleting a assessmentControl .", ex);
            throw new FetchDataException("An error occurred while deleting data.", ex);
        }
    }

    public async Task<GetByIdAssessmentControlResponse> GetOneFullAssessmentControlByIdAsync(
    Guid assessmentId,
    Guid controlId
    )
    {
        try
        {
            string? apiBaseUrl = await RetrieveApiBaseUrl();
            string uri = $"api/assessmentControls/i/{assessmentId}";
            GetByIdAssessmentControlRequest request = new() { 
 AssessmentId = assessmentId, 
 ControlId = controlId, 
};
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
                    _logger.LogWarning($"AssessmentControl with assessmentControlId {controlId} not found.");
                    throw new NotFoundException($"AssessmentControl with assessmentControlId {controlId} not found.", controlId.ToString());
                }
                throw new FetchDataException($"Server returned non-success status code: {response.StatusCode}");
            }

            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            GetByIdAssessmentControlResponse? responseDto =
                JsonSerializer.Deserialize<GetByIdAssessmentControlResponse>(responseString, settings);

            if (responseDto?.AssessmentControl == null)
            {
                _logger.LogWarning($"Deserialized response contains no assessmentControl data for ID {controlId}");
                throw new NotFoundException($"No assessmentControl data found for assessmentControlId {controlId}", controlId.ToString());
            }

            return responseDto;
        }
        catch (FlurlHttpTimeoutException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Timeout occurred while fetching assessmentControl. Status code: {StatusCode}, Response: {Response}",
                statusCode, responseBody);
            throw new FetchDataException($"Request timed out while fetching assessmentControl {controlId}. Status code: {statusCode}", ex);
        }
        catch (FlurlHttpException ex)
        {
            string responseBody = await ex.GetResponseStringAsync();
            string? errorMsg = BuildFlurlErrorMessage($"An error occurred while fetching assessmentControl {controlId}", ex);
            _logger.LogError(ex, responseBody);
            throw new FetchDataException(responseBody, ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex.Message, "Error deserializing assessmentControl response for ID {AssessmentControlId}", controlId);
            throw new FetchDataException($"Error processing response for assessmentControl {controlId}", ex);
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "An unexpected error occurred while fetching assessmentControl {AssessmentControlId}", controlId);
            throw new FetchDataException($"An unexpected error occurred while fetching assessmentControl {controlId}", ex);
        }
    }

    public async Task<ListAssessmentControlResponse?> GetFilteredAssessmentControlsListAsync(int pageNumber, int pageSize, Dictionary<string, string> filters = null, List<PCIShieldLib.SharedKernel.Interfaces.Sort> sorting = null)
    {
        try
        {
            if (sorting == null && filters == null)
            {
                return await GetPagedAssessmentControlsListAsync(pageNumber, pageSize);
            }
           if (sorting == null && filters.Count == 1 && filters.TryGetValue("search", out string? filter))
            {
                var searchResults = await SearchAssessmentControlsAsync(filter);
                return new ListAssessmentControlResponse { AssessmentControls = searchResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList(), Count = searchResults.Count };
            }
            string uri = $"api/assessmentControls/filtered_list";
            string apiBaseUrl = await RetrieveApiBaseUrl();
            var request = new FilteredAssessmentControlRequest
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
            return JsonSerializer.Deserialize<ListAssessmentControlResponse>(responseString, settings);
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
            _logger.LogError(ex, "An unexpected error occurred while fetching filtered assessmentControl list.");
            throw new FetchDataException("An unexpected error occurred while fetching filtered assessmentControl list.", ex);
        }
    }
    public async Task<ListAssessmentControlResponse?> GetPagedAssessmentControlsListAsync(int pageNumber, int pageSize)
    {
        string uri = $"api/assessmentControls/paged_list";
        try
        {
            string apiBaseUrl = await RetrieveApiBaseUrl();
            GetAssessmentControlListRequest request = new() { PageNumber = pageNumber, PageSize = pageSize };
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
            ListAssessmentControlResponse? responseDto = JsonSerializer.Deserialize<ListAssessmentControlResponse>(responseString, settings);
            if (responseDto?.AssessmentControls != null) { }
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
            _logger.LogError(ex, "An unexpected error occurred while fetching paged assessmentControl list.");
            throw new FetchDataException("An unexpected error occurred while fetching paged assessmentControl list.", ex);
        }
    }

    public async Task<List<AssessmentControlDto>> SearchAssessmentControlsAsync(string searchTerm)
    {
        string uri = $"api/assessmentControls/search";
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
                    return new List<AssessmentControlDto>();
                }
                throw new FetchDataException($"Server returned non-success status code: {response.StatusCode}");
            }
            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            List<AssessmentControlDto>? assessmentControls = JsonSerializer.Deserialize<List<AssessmentControlDto>>(responseString, settings);
            return assessmentControls ?? new List<AssessmentControlDto>();
        }
        catch (FlurlHttpTimeoutException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Timeout occurred while searching assessmentControls. Status code: {StatusCode}, Response: {Response}", statusCode, responseBody);
            throw new FetchDataException($"Search request timed out. Status code: {statusCode}", ex);
        }
        catch (FlurlHttpException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "HTTP error occurred while searching assessmentControls. Status code: {StatusCode}, Response: {Response}", statusCode, responseBody);
            throw new FetchDataException($"An HTTP error occurred while searching. Status code: {statusCode}", ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error deserializing assessmentControl search response");
            throw new FetchDataException("Error processing search results", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while searching assessmentControls");
            throw new FetchDataException("An unexpected error occurred while searching assessmentControls", ex);
        }
    }
 
    public async Task<CreateControlEvidenceResponse> CreateControlEvidenceAsync(CreateControlEvidenceJoinRequest data)
    {
        string uri = $"api/controlEvidences/assessment";
        try
        {
            string apiBaseUrl = await RetrieveApiBaseUrl();
            CreateControlEvidenceResponse response = await _flurlClient.Request($"{apiBaseUrl}{uri}")
                .AllowAnyHttpStatus()
                .PostJsonAsync(data).ReceiveJson<CreateControlEvidenceResponse>();

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while creating a controlEvidence .", ex);
            throw new FetchDataException("An error occurred while creating data.", ex);
        }
    }

    public async Task<ControlEvidenceDto> UpdateControlEvidenceAsync(ControlEvidenceDto data)
    {
        string uri = $"api/controlEvidences";
        try
        {

            var request = new UpdateControlEvidenceRequest() { ControlEvidence = data };
            string apiBaseUrl = await RetrieveApiBaseUrl();
            ControlEvidenceDto response = await _flurlClient.Request($"{apiBaseUrl}{uri}")
                .AllowAnyHttpStatus()
                .PutJsonAsync(request)
                .ReceiveJson<ControlEvidenceDto>();
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while updating a controlEvidence .", ex);
            throw new FetchDataException("An error occurred while updating data.", ex);
        }
    }

    public async Task DeleteControlEvidenceAsync(
    Guid controlId, 
    Guid evidenceId, 
    Guid assessmentId 
    )
    {
        string uri = $"api/assessment/controlEvidences/{controlId}/{evidenceId}/{assessmentId}";
        try
        {
            string apiBaseUrl = await RetrieveApiBaseUrl();
            await _flurlClient.Request($"{apiBaseUrl}{uri}")
                .AllowAnyHttpStatus()
                .DeleteAsync()
                .ReceiveJson<DeleteControlEvidenceResponse>();
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while deleting a controlEvidence .", ex);
            throw new FetchDataException("An error occurred while deleting data.", ex);
        }
    }

    public async Task<GetByIdControlEvidenceResponse> GetOneFullControlEvidenceByIdAsync(
    Guid controlId,
    Guid evidenceId,
    Guid assessmentId
    )
    {
        try
        {
            string? apiBaseUrl = await RetrieveApiBaseUrl();
            string uri = $"api/controlEvidences/i/{assessmentId}";
            GetByIdControlEvidenceRequest request = new() { 
 ControlId = controlId, 
 EvidenceId = evidenceId, 
 AssessmentId = assessmentId, 
};
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
                    _logger.LogWarning($"ControlEvidence with controlEvidenceId {evidenceId} not found.");
                    throw new NotFoundException($"ControlEvidence with controlEvidenceId {evidenceId} not found.", evidenceId.ToString());
                }
                throw new FetchDataException($"Server returned non-success status code: {response.StatusCode}");
            }

            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            GetByIdControlEvidenceResponse? responseDto =
                JsonSerializer.Deserialize<GetByIdControlEvidenceResponse>(responseString, settings);

            if (responseDto?.ControlEvidence == null)
            {
                _logger.LogWarning($"Deserialized response contains no controlEvidence data for ID {evidenceId}");
                throw new NotFoundException($"No controlEvidence data found for controlEvidenceId {evidenceId}", evidenceId.ToString());
            }

            return responseDto;
        }
        catch (FlurlHttpTimeoutException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Timeout occurred while fetching controlEvidence. Status code: {StatusCode}, Response: {Response}",
                statusCode, responseBody);
            throw new FetchDataException($"Request timed out while fetching controlEvidence {evidenceId}. Status code: {statusCode}", ex);
        }
        catch (FlurlHttpException ex)
        {
            string responseBody = await ex.GetResponseStringAsync();
            string? errorMsg = BuildFlurlErrorMessage($"An error occurred while fetching controlEvidence {evidenceId}", ex);
            _logger.LogError(ex, responseBody);
            throw new FetchDataException(responseBody, ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex.Message, "Error deserializing controlEvidence response for ID {ControlEvidenceId}", evidenceId);
            throw new FetchDataException($"Error processing response for controlEvidence {evidenceId}", ex);
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "An unexpected error occurred while fetching controlEvidence {ControlEvidenceId}", evidenceId);
            throw new FetchDataException($"An unexpected error occurred while fetching controlEvidence {evidenceId}", ex);
        }
    }

    public async Task<ListControlEvidenceResponse?> GetFilteredControlEvidencesListAsync(int pageNumber, int pageSize, Dictionary<string, string> filters = null, List<PCIShieldLib.SharedKernel.Interfaces.Sort> sorting = null)
    {
        try
        {
            if (sorting == null && filters == null)
            {
                return await GetPagedControlEvidencesListAsync(pageNumber, pageSize);
            }
           if (sorting == null && filters.Count == 1 && filters.TryGetValue("search", out string? filter))
            {
                var searchResults = await SearchControlEvidencesAsync(filter);
                return new ListControlEvidenceResponse { ControlEvidences = searchResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList(), Count = searchResults.Count };
            }
            string uri = $"api/controlEvidences/filtered_list";
            string apiBaseUrl = await RetrieveApiBaseUrl();
            var request = new FilteredControlEvidenceRequest
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
            return JsonSerializer.Deserialize<ListControlEvidenceResponse>(responseString, settings);
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
            _logger.LogError(ex, "An unexpected error occurred while fetching filtered controlEvidence list.");
            throw new FetchDataException("An unexpected error occurred while fetching filtered controlEvidence list.", ex);
        }
    }
    public async Task<ListControlEvidenceResponse?> GetPagedControlEvidencesListAsync(int pageNumber, int pageSize)
    {
        string uri = $"api/controlEvidences/paged_list";
        try
        {
            string apiBaseUrl = await RetrieveApiBaseUrl();
            GetControlEvidenceListRequest request = new() { PageNumber = pageNumber, PageSize = pageSize };
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
            ListControlEvidenceResponse? responseDto = JsonSerializer.Deserialize<ListControlEvidenceResponse>(responseString, settings);
            if (responseDto?.ControlEvidences != null) { }
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
            _logger.LogError(ex, "An unexpected error occurred while fetching paged controlEvidence list.");
            throw new FetchDataException("An unexpected error occurred while fetching paged controlEvidence list.", ex);
        }
    }

    public async Task<List<ControlEvidenceDto>> SearchControlEvidencesAsync(string searchTerm)
    {
        string uri = $"api/controlEvidences/search";
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
                    return new List<ControlEvidenceDto>();
                }
                throw new FetchDataException($"Server returned non-success status code: {response.StatusCode}");
            }
            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            List<ControlEvidenceDto>? controlEvidences = JsonSerializer.Deserialize<List<ControlEvidenceDto>>(responseString, settings);
            return controlEvidences ?? new List<ControlEvidenceDto>();
        }
        catch (FlurlHttpTimeoutException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Timeout occurred while searching controlEvidences. Status code: {StatusCode}, Response: {Response}", statusCode, responseBody);
            throw new FetchDataException($"Search request timed out. Status code: {statusCode}", ex);
        }
        catch (FlurlHttpException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "HTTP error occurred while searching controlEvidences. Status code: {StatusCode}, Response: {Response}", statusCode, responseBody);
            throw new FetchDataException($"An HTTP error occurred while searching. Status code: {statusCode}", ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error deserializing controlEvidence search response");
            throw new FetchDataException("Error processing search results", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while searching controlEvidences");
            throw new FetchDataException("An unexpected error occurred while searching controlEvidences", ex);
        }
    }
    public async Task<CreateROCPackageResponse> CreateROCPackageAsync(ROCPackageDto data)
    {
        string uri = $"api/rocpackages";
        try
        {
            var request = new CreateROCPackageRequest { ROCPackage = data };
            string apiBaseUrl = await RetrieveApiBaseUrl();
            CreateROCPackageResponse response = await _flurlClient.Request($"{apiBaseUrl}{uri}")
                .AllowAnyHttpStatus()
                .PostJsonAsync(request).ReceiveJson<CreateROCPackageResponse>();

            var aa = response.ROCPackage;
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while creating a rocpackage .", ex);
            throw new FetchDataException("An error occurred while creating data.", ex);
        }
    }

    public async Task<ROCPackageDto> UpdateROCPackageAsync(ROCPackageDto data)
    {
        string uri = $"api/rocpackages";
        try
        {

            var request = new UpdateROCPackageRequest() { ROCPackage = data };
            string apiBaseUrl = await RetrieveApiBaseUrl();
            ROCPackageDto response = await _flurlClient.Request($"{apiBaseUrl}{uri}")
                .AllowAnyHttpStatus()
                .PutJsonAsync(request)
                .ReceiveJson<ROCPackageDto>();
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while updating a rocpackage .", ex);
            throw new FetchDataException("An error occurred while updating data.", ex);
        }
    }

    public async Task DeleteROCPackageAsync(Guid rocpackageId)
    {
        string uri = $"api/rocpackages/{rocpackageId}";
        try
        {
            string apiBaseUrl = await RetrieveApiBaseUrl();
            await _flurlClient.Request($"{apiBaseUrl}{uri}")
                .AllowAnyHttpStatus()
                .DeleteAsync()
                .ReceiveJson<DeleteROCPackageResponse>();
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while deleting a rocpackage .", ex);
            throw new FetchDataException("An error occurred while deleting data.", ex);
        }
    }

    public async Task<GetByIdROCPackageResponse> GetOneFullROCPackageByIdAsync(Guid rocpackageId)
    {
        try
        {
            string? apiBaseUrl = await RetrieveApiBaseUrl();
            string uri = $"api/rocpackages/i/{rocpackageId}";
             GetByIdROCPackageRequest request = new() { ROCPackageId = rocpackageId };
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
                    _logger.LogWarning($"ROCPackage with rocpackage not found.");
                    throw new NotFoundException($"ROCPackage rocpackageId not found.", rocpackageId.ToString());
                }
                throw new FetchDataException($"Server returned non-success status code: {response.StatusCode}");
            }

            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            GetByIdROCPackageResponse? responseDto =
                JsonSerializer.Deserialize<GetByIdROCPackageResponse>(responseString, settings);

            if (responseDto?.ROCPackage == null)
            {
                _logger.LogWarning($"Deserialized response contains no rocpackage data for ID ");
                throw new NotFoundException($"No rocpackage data found for rocpackageId", rocpackageId.ToString());
            }

            return responseDto;
        }
        catch (FlurlHttpTimeoutException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Timeout occurred while fetching rocpackage. Status code: {StatusCode}, Response: {Response}",
                statusCode, responseBody);
            throw new FetchDataException($"Request timed out while fetching rocpackage data for ID ");
        }
        catch (FlurlHttpException ex)
        {
            string responseBody = await ex.GetResponseStringAsync();
            string? errorMsg = BuildFlurlErrorMessage($"An error occurred while fetching rocpackage ", ex);
            _logger.LogError(ex, responseBody);
            throw new FetchDataException(responseBody, ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex.Message, "Error deserializing rocpackage response for ID {ROCPackageId}", rocpackageId);
            throw new FetchDataException($"Error processing response for rocpackage {rocpackageId}", ex);
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "An unexpected error occurred while fetching rocpackage ROCPackage");
            throw new FetchDataException($"An unexpected error occurred while fetching rocpackage ", ex);
        }
    }

    public async Task<ListROCPackageResponse?> GetFilteredROCPackagesListAsync(int pageNumber, int pageSize, Dictionary<string, string> filters = null, List< PCIShieldLib.SharedKernel.Interfaces.Sort> sorting = null)
    {
        try
        {
            if (sorting == null && filters == null)
            {
                return await GetPagedROCPackagesListAsync(pageNumber, pageSize);
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
    public async Task<ListROCPackageResponse?> GetPagedROCPackagesListAsync(int pageNumber, int pageSize)
    {
        string uri = $"api/rocpackages/paged_list";
        try
        {
            string apiBaseUrl = await RetrieveApiBaseUrl();
            GetROCPackageListRequest request = new() { PageNumber = pageNumber, PageSize = pageSize };
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
    public async Task<ListPaymentPageResponse?> GetFilteredPaymentPagesListAsync(
        Guid paymentChannelId,
        int pageNumber,
        int pageSize,
        Dictionary<string, string> filters = null,
        List<PCIShieldLib.SharedKernel.Interfaces.Sort> sorting = null)
    {
        try
        {
            if (sorting == null && filters == null)
            {
                return await GetPagedPaymentPagesListAsync(
                    pageNumber, 
                    pageSize, 
                    paymentChannelId
                    );
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
    public async Task<ListPaymentPageResponse?> GetPagedPaymentPagesListAsync(

        int pageNumber, 
        int pageSize,
        Guid paymentChannelId

        )
    {
        string uri = $"api/paymentPages/paged_list";
        try
        {
            string apiBaseUrl = await RetrieveApiBaseUrl();
            GetPaymentPageListRequest request = new() {

                PageNumber = pageNumber,
                PageSize = pageSize,
                PaymentChannelId = paymentChannelId

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
    public async Task<ListScriptResponse?> GetFilteredScriptsListAsync(
        Guid paymentPageId,
        int pageNumber,
        int pageSize,
        Dictionary<string, string> filters = null,
        List<PCIShieldLib.SharedKernel.Interfaces.Sort> sorting = null)
    {
        try
        {
            if (sorting == null && filters == null)
            {
                return await GetPagedScriptsListAsync(
                    pageNumber, 
                    pageSize, 
                    paymentPageId
                    );
            }
           if (sorting == null && filters.Count == 1 && filters.TryGetValue("search", out string? filter))
            {
                var searchResults = await SearchScriptsAsync(filter);
                return new ListScriptResponse { Scripts = searchResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList(), Count = searchResults.Count };
            }
            string uri = $"api/scripts/filtered_list";
            string apiBaseUrl = await RetrieveApiBaseUrl();
            var request = new FilteredScriptRequest
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
            return JsonSerializer.Deserialize<ListScriptResponse>(responseString, settings);
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
            _logger.LogError(ex, "An unexpected error occurred while fetching filtered script list.");
            throw new FetchDataException("An unexpected error occurred while fetching filtered script list.", ex);
        }
    }
    public async Task<ListScriptResponse?> GetPagedScriptsListAsync(

        int pageNumber, 
        int pageSize,
        Guid paymentPageId

        )
    {
        string uri = $"api/scripts/paged_list";
        try
        {
            string apiBaseUrl = await RetrieveApiBaseUrl();
            GetScriptListRequest request = new() {

                PageNumber = pageNumber,
                PageSize = pageSize,
                PaymentPageId = paymentPageId

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
            ListScriptResponse? responseDto = JsonSerializer.Deserialize<ListScriptResponse>(responseString, settings);
            if (responseDto?.Scripts != null) { }
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
            _logger.LogError(ex, "An unexpected error occurred while fetching paged script list.");
            throw new FetchDataException("An unexpected error occurred while fetching paged script list.", ex);
        }
    }
    public async Task<List<ScriptDto>> SearchScriptsAsync(string searchTerm)
    {
        string uri = $"api/scripts/search";
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
                    return new List<ScriptDto>();
                }
                throw new FetchDataException($"Server returned non-success status code: {response.StatusCode}");
            }
            JsonSerializerOptions settings = GJset.GetSystemTextJsonSettings();
            List<ScriptDto>? scripts = JsonSerializer.Deserialize<List<ScriptDto>>(responseString, settings);
            return scripts ?? new List<ScriptDto>();
        }
        catch (FlurlHttpTimeoutException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "Timeout occurred while searching scripts. Status code: {StatusCode}, Response: {Response}", statusCode, responseBody);
            throw new FetchDataException($"Search request timed out. Status code: {statusCode}", ex);
        }
        catch (FlurlHttpException ex)
        {
            int? statusCode = ex.Call.Response?.StatusCode;
            string? responseBody = await ex.GetResponseStringAsync();
            _logger.LogError(ex.Message, "HTTP error occurred while searching scripts. Status code: {StatusCode}, Response: {Response}", statusCode, responseBody);
            throw new FetchDataException($"An HTTP error occurred while searching. Status code: {statusCode}", ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error deserializing script search response");
            throw new FetchDataException("Error processing search results", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while searching scripts");
            throw new FetchDataException("An unexpected error occurred while searching scripts", ex);
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
        string uri = $"api/merchants/{merchantId}";
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
            string uri = $"api/merchants/i/{merchantId}";
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

