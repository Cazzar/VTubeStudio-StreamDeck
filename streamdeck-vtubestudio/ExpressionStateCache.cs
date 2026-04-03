using System;
using System.Collections.Generic;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Events;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Requests;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Responses;
using Microsoft.Extensions.Logging;

namespace Cazzar.StreamDeck.VTubeStudio
{
    public class ExpressionStateCache
    {
        private readonly VTubeStudioWebsocketClient _vts;
        private readonly ILogger<ExpressionStateCache> _logger;

        public List<Expression> Expressions { get; private set; } = new();

        public ExpressionStateCache(VTubeStudioWebsocketClient vts, ILogger<ExpressionStateCache> logger)
        {
            _vts = vts;
            _logger = logger;

            VTubeStudioWebsocketClient.OnAuthenticationResponse += OnAuthenticated;
            VTubeStudioWebsocketClient.OnHotkeyTriggeredEvent += OnHotkeyTriggered;
            VTubeStudioWebsocketClient.OnExpressionState += OnExpressionState;
            VTubeStudioWebsocketClient.OnModelLoad += OnModelLoad;
        }

        private void OnAuthenticated(object sender, ApiEventArgs<AuthenticationResponse> e)
        {
            if (!e.Response.Authenticated) return;

            _logger.LogInformation("Authenticated, requesting expression states");
            _vts.Send(new ExpressionStateRequest());
        }

        private void OnModelLoad(object sender, ApiEventArgs<ModelLoadResponse> e)
        {
            _logger.LogInformation("Model loaded, requesting expression states");
            _vts.Send(new ExpressionStateRequest());
        }

        private void OnHotkeyTriggered(object sender, ApiEventArgs<HotkeyTriggeredEvent> e)
        {
            if (e.Response.HotkeyType != "ToggleExpression") return;

            _logger.LogDebug("ToggleExpression hotkey fired, refreshing expression states");
            _vts.Send(new ExpressionStateRequest());
        }

        private void OnExpressionState(object sender, ApiEventArgs<ExpressionStateResponse> e)
        {
            Expressions = e.Response.Expressions;
            _logger.LogDebug("Expression states updated, {Count} expressions", Expressions.Count);
            Updated?.Invoke(this, new() { Expressions = Expressions });
        }

        public void Refresh()
        {
            if (_vts.IsAuthed)
                _vts.Send(new ExpressionStateRequest());
        }

        public event EventHandler<ExpressionStateCacheUpdatedEventArgs> Updated;
    }

    public class ExpressionStateCacheUpdatedEventArgs : EventArgs
    {
        public List<Expression> Expressions { get; init; } = new();
    }
}
