using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Responses;
using Newtonsoft.Json;

namespace Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Requests;

public class ExpressionActivationRequest : ApiRequest
{
    [JsonProperty("expressionFile")]
    public string ExpressionFile { get; set; }

    [JsonProperty("active")]
    public bool Active { get; set; }

    [JsonProperty("fadeTime", NullValueHandling = NullValueHandling.Ignore)]
    public float? FadeTime { get; set; }

    public ExpressionActivationRequest(string expressionFile, bool active, float? fadeTime = null)
    {
        ExpressionFile = expressionFile;
        Active = active;
        FadeTime = fadeTime;
    }

    public override RequestType MessageType { get; } = RequestType.ExpressionActivationRequest;
}
