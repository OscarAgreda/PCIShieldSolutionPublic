using System;
using System.Linq;
using Ardalis.GuardClauses;
using Ardalis.Specification;
using  PCIShield.Domain.Entities;
using  PCIShield.Domain.ModelEntityDto;
  	  
namespace  PCIShield.Domain.Specifications
{
    public class EvidenceByIdJustOneSpec : Specification<Evidence, EvidenceEntityDto>
    {
        public EvidenceByIdJustOneSpec(Guid evidenceId)
        {
            _ = Guard.Against.NullOrEmpty(evidenceId, nameof(evidenceId));
            _ = Query.Where(evidence => evidence.EvidenceId == evidenceId);
            _ = Query
                .Select(x => new EvidenceEntityDto
                {
                    EvidenceId = x.EvidenceId,
                    MerchantId = x.MerchantId,
                })
                .AsNoTracking()
                .EnableCache($"EvidenceByIdJustOne-{evidenceId.ToString()}");
        }
    }
}

