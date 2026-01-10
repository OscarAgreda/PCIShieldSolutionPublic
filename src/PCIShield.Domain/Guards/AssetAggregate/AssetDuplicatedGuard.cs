using System.Collections.Generic;
using System.Linq;
using PCIShield.Domain.Entities;
using PCIShield.Domain.Exceptions;

namespace Ardalis.GuardClauses
{
    public static class AssetGuardExtensions
    {
        public static void DuplicateAsset(this IGuardClause guardClause, IEnumerable<Asset> existingAssets, Asset newAsset, string parameterName)
        {
            if (existingAssets.Any(a => a.AssetId == newAsset.AssetId))
            {
                throw new DuplicateAssetException("Cannot add duplicate asset.", parameterName);
            }
        }
    }
}

