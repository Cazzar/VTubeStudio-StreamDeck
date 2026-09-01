using System.Text.Json.Serialization;

namespace VTubeStudio.Api.Requests;

public sealed record ModelLoadRequest(
    [property: JsonPropertyName("modelID")] string ModelId) : IVtsRequest
{
    public string MessageType => "ModelLoadRequest";
}
