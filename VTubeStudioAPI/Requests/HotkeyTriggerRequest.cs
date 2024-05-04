using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Models;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Responses;
using Newtonsoft.Json;

namespace Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Requests;

public class HotkeyTriggerRequest : ApiRequest
{
    [JsonProperty("hotkeyID")]
    public string Id { get; set; }

    public HotkeyTriggerRequest(string hotkeyId)
    {
        Id = hotkeyId;
     }

    public HotkeyTriggerRequest(Hotkey hotkey) : this(hotkey.Id)
    {}

    public override RequestType MessageType { get; } = RequestType.HotkeyTriggerRequest;
}
    
public class EventSubscriptionRequest<T>(string eventName, T? config = default, bool subscribe = true) : ApiRequest
{
    [JsonProperty("eventName")]
    public string Event { get; set; } = eventName;
        
    [JsonProperty("subscribe")]
    public bool Subscribe { get; set; } = subscribe;
        
    [JsonProperty("config")]
    public T? Config { get; set; } = config;
    public override RequestType MessageType { get; } = RequestType.EventSubscriptionRequest;
}