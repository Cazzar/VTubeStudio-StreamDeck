using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Responses;
using Newtonsoft.Json;

namespace Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Requests;

[AuthLess]
public class AuthWithTokenRequest : ApiRequest
{
    [JsonProperty("pluginName")] 
    public string Name { get; } = "StreamDeck Integration";
    [JsonProperty("pluginDeveloper")] 
    public string Developer { get; } = "Cazzar";

    [JsonProperty("authenticationToken")]
    public string Token { get; set; }

    public AuthWithTokenRequest(string token)
    {
        Token = token;
    }
        
    public override RequestType MessageType { get; } = RequestType.AuthenticationRequest;
}