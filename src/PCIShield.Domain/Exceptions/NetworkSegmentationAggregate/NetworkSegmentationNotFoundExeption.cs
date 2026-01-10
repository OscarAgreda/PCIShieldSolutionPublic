using System;

namespace PCIShield.Domain.Exceptions
{
    public class NetworkSegmentationNotFoundException : Exception
    {
        public NetworkSegmentationNotFoundException(string message) : base(message)
        {
        }
        
        public NetworkSegmentationNotFoundException(Guid networkSegmentationId) : base($"No networkSegmentation with id {networkSegmentationId} found.")
        {
        }
    }
    
    public class InvalidNetworkSegmentationStateTransitionException : Exception
    {
        public InvalidNetworkSegmentationStateTransitionException(string currentState, string newState)
        : base($"Invalid state transition for NetworkSegmentation from {currentState} to {newState}.")
        {
        }
    }
    
    public class NetworkSegmentationValidationException : Exception
    {
        public NetworkSegmentationValidationException(string message) : base(message)
        {
        }
        
        public NetworkSegmentationValidationException(string propertyName, string errorMessage)
        : base($"Validation failed for NetworkSegmentation.{propertyName}: {errorMessage}")
        {
        }
    }
}

