using System.Collections.Generic;
using System.Linq;
using PCIShield.Domain.Entities;
using PCIShield.Domain.Exceptions;

namespace Ardalis.GuardClauses
{
    public static class PaymentChannelGuardExtensions
    {
        public static void DuplicatePaymentChannel(this IGuardClause guardClause, IEnumerable<PaymentChannel> existingPaymentChannels, PaymentChannel newPaymentChannel, string parameterName)
        {
            if (existingPaymentChannels.Any(a => a.PaymentChannelId == newPaymentChannel.PaymentChannelId))
            {
                throw new DuplicatePaymentChannelException("Cannot add duplicate paymentChannel.", parameterName);
            }
        }
    }
}

