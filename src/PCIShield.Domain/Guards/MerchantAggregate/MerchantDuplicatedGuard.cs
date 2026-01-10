using System.Collections.Generic;
using System.Linq;
using PCIShield.Domain.Entities;
using PCIShield.Domain.Exceptions;

namespace Ardalis.GuardClauses
{
    public static class MerchantGuardExtensions
    {
        public static void DuplicateMerchant(this IGuardClause guardClause, IEnumerable<Merchant> existingMerchants, Merchant newMerchant, string parameterName)
        {
            if (existingMerchants.Any(a => a.MerchantId == newMerchant.MerchantId))
            {
                throw new DuplicateMerchantException("Cannot add duplicate merchant.", parameterName);
            }
        }
    }
}

