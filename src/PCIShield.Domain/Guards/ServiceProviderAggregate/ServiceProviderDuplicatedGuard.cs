using System.Collections.Generic;
using System.Linq;
using PCIShield.Domain.Entities;
using PCIShield.Domain.Exceptions;

namespace Ardalis.GuardClauses
{
    public static class ServiceProviderGuardExtensions
    {
        public static void DuplicateServiceProvider(this IGuardClause guardClause, IEnumerable<ServiceProvider> existingServiceProviders, ServiceProvider newServiceProvider, string parameterName)
        {
            if (existingServiceProviders.Any(a => a.ServiceProviderId == newServiceProvider.ServiceProviderId))
            {
                throw new DuplicateServiceProviderException("Cannot add duplicate serviceProvider.", parameterName);
            }
        }
    }
}

