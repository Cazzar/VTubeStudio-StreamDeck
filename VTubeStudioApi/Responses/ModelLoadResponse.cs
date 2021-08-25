using Newtonsoft.Json;

namespace Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Responses
{
    public class ModelLoadResponse
    {
        [JsonProperty("modelID")]
        public string Id { get; set; }
    }
}
