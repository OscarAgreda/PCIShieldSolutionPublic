using System;
using System.Linq;
using Ardalis.GuardClauses;
using Ardalis.Specification;
using  PCIShield.Domain.Entities;
using  PCIShield.Domain.ModelEntityDto;
  	  
namespace  PCIShield.Domain.Specifications
{
    public class PaymentPageByIdJustOneSpec : Specification<PaymentPage, PaymentPageEntityDto>
    {
        public PaymentPageByIdJustOneSpec(Guid paymentPageId)
        {
            _ = Guard.Against.NullOrEmpty(paymentPageId, nameof(paymentPageId));
            _ = Query.Where(paymentPage => paymentPage.PaymentPageId == paymentPageId);
            _ = Query
                .Select(x => new PaymentPageEntityDto
                {
                    PaymentPageId = x.PaymentPageId,
                    PaymentChannelId = x.PaymentChannelId,
                })
                .AsNoTracking()
                .EnableCache($"PaymentPageByIdJustOne-{paymentPageId.ToString()}");
        }
    }
}

