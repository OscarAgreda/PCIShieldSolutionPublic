using System;
using System.Linq;
using Ardalis.GuardClauses;
using Ardalis.Specification;
using  PCIShield.Domain.Entities;
using  PCIShield.Domain.ModelEntityDto;
  	  
namespace  PCIShield.Domain.Specifications
{
    public class MerchantByIdJustOneSpec : Specification<Merchant, MerchantEntityDto>
    {
        public MerchantByIdJustOneSpec(Guid merchantId)
        {
            _ = Guard.Against.NullOrEmpty(merchantId, nameof(merchantId));
            _ = Query.Where(merchant => merchant.MerchantId == merchantId);
            _ = Query
                .Select(x => new MerchantEntityDto
                {
                    MerchantId = x.MerchantId,
                })
                .AsNoTracking()
                .EnableCache($"MerchantByIdJustOne-{merchantId.ToString()}");
        }
    }
}

