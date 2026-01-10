using System;

namespace PCIShield.Domain.Exceptions
{
    public class DuplicateNetworkSegmentationException : ArgumentException
    {
        public DuplicateNetworkSegmentationException(string message, string paramName) : base(message, paramName)
        {
        }
    }
}

