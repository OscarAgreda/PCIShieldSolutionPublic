using System;

namespace PCIShield.Domain.Exceptions
{
    public class CompensatingControlNotFoundException : Exception
    {
        public CompensatingControlNotFoundException(string message) : base(message)
        {
        }
        
        public CompensatingControlNotFoundException(Guid compensatingControlId) : base($"No compensatingControl with id {compensatingControlId} found.")
        {
        }
    }
    
    public class InvalidCompensatingControlStateTransitionException : Exception
    {
        public InvalidCompensatingControlStateTransitionException(string currentState, string newState)
        : base($"Invalid state transition for CompensatingControl from {currentState} to {newState}.")
        {
        }
    }
    
    public class CompensatingControlValidationException : Exception
    {
        public CompensatingControlValidationException(string message) : base(message)
        {
        }
        
        public CompensatingControlValidationException(string propertyName, string errorMessage)
        : base($"Validation failed for CompensatingControl.{propertyName}: {errorMessage}")
        {
        }
    }
}

