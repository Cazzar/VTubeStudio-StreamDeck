using System;
using System.IO;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using BarRaider.SdTools;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Requests;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Timer = System.Timers.Timer;

namespace Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi
{
    [Obsolete("Use new client", error:true)]
    public class ApiClient : IAsyncDisposable
    {
        public static ApiClient Instance { get; } = new ApiClient();

        public bool IsAuthed
        {
            get => _isAuthed && _webSocket.State == WebSocketState.Open;
            private set => _isAuthed = value;
        }

        public EventHandler<AuthStateChangedEvent> AuthStateChanged;

        private ClientWebSocket _webSocket;

        private readonly SemaphoreSlim _messageLock = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim _initLock = new SemaphoreSlim(1, 1);
        private bool _isAuthed = false;

        private ApiClient()
        {
            Timer t = new Timer(TimeSpan.FromSeconds(5).TotalMilliseconds);
            t.Elapsed += KeepAliveTimer;
            t.Start();
            
            Connect().GetAwaiter().GetResult();
            
            GlobalSettingsManager.Instance.TokenChanged += async (sender, settings) => await Authenticate();
        }

        private async Task Connect()
        {
            try
            {
                IsAuthed = false;
                _webSocket = new ClientWebSocket(); //fuck it, just create a new connection, old one will get GC'd
                await _webSocket.ConnectAsync(new Uri("ws://localhost:8001"), CancellationToken.None);
            }
            catch (Exception ex) when (ex is SocketException || ex is WebSocketException)
            {
            }
        }

        private async Task<string> Authenticate()
        {
            if (IsAuthed) return "Already authenticated";

            string token = GlobalSettingsManager.Instance.Settings.Token;

            bool saveSettings = false;
            if (String.IsNullOrEmpty(token))
            {
                token = (await SendMessage(new AuthenticateRequest())).AuthToken;
                GlobalSettingsManager.Instance.Settings.Token = token;
                saveSettings = true;
            }

            var response = await SendMessage(new AuthWithTokenRequest(token));

            IsAuthed = response.Authenticated;
            Logger.Instance.LogMessage(TracingLevel.INFO, $"Authenticated? {response.Authenticated} reason: {response.Reason}");
            if (!IsAuthed)
            {
                GlobalSettingsManager.Instance.Settings.Token = null;
                saveSettings = true; //magic will force a re-auth
            }

            if (saveSettings) GlobalSettingsManager.Instance.SaveSettings();
            
            AuthStateChanged?.Invoke(this, new AuthStateChangedEvent()
            {
                IsAuthed = response.Authenticated,
                Reason = response.Reason,
            });

            return response.Reason;
        }

        public async Task<TOut> SendMessage<TOut>(ApiRequest<TOut> message)
        {
            if (_webSocket.State != WebSocketState.Open)
            {
                await Connect();
                if (string.IsNullOrEmpty(GlobalSettingsManager.Instance.Settings?.Token))
                    await Authenticate();
            }

            if (!IsAuthed && !IsAuthLess(message)) TryToReAuth();
            await _messageLock.WaitAsync();

            try
            {
                ReadOnlyMemory<byte> rom = new ReadOnlyMemory<byte>(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new RequestWrapper<TOut>(message))));
                await _webSocket.SendAsync(rom, WebSocketMessageType.Text, true, CancellationToken.None);

                StringBuilder data = new StringBuilder();
                WebSocketReceiveResult result;
                do
                {
                    var recv = new byte[1024];
                    ArraySegment<byte> response = new ArraySegment<byte>(recv);

                    result = await _webSocket.ReceiveAsync(response, CancellationToken.None);
                    data.Append(Encoding.UTF8.GetString(recv[..result.Count]));
                } while (!result.EndOfMessage);

                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<TOut>>(data.ToString());

                if (apiResponse.MessageType == "APIError")
                {
                    throw new ArgumentException(data.ToString());
                }

                return apiResponse.Data;
            }
            catch (IOException)
            {
                await Connect();
                return await SendMessage(message);
            }
            catch (WebSocketException)
            {
                await Connect();
                return await SendMessage(message);
            }
            finally
            {
                _messageLock.Release();
            }
        }

        private async void TryToReAuth()
        {
            // throw new NotAuthenticatedException();
            if (_webSocket.State != WebSocketState.Open)
            {
                await Connect();
            }

            await Authenticate();
        }

        private static bool IsAuthLess<TOut>(ApiRequest<TOut> message) => message.GetType().GetCustomAttribute<AuthLess>() != null;

        public ValueTask DisposeAsync()
        {
            _webSocket.Dispose();

            return ValueTask.CompletedTask;
        }

        private async void KeepAliveTimer(object sender, ElapsedEventArgs e)
        {
            var response = await SendMessage(new ApiStateRequest());

            IsAuthed = response.Authenticated;
        }

        public async void Reconnect()
        {
            await Connect();
        }
    }

    public class AuthStateChangedEvent
    {
        public bool IsAuthed { get; set; }
        public string Reason { get; set; }
    }
}