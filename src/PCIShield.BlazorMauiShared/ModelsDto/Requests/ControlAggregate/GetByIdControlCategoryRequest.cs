using System;
using PCIShield.BlazorMauiShared.Models;

namespace BlazorMauiShared.Models.ControlCategory
{
    public class GetByIdControlCategoryRequest : BaseRequest
    {
        public Guid ControlCategoryId { get; set; }
        public bool WithPostGraph { get; set; }
    }
}

