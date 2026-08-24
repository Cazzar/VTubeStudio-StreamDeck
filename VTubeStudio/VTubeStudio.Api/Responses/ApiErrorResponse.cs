using System.Text.Json.Serialization;

namespace VTubeStudio.Api.Responses;

public sealed record ApiErrorResponse
{
    [JsonPropertyName("errorID")] public int ErrorId { get; init; }
    [JsonPropertyName("message")] public string Message { get; init; } = string.Empty;
}
