using System.Text.Json.Nodes;
using Cazzar.Deck.Abstractions.Actions;
using Cazzar.Deck.Abstractions;
using Cazzar.Deck.VTubeStudio.Caches;
using VTubeStudio.Api.Requests;
using VTubeStudio.Api;

namespace Cazzar.Deck.VTubeStudio.Actions.Expressions;

[DeckAction(Name = "Hold Expression", Tooltip = "VTubeStudio [Hold Expression]",
    Icon = "vts_logo_transparent", PropertyView = "HoldExpression")]
public sealed class HoldExpressionAction : VTubeStudioAction<ExpressionSettings>, IDisposable
{
    private readonly ExpressionCache _expressions;
    private readonly ModelCache _models;

    public HoldExpressionAction(
        DeckActionContext context,
        IVTubeStudio vts,
        ExpressionCache expressions,
        ModelCache models) : base(context, vts)
    {
        _expressions = expressions;
        _models = models;
        _expressions.Updated += OnExpressionsUpdated;
    }

    protected override void Pressed() => Activate(true);

    protected override void Released() => Activate(false);

    private void Activate(bool active)
    {
        if (string.IsNullOrEmpty(Settings.ExpressionFile)) return;

        Vts.Send(new ExpressionActivationRequest(Settings.ExpressionFile, active));
    }

    protected override JsonNode ClientData() => new JsonObject
    {
        ["models"] = Choices(_models.Models, m => m.Id, m => m.Name),
        ["expressions"] = Choices(_expressions.For(Settings.ModelId), e => e.File, e => e.Name),
        ["connected"] = Vts.IsAuthenticated,
    };

    protected override void OnSettingsChanged(ExpressionSettings previous, ExpressionSettings current) => UpdateTitle();

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

    private void OnExpressionsUpdated(object? sender, EventArgs e) => UpdateTitle();

    private void UpdateTitle()
    {
        if (!Settings.ShowName)
        {
            Title = null;
            return;
        }

        if (_expressions.For(Settings.ModelId).FirstOrDefault(e => e.File == Settings.ExpressionFile) is { } expression)
            Title = expression.Name;
    }

    public void Dispose() => _expressions.Updated -= OnExpressionsUpdated;
}
