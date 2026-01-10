using System;
using System.Collections.Generic;
using Bogus;
using PCIShield.BlazorMauiShared.Models.Control;
using PCIShield.Domain.ModelsDto;
using PCIShield.PerformanceTests.Infrastructure;

namespace PCIShield.PerformanceTests.DataFakers;

public sealed class ControlFaker : Faker<ControlDto>
{
    private static readonly object _lock = new();
    private static int _seedCounter = 0;

    public ControlFaker(int? seed = null)
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
        RuleFor(o => o.ControlId, _ => Guid.NewGuid());
        RuleFor(o => o.TenantId, _ => Guid.NewGuid());
        RuleFor(o => o.ControlCode, f => f.Random.AlphaNumeric(10).ToUpper());
        RuleFor(o => o.RequirementNumber, f => f.Lorem.Word());
        RuleFor(o => o.ControlTitle, f => f.Lorem.Word());
        RuleFor(o => o.ControlDescription, f => f.Lorem.Word());
        RuleFor(o => o.TestingGuidance, f => f.Random.AlphaNumeric(10).ToUpper());
        RuleFor(o => o.FrequencyDays, f => f.Random.Int(0, 1000));
        RuleFor(o => o.IsMandatory, f => f.Random.Bool(0.75f));
        RuleFor(o => o.EffectiveDate, f => f.Date.Recent(90));
        RuleFor(o => o.CreatedAt, f => f.Date.Recent(90));
        RuleFor(o => o.CreatedBy, _ => Guid.NewGuid());
        RuleFor(o => o.UpdatedAt, _ => default(DateTime?)! /* Default for DateTime? */);
        RuleFor(o => o.UpdatedBy, _ => default(Guid?)! /* Default for Guid? */);
        RuleFor(o => o.IsDeleted, _ => false);
    }

    public ControlDto GenerateWithRelations(Dictionary<string, object> relations = null)
    {
        var entity = Generate();

        if (relations != null)
        {
        }

        return entity;
    }
}
