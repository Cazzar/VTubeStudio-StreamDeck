namespace Cazzar.Deck.Abstractions.Actions.Handlers;

public interface IPropertyViewHandler
{
    void PropertyViewOpened();
    void PropertyViewClosed();
    void PropertyViewMessage(IPayload body);
}
