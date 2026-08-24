using System.Text.Json.Serialization;

namespace VTubeStudio.Api.Models;

public sealed record Expression
{
    [JsonPropertyName("name")] public string Name { get; init; } = string.Empty;
    [JsonPropertyName("file")] public string File { get; init; } = string.Empty;
    [JsonPropertyName("active")] public bool Active { get; init; }
    [JsonPropertyName("deactivateWhenKeyIsLetGo")] public bool DeactivateWhenKeyIsLetGo { get; init; }
    [JsonPropertyName("secondsRemaining")] public float SecondsRemaining { get; init; }
    [JsonPropertyName("usedInHotkeys")] public IReadOnlyList<ExpressionHotkeyReference> UsedInHotkeys { get; init; } = [];
}
