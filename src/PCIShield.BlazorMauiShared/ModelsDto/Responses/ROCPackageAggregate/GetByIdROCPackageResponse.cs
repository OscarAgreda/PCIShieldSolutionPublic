using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.ROCPackage
{
    public class GetByIdROCPackageResponse : BaseResponse
    {
        public GetByIdROCPackageResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public GetByIdROCPackageResponse()
        {
        }
        
        public ROCPackageDto ROCPackage { get; set; } = new();
    }
}

