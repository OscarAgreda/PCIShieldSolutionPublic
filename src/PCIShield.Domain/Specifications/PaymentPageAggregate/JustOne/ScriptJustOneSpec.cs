using System;
using System.Linq;
using Ardalis.GuardClauses;
using Ardalis.Specification;
using  PCIShield.Domain.Entities;
using  PCIShield.Domain.ModelEntityDto;
  	  
namespace  PCIShield.Domain.Specifications
{
    public class ScriptByIdJustOneSpec : Specification<Script, ScriptEntityDto>
    {
        public ScriptByIdJustOneSpec(Guid scriptId)
        {
            _ = Guard.Against.NullOrEmpty(scriptId, nameof(scriptId));
            _ = Query.Where(script => script.ScriptId == scriptId);
            _ = Query
                .Select(x => new ScriptEntityDto
                {
                    ScriptId = x.ScriptId,
                    PaymentPageId = x.PaymentPageId,
                })
                .AsNoTracking()
                .EnableCache($"ScriptByIdJustOne-{scriptId.ToString()}");
        }
    }
}

