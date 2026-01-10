using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.Asset
{
    public class CreateAssetResponse : BaseResponse
    {
        public CreateAssetResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public CreateAssetResponse()
        {
        }
        
        public AssetDto Asset { get; set; } = new();
    }
}

