using System;
using System.Collections.Generic;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.ControlCategory
{
    public class ListControlCategoryResponse : BaseResponse
    {
        public ListControlCategoryResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public ListControlCategoryResponse()
        {
        }
        
        public List<ControlCategoryDto>? ControlCategories { get; set; } = new();
        
        public int Count { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}

