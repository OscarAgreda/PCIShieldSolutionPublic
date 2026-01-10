using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.ControlCategory
{
    public class GetByIdControlCategoryResponse : BaseResponse
    {
        public GetByIdControlCategoryResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public GetByIdControlCategoryResponse()
        {
        }
        
        public ControlCategoryDto ControlCategory { get; set; } = new();
    }
}

