using System;
using System.Collections.Generic;
using Bogus;
using PCIShield.BlazorMauiShared.Models.CompensatingControl;
using PCIShield.Domain.ModelsDto;
using PCIShield.PerformanceTests.Infrastructure;

namespace PCIShield.PerformanceTests.DataFakers;

public sealed class CompensatingControlFaker : Faker<CompensatingControlDto>
{
    private static readonly object _lock = new();
    private static int _seedCounter = 0;

    public CompensatingControlFaker(int? seed = null)
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
        RuleFor(o => o.CompensatingControlId, _ => Guid.NewGuid());
        RuleFor(o => o.TenantId, _ => Guid.NewGuid());
        // Foreign Key: ControlId - will be set by test context
        RuleFor(o => o.ControlId, _ => Guid.Empty);
        // Foreign Key: MerchantId - will be set by test context
        RuleFor(o => o.MerchantId, _ => Guid.Empty);
        RuleFor(o => o.Justification, f => f.Lorem.Word());
        RuleFor(o => o.ImplementationDetails, f => f.Lorem.Word());
        RuleFor(o => o.ApprovedBy, _ => default(Guid?)! /* Default for Guid? */);
        RuleFor(o => o.ApprovalDate, _ => default(DateTime?)! /* Default for DateTime? */);
        RuleFor(o => o.ExpiryDate, f => f.Date.Recent(90));
        RuleFor(o => o.Rank, f => f.Random.Int(1, 10));
        RuleFor(o => o.CreatedAt, f => f.Date.Recent(90));
        RuleFor(o => o.CreatedBy, _ => Guid.NewGuid());
        RuleFor(o => o.UpdatedAt, _ => default(DateTime?)! /* Default for DateTime? */);
        RuleFor(o => o.UpdatedBy, _ => default(Guid?)! /* Default for Guid? */);
        RuleFor(o => o.IsDeleted, _ => false);
    }

    public CompensatingControlDto GenerateWithRelations(Dictionary<string, object> relations = null)
    {
        var entity = Generate();

        if (relations != null)
        {
            if (relations.TryGetValue("ControlId", out var controlId) && controlId is Guid controlIdGuid)
                entity.ControlId = controlIdGuid;
            if (relations.TryGetValue("MerchantId", out var merchantId) && merchantId is Guid merchantIdGuid)
                entity.MerchantId = merchantIdGuid;
        }

        return entity;
    }
}
