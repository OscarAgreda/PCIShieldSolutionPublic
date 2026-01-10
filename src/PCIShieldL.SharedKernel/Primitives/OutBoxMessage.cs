using System;
using LanguageExt.Common;
using System.Threading.Tasks;
using System.Threading;
using LanguageExt;
using System.Collections.Generic;
namespace PCIShieldLib.SharedKernel
{
    public abstract  class OutBoxMessage
    {
        public string? PCIShieldSolutionuperUserId { get; set; }
        public Guid EventId { get;  set; } = Guid.NewGuid();
        public Guid UserId { get;  set; } 
        public Guid TenantId { get;  set; } 
        public string? Consumer { get; set; }
        public string? Message { get; set; }
        public string EventType { get; set; }
        public string EntityNameType { get; set; }
        public string? ActionOnMessageReceived { get; set; }
        public string? Content { get; set; }
        public DateTime? OccurredOnUtc { get; set; } = DateTime.UtcNow;
        public DateTime? ProcessedOnUtc { get; set; }
        public bool? IsProcessed { get; set; }
    }
}