using System;

namespace PCIShield.Domain.Exceptions
{
    public class DuplicateScriptException : ArgumentException
    {
        public DuplicateScriptException(string message, string paramName) : base(message, paramName)
        {
        }
    }
}

