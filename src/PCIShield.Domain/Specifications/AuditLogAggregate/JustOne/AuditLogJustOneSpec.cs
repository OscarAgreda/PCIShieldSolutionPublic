using System;
using System.Linq;
using Ardalis.GuardClauses;
using Ardalis.Specification;
using  PCIShield.Domain.Entities;
using  PCIShield.Domain.ModelEntityDto;
  	  
namespace  PCIShield.Domain.Specifications
{
    public class AuditLogByIdJustOneSpec : Specification<AuditLog, AuditLogEntityDto>
    {
        public AuditLogByIdJustOneSpec(Guid auditLogId)
        {
            _ = Guard.Against.NullOrEmpty(auditLogId, nameof(auditLogId));
            _ = Query.Where(auditLog => auditLog.AuditLogId == auditLogId);
            _ = Query
                .Select(x => new AuditLogEntityDto
                {
                    AuditLogId = x.AuditLogId,
                })
                .AsNoTracking()
                .EnableCache($"AuditLogByIdJustOne-{auditLogId.ToString()}");
        }
    }
}

