using System;

using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.PaymentChannel
{
    public class CreatePaymentChannelRequest : BaseRequest
    {
        
        public Guid MerchantId { get; set; }
        
        public PaymentChannelDto PaymentChannel { get; set; }
        
        public CreatePaymentChannelRequest()
        {
            if (PaymentChannel != null)
            {
                PaymentChannel.PaymentChannelId = Guid.NewGuid();
            }
        }
        
    }
    public class GetPaymentChannelTransactionHistoryRequest
    {
        public Guid PaymentChannelId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
    public class TransactionDto
    {
        public Guid TransactionId { get; set; }
        public Guid PaymentChannelId { get; set; }
        public string TransactionType { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
    }
    public class GetPaymentChannelPurchaseHistoryRequest
    {
        public Guid PaymentChannelId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class PaymentChannelPurchaseHistoryDto
    {
        public Guid PaymentChannelId { get; set; }
        public decimal TotalPurchased { get; set; }
        public int TotalOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class CanPlaceOrderRequest
    {
        public Guid PaymentChannelId { get; set; }
        public decimal OrderAmount { get; set; }
    }
    public class CanPlaceOrderResponse
    {
        public bool CanPlaceOrder { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class ValidatePaymentChannelStatusRequest
    {
        public Guid PaymentChannelId { get; set; }
        public PaymentChannelOperationType OperationType { get; set; }
    }
    public class ValidatePaymentChannelStatusResponse
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
    }
    public enum PaymentChannelOperationType
    {
        PlaceOrder,
        RequestCredit,
    }
    public class PaymentChannelPaymentTermsDto
    {
        public Guid PaymentChannelId { get; set; }
        public int PaymentTermDays { get; set; }
        public decimal CreditLimitAmount { get; set; }
    }
    public class UpdatePaymentChannelPaymentTermsRequest
    {
        public Guid PaymentChannelId { get; set; }
        public int NewPaymentTermDays { get; set; }
    }
}

