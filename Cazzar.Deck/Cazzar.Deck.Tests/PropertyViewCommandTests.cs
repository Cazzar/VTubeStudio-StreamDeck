using Cazzar.Deck.Abstractions.Protocol.Events;
using Cazzar.Deck.Abstractions;
using Cazzar.Deck.Core;
using System.Text.Json.Nodes;
using Xunit;

namespace Cazzar.Deck.Tests;

public class PropertyViewCommandTests
{
    private static readonly ActionRef Ref = new("ctx-1", "test.commanding");

    private static IPayload Body(string json) => JsonPayload.From(JsonNode.Parse(json));

    private static CommandingAction Send(Harness harness, params string[] bodies)
    {
        harness.Dispatcher.Dispatch(new ActionAppeared(Ref, IPayload.Empty));

        foreach (var body in bodies)
            harness.Dispatcher.Dispatch(new PropertyViewMessage(Ref, Body(body)));

        CommandingAction? action = null;
        harness.Instances.Invoke<CommandingAction>(Ref, a => action = a);

        return action ?? throw new InvalidOperationException("the action was never created");
    }

    [Fact]
    public async Task A_command_reaches_the_method_that_declares_it()
    {
        await using var harness = new Harness(DeckHost.StreamDeck);

        var action = Send(harness, """{ "command": "with-payload", "value": "hello" }""");

        Assert.Equal(["with-payload:hello"], action.Invoked);
        Assert.Empty(harness.Faults.All);
    }

    [Fact]
    public async Task A_command_on_a_parameterless_method_is_dispatched()
    {
        await using var harness = new Harness(DeckHost.StreamDeck);

        var action = Send(harness, """{ "command": "no-payload" }""");

        Assert.Equal(["no-payload"], action.Invoked);
    }

    [Fact]
    public async Task Command_matching_ignores_case()
    {
        await using var harness = new Harness(DeckHost.StreamDeck);

        var action = Send(harness, """{ "command": "MIXED-CASE" }""");

        Assert.Equal(["mixed-case"], action.Invoked);
    }

    [Fact]
    public async Task An_unknown_command_falls_through_to_unhandled()
    {
        await using var harness = new Harness(DeckHost.StreamDeck);

        var action = Send(harness, """{ "command": "nope" }""");

        Assert.Empty(action.Invoked);
        Assert.Equal("nope", action.UnhandledCommand);
    }

    [Fact]
    public async Task A_body_without_a_command_is_ignored()
    {
        await using var harness = new Harness(DeckHost.StreamDeck);

        var action = Send(harness, """{ "value": "hello" }""");

        Assert.Empty(action.Invoked);
        Assert.Null(action.UnhandledCommand);
    }
}
