using System.Net;
using System.Runtime.Serialization;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Responses;

namespace Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi
{
    public class VTubeStudioServer : StateBroadcast
    {
        public VTubeStudioServer(StateBroadcast child, IPEndPoint endPoint)
        {
            IpAddress = endPoint.Address.ToString();
            IsActive = child.IsActive;
            Port = child.Port;
            InstanceId = child.InstanceId;
            WindowTitle = child.WindowTitle;
        }
        
        [DataMember(Name = "ip")]
        public string IpAddress { get; set; }
    }
}
