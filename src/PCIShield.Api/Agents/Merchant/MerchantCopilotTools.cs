using MediatR;

using PCIShield.Domain.ModelsDto;

namespace PCIShield.Api.Agents.Merchant;

/// <summary>
/// AG-UI Copilot Tools for Merchant operations.
/// These are the tools the AI agent can call to interact with merchant data.
/// They call into existing CQRS/MediatR handlers and specifications.
/// </summary>
public sealed class MerchantCopilotTools
{
    private readonly MerchantService _merchantService;

    public MerchantCopilotTools(MerchantService merchantService)
    {
        _merchantService = merchantService ?? throw new ArgumentNullException(nameof(merchantService));
    }

    /// <summary>
    /// Search merchants by name, code, or other fields.
    /// </summary>
    public async Task<MerchantSearchResultState> SearchMerchantsAsync(
        string query,
        int topK = 10,
        CancellationToken ct = default)
    {
        var result = await _merchantService.SearchMerchantsAsync(query);

        return result.Match(
            Right: merchants => new MerchantSearchResultState(
                Results: merchants.Take(topK).Select(m => new MerchantSummaryState(
                    m.MerchantId,
                    m.MerchantName ?? "",
                    m.MerchantCode ?? "",
                    m.MerchantLevel,
                    m.ComplianceRank,
                    m.AnnualCardVolume
                )).ToList(),
                SearchTerm: query,
                TotalCount: merchants.Count
            ),
            Left: error => new MerchantSearchResultState(
                Results: Array.Empty<MerchantSummaryState>(),
                SearchTerm: query,
                TotalCount: 0
            )
        );
    }

    /// <summary>
    /// Start a new merchant draft. Does NOT persist until PostMerchant is called.
    /// </summary>
    public Task<MerchantDraftState> StartMerchantDraftAsync(CancellationToken ct = default)
    {
        return Task.FromResult(new MerchantDraftState(
            MerchantId: null,
            MerchantCode: $"M-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6]}",
            MerchantName: null,
            MerchantLevel: 4, // Default to Level 4 (smallest volume)
            AcquirerName: null,
            ProcessorMID: null,
            AnnualCardVolume: 0m,
            NextAssessmentDue: DateTime.UtcNow.AddMonths(3),
            ComplianceRank: 0,
            IsNew: true
        ));
    }

    /// <summary>
    /// Load an existing merchant into draft state for editing.
    /// </summary>
    public async Task<MerchantDraftState> LoadMerchantDraftAsync(
        Guid merchantId,
        CancellationToken ct = default)
    {
        var result = await _merchantService.GetOneFullMerchantByIdAsync(merchantId, withPostGraph: false);

        return result.Match(
            Right: merchant => new MerchantDraftState(
                MerchantId: merchant.MerchantId,
                MerchantCode: merchant.MerchantCode,
                MerchantName: merchant.MerchantName,
                MerchantLevel: merchant.MerchantLevel,
                AcquirerName: merchant.AcquirerName,
                ProcessorMID: merchant.ProcessorMID,
                AnnualCardVolume: merchant.AnnualCardVolume,
                NextAssessmentDue: merchant.NextAssessmentDue,
                ComplianceRank: merchant.ComplianceRank,
                IsNew: false
            ),
            Left: error => new MerchantDraftState(
                MerchantId: null,
                MerchantCode: null,
                MerchantName: null,
                MerchantLevel: null,
                AcquirerName: null,
                ProcessorMID: null,
                AnnualCardVolume: null,
                NextAssessmentDue: null,
                ComplianceRank: null,
                IsNew: false
            )
        );
    }

    /// <summary>
    /// Update a field in the current draft. Returns the updated draft state.
    /// </summary>
    public Task<MerchantDraftState> PatchMerchantDraftAsync(
        MerchantDraftState currentDraft,
        string? merchantName = null,
        string? merchantCode = null,
        int? merchantLevel = null,
        string? acquirerName = null,
        string? processorMID = null,
        decimal? annualCardVolume = null,
        DateTime? nextAssessmentDue = null,
        CancellationToken ct = default)
    {
        return Task.FromResult(currentDraft with
        {
            MerchantName = merchantName ?? currentDraft.MerchantName,
            MerchantCode = merchantCode ?? currentDraft.MerchantCode,
            MerchantLevel = merchantLevel ?? currentDraft.MerchantLevel,
            AcquirerName = acquirerName ?? currentDraft.AcquirerName,
            ProcessorMID = processorMID ?? currentDraft.ProcessorMID,
            AnnualCardVolume = annualCardVolume ?? currentDraft.AnnualCardVolume,
            NextAssessmentDue = nextAssessmentDue ?? currentDraft.NextAssessmentDue
        });
    }

