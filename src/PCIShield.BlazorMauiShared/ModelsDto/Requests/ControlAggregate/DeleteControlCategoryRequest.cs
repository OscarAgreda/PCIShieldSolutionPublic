using System;
using PCIShield.BlazorMauiShared.Models;

namespace BlazorMauiShared.Models.ControlCategory
{
    public class DeleteControlCategoryRequest : BaseRequest
    {
        public Guid ControlCategoryId { get; set; }
    }
}

