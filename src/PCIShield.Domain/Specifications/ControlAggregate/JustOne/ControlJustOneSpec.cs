using System;
using System.Linq;
using Ardalis.GuardClauses;
using Ardalis.Specification;
using  PCIShield.Domain.Entities;
using  PCIShield.Domain.ModelEntityDto;
  	  
namespace  PCIShield.Domain.Specifications
{
    public class ControlByIdJustOneSpec : Specification<Control, ControlEntityDto>
    {
        public ControlByIdJustOneSpec(Guid controlId)
        {
            _ = Guard.Against.NullOrEmpty(controlId, nameof(controlId));
            _ = Query.Where(control => control.ControlId == controlId);
            _ = Query
                .Select(x => new ControlEntityDto
                {
                    ControlId = x.ControlId,
                })
                .AsNoTracking()
                .EnableCache($"ControlByIdJustOne-{controlId.ToString()}");
        }
    }
}

