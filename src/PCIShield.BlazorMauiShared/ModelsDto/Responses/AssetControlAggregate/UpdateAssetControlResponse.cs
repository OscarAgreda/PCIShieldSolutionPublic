using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.AssetControl
{
    public class UpdateAssetControlResponse : BaseResponse
    {
        public UpdateAssetControlResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public UpdateAssetControlResponse()
        {
        }
        
        public AssetControlDto AssetControl { get; set; } = new();
    }
}

