using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.ApplicationUser
{
    public class DeleteApplicationUserResponse : BaseResponse
    {
        public DeleteApplicationUserResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public DeleteApplicationUserResponse()
        {
        }
        
    }
}

