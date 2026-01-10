using System;
using System.Linq;
using Ardalis.GuardClauses;
using Ardalis.Specification;
using  PCIShield.Domain.Entities;
using  PCIShield.Domain.ModelEntityDto;
  	  
namespace  PCIShield.Domain.Specifications
{
    public class ScanScheduleByIdJustOneSpec : Specification<ScanSchedule, ScanScheduleEntityDto>
    {
        public ScanScheduleByIdJustOneSpec(Guid scanScheduleId)
        {
            _ = Guard.Against.NullOrEmpty(scanScheduleId, nameof(scanScheduleId));
            _ = Query.Where(scanSchedule => scanSchedule.ScanScheduleId == scanScheduleId);
            _ = Query
                .Select(x => new ScanScheduleEntityDto
                {
                    ScanScheduleId = x.ScanScheduleId,
                    AssetId = x.AssetId,
                })
                .AsNoTracking()
                .EnableCache($"ScanScheduleByIdJustOne-{scanScheduleId.ToString()}");
        }
    }
}

