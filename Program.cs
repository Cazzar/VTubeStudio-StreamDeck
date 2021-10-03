using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using BarRaider.SdTools;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Requests;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Responses;
using WebSocketSharp;

namespace Cazzar.StreamDeck.VTubeStudio
{
    class Program
    {
        static void Main(string[] args)
        {
            // Test().GetAwaiter().GetResult();
            // DiscoveryListener.ServersUpdated += (sender, eventArgs) =>
            // {
            //     foreach (var (id, server) in eventArgs.Servers)
            //     {
            //         Console.WriteLine($"Found server: {id} at {server.IpAddress}:{server.Port} window name: {server.WindowTitle} ({server.IsActive})");
            //     }
            // };
            // var foo = DiscoveryListener.Instance.GetType();
            // while (!System.Diagnostics.Debugger.IsAttached) { System.Threading.Thread.Sleep(100); }
            
            AuthManager.RegisterEvents();
            StateManager.RegisterEvents();
            
            SDWrapper.Run(args);
        }
    }
}
