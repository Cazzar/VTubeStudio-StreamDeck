using System.Net.WebSockets;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Text;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VTubeStudio.Api.Events;
using VTubeStudio.Api.Requests;
using VTubeStudio.Api.Responses;

namespace VTubeStudio.Api;

[UsedImplicitly]
public sealed class VTubeStudioClient(
    IOptionsMonitor<VtsConnectionOptions> options,
    IVtsSettingsStore settings,
    IVtsPluginInfo plugin,
    ILogger<VTubeStudioClient> logger) : IVTubeStudio, IAsyncDisposable
{
    private const int BufferSize = 1024 * 64;

    private readonly SemaphoreSlim _connectGate = new(1, 1);
    private ClientWebSocket? _socket;
    private CancellationTokenSource? _readLoop;
    private bool _authenticated;

    public bool IsConnected => _socket?.State == WebSocketState.Open;
    public bool IsAuthenticated => _authenticated && IsConnected;

    public event EventHandler? Connected;
    public event EventHandler? Disconnected;
    public event EventHandler<VtsEventArgs<AuthenticationResponse>>? Authenticated;
    public event EventHandler<VtsEventArgs<ApiErrorResponse>>? ApiError;
    public event EventHandler<VtsEventArgs<CurrentModelResponse>>? CurrentModel;
    public event EventHandler<VtsEventArgs<AvailableModelsResponse>>? AvailableModels;
    public event EventHandler<VtsEventArgs<ModelHotkeysResponse>>? ModelHotkeys;
    public event EventHandler<VtsEventArgs<HotkeyTriggerResponse>>? HotkeyTriggerCompleted;
    public event EventHandler<VtsEventArgs<HotkeyTriggeredEvent>>? HotkeyTriggered;
    public event EventHandler<VtsEventArgs<ExpressionToggledEvent>>? ExpressionToggled;
    public event EventHandler<VtsEventArgs<ExpressionStateResponse>>? ExpressionState;
    public event EventHandler<VtsEventArgs<ModelMovedEvent>>? ModelMoved;
    public event EventHandler<VtsEventArgs<ModelLoadedEvent>>? ModelLoaded;
    public event EventHandler<VtsEventArgs<ModelConfigChangedEvent>>? ModelConfigChanged;

    public void EnsureConnected()
    {
        if (!settings.IsLoaded)
        {
            logger.LogTrace("Waiting for stored settings before connecting");
            return;
        }

        if (IsConnected || _connectGate.CurrentCount == 0) return;

        _ = Task.Run(() => ConnectAsync());
    }

    public void Reconnect() => _ = Task.Run(() => ConnectAsync(force: true));

    public void SetConnection(string host, ushort port)
    {
        if (options.CurrentValue.Host == host && options.CurrentValue.Port == port) return;

        options.CurrentValue.Host = host;
        options.CurrentValue.Port = port;
        settings.SetEndpoint(host, port);
        Reconnect();
    }

    // Closing and reopening have to happen under the same gate, or a reconnect can tear down a
    // socket another caller is still handshaking on.
    private async Task ConnectAsync(bool force = false)
    {
        if (!await _connectGate.WaitAsync(force ? Timeout.Infinite : 0)) return;

        try
        {
            if (IsConnected && !force) return;

            await CloseAsync();

            var uri = new UriBuilder("ws", options.CurrentValue.Host, options.CurrentValue.Port).Uri;
            logger.LogInformation("Connecting to VTube Studio at {Uri}", uri);

            _authenticated = false;
            _socket = new();
            _readLoop = new();

            await _socket.ConnectAsync(uri, CancellationToken.None);

            Connected?.Invoke(this, EventArgs.Empty);
            _ = Task.Run(() => ReadAsync(_readLoop.Token));

            Authenticate();
        }
        catch (Exception e)
        {
            logger.LogWarning(e, "Could not connect to VTube Studio");
            Disconnected?.Invoke(this, EventArgs.Empty);
        }
        finally
        {
            _connectGate.Release();
        }
    }

    private void Authenticate()
    {
        if (string.IsNullOrEmpty(settings.Token))
            Send(new AuthenticationTokenRequest(plugin.Name, plugin.Developer, plugin.Icon));
        else
            Send(new AuthenticationRequest(plugin.Name, plugin.Developer, settings.Token));
    }

    public void Send(IVtsRequest request, string? requestId = null)
    {
        if (_socket is not { State: WebSocketState.Open })
        {
            logger.LogDebug("Not connected; dropping {MessageType}", request.MessageType);
            return;
        }

        if (!_authenticated && request is not IUnauthenticatedRequest)
        {
            logger.LogDebug("Not authenticated; dropping {MessageType}", request.MessageType);
            return;
        }

        var envelope = new JsonObject
        {
            ["apiName"] = "VTubeStudioPublicAPI",
            ["apiVersion"] = "1.0",
            ["messageType"] = request.MessageType,
            ["data"] = JsonSerializer.SerializeToNode(request, VtsJson.Options.GetTypeInfo(request.GetType())),
        };

        if (requestId is not null) envelope["requestID"] = requestId;

        var json = envelope.ToJsonString(VtsJson.Options);
        logger.LogTrace("-> {Json}", json);

        _ = _socket.SendAsync(Encoding.UTF8.GetBytes(json), WebSocketMessageType.Text, true, CancellationToken.None);
    }

    private async Task ReadAsync(CancellationToken cancellationToken)
    {
        var buffer = new byte[BufferSize];
        var text = new StringBuilder();

        // A multi-byte character can straddle two fragments, so the decoder has to carry the
        // trailing bytes across receives rather than each fragment being decoded on its own.
        var decoder = Encoding.UTF8.GetDecoder();

        try
        {
            while (_socket is { State: WebSocketState.Open } && !cancellationToken.IsCancellationRequested)
            {
                var result = await _socket.ReceiveAsync(buffer.AsMemory(), cancellationToken);

                if (result.MessageType == WebSocketMessageType.Close) break;
                if (result.MessageType != WebSocketMessageType.Text) continue;

                var chars = new char[Encoding.UTF8.GetMaxCharCount(result.Count)];
                text.Append(chars, 0, decoder.GetChars(buffer, 0, result.Count, chars, 0, result.EndOfMessage));

                if (!result.EndOfMessage) continue;

                Handle(text.ToString());
                text.Clear();
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception e)
        {
            logger.LogWarning(e, "VTube Studio read loop ended");
        }

        _authenticated = false;
        Disconnected?.Invoke(this, EventArgs.Empty);
    }

    private void Handle(string frame)
    {
        logger.LogTrace("<- {Json}", frame);

        if (JsonNode.Parse(frame) is not JsonObject message) return;

        var requestId = message["requestID"]?.GetValue<string>();
        var data = message["data"];

        switch (message["messageType"]?.GetValue<string>())
        {
            case "APIError":
                Raise(ApiError, data, requestId);
                break;

            case "APIStateResponse":
                _authenticated = As<ApiStateResponse>(data)?.Authenticated ?? false;
                break;

            case "AuthenticationTokenResponse":
                settings.Token = As<AuthenticationTokenResponse>(data)?.Token;
                Authenticate();
                break;

            case "AuthenticationResponse":
                var auth = As<AuthenticationResponse>(data);
                _authenticated = auth?.Authenticated ?? false;

                // A rejected token is a dead token; drop it and ask for a fresh one.
                if (!_authenticated)
                {
                    settings.Token = null;
                    Send(new AuthenticationTokenRequest(plugin.Name, plugin.Developer, plugin.Icon));
                }
                else
                {
                    SubscribeToEvents();
                }

                if (auth is not null) Authenticated?.Invoke(this, new(auth, requestId));
                break;

            case "CurrentModelResponse": Raise(CurrentModel, data, requestId); break;
            case "AvailableModelsResponse": Raise(AvailableModels, data, requestId); break;
            case "HotkeysInCurrentModelResponse": Raise(ModelHotkeys, data, requestId); break;
            case "HotkeyTriggerResponse": Raise(HotkeyTriggerCompleted, data, requestId); break;
            case "ExpressionStateResponse": Raise(ExpressionState, data, requestId); break;
            case "ModelMovedEvent": Raise(ModelMoved, data, requestId); break;
            case "ModelLoadedEvent": Raise(ModelLoaded, data, requestId); break;
            case "ModelConfigChangedEvent": Raise(ModelConfigChanged, data, requestId); break;
            case "HotkeyTriggeredEvent": Raise(HotkeyTriggered, data, requestId); break;
            case "ExpressionToggledEvent": Raise(ExpressionToggled, data, requestId); break;
        }
    }

    private void SubscribeToEvents()
    {
        Send(new EventSubscriptionRequest("ModelMovedEvent"));
        Send(new EventSubscriptionRequest("ModelConfigChangedEvent"));
        Send(new EventSubscriptionRequest("HotkeyTriggeredEvent"));
        Send(new EventSubscriptionRequest("ModelLoadedEvent"));
        Send(
            new EventSubscriptionRequest("ExpressionToggledEvent")
            {
                Config = new JsonObject
                {
                    ["sendAllActiveStatesOnSubscription"] = true,
                    ["ignoreLive2DItems"] = true,
                },
            });

        Send(new CurrentModelRequest());
    }

    private static T? As<T>(JsonNode? node) => node is null ? default : node.Deserialize(VtsJson.TypeInfo<T>());

    private void Raise<T>(EventHandler<VtsEventArgs<T>>? handler, JsonNode? data, string? requestId)
    {
        if (handler is null || As<T>(data) is not { } response) return;

        handler(this, new(response, requestId));
    }

    private async Task CloseAsync()
    {
        if (_readLoop is not null) await _readLoop.CancelAsync();

        if (_socket is { State: WebSocketState.Open })
        {
            try
            {
                await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
            }
            catch (WebSocketException)
            {
            }
        }

        _socket?.Dispose();
        _socket = null;
        _authenticated = false;
    }

    public async ValueTask DisposeAsync()
    {
        await CloseAsync();
        _connectGate.Dispose();
    }
}
