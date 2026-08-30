using Cazzar.Deck.Abstractions.Protocol.Events;
using Cazzar.Deck.Abstractions.Protocol;
using Cazzar.Deck.Abstractions;
using Cazzar.Deck.Core;
using Cazzar.Deck.Hosts.StreamDeck;
using System.Text.Json.Nodes;
using Xunit;

namespace Cazzar.Deck.VTubeStudio.Tests;

public class VtsInfoPayloadTests
{
    // VtsInfoPayload used to be nested in the generic action, so every TSettings produced a distinct
    // closed type and only the one the context happened to register could be deserialised.
    [Fact]
    public async Task Connection_details_reach_the_client_from_any_action()
    {
        await using var harness = new PluginHarness(new StreamDeckHostInfo());

        foreach (var actionId in new[]
                 {
                     "dev.cazzar.vtubestudio.toggleexpression",
                     "dev.cazzar.vtubestudio.triggerhotkey",
                     "dev.cazzar.vtubestudio.movemodel",
                 })
        {
            var @ref = new ActionRef($"ctx-{actionId}", actionId);
            harness.Dispatcher.Dispatch(new ActionAppeared(@ref, IPayload.Empty));
            harness.Dispatcher.Dispatch(new PropertyViewMessage(@ref, JsonPayload.From(new JsonObject
            {
                ["command"] = "set-vtsinfo",
                ["payload"] = new JsonObject { ["host"] = "10.0.0.5", ["port"] = 9001 },
            })));

            Assert.Equal(("10.0.0.5", (ushort)9001), harness.Vts.Connection);
        }

        Assert.Empty(harness.Faults.All.Select(f => f.Exception.ToString()));
    }
}
