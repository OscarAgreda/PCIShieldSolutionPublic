using System;
using System.Linq;
using Ardalis.GuardClauses;
using Ardalis.Specification;
using  PCIShield.Domain.Entities;
using  PCIShield.Domain.ModelEntityDto;
  	  
namespace  PCIShield.Domain.Specifications
{
    public class AssetByIdJustOneSpec : Specification<Asset, AssetEntityDto>
    {
        public AssetByIdJustOneSpec(Guid assetId)
        {
            _ = Guard.Against.NullOrEmpty(assetId, nameof(assetId));
            _ = Query.Where(asset => asset.AssetId == assetId);
            _ = Query
                .Select(x => new AssetEntityDto
                {
                    AssetId = x.AssetId,
                    MerchantId = x.MerchantId,
                })
                .AsNoTracking()
                .EnableCache($"AssetByIdJustOne-{assetId.ToString()}");
        }
    }
}

