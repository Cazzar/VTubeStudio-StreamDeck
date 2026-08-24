using Cazzar.Deck.Abstractions.Actions.Handlers;
using Cazzar.Deck.Abstractions;

namespace Cazzar.Deck.Hosts.Loopback;

public sealed class RecordedFaults : IActionFaultObserver
{
    private readonly List<(ActionRef Ref, Exception Exception)> _faults = [];

    public IReadOnlyList<(ActionRef Ref, Exception Exception)> All
    {
        get { lock (_faults) return [.. _faults]; }
    }

    public void Faulted(ActionRef @ref, Exception exception)
    {
        lock (_faults) _faults.Add((@ref, exception));
    }
}
