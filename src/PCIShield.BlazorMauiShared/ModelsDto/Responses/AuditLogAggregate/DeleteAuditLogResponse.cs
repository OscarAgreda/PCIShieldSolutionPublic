using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.AuditLog
{
    public class DeleteAuditLogResponse : BaseResponse
    {
        public DeleteAuditLogResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public DeleteAuditLogResponse()
        {
        }
        
    }
}

