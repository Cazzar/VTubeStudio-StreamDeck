using System.Text.Json.Nodes;
using Cazzar.Deck.Abstractions.Actions;
using Cazzar.Deck.Abstractions;
using Cazzar.Deck.VTubeStudio.Caches;
using VTubeStudio.Api.Requests;
using VTubeStudio.Api;

namespace Cazzar.Deck.VTubeStudio.Actions.Expressions;

[DeckAction(Name = "Hold Expression", Tooltip = "VTubeStudio [Hold Expression]",
    Icon = "vts_logo_transparent", PropertyView = "HoldExpression")]
public sealed class HoldExpressionAction(
    DeckActionContext context,
    IVTubeStudio vts,
    ExpressionCache expressions,
    ModelCache models) : VTubeStudioAction<ExpressionSettings>(context, vts)
{
    protected override void Pressed() => Activate(true);

    protected override void Released() => Activate(false);

    private void Activate(bool active)
    {
        if (string.IsNullOrEmpty(Settings.ExpressionFile)) return;

        Vts.Send(new ExpressionActivationRequest(Settings.ExpressionFile, active));
    }

    protected override JsonNode ClientData() => new JsonObject
    {
        ["models"] = Choices(models.Models, m => m.Id, m => m.Name),
        ["expressions"] = Choices(expressions.For(Settings.ModelId), e => e.File, e => e.Name),
        ["connected"] = Vts.IsAuthenticated,
    };

    protected override void OnSettingsChanged(ExpressionSettings previous, ExpressionSettings current)
    {
        if (!current.ShowName)
        {
            _ = SetTitleAsync(null);
            return;
        }

        if (expressions.For(current.ModelId).FirstOrDefault(e => e.File == current.ExpressionFile) is { } expression)
            _ = SetTitleAsync(expression.Name);
    }

    public override void Refresh(IPayload body)
    {
        expressions.Refresh();
        base.Refresh(body);
    }

    [PropertyViewCommand("select-current-model")]
    public void SelectCurrentModel(IPayload body)
    {
        if (models.CurrentModelId is not { } id) return;

        Settings.ModelId = id;
        _ = SaveSettingsAsync();
        _ = UpdateClientAsync();
    }
}
