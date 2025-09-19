using System.Diagnostics;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Events;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Requests;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Responses;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using VTubeStudioAPI.Responses;
using WebSocketSharp;

namespace Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi;

public class VTubeStudioWebsocketClient
{
    private readonly IAuthManger _authManager;
    private readonly ILogger<VTubeStudioWebsocketClient> _logger;
    public bool IsAuthed => _authed && WsIsAlive;
    public bool WsIsAlive => _ws?.IsAlive ?? false;

    private bool _tryingToConnect;
    private WebSocket? _ws;
    private bool _authed;

    private string? _host;
    private ushort? _port;
    
    public VTubeStudioWebsocketClient(IAuthManger authManager, ILogger<VTubeStudioWebsocketClient> logger)
    {
        _authManager = authManager;
        _logger = logger;

        OnAuthenticationResponse += HandleAuthResponse;
    }

    ~VTubeStudioWebsocketClient()
    {
        OnAuthenticationResponse -= HandleAuthResponse;
    }

    private void HandleAuthResponse(object? sender, ApiEventArgs<AuthenticationResponse> args)
    {
        if (!args.Response.Authenticated)
            return;

        Send(new EventSubscriptionRequest<object>("ModelMovedEvent"));
        Send(new EventSubscriptionRequest<object>("ModelConfigChangedEvent"));
        Send(new EventSubscriptionRequest<object>("HotkeyTriggeredEvent"));
    }

    public void Send(ApiRequest request, string? requestId = null)
    {
        if (_ws is null)
        {
            _logger.LogDebug("WebSocket is null, not sending message");
            return;
        }
        
        if (!_ws.IsAlive)
        {
            _logger.LogDebug("WebSocket is not alive, not sending message");
            return;
        }
        
        var data = JsonConvert.SerializeObject(new RequestWrapper(request) { RequestId = requestId });
        _logger.LogDebug("Sending message: {Data}", data);
        _ws.Send(data);
    }

    public void SetConnection(string host, ushort port)
    {
        if ((host, port) == (_host, _port)) return;

        _logger.LogInformation("New connection details, set to: ws://{IpAddress}:{Port}", host, port);

        _host = host;
        _port = port;
        Connect();
    }

    public void ConnectIfNeeded()
    {
        //_logger.LogDebug("Ws is null? {WsNull}, ws alive? {WsAlive}, trying to connect? {TryingToConnect}", _ws == null, _ws?.IsAlive, _tryingToConnect);
        if (_ws is not null && _ws.IsAlive) return;
        if (_tryingToConnect) return;

        _tryingToConnect = true;
        Task.Run(
            () =>
            {
                Connect();
                _tryingToConnect = false;
                _tryingToConnect = false;
            }
        );
    }

    private void Connect()
    {
        _logger.LogDebug("Connect called");
        _authed = false;
        if (_host == null || _port == null)
            return;

        var uri = new UriBuilder() { Host = _host, Port = _port.Value, Scheme = "ws" }.Uri;
        _logger.LogInformation("Connecting to {Uri}", uri);
        _ws = new(uri.ToString());
        SetupEvents();
        _ws.Connect();
    }

    private void SetupEvents()
    {
        Debug.Assert(_ws != null, nameof(_ws) + " != null");
        
        _ws.OnOpen += (_, _) =>
        {
            if (!string.IsNullOrEmpty(_authManager.Token))
                Send(new AuthWithTokenRequest(_authManager.Token));
            else
                Send(new AuthenticateRequest());
            SocketConnected?.Invoke(this, EventArgs.Empty);
        };
        _ws.OnMessage += MessageReceived;
        _ws.OnError += (_, e) => _logger.LogInformation("Error from WebSocket: {Error}", e.Message);
        _ws.OnClose += (_, args) => {
            _logger.LogInformation("Disconnected from WebSocket: ({Code}) {Reason}", args.Code, args.Reason);
            SocketClosed?.Invoke(this, args);
        };
    }

