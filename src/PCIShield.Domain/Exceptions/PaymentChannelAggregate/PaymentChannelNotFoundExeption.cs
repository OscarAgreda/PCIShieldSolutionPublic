using System;

namespace PCIShield.Domain.Exceptions
{
    public class PaymentChannelNotFoundException : Exception
    {
        public PaymentChannelNotFoundException(string message) : base(message)
        {
        }
        
        public PaymentChannelNotFoundException(Guid paymentChannelId) : base($"No paymentChannel with id {paymentChannelId} found.")
        {
        }
    }
    
    public class InvalidPaymentChannelStateTransitionException : Exception
    {
        public InvalidPaymentChannelStateTransitionException(string currentState, string newState)
        : base($"Invalid state transition for PaymentChannel from {currentState} to {newState}.")
        {
        }
    }
    
    public class PaymentChannelValidationException : Exception
    {
        public PaymentChannelValidationException(string message) : base(message)
        {
        }
        
        public PaymentChannelValidationException(string propertyName, string errorMessage)
        : base($"Validation failed for PaymentChannel.{propertyName}: {errorMessage}")
        {
        }
    }
}

