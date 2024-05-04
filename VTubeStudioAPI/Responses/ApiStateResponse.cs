using Newtonsoft.Json;

namespace Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Responses;

public class ApiStateResponse
{
    [JsonProperty("active")]
    public bool Active { get; set; }
    
    [JsonProperty("vTubeStudioVersion")]
    public required string Version { get; set; }
    
    [JsonProperty("currentSessionAuthenticated")]
    public bool Authenticated { get; set; }
}