using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Responses;
using Newtonsoft.Json;
using NLog.Layouts;

namespace Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi
{
    public class DiscoveryListener
    {
        private UdpClient _udpClient;
        private IPEndPoint _endPoint;
        public static DiscoveryListener Instance { get; } = new();

        public Dictionary<string, VTubeStudioServer> _servers = new();

        public DiscoveryListener()
        {
            _udpClient = new(47779);
            _endPoint = new(IPAddress.Any, 47779);


            Task.Factory.StartNew(RunLoop);
        }

        private void RunLoop()
        {
            try
            {
                while (true)
                {
                    byte[] data = _udpClient.Receive(ref _endPoint);
                    try
                    {
                        var bcast = JsonConvert.DeserializeObject<ApiResponse<StateBroadcast>>(Encoding.Default.GetString(data));
                        var child = new VTubeStudioServer(bcast.Data, _endPoint);
                        if (_servers.ContainsKey(child.InstanceId)) _servers.Remove(child.InstanceId);

                        _servers.Add(child.InstanceId, child);
                        
                        ServersUpdated?.Invoke(this, new ServerEventArgs()
                        {
                            Servers = new ReadOnlyDictionary<string, VTubeStudioServer>(_servers),
                        });
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
            }
            catch (SocketException)
            {
                //ignored
            }
            finally
            {
                _udpClient.Close();
            }
#pragma warning disable 8763
        }
#pragma warning restore 8763

        public static event EventHandler<ServerEventArgs> ServersUpdated;
        
        public class ServerEventArgs : EventArgs
        {
            public IDictionary<string, VTubeStudioServer> Servers { get; set; }
        }
    }
}
