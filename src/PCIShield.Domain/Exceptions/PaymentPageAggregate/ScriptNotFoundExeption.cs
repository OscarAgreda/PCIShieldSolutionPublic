using System;

namespace PCIShield.Domain.Exceptions
{
    public class ScriptNotFoundException : Exception
    {
        public ScriptNotFoundException(string message) : base(message)
        {
        }
        
        public ScriptNotFoundException(Guid scriptId) : base($"No script with id {scriptId} found.")
        {
        }
    }
    
    public class InvalidScriptStateTransitionException : Exception
    {
        public InvalidScriptStateTransitionException(string currentState, string newState)
        : base($"Invalid state transition for Script from {currentState} to {newState}.")
        {
        }
    }
    
    public class ScriptValidationException : Exception
    {
        public ScriptValidationException(string message) : base(message)
        {
        }
        
        public ScriptValidationException(string propertyName, string errorMessage)
        : base($"Validation failed for Script.{propertyName}: {errorMessage}")
        {
        }
    }
}

