using System;

using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.AssetControl
{
    public class CreateAssetControlRequest : BaseRequest
    {
        
        public Guid AssetId { get; set; }
        public Guid ControlId { get; set; }
        
        public AssetControlDto AssetControl { get; set; }
        
        public CreateAssetControlRequest()
        {
        }
        
    }
    public class CreateAssetControlJoinRequest
    {
        public AssetControlDto AssetControl { get; set; }
        public int RowId { get; set; }
        public AssetDto Asset { get; set; }
        public Guid AssetId { get; set; }
        public ControlDto Control { get; set; }
        public Guid ControlId { get; set; }
    }
    
    public class GetAssetControlTransactionHistoryRequest
    {
        public Guid RowId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
    public class TransactionDto
    {
        public Guid TransactionId { get; set; }
        public Guid RowId { get; set; }
        public string TransactionType { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
    }
    public class GetAssetControlPurchaseHistoryRequest
    {
        public Guid RowId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class AssetControlPurchaseHistoryDto
    {
        public Guid RowId { get; set; }
        public decimal TotalPurchased { get; set; }
        public int TotalOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class CanPlaceOrderRequest
    {
        public Guid RowId { get; set; }
        public decimal OrderAmount { get; set; }
    }
    public class CanPlaceOrderResponse
    {
        public bool CanPlaceOrder { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class ValidateAssetControlStatusRequest
    {
        public Guid RowId { get; set; }
        public AssetControlOperationType OperationType { get; set; }
    }
    public class ValidateAssetControlStatusResponse
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
    }
    public enum AssetControlOperationType
    {
        PlaceOrder,
        RequestCredit,
    }
    public class AssetControlPaymentTermsDto
    {
        public Guid RowId { get; set; }
        public int PaymentTermDays { get; set; }
        public decimal CreditLimitAmount { get; set; }
    }
    public class UpdateAssetControlPaymentTermsRequest
    {
        public Guid RowId { get; set; }
        public int NewPaymentTermDays { get; set; }
    }
}

