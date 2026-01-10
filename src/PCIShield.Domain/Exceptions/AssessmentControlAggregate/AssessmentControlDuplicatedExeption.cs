using System;

namespace PCIShield.Domain.Exceptions
{
    public class DuplicateAssessmentControlException : ArgumentException
    {
        public DuplicateAssessmentControlException(string message, string paramName) : base(message, paramName)
        {
        }
    }
}

