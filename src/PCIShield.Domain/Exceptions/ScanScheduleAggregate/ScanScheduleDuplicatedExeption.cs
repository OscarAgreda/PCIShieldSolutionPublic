using System;

namespace PCIShield.Domain.Exceptions
{
    public class DuplicateScanScheduleException : ArgumentException
    {
        public DuplicateScanScheduleException(string message, string paramName) : base(message, paramName)
        {
        }
    }
}

