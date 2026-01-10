using System;
using System.Linq;
using Ardalis.GuardClauses;
using Ardalis.Specification;
using  PCIShield.Domain.Entities;
using  PCIShield.Domain.ModelEntityDto;
  	  
namespace  PCIShield.Domain.Specifications
{
    public class ApplicationUserByIdJustOneSpec : Specification<ApplicationUser, ApplicationUserEntityDto>
    {
        public ApplicationUserByIdJustOneSpec(Guid applicationUserId)
        {
            _ = Guard.Against.NullOrEmpty(applicationUserId, nameof(applicationUserId));
            _ = Query.Where(applicationUser => applicationUser.ApplicationUserId == applicationUserId);
            _ = Query
                .Select(x => new ApplicationUserEntityDto
                {
                    ApplicationUserId = x.ApplicationUserId,
                })
                .AsNoTracking()
                .EnableCache($"ApplicationUserByIdJustOne-{applicationUserId.ToString()}");
        }
    }
}

