using System.Collections.Generic;
using System.Linq;
using PCIShield.Domain.Entities;
using PCIShield.Domain.Exceptions;

namespace Ardalis.GuardClauses
{
    public static class NetworkSegmentationGuardExtensions
    {
        public static void DuplicateNetworkSegmentation(this IGuardClause guardClause, IEnumerable<NetworkSegmentation> existingNetworkSegmentations, NetworkSegmentation newNetworkSegmentation, string parameterName)
        {
            if (existingNetworkSegmentations.Any(a => a.NetworkSegmentationId == newNetworkSegmentation.NetworkSegmentationId))
            {
                throw new DuplicateNetworkSegmentationException("Cannot add duplicate networkSegmentation.", parameterName);
            }
        }
    }
}

