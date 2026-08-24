using Cazzar.Deck.Abstractions.Protocol.Commands;

namespace Cazzar.Deck.Abstractions.Protocol;

public interface IDeckHandshake
{
    IEnumerable<IDeckCommand> OpeningMessages();
}
