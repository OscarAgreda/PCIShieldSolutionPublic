using System;

namespace PCIShield.Domain.Exceptions
{
    public class AssessmentNotFoundException : Exception
    {
        public AssessmentNotFoundException(string message) : base(message)
        {
        }
        
        public AssessmentNotFoundException(Guid assessmentId) : base($"No assessment with id {assessmentId} found.")
        {
        }
    }
    
    public class InvalidAssessmentStateTransitionException : Exception
    {
        public InvalidAssessmentStateTransitionException(string currentState, string newState)
        : base($"Invalid state transition for Assessment from {currentState} to {newState}.")
        {
        }
    }
    
    public class AssessmentValidationException : Exception
    {
        public AssessmentValidationException(string message) : base(message)
        {
        }
        
        public AssessmentValidationException(string propertyName, string errorMessage)
        : base($"Validation failed for Assessment.{propertyName}: {errorMessage}")
        {
        }
    }
}

