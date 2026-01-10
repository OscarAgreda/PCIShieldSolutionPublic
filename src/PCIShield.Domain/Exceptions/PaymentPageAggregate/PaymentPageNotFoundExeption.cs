using System;

namespace PCIShield.Domain.Exceptions
{
    public class PaymentPageNotFoundException : Exception
    {
        public PaymentPageNotFoundException(string message) : base(message)
        {
        }
        
        public PaymentPageNotFoundException(Guid paymentPageId) : base($"No paymentPage with id {paymentPageId} found.")
        {
        }
    }
    
    public class InvalidPaymentPageStateTransitionException : Exception
    {
        public InvalidPaymentPageStateTransitionException(string currentState, string newState)
        : base($"Invalid state transition for PaymentPage from {currentState} to {newState}.")
        {
        }
    }
    
    public class PaymentPageValidationException : Exception
    {
        public PaymentPageValidationException(string message) : base(message)
        {
        }
        
        public PaymentPageValidationException(string propertyName, string errorMessage)
        : base($"Validation failed for PaymentPage.{propertyName}: {errorMessage}")
        {
        }
    }
}

