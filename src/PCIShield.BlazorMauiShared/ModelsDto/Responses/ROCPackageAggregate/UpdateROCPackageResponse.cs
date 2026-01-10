using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.ROCPackage
{
    public class UpdateROCPackageResponse : BaseResponse
    {
        public UpdateROCPackageResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public UpdateROCPackageResponse()
        {
        }
        
        public ROCPackageDto ROCPackage { get; set; } = new();
    }
}