    /// <summary>
    /// Commit (save) the merchant draft to the database.
    /// This is an approval-gated operation (human-in-the-loop).
    /// </summary>
    public async Task<MerchantDraftResult> PostMerchantAsync(
        MerchantDraftState draft,
        CancellationToken ct = default)
    {
        if (draft.MerchantName is null || draft.MerchantCode is null)
        {
            return new MerchantDraftResult(false, null, "Merchant name and code are required");
        }

        var dto = new MerchantDto
        {
            MerchantId = draft.MerchantId ?? Guid.CreateVersion7(),
            MerchantCode = draft.MerchantCode,
            MerchantName = draft.MerchantName,
            MerchantLevel = draft.MerchantLevel ?? 4,
            AcquirerName = draft.AcquirerName ?? "Unknown",
            ProcessorMID = draft.ProcessorMID ?? "TBD",
            AnnualCardVolume = draft.AnnualCardVolume ?? 0m,
            NextAssessmentDue = draft.NextAssessmentDue ?? DateTime.UtcNow.AddMonths(3),
            ComplianceRank = draft.ComplianceRank ?? 0,
            CreatedAt = DateTime.UtcNow,
            TenantId = Guid.Empty,
            CreatedBy = Guid.Empty,
            IsDeleted = false
        };

        if (draft.IsNew)
        {
            var result = await _merchantService.CreateMerchantAsync(dto);
            return result.Match(
                Right: merchant => new MerchantDraftResult(true, merchant.MerchantId, "Merchant created successfully"),
                Left: error => new MerchantDraftResult(false, null, $"Failed to create: {error}")
            );
        }
        else
        {
            var result = await _merchantService.UpdateMerchantAsync(draft.MerchantId!.Value, dto);
            return result.Match(
                Right: merchant => new MerchantDraftResult(true, merchant.MerchantId, "Merchant updated successfully"),
                Left: error => new MerchantDraftResult(false, null, $"Failed to update: {error}")
            );
        }
    }

    /// <summary>
    /// Delete a merchant (soft delete).
    /// This is an approval-gated operation (human-in-the-loop).
    /// </summary>
    public async Task<MerchantDraftResult> DeleteMerchantAsync(
        Guid merchantId,
        CancellationToken ct = default)
    {
        var result = await _merchantService.DeleteMerchantAsync(merchantId);
        return result.Match(
            Right: success => new MerchantDraftResult(success, merchantId, success ? "Merchant deleted" : "Delete failed"),
            Left: error => new MerchantDraftResult(false, merchantId, $"Failed to delete: {error}")
        );
    }

    /// <summary>
    /// Get high-risk merchants that need attention.
    /// </summary>
    public async Task<MerchantSearchResultState> GetHighRiskMerchantsAsync(
        int maxComplianceRank = 60,
        int topK = 20,
        CancellationToken ct = default)
    {
        var result = await _merchantService.GetMerchantPagedList(1, topK * 2);

        return result.Match(
            Right: response =>
            {
                var highRisk = response.Merchants?
                    .Where(m => m.ComplianceRank < maxComplianceRank)
                    .OrderBy(m => m.ComplianceRank)
                    .Take(topK)
                    .Select(m => new MerchantSummaryState(
                        m.MerchantId,
                        m.MerchantName ?? "",
                        m.MerchantCode ?? "",
                        m.MerchantLevel,
                        m.ComplianceRank,
                        m.AnnualCardVolume
                    )).ToList() ?? new List<MerchantSummaryState>();

                return new MerchantSearchResultState(
                    Results: highRisk,
                    SearchTerm: $"compliance_rank < {maxComplianceRank}",
                    TotalCount: highRisk.Count
                );
            },
            Left: error => new MerchantSearchResultState(
                Results: Array.Empty<MerchantSummaryState>(),
                SearchTerm: null,
                TotalCount: 0
            )
        );
    }
    /// <summary>
    /// Update a specific field for a merchant.
    /// Use this when the user asks to "set" or "update" a single property (e.g., Compliance Rank, Annual Card Volume).
    /// </summary>
    /// <param name="merchantId">The ID of the merchant to update</param>
    /// <param name="fieldName">The name of the field to update (e.g., "ComplianceRank", "AnnualCardVolume")</param>
    /// <param name="value">The new value to set</param>
    /// <param name="ct">Cancellation token</param>
    public Task<MerchantUpdateResult> UpdateMerchantFieldAsync(
        Guid merchantId,
        string fieldName,
        string value,
        CancellationToken ct = default)
    {
        // This tool primarily serves as a signal to the UI (AiCommandPanel) to perform the update locally.
        // We return a success result so the AI knows the intent was captured.
        // The actual persistence happens when the user clicks "Save" in the UI.
        return Task.FromResult(new MerchantUpdateResult(true, $"Updated {fieldName} to {value}"));
    }

    /// <summary>
    /// Result of a single field update.
    /// </summary>
    public sealed record MerchantUpdateResult(bool Success, string Message);
}

/// <summary>
/// Result of a merchant draft operation.
/// </summary>
public sealed record MerchantDraftResult(bool Success, Guid? MerchantId, string Message);
