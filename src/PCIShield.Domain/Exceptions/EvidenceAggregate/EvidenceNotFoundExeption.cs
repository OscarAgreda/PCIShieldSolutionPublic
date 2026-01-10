using System;

namespace PCIShield.Domain.Exceptions
{
    public class EvidenceNotFoundException : Exception
    {
        public EvidenceNotFoundException(string message) : base(message)
        {
        }
        
        public EvidenceNotFoundException(Guid evidenceId) : base($"No evidence with id {evidenceId} found.")
        {
        }
    }
    
    public class InvalidEvidenceStateTransitionException : Exception
    {
        public InvalidEvidenceStateTransitionException(string currentState, string newState)
        : base($"Invalid state transition for Evidence from {currentState} to {newState}.")
        {
        }
    }
    
    public class EvidenceValidationException : Exception
    {
        public EvidenceValidationException(string message) : base(message)
        {
        }
        
        public EvidenceValidationException(string propertyName, string errorMessage)
        : base($"Validation failed for Evidence.{propertyName}: {errorMessage}")
        {
        }
    }
}

