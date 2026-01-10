using System;

using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.CompensatingControl
{
    public class CreateCompensatingControlRequest : BaseRequest
    {
        
        public Guid ControlId { get; set; }
        public Guid MerchantId { get; set; }
        
        public CompensatingControlDto CompensatingControl { get; set; }
        
        public CreateCompensatingControlRequest()
        {
            if (CompensatingControl != null)
            {
                CompensatingControl.CompensatingControlId = Guid.NewGuid();
            }
        }
        
    }
    public class GetCompensatingControlTransactionHistoryRequest
    {
        public Guid CompensatingControlId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
    public class TransactionDto
    {
        public Guid TransactionId { get; set; }
        public Guid CompensatingControlId { get; set; }
        public string TransactionType { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
    }
    public class GetCompensatingControlPurchaseHistoryRequest
    {
        public Guid CompensatingControlId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class CompensatingControlPurchaseHistoryDto
    {
        public Guid CompensatingControlId { get; set; }
        public decimal TotalPurchased { get; set; }
        public int TotalOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class CanPlaceOrderRequest
    {
        public Guid CompensatingControlId { get; set; }
        public decimal OrderAmount { get; set; }
    }
    public class CanPlaceOrderResponse
    {
        public bool CanPlaceOrder { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class ValidateCompensatingControlStatusRequest
    {
        public Guid CompensatingControlId { get; set; }
        public CompensatingControlOperationType OperationType { get; set; }
    }
    public class ValidateCompensatingControlStatusResponse
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
    }
    public enum CompensatingControlOperationType
    {
        PlaceOrder,
        RequestCredit,
    }
    public class CompensatingControlPaymentTermsDto
    {
        public Guid CompensatingControlId { get; set; }
        public int PaymentTermDays { get; set; }
        public decimal CreditLimitAmount { get; set; }
    }
    public class UpdateCompensatingControlPaymentTermsRequest
    {
        public Guid CompensatingControlId { get; set; }
        public int NewPaymentTermDays { get; set; }
    }
}

