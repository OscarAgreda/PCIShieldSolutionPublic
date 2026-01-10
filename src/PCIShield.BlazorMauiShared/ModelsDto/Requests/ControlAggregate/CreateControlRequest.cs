using System;

using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.Control
{
    public class CreateControlRequest : BaseRequest
    {
        public ControlDto Control { get; set; }
        
        public CreateControlRequest()
        {
            if (Control != null)
            {
                Control.ControlId = Guid.NewGuid();
            }
        }
        
    }
    public class GetControlTransactionHistoryRequest
    {
        public Guid ControlId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
    public class TransactionDto
    {
        public Guid TransactionId { get; set; }
        public Guid ControlId { get; set; }
        public string TransactionType { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
    }
    public class GetControlPurchaseHistoryRequest
    {
        public Guid ControlId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class ControlPurchaseHistoryDto
    {
        public Guid ControlId { get; set; }
        public decimal TotalPurchased { get; set; }
        public int TotalOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class CanPlaceOrderRequest
    {
        public Guid ControlId { get; set; }
        public decimal OrderAmount { get; set; }
    }
    public class CanPlaceOrderResponse
    {
        public bool CanPlaceOrder { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class ValidateControlStatusRequest
    {
        public Guid ControlId { get; set; }
        public ControlOperationType OperationType { get; set; }
    }
    public class ValidateControlStatusResponse
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
    }
    public enum ControlOperationType
    {
        PlaceOrder,
        RequestCredit,
    }
    public class ControlPaymentTermsDto
    {
        public Guid ControlId { get; set; }
        public int PaymentTermDays { get; set; }
        public decimal CreditLimitAmount { get; set; }
    }
    public class UpdateControlPaymentTermsRequest
    {
        public Guid ControlId { get; set; }
        public int NewPaymentTermDays { get; set; }
    }
}

