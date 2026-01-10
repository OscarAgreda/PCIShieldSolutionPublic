using System;

using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.ROCPackage
{
    public class CreateROCPackageRequest : BaseRequest
    {
        
        public Guid AssessmentId { get; set; }
        
        public ROCPackageDto ROCPackage { get; set; }
        
        public CreateROCPackageRequest()
        {
            if (ROCPackage != null)
            {
                ROCPackage.ROCPackageId = Guid.NewGuid();
            }
        }
        
    }
    public class GetROCPackageTransactionHistoryRequest
    {
        public Guid ROCPackageId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
    public class TransactionDto
    {
        public Guid TransactionId { get; set; }
        public Guid ROCPackageId { get; set; }
        public string TransactionType { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
    }
    public class GetROCPackagePurchaseHistoryRequest
    {
        public Guid ROCPackageId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class ROCPackagePurchaseHistoryDto
    {
        public Guid ROCPackageId { get; set; }
        public decimal TotalPurchased { get; set; }
        public int TotalOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class CanPlaceOrderRequest
    {
        public Guid ROCPackageId { get; set; }
        public decimal OrderAmount { get; set; }
    }
    public class CanPlaceOrderResponse
    {
        public bool CanPlaceOrder { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class ValidateROCPackageStatusRequest
    {
        public Guid ROCPackageId { get; set; }
        public ROCPackageOperationType OperationType { get; set; }
    }
    public class ValidateROCPackageStatusResponse
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
    }
    public enum ROCPackageOperationType
    {
        PlaceOrder,
        RequestCredit,
    }
    public class ROCPackagePaymentTermsDto
    {
        public Guid ROCPackageId { get; set; }
        public int PaymentTermDays { get; set; }
        public decimal CreditLimitAmount { get; set; }
    }
    public class UpdateROCPackagePaymentTermsRequest
    {
        public Guid ROCPackageId { get; set; }
        public int NewPaymentTermDays { get; set; }
    }
}

