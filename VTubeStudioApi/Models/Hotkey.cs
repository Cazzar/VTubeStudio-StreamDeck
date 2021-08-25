using Newtonsoft.Json;

namespace Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Models
{
    public class Hotkey
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("file")]
        public string File { get; set; }
        [JsonProperty("hotkeyID")]
        public string Id { get; set; }
    }
}
