using System;

namespace PCIShield.Domain.Exceptions
{
    public class DuplicateServiceProviderException : ArgumentException
    {
        public DuplicateServiceProviderException(string message, string paramName) : base(message, paramName)
        {
        }
    }
}

