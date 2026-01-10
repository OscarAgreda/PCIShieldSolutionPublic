using System.Collections.Generic;
using System.Linq;
using PCIShield.Domain.Entities;
using PCIShield.Domain.Exceptions;

namespace Ardalis.GuardClauses
{
    public static class ROCPackageGuardExtensions
    {
        public static void DuplicateROCPackage(this IGuardClause guardClause, IEnumerable<ROCPackage> existingROCPackages, ROCPackage newROCPackage, string parameterName)
        {
            if (existingROCPackages.Any(a => a.ROCPackageId == newROCPackage.ROCPackageId))
            {
                throw new DuplicateROCPackageException("Cannot add duplicate rocpackage.", parameterName);
            }
        }
    }
}

