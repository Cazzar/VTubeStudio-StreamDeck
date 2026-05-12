using System;
using System.Collections.Concurrent;
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

        private readonly ConcurrentDictionary<string, List<Expression>> _cache = new();
        public IDictionary<string, List<Expression>> Expressions => _cache;

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
            if (string.IsNullOrEmpty(e.Response.ModelId)) return;
            _cache.AddOrUpdate(e.Response.ModelId, e.Response.Expressions, (_, _) => e.Response.Expressions);
            _logger.LogDebug("Expression states updated for {ModelId}, {Count} expressions", e.Response.ModelId, e.Response.Expressions.Count);
            Updated?.Invoke(this, new() { Expressions = _cache });
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
        public IDictionary<string, List<Expression>> Expressions { get; init; } = new Dictionary<string, List<Expression>>();
    }
}
