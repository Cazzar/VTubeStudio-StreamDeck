using System.Text.Json.Serialization;

namespace VTubeStudio.Api.Responses;

public sealed record ApiStateResponse
{
    [JsonPropertyName("active")] public bool Active { get; init; }
    [JsonPropertyName("vTubeStudioVersion")] public string Version { get; init; } = string.Empty;
    [JsonPropertyName("currentSessionAuthenticated")] public bool Authenticated { get; init; }
}
