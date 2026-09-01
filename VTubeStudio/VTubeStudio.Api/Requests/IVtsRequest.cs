using System.Text.Json.Serialization;

namespace VTubeStudio.Api.Requests;

public interface IVtsRequest
{
    [JsonIgnore]
    string MessageType { get; }
}
