using System;
using System.Linq;
using Ardalis.GuardClauses;
using Ardalis.Specification;
using  PCIShield.Domain.Entities;
using  PCIShield.Domain.ModelEntityDto;
  	  
namespace  PCIShield.Domain.Specifications
{
    public class EvidenceTypeByIdJustOneSpec : Specification<EvidenceType, EvidenceTypeEntityDto>
    {
        public EvidenceTypeByIdJustOneSpec(Guid evidenceTypeId)
        {
            _ = Guard.Against.NullOrEmpty(evidenceTypeId, nameof(evidenceTypeId));
            _ = Query.Where(evidenceType => evidenceType.EvidenceTypeId == evidenceTypeId);
            _ = Query
                .Select(x => new EvidenceTypeEntityDto
                {
                    EvidenceTypeId = x.EvidenceTypeId,
                })
                .AsNoTracking()
                .EnableCache($"EvidenceTypeByIdJustOne-{evidenceTypeId.ToString()}");
        }
    }
}

