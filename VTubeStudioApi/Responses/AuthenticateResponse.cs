using Newtonsoft.Json;

namespace Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Responses
{
    public class AuthenticateResponse
    {
        [JsonProperty("authenticationToken")]
        public string AuthToken { get; set; }
    }
}