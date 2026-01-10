using PCIShield.BlazorMauiShared.Models.Agui;

namespace PCIShield.Client.Services.Agui;

/// <summary>
/// HTTP client service for AG-UI endpoint communication.
/// Streams AI copilot responses via Server-Sent Events (SSE).
/// </summary>
public interface IHttpAguiClientService
{
    /// <summary>
    /// Sends a chat message to the AG-UI Merchant Copilot endpoint.
    /// Returns an async stream of message events (SSE).
    /// </summary>
    /// <param name="message">User message text</param>
    /// <param name="conversationHistory">Previous messages in conversation</param>
    /// <param name="sharedState">Current merchant state (optional)</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Async stream of AG-UI message events</returns>
    IAsyncEnumerable<AguiMessageEvent> SendMessageAsync(
        string message,
        IReadOnlyList<AguiChatMessage>? conversationHistory = null,
        Dictionary<string, object?>? sharedState = null,
        CancellationToken ct = default);

    /// <summary>
    /// Gets the AG-UI endpoint URL.
    /// </summary>
    string EndpointUrl { get; }
}
