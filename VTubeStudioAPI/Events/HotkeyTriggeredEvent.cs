using Newtonsoft.Json;

namespace Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Events;

public class HotkeyTriggeredEvent
{
    [JsonProperty("hotkeyID")]
    public string HotkeyId { get; set; } = string.Empty;

    [JsonProperty("hotkeyName")]
    public string HotkeyName { get; set; } = string.Empty;

    [JsonProperty("hotkeyType")]
    public string HotkeyType { get; set; } = string.Empty;
}
