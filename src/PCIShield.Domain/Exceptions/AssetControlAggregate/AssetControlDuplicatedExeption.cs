using System;

namespace PCIShield.Domain.Exceptions
{
    public class DuplicateAssetControlException : ArgumentException
    {
        public DuplicateAssetControlException(string message, string paramName) : base(message, paramName)
        {
        }
    }
}

