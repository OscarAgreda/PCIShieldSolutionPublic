using System;
using PCIShield.BlazorMauiShared.Models;

namespace BlazorMauiShared.Models.AuditLog
{
    public class DeleteAuditLogRequest : BaseRequest
    {
        public Guid AuditLogId { get; set; }
    }
}

