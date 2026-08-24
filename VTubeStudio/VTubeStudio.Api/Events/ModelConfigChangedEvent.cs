using System.Text.Json.Serialization;

namespace VTubeStudio.Api.Events;

public sealed record ModelConfigChangedEvent
{
    [JsonPropertyName("modelID")] public string ModelId { get; init; } = string.Empty;
    [JsonPropertyName("hotkeyConfigChanged")] public bool HotkeyConfigChanged { get; init; }
}
