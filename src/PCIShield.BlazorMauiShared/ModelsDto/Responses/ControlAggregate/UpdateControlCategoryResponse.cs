using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.ControlCategory
{
    public class UpdateControlCategoryResponse : BaseResponse
    {
        public UpdateControlCategoryResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public UpdateControlCategoryResponse()
        {
        }
        
        public ControlCategoryDto ControlCategory { get; set; } = new();
    }
}

