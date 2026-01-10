using System;

using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.Evidence
{
    public class CreateEvidenceRequest : BaseRequest
    {
        
        public Guid MerchantId { get; set; }
        
        public EvidenceDto Evidence { get; set; }
        
        public CreateEvidenceRequest()
        {
            if (Evidence != null)
            {
                Evidence.EvidenceId = Guid.NewGuid();
            }
        }
        
    }
    public class GetEvidenceTransactionHistoryRequest
    {
        public Guid EvidenceId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
    public class TransactionDto
    {
        public Guid TransactionId { get; set; }
        public Guid EvidenceId { get; set; }
        public string TransactionType { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
    }
    public class GetEvidencePurchaseHistoryRequest
    {
        public Guid EvidenceId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class EvidencePurchaseHistoryDto
    {
        public Guid EvidenceId { get; set; }
        public decimal TotalPurchased { get; set; }
        public int TotalOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class CanPlaceOrderRequest
    {
        public Guid EvidenceId { get; set; }
        public decimal OrderAmount { get; set; }
    }
    public class CanPlaceOrderResponse
    {
        public bool CanPlaceOrder { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class ValidateEvidenceStatusRequest
    {
        public Guid EvidenceId { get; set; }
        public EvidenceOperationType OperationType { get; set; }
    }
    public class ValidateEvidenceStatusResponse
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
    }
    public enum EvidenceOperationType
    {
        PlaceOrder,
        RequestCredit,
    }
    public class EvidencePaymentTermsDto
    {
        public Guid EvidenceId { get; set; }
        public int PaymentTermDays { get; set; }
        public decimal CreditLimitAmount { get; set; }
    }
    public class UpdateEvidencePaymentTermsRequest
    {
        public Guid EvidenceId { get; set; }
        public int NewPaymentTermDays { get; set; }
    }
}

