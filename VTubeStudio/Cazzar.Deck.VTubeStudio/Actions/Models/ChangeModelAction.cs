using System.Text.Json.Nodes;
using Cazzar.Deck.Abstractions.Actions;
using Cazzar.Deck.Abstractions;
using Cazzar.Deck.VTubeStudio.Caches;
using System.Text.Json.Serialization;
using VTubeStudio.Api.Requests;
using VTubeStudio.Api;

namespace Cazzar.Deck.VTubeStudio.Actions.Models;

[DeckAction(Name = "Change Model", Tooltip = "VTubeStudio [Change Model]",
    Icon = "vts_logo_transparent", PropertyView = "ModelChange")]
public sealed class ChangeModelAction(
    DeckActionContext context,
    IVTubeStudio vts,
    ModelCache models) : VTubeStudioAction<ChangeModelAction.Options>(context, vts)
{
    public sealed class Options
    {
        [JsonPropertyName("modelId")] public string ModelId { get; set; } = string.Empty;
        [JsonPropertyName("showName")] public bool ShowName { get; set; } = true;
    }

    protected override void Pressed()
    {
        if (string.IsNullOrEmpty(Settings.ModelId)) return;

        Vts.Send(new ModelLoadRequest(Settings.ModelId));
    }

    protected override JsonNode ClientData() => new JsonObject
    {
        ["models"] = Choices(models.Models, m => m.Id, m => m.Name),
        ["connected"] = Vts.IsAuthenticated,
    };

    protected override void OnSettingsChanged(Options previous, Options current)
    {
        if (previous.ShowName && !current.ShowName) Title = null;

        UpdateTitle();
    }

    public override void Refresh(IPayload body)
    {
        models.Refresh();
        base.Refresh(body);
    }

    public override void Tick()
    {
        base.Tick();
        UpdateTitle();
    }

    private void UpdateTitle()
    {
        if (!Settings.ShowName) return;

        if (models.Models.FirstOrDefault(m => m.Id == Settings.ModelId)?.Name is { Length: > 0 } name)
            Title = name;
    }
}
