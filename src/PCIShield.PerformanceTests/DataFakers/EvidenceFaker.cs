using System;
using System.Collections.Generic;
using Bogus;
using PCIShield.BlazorMauiShared.Models.Evidence;
using PCIShield.Domain.ModelsDto;
using PCIShield.PerformanceTests.Infrastructure;

namespace PCIShield.PerformanceTests.DataFakers;

public sealed class EvidenceFaker : Faker<EvidenceDto>
{
    private static readonly object _lock = new();
    private static int _seedCounter = 0;

    public EvidenceFaker(int? seed = null)
    {
        lock (_lock)
        {
            var actualSeed = seed ?? (_seedCounter++ + DateTime.Now.Millisecond);
            UseSeed(actualSeed);
        }

        ConfigureRules();
    }

    private void ConfigureRules()
    {
        RuleFor(o => o.EvidenceId, _ => Guid.NewGuid());
        RuleFor(o => o.TenantId, _ => Guid.NewGuid());
        // Foreign Key: MerchantId - will be set by test context
        RuleFor(o => o.MerchantId, _ => Guid.Empty);
        RuleFor(o => o.EvidenceCode, f => f.Lorem.Word());
        RuleFor(o => o.EvidenceTitle, f => f.Lorem.Word());
        RuleFor(o => o.EvidenceType, f => f.Random.Number(10000, 999999));
        RuleFor(o => o.CollectedDate, f => f.Date.Recent(90));
        RuleFor(o => o.FileHash, f => f.Lorem.Word());
        RuleFor(o => o.StorageUri, f => f.Lorem.Word());
        RuleFor(o => o.IsValid, f => f.Random.Bool());
        RuleFor(o => o.CreatedAt, f => f.Date.Recent(90));
        RuleFor(o => o.CreatedBy, _ => Guid.NewGuid());
        RuleFor(o => o.UpdatedAt, _ => default(DateTime?)! /* Default for DateTime? */);
        RuleFor(o => o.UpdatedBy, _ => default(Guid?)! /* Default for Guid? */);
        RuleFor(o => o.IsDeleted, _ => false);
    }

    public EvidenceDto GenerateWithRelations(Dictionary<string, object> relations = null)
    {
        var entity = Generate();

        if (relations != null)
        {
            if (relations.TryGetValue("MerchantId", out var merchantId) && merchantId is Guid merchantIdGuid)
                entity.MerchantId = merchantIdGuid;
        }

        return entity;
    }
}
