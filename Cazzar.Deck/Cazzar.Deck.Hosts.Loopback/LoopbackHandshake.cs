using Cazzar.Deck.Abstractions.Protocol.Commands;
using Cazzar.Deck.Abstractions.Protocol;

namespace Cazzar.Deck.Hosts.Loopback;

public sealed class LoopbackHandshake : IDeckHandshake
{
    public IEnumerable<IDeckCommand> OpeningMessages() => [];
}
