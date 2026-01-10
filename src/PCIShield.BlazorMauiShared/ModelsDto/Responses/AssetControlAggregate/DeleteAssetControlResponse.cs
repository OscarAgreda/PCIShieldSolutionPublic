using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.AssetControl
{
    public class DeleteAssetControlResponse : BaseResponse
    {
        public DeleteAssetControlResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public DeleteAssetControlResponse()
        {
        }
        
    }
}

