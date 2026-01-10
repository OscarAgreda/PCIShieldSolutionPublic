using System;
using PCIShield.BlazorMauiShared.Models;
using PCIShield.Domain.ModelsDto;

namespace BlazorMauiShared.Models.AssetControl
{
    public class CreateAssetControlResponse : BaseResponse
    {
        public CreateAssetControlResponse(Guid correlationId)
        : base(correlationId)
        {
        }
        
        public CreateAssetControlResponse()
        {
        }
        
        public AssetControlDto AssetControl { get; set; } = new();
    }
}

