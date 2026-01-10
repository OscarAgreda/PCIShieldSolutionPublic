using System;

using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.Asset
{
    public class CreateAssetRequest : BaseRequest
    {
        
        public Guid MerchantId { get; set; }
        
        public AssetDto Asset { get; set; }
        
        public CreateAssetRequest()
        {
            if (Asset != null)
            {
                Asset.AssetId = Guid.NewGuid();
            }
        }
        
    }
    public class GetAssetTransactionHistoryRequest
    {
        public Guid AssetId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
    public class TransactionDto
    {
        public Guid TransactionId { get; set; }
        public Guid AssetId { get; set; }
        public string TransactionType { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
    }
    public class GetAssetPurchaseHistoryRequest
    {
        public Guid AssetId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class AssetPurchaseHistoryDto
    {
        public Guid AssetId { get; set; }
        public decimal TotalPurchased { get; set; }
        public int TotalOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class CanPlaceOrderRequest
    {
        public Guid AssetId { get; set; }
        public decimal OrderAmount { get; set; }
    }
    public class CanPlaceOrderResponse
    {
        public bool CanPlaceOrder { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class ValidateAssetStatusRequest
    {
        public Guid AssetId { get; set; }
        public AssetOperationType OperationType { get; set; }
    }
    public class ValidateAssetStatusResponse
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
    }
    public enum AssetOperationType
    {
        PlaceOrder,
        RequestCredit,
    }
    public class AssetPaymentTermsDto
    {
        public Guid AssetId { get; set; }
        public int PaymentTermDays { get; set; }
        public decimal CreditLimitAmount { get; set; }
    }
    public class UpdateAssetPaymentTermsRequest
    {
        public Guid AssetId { get; set; }
        public int NewPaymentTermDays { get; set; }
    }
}

