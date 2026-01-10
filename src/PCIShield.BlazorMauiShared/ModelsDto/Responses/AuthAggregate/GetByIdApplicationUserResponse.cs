using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.ApplicationUser
{
    public class GetByIdApplicationUserResponse : BaseResponse
    {
        public GetByIdApplicationUserResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public GetByIdApplicationUserResponse()
        {
        }
        
        public ApplicationUserDto ApplicationUser { get; set; } = new();
    }
}

