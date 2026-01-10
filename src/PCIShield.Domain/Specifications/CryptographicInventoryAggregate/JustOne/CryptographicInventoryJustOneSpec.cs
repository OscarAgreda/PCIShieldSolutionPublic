using System;
using System.Linq;
using Ardalis.GuardClauses;
using Ardalis.Specification;
using  PCIShield.Domain.Entities;
using  PCIShield.Domain.ModelEntityDto;
  	  
namespace  PCIShield.Domain.Specifications
{
    public class CryptographicInventoryByIdJustOneSpec : Specification<CryptographicInventory, CryptographicInventoryEntityDto>
    {
        public CryptographicInventoryByIdJustOneSpec(Guid cryptographicInventoryId)
        {
            _ = Guard.Against.NullOrEmpty(cryptographicInventoryId, nameof(cryptographicInventoryId));
            _ = Query.Where(cryptographicInventory => cryptographicInventory.CryptographicInventoryId == cryptographicInventoryId);
            _ = Query
                .Select(x => new CryptographicInventoryEntityDto
                {
                    CryptographicInventoryId = x.CryptographicInventoryId,
                    MerchantId = x.MerchantId,
                })
                .AsNoTracking()
                .EnableCache($"CryptographicInventoryByIdJustOne-{cryptographicInventoryId.ToString()}");
        }
    }
}

