using VTubeStudio.Api;
using VTubeStudio.Api.Events;
using VTubeStudio.Api.Requests;
using VTubeStudio.Api.Responses;

namespace Cazzar.Deck.VTubeStudio.Tests;

// Stands in for the real client so tests never open a socket to a VTube Studio that may be running locally.
public sealed class FakeVTubeStudio : IVTubeStudio
{
    private readonly List<IVtsRequest> _sent = [];

    public IReadOnlyList<IVtsRequest> Sent
    {
        get { lock (_sent) return [.. _sent]; }
    }

    public bool IsAuthenticated => true;
    public bool IsConnected => true;

    public void Send(IVtsRequest request, string? requestId = null)
    {
        lock (_sent) _sent.Add(request);
    }

    public void EnsureConnected()
    {
    }

    public void Reconnect()
    {
    }

    public (string Host, ushort Port)? Connection { get; private set; }

    public void SetConnection(string host, ushort port) => Connection = (host, port);
    
    public void RaiseModelLoaded(ModelLoadedEvent loaded) => ModelLoaded?.Invoke(this, new(loaded));
    
    public void RaiseExpressionToggled(ExpressionToggledEvent toggled) 
        => ExpressionToggled?.Invoke(this, new(toggled));

    public void RaiseExpressionState(ExpressionStateResponse state) =>
        ExpressionState?.Invoke(this, new(state));

#pragma warning disable CS0067 // Nothing raises these; actions only subscribe.
    public event EventHandler? Connected;
    public event EventHandler? Disconnected;
    public event EventHandler<VtsEventArgs<AuthenticationResponse>>? Authenticated;
    public event EventHandler<VtsEventArgs<ApiErrorResponse>>? ApiError;
    public event EventHandler<VtsEventArgs<CurrentModelResponse>>? CurrentModel;
    public event EventHandler<VtsEventArgs<AvailableModelsResponse>>? AvailableModels;
    public event EventHandler<VtsEventArgs<ModelHotkeysResponse>>? ModelHotkeys;
    public event EventHandler<VtsEventArgs<HotkeyTriggerResponse>>? HotkeyTriggerCompleted;
    public event EventHandler<VtsEventArgs<HotkeyTriggeredEvent>>? HotkeyTriggered;
    public event EventHandler<VtsEventArgs<ExpressionStateResponse>>? ExpressionState;
    public event EventHandler<VtsEventArgs<ExpressionToggledEvent>>? ExpressionToggled;
    public event EventHandler<VtsEventArgs<ModelMovedEvent>>? ModelMoved;
    public event EventHandler<VtsEventArgs<ModelLoadedEvent>>? ModelLoaded;
    public event EventHandler<VtsEventArgs<ModelConfigChangedEvent>>? ModelConfigChanged;
#pragma warning restore CS0067
}
