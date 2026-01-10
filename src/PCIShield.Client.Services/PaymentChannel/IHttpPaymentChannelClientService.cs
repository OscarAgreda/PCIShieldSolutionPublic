using BlazorMauiShared.Models.Merchant;
using BlazorMauiShared.Models.PaymentChannel;
using BlazorMauiShared.Models.PaymentPage;
using BlazorMauiShared.Models.ROCPackage;
using BlazorMauiShared.Models.ScanSchedule;
using BlazorMauiShared.Models.Vulnerability;

using PCIShield.BlazorMauiShared.Models;
using PCIShield.BlazorMauiShared.Models.PaymentChannel;
using PCIShield.Domain.ModelsDto;

namespace PCIShield.Client.Services.PaymentChannel;

public interface IHttpPaymentChannelClientService
{
    Task<UpdatePaymentChannelResponse> UpdatePutPaymentChannelAsync(UpdatePaymentChannelRequest data);
    Task<CreatePaymentChannelResponse> CreatePostPaymentChannelAsync(CreatePaymentChannelRequest data);
    Task<GetByIdPaymentChannelResponse> GetLastCreatedPaymentChannelAsync();
    Task<DeletePaymentChannelResponse> DeletePaymentChannelAsync(Guid paymentChannelId);
    Task<ListPaymentChannelResponse?> GetFilteredPaymentChannelListAsync(int pageNumber, int pageSize, Dictionary<string, string> filters = null, List<PCIShieldLib.SharedKernel.Interfaces.Sort> sorting = null);
    Task<ListPaymentChannelResponse?> GetPagedPaymentChannelsListAsync(int pageNumber, int pageSize);
    Task<List<PaymentChannelDto>> SearchPaymentChannelByPaymentChannelAsync(string searchTerm);
    Task<GetByIdPaymentChannelResponse> GetOneFullPaymentChannelByIdAsync(Guid paymentChannelId, bool withPostGraph = false);
    Task<SpecificationPerformanceResponse<PaymentChannelDto>> RunPaymentChannelPerformanceTests(Guid paymentChannelId);
    Task<CreatePaymentPageResponse> CreatePaymentPageAsync(PaymentPageDto data);
    Task<PaymentPageDto> UpdatePaymentPageAsync(PaymentPageDto data);
    Task DeletePaymentPageAsync(Guid paymentPageId);
    Task<GetByIdPaymentPageResponse> GetOneFullPaymentPageByIdAsync(Guid paymentPageId);
    Task<ListPaymentPageResponse?> GetFilteredPaymentPagesListAsync(int pageNumber, int pageSize, Dictionary<string, string> filters = null, List< PCIShieldLib.SharedKernel.Interfaces.Sort> sorting = null);
    Task<ListPaymentPageResponse?> GetPagedPaymentPagesListAsync(int pageNumber, int pageSize);
    Task<List<PaymentPageDto>> SearchPaymentPagesAsync(string searchTerm);

    Task<ListROCPackageResponse?> GetFilteredROCPackagesListAsync(
        Guid assessmentId,
        int pageNumber,
        int pageSize,
        Dictionary<string, string> filters = null,
        List<PCIShieldLib.SharedKernel.Interfaces.Sort> sorting = null);

    Task<ListROCPackageResponse?> GetPagedROCPackagesListAsync(

        int pageNumber, 
        int pageSize,
        Guid assessmentId

    );

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
    Task<CreateMerchantResponse> CreateMerchantAsync(MerchantDto data);
    Task<MerchantDto> UpdateMerchantAsync(MerchantDto data);
    Task DeleteMerchantAsync(Guid merchantId);
    Task<GetByIdMerchantResponse> GetOneFullMerchantByIdAsync(Guid merchantId);
    Task<ListMerchantResponse?> GetFilteredMerchantsListAsync(int pageNumber, int pageSize, Dictionary<string, string> filters = null, List< PCIShieldLib.SharedKernel.Interfaces.Sort> sorting = null);
    Task<ListMerchantResponse?> GetPagedMerchantsListAsync(int pageNumber, int pageSize);
    Task<List<MerchantDto>> SearchMerchantsAsync(string searchTerm);
    void Dispose();
}