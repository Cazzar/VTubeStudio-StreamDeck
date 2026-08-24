using Cazzar.Deck.Abstractions.Actions;
using Cazzar.Deck.Abstractions;
using Cazzar.Deck.VTubeStudio.Caches;
using VTubeStudio.Api.Requests;
using VTubeStudio.Api;

namespace Cazzar.Deck.VTubeStudio.Actions.Models;

[DeckAction("reloadmodel",
    Name = "Reload Model", Tooltip = "VTubeStudio [Reload Model]",
    Icon = "vts_logo_transparent")]
public sealed class ReloadCurrentModelAction(
    DeckActionContext context,
    IVTubeStudio vts,
    ModelCache models) : VTubeStudioAction<ReloadCurrentModelAction.Options>(context, vts)
{
    public sealed class Options;

    protected override void Pressed()
    {
        if (models.CurrentModelId is not { } id) return;

        Vts.Send(new ModelLoadRequest(id));
    }
}
