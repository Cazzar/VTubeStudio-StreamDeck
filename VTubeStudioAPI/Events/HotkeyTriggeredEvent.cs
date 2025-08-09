using Newtonsoft.Json;

namespace Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Events;

public class HotkeyTriggeredEvent
{
    [JsonProperty("hotkeyID")]
    public required string Id { get; set; }
    
    [JsonProperty("hotkeyName")]
    public required string Name { get; set; }
    
    [JsonProperty("hotkeyAction")]
    public required string Action { get; set; }
    
    [JsonProperty("hotkeyFile")]
    public required string File { get; set; }
    
    [JsonProperty("hotkeyTriggeredByAPI")]
    public bool ApiTriggered { get; set; }
    
    [JsonProperty("modelID")]
    public required string ModelId { get; set; }
    
    [JsonProperty("modelName")]
    public required string ModelName { get; set; }
    
    [JsonProperty("isLive2DItem")]
    public bool IsLive2DItem { get; set; }
}
