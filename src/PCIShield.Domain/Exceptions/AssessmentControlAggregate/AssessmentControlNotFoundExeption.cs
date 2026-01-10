using System;

namespace PCIShield.Domain.Exceptions
{
    public class AssessmentControlNotFoundException : Exception
    {
        public AssessmentControlNotFoundException(string message) : base(message)
        {
        }
        
        public AssessmentControlNotFoundException(int rowId) : base($"No assessmentControl with id {rowId} found.")
        {
        }
    }
    
    public class InvalidAssessmentControlStateTransitionException : Exception
    {
        public InvalidAssessmentControlStateTransitionException(string currentState, string newState)
        : base($"Invalid state transition for AssessmentControl from {currentState} to {newState}.")
        {
        }
    }
    
    public class AssessmentControlValidationException : Exception
    {
        public AssessmentControlValidationException(string message) : base(message)
        {
        }
        
        public AssessmentControlValidationException(string propertyName, string errorMessage)
        : base($"Validation failed for AssessmentControl.{propertyName}: {errorMessage}")
        {
        }
    }
}

