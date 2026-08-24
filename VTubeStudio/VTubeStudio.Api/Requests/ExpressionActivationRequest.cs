using System.Text.Json.Serialization;

namespace VTubeStudio.Api.Requests;

public sealed record ExpressionActivationRequest(
    [property: JsonPropertyName("expressionFile")] string ExpressionFile,
    [property: JsonPropertyName("active")] bool Active,
    [property: JsonPropertyName("fadeTime")] float? FadeTime = null) : IVtsRequest
{
    public string MessageType => "ExpressionActivationRequest";
}
