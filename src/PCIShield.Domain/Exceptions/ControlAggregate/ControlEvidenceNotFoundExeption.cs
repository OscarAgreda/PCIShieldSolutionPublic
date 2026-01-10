using System;

namespace PCIShield.Domain.Exceptions
{
    public class ControlEvidenceNotFoundException : Exception
    {
        public ControlEvidenceNotFoundException(string message) : base(message)
        {
        }
        
        public ControlEvidenceNotFoundException(int rowId) : base($"No controlEvidence with id {rowId} found.")
        {
        }
    }
    
    public class InvalidControlEvidenceStateTransitionException : Exception
    {
        public InvalidControlEvidenceStateTransitionException(string currentState, string newState)
        : base($"Invalid state transition for ControlEvidence from {currentState} to {newState}.")
        {
        }
    }
    
    public class ControlEvidenceValidationException : Exception
    {
        public ControlEvidenceValidationException(string message) : base(message)
        {
        }
        
        public ControlEvidenceValidationException(string propertyName, string errorMessage)
        : base($"Validation failed for ControlEvidence.{propertyName}: {errorMessage}")
        {
        }
    }
}

