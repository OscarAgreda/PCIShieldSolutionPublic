using System;

namespace PCIShield.Domain.Exceptions
{
    public class ApplicationUserNotFoundException : Exception
    {
        public ApplicationUserNotFoundException(string message) : base(message)
        {
        }
        
        public ApplicationUserNotFoundException(Guid applicationUserId) : base($"No applicationUser with id {applicationUserId} found.")
        {
        }
    }
    
    public class InvalidApplicationUserStateTransitionException : Exception
    {
        public InvalidApplicationUserStateTransitionException(string currentState, string newState)
        : base($"Invalid state transition for ApplicationUser from {currentState} to {newState}.")
        {
        }
    }
    
    public class ApplicationUserValidationException : Exception
    {
        public ApplicationUserValidationException(string message) : base(message)
        {
        }
        
        public ApplicationUserValidationException(string propertyName, string errorMessage)
        : base($"Validation failed for ApplicationUser.{propertyName}: {errorMessage}")
        {
        }
    }
}

