using System;
using PCIShield.BlazorMauiShared.Models;

namespace BlazorMauiShared.Models.AuditLog
{
    public class GetByIdAuditLogRequest : BaseRequest
    {
        public Guid AuditLogId { get; set; }
        public bool WithPostGraph { get; set; }
    }
}

