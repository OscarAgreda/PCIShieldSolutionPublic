using System;

namespace PCIShield.Domain.Exceptions
{
    public class DuplicateEvidenceException : ArgumentException
    {
        public DuplicateEvidenceException(string message, string paramName) : base(message, paramName)
        {
        }
    }
}

