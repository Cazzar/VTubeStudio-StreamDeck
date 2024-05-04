using Newtonsoft.Json;

namespace Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Responses;

public class AuthenticateResponse
{
    [JsonProperty("authenticationToken")]
    public required string AuthToken { get; set; }
}