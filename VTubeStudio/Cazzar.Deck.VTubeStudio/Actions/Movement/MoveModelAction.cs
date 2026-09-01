using Cazzar.Deck.Abstractions.Actions;
using Cazzar.Deck.Abstractions;
using VTubeStudio.Api.Requests;
using VTubeStudio.Api;

namespace Cazzar.Deck.VTubeStudio.Actions.Movement;

[DeckAction(Name = "Move Model", Tooltip = "VTubeStudio [Move Model]",
    Icon = "vts_logo_transparent", PropertyView = "MoveModel")]
public sealed class MoveModelAction(
    DeckActionContext context,
    IVTubeStudio vts,
    ModelPositionTracker tracker) : VTubeStudioAction<MoveSettings>(context, vts)
{
    protected override void Pressed() => Vts.Send(new MoveModelRequest
    {
        PositionX = Settings.PosX,
        PositionY = Settings.PosY,
        RelativeMove = Settings.Relative,
        Rotation = Settings.Rotation,
        Size = Settings.Size,
        TimeInSeconds = Settings.Seconds,
    });

    [PropertyViewCommand("get-params")]
    public void CaptureCurrentPosition(IPayload body)
    {
        Settings.PosX = tracker.Position.X;
        Settings.PosY = tracker.Position.Y;
        Settings.Rotation = tracker.Position.Rotation;
        Settings.Size = tracker.Position.Size;
        Settings.Relative = false;

        _ = SaveSettingsAsync();
        _ = UpdateClientAsync();
    }
}
