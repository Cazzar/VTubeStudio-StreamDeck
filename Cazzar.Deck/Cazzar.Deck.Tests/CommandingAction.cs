using Cazzar.Deck.Abstractions.Actions.Handlers;
using Cazzar.Deck.Abstractions.Actions;
using Cazzar.Deck.Abstractions;
using Cazzar.Deck.Core.Actions;

namespace Cazzar.Deck.Tests;

[DeckAction("test.commanding")]
public class CommandingAction(DeckActionContext context)
    : DeckAction<CommandingAction.Options>(context), IPropertyViewHandler, IPropertyViewCommands
{
    public class Options;

    public List<string> Invoked { get; } = [];

    public string? UnhandledCommand { get; private set; }

    public void PropertyViewOpened()
    {
    }

    public void PropertyViewClosed()
    {
    }

    public void PropertyViewMessage(IPayload body) => PropertyViewCommandRouter.Route(this, body);

    public void Unhandled(string command, IPayload payload) => UnhandledCommand = command;

    [PropertyViewCommand("with-payload")]
    public void WithPayload(IPayload body) =>
        Invoked.Add(body.TryGet<string>("value", out var value) ? $"with-payload:{value}" : "with-payload:none");

    [PropertyViewCommand("no-payload")]
    public void NoPayload() => Invoked.Add("no-payload");

    [PropertyViewCommand("Mixed-Case")]
    public void MixedCase(IPayload body) => Invoked.Add("mixed-case");
}
