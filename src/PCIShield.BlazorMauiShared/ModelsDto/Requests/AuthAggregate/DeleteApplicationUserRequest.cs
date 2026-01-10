using System;
using PCIShield.BlazorMauiShared.Models;

namespace BlazorMauiShared.Models.ApplicationUser
{
    public class DeleteApplicationUserRequest : BaseRequest
    {
        public Guid ApplicationUserId { get; set; }
    }
}

