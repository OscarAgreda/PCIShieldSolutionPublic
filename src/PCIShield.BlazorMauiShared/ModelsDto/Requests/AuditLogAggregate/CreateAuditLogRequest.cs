using System;

using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.AuditLog
{
    public class CreateAuditLogRequest : BaseRequest
    {
        public AuditLogDto AuditLog { get; set; }
        
        public CreateAuditLogRequest()
        {
            if (AuditLog != null)
            {
                AuditLog.AuditLogId = Guid.NewGuid();
            }
        }
        
    }
    public class GetAuditLogTransactionHistoryRequest
    {
        public Guid AuditLogId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
    public class TransactionDto
    {
        public Guid TransactionId { get; set; }
        public Guid AuditLogId { get; set; }
        public string TransactionType { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
    }
    public class GetAuditLogPurchaseHistoryRequest
    {
        public Guid AuditLogId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class AuditLogPurchaseHistoryDto
    {
        public Guid AuditLogId { get; set; }
        public decimal TotalPurchased { get; set; }
        public int TotalOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class CanPlaceOrderRequest
    {
        public Guid AuditLogId { get; set; }
        public decimal OrderAmount { get; set; }
    }
    public class CanPlaceOrderResponse
    {
        public bool CanPlaceOrder { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class ValidateAuditLogStatusRequest
    {
        public Guid AuditLogId { get; set; }
        public AuditLogOperationType OperationType { get; set; }
    }
    public class ValidateAuditLogStatusResponse
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
    }
    public enum AuditLogOperationType
    {
        PlaceOrder,
        RequestCredit,
    }
    public class AuditLogPaymentTermsDto
    {
        public Guid AuditLogId { get; set; }
        public int PaymentTermDays { get; set; }
        public decimal CreditLimitAmount { get; set; }
    }
    public class UpdateAuditLogPaymentTermsRequest
    {
        public Guid AuditLogId { get; set; }
        public int NewPaymentTermDays { get; set; }
    }
}

