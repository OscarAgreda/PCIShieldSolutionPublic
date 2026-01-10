using System;

namespace PCIShield.Domain.Exceptions
{
    public class DuplicateROCPackageException : ArgumentException
    {
        public DuplicateROCPackageException(string message, string paramName) : base(message, paramName)
        {
        }
    }
}

