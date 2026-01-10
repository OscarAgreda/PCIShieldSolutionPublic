using System;

using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.Logs
{
    public class CreateLogsRequest : BaseRequest
    {
        public LogsDto Logs { get; set; }
        
        public CreateLogsRequest()
        {
        }
        
    }
    public class GetLogsTransactionHistoryRequest
    {
        public Guid Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
    public class TransactionDto
    {
        public Guid TransactionId { get; set; }
        public Guid Id { get; set; }
        public string TransactionType { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
    }
    public class GetLogsPurchaseHistoryRequest
    {
        public Guid Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class LogsPurchaseHistoryDto
    {
        public Guid Id { get; set; }
        public decimal TotalPurchased { get; set; }
        public int TotalOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class CanPlaceOrderRequest
    {
        public Guid Id { get; set; }
        public decimal OrderAmount { get; set; }
    }
    public class CanPlaceOrderResponse
    {
        public bool CanPlaceOrder { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class ValidateLogsStatusRequest
    {
        public Guid Id { get; set; }
        public LogsOperationType OperationType { get; set; }
    }
    public class ValidateLogsStatusResponse
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
    }
    public enum LogsOperationType
    {
        PlaceOrder,
        RequestCredit,
    }
    public class LogsPaymentTermsDto
    {
        public Guid Id { get; set; }
        public int PaymentTermDays { get; set; }
        public decimal CreditLimitAmount { get; set; }
    }
    public class UpdateLogsPaymentTermsRequest
    {
        public Guid Id { get; set; }
        public int NewPaymentTermDays { get; set; }
    }
}

