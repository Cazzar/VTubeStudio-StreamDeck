using System.Text.Json.Serialization;

namespace VTubeStudio.Api.Models;

public sealed record ExpressionHotkeyReference
{
    [JsonPropertyName("name")] public string Name { get; init; } = string.Empty;
    [JsonPropertyName("id")] public string Id { get; init; } = string.Empty;
}
