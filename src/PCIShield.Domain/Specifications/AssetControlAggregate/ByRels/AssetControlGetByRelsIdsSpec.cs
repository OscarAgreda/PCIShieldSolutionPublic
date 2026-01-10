using System;
using System.Linq;
using Ardalis.GuardClauses;
using Ardalis.Specification;
using PCIShield.Domain.Entities;

namespace PCIShield.Domain.Specifications
{
    public class AssetControlByRelIdsNoTypeSpec : Specification<AssetControl>
    {
        public AssetControlByRelIdsNoTypeSpec(Guid assetId, Guid controlId)
        {
            Guard.Against.Default(assetId, nameof(assetId));
            Guard.Against.Default(controlId, nameof(controlId));

            _ = Query.Where(assetControl => assetControl.AssetId == assetId && assetControl.ControlId == controlId).AsSplitQuery().AsNoTracking().EnableCache($"AssetControlByRelIdsSpec-{assetId}-{assetId}-{controlId}-{controlId}");
  }
  }
    public class AssetControlByRelIdsSpec : Specification<AssetControl>
    {
        public AssetControlByRelIdsSpec(Guid assetId, Guid controlId)
        {
            Guard.Against.Default(assetId, nameof(assetId));
            Guard.Against.Default(controlId, nameof(controlId));

            _ = Query.Where(assetControl => assetControl.AssetId == assetId && assetControl.ControlId == controlId).AsSplitQuery().AsNoTracking().EnableCache($"AssetControlByRelIdsSpec-{assetId}-{assetId}-{controlId}-{controlId}");
  }
  }
}
