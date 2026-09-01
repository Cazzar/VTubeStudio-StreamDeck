using System.Text.Json.Nodes;
using Cazzar.Deck.Abstractions.Actions;
using Cazzar.Deck.Abstractions;
using Cazzar.Deck.VTubeStudio.Caches;
using VTubeStudio.Api.Requests;
using VTubeStudio.Api;
using VTubeStudio.Api.Events;

namespace Cazzar.Deck.VTubeStudio.Actions.Expressions;

[DeckAction(Name = "Toggle Expression", Tooltip = "VTubeStudio [Toggle Expression]",
    Icon = "vts_logo_transparent", PropertyView = "ToggleExpression")]
public sealed class ToggleExpressionAction : VTubeStudioAction<ExpressionSettings, ToggleExpressionAction.State>, IDisposable
{
    public enum State : uint
    {
        [DeckState(Name = "Inactive", Icon = "vts_logo_transparent_off")]
        Inactive = 0,
        [DeckState(Name = "Active", Icon = "vts_logo_transparent")] 
        Active = 1,
    }

    private readonly ExpressionCache _expressions;
    private readonly ModelCache _models;
    private string _expressionName = string.Empty;

    public ToggleExpressionAction(
        DeckActionContext context,
        IVTubeStudio vts,
        ExpressionCache expressions,
        ModelCache models) : base(context, vts)
    {
        _expressions = expressions;
        _models = models;
        _expressions.Updated += OnExpressionsUpdated;
        Vts.ModelLoaded += OnModelLoaded;
    }

    protected override void Pressed()
    {
        if (string.IsNullOrEmpty(Settings.ExpressionFile)) return;

        var next = CurrentState == State.Active ? State.Inactive : State.Active;
        CurrentState = next;

        Vts.Send(new ExpressionActivationRequest(Settings.ExpressionFile, next == State.Active));
        _expressions.Refresh();
    }

    protected override JsonNode ClientData() => new JsonObject
    {
        ["models"] = Choices(_models.Models, m => m.Id, m => m.Name),
        ["expressions"] = Choices(_expressions.For(Settings.ModelId), e => e.File, e => e.Name),
        ["connected"] = Vts.IsAuthenticated,
    };

    protected override void OnSettingsChanged(ExpressionSettings previous, ExpressionSettings current)
    {
        if (previous.ShowName && !current.ShowName) Title = null;

        if (previous.ModelId != current.ModelId || previous.ExpressionFile != current.ExpressionFile)
            _expressionName = string.Empty;

        Sync(_expressions.For(current.ModelId));
    }
    
    private void OnModelLoaded(object? sender, VtsEventArgs<ModelLoadedEvent> e)
    {
        if (e.Response.ModelId == Settings.ModelId) return;

        CurrentState = State.Inactive;
    }

    public override void Refresh(IPayload body)
    {
        _expressions.Refresh();
        base.Refresh(body);
    }

    [PropertyViewCommand("select-current-model")]
    public void SelectCurrentModel(IPayload body)
    {
        if (_models.CurrentModelId is not { } id) return;

        Settings.ModelId = id;
        _ = SaveSettingsAsync();
        _ = UpdateClientAsync();
    }

    private void OnExpressionsUpdated(object? sender, EventArgs e) =>
        Sync(_expressions.For(Settings.ModelId));

    private void Sync(IReadOnlyList<ExpressionStatus> expressions)
    {
        if (string.IsNullOrEmpty(Settings.ExpressionFile)) return;
        if (expressions.FirstOrDefault(e => e.File == Settings.ExpressionFile) is not { } expression) return;

        _expressionName = expression.Name;

        CurrentState = expression.IsActive ? State.Active : State.Inactive;

        if (Settings.ShowName && _expressionName.Length > 0) Title = _expressionName;
    }

    public void Dispose()
    {
        _expressions.Updated -= OnExpressionsUpdated;
        Vts.ModelLoaded -= OnModelLoaded;
    }
}
