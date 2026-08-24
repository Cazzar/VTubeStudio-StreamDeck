using Cazzar.Deck.Abstractions.Actions;
using Cazzar.Deck.Abstractions;
using Cazzar.Deck.Core.Actions;

namespace Cazzar.Deck.Tests;

[DeckAction("test.portable")]
public class PortableAction(DeckActionContext context) : DeckAction<PortableAction.Options>(context)
{
    public class Options
    {
        public string Name { get; set; } = string.Empty;
    }

    public bool Pressed { get; private set; }

    public override void KeyDown(uint state) => Pressed = true;
}
