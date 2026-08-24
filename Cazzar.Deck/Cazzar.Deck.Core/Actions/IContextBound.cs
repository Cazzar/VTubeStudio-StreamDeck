using Cazzar.Deck.Abstractions;
namespace Cazzar.Deck.Core.Actions;

public interface IContextBound
{
    void Bind(ActionRef @ref);
}
