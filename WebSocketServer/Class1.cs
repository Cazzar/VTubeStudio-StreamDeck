using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace WebSocketServer
{
    public class WebSocketServer : IHostedService
    {
        public Task StartAsync(CancellationToken cancellationToken)
        {
            var ws = new System.Net.WebSockets.();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            
        }
    }
}
