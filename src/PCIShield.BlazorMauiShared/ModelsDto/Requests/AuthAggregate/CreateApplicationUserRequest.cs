using System;

using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.ApplicationUser
{
    public class CreateApplicationUserRequest : BaseRequest
    {
        public ApplicationUserDto ApplicationUser { get; set; }
        
        public CreateApplicationUserRequest()
        {
            if (ApplicationUser != null)
            {
                ApplicationUser.ApplicationUserId = Guid.NewGuid();
            }
        }
        
    }
    public class GetApplicationUserTransactionHistoryRequest
    {
        public Guid ApplicationUserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
    public class TransactionDto
    {
        public Guid TransactionId { get; set; }
        public Guid ApplicationUserId { get; set; }
        public string TransactionType { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
    }
    public class GetApplicationUserPurchaseHistoryRequest
    {
        public Guid ApplicationUserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class ApplicationUserPurchaseHistoryDto
    {
        public Guid ApplicationUserId { get; set; }
        public decimal TotalPurchased { get; set; }
        public int TotalOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class CanPlaceOrderRequest
    {
        public Guid ApplicationUserId { get; set; }
        public decimal OrderAmount { get; set; }
    }
    public class CanPlaceOrderResponse
    {
        public bool CanPlaceOrder { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class ValidateApplicationUserStatusRequest
    {
        public Guid ApplicationUserId { get; set; }
        public ApplicationUserOperationType OperationType { get; set; }
    }
    public class ValidateApplicationUserStatusResponse
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
    }
    public enum ApplicationUserOperationType
    {
        PlaceOrder,
        RequestCredit,
    }
    public class ApplicationUserPaymentTermsDto
    {
        public Guid ApplicationUserId { get; set; }
        public int PaymentTermDays { get; set; }
        public decimal CreditLimitAmount { get; set; }
    }
    public class UpdateApplicationUserPaymentTermsRequest
    {
        public Guid ApplicationUserId { get; set; }
        public int NewPaymentTermDays { get; set; }
    }
}

