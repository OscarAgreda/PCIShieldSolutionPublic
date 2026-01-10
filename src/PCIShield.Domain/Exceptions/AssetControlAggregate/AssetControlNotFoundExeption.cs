using System;

namespace PCIShield.Domain.Exceptions
{
    public class AssetControlNotFoundException : Exception
    {
        public AssetControlNotFoundException(string message) : base(message)
        {
        }
        
        public AssetControlNotFoundException(int rowId) : base($"No assetControl with id {rowId} found.")
        {
        }
    }
    
    public class InvalidAssetControlStateTransitionException : Exception
    {
        public InvalidAssetControlStateTransitionException(string currentState, string newState)
        : base($"Invalid state transition for AssetControl from {currentState} to {newState}.")
        {
        }
    }
    
    public class AssetControlValidationException : Exception
    {
        public AssetControlValidationException(string message) : base(message)
        {
        }
        
        public AssetControlValidationException(string propertyName, string errorMessage)
        : base($"Validation failed for AssetControl.{propertyName}: {errorMessage}")
        {
        }
    }
}

