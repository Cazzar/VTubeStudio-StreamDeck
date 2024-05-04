using System.Collections.Generic;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Models;
using Newtonsoft.Json;

namespace Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Responses;

public class ModelHotkeysResponse
{
    [JsonProperty("modelLoaded")]
    public bool ModelLoaded { get; set; }
    
    [JsonProperty("modelName")]
    public required string ModelName { get; set; }
    
    [JsonProperty("ModelId")]
    public required string ModelId { get; set; }
    
    [JsonProperty("availableHotkeys")]
    public required List<Hotkey> Hotkeys { get; set; }
}