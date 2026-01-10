using BlazorMauiShared.Models.Assessment;
using BlazorMauiShared.Models.AssessmentControl;
using BlazorMauiShared.Models.ControlEvidence;
using BlazorMauiShared.Models.Merchant;
using BlazorMauiShared.Models.PaymentPage;
using BlazorMauiShared.Models.ROCPackage;
using BlazorMauiShared.Models.ScanSchedule;
using BlazorMauiShared.Models.Script;
using BlazorMauiShared.Models.Vulnerability;

using PCIShield.BlazorMauiShared.Models;
using PCIShield.BlazorMauiShared.Models.Assessment;
using PCIShield.Domain.ModelsDto;

namespace PCIShield.Client.Services.Assessment;

public interface IHttpAssessmentClientService
{
    Task<UpdateAssessmentResponse> UpdatePutAssessmentAsync(UpdateAssessmentRequest data);
    Task<CreateAssessmentResponse> CreatePostAssessmentAsync(CreateAssessmentRequest data);
    Task<GetByIdAssessmentResponse> GetLastCreatedAssessmentAsync();
    Task<DeleteAssessmentResponse> DeleteAssessmentAsync(Guid assessmentId);
    Task<ListAssessmentResponse?> GetFilteredAssessmentListAsync(int pageNumber, int pageSize, Dictionary<string, string> filters = null, List<PCIShieldLib.SharedKernel.Interfaces.Sort> sorting = null);
    Task<ListAssessmentResponse?> GetPagedAssessmentsListAsync(int pageNumber, int pageSize);
    Task<List<AssessmentDto>> SearchAssessmentByAssessmentAsync(string searchTerm);
    Task<GetByIdAssessmentResponse> GetOneFullAssessmentByIdAsync(Guid assessmentId, bool withPostGraph = false);
    Task<SpecificationPerformanceResponse<AssessmentDto>> RunAssessmentPerformanceTests(Guid assessmentId);
    Task<CreateAssessmentControlResponse> CreateAssessmentControlAsync(CreateAssessmentControlJoinRequest data);
    Task<AssessmentControlDto> UpdateAssessmentControlAsync(AssessmentControlDto data);

    Task DeleteAssessmentControlAsync(
        Guid assessmentId, 
        Guid controlId 
    );

    Task<GetByIdAssessmentControlResponse> GetOneFullAssessmentControlByIdAsync(
        Guid assessmentId,
        Guid controlId
    );

    Task<ListAssessmentControlResponse?> GetFilteredAssessmentControlsListAsync(int pageNumber, int pageSize, Dictionary<string, string> filters = null, List<PCIShieldLib.SharedKernel.Interfaces.Sort> sorting = null);
    Task<ListAssessmentControlResponse?> GetPagedAssessmentControlsListAsync(int pageNumber, int pageSize);
    Task<List<AssessmentControlDto>> SearchAssessmentControlsAsync(string searchTerm);
    Task<CreateControlEvidenceResponse> CreateControlEvidenceAsync(CreateControlEvidenceJoinRequest data);
    Task<ControlEvidenceDto> UpdateControlEvidenceAsync(ControlEvidenceDto data);

    Task DeleteControlEvidenceAsync(
        Guid controlId, 
        Guid evidenceId, 
        Guid assessmentId 
    );

    Task<GetByIdControlEvidenceResponse> GetOneFullControlEvidenceByIdAsync(
        Guid controlId,
        Guid evidenceId,
        Guid assessmentId
    );

