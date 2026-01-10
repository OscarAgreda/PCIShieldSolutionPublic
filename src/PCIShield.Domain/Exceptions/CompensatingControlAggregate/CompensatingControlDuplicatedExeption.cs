using System;

namespace PCIShield.Domain.Exceptions
{
    public class DuplicateCompensatingControlException : ArgumentException
    {
        public DuplicateCompensatingControlException(string message, string paramName) : base(message, paramName)
        {
        }
    }
}

