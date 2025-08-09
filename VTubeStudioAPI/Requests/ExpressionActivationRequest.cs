using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Responses;
using Newtonsoft.Json;

namespace Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Requests;

public class ExpressionActivationRequest(string? expressionFile = null, bool activate = true, float fadeTime = 0.5f) : ApiRequest
{
    [JsonProperty("expressionFile")]
    public string? ExpressionFile { get; set; } = expressionFile;
    [JsonProperty("active")]
    public bool? Activate { get; set; } = activate;
    [JsonProperty("fadeTime")]
    public float FadeTime { get; set; } = fadeTime;

    public override RequestType MessageType { get; } = RequestType.ExpressionActivationRequest;
}
