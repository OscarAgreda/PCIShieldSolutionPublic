using System;

namespace PCIShield.Domain.Exceptions
{
    public class DuplicateComplianceOfficerException : ArgumentException
    {
        public DuplicateComplianceOfficerException(string message, string paramName) : base(message, paramName)
        {
        }
    }
}

