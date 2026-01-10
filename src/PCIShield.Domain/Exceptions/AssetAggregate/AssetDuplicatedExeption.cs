using System;

namespace PCIShield.Domain.Exceptions
{
    public class DuplicateAssetException : ArgumentException
    {
        public DuplicateAssetException(string message, string paramName) : base(message, paramName)
        {
        }
    }
}

