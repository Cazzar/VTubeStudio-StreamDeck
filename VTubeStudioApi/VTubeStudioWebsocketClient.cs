using System;
using System.Data;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using BarRaider.SdTools;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Requests;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Responses;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebSocketSharp;
using Logger = BarRaider.SdTools.Logger;

namespace Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi
{
    public class VTubeStudioWebsocketClient
    {
        public static VTubeStudioWebsocketClient Instance { get; } = new();

        public bool IsAuthed => _authed && _ws.IsAlive;
        public bool WsIsAlive => _ws.IsAlive;

        private bool _tryingToConnect = false;
        private WebSocket _ws = null;
        private bool _authed = false;

        static VTubeStudioWebsocketClient()
        {
            EventRegistrar.CallAll();
        }

        private VTubeStudioWebsocketClient()
        {
        }

        public void Send(ApiRequest request)
        {
            var data = JsonConvert.SerializeObject(new RequestWrapper(request));
            // Logger.Instance.LogMessage(TracingLevel.INFO, $">>> {data}");
            _ws.Send(data);
        }


        public void ConnectIfNeeded()
        {
            if (_ws is not null && _ws.IsAlive) return;
            if (_tryingToConnect) return;

            _tryingToConnect = true;
            Task.Run(
                () =>
                {
                    Connect();
                    _tryingToConnect = false;
                }
            );
        }

        private void Connect()
        {
            Logger.Instance.LogMessage(TracingLevel.INFO, "Connecting to websocket");
            _authed = false;
            _ws = new("ws://localhost:8001");
            SetupEvents();
            _ws.Connect();
        }

        private void SetupEvents()
        {
            _ws.OnOpen += (sender, args) => SocketConnected?.Invoke(this, null);
            _ws.OnMessage += MessageReceived;
            _ws.OnClose += (sender, args) => SocketClosed?.Invoke(this, args);
        }

        private void MessageReceived(object sender, MessageEventArgs e)
        {
            if (!e.IsText) return; //Don't handle binary.
            // Logger.Instance.LogMessage(TracingLevel.INFO, $"<<< {e.Data}");

            var response = JsonConvert.DeserializeObject<ApiResponse>(e.Data);

            switch (response.MessageType)
            {
                case ResponseType.APIError:
                    OnApiError?.Invoke(this, new (response.Data.ToObject<ApiError>()));
                    break;
                case ResponseType.APIStateResponse:
                    var data = response.Data.ToObject<ApiStateResponse>();
                    _authed = data?.Authenticated ?? false;
                    OnApiState?.Invoke(this, new (data));
                    break;
                case ResponseType.AuthenticationTokenResponse:
                    OnTokenResponse?.Invoke(this, new (response.Data.ToObject<AuthenticateResponse>()));
                    break;
                case ResponseType.AuthenticationResponse:
                    var d = response.Data.ToObject<AuthenticationResponse>();
                    _authed = d?.Authenticated ?? false;
                    OnAuthenticationResponse?.Invoke(this, new (d));
                    break;
                case ResponseType.StatisticsResponse: break;
                case ResponseType.VTSFolderInfoResponse: break;
                case ResponseType.CurrentModelResponse:
                    OnCurrentModelInformation?.Invoke(this, new (response.Data.ToObject<CurrentModelResponse>()));
                    break;
                case ResponseType.AvailableModelsResponse:
                    OnAvailableModels?.Invoke(this, new (response.Data.ToObject<AvailableModelsResponse>()));
                    break;
                case ResponseType.ModelLoadResponse:
                    OnModelLoad?.Invoke(this, new(response.Data.ToObject<ModelLoadResponse>()));
                    break;
                case ResponseType.MoveModelResponse: break;
                case ResponseType.HotkeysInCurrentModelResponse:
                    OnModelHotkeys?.Invoke(this, new (response.Data.ToObject<ModelHotkeysResponse>()));
                    break;
                case ResponseType.HotkeyTriggerResponse:
                    OnHotkeyTriggered?.Invoke(this, new (response.Data.ToObject<HotkeyTriggerResponse>()));
                    break;
                case ResponseType.ArtMeshListResponse: break;
                case ResponseType.ColorTintResponse: break;
                case ResponseType.FaceFoundResponse: break;
                case ResponseType.InputParameterListResponse: break;
                case ResponseType.ParameterValueResponse: break;
                case ResponseType.Live2DParameterListResponse: break;
                case ResponseType.ParameterCreationResponse: break;
                case ResponseType.ParameterDeletionResponse: break;
                case ResponseType.InjectParameterDataResponse: break;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        private void Disconnect()
        {
            if (_ws == null) return;

            try
            {
                _ws.Close();
                _ws = null;
            }
            catch
            {
                // ignored
            }
        }

        public void Reconnect()
        {
            Disconnect();
            Connect();
        }

#region Events

        public static event EventHandler<EventArgs> SocketConnected;
        public static event EventHandler<CloseEventArgs> SocketClosed;
        public static event EventHandler<ApiEventArgs<ApiError>> OnApiError;
        public static event EventHandler<ApiEventArgs<ApiStateResponse>> OnApiState;
        public static event EventHandler<ApiEventArgs<AuthenticateResponse>> OnTokenResponse;
        public static event EventHandler<ApiEventArgs<AuthenticationResponse>> OnAuthenticationResponse;
        public static event EventHandler<ApiEventArgs<AvailableModelsResponse>> OnAvailableModels;
        public static event EventHandler<ApiEventArgs<ModelLoadResponse>> OnModelLoad;
        public static event EventHandler<ApiEventArgs<ModelHotkeysResponse>> OnModelHotkeys;
        public static event EventHandler<ApiEventArgs<HotkeyTriggerResponse>> OnHotkeyTriggered;
        public static event EventHandler<ApiEventArgs<CurrentModelResponse>> OnCurrentModelInformation;

#endregion
    }
}
