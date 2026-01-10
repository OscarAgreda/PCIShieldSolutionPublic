using System;

using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.ControlEvidence
{
    public class CreateControlEvidenceRequest : BaseRequest
    {
        
        public Guid AssessmentId { get; set; }
        public Guid ControlId { get; set; }
        public Guid EvidenceId { get; set; }
        
        public ControlEvidenceDto ControlEvidence { get; set; }
        
        public CreateControlEvidenceRequest()
        {
        }
        
    }
    public class CreateControlEvidenceJoinRequest
    {
        public ControlEvidenceDto ControlEvidence { get; set; }
        public int RowId { get; set; }
        public ControlDto Control { get; set; }
        public Guid ControlId { get; set; }
        public EvidenceDto Evidence { get; set; }
        public Guid EvidenceId { get; set; }
        public AssessmentDto Assessment { get; set; }
        public Guid AssessmentId { get; set; }
    }
    
    public class GetControlEvidenceTransactionHistoryRequest
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
    public class GetControlEvidencePurchaseHistoryRequest
    {
        public Guid RowId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class ControlEvidencePurchaseHistoryDto
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
    public class ValidateControlEvidenceStatusRequest
    {
        public Guid RowId { get; set; }
        public ControlEvidenceOperationType OperationType { get; set; }
    }
    public class ValidateControlEvidenceStatusResponse
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
    }
    public enum ControlEvidenceOperationType
    {
        PlaceOrder,
        RequestCredit,
    }
    public class ControlEvidencePaymentTermsDto
    {
        public Guid RowId { get; set; }
        public int PaymentTermDays { get; set; }
        public decimal CreditLimitAmount { get; set; }
    }
    public class UpdateControlEvidencePaymentTermsRequest
    {
        public Guid RowId { get; set; }
        public int NewPaymentTermDays { get; set; }
    }
}

