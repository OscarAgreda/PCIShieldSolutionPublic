using System;

using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.ServiceProvider
{
    public class CreateServiceProviderRequest : BaseRequest
    {
        
        public Guid MerchantId { get; set; }
        
        public ServiceProviderDto ServiceProvider { get; set; }
        
        public CreateServiceProviderRequest()
        {
            if (ServiceProvider != null)
            {
                ServiceProvider.ServiceProviderId = Guid.NewGuid();
            }
        }
        
    }
    public class GetServiceProviderTransactionHistoryRequest
    {
        public Guid ServiceProviderId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
    public class TransactionDto
    {
        public Guid TransactionId { get; set; }
        public Guid ServiceProviderId { get; set; }
        public string TransactionType { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
    }
    public class GetServiceProviderPurchaseHistoryRequest
    {
        public Guid ServiceProviderId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class ServiceProviderPurchaseHistoryDto
    {
        public Guid ServiceProviderId { get; set; }
        public decimal TotalPurchased { get; set; }
        public int TotalOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class CanPlaceOrderRequest
    {
        public Guid ServiceProviderId { get; set; }
        public decimal OrderAmount { get; set; }
    }
    public class CanPlaceOrderResponse
    {
        public bool CanPlaceOrder { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class ValidateServiceProviderStatusRequest
    {
        public Guid ServiceProviderId { get; set; }
        public ServiceProviderOperationType OperationType { get; set; }
    }
    public class ValidateServiceProviderStatusResponse
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
    }
    public enum ServiceProviderOperationType
    {
        PlaceOrder,
        RequestCredit,
    }
    public class ServiceProviderPaymentTermsDto
    {
        public Guid ServiceProviderId { get; set; }
        public int PaymentTermDays { get; set; }
        public decimal CreditLimitAmount { get; set; }
    }
    public class UpdateServiceProviderPaymentTermsRequest
    {
        public Guid ServiceProviderId { get; set; }
        public int NewPaymentTermDays { get; set; }
    }
}

