using System;

namespace PCIShield.Domain.Exceptions
{
    public class DuplicatePaymentChannelException : ArgumentException
    {
        public DuplicatePaymentChannelException(string message, string paramName) : base(message, paramName)
        {
        }
    }
}

