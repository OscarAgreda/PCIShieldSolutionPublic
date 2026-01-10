using System;
using System.Collections.Generic;
using System.Linq;
using Ardalis.Specification;
using PCIShield.Domain.Entities;

namespace PCIShieldCore.Domain.Specifications
{
    public class MerchantGetConnectedListSpec : Specification<Merchant>
    {
        public MerchantGetConnectedListSpec(IEnumerable<string> merchantIds)
        {
            Query.Where(merchant =>
                    merchant.IsDeleted != true
                    && merchantIds.Contains(merchant.TenantId.ToString()))
                .AsNoTracking();
        }
    }

    public class ComplianceOfficerGetConnectedListSpec : Specification<ComplianceOfficer>
    {
        public ComplianceOfficerGetConnectedListSpec(IEnumerable<string> complianceOfficerIds)
        {
            Query.Where(complianceOfficer =>
                                    complianceOfficer.IsDeleted != true
                                   && 
                    complianceOfficerIds.Contains(complianceOfficer.TenantId.ToString()))
                .AsNoTracking();
        }
    }

    public class ComplianceOfficerGetListSpec : Specification<ComplianceOfficer>
    {
        public ComplianceOfficerGetListSpec()
        {
            Query.Where(complianceOfficer => !complianceOfficer.IsDeleted)
      .AsNoTracking();
        }
    }
}
