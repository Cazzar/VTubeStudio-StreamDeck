using System.Text.Json.Serialization;

namespace VTubeStudio.Api.Events;

public sealed record HotkeyTriggeredEvent
{
    [JsonPropertyName("hotkeyID")] public string HotkeyId { get; init; } = string.Empty;
    [JsonPropertyName("hotkeyName")] public string HotkeyName { get; init; } = string.Empty;
    [JsonPropertyName("hotkeyType")] public string HotkeyType { get; init; } = string.Empty;
}