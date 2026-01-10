using System;

namespace PCIShield.Domain.Exceptions
{
    public class DuplicateControlEvidenceException : ArgumentException
    {
        public DuplicateControlEvidenceException(string message, string paramName) : base(message, paramName)
        {
        }
    }
}

