using System.Text.Json.Serialization;

namespace VTubeStudio.Api.Requests;

public sealed record ExpressionStateRequest(
    [property: JsonPropertyName("details")] bool Details = true) : IVtsRequest
{
    public string MessageType => "ExpressionStateRequest";
}
