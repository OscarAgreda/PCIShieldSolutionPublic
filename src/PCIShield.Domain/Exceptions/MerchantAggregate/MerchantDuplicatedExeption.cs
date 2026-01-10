using System;

namespace PCIShield.Domain.Exceptions
{
    public class DuplicateMerchantException : ArgumentException
    {
        public DuplicateMerchantException(string message, string paramName) : base(message, paramName)
        {
        }
    }
}

