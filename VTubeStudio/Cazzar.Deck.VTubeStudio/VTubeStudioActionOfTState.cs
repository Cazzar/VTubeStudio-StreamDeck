using Cazzar.Deck.Abstractions.Actions;
using VTubeStudio.Api;

namespace Cazzar.Deck.VTubeStudio;

public abstract class VTubeStudioAction<TSettings, TState>(DeckActionContext context, IVTubeStudio vts)
    : VTubeStudioAction<TSettings>(context, vts)
    where TSettings : new()
    where TState : struct, Enum
{
    protected TState CurrentState
    {
        get;
        set
        {
            if (EqualityComparer<TState>.Default.Equals(field, value)) return;

            field = value;
            _ = SetStateAsync(Convert.ToUInt32(value));
        }
    }
}
