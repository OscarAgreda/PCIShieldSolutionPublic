using System;
using System.Linq;
using Ardalis.GuardClauses;
using Ardalis.Specification;
using  PCIShield.Domain.Entities;
using  PCIShield.Domain.ModelEntityDto;
  	  
namespace  PCIShield.Domain.Specifications
{
    public class CompensatingControlByIdJustOneSpec : Specification<CompensatingControl, CompensatingControlEntityDto>
    {
        public CompensatingControlByIdJustOneSpec(Guid compensatingControlId)
        {
            _ = Guard.Against.NullOrEmpty(compensatingControlId, nameof(compensatingControlId));
            _ = Query.Where(compensatingControl => compensatingControl.CompensatingControlId == compensatingControlId);
            _ = Query
                .Select(x => new CompensatingControlEntityDto
                {
                    CompensatingControlId = x.CompensatingControlId,
                    ControlId = x.ControlId,
                    MerchantId = x.MerchantId,
                })
                .AsNoTracking()
                .EnableCache($"CompensatingControlByIdJustOne-{compensatingControlId.ToString()}");
        }
    }
}

