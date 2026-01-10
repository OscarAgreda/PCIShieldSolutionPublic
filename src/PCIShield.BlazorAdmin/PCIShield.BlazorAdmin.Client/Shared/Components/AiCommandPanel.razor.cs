using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

using LanguageExt;
using LanguageExt.Common;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Localization;

using MudBlazor;

using PCIShield.BlazorMauiShared.Models.Agui;

using PCIShield.Client.Services.Agui;
using PCIShield.Domain.ModelsDto;

using static LanguageExt.Prelude;

namespace PCIShield.BlazorAdmin.Client.Shared.Components;

/// <summary>
/// Code-behind for AiCommandPanel.razor.
/// FIXED VERSION - Follows PCIShield MudBlazor patterns (matches MerchantMasterOrchestrator style).
/// </summary>
public partial class AiCommandPanel : ComponentBase, IAsyncDisposable
{
    // ✅ INJECTED SERVICES
    // Note: ISnackbar and IDialogService are globally injected via _Imports.razor
    [Inject] private ILogger<AiCommandPanel> _logger { get; set; } = default!;
    [Inject] private IStringLocalizer<AiCommandPanel> Localizer { get; set; } = default!;
    [Inject] private IHttpAguiClientService AguiClient { get; set; } = default!;


    /// <summary>
    /// Optional: The current merchant being edited (for context).
    /// </summary>
    [Parameter] public MerchantDto? CurrentMerchant { get; set; }

    /// <summary>
    /// Callback when a field update is requested by AI.
    /// </summary>
    [Parameter] public EventCallback<AiFieldUpdate> OnFieldUpdateRequested { get; set; }

    /// <summary>
    /// Callback when the panel close button is clicked.
    /// </summary>
    [Parameter] public EventCallback OnClose { get; set; }

    // Private state
    private readonly List<ChatMessage> _messages = new();
    private readonly List<AguiChatMessage> _conversationHistory = new();
    private readonly CompositeDisposable _disposables = new();

    private string _inputMessage = string.Empty;
    private bool _isStreaming;
    private PendingApproval? _pendingApproval;

    /// <summary>
    /// Internal chat message for UI display.
    /// </summary>
    private sealed class ChatMessage
    {
        public string Content { get; init; } = string.Empty;
        public bool IsUser { get; init; }
        public bool IsToolCall { get; init; }
        public string? ToolName { get; init; }
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Pending approval for destructive operations.
    /// </summary>
    private sealed class PendingApproval
    {
        public string Message { get; init; } = string.Empty;
        public string ApprovalToken { get; init; } = string.Empty;
    }

    protected override Task OnInitializedAsync()
    {
        _logger.LogInformation("AiCommandPanel initialized");
        return Task.CompletedTask;
    }

