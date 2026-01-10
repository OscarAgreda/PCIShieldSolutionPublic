using System.Collections.Generic;
using System.Linq;
using PCIShield.Domain.Entities;
using PCIShield.Domain.Exceptions;

namespace Ardalis.GuardClauses
{
    public static class ComplianceOfficerGuardExtensions
    {
        public static void DuplicateComplianceOfficer(this IGuardClause guardClause, IEnumerable<ComplianceOfficer> existingComplianceOfficers, ComplianceOfficer newComplianceOfficer, string parameterName)
        {
            if (existingComplianceOfficers.Any(a => a.ComplianceOfficerId == newComplianceOfficer.ComplianceOfficerId))
            {
                throw new DuplicateComplianceOfficerException("Cannot add duplicate complianceOfficer.", parameterName);
            }
        }
    }
}

