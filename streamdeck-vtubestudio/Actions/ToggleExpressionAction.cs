using System.Collections.Generic;
using System.Linq;
using Cazzar.StreamDeck.VTubeStudio.Models;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Requests;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Responses;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StreamDeckLib;

namespace Cazzar.StreamDeck.VTubeStudio.Actions;

[StreamDeckAction("dev.cazzar.vtubestudio.toggleexpression")]
public class ToggleExpressionAction : BaseAction<ToggleExpressionAction.PluginSettings>
{
    public class PluginSettings
    {
        [JsonProperty("expressionFile")]
        public string ExpressionFile { get; set; } = string.Empty;

        [JsonProperty("showName")]
        public bool ShowName { get; set; } = true;
    }

    private readonly ExpressionStateCache _expressionCache;
    private bool _isActive;
    private string _expressionName = string.Empty;

    public ToggleExpressionAction(GlobalSettingsManager gsm, VTubeStudioWebsocketClient vts, IStreamDeckConnection isd, ILogger<ToggleExpressionAction> logger, ExpressionStateCache expressionCache) : base(gsm, vts, isd, logger)
    {
        _expressionCache = expressionCache;
        _expressionCache.Updated += OnCacheUpdated;
    }

    private void OnCacheUpdated(object sender, ExpressionStateCacheUpdatedEventArgs e)
    {
        UpdateFromExpressions(e.Expressions);
    }

    private void UpdateFromExpressions(List<Expression> expressions)
    {
        if (string.IsNullOrEmpty(Settings.ExpressionFile)) return;

        var expr = expressions.FirstOrDefault(x => x.File == Settings.ExpressionFile);
        if (expr == null) return;

        _expressionName = expr.Name;

        var wasActive = _isActive;
        _isActive = expr.Active;

        if (wasActive != _isActive)
            SetState(_isActive ? 1u : 0u);

        if (Settings.ShowName && !string.IsNullOrEmpty(_expressionName))
            SetTitle(_expressionName);
    }

    protected override void Pressed()
    {
        if (string.IsNullOrEmpty(Settings.ExpressionFile)) return;

        _isActive = !_isActive;
        SetState(_isActive ? 1u : 0u);
        Vts.Send(new ExpressionActivationRequest(Settings.ExpressionFile, _isActive));
        _expressionCache.Refresh();
    }

    protected override void Released() { }

    protected override object GetClientData()
    {
        var expressions = _expressionCache.Expressions
            .Select(e => new VTubeReference { Id = e.File, Name = e.Name })
            .ToList();
        return new { Expressions = expressions, Connected = Vts.IsAuthed };
    }

    protected override void SettingsUpdated(PluginSettings oldSettings, PluginSettings newSettings)
    {
        if (!newSettings.ShowName && oldSettings.ShowName)
            SetTitle(null);

        if (newSettings.ExpressionFile != oldSettings.ExpressionFile)
            UpdateFromExpressions(_expressionCache.Expressions);
    }

    [PluginCommand("refresh")]
    public override async void Refresh(PluginPayload pl)
    {
        _expressionCache.Refresh();
        await UpdateClient();
    }
}
