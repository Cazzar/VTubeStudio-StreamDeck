using System.Text.Json.Serialization;

namespace VTubeStudio.Api.Models;

public sealed record Model
{
    [JsonPropertyName("modelLoaded")] public bool Loaded { get; init; }
    [JsonPropertyName("modelName")] public string Name { get; init; } = string.Empty;
    [JsonPropertyName("modelID")] public string Id { get; init; } = string.Empty;
    [JsonPropertyName("vtsModelName")] public string? VtsModelName { get; init; }
    [JsonPropertyName("vtsModelIconName")] public string? VtsModelIconName { get; init; }
}
