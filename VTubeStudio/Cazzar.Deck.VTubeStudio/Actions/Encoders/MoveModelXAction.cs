using Cazzar.Deck.Abstractions.Actions;
using Cazzar.Deck.Abstractions.Surfaces;
using Cazzar.Deck.Abstractions;
using Cazzar.Deck.VTubeStudio.Actions.Movement;
using VTubeStudio.Api.Models;
using VTubeStudio.Api.Requests;
using VTubeStudio.Api;

namespace Cazzar.Deck.VTubeStudio.Actions.Encoders;

[DeckAction("movemodel.x",
    Name = "Move Model X", Tooltip = "VTubeStudio [Move Model X]",
    Icon = "vts_logo_transparent", PropertyView = "MoveModelAxis")]
public sealed class MoveModelXAction(
    DeckActionContext context, IVTubeStudio vts, IEncoderSurface encoder, ModelPositionTracker tracker)
    : MoveAxisAction(context, vts, encoder, tracker)
{
    protected override double Read(ModelPosition position) => position.X;

    protected override MoveModelRequest Write(double value) => new() { PositionX = value, TimeInSeconds = 0.05d };
}
