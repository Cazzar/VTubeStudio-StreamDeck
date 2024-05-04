using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Responses;
using Newtonsoft.Json;

namespace Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Events;

public class ModelMoveEvent
{
    [JsonProperty("modelID")]
    public required string Id { get; set; }
    
    [JsonProperty("modelName")]
    public required string Name { get; set; }
    
    [JsonProperty("modelPosition")]
    public required ModelPosition Position { get; set; }
}