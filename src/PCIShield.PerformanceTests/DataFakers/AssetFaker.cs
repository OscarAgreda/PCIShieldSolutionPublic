using System;
using System.Collections.Generic;
using Bogus;
using PCIShield.BlazorMauiShared.Models.Asset;
using PCIShield.Domain.ModelsDto;
using PCIShield.PerformanceTests.Infrastructure;

namespace PCIShield.PerformanceTests.DataFakers;

public sealed class AssetFaker : Faker<AssetDto>
{
    private static readonly object _lock = new();
    private static int _seedCounter = 0;

    public AssetFaker(int? seed = null)
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
        RuleFor(o => o.AssetId, _ => Guid.NewGuid());
        RuleFor(o => o.TenantId, _ => Guid.NewGuid());
        // Foreign Key: MerchantId - will be set by test context
        RuleFor(o => o.MerchantId, _ => Guid.Empty);
        RuleFor(o => o.AssetCode, f => f.Random.AlphaNumeric(10).ToUpper());
        RuleFor(o => o.AssetName, f => f.Lorem.Word());
        RuleFor(o => o.AssetType, f => f.Random.Int(1, 4));
        RuleFor(o => o.IPAddress, f => f.Internet.Email());
        RuleFor(o => o.Hostname, f => f.Lorem.Word());
        RuleFor(o => o.IsInCDE, f => f.Random.Bool(0.75f));
        RuleFor(o => o.NetworkZone, f => f.Lorem.Word());
        RuleFor(o => o.LastScanDate, _ => default(DateTime?)! /* Default for DateTime? */);
        RuleFor(o => o.CreatedAt, f => f.Date.Recent(90));
        RuleFor(o => o.CreatedBy, _ => Guid.NewGuid());
        RuleFor(o => o.UpdatedAt, _ => default(DateTime?)! /* Default for DateTime? */);
        RuleFor(o => o.UpdatedBy, _ => default(Guid?)! /* Default for Guid? */);
        RuleFor(o => o.IsDeleted, _ => false);
    }

    public AssetDto GenerateWithRelations(Dictionary<string, object> relations = null)
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
