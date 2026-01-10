using System;

using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.AssessmentType
{
    public class CreateAssessmentTypeRequest : BaseRequest
    {
        public AssessmentTypeDto AssessmentType { get; set; }
        
        public CreateAssessmentTypeRequest()
        {
            if (AssessmentType != null)
            {
                AssessmentType.AssessmentTypeId = Guid.NewGuid();
            }
        }
        
    }
    public class GetAssessmentTypeTransactionHistoryRequest
    {
        public Guid AssessmentTypeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
    public class TransactionDto
    {
        public Guid TransactionId { get; set; }
        public Guid AssessmentTypeId { get; set; }
        public string TransactionType { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
    }
    public class GetAssessmentTypePurchaseHistoryRequest
    {
        public Guid AssessmentTypeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class AssessmentTypePurchaseHistoryDto
    {
        public Guid AssessmentTypeId { get; set; }
        public decimal TotalPurchased { get; set; }
        public int TotalOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class CanPlaceOrderRequest
    {
        public Guid AssessmentTypeId { get; set; }
        public decimal OrderAmount { get; set; }
    }
    public class CanPlaceOrderResponse
    {
        public bool CanPlaceOrder { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class ValidateAssessmentTypeStatusRequest
    {
        public Guid AssessmentTypeId { get; set; }
        public AssessmentTypeOperationType OperationType { get; set; }
    }
    public class ValidateAssessmentTypeStatusResponse
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
    }
    public enum AssessmentTypeOperationType
    {
        PlaceOrder,
        RequestCredit,
    }
    public class AssessmentTypePaymentTermsDto
    {
        public Guid AssessmentTypeId { get; set; }
        public int PaymentTermDays { get; set; }
        public decimal CreditLimitAmount { get; set; }
    }
    public class UpdateAssessmentTypePaymentTermsRequest
    {
        public Guid AssessmentTypeId { get; set; }
        public int NewPaymentTermDays { get; set; }
    }
}

