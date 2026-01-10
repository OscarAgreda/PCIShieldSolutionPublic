using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.ROCPackage
{
    public class CreateROCPackageResponse : BaseResponse
    {
        public CreateROCPackageResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public CreateROCPackageResponse()
        {
        }
        
        public ROCPackageDto ROCPackage { get; set; } = new();
    }
}

