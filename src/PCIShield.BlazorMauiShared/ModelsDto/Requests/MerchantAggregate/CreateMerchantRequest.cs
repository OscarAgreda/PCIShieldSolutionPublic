using System;

using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.Merchant
{
    public class CreateMerchantRequest : BaseRequest
    {
        public MerchantDto Merchant { get; set; }
        
        public CreateMerchantRequest()
        {
            if (Merchant != null)
            {
                Merchant.MerchantId = Guid.NewGuid();
            }
        }
        
    }
    public class GetMerchantTransactionHistoryRequest
    {
        public Guid MerchantId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
    public class TransactionDto
    {
        public Guid TransactionId { get; set; }
        public Guid MerchantId { get; set; }
        public string TransactionType { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
    }
    public class GetMerchantPurchaseHistoryRequest
    {
        public Guid MerchantId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class MerchantPurchaseHistoryDto
    {
        public Guid MerchantId { get; set; }
        public decimal TotalPurchased { get; set; }
        public int TotalOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class CanPlaceOrderRequest
    {
        public Guid MerchantId { get; set; }
        public decimal OrderAmount { get; set; }
    }
    public class CanPlaceOrderResponse
    {
        public bool CanPlaceOrder { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class ValidateMerchantStatusRequest
    {
        public Guid MerchantId { get; set; }
        public MerchantOperationType OperationType { get; set; }
    }
    public class ValidateMerchantStatusResponse
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
    }
    public enum MerchantOperationType
    {
        PlaceOrder,
        RequestCredit,
    }
    public class MerchantPaymentTermsDto
    {
        public Guid MerchantId { get; set; }
        public int PaymentTermDays { get; set; }
        public decimal CreditLimitAmount { get; set; }
    }
    public class UpdateMerchantPaymentTermsRequest
    {
        public Guid MerchantId { get; set; }
        public int NewPaymentTermDays { get; set; }
    }
}

