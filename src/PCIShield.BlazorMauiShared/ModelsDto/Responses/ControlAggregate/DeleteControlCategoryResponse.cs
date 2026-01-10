using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.ControlCategory
{
    public class DeleteControlCategoryResponse : BaseResponse
    {
        public DeleteControlCategoryResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public DeleteControlCategoryResponse()
        {
        }
        
    }
}

