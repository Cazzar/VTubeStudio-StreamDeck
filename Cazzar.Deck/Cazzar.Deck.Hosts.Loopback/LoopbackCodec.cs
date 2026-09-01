using Cazzar.Deck.Core.Actions;
using Cazzar.Deck.Abstractions.Protocol.Commands;
using Cazzar.Deck.Abstractions.Protocol.Events;
using Cazzar.Deck.Abstractions.Protocol;

namespace Cazzar.Deck.Hosts.Loopback;

// Records commands rather than rendering them, and refuses the ones this host is pretending not to have.
public sealed class LoopbackCodec(IDeckHostInfo host) : IDeckCodec
{
    private readonly List<IDeckCommand> _commands = [];

    public IReadOnlyList<IDeckCommand> Commands
    {
        get { lock (_commands) return [.. _commands]; }
    }

    public IReadOnlyList<IDeckEvent> Decode(string frame) => [];

    public string? Encode(IDeckCommand command)
    {
        if (!Supported(command)) return null;

        lock (_commands) _commands.Add(command);
        return command.ToString();
    }

    private bool Supported(IDeckCommand command) =>
        ReflectionActionProvider.RequiredFeatures(command.GetType()) is var required &&
        (required & host.Features) == required;
}
