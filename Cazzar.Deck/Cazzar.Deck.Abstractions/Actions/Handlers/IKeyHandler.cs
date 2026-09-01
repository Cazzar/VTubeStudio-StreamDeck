namespace Cazzar.Deck.Abstractions.Actions.Handlers;

public interface IKeyHandler
{
    void KeyDown(uint state);
    void KeyUp(uint state);
}
