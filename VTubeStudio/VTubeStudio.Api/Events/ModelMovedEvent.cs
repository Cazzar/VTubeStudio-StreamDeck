using System.Text.Json.Serialization;
using VTubeStudio.Api.Models;

namespace VTubeStudio.Api.Events;

public sealed record ModelMovedEvent
{
    [JsonPropertyName("modelID")] public string ModelId { get; init; } = string.Empty;
    [JsonPropertyName("modelPosition")] public ModelPosition Position { get; init; } = new();
}
