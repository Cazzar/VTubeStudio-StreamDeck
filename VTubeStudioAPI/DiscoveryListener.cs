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

namespace Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi;

public class DiscoveryListener
{
    private readonly UdpClient _udpClient;
    private IPEndPoint _endPoint;
    public static DiscoveryListener Instance { get; } = new();

    public readonly Dictionary<string, VTubeStudioServer> Servers = new();

    public DiscoveryListener()
    {
        _udpClient = new(47779);
        _udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        _endPoint = new(IPAddress.Any, 47779);
        
        Task.Factory.StartNew(RunLoop);
    }

    private void RunLoop()
    {
        try
        {
            while (true)
            {
                var data = _udpClient.Receive(ref _endPoint);
                
                try
                {
                    var broadcast = JsonConvert.DeserializeObject<ApiResponse<VTubeStudioServer>>(Encoding.Default.GetString(data));
                    if (broadcast is null) continue;
                    var child = broadcast.Data with {IpAddress = _endPoint.Address.ToString()};
                    Servers.Remove(child.InstanceId);

                    Servers.Add(child.InstanceId, child);
                        
                    ServersUpdated?.Invoke(this, new()
                    {
                        Servers = new ReadOnlyDictionary<string, VTubeStudioServer>(Servers),
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

    public static event EventHandler<ServerEventArgs>? ServersUpdated;
        
    public class ServerEventArgs : EventArgs
    {
        public required IDictionary<string, VTubeStudioServer> Servers { get; set; }
    }
}