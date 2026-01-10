using System;
using System.Linq;
using Ardalis.GuardClauses;
using Ardalis.Specification;
using  PCIShield.Domain.Entities;
using  PCIShield.Domain.ModelEntityDto;
  	  
namespace  PCIShield.Domain.Specifications
{
    public class ROCPackageByIdJustOneSpec : Specification<ROCPackage, ROCPackageEntityDto>
    {
        public ROCPackageByIdJustOneSpec(Guid rocpackageId)
        {
            _ = Guard.Against.NullOrEmpty(rocpackageId, nameof(rocpackageId));
            _ = Query.Where(rocpackage => rocpackage.ROCPackageId == rocpackageId);
            _ = Query
                .Select(x => new ROCPackageEntityDto
                {
                    ROCPackageId = x.ROCPackageId,
                    AssessmentId = x.AssessmentId,
                })
                .AsNoTracking()
                .EnableCache($"ROCPackageByIdJustOne-{rocpackageId.ToString()}");
        }
    }
}

