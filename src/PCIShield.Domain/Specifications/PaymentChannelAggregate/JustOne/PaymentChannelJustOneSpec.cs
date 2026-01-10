using System;
using System.Linq;
using Ardalis.GuardClauses;
using Ardalis.Specification;
using  PCIShield.Domain.Entities;
using  PCIShield.Domain.ModelEntityDto;
  	  
namespace  PCIShield.Domain.Specifications
{
    public class PaymentChannelByIdJustOneSpec : Specification<PaymentChannel, PaymentChannelEntityDto>
    {
        public PaymentChannelByIdJustOneSpec(Guid paymentChannelId)
        {
            _ = Guard.Against.NullOrEmpty(paymentChannelId, nameof(paymentChannelId));
            _ = Query.Where(paymentChannel => paymentChannel.PaymentChannelId == paymentChannelId);
            _ = Query
                .Select(x => new PaymentChannelEntityDto
                {
                    PaymentChannelId = x.PaymentChannelId,
                    MerchantId = x.MerchantId,
                })
                .AsNoTracking()
                .EnableCache($"PaymentChannelByIdJustOne-{paymentChannelId.ToString()}");
        }
    }
}

