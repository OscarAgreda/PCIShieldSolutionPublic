using System;

namespace PCIShield.Domain.Exceptions
{
    public class ControlCategoryNotFoundException : Exception
    {
        public ControlCategoryNotFoundException(string message) : base(message)
        {
        }
        
        public ControlCategoryNotFoundException(Guid controlCategoryId) : base($"No controlCategory with id {controlCategoryId} found.")
        {
        }
    }
    
    public class InvalidControlCategoryStateTransitionException : Exception
    {
        public InvalidControlCategoryStateTransitionException(string currentState, string newState)
        : base($"Invalid state transition for ControlCategory from {currentState} to {newState}.")
        {
        }
    }
    
    public class ControlCategoryValidationException : Exception
    {
        public ControlCategoryValidationException(string message) : base(message)
        {
        }
        
        public ControlCategoryValidationException(string propertyName, string errorMessage)
        : base($"Validation failed for ControlCategory.{propertyName}: {errorMessage}")
        {
        }
    }
}

