using System.Text.Json.Serialization;

namespace VTubeStudio.Api.Requests;

public sealed record MoveModelRequest : IVtsRequest
{
    // VTube Studio rejects the request outright if this is absent, so it is written even when null.
    [JsonPropertyName("timeInSeconds")]
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public double? TimeInSeconds { get; init; }
    [JsonPropertyName("valuesAreRelativeToModel")] public bool? RelativeMove { get; init; }
    [JsonPropertyName("positionX")] public double? PositionX { get; init; }
    [JsonPropertyName("positionY")] public double? PositionY { get; init; }
    [JsonPropertyName("rotation")] public double? Rotation { get; init; }
    [JsonPropertyName("size")] public double? Size { get; init; }

    public string MessageType => "MoveModelRequest";
}
