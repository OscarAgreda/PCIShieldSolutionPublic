using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.ApplicationUser
{
    public class CreateApplicationUserResponse : BaseResponse
    {
        public CreateApplicationUserResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public CreateApplicationUserResponse()
        {
        }
        
        public ApplicationUserDto ApplicationUser { get; set; } = new();
    }
}

