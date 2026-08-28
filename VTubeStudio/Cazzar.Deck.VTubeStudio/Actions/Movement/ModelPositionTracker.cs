using VTubeStudio.Api;
using VTubeStudio.Api.Models;

namespace Cazzar.Deck.VTubeStudio.Actions.Movement;

public sealed class ModelPositionTracker
{
    public ModelPositionTracker(IVTubeStudio vts)
    {
        vts.ModelMoved += (_, e) => Set(e.Response.Position, true);
        vts.CurrentModel += (_, e) => Set(e.Response.Position, e.Response.IsLoaded);
    }

    public ModelPosition Position { get; private set; } = new();

    public bool IsLoaded { get; private set; }

    public event EventHandler? Updated;

    private void Set(ModelPosition position, bool loaded)
    {
        (Position, IsLoaded) = (position, loaded);
        Updated?.Invoke(this, EventArgs.Empty);
    }
}
