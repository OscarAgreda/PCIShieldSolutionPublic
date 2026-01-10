using System;

namespace PCIShield.Domain.Exceptions
{
    public class DuplicateAssessmentTypeException : ArgumentException
    {
        public DuplicateAssessmentTypeException(string message, string paramName) : base(message, paramName)
        {
        }
    }
}

