using System;

namespace PCIShield.Domain.Exceptions
{
    public class DuplicateControlException : ArgumentException
    {
        public DuplicateControlException(string message, string paramName) : base(message, paramName)
        {
        }
    }
}

