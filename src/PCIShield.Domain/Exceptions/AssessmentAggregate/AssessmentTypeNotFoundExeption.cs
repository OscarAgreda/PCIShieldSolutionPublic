using System;

namespace PCIShield.Domain.Exceptions
{
    public class AssessmentTypeNotFoundException : Exception
    {
        public AssessmentTypeNotFoundException(string message) : base(message)
        {
        }
        
        public AssessmentTypeNotFoundException(Guid assessmentTypeId) : base($"No assessmentType with id {assessmentTypeId} found.")
        {
        }
    }
    
    public class InvalidAssessmentTypeStateTransitionException : Exception
    {
        public InvalidAssessmentTypeStateTransitionException(string currentState, string newState)
        : base($"Invalid state transition for AssessmentType from {currentState} to {newState}.")
        {
        }
    }
    
    public class AssessmentTypeValidationException : Exception
    {
        public AssessmentTypeValidationException(string message) : base(message)
        {
        }
        
        public AssessmentTypeValidationException(string propertyName, string errorMessage)
        : base($"Validation failed for AssessmentType.{propertyName}: {errorMessage}")
        {
        }
    }
}

