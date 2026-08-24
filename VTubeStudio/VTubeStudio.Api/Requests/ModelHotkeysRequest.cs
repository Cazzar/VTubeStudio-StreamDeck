using System.Text.Json.Serialization;

namespace VTubeStudio.Api.Requests;

public sealed record ModelHotkeysRequest(
    [property: JsonPropertyName("modelID")] string? ModelId = null) : IVtsRequest
{
    public string MessageType => "HotkeysInCurrentModelRequest";
}
