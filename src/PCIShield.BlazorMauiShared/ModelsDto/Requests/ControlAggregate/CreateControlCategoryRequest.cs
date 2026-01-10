using System;

using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.ControlCategory
{
    public class CreateControlCategoryRequest : BaseRequest
    {
        public ControlCategoryDto ControlCategory { get; set; }
        
        public CreateControlCategoryRequest()
        {
            if (ControlCategory != null)
            {
                ControlCategory.ControlCategoryId = Guid.NewGuid();
            }
        }
        
    }
    public class GetControlCategoryTransactionHistoryRequest
    {
        public Guid ControlCategoryId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
    public class TransactionDto
    {
        public Guid TransactionId { get; set; }
        public Guid ControlCategoryId { get; set; }
        public string TransactionType { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
    }
    public class GetControlCategoryPurchaseHistoryRequest
    {
        public Guid ControlCategoryId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class ControlCategoryPurchaseHistoryDto
    {
        public Guid ControlCategoryId { get; set; }
        public decimal TotalPurchased { get; set; }
        public int TotalOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class CanPlaceOrderRequest
    {
        public Guid ControlCategoryId { get; set; }
        public decimal OrderAmount { get; set; }
    }
    public class CanPlaceOrderResponse
    {
        public bool CanPlaceOrder { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class ValidateControlCategoryStatusRequest
    {
        public Guid ControlCategoryId { get; set; }
        public ControlCategoryOperationType OperationType { get; set; }
    }
    public class ValidateControlCategoryStatusResponse
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
    }
    public enum ControlCategoryOperationType
    {
        PlaceOrder,
        RequestCredit,
    }
    public class ControlCategoryPaymentTermsDto
    {
        public Guid ControlCategoryId { get; set; }
        public int PaymentTermDays { get; set; }
        public decimal CreditLimitAmount { get; set; }
    }
    public class UpdateControlCategoryPaymentTermsRequest
    {
        public Guid ControlCategoryId { get; set; }
        public int NewPaymentTermDays { get; set; }
    }
}

