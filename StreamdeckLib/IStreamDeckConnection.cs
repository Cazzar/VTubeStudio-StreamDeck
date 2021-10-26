using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using StreamDeckLib.Extensions;
using StreamDeckLib.Models;

namespace StreamDeckLib
{
    public interface IStreamDeckConnection
    {
        Task Run();
        Task Cancel();
        Task SendMessage(EventMessage message);

        void OpenUrl(Uri url);
        void OpenUrl(string url);

        public event EventHandler<StreamDeckEventArgs<DeviceDidConnect>> OnDeviceConnected;
        public event EventHandler<StreamDeckEventArgs<DeviceDidDisconnect>>? OnDeviceDisconnected;
        public event EventHandler<StreamDeckEventArgs<SystemDidWakeUp>>? OnSystemWakeup;
    }
}
