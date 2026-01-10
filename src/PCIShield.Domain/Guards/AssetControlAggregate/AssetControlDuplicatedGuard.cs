using System.Collections.Generic;
using System.Linq;
using PCIShield.Domain.Entities;
using PCIShield.Domain.Exceptions;

namespace Ardalis.GuardClauses
{
    public static class AssetControlGuardExtensions
    {
        public static void DuplicateAssetControl(this IGuardClause guardClause, IEnumerable<AssetControl> existingAssetControls, AssetControl newAssetControl, string parameterName)
        {
            if (existingAssetControls.Any(a => a.RowId == newAssetControl.RowId))
            {
                throw new DuplicateAssetControlException("Cannot add duplicate assetControl.", parameterName);
            }
        }
    }
}

