using System;
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
public class ToggleExpressionAction : BaseAction<ToggleExpressionAction.PluginSettings, ToggleExpressionAction.State>, IDisposable
{
    public enum State : uint { Inactive = 0, Active = 1 }

    public class PluginSettings
    {
        [JsonProperty("modelId")]
        public string ModelId { get; set; } = string.Empty;

        [JsonProperty("expressionFile")]
        public string ExpressionFile { get; set; } = string.Empty;

        [JsonProperty("showName")]
        public bool ShowName { get; set; } = true;
    }

    private readonly ExpressionStateCache _expressionCache;
    private readonly ModelCache _modelCache;
    private string _expressionName = string.Empty;

    public ToggleExpressionAction(GlobalSettingsManager gsm, VTubeStudioWebsocketClient vts, IStreamDeckConnection isd, ILogger<ToggleExpressionAction> logger, ExpressionStateCache expressionCache, ModelCache modelCache) : base(gsm, vts, isd, logger)
    {
        _expressionCache = expressionCache;
        _modelCache = modelCache;
        _expressionCache.Updated += OnCacheUpdated;
    }

    private void OnCacheUpdated(object sender, ExpressionStateCacheUpdatedEventArgs e)
    {
        if (string.IsNullOrEmpty(Settings.ModelId)) return;
        if (!e.Expressions.TryGetValue(Settings.ModelId, out var expressions)) return;
        UpdateFromExpressions(expressions);
    }

    private void UpdateFromExpressions(List<Expression> expressions)
    {
        if (string.IsNullOrEmpty(Settings.ExpressionFile)) return;

        var expr = expressions.FirstOrDefault(x => x.File == Settings.ExpressionFile);
        if (expr == null) return;

        _expressionName = expr.Name;

        var newState = expr.Active ? State.Active : State.Inactive;
        if (newState != CurrentState)
            SetState(newState);

        if (Settings.ShowName && !string.IsNullOrEmpty(_expressionName))
            SetTitle(_expressionName);
    }

    protected override void Pressed()
    {
        if (string.IsNullOrEmpty(Settings.ExpressionFile)) return;

        var newState = CurrentState == State.Active ? State.Inactive : State.Active;
        SetState(newState);
        Vts.Send(new ExpressionActivationRequest(Settings.ExpressionFile, newState == State.Active));
        _expressionCache.Refresh();
    }

    protected override void Released() { }

    protected override object GetClientData()
    {
        var models = _modelCache.Models.Select(s => new VTubeReference { Id = s.Id, Name = s.Name }).ToList();
        List<VTubeReference> expressions = new();
        if (!string.IsNullOrEmpty(Settings.ModelId) && _expressionCache.Expressions.TryGetValue(Settings.ModelId, out var exprs))
            expressions = exprs.Select(e => new VTubeReference { Id = e.File, Name = e.Name }).ToList();
        return new { Models = models, Expressions = expressions, Connected = Vts.IsAuthed };
    }

    protected override void SettingsUpdated(PluginSettings oldSettings, PluginSettings newSettings)
    {
        if (!newSettings.ShowName && oldSettings.ShowName)
            SetTitle(null);
        else if (newSettings.ShowName && !oldSettings.ShowName && !string.IsNullOrEmpty(_expressionName))
            SetTitle(_expressionName);

        if (newSettings.ModelId != oldSettings.ModelId || newSettings.ExpressionFile != oldSettings.ExpressionFile)
        {
            _expressionName = string.Empty;
            if (!string.IsNullOrEmpty(newSettings.ModelId) && _expressionCache.Expressions.TryGetValue(newSettings.ModelId, out var expressions))
                UpdateFromExpressions(expressions);
        }
    }

    [PluginCommand("refresh")]
    public override async void Refresh(PluginPayload pl)
    {
        _expressionCache.Refresh();
        await UpdateClient();
    }

    [PluginCommand("select-current-model")]
    public async void SelectCurrentModel(PluginPayload pl)
    {
        if (string.IsNullOrEmpty(_modelCache.CurrentModelId)) return;
        Settings.ModelId = _modelCache.CurrentModelId;
        SaveSettings();
        await UpdateClient();
    }

    public void Dispose()
    {
        _expressionCache.Updated -= OnCacheUpdated;
    }
}
