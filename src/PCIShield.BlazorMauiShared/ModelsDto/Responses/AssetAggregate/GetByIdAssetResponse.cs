using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.Asset
{
    public class GetByIdAssetResponse : BaseResponse
    {
        public GetByIdAssetResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public GetByIdAssetResponse()
        {
        }
        
        public AssetDto Asset { get; set; } = new();
    }
}

