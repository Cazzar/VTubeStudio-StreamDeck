using Newtonsoft.Json;

namespace Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Events;

public class ModelConfigModifiedEvent
{
    [JsonProperty("modelID")]
    public string Id { get; set; }
    
    [JsonProperty("modelName")]
    public string Name { get; set; }
    
    [JsonProperty("hotkeyConfigChanged")]
    public bool HotkeyConfigChanged { get; set; }
}
