using Cazzar.Deck.Abstractions.Actions;
using Cazzar.Deck.Abstractions.Surfaces;
using Cazzar.Deck.Abstractions;
using VTubeStudio.Api.Models;
using VTubeStudio.Api.Requests;
using VTubeStudio.Api;

namespace Cazzar.Deck.VTubeStudio.Actions.Encoders;

[DeckAction("movemodel.y",
    Name = "Move Model Y", Tooltip = "VTubeStudio [Move Model Y]",
    Icon = "vts_logo_transparent", PropertyView = "MoveModelAxis")]
public sealed class MoveModelYAction(DeckActionContext context, IVTubeStudio vts, IEncoderSurface encoder)
    : MoveAxisAction(context, vts, encoder)
{
    protected override double Read(ModelPosition position) => position.Y;

    protected override MoveModelRequest Write(double value) => new() { PositionY = value, TimeInSeconds = 0.05d };
}
