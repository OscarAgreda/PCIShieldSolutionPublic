using System;

namespace PCIShield.Domain.Exceptions
{
    public class ServiceProviderNotFoundException : Exception
    {
        public ServiceProviderNotFoundException(string message) : base(message)
        {
        }
        
        public ServiceProviderNotFoundException(Guid serviceProviderId) : base($"No serviceProvider with id {serviceProviderId} found.")
        {
        }
    }
    
    public class InvalidServiceProviderStateTransitionException : Exception
    {
        public InvalidServiceProviderStateTransitionException(string currentState, string newState)
        : base($"Invalid state transition for ServiceProvider from {currentState} to {newState}.")
        {
        }
    }
    
    public class ServiceProviderValidationException : Exception
    {
        public ServiceProviderValidationException(string message) : base(message)
        {
        }
        
        public ServiceProviderValidationException(string propertyName, string errorMessage)
        : base($"Validation failed for ServiceProvider.{propertyName}: {errorMessage}")
        {
        }
    }
}

