using System.Collections.Generic;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Models;
using Newtonsoft.Json;

namespace Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Responses
{
    public class AvailableModelsResponse
    {
        [JsonProperty("numberOfModels")]
        public int Count { get; set; }

        [JsonProperty("availableModels")]
        public List<Model> Models { get; set; }
    }
}
