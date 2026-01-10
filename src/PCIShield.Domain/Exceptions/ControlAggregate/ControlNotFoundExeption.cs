using System;

namespace PCIShield.Domain.Exceptions
{
    public class ControlNotFoundException : Exception
    {
        public ControlNotFoundException(string message) : base(message)
        {
        }
        
        public ControlNotFoundException(Guid controlId) : base($"No control with id {controlId} found.")
        {
        }
    }
    
    public class InvalidControlStateTransitionException : Exception
    {
        public InvalidControlStateTransitionException(string currentState, string newState)
        : base($"Invalid state transition for Control from {currentState} to {newState}.")
        {
        }
    }
    
    public class ControlValidationException : Exception
    {
        public ControlValidationException(string message) : base(message)
        {
        }
        
        public ControlValidationException(string propertyName, string errorMessage)
        : base($"Validation failed for Control.{propertyName}: {errorMessage}")
        {
        }
    }
}

