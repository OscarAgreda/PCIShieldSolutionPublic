using System;

namespace PCIShield.Domain.Exceptions
{
    public class CryptographicInventoryNotFoundException : Exception
    {
        public CryptographicInventoryNotFoundException(string message) : base(message)
        {
        }
        
        public CryptographicInventoryNotFoundException(Guid cryptographicInventoryId) : base($"No cryptographicInventory with id {cryptographicInventoryId} found.")
        {
        }
    }
    
    public class InvalidCryptographicInventoryStateTransitionException : Exception
    {
        public InvalidCryptographicInventoryStateTransitionException(string currentState, string newState)
        : base($"Invalid state transition for CryptographicInventory from {currentState} to {newState}.")
        {
        }
    }
    
    public class CryptographicInventoryValidationException : Exception
    {
        public CryptographicInventoryValidationException(string message) : base(message)
        {
        }
        
        public CryptographicInventoryValidationException(string propertyName, string errorMessage)
        : base($"Validation failed for CryptographicInventory.{propertyName}: {errorMessage}")
        {
        }
    }
}

