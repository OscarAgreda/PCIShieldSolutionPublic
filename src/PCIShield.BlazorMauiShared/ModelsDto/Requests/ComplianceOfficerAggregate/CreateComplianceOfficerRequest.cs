using System;

using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.ComplianceOfficer
{
    public class CreateComplianceOfficerRequest : BaseRequest
    {
        
        public Guid MerchantId { get; set; }
        
        public ComplianceOfficerDto ComplianceOfficer { get; set; }
        
        public CreateComplianceOfficerRequest()
        {
            if (ComplianceOfficer != null)
            {
                ComplianceOfficer.ComplianceOfficerId = Guid.NewGuid();
            }
        }
        
    }
    public class GetComplianceOfficerTransactionHistoryRequest
    {
        public Guid ComplianceOfficerId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
    public class TransactionDto
    {
        public Guid TransactionId { get; set; }
        public Guid ComplianceOfficerId { get; set; }
        public string TransactionType { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
    }
    public class GetComplianceOfficerPurchaseHistoryRequest
    {
        public Guid ComplianceOfficerId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class ComplianceOfficerPurchaseHistoryDto
    {
        public Guid ComplianceOfficerId { get; set; }
        public decimal TotalPurchased { get; set; }
        public int TotalOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class CanPlaceOrderRequest
    {
        public Guid ComplianceOfficerId { get; set; }
        public decimal OrderAmount { get; set; }
    }
    public class CanPlaceOrderResponse
    {
        public bool CanPlaceOrder { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class ValidateComplianceOfficerStatusRequest
    {
        public Guid ComplianceOfficerId { get; set; }
        public ComplianceOfficerOperationType OperationType { get; set; }
    }
    public class ValidateComplianceOfficerStatusResponse
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
    }
    public enum ComplianceOfficerOperationType
    {
        PlaceOrder,
        RequestCredit,
    }
    public class ComplianceOfficerPaymentTermsDto
    {
        public Guid ComplianceOfficerId { get; set; }
        public int PaymentTermDays { get; set; }
        public decimal CreditLimitAmount { get; set; }
    }
    public class UpdateComplianceOfficerPaymentTermsRequest
    {
        public Guid ComplianceOfficerId { get; set; }
        public int NewPaymentTermDays { get; set; }
    }
}

