using System;
using PCIShield.BlazorMauiShared.Models;

namespace BlazorMauiShared.Models.ApplicationUser
{
    public class GetByIdApplicationUserRequest : BaseRequest
    {
        public Guid ApplicationUserId { get; set; }
        public bool WithPostGraph { get; set; }
    }
}

