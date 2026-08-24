using Cazzar.Deck.Abstractions.Actions;
using Cazzar.Deck.Abstractions;
using System.Text.Json.Serialization;
using VTubeStudio.Api.Requests;
using VTubeStudio.Api;

namespace Cazzar.Deck.VTubeStudio.Actions.Movement;

[DeckAction(Name = "Scale Model", Tooltip = "VTubeStudio [Scale Model]",
    Icon = "vts_logo_transparent", PropertyView = "ScaleModel")]
public sealed class ScaleModelAction(DeckActionContext context, IVTubeStudio vts)
    : VTubeStudioAction<ScaleModelAction.Options>(context, vts)
{
    public sealed class Options
    {
        [JsonPropertyName("size")] public double Size { get; set; }
    }

    // Absolute by design: a relative scale would compound on every press.
    protected override void Pressed() => Vts.Send(new MoveModelRequest { Size = Settings.Size });
}
