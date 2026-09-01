using System.Text.Json.Serialization;
using VTubeStudio.Api.Models;

namespace VTubeStudio.Api.Responses;

public sealed record CurrentModelResponse
{
    [JsonPropertyName("modelLoaded")] public bool IsLoaded { get; init; }
    [JsonPropertyName("modelName")] public string Name { get; init; } = string.Empty;
    [JsonPropertyName("modelID")] public string Id { get; init; } = string.Empty;
    [JsonPropertyName("modelPosition")] public ModelPosition Position { get; init; } = new();
}
