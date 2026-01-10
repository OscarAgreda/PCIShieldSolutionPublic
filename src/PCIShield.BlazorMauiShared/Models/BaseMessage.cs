using System;
using System.Diagnostics;
namespace PCIShield.BlazorMauiShared.Models
{
    public abstract class BaseMessage
    {
        protected Guid _correlationId = Guid.NewGuid();
        public Guid CorrelationId()
        {
            return _correlationId;
        }
    }
}