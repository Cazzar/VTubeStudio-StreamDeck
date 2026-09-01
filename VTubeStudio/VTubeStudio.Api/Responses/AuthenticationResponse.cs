using System.Text.Json.Serialization;

namespace VTubeStudio.Api.Responses;

public sealed record AuthenticationResponse
{
    [JsonPropertyName("authenticated")] public bool Authenticated { get; init; }
    [JsonPropertyName("reason")] public string? Reason { get; init; }
}
