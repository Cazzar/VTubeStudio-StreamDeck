using Cazzar.Deck.Abstractions.Protocol.Commands;
using Cazzar.Deck.Abstractions.Protocol.Events;

namespace Cazzar.Deck.Abstractions.Protocol;

public interface IDeckCodec
{
    IReadOnlyList<IDeckEvent> Decode(string frame);

    string? Encode(IDeckCommand command);
}
