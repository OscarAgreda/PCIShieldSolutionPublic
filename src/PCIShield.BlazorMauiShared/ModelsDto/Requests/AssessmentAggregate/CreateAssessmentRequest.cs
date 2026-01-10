using System;

using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.Assessment
{
    public class CreateAssessmentRequest : BaseRequest
    {
        
        public Guid MerchantId { get; set; }
        
        public AssessmentDto Assessment { get; set; }
        
        public CreateAssessmentRequest()
        {
            if (Assessment != null)
            {
                Assessment.AssessmentId = Guid.NewGuid();
            }
        }
        
    }
    public class GetAssessmentTransactionHistoryRequest
    {
        public Guid AssessmentId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
    public class TransactionDto
    {
        public Guid TransactionId { get; set; }
        public Guid AssessmentId { get; set; }
        public string TransactionType { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
    }
    public class GetAssessmentPurchaseHistoryRequest
    {
        public Guid AssessmentId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class AssessmentPurchaseHistoryDto
    {
        public Guid AssessmentId { get; set; }
        public decimal TotalPurchased { get; set; }
        public int TotalOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class CanPlaceOrderRequest
    {
        public Guid AssessmentId { get; set; }
        public decimal OrderAmount { get; set; }
    }
    public class CanPlaceOrderResponse
    {
        public bool CanPlaceOrder { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class ValidateAssessmentStatusRequest
    {
        public Guid AssessmentId { get; set; }
        public AssessmentOperationType OperationType { get; set; }
    }
    public class ValidateAssessmentStatusResponse
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
    }
    public enum AssessmentOperationType
    {
        PlaceOrder,
        RequestCredit,
    }
    public class AssessmentPaymentTermsDto
    {
        public Guid AssessmentId { get; set; }
        public int PaymentTermDays { get; set; }
        public decimal CreditLimitAmount { get; set; }
    }
    public class UpdateAssessmentPaymentTermsRequest
    {
        public Guid AssessmentId { get; set; }
        public int NewPaymentTermDays { get; set; }
    }
}

