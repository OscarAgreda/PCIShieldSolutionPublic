using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

using PCIShield.Api.Agents.Merchant;

using System.Text.Json;
using System.Text.Json.Serialization;

namespace PCIShield.Api.Agents;

/// <summary>
/// AG-UI Agent Factory for PCIShield Copilot.
/// Creates and configures the AI agent with tools, shared state, and instructions.
/// Uses AIAgentBuilder.Use() pattern for middleware.
/// </summary>
public static class AgentFactory
{
    /// <summary>
    /// Creates the Merchant Copilot agent with all tools configured.
    /// </summary>
    public static AIAgent CreateMerchantCopilot(
        IChatClient chatClient,
        MerchantCopilotTools merchantTools,
        DomainMetadataTools metadataTools,
        JsonSerializerOptions jsonOptions)
    {
        var tools = new List<AITool>
        {
            // Merchant query/search tools
            AIFunctionFactory.Create(merchantTools.SearchMerchantsAsync),
            AIFunctionFactory.Create(merchantTools.GetHighRiskMerchantsAsync),
            
            // Draft creation and editing tools
            AIFunctionFactory.Create(merchantTools.StartMerchantDraftAsync),
            AIFunctionFactory.Create(merchantTools.LoadMerchantDraftAsync),
            AIFunctionFactory.Create(merchantTools.PatchMerchantDraftAsync),
            
            // Metadata discovery tools
            AIFunctionFactory.Create(metadataTools.ListCapabilities),
            AIFunctionFactory.Create(metadataTools.GetCapabilitySchema),
            
            // Commit tools - marked with [APPROVAL REQUIRED] in description
            AIFunctionFactory.Create(
                merchantTools.PostMerchantAsync,
                name: "PostMerchantAsync",
                description: "[REQUIRES APPROVAL] Commit (save) the merchant draft to the database. Human approval required."),
            AIFunctionFactory.Create(
                merchantTools.DeleteMerchantAsync,
                name: "DeleteMerchantAsync",
                description: "[REQUIRES APPROVAL] Delete a merchant. Human approval required."),

            // Field update tool
            AIFunctionFactory.Create(
                merchantTools.UpdateMerchantFieldAsync,
                name: "UpdateMerchantFieldAsync",
                description: "Update a specific field (e.g. ComplianceRank, AnnualCardVolume) for the current merchant.")
        };

        // Create base agent with PCIShield-specific instructions
        var baseAgent = chatClient.CreateAIAgent(
            name: "PCIShield Merchant Copilot",
            instructions: """
                You are PCIShield Merchant Copilot, an AI assistant for PCI compliance management.


                YOUR CAPABILITIES:
                -Search and browse merchants by name, code, or compliance status
                - Create new merchants with proper PCI compliance tracking
                - Edit existing merchant information
                - Identify high - risk merchants that need attention
                - Analyze merchant risk and compliance scores


                RULES:
                1.NEVER invent merchant IDs or data - always use tools to query real data
                2.For ANY write operation(create / update / delete), require human approval
                3.Always show draft state to user before committing changes
                4.Explain compliance implications when relevant
                5.Use the metadata tools to understand what fields are required


                WORKFLOW:
                -For searches: Use SearchMerchantsAsync or GetHighRiskMerchantsAsync
                - For creating: StartMerchantDraftAsync → PatchMerchantDraftAsync → PostMerchantAsync(with approval)
                - For editing: LoadMerchantDraftAsync → PatchMerchantDraftAsync → PostMerchantAsync(with approval)
                - For deletion: DeleteMerchantAsync(with approval)


                Be concise, professional, and helpful.Focus on PCI compliance best practices.
                """,
            tools: tools) ;

        // Return the base agent - AG-UI handles state sync via its protocol
        // The shared state is managed by the AG-UI endpoint
        return baseAgent;
    }
}


