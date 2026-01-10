using System;

using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.PaymentPage
{
    public class CreatePaymentPageRequest : BaseRequest
    {
        
        public Guid PaymentChannelId { get; set; }
        
        public PaymentPageDto PaymentPage { get; set; }
        
        public CreatePaymentPageRequest()
        {
            if (PaymentPage != null)
            {
                PaymentPage.PaymentPageId = Guid.NewGuid();
            }
        }
        
    }
    public class GetPaymentPageTransactionHistoryRequest
    {
        public Guid PaymentPageId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
    public class TransactionDto
    {
        public Guid TransactionId { get; set; }
        public Guid PaymentPageId { get; set; }
        public string TransactionType { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
    }
    public class GetPaymentPagePurchaseHistoryRequest
    {
        public Guid PaymentPageId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class PaymentPagePurchaseHistoryDto
    {
        public Guid PaymentPageId { get; set; }
        public decimal TotalPurchased { get; set; }
        public int TotalOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class CanPlaceOrderRequest
    {
        public Guid PaymentPageId { get; set; }
        public decimal OrderAmount { get; set; }
    }
    public class CanPlaceOrderResponse
    {
        public bool CanPlaceOrder { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class ValidatePaymentPageStatusRequest
    {
        public Guid PaymentPageId { get; set; }
        public PaymentPageOperationType OperationType { get; set; }
    }
    public class ValidatePaymentPageStatusResponse
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
    }
    public enum PaymentPageOperationType
    {
        PlaceOrder,
        RequestCredit,
    }
    public class PaymentPagePaymentTermsDto
    {
        public Guid PaymentPageId { get; set; }
        public int PaymentTermDays { get; set; }
        public decimal CreditLimitAmount { get; set; }
    }
    public class UpdatePaymentPagePaymentTermsRequest
    {
        public Guid PaymentPageId { get; set; }
        public int NewPaymentTermDays { get; set; }
    }
}

