using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.AuditLog
{
    public class CreateAuditLogResponse : BaseResponse
    {
        public CreateAuditLogResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public CreateAuditLogResponse()
        {
        }
        
        public AuditLogDto AuditLog { get; set; } = new();
    }
}

