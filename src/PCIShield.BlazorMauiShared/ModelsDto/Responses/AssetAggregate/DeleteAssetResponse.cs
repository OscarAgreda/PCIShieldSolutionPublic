using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.Asset
{
    public class DeleteAssetResponse : BaseResponse
    {
        public DeleteAssetResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public DeleteAssetResponse()
        {
        }
        
    }
}

