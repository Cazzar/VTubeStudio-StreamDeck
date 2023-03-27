using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StreamDeckLib.Extensions.Models;
using StreamDeckLib.Json;
using StreamDeckLib.Models;

namespace StreamDeckLib
{
    class StreamDeckConnection : IStreamDeckConnection, IDisposable
    {
        private const int BufferSize = 1024 * 1024;

        private readonly ILogger<StreamDeckConnection> _logger;
        private readonly ActionRepository _actionRepository;
        private readonly SemaphoreSlim _send = new SemaphoreSlim(1);

        private readonly ClientWebSocket _socket = new();
        private readonly StreamDeckLaunchOptions _streamDeckLaunchOptions;

        private readonly CancellationTokenSource _cancelSource = new();
        private readonly IEnumerable<IGlobalSettingsHandler> _globalSettingsHandlers;
        private readonly IEnumerable<IApplicationHandler> _applicationHandlers;

        public StreamDeckConnection(
            IOptions<StreamDeckLaunchOptions> options, 
            ILogger<StreamDeckConnection> logger, 
            IEnumerable<IGlobalSettingsHandler> globalSettingsHandlers,
            IEnumerable<IApplicationHandler> applicationHandlers,
            ActionRepository actionRepository
        )
        {
            _logger = logger;
            _globalSettingsHandlers = globalSettingsHandlers;
            _applicationHandlers = applicationHandlers;
            _actionRepository = actionRepository;
            _streamDeckLaunchOptions = options.Value;
        }

        public async Task Run()
        {
            _logger.LogDebug("Connecting to localhost:{0}", _streamDeckLaunchOptions.Port);
            await _socket.ConnectAsync(new Uri($"ws://localhost:{_streamDeckLaunchOptions.Port}"), CancellationToken.None);

            while (_socket.State == WebSocketState.Connecting)
            {
                Thread.Sleep(100);
            }

            if (_socket.State != WebSocketState.Open)
            {
                _logger.LogError("Unable to connect to websocket, current state: {0}", _socket.State);
                throw new WebsocketNotConnectedException();
            }

            await SendMessage(new RegisterEvent()
            {
                Event = _streamDeckLaunchOptions.RegisterEvent,
                Uuid = _streamDeckLaunchOptions.PluginUuid,
            });

            await SendMessage(new GetGlobalSettings() { Context = _streamDeckLaunchOptions.PluginUuid});

            await ReceiveAsync();
        }

        private async Task<WebSocketCloseStatus> ReceiveAsync()
        {
            try 
            {
                var buffer = new byte[BufferSize];
                var arrayBuffer = new ArraySegment<byte>(buffer);
                var textBuffer = new StringBuilder(BufferSize);

                while (!_cancelSource.IsCancellationRequested)
                {
                    var result = await _socket.ReceiveAsync(arrayBuffer, _cancelSource.Token);

                    if (result.MessageType == WebSocketMessageType.Close || (result.CloseStatus is { } && result.CloseStatus.Value != WebSocketCloseStatus.Empty))
                    {
                        _logger.LogInformation("Received disconnect event, of type {CloseStatus} reason given? {CloseStatusDescription}", result.CloseStatus, result.CloseStatusDescription);

                        return result.CloseStatus.GetValueOrDefault();
                    }

                    if (result.MessageType != WebSocketMessageType.Text) continue;

                    textBuffer.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
                    if (!result.EndOfMessage) continue;
                
                    var message = JsonConvert.DeserializeObject<EventMessage>(textBuffer.ToString(), new StreamDeckMessageConverter());
                    _logger.LogDebug("Got message of {Event} JSON data: {ToString}", message?.Event, textBuffer.ToString());

                    switch (message)
                    {
                        case DidReceiveSettings m:
                            _actionRepository.GotSettings(m);
                            break;
                        case GlobalSettings m:
                            foreach (var globalSettingsHandler in _globalSettingsHandlers)
                                globalSettingsHandler.GotGlobalSettings(m.Payload.Settings);
                            break;
                        case KeyDown m:
                            _actionRepository.ButtonDown(m);
                            break;
                        case KeyUp m:
                            _actionRepository.ButtonUp(m);
                            break;
                        case OnWillAppear m:
                            _actionRepository.Appeared(m);
                            break;
                        case OnWillDisappear m:
                            _actionRepository.Disappeared(m);
                            break;
                        case TitleParametersDidChange m:
                            _actionRepository.TitleParamsChange(m);
                            break;
                        case DeviceDidConnect m:
                            OnDeviceConnected?.Invoke(this, new () { Event = m.Event, Payload = m, });
                            break;
                        case DeviceDidDisconnect m:
                            OnDeviceDisconnected?.Invoke(this, new () { Event = m.Event, Payload = m, });
                            break;
                        case ApplicationDidLaunch m:
                            foreach (var applicationHandler in _applicationHandlers)
                                applicationHandler.Launched(m);
                            break;
                        case ApplicationDidTerminate m:
                            foreach (var applicationHandler in _applicationHandlers)
                                applicationHandler.Terminated(m);
                            break;
                        case SystemDidWakeUp m:
                            OnSystemWakeup?.Invoke(this, new () { Event = m.Event, Payload = m, });
                            break;
                        case PropertyInspectorDidAppear m:
                            _actionRepository.PropertyInspectorAppeared(m);
                            break;
                        case PropertyInspectorDidDisappear m:
                            _actionRepository.PropertyInspectorDisappeared(m);
                            break;
                        case SendToPlugin m:
                            _actionRepository.SendToPlugin(m);
                            break;        
                    }

                    textBuffer.Clear();
                }

                return WebSocketCloseStatus.NormalClosure;
            } 
            catch (WebSocketException) 
            {
                return WebSocketCloseStatus.ProtocolError;
            } 
        }

        public async Task Cancel()
        {
            if (_socket.State == WebSocketState.Open)
                await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
        }

        public async Task SendMessage(EventMessage message)
        {
            if (_socket.State != WebSocketState.Open) throw new WebsocketNotConnectedException();

            try
            {
                await _send.WaitAsync();
                var json = JsonConvert.SerializeObject(message, new JsonSerializerSettings() { ContractResolver = new DefaultContractResolver() { NamingStrategy = new CamelCaseNamingStrategy() }, });
                _logger.LogDebug("Sending message of {Type} with JSON data {Json}", message.Event, json);

                var buf = Encoding.UTF8.GetBytes(json);
                await _socket.SendAsync(new ArraySegment<byte>(buf), WebSocketMessageType.Text, true, _cancelSource.Token);
            }
            finally
            {
                _send.Release();
            }
        }

        public async void OpenUrl(Uri url)
        {
            await SendMessage(new OpenUrl { Payload = new(url.ToString()) });
        }

        public void OpenUrl(string url)
        {
            OpenUrl(new Uri(url));
        }

        public event EventHandler<StreamDeckEventArgs<DeviceDidConnect>>? OnDeviceConnected;
        public event EventHandler<StreamDeckEventArgs<DeviceDidDisconnect>>? OnDeviceDisconnected;
        public event EventHandler<StreamDeckEventArgs<SystemDidWakeUp>>? OnSystemWakeup;

        public void Dispose()
        {
            _send.Dispose();
            _socket.Dispose();
        }
    }
}
