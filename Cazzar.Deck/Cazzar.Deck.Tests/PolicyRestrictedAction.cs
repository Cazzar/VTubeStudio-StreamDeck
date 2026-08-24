using Cazzar.Deck.Abstractions.Actions;
using Cazzar.Deck.Abstractions;
using Cazzar.Deck.Core.Actions;

namespace Cazzar.Deck.Tests;

[DeckAction("test.policy", Hosts = DeckHost.StreamDeck)]
public class PolicyRestrictedAction(DeckActionContext context) : DeckAction<PortableAction.Options>(context);
