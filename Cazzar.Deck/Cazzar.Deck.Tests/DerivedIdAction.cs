using Cazzar.Deck.Abstractions.Actions;
using Cazzar.Deck.Abstractions;
using Cazzar.Deck.Core.Actions;

namespace Cazzar.Deck.Tests;

[DeckAction]
public class DerivedIdAction(DeckActionContext context) : DeckAction<DerivedIdAction.Options>(context)
{
    public class Options;
}
