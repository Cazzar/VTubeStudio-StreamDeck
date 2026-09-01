using Cazzar.Deck.Abstractions.Protocol.Commands;
using Cazzar.Deck.Abstractions.Protocol;

namespace Cazzar.Deck.Core.Protocol;

public sealed class DeckHandshake(IDeckLaunchOptions options) : IDeckHandshake
{
    public IEnumerable<IDeckCommand> OpeningMessages()
    {
        yield return new Register(options.Uuid, options.RegisterEvent);
        yield return new RequestGlobalSettings();
    }
}
