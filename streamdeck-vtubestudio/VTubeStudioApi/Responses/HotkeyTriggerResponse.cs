using Newtonsoft.Json;

namespace Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Responses
{
    public class HotkeyTriggerResponse
    {
        [JsonProperty("hotkeyID")]
        public string Id { get; set; }
    }
}
