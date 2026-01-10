using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.AuditLog
{
    public class UpdateAuditLogResponse : BaseResponse
    {
        public UpdateAuditLogResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public UpdateAuditLogResponse()
        {
        }
        
        public AuditLogDto AuditLog { get; set; } = new();
    }
}