    private void MessageReceived(object? sender, MessageEventArgs e)
    {
        if (!e.IsText) return; //Don't handle binary.

        var response = JsonConvert.DeserializeObject<ApiResponse>(e.Data);
        if (response is null) return;
        _logger.LogDebug("Got message: {JsonString}", e.Data);
        _logger.LogDebug("Got message of type: {Type}", response.MessageType);

        switch (response.MessageType)
        {
            case ResponseType.APIError:
                OnApiError?.Invoke(this, new (response.Data!.ToObject<ApiError>()){ RequestId = response.RequestId });
                break;
            case ResponseType.APIStateResponse:
                var data = response.Data!.ToObject<ApiStateResponse>();
                _authed = data?.Authenticated ?? false;
                OnApiState?.Invoke(this, new (data!));
                break;
            case ResponseType.AuthenticationTokenResponse:
                var authenticateResponse = response.Data!.ToObject<AuthenticateResponse>();
                _authManager.Token = authenticateResponse!.AuthToken;
                Send(new AuthWithTokenRequest(authenticateResponse.AuthToken));
                // OnTokenResponse?.Invoke(this, new (authenticateResponse) { RequestId = response.RequestId });
                break;
            case ResponseType.AuthenticationResponse:
                var authResponse = response.Data!.ToObject<AuthenticationResponse>();
                _authed = authResponse?.Authenticated ?? false;
                if (!_authed) _authManager.Token = null;

                OnAuthenticationResponse?.Invoke(this, new (authResponse!) { RequestId = response.RequestId });
                break;
            case ResponseType.StatisticsResponse: break;
            case ResponseType.VTSFolderInfoResponse: break;
            case ResponseType.CurrentModelResponse:
                OnCurrentModelInformation?.Invoke(this, new (response.Data!.ToObject<CurrentModelResponse>()!){ RequestId = response.RequestId });
                break;
            case ResponseType.AvailableModelsResponse:
                OnAvailableModels?.Invoke(this, new (response.Data!.ToObject<AvailableModelsResponse>()!){ RequestId = response.RequestId });
                break;
            case ResponseType.ModelLoadResponse:
                OnModelLoad?.Invoke(this, new(response.Data!.ToObject<ModelLoadResponse>()!){ RequestId = response.RequestId });
                break;
            case ResponseType.MoveModelResponse:
                // OnModelMove?.Invoke(this, new(response.Data.ToObject<EmptyResponse>()){ RequestId = response.RequestId }); //empty response, this can be ignored
                break;
            case ResponseType.HotkeysInCurrentModelResponse:
                OnModelHotkeys?.Invoke(this, new (response.Data!.ToObject<ModelHotkeysResponse>()!){ RequestId = response.RequestId });
                break;
            case ResponseType.HotkeyTriggerResponse:
                OnHotkeyTriggered?.Invoke(this, new (response.Data!.ToObject<HotkeyTriggerResponse>()!){ RequestId = response.RequestId });
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
            case ResponseType.EventSubscriptionResponse: break;
            case ResponseType.ModelMovedEvent:
                OnModelMove?.Invoke(this, new (response.Data!.ToObject<ModelMoveEvent>()!));
                break;
            case ResponseType.ModelConfigChangedEvent:
                OnModelConfigChangedEvent?.Invoke(this, new (response.Data!.ToObject<ModelConfigChangedEvent>()!));
                break;
            case ResponseType.HotkeyTriggeredEvent:
                OnHotkeyTriggeredEvent?.Invoke(this, new (response.Data!.ToObject<HotkeyTriggeredEvent>()!));
                break;
            case ResponseType.ExpressionStateResponse:
                OnExpressionState?.Invoke(this, new (response.Data!.ToObject<ExpressionStateResponse>()!));
                break;
            case ResponseType.ExpressionActivationResponse: 
                OnExpressionActivation?.Invoke(this, EventArgs.Empty);
                break;
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

    public static event EventHandler<EventArgs>? SocketConnected;
    public static event EventHandler<CloseEventArgs>? SocketClosed;
    public static event EventHandler<ApiEventArgs<ApiError>>? OnApiError;
    public static event EventHandler<ApiEventArgs<ApiStateResponse>>? OnApiState;
    public static event EventHandler<ApiEventArgs<AuthenticationResponse>>? OnAuthenticationResponse;
    public static event EventHandler<ApiEventArgs<AvailableModelsResponse>>? OnAvailableModels;
    public static event EventHandler<ApiEventArgs<ModelLoadResponse>>? OnModelLoad;
    public static event EventHandler<ApiEventArgs<ModelHotkeysResponse>>? OnModelHotkeys;
    public static event EventHandler<ApiEventArgs<HotkeyTriggerResponse>>? OnHotkeyTriggered;
    public static event EventHandler<ApiEventArgs<CurrentModelResponse>>? OnCurrentModelInformation;
    public static event EventHandler<ApiEventArgs<ModelMoveEvent>>? OnModelMove;
    public static event EventHandler<ApiEventArgs<ModelConfigChangedEvent>>? OnModelConfigChangedEvent;
    public static event EventHandler<ApiEventArgs<HotkeyTriggeredEvent>>? OnHotkeyTriggeredEvent;
    public static event EventHandler<ApiEventArgs<ExpressionStateResponse>>? OnExpressionState;
    public static event EventHandler<EventArgs>? OnExpressionActivation;
#endregion
}