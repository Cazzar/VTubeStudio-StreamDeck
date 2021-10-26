using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Responses
{
    public class StateBroadcast
    {
        [JsonProperty("active")]
        public bool IsActive { get; set; }

        [JsonProperty("port")]
        public ushort Port { get; set; }
        
        [JsonProperty("instanceId")]
        public string InstanceId { get; set; }
        
        [JsonProperty("windowTitle")]
        public string WindowTitle { get; set; }
    }
}