    /// <summary>
    /// Sends user message to AI agent and streams responses.
    /// ✅ FIXED: Added InvokeAsync + StateHasChanged pattern (matches MerchantMasterOrchestrator).
    /// </summary>
    private async Task SendMessageAsync()
    {
        if (string.IsNullOrWhiteSpace(_inputMessage)) return;

        var userMessage = _inputMessage.Trim();
        _inputMessage = string.Empty;

        // Add user message to chat
        _messages.Add(new ChatMessage
        {
            Content = userMessage,
            IsUser = true
        });

        // ✅ FIXED: Call StateHasChanged to update UI immediately
        await InvokeAsync(StateHasChanged);

        _isStreaming = true;

        try
        {
            var history = _conversationHistory.ToList();
            var sharedState = CurrentMerchant != null
                ? new Dictionary<string, object?> { ["currentMerchant"] = CurrentMerchant }
                : null;

            // Add user message to history
            _conversationHistory.Add(new AguiChatMessage
            {
                Role = "user",
                Content = userMessage
            });

            var assistantMessageBuilder = new System.Text.StringBuilder();

            // Stream responses from AI
            await foreach (var evt in AguiClient.SendMessageAsync(userMessage, history, sharedState))
            {
                // ✅ FIXED: Wrap in InvokeAsync for thread-safe UI updates
                await InvokeAsync(async () => await ProcessAguiEventAsync(evt, assistantMessageBuilder));
            }

            // Add final assistant message to history
            if (assistantMessageBuilder.Length > 0)
            {
                _conversationHistory.Add(new AguiChatMessage
                {
                    Role = "assistant",
                    Content = assistantMessageBuilder.ToString()
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error streaming AI response");

            // ✅ FIXED: InvokeAsync for error message
            await InvokeAsync(() =>
            {
                _messages.Add(new ChatMessage
                {
                    Content = $"Error: {ex.Message}",
                    IsUser = false
                });
                Snackbar.Add("Failed to communicate with AI", Severity.Error);
                StateHasChanged();
            });
        }
        finally
        {
            _isStreaming = false;

            // ✅ FIXED: Final StateHasChanged
            await InvokeAsync(StateHasChanged);
        }
    }

    /// <summary>
    /// Processes individual SSE events from the AI agent.
    /// ✅ FIXED: Now properly async and calls StateHasChanged.
    /// </summary>
    private async Task ProcessAguiEventAsync(AguiMessageEvent evt, System.Text.StringBuilder assistantMessageBuilder)
    {
        // Map AG-UI protocol event types to our handlers
        var eventType = evt.Type.ToUpperInvariant();
        
        switch (eventType)
        {
            case "TEXT_MESSAGE_START":
            case "TEXT_MESSAGE_CONTENT":
            case "TEXT_CHUNK":
                // Streaming text from AI (handle both protocol names)
                if (!string.IsNullOrEmpty(evt.Content))
                {
                    assistantMessageBuilder.Append(evt.Content);

                    // Update last message or create new one
                    if (_messages.Count > 0 && !_messages[^1].IsUser)
                    {
                        // Update existing assistant message (can't mutate, so remove and re-add)
                        var lastMsg = _messages[^1];
                        _messages.RemoveAt(_messages.Count - 1);
                        _messages.Add(new ChatMessage
                        {
                            Content = assistantMessageBuilder.ToString(),
                            IsUser = false,
                            Timestamp = lastMsg.Timestamp
                        });
                    }
                    else
                    {
                        _messages.Add(new ChatMessage
                        {
                            Content = assistantMessageBuilder.ToString(),
                            IsUser = false
                        });
                    }

                    // ✅ FIXED: Update UI as text streams in
                    StateHasChanged();
                }
                break;

            case "TEXT_MESSAGE_END":
                // Message complete (no action needed, final content already appended)
                break;

            case "RUN_STARTED":
                // AI agent started processing (could show "thinking" indicator here if desired)
                break;

            case "RUN_FINISHED":
                // AI agent finished processing
                break;

            case "TOOL_CALL_START":
            case "TOOL_CALL":
            case "TOOL_CALL_END":
                // AI is calling a tool
                _messages.Add(new ChatMessage
                {
                    Content = evt.Content ?? "Using tool...",
                    IsUser = false,
                    IsToolCall = true,
                    ToolName = evt.ToolName
                });
                StateHasChanged();
                break;

            case "TOOL_CALL_ARGS":
                // Tool arguments being passed (could display details if needed)
                break;

            case "TOOL_RESULT":
            case "RESULT":
                // Tool execution result
                if (!string.IsNullOrEmpty(evt.Content))
                {
                    _messages.Add(new ChatMessage
                    {
                        Content = $"✓ {evt.Content}",
                        IsUser = false
                    });
                    StateHasChanged();
                }
                break;

            case "STATE_SNAPSHOT":
            case "STATE_DELTA":
                // State synchronization events (could update shared state here)
                break;

            case "REQUIRES_APPROVAL":
            case "APPROVAL_REQUIRED":
                // Destructive operation needs approval
                _pendingApproval = new PendingApproval
                {
                    Message = evt.Content ?? "This action requires approval",
                    ApprovalToken = evt.ApprovalToken ?? Guid.NewGuid().ToString()
                };
                StateHasChanged();
                break;

            case "FIELD_UPDATE":
            case "UPDATE_FIELD":
                // AI wants to update a field
                if (OnFieldUpdateRequested.HasDelegate && evt.FieldUpdates != null)
                {
                    foreach (dynamic update in evt.FieldUpdates)
                    {
                        await OnFieldUpdateRequested.InvokeAsync(new AiFieldUpdate
                        {
                            FieldName = update.FieldName,
                            Value = update.NewValue ?? update.Value
                        });
                    }
                }
                break;

            case "ERROR":
            case "EXCEPTION":
                _messages.Add(new ChatMessage
                {
                    Content = $"❌ {evt.Content}",
                    IsUser = false
                });
                Snackbar.Add(evt.Content ?? "An error occurred", Severity.Error);
                StateHasChanged();
                break;

            case "DONE":
            case "COMPLETE":
            case "FINISHED":
                // Stream complete
                break;

            default:
                // Silently ignore unknown event types (they're informational)
                _logger.LogDebug("Unhandled AG-UI event type: {Type}", evt.Type);
                break;
        }
    }

    /// <summary>
    /// Handles keyboard shortcuts (Enter = send).
    /// </summary>
    private async Task HandleKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Enter" && !e.ShiftKey && !string.IsNullOrWhiteSpace(_inputMessage))
        {
            await SendMessageAsync();
        }
    }

    /// <summary>
    /// Closes the AI panel.
    /// </summary>
    private async Task OnCloseClicked()
    {
        if (OnClose.HasDelegate)
        {
            await OnClose.InvokeAsync();
        }
    }

    /// <summary>
    /// User approves a destructive operation.
    /// </summary>
    private async Task ApproveAction()
    {
        if (_pendingApproval == null) return;

        try
        {
            // Send approval back to AI
            var approvalMessage = $"[APPROVED:{_pendingApproval.ApprovalToken}]";

            _messages.Add(new ChatMessage
            {
                Content = "✓ Approved",
                IsUser = true
            });

            _pendingApproval = null;

            // ✅ FIXED: StateHasChanged after approval
            await InvokeAsync(StateHasChanged);

            // Continue conversation with approval
            await SendApprovalMessageAsync(approvalMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving action");
            Snackbar.Add("Failed to approve action", Severity.Error);
        }
    }

    /// <summary>
    /// User rejects a destructive operation.
    /// </summary>
    private async Task RejectAction()
    {
        if (_pendingApproval == null) return;

        _messages.Add(new ChatMessage
        {
            Content = "✗ Cancelled",
            IsUser = true
        });

        _pendingApproval = null;

        // ✅ FIXED: StateHasChanged after rejection
        await InvokeAsync(StateHasChanged);

        Snackbar.Add("Action cancelled", Severity.Info);
    }

    /// <summary>
    /// Sends approval message to continue AI workflow.
    /// </summary>
    private async Task SendApprovalMessageAsync(string approvalMessage)
    {
        _isStreaming = true;

        try
        {
            var history = _conversationHistory.ToList();
            var assistantMessageBuilder = new System.Text.StringBuilder();

            await foreach (var evt in AguiClient.SendMessageAsync(approvalMessage, history, null))
            {
                await InvokeAsync(async () => await ProcessAguiEventAsync(evt, assistantMessageBuilder));
            }

            if (assistantMessageBuilder.Length > 0)
            {
                _conversationHistory.Add(new AguiChatMessage
                {
                    Role = "assistant",
                    Content = assistantMessageBuilder.ToString()
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending approval");
            Snackbar.Add("Failed to process approval", Severity.Error);
        }
        finally
        {
            _isStreaming = false;
            await InvokeAsync(StateHasChanged);
        }
    }

    public async ValueTask DisposeAsync()
    {
        _disposables.Dispose();
        await Task.CompletedTask;
    }
}

/// <summary>
/// DTO for AI field update requests.
/// </summary>
public sealed class AiFieldUpdate
{
    public string FieldName { get; init; } = string.Empty;
    public object? Value { get; init; }
}
