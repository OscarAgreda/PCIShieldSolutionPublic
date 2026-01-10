using System;

namespace PCIShield.Domain.Exceptions
{
    public class ScanScheduleNotFoundException : Exception
    {
        public ScanScheduleNotFoundException(string message) : base(message)
        {
        }
        
        public ScanScheduleNotFoundException(Guid scanScheduleId) : base($"No scanSchedule with id {scanScheduleId} found.")
        {
        }
    }
    
    public class InvalidScanScheduleStateTransitionException : Exception
    {
        public InvalidScanScheduleStateTransitionException(string currentState, string newState)
        : base($"Invalid state transition for ScanSchedule from {currentState} to {newState}.")
        {
        }
    }
    
    public class ScanScheduleValidationException : Exception
    {
        public ScanScheduleValidationException(string message) : base(message)
        {
        }
        
        public ScanScheduleValidationException(string propertyName, string errorMessage)
        : base($"Validation failed for ScanSchedule.{propertyName}: {errorMessage}")
        {
        }
    }
}

