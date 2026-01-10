using System.Text.Json.Serialization;

namespace PCIShield.Api.Agents.Merchant;

/// <summary>
/// AG-UI Shared State for Merchant Copilot.
/// This state is synced between agent and frontend, enabling chat-driven form composition.
/// </summary>
public sealed record MerchantCopilotState(
    MerchantDraftState? Draft,
    string? UiHint // e.g. "show_merchant_form", "show_risk_analysis", "show_search_results"
);

/// <summary>
/// Draft state for merchant creation/editing.
/// This is what the MudBlazor UI renders as a form.
/// </summary>
public sealed record MerchantDraftState(
    Guid? MerchantId,
    string? MerchantCode,
    string? MerchantName,
    int? MerchantLevel,
    string? AcquirerName,
    string? ProcessorMID,
    decimal? AnnualCardVolume,
    DateTime? NextAssessmentDue,
    int? ComplianceRank,
    bool IsNew
);

/// <summary>
/// Search result state when user asks to find merchants.
/// </summary>
public sealed record MerchantSearchResultState(
    IReadOnlyList<MerchantSummaryState> Results,
    string? SearchTerm,
    int TotalCount
);

/// <summary>
/// Summary of a merchant for list/search views.
/// </summary>
public sealed record MerchantSummaryState(
    Guid MerchantId,
    string MerchantName,
    string MerchantCode,
    int MerchantLevel,
    int ComplianceRank,
    decimal AnnualCardVolume
);

/// <summary>
/// Risk analysis state from BI engines.
/// </summary>
public sealed record MerchantRiskAnalysisState(
    Guid MerchantId,
    string MerchantName,
    double OverallRiskScore,
    string RiskLevel, // "Low", "Medium", "High", "Critical"
    IReadOnlyList<RiskIndicatorState> Indicators,
    DateTime AnalyzedAt
);

public sealed record RiskIndicatorState(
    string Category,
    string Description,
    double Score,
    string Severity
);

/// <summary>
/// Capability descriptor for domain metadata discovery.
/// Allows the AI to understand what operations are available.
/// </summary>
public sealed record CapabilityDescriptor(string Id, string Description);

/// <summary>
/// Schema for a capability, describing its fields.
/// </summary>
public sealed record CapabilitySchema(string Id, IReadOnlyList<FieldSpec> Fields)
{
    public static CapabilitySchema MerchantCreate() => new(
        "merchant.create",
        new[]
        {
            new FieldSpec("MerchantName", "string", true, "text"),
            new FieldSpec("MerchantCode", "string", true, "text"),
            new FieldSpec("MerchantLevel", "int", true, "select:1,2,3,4"),
            new FieldSpec("AcquirerName", "string", true, "text"),
            new FieldSpec("ProcessorMID", "string", true, "text"),
            new FieldSpec("AnnualCardVolume", "decimal", true, "currency"),
            new FieldSpec("NextAssessmentDue", "DateTime", true, "date"),
        });

    public static CapabilitySchema MerchantSearch() => new(
        "merchant.search",
        new[]
        {
            new FieldSpec("SearchTerm", "string", true, "text"),
            new FieldSpec("PageNumber", "int", false, "number"),
            new FieldSpec("PageSize", "int", false, "number"),
        });

    public static CapabilitySchema MerchantRiskAnalysis() => new(
        "merchant.analyze_risk",
        new[]
        {
            new FieldSpec("MerchantId", "Guid", true, "lookup:merchant"),
        });

    public static CapabilitySchema NotFound(string id) => new(id, Array.Empty<FieldSpec>());
}

public sealed record FieldSpec(string Name, string Type, bool Required, string Ui);

/// <summary>
/// Knowledge base hit from RAG search.
/// </summary>
public sealed record KbHit(string Source, string Text, double Similarity);
