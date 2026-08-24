using System.Text.Json.Serialization;

namespace Cazzar.Deck.VTubeStudio.Actions.Movement;

public sealed class MoveSettings
{
    [JsonPropertyName("seconds")] public double? Seconds { get; set; } = 0;
    [JsonPropertyName("posX")] public double? PosX { get; set; } = 0;
    [JsonPropertyName("posY")] public double? PosY { get; set; } = 0;
    [JsonPropertyName("rotation")] public double? Rotation { get; set; } = 0;
    [JsonPropertyName("size")] public double? Size { get; set; }
    [JsonPropertyName("relative")] public bool Relative { get; set; } = true;
}
