using System.Collections.Generic;
using System.Linq;
using PCIShield.Domain.Entities;
using PCIShield.Domain.Exceptions;

namespace Ardalis.GuardClauses
{
    public static class PaymentPageGuardExtensions
    {
        public static void DuplicatePaymentPage(this IGuardClause guardClause, IEnumerable<PaymentPage> existingPaymentPages, PaymentPage newPaymentPage, string parameterName)
        {
            if (existingPaymentPages.Any(a => a.PaymentPageId == newPaymentPage.PaymentPageId))
            {
                throw new DuplicatePaymentPageException("Cannot add duplicate paymentPage.", parameterName);
            }
        }
    }
}

