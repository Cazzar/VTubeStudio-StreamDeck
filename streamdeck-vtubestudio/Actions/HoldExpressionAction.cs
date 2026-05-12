using System;
using System.Linq;
using Cazzar.StreamDeck.VTubeStudio.Models;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Requests;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StreamDeckLib;

namespace Cazzar.StreamDeck.VTubeStudio.Actions;

[StreamDeckAction("dev.cazzar.vtubestudio.holdexpression")]
public class HoldExpressionAction : BaseAction<HoldExpressionAction.PluginSettings, HoldExpressionAction.State>, IDisposable
{
    public enum State : uint { Inactive = 0, Active = 1 }

    public class PluginSettings
    {
        [JsonProperty("expressionFile")]
        public string ExpressionFile { get; set; } = string.Empty;

        [JsonProperty("showName")]
        public bool ShowName { get; set; } = true;
    }

    private readonly ExpressionStateCache _expressionCache;
    private string _expressionName = string.Empty;

    public HoldExpressionAction(GlobalSettingsManager gsm, VTubeStudioWebsocketClient vts, IStreamDeckConnection isd, ILogger<HoldExpressionAction> logger, ExpressionStateCache expressionCache) : base(gsm, vts, isd, logger)
    {
        _expressionCache = expressionCache;
        _expressionCache.Updated += OnCacheUpdated;
    }

    private void OnCacheUpdated(object sender, ExpressionStateCacheUpdatedEventArgs e)
    {
        if (string.IsNullOrEmpty(Settings.ExpressionFile)) return;
        var expr = e.Expressions.FirstOrDefault(x => x.File == Settings.ExpressionFile);
        if (expr == null) return;
        _expressionName = expr.Name;
        if (Settings.ShowName && !string.IsNullOrEmpty(_expressionName))
            SetTitle(_expressionName);
    }

    protected override void Pressed()
    {
        if (string.IsNullOrEmpty(Settings.ExpressionFile)) return;
        SetState(State.Active);
        Vts.Send(new ExpressionActivationRequest(Settings.ExpressionFile, true));
    }

    protected override void Released()
    {
        if (string.IsNullOrEmpty(Settings.ExpressionFile)) return;
        SetState(State.Inactive);
        Vts.Send(new ExpressionActivationRequest(Settings.ExpressionFile, false));
        _expressionCache.Refresh();
    }

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
            OnCacheUpdated(this, new() { Expressions = _expressionCache.Expressions });
    }

    [PluginCommand("refresh")]
    public override async void Refresh(PluginPayload pl)
    {
        _expressionCache.Refresh();
        await UpdateClient();
    }

    public void Dispose()
    {
        _expressionCache.Updated -= OnCacheUpdated;
    }
}
