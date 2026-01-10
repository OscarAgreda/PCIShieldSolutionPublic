using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Hosting.AGUI.AspNetCore;
using Microsoft.Extensions.AI;

using PCIShield.Api.Agents.Merchant;

using System.Text.Json.Serialization;

namespace PCIShield.Api.Agents;

/// <summary>
/// Extension methods for configuring AG-UI agent services and endpoints.
/// </summary>
public static class AgentExtensions
{
    /// <summary>
    /// Adds AG-UI agent services to the DI container.
    /// </summary>
    public static IServiceCollection AddPCIShieldAgents(this IServiceCollection services)
    {
        // Add AG-UI plumbing
        services.AddAGUI();

        // Configure JSON options for shared state
        /*
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.TypeInfoResolverChain.Insert(0, AgentJsonSerializerContext.Default);
        });
        */

        // Register copilot tools
        services.AddScoped<MerchantCopilotTools>();
        services.AddScoped<DomainMetadataTools>();

        // Register the AIAgent factory
        services.AddScoped<AIAgent>(sp =>
        {
            var chatClient = sp.GetRequiredService<IChatClient>();
            var merchantTools = sp.GetRequiredService<MerchantCopilotTools>();
            var metadataTools = sp.GetRequiredService<DomainMetadataTools>();


            try
            {
                var test = AgentFactory.CreateMerchantCopilot(
                chatClient,
                merchantTools,
                metadataTools,
                new System.Text.Json.JsonSerializerOptions());
            }
            catch (Exception ex)
            {

                throw;
            }
            return AgentFactory.CreateMerchantCopilot(
                chatClient,
                merchantTools,
                metadataTools,
                new System.Text.Json.JsonSerializerOptions());
        });

        return services;
    }

    /// <summary>
    /// Maps the AG-UI Merchant Copilot endpoint.
    /// </summary>
    public static IEndpointRouteBuilder MapPCIShieldAgents(this IEndpointRouteBuilder app)
    {

        try
        {
            var test = app.ServiceProvider.GetRequiredService<AIAgent>();
        }
        catch (Exception ex )
        {

            throw;
        }

   
        // AG-UI endpoint - frontend connects here for chat-driven forms
        app.MapAGUI("/agui/merchant-copilot", app.ServiceProvider.GetRequiredService<AIAgent>());

        return app;
    }
}
