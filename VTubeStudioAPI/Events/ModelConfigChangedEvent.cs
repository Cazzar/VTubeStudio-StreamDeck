using Newtonsoft.Json;

namespace Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Events;

public class ModelConfigChangedEvent
{
    [JsonProperty("modelID")]
    public string ModelId { get; set; }
    
    [JsonProperty("modelName")]
    public string Name { get; set; }
    
    [JsonProperty("hotkeyConfigChanged")]
    public bool HotkeyConfigChanged { get; set; }
}
