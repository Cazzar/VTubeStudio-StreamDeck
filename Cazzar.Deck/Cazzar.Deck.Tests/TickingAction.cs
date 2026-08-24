using Cazzar.Deck.Abstractions.Actions.Handlers;
using Cazzar.Deck.Abstractions.Actions;
using Cazzar.Deck.Abstractions;
using Cazzar.Deck.Core.Actions;

namespace Cazzar.Deck.Tests;

[DeckAction("test.ticks")]
public class TickingAction(DeckActionContext context) : DeckAction<PortableAction.Options>(context), ITickHandler
{
    public int Ticks { get; private set; }

    public void Tick() => Ticks++;
}
