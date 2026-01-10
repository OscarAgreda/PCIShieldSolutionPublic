using System;
using System.Collections.Generic;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.Script
{
    public class ListScriptResponse : BaseResponse
    {
        public ListScriptResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public ListScriptResponse()
        {
        }
        
        public List<ScriptDto>? Scripts { get; set; } = new();
        
        public int Count { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}

