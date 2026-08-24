using Cazzar.Deck.Abstractions.Actions;
using Cazzar.Deck.Abstractions;
using VTubeStudio.Api.Models;
using VTubeStudio.Api.Requests;
using VTubeStudio.Api;

namespace Cazzar.Deck.VTubeStudio.Actions.Movement;

[DeckAction(Name = "Hold Transform", Tooltip = "VTubeStudio [Hold Transform]",
    Icon = "vts_logo_transparent", PropertyView = "HoldTransform")]
public sealed class HoldTransformAction(
    DeckActionContext context,
    IVTubeStudio vts,
    ModelPositionTracker tracker) : VTubeStudioAction<MoveSettings>(context, vts)
{
    private ModelPosition _restoreTo = new();
    private bool _held;

    protected override void Pressed()
    {
        // Capture before moving, or the restore target becomes the transform we are about to apply.
        if (!_held) _restoreTo = tracker.Position;
        _held = true;

        Vts.Send(new MoveModelRequest
        {
            PositionX = Settings.PosX,
            PositionY = Settings.PosY,
            RelativeMove = false,
            Rotation = Settings.Rotation,
            Size = Settings.Size,
            TimeInSeconds = Settings.Seconds,
        });
    }

    protected override void Released()
    {
        if (!_held) return;

        _held = false;

        Vts.Send(new MoveModelRequest
        {
            PositionX = _restoreTo.X,
            PositionY = _restoreTo.Y,
            RelativeMove = false,
            Rotation = _restoreTo.Rotation,
            Size = _restoreTo.Size,
            TimeInSeconds = Settings.Seconds,
        });
    }

    [PropertyViewCommand("get-params")]
    public void CaptureCurrentPosition(IPayload body)
    {
        Settings.PosX = tracker.Position.X;
        Settings.PosY = tracker.Position.Y;
        Settings.Rotation = tracker.Position.Rotation;
        Settings.Size = tracker.Position.Size;

        _ = SaveSettingsAsync();
        _ = UpdateClientAsync();
    }
}
