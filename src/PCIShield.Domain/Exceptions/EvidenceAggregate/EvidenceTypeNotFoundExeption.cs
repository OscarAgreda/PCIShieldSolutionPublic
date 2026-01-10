using System;

namespace PCIShield.Domain.Exceptions
{
    public class EvidenceTypeNotFoundException : Exception
    {
        public EvidenceTypeNotFoundException(string message) : base(message)
        {
        }
        
        public EvidenceTypeNotFoundException(Guid evidenceTypeId) : base($"No evidenceType with id {evidenceTypeId} found.")
        {
        }
    }
    
    public class InvalidEvidenceTypeStateTransitionException : Exception
    {
        public InvalidEvidenceTypeStateTransitionException(string currentState, string newState)
        : base($"Invalid state transition for EvidenceType from {currentState} to {newState}.")
        {
        }
    }
    
    public class EvidenceTypeValidationException : Exception
    {
        public EvidenceTypeValidationException(string message) : base(message)
        {
        }
        
        public EvidenceTypeValidationException(string propertyName, string errorMessage)
        : base($"Validation failed for EvidenceType.{propertyName}: {errorMessage}")
        {
        }
    }
}

