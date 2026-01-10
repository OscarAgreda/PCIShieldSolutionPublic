using System.Collections;
using System.Text.Json.Serialization;

namespace PCIShield.BlazorMauiShared.Models.Agui;

/// <summary>
/// AG-UI protocol message events from the AI copilot.
/// </summary>
public record AguiMessageEvent
{
    [JsonPropertyName("type")]
    public string Type { get; init; } = string.Empty;

    [JsonPropertyName("messageId")]
    public string? MessageId { get; init; }

    [JsonPropertyName("parentMessageId")]
    public string? ParentMessageId { get; init; }

    [JsonPropertyName("delta")]
    public string? Delta { get; init; }

    [JsonPropertyName("toolCallId")]
    public string? ToolCallId { get; init; }

    [JsonPropertyName("toolName")]
    public string? ToolName { get; init; }

    [JsonPropertyName("args")]
    public Dictionary<string, object?>? Args { get; init; }

    [JsonPropertyName("result")]
    public string? Result { get; init; }

    public string ApprovalToken { get; set; }
    public string? Content { get; set; }
    public IEnumerable FieldUpdates { get; set; }
}

/// <summary>
/// AG-UI event types from the protocol.
/// </summary>
public static class AguiEventTypes
{
    public const string TextMessageStart = "TEXT_MESSAGE_START";
    public const string TextMessageContent = "TEXT_MESSAGE_CONTENT";
    public const string TextMessageEnd = "TEXT_MESSAGE_END";
    public const string ToolCallStart = "TOOL_CALL_START";
    public const string ToolCallArgs = "TOOL_CALL_ARGS";
    public const string ToolCallEnd = "TOOL_CALL_END";
    public const string RunStarted = "RUN_STARTED";
    public const string RunFinished = "RUN_FINISHED";
    public const string StateSnapshot = "STATE_SNAPSHOT";
    public const string StateDelta = "STATE_DELTA";
}

/// <summary>
/// Request message to send to AG-UI endpoint.
/// </summary>
public record AguiRequest
{
    [JsonPropertyName("messages")]
    public List<AguiChatMessage> Messages { get; init; } = new();

    [JsonPropertyName("state")]
    public Dictionary<string, object?>? State { get; init; }
}

/// <summary>
/// Chat message for AG-UI protocol.
/// </summary>
public record AguiChatMessage
{
    [JsonPropertyName("role")]
    public string Role { get; init; } = "user";

    [JsonPropertyName("content")]
    public string Content { get; init; } = string.Empty;

    [JsonPropertyName("id")]
    public string? Id { get; init; }
}
