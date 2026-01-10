using System;

using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.EvidenceType
{
    public class CreateEvidenceTypeRequest : BaseRequest
    {
        public EvidenceTypeDto EvidenceType { get; set; }
        
        public CreateEvidenceTypeRequest()
        {
            if (EvidenceType != null)
            {
                EvidenceType.EvidenceTypeId = Guid.NewGuid();
            }
        }
        
    }
    public class GetEvidenceTypeTransactionHistoryRequest
    {
        public Guid EvidenceTypeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
    public class TransactionDto
    {
        public Guid TransactionId { get; set; }
        public Guid EvidenceTypeId { get; set; }
        public string TransactionType { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
    }
    public class GetEvidenceTypePurchaseHistoryRequest
    {
        public Guid EvidenceTypeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class EvidenceTypePurchaseHistoryDto
    {
        public Guid EvidenceTypeId { get; set; }
        public decimal TotalPurchased { get; set; }
        public int TotalOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class CanPlaceOrderRequest
    {
        public Guid EvidenceTypeId { get; set; }
        public decimal OrderAmount { get; set; }
    }
    public class CanPlaceOrderResponse
    {
        public bool CanPlaceOrder { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class ValidateEvidenceTypeStatusRequest
    {
        public Guid EvidenceTypeId { get; set; }
        public EvidenceTypeOperationType OperationType { get; set; }
    }
    public class ValidateEvidenceTypeStatusResponse
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
    }
    public enum EvidenceTypeOperationType
    {
        PlaceOrder,
        RequestCredit,
    }
    public class EvidenceTypePaymentTermsDto
    {
        public Guid EvidenceTypeId { get; set; }
        public int PaymentTermDays { get; set; }
        public decimal CreditLimitAmount { get; set; }
    }
    public class UpdateEvidenceTypePaymentTermsRequest
    {
        public Guid EvidenceTypeId { get; set; }
        public int NewPaymentTermDays { get; set; }
    }
}

