using Newtonsoft.Json;

namespace Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Events;

public class ModelLoadedEvent
{
    [JsonProperty("modelLoaded")]
    public bool ModelLoaded { get; set; }

    [JsonProperty("modelName")]
    public string ModelName { get; set; } = string.Empty;

    [JsonProperty("modelID")]
    public string ModelId { get; set; } = string.Empty;
}
