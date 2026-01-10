using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.ApplicationUser
{
    public class UpdateApplicationUserResponse : BaseResponse
    {
        public UpdateApplicationUserResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public UpdateApplicationUserResponse()
        {
        }
        
        public ApplicationUserDto ApplicationUser { get; set; } = new();
    }
}

