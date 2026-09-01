using VTubeStudio.Api.Events;
using VTubeStudio.Api.Requests;
using VTubeStudio.Api.Responses;

namespace VTubeStudio.Api;

public interface IVTubeStudio
{
    bool IsAuthenticated { get; }
    bool IsConnected { get; }

    void Send(IVtsRequest request, string? requestId = null);
    void EnsureConnected();
    void Reconnect();
    void SetConnection(string host, ushort port);

    event EventHandler? Connected;
    event EventHandler? Disconnected;
    event EventHandler<VtsEventArgs<AuthenticationResponse>>? Authenticated;
    event EventHandler<VtsEventArgs<ApiErrorResponse>>? ApiError;
    event EventHandler<VtsEventArgs<CurrentModelResponse>>? CurrentModel;
    event EventHandler<VtsEventArgs<AvailableModelsResponse>>? AvailableModels;
    event EventHandler<VtsEventArgs<ModelHotkeysResponse>>? ModelHotkeys;
    event EventHandler<VtsEventArgs<HotkeyTriggerResponse>>? HotkeyTriggerCompleted;
    event EventHandler<VtsEventArgs<HotkeyTriggeredEvent>>? HotkeyTriggered;
    event EventHandler<VtsEventArgs<ExpressionToggledEvent>>? ExpressionToggled;
    event EventHandler<VtsEventArgs<ExpressionStateResponse>>? ExpressionState;
    event EventHandler<VtsEventArgs<ModelMovedEvent>>? ModelMoved;
    event EventHandler<VtsEventArgs<ModelLoadedEvent>>? ModelLoaded;
    event EventHandler<VtsEventArgs<ModelConfigChangedEvent>>? ModelConfigChanged;
}

