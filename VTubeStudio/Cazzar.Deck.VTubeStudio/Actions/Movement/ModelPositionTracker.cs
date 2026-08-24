using VTubeStudio.Api;
using VTubeStudio.Api.Models;

namespace Cazzar.Deck.VTubeStudio.Actions.Movement;

public sealed class ModelPositionTracker
{
    public ModelPositionTracker(IVTubeStudio vts)
    {
        vts.ModelMoved += (_, e) => Position = e.Response.Position;
        vts.CurrentModel += (_, e) => Position = e.Response.Position;
    }

    public ModelPosition Position { get; private set; } = new();
}
