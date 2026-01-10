using System;

namespace PCIShield.Domain.Exceptions
{
    public class DuplicatePaymentPageException : ArgumentException
    {
        public DuplicatePaymentPageException(string message, string paramName) : base(message, paramName)
        {
        }
    }
}

