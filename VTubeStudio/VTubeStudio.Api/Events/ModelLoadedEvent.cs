using System.Text.Json.Serialization;

namespace VTubeStudio.Api.Events;

public sealed record ModelLoadedEvent
{
    [JsonPropertyName("modelLoaded")] public bool Loaded { get; init; }
    [JsonPropertyName("modelName")] public string ModelName { get; init; } = string.Empty;
    [JsonPropertyName("modelID")] public string ModelId { get; init; } = string.Empty;
}
