namespace Cazzar.Deck.Abstractions.Actions.Handlers;

public interface IActionFaultObserver
{
    void Faulted(ActionRef @ref, Exception exception);
}
