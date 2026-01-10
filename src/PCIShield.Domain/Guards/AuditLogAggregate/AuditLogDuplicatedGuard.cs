using System.Collections.Generic;
using System.Linq;
using PCIShield.Domain.Entities;
using PCIShield.Domain.Exceptions;

namespace Ardalis.GuardClauses
{
    public static class AuditLogGuardExtensions
    {
        public static void DuplicateAuditLog(this IGuardClause guardClause, IEnumerable<AuditLog> existingAuditLogs, AuditLog newAuditLog, string parameterName)
        {
            if (existingAuditLogs.Any(a => a.AuditLogId == newAuditLog.AuditLogId))
            {
                throw new DuplicateAuditLogException("Cannot add duplicate auditLog.", parameterName);
            }
        }
    }
}

