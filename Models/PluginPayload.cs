using Newtonsoft.Json;

namespace Cazzar.StreamDeck.VTubeStudio.Models
{
    internal class PluginPayload
    {
        [JsonProperty("command")]
        public string Command { get; set; }
            
        [JsonProperty("payload")]
        public dynamic Payload { get; set; }
    }
}
