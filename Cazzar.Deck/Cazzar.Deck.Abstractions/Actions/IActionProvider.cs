namespace Cazzar.Deck.Abstractions.Actions;

public interface IActionProvider
{
    IEnumerable<DeckActionDescriptor> GetActions();
}
