using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace VTubeStudio.Api.Requests;

public sealed record EventSubscriptionRequest(
    [property: JsonPropertyName("eventName")] string EventName,
    [property: JsonPropertyName("subscribe")] bool Subscribe = true) : IVtsRequest
{
    [JsonPropertyName("config")] public JsonObject Config { get; init; } = new();

    public string MessageType => "EventSubscriptionRequest";
}
