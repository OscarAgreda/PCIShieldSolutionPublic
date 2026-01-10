using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.Asset
{
    public class UpdateAssetResponse : BaseResponse
    {
        public UpdateAssetResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public UpdateAssetResponse()
        {
        }
        
        public AssetDto Asset { get; set; } = new();
    }
}