    Task<ListControlEvidenceResponse?> GetFilteredControlEvidencesListAsync(int pageNumber, int pageSize, Dictionary<string, string> filters = null, List<PCIShieldLib.SharedKernel.Interfaces.Sort> sorting = null);
    Task<ListControlEvidenceResponse?> GetPagedControlEvidencesListAsync(int pageNumber, int pageSize);
    Task<List<ControlEvidenceDto>> SearchControlEvidencesAsync(string searchTerm);
    Task<CreateROCPackageResponse> CreateROCPackageAsync(ROCPackageDto data);
    Task<ROCPackageDto> UpdateROCPackageAsync(ROCPackageDto data);
    Task DeleteROCPackageAsync(Guid rocpackageId);
    Task<GetByIdROCPackageResponse> GetOneFullROCPackageByIdAsync(Guid rocpackageId);
    Task<ListROCPackageResponse?> GetFilteredROCPackagesListAsync(int pageNumber, int pageSize, Dictionary<string, string> filters = null, List< PCIShieldLib.SharedKernel.Interfaces.Sort> sorting = null);
    Task<ListROCPackageResponse?> GetPagedROCPackagesListAsync(int pageNumber, int pageSize);
    Task<List<ROCPackageDto>> SearchROCPackagesAsync(string searchTerm);

    Task<ListScanScheduleResponse?> GetFilteredScanSchedulesListAsync(
        Guid assetId,
        int pageNumber,
        int pageSize,
        Dictionary<string, string> filters = null,
        List<PCIShieldLib.SharedKernel.Interfaces.Sort> sorting = null);

    Task<ListScanScheduleResponse?> GetPagedScanSchedulesListAsync(

        int pageNumber, 
        int pageSize,
        Guid assetId

    );

    Task<List<ScanScheduleDto>> SearchScanSchedulesAsync(string searchTerm);

    Task<ListVulnerabilityResponse?> GetFilteredVulnerabilitiesListAsync(
        Guid assetId,
        int pageNumber,
        int pageSize,
        Dictionary<string, string> filters = null,
        List<PCIShieldLib.SharedKernel.Interfaces.Sort> sorting = null);

    Task<ListVulnerabilityResponse?> GetPagedVulnerabilitiesListAsync(

        int pageNumber, 
        int pageSize,
        Guid assetId

    );

    Task<List<VulnerabilityDto>> SearchVulnerabilitiesAsync(string searchTerm);

    Task<ListPaymentPageResponse?> GetFilteredPaymentPagesListAsync(
        Guid paymentChannelId,
        int pageNumber,
        int pageSize,
        Dictionary<string, string> filters = null,
        List<PCIShieldLib.SharedKernel.Interfaces.Sort> sorting = null);

    Task<ListPaymentPageResponse?> GetPagedPaymentPagesListAsync(

        int pageNumber, 
        int pageSize,
        Guid paymentChannelId

    );

    Task<List<PaymentPageDto>> SearchPaymentPagesAsync(string searchTerm);

    Task<ListScriptResponse?> GetFilteredScriptsListAsync(
        Guid paymentPageId,
        int pageNumber,
        int pageSize,
        Dictionary<string, string> filters = null,
        List<PCIShieldLib.SharedKernel.Interfaces.Sort> sorting = null);

    Task<ListScriptResponse?> GetPagedScriptsListAsync(

        int pageNumber, 
        int pageSize,
        Guid paymentPageId

    );

    Task<List<ScriptDto>> SearchScriptsAsync(string searchTerm);
    Task<CreateMerchantResponse> CreateMerchantAsync(MerchantDto data);
    Task<MerchantDto> UpdateMerchantAsync(MerchantDto data);
    Task DeleteMerchantAsync(Guid merchantId);
    Task<GetByIdMerchantResponse> GetOneFullMerchantByIdAsync(Guid merchantId);
    Task<ListMerchantResponse?> GetFilteredMerchantsListAsync(int pageNumber, int pageSize, Dictionary<string, string> filters = null, List< PCIShieldLib.SharedKernel.Interfaces.Sort> sorting = null);
    Task<ListMerchantResponse?> GetPagedMerchantsListAsync(int pageNumber, int pageSize);
    Task<List<MerchantDto>> SearchMerchantsAsync(string searchTerm);
    void Dispose();
}