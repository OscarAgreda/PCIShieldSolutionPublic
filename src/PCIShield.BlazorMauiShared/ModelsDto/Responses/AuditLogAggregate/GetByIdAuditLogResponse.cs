using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.AuditLog
{
    public class GetByIdAuditLogResponse : BaseResponse
    {
        public GetByIdAuditLogResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public GetByIdAuditLogResponse()
        {
        }
        
        public AuditLogDto AuditLog { get; set; } = new();
    }
}

