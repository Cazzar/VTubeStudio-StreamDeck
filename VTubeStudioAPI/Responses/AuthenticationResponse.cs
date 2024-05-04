using Newtonsoft.Json;

namespace Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Responses;

public class AuthenticationResponse
{
    [JsonProperty("authenticated")] public bool Authenticated { get; set; }
    [JsonProperty("reason")] public string? Reason { get; set; }
}