using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.ROCPackage
{
    public class DeleteROCPackageResponse : BaseResponse
    {
        public DeleteROCPackageResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public DeleteROCPackageResponse()
        {
        }
        
    }
}

