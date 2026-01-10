using System;

using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.Script
{
    public class CreateScriptRequest : BaseRequest
    {
        
        public Guid PaymentPageId { get; set; }
        
        public ScriptDto Script { get; set; }
        
        public CreateScriptRequest()
        {
            if (Script != null)
            {
                Script.ScriptId = Guid.NewGuid();
            }
        }
        
    }
    public class GetScriptTransactionHistoryRequest
    {
        public Guid ScriptId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
    public class TransactionDto
    {
        public Guid TransactionId { get; set; }
        public Guid ScriptId { get; set; }
        public string TransactionType { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
    }
    public class GetScriptPurchaseHistoryRequest
    {
        public Guid ScriptId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class ScriptPurchaseHistoryDto
    {
        public Guid ScriptId { get; set; }
        public decimal TotalPurchased { get; set; }
        public int TotalOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class CanPlaceOrderRequest
    {
        public Guid ScriptId { get; set; }
        public decimal OrderAmount { get; set; }
    }
    public class CanPlaceOrderResponse
    {
        public bool CanPlaceOrder { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class ValidateScriptStatusRequest
    {
        public Guid ScriptId { get; set; }
        public ScriptOperationType OperationType { get; set; }
    }
    public class ValidateScriptStatusResponse
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
    }
    public enum ScriptOperationType
    {
        PlaceOrder,
        RequestCredit,
    }
    public class ScriptPaymentTermsDto
    {
        public Guid ScriptId { get; set; }
        public int PaymentTermDays { get; set; }
        public decimal CreditLimitAmount { get; set; }
    }
    public class UpdateScriptPaymentTermsRequest
    {
        public Guid ScriptId { get; set; }
        public int NewPaymentTermDays { get; set; }
    }
}

