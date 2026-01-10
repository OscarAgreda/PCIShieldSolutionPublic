using System;

namespace PCIShield.Domain.Exceptions
{
    public class DuplicateControlCategoryException : ArgumentException
    {
        public DuplicateControlCategoryException(string message, string paramName) : base(message, paramName)
        {
        }
    }
}

