namespace PCIShield.Api.Agents.Merchant;

/// <summary>
/// Domain Metadata Tools for Merchant Copilot.
/// Allows the AI to discover what capabilities and schemas are available.
/// </summary>
public sealed class DomainMetadataTools
{
    private static readonly CapabilityDescriptor[] _capabilities =
    [
        new("merchant.create", "Create a new merchant with PCI compliance tracking"),
        new("merchant.search", "Search merchants by name, code, or other fields"),
        new("merchant.update", "Update an existing merchant's information"),
        new("merchant.delete", "Delete (soft) a merchant and associated data"),
        new("merchant.analyze_risk", "Analyze merchant risk score and indicators"),
        new("merchant.get_high_risk", "Get high-risk merchants that need attention"),
        new("merchant.list", "List all merchants with pagination"),
        new("assessment.create", "Start a new compliance assessment"),
        new("evidence.upload", "Upload compliance evidence documents"),
    ];

    /// <summary>
    /// List all available capabilities the AI can use.
    /// </summary>
    public IReadOnlyList<CapabilityDescriptor> ListCapabilities() => _capabilities;

    /// <summary>
    /// Get the schema for a specific capability.
    /// Returns field definitions, types, and UI hints.
    /// </summary>
    public CapabilitySchema GetCapabilitySchema(string capabilityId)
    {
        return capabilityId switch
        {
            "merchant.create" => CapabilitySchema.MerchantCreate(),
            "merchant.search" => CapabilitySchema.MerchantSearch(),
            "merchant.analyze_risk" => CapabilitySchema.MerchantRiskAnalysis(),
            _ => CapabilitySchema.NotFound(capabilityId)
        };
    }
}
