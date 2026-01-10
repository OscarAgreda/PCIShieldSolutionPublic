using System;

namespace PCIShield.Domain.Exceptions
{
    public class DuplicateCryptographicInventoryException : ArgumentException
    {
        public DuplicateCryptographicInventoryException(string message, string paramName) : base(message, paramName)
        {
        }
    }
}

