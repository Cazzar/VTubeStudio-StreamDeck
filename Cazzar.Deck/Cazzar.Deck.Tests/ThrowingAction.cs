using Cazzar.Deck.Abstractions.Actions.Handlers;
using Cazzar.Deck.Abstractions.Actions;
using Cazzar.Deck.Abstractions;
using Cazzar.Deck.Core.Actions;

namespace Cazzar.Deck.Tests;

[DeckAction("test.throws")]
public class ThrowingAction(DeckActionContext context) : DeckAction<PortableAction.Options>(context), ITickHandler
{
    public override void KeyDown(uint state) => throw new InvalidOperationException("boom");

    public void Tick() => throw new InvalidOperationException("tick boom");
}
