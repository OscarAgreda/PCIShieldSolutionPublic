using System.Collections.Generic;
using System.Linq;
using PCIShield.Domain.Entities;
using PCIShield.Domain.Exceptions;

namespace Ardalis.GuardClauses
{
    public static class CryptographicInventoryGuardExtensions
    {
        public static void DuplicateCryptographicInventory(this IGuardClause guardClause, IEnumerable<CryptographicInventory> existingCryptographicInventories, CryptographicInventory newCryptographicInventory, string parameterName)
        {
            if (existingCryptographicInventories.Any(a => a.CryptographicInventoryId == newCryptographicInventory.CryptographicInventoryId))
            {
                throw new DuplicateCryptographicInventoryException("Cannot add duplicate cryptographicInventory.", parameterName);
            }
        }
    }
}

