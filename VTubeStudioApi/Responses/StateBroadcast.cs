using System.Runtime.Serialization;

namespace Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Responses
{
    public class StateBroadcast
    {
        [DataMember(Name = "active")]
        public bool IsActive { get; set; }

        [DataMember(Name = "port")]
        public ushort Port { get; set; }
        
        [DataMember(Name = "instanceId")]
        public string InstanceId { get; set; }
        
        [DataMember(Name = "windowTitle")]
        public string WindowTitle { get; set; }
    }
}
