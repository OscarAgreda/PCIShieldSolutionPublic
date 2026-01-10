using System;
using System.Collections.Generic;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.ROCPackage
{
    public class ListROCPackageResponse : BaseResponse
    {
        public ListROCPackageResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public ListROCPackageResponse()
        {
        }
        
        public List<ROCPackageDto>? ROCPackages { get; set; } = new();
        
        public int Count { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}

