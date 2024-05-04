using Newtonsoft.Json;

namespace Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Responses;

public class ModelLoadResponse
{
    [JsonProperty("modelID")]
    public required string Id { get; set; }
}