using System;

using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.CryptographicInventory
{
    public class CreateCryptographicInventoryRequest : BaseRequest
    {
        
        public Guid MerchantId { get; set; }
        
        public CryptographicInventoryDto CryptographicInventory { get; set; }
        
        public CreateCryptographicInventoryRequest()
        {
            if (CryptographicInventory != null)
            {
                CryptographicInventory.CryptographicInventoryId = Guid.NewGuid();
            }
        }
        
    }
    public class GetCryptographicInventoryTransactionHistoryRequest
    {
        public Guid CryptographicInventoryId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
    public class TransactionDto
    {
        public Guid TransactionId { get; set; }
        public Guid CryptographicInventoryId { get; set; }
        public string TransactionType { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
    }
    public class GetCryptographicInventoryPurchaseHistoryRequest
    {
        public Guid CryptographicInventoryId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class CryptographicInventoryPurchaseHistoryDto
    {
        public Guid CryptographicInventoryId { get; set; }
        public decimal TotalPurchased { get; set; }
        public int TotalOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class CanPlaceOrderRequest
    {
        public Guid CryptographicInventoryId { get; set; }
        public decimal OrderAmount { get; set; }
    }
    public class CanPlaceOrderResponse
    {
        public bool CanPlaceOrder { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class ValidateCryptographicInventoryStatusRequest
    {
        public Guid CryptographicInventoryId { get; set; }
        public CryptographicInventoryOperationType OperationType { get; set; }
    }
    public class ValidateCryptographicInventoryStatusResponse
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
    }
    public enum CryptographicInventoryOperationType
    {
        PlaceOrder,
        RequestCredit,
    }
    public class CryptographicInventoryPaymentTermsDto
    {
        public Guid CryptographicInventoryId { get; set; }
        public int PaymentTermDays { get; set; }
        public decimal CreditLimitAmount { get; set; }
    }
    public class UpdateCryptographicInventoryPaymentTermsRequest
    {
        public Guid CryptographicInventoryId { get; set; }
        public int NewPaymentTermDays { get; set; }
    }
}

