using System;

namespace PCIShield.Domain.Exceptions
{
    public class DuplicateEvidenceTypeException : ArgumentException
    {
        public DuplicateEvidenceTypeException(string message, string paramName) : base(message, paramName)
        {
        }
    }
}

