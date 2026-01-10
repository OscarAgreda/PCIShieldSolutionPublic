using System;

using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.AssessmentControl
{
    public class CreateAssessmentControlRequest : BaseRequest
    {
        
        public Guid AssessmentId { get; set; }
        public Guid ControlId { get; set; }
        
        public AssessmentControlDto AssessmentControl { get; set; }
        
        public CreateAssessmentControlRequest()
        {
        }
        
    }
    public class CreateAssessmentControlJoinRequest
    {
        public AssessmentControlDto AssessmentControl { get; set; }
        public int RowId { get; set; }
        public AssessmentDto Assessment { get; set; }
        public Guid AssessmentId { get; set; }
        public ControlDto Control { get; set; }
        public Guid ControlId { get; set; }
    }
    
    public class GetAssessmentControlTransactionHistoryRequest
    {
        public Guid RowId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
    public class TransactionDto
    {
        public Guid TransactionId { get; set; }
        public Guid RowId { get; set; }
        public string TransactionType { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
    }
    public class GetAssessmentControlPurchaseHistoryRequest
    {
        public Guid RowId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class AssessmentControlPurchaseHistoryDto
    {
        public Guid RowId { get; set; }
        public decimal TotalPurchased { get; set; }
        public int TotalOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class CanPlaceOrderRequest
    {
        public Guid RowId { get; set; }
        public decimal OrderAmount { get; set; }
    }
    public class CanPlaceOrderResponse
    {
        public bool CanPlaceOrder { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class ValidateAssessmentControlStatusRequest
    {
        public Guid RowId { get; set; }
        public AssessmentControlOperationType OperationType { get; set; }
    }
    public class ValidateAssessmentControlStatusResponse
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
    }
    public enum AssessmentControlOperationType
    {
        PlaceOrder,
        RequestCredit,
    }
    public class AssessmentControlPaymentTermsDto
    {
        public Guid RowId { get; set; }
        public int PaymentTermDays { get; set; }
        public decimal CreditLimitAmount { get; set; }
    }
    public class UpdateAssessmentControlPaymentTermsRequest
    {
        public Guid RowId { get; set; }
        public int NewPaymentTermDays { get; set; }
    }
}

