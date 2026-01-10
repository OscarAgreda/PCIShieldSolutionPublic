using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.AssetControl
{
    public class GetByIdAssetControlResponse : BaseResponse
    {
        public GetByIdAssetControlResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public GetByIdAssetControlResponse()
        {
        }
        
        public AssetControlDto AssetControl { get; set; } = new();
    }
}

