using System;
using System.Linq;
using Ardalis.GuardClauses;
using Ardalis.Specification;
using  PCIShield.Domain.Entities;
using  PCIShield.Domain.ModelEntityDto;
  	  
namespace  PCIShield.Domain.Specifications
{
    public class ControlCategoryByIdJustOneSpec : Specification<ControlCategory, ControlCategoryEntityDto>
    {
        public ControlCategoryByIdJustOneSpec(Guid controlCategoryId)
        {
            _ = Guard.Against.NullOrEmpty(controlCategoryId, nameof(controlCategoryId));
            _ = Query.Where(controlCategory => controlCategory.ControlCategoryId == controlCategoryId);
            _ = Query
                .Select(x => new ControlCategoryEntityDto
                {
                    ControlCategoryId = x.ControlCategoryId,
                })
                .AsNoTracking()
                .EnableCache($"ControlCategoryByIdJustOne-{controlCategoryId.ToString()}");
        }
    }
}

