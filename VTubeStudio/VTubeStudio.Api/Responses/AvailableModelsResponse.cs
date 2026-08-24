using System.Text.Json.Serialization;
using VTubeStudio.Api.Models;

namespace VTubeStudio.Api.Responses;

public sealed record AvailableModelsResponse
{
    [JsonPropertyName("numberOfModels")] public int Count { get; init; }
    [JsonPropertyName("availableModels")] public IReadOnlyList<Model> Models { get; init; } = [];
}
