using System.Text.Json.Serialization;

namespace VTubeStudio.Api.Responses;

public sealed record AuthenticationTokenResponse
{
    [JsonPropertyName("authenticationToken")] public string Token { get; init; } = string.Empty;
}
