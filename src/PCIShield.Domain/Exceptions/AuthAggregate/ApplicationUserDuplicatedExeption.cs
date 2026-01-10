using System;

namespace PCIShield.Domain.Exceptions
{
    public class DuplicateApplicationUserException : ArgumentException
    {
        public DuplicateApplicationUserException(string message, string paramName) : base(message, paramName)
        {
        }
    }
}

