using Cazzar.Deck.Abstractions.Protocol.Commands;
using Cazzar.Deck.Abstractions.Protocol.Events;
using Cazzar.Deck.Abstractions;
using Cazzar.Deck.Core;
using System.Text.Json.Nodes;
using Xunit;

namespace Cazzar.Deck.Tests;

public class LifecycleTests
{
    private static IPayload Settings(string json) => JsonPayload.From(JsonNode.Parse(json));

    [Fact]
    public async Task An_appearing_action_receives_its_settings_and_its_placement()
    {
        await using var harness = new Harness(DeckHost.StreamDeck);
        var @ref = new ActionRef("ctx-1", "test.portable");

        harness.Dispatcher.Dispatch(new ActionAppeared(@ref, Settings("""{ "name": "hello" }""")));
        harness.Dispatcher.Dispatch(new KeyPressed(@ref, 0));

        Assert.Empty(harness.Faults.All);
    }

    [Fact]
    public async Task A_throwing_action_is_contained_and_reported()
    {
        await using var harness = new Harness(DeckHost.StreamDeck);
        var @ref = new ActionRef("ctx-1", "test.throws");

        harness.Dispatcher.Dispatch(new ActionAppeared(@ref, IPayload.Empty));
        harness.Dispatcher.Dispatch(new KeyPressed(@ref, 0));

        var fault = Assert.Single(harness.Faults.All);
        Assert.Equal("ctx-1", fault.Ref.ContextId);
        Assert.Equal("boom", fault.Exception.Message);
    }

    [Fact]
    public async Task One_action_that_throws_on_tick_does_not_stop_the_others()
    {
        await using var harness = new Harness(DeckHost.StreamDeck);

        harness.Dispatcher.Dispatch(new ActionAppeared(new("ctx-throws", "test.throws"), IPayload.Empty));
        harness.Dispatcher.Dispatch(new ActionAppeared(new("ctx-ticks", "test.ticks"), IPayload.Empty));
        harness.Dispatcher.Dispatch(new ActionAppeared(new("ctx-quiet", "test.portable"), IPayload.Empty));

        harness.Instances.Tick();
        harness.Instances.Tick();

        Assert.Equal(2, harness.Faults.All.Count);
        Assert.Contains(harness.Codec.Commands, c => c is ShowAlert);
    }

    [Fact]
    public async Task A_disappearing_action_is_disposed_and_stops_receiving_events()
    {
        await using var harness = new Harness(DeckHost.StreamDeck);
        var @ref = new ActionRef("ctx-1", "test.ticks");

        harness.Dispatcher.Dispatch(new ActionAppeared(@ref, IPayload.Empty));
        harness.Dispatcher.Dispatch(new ActionDisappeared(@ref));
        harness.Instances.Tick();

        Assert.Empty(harness.Faults.All);
    }

    [Fact]
    public async Task Events_for_an_unknown_context_are_ignored_rather_than_thrown()
    {
        await using var harness = new Harness(DeckHost.StreamDeck);

        harness.Dispatcher.Dispatch(new KeyPressed(new("never-appeared", "test.portable"), 0));

        Assert.Empty(harness.Faults.All);
    }

    [Fact]
    public async Task Calling_an_unsupported_surface_throws_rather_than_silently_doing_nothing()
    {
        await using var harness = new Harness(DeckHost.CreatorCentral, DeckFeature.MultiState | DeckFeature.GlobalSettings);
        var client = harness.Client;

        Assert.Throws<NotSupportedException>(() =>
            client.Send(new SetFeedbackLayout(new("ctx-1", "a"), "$B1")));
    }
}
