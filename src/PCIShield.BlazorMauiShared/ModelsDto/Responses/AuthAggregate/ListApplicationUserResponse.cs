using System;
using System.Collections.Generic;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.ApplicationUser
{
    public class ListApplicationUserResponse : BaseResponse
    {
        public ListApplicationUserResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public ListApplicationUserResponse()
        {
        }
        
        public List<ApplicationUserDto>? ApplicationUsers { get; set; } = new();
        
        public int Count { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}

