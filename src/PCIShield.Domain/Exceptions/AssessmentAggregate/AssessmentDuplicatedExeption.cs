using System;

namespace PCIShield.Domain.Exceptions
{
    public class DuplicateAssessmentException : ArgumentException
    {
        public DuplicateAssessmentException(string message, string paramName) : base(message, paramName)
        {
        }
    }
}

