using System;

using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.NetworkSegmentation
{
    public class CreateNetworkSegmentationRequest : BaseRequest
    {
        
        public Guid MerchantId { get; set; }
        
        public NetworkSegmentationDto NetworkSegmentation { get; set; }
        
        public CreateNetworkSegmentationRequest()
        {
            if (NetworkSegmentation != null)
            {
                NetworkSegmentation.NetworkSegmentationId = Guid.NewGuid();
            }
        }
        
    }
    public class GetNetworkSegmentationTransactionHistoryRequest
    {
        public Guid NetworkSegmentationId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
    public class TransactionDto
    {
        public Guid TransactionId { get; set; }
        public Guid NetworkSegmentationId { get; set; }
        public string TransactionType { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
    }
    public class GetNetworkSegmentationPurchaseHistoryRequest
    {
        public Guid NetworkSegmentationId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class NetworkSegmentationPurchaseHistoryDto
    {
        public Guid NetworkSegmentationId { get; set; }
        public decimal TotalPurchased { get; set; }
        public int TotalOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class CanPlaceOrderRequest
    {
        public Guid NetworkSegmentationId { get; set; }
        public decimal OrderAmount { get; set; }
    }
    public class CanPlaceOrderResponse
    {
        public bool CanPlaceOrder { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class ValidateNetworkSegmentationStatusRequest
    {
        public Guid NetworkSegmentationId { get; set; }
        public NetworkSegmentationOperationType OperationType { get; set; }
    }
    public class ValidateNetworkSegmentationStatusResponse
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
    }
    public enum NetworkSegmentationOperationType
    {
        PlaceOrder,
        RequestCredit,
    }
    public class NetworkSegmentationPaymentTermsDto
    {
        public Guid NetworkSegmentationId { get; set; }
        public int PaymentTermDays { get; set; }
        public decimal CreditLimitAmount { get; set; }
    }
    public class UpdateNetworkSegmentationPaymentTermsRequest
    {
        public Guid NetworkSegmentationId { get; set; }
        public int NewPaymentTermDays { get; set; }
    }
}

