using System;
using System.Collections.Generic;
using Bogus;
using PCIShield.BlazorMauiShared.Models.Merchant;
using PCIShield.Domain.ModelsDto;
using PCIShield.PerformanceTests.Infrastructure;

namespace PCIShield.PerformanceTests.DataFakers;

public sealed class MerchantFaker : Faker<MerchantDto>
{
    private static readonly object _lock = new();
    private static int _seedCounter = 0;

    public MerchantFaker(int? seed = null)
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
        RuleFor(o => o.MerchantId, _ => Guid.NewGuid());
        RuleFor(o => o.TenantId, _ => Guid.NewGuid());
        RuleFor(o => o.MerchantCode, f => f.MerchantCode());
        RuleFor(o => o.MerchantName, f => f.Lorem.Word());
        RuleFor(o => o.MerchantLevel, f => f.Random.Int(1, 10));
        RuleFor(o => o.AcquirerName, f => f.Lorem.Word());
        RuleFor(o => o.ProcessorMID, f => f.Random.AlphaNumeric(10).ToUpper());
        RuleFor(o => o.AnnualCardVolume, f => Math.Round(f.Random.Decimal(0, 1000), 2));
        RuleFor(o => o.LastAssessmentDate, _ => default(DateTime?)! /* Default for DateTime? */);
        RuleFor(o => o.NextAssessmentDue, f => f.Date.Soon(45));
        RuleFor(o => o.ComplianceRank, f => f.Random.Int(1, 10));
        RuleFor(o => o.CreatedAt, f => f.Date.Recent(90));
        RuleFor(o => o.CreatedBy, _ => Guid.NewGuid());
        RuleFor(o => o.UpdatedAt, _ => default(DateTime?)! /* Default for DateTime? */);
        RuleFor(o => o.UpdatedBy, _ => default(Guid?)! /* Default for Guid? */);
        RuleFor(o => o.IsDeleted, _ => false);
    }

    public MerchantDto GenerateWithRelations(Dictionary<string, object> relations = null)
    {
        var entity = Generate();

        if (relations != null)
        {
        }

        return entity;
    }
}
