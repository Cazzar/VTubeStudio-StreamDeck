using System.Text.Json.Serialization;

namespace VTubeStudio.Api.Requests;

public sealed record HotkeyTriggerRequest(
    [property: JsonPropertyName("hotkeyID")] string HotkeyId) : IVtsRequest
{
    public string MessageType => "HotkeyTriggerRequest";
}
