using System.Text.Json.Serialization;

namespace VTubeStudio.Api.Responses;

public sealed record ModelLoadResponse
{
    [JsonPropertyName("modelID")] public string Id { get; init; } = string.Empty;
}
