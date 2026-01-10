using System;

namespace PCIShield.Domain.Exceptions
{
    public class AssetNotFoundException : Exception
    {
        public AssetNotFoundException(string message) : base(message)
        {
        }
        
        public AssetNotFoundException(Guid assetId) : base($"No asset with id {assetId} found.")
        {
        }
    }
    
    public class InvalidAssetStateTransitionException : Exception
    {
        public InvalidAssetStateTransitionException(string currentState, string newState)
        : base($"Invalid state transition for Asset from {currentState} to {newState}.")
        {
        }
    }
    
    public class AssetValidationException : Exception
    {
        public AssetValidationException(string message) : base(message)
        {
        }
        
        public AssetValidationException(string propertyName, string errorMessage)
        : base($"Validation failed for Asset.{propertyName}: {errorMessage}")
        {
        }
    }
}

