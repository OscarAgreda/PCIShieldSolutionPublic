using System;

namespace PCIShield.Domain.Exceptions
{
    public class DuplicateAuditLogException : ArgumentException
    {
        public DuplicateAuditLogException(string message, string paramName) : base(message, paramName)
        {
        }
    }
}

