using System;

namespace PCIShield.Domain.Exceptions
{
    public class ROCPackageNotFoundException : Exception
    {
        public ROCPackageNotFoundException(string message) : base(message)
        {
        }
        
        public ROCPackageNotFoundException(Guid rocpackageId) : base($"No rocpackage with id {rocpackageId} found.")
        {
        }
    }
    
    public class InvalidROCPackageStateTransitionException : Exception
    {
        public InvalidROCPackageStateTransitionException(string currentState, string newState)
        : base($"Invalid state transition for ROCPackage from {currentState} to {newState}.")
        {
        }
    }
    
    public class ROCPackageValidationException : Exception
    {
        public ROCPackageValidationException(string message) : base(message)
        {
        }
        
        public ROCPackageValidationException(string propertyName, string errorMessage)
        : base($"Validation failed for ROCPackage.{propertyName}: {errorMessage}")
        {
        }
    }
}

