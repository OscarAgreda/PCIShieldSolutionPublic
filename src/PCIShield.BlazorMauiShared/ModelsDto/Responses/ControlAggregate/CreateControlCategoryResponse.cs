using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.ControlCategory
{
    public class CreateControlCategoryResponse : BaseResponse
    {
        public CreateControlCategoryResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public CreateControlCategoryResponse()
        {
        }
        
        public ControlCategoryDto ControlCategory { get; set; } = new();
    }
}

