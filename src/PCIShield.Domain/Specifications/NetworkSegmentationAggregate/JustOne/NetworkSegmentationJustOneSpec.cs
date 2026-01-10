using System;
using System.Linq;
using Ardalis.GuardClauses;
using Ardalis.Specification;
using  PCIShield.Domain.Entities;
using  PCIShield.Domain.ModelEntityDto;
  	  
namespace  PCIShield.Domain.Specifications
{
    public class NetworkSegmentationByIdJustOneSpec : Specification<NetworkSegmentation, NetworkSegmentationEntityDto>
    {
        public NetworkSegmentationByIdJustOneSpec(Guid networkSegmentationId)
        {
            _ = Guard.Against.NullOrEmpty(networkSegmentationId, nameof(networkSegmentationId));
            _ = Query.Where(networkSegmentation => networkSegmentation.NetworkSegmentationId == networkSegmentationId);
            _ = Query
                .Select(x => new NetworkSegmentationEntityDto
                {
                    NetworkSegmentationId = x.NetworkSegmentationId,
                    MerchantId = x.MerchantId,
                })
                .AsNoTracking()
                .EnableCache($"NetworkSegmentationByIdJustOne-{networkSegmentationId.ToString()}");
        }
    }
}

