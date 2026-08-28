using Cazzar.Deck.Abstractions.Actions;
using VTubeStudio.Api;

namespace Cazzar.Deck.VTubeStudio;

public abstract class VTubeStudioAction<TSettings, TState>(DeckActionContext context, IVTubeStudio vts)
    : VTubeStudioAction<TSettings>(context, vts)
    where TSettings : new()
    where TState : struct, Enum
{
    private bool _stateWritten;

    protected TState CurrentState
    {
        get;
        set
        {
            if (_stateWritten && EqualityComparer<TState>.Default.Equals(field, value)) return;

            (field, _stateWritten) = (value, true);
            _ = SetStateAsync(Convert.ToUInt32(value));
        }
    }
}
