using System;

using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.ScanSchedule
{
    public class CreateScanScheduleRequest : BaseRequest
    {
        
        public Guid AssetId { get; set; }
        
        public ScanScheduleDto ScanSchedule { get; set; }
        
        public CreateScanScheduleRequest()
        {
            if (ScanSchedule != null)
            {
                ScanSchedule.ScanScheduleId = Guid.NewGuid();
            }
        }
        
    }
    public class GetScanScheduleTransactionHistoryRequest
    {
        public Guid ScanScheduleId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
    public class TransactionDto
    {
        public Guid TransactionId { get; set; }
        public Guid ScanScheduleId { get; set; }
        public string TransactionType { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
    }
    public class GetScanSchedulePurchaseHistoryRequest
    {
        public Guid ScanScheduleId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class ScanSchedulePurchaseHistoryDto
    {
        public Guid ScanScheduleId { get; set; }
        public decimal TotalPurchased { get; set; }
        public int TotalOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class CanPlaceOrderRequest
    {
        public Guid ScanScheduleId { get; set; }
        public decimal OrderAmount { get; set; }
    }
    public class CanPlaceOrderResponse
    {
        public bool CanPlaceOrder { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class ValidateScanScheduleStatusRequest
    {
        public Guid ScanScheduleId { get; set; }
        public ScanScheduleOperationType OperationType { get; set; }
    }
    public class ValidateScanScheduleStatusResponse
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
    }
    public enum ScanScheduleOperationType
    {
        PlaceOrder,
        RequestCredit,
    }
    public class ScanSchedulePaymentTermsDto
    {
        public Guid ScanScheduleId { get; set; }
        public int PaymentTermDays { get; set; }
        public decimal CreditLimitAmount { get; set; }
    }
    public class UpdateScanSchedulePaymentTermsRequest
    {
        public Guid ScanScheduleId { get; set; }
        public int NewPaymentTermDays { get; set; }
    }
}

