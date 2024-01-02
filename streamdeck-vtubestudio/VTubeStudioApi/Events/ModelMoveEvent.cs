using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Responses;
using Newtonsoft.Json;

namespace Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Events;

public class ModelMoveEvent
{
    [JsonProperty("modelID")]
    public string Id { get; set; }
    
    [JsonProperty("modelName")]
    public string Name { get; set; }
    
    [JsonProperty("modelPosition")]
    public ModelPosition Position { get; set; }
}