using Cazzar.Deck.Abstractions.Actions;
using VTubeStudio.Api;

namespace Cazzar.Deck.VTubeStudio;

public abstract class VTubeStudioAction<TSettings, TState>(DeckActionContext context, IVTubeStudio vts)
    : VTubeStudioAction<TSettings>(context, vts)
    where TSettings : new()
    where TState : struct, Enum
{
    public TState CurrentState { get; private set; }

    protected void SetState(TState state)
    {
        CurrentState = state;
        _ = SetStateAsync(Convert.ToUInt32(state));
    }
}
