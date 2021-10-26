using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Models;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Responses;
using Newtonsoft.Json;

namespace Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Requests
{
    public class HotkeyTriggerRequest : ApiRequest
    {
        [JsonProperty("hotkeyID")]
        public string Id { get; set; }

        public HotkeyTriggerRequest(string hotkeyId)
        {
            this.Id = hotkeyId;
        }

        public HotkeyTriggerRequest(Hotkey hotkey) : this(hotkey.Id)
        {
            
        }

        public override RequestType MessageType { get; } = RequestType.HotkeyTriggerRequest;
    }
}
