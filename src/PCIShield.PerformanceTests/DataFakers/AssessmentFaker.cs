using System;
using System.Collections.Generic;
using Bogus;
using PCIShield.BlazorMauiShared.Models.Assessment;
using PCIShield.Domain.ModelsDto;
using PCIShield.PerformanceTests.Infrastructure;

namespace PCIShield.PerformanceTests.DataFakers;

public sealed class AssessmentFaker : Faker<AssessmentDto>
{
    private static readonly object _lock = new();
    private static int _seedCounter = 0;

    public AssessmentFaker(int? seed = null)
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
        RuleFor(o => o.AssessmentId, _ => Guid.NewGuid());
        RuleFor(o => o.TenantId, _ => Guid.NewGuid());
        // Foreign Key: MerchantId - will be set by test context
        RuleFor(o => o.MerchantId, _ => Guid.Empty);
        RuleFor(o => o.AssessmentCode, f => f.Random.AlphaNumeric(10).ToUpper());
        RuleFor(o => o.AssessmentType, f => f.Random.Int(1, 4));
        RuleFor(o => o.AssessmentPeriod, f => f.Lorem.Word());
        RuleFor(o => o.StartDate, f => f.Date.Past(2));
        RuleFor(o => o.EndDate, (f, u) => f.DateBetween(u.StartDate, u.StartDate.AddYears(3)));
        RuleFor(o => o.CompletionDate, _ => default(DateTime?)! /* Default for DateTime? */);
        RuleFor(o => o.Rank, f => f.Random.Int(1, 10));
        RuleFor(o => o.ComplianceScore, _ => default(decimal?)! /* Default for decimal? */);
        RuleFor(o => o.QSAReviewRequired, f => f.Random.Bool());
        RuleFor(o => o.CreatedAt, f => f.Date.Recent(90));
        RuleFor(o => o.CreatedBy, _ => Guid.NewGuid());
        RuleFor(o => o.UpdatedAt, _ => default(DateTime?)! /* Default for DateTime? */);
        RuleFor(o => o.UpdatedBy, _ => default(Guid?)! /* Default for Guid? */);
        RuleFor(o => o.IsDeleted, _ => false);
    }

    public AssessmentDto GenerateWithRelations(Dictionary<string, object> relations = null)
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
