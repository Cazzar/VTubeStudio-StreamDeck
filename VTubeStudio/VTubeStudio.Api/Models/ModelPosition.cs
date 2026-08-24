using System.Text.Json.Serialization;

namespace VTubeStudio.Api.Models;

public sealed record ModelPosition
{
    [JsonPropertyName("positionX")] public double X { get; init; }
    [JsonPropertyName("positionY")] public double Y { get; init; }
    [JsonPropertyName("rotation")] public double Rotation { get; init; }
    [JsonPropertyName("size")] public double Size { get; init; }
}
