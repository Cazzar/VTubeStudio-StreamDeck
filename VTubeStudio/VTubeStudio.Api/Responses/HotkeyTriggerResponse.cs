using System.Text.Json.Serialization;

namespace VTubeStudio.Api.Responses;

public sealed record HotkeyTriggerResponse
{
    [JsonPropertyName("hotkeyID")] public string Id { get; init; } = string.Empty;
}
