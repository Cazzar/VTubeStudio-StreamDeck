using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Responses;

public record StateBroadcast
{
    [JsonProperty("active")]
    public bool IsActive { get; set; }

    [JsonProperty("port")]
    public ushort Port { get; set; }
        
    [JsonProperty("instanceId")]
    public required string InstanceId { get; set; }
        
    [JsonProperty("windowTitle")]
    public required string WindowTitle { get; set; }
}