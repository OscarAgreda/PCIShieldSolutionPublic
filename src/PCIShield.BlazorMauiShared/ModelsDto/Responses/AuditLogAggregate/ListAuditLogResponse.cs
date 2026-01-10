using System;
using System.Collections.Generic;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.AuditLog
{
    public class ListAuditLogResponse : BaseResponse
    {
        public ListAuditLogResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public ListAuditLogResponse()
        {
        }
        
        public List<AuditLogDto>? AuditLogs { get; set; } = new();
        
        public int Count { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}

