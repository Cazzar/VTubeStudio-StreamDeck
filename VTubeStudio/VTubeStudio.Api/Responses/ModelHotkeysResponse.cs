using System.Text.Json.Serialization;
using VTubeStudio.Api.Models;

namespace VTubeStudio.Api.Responses;

public sealed record ModelHotkeysResponse
{
    [JsonPropertyName("modelLoaded")] public bool ModelLoaded { get; init; }
    [JsonPropertyName("modelName")] public string ModelName { get; init; } = string.Empty;
    [JsonPropertyName("modelID")] public string ModelId { get; init; } = string.Empty;
    [JsonPropertyName("availableHotkeys")] public IReadOnlyList<Hotkey> Hotkeys { get; init; } = [];
}
