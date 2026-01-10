using System;

namespace PCIShield.Domain.Exceptions
{
    public class MerchantNotFoundException : Exception
    {
        public MerchantNotFoundException(string message) : base(message)
        {
        }
        
        public MerchantNotFoundException(Guid merchantId) : base($"No merchant with id {merchantId} found.")
        {
        }
    }
    
    public class InvalidMerchantStateTransitionException : Exception
    {
        public InvalidMerchantStateTransitionException(string currentState, string newState)
        : base($"Invalid state transition for Merchant from {currentState} to {newState}.")
        {
        }
    }
    
    public class MerchantValidationException : Exception
    {
        public MerchantValidationException(string message) : base(message)
        {
        }
        
        public MerchantValidationException(string propertyName, string errorMessage)
        : base($"Validation failed for Merchant.{propertyName}: {errorMessage}")
        {
        }
    }
}

