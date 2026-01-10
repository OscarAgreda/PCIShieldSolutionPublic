using System;

namespace PCIShield.Domain.Exceptions
{
    public class AuditLogNotFoundException : Exception
    {
        public AuditLogNotFoundException(string message) : base(message)
        {
        }
        
        public AuditLogNotFoundException(Guid auditLogId) : base($"No auditLog with id {auditLogId} found.")
        {
        }
    }
    
    public class InvalidAuditLogStateTransitionException : Exception
    {
        public InvalidAuditLogStateTransitionException(string currentState, string newState)
        : base($"Invalid state transition for AuditLog from {currentState} to {newState}.")
        {
        }
    }
    
    public class AuditLogValidationException : Exception
    {
        public AuditLogValidationException(string message) : base(message)
        {
        }
        
        public AuditLogValidationException(string propertyName, string errorMessage)
        : base($"Validation failed for AuditLog.{propertyName}: {errorMessage}")
        {
        }
    }
}

