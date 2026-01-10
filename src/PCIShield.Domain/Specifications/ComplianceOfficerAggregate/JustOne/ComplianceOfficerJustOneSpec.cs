using System;
using System.Linq;
using Ardalis.GuardClauses;
using Ardalis.Specification;
using  PCIShield.Domain.Entities;
using  PCIShield.Domain.ModelEntityDto;
  	  
namespace  PCIShield.Domain.Specifications
{
    public class ComplianceOfficerByIdJustOneSpec : Specification<ComplianceOfficer, ComplianceOfficerEntityDto>
    {
        public ComplianceOfficerByIdJustOneSpec(Guid complianceOfficerId)
        {
            _ = Guard.Against.NullOrEmpty(complianceOfficerId, nameof(complianceOfficerId));
            _ = Query.Where(complianceOfficer => complianceOfficer.ComplianceOfficerId == complianceOfficerId);
            _ = Query
                .Select(x => new ComplianceOfficerEntityDto
                {
                    ComplianceOfficerId = x.ComplianceOfficerId,
                    MerchantId = x.MerchantId,
                })
                .AsNoTracking()
                .EnableCache($"ComplianceOfficerByIdJustOne-{complianceOfficerId.ToString()}");
        }
    }
}

