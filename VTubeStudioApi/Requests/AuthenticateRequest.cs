using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Responses;
using Newtonsoft.Json;

namespace Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Requests
{
    [AuthLess]
    class AuthenticateRequest : ApiRequest
    {
        [JsonProperty("pluginName")] public string Name { get; } = "StreamDeck Integration";
        [JsonProperty("pluginDeveloper")] public string Developer { get; } = "Cazzar";
        [JsonProperty("pluginIcon")] public string Icon { get; } = null;

        [JsonIgnore]
        public override RequestType MessageType { get; } = RequestType.AuthenticationTokenRequest;
    }
}