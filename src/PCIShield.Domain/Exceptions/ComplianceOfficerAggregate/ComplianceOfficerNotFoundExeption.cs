using System;

namespace PCIShield.Domain.Exceptions
{
    public class ComplianceOfficerNotFoundException : Exception
    {
        public ComplianceOfficerNotFoundException(string message) : base(message)
        {
        }
        
        public ComplianceOfficerNotFoundException(Guid complianceOfficerId) : base($"No complianceOfficer with id {complianceOfficerId} found.")
        {
        }
    }
    
    public class InvalidComplianceOfficerStateTransitionException : Exception
    {
        public InvalidComplianceOfficerStateTransitionException(string currentState, string newState)
        : base($"Invalid state transition for ComplianceOfficer from {currentState} to {newState}.")
        {
        }
    }
    
    public class ComplianceOfficerValidationException : Exception
    {
        public ComplianceOfficerValidationException(string message) : base(message)
        {
        }
        
        public ComplianceOfficerValidationException(string propertyName, string errorMessage)
        : base($"Validation failed for ComplianceOfficer.{propertyName}: {errorMessage}")
        {
        }
    }
}

