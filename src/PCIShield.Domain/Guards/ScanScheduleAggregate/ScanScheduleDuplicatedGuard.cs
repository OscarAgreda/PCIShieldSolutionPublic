using System.Collections.Generic;
using System.Linq;
using PCIShield.Domain.Entities;
using PCIShield.Domain.Exceptions;

namespace Ardalis.GuardClauses
{
    public static class ScanScheduleGuardExtensions
    {
        public static void DuplicateScanSchedule(this IGuardClause guardClause, IEnumerable<ScanSchedule> existingScanSchedules, ScanSchedule newScanSchedule, string parameterName)
        {
            if (existingScanSchedules.Any(a => a.ScanScheduleId == newScanSchedule.ScanScheduleId))
            {
                throw new DuplicateScanScheduleException("Cannot add duplicate scanSchedule.", parameterName);
            }
        }
    }
}

