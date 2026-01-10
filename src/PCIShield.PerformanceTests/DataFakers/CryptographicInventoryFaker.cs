using System;
using System.Collections.Generic;
using Bogus;
using PCIShield.BlazorMauiShared.Models.CryptographicInventory;
using PCIShield.Domain.ModelsDto;
using PCIShield.PerformanceTests.Infrastructure;

namespace PCIShield.PerformanceTests.DataFakers;

public sealed class CryptographicInventoryFaker : Faker<CryptographicInventoryDto>
{
    private static readonly object _lock = new();
    private static int _seedCounter = 0;

    public CryptographicInventoryFaker(int? seed = null)
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
        RuleFor(o => o.CryptographicInventoryId, _ => Guid.NewGuid());
        RuleFor(o => o.TenantId, _ => Guid.NewGuid());
        // Foreign Key: MerchantId - will be set by test context
        RuleFor(o => o.MerchantId, _ => Guid.Empty);
        RuleFor(o => o.KeyName, f => f.Lorem.Word());
        RuleFor(o => o.KeyType, f => f.PickRandom(new[] { "Primary", "Secondary", "Audit", "Compliance" }));
        RuleFor(o => o.Algorithm, f => f.Lorem.Word());
        RuleFor(o => o.KeyLength, f => f.Random.Int(0, 1000));
        RuleFor(o => o.KeyLocation, f => f.Address.StreetAddress());
        RuleFor(o => o.CreationDate, f => f.Date.Past(2));
        RuleFor(o => o.LastRotationDate, _ => default(DateTime?)! /* Default for DateTime? */);
        RuleFor(o => o.NextRotationDue, f => f.Date.Soon(45));
        RuleFor(o => o.CreatedAt, f => f.Date.Recent(90));
        RuleFor(o => o.CreatedBy, _ => Guid.NewGuid());
        RuleFor(o => o.UpdatedAt, _ => default(DateTime?)! /* Default for DateTime? */);
        RuleFor(o => o.UpdatedBy, _ => default(Guid?)! /* Default for Guid? */);
        RuleFor(o => o.IsDeleted, _ => false);
    }

    public CryptographicInventoryDto GenerateWithRelations(Dictionary<string, object> relations = null)
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
