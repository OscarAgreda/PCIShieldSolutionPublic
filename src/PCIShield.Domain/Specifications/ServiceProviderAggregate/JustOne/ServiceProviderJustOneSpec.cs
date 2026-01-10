using System;
using System.Linq;
using Ardalis.GuardClauses;
using Ardalis.Specification;
using  PCIShield.Domain.Entities;
using  PCIShield.Domain.ModelEntityDto;
  	  
namespace  PCIShield.Domain.Specifications
{
    public class ServiceProviderByIdJustOneSpec : Specification<ServiceProvider, ServiceProviderEntityDto>
    {
        public ServiceProviderByIdJustOneSpec(Guid serviceProviderId)
        {
            _ = Guard.Against.NullOrEmpty(serviceProviderId, nameof(serviceProviderId));
            _ = Query.Where(serviceProvider => serviceProvider.ServiceProviderId == serviceProviderId);
            _ = Query
                .Select(x => new ServiceProviderEntityDto
                {
                    ServiceProviderId = x.ServiceProviderId,
                    MerchantId = x.MerchantId,
                })
                .AsNoTracking()
                .EnableCache($"ServiceProviderByIdJustOne-{serviceProviderId.ToString()}");
        }
    }
}

