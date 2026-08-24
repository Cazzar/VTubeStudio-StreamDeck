using Cazzar.Deck.Abstractions.Actions;
using Cazzar.Deck.Abstractions;

namespace Cazzar.Deck.Core.Actions;

public static class PropertyViewCommandRouter
{
    private static Func<IPropertyViewCommands, string, IPayload, bool>? _dispatch;

    public static void Use(Func<IPropertyViewCommands, string, IPayload, bool> dispatch) => _dispatch = dispatch;

    public static bool Route(IPropertyViewCommands target, IPayload body)
    {
        if (!body.TryGet<string>("command", out var command) || string.IsNullOrEmpty(command)) return false;

        if (_dispatch?.Invoke(target, command.ToLowerInvariant(), body) is true) return true;

        target.Unhandled(command, body);
        return false;
    }
}
