using Cazzar.Deck.Abstractions.Protocol.Events;
using Cazzar.Deck.Abstractions.Protocol;
using Cazzar.Deck.Abstractions;
using Cazzar.Deck.Hosts.CreatorCentral;
using Cazzar.Deck.Hosts.StreamDeck;
using Xunit;

namespace Cazzar.Deck.VTubeStudio.Tests;

public class ActionPortabilityTests
{
    private static readonly string[] EncoderActions =
    [
        "dev.cazzar.vtubestudio.zoommodel",
        "dev.cazzar.vtubestudio.movemodel.x",
        "dev.cazzar.vtubestudio.movemodel.y",
        "dev.cazzar.vtubestudio.rotatemodel",
    ];

    private static readonly string[] PortableActions =
    [
        "dev.cazzar.vtubestudio.triggerhotkey",
        "dev.cazzar.vtubestudio.changemodel",
        "dev.cazzar.vtubestudio.reloadmodel",
        "dev.cazzar.vtubestudio.movemodel",
        "dev.cazzar.vtubestudio.scalemodel",
        "dev.cazzar.vtubestudio.holdtransform",
        "dev.cazzar.vtubestudio.toggleexpression",
        "dev.cazzar.vtubestudio.holdexpression",
    ];

    [Fact]
    public async Task Stream_deck_gets_every_action()
    {
        await using var harness = new PluginHarness(new StreamDeckHostInfo());

        foreach (var id in PortableActions.Concat(EncoderActions))
            Assert.True(harness.Catalog.Find(id) is not null, $"{id} should be registered on Stream Deck");
    }

    [Fact]
    public async Task Creator_central_gets_the_portable_actions_only()
    {
        await using var harness = new PluginHarness(new CreatorCentralHostInfo());

        foreach (var id in PortableActions)
            Assert.True(harness.Catalog.Find(id) is not null, $"{id} should be registered on Creator Central");

        foreach (var id in EncoderActions)
            Assert.True(harness.Catalog.Find(id) is null, $"{id} needs an encoder and must not register");
    }

    [Theory]
    [MemberData(nameof(EveryActionOnEveryHost))]
    public async Task Every_registered_action_survives_a_full_lifecycle(string hostName, string actionId)
    {
        IDeckHostInfo host = hostName == "streamdeck" ? new StreamDeckHostInfo() : new CreatorCentralHostInfo();
        await using var harness = new PluginHarness(host);

        var @ref = new ActionRef("ctx-1", actionId);

        harness.Dispatcher.Dispatch(new ActionAppeared(@ref, IPayload.Empty));
        harness.Dispatcher.Dispatch(new SettingsReceived(@ref, IPayload.Empty));
        harness.Dispatcher.Dispatch(new PropertyViewOpened(@ref));
        harness.Dispatcher.Dispatch(new KeyPressed(@ref, 0));
        harness.Dispatcher.Dispatch(new KeyReleased(@ref, 0));
        harness.Dispatcher.Dispatch(new PropertyViewClosed(@ref));
        harness.Dispatcher.Dispatch(new ActionDisappeared(@ref));

        Assert.Empty(harness.Faults.All.Select(f => f.Exception.ToString()));
    }

    public static TheoryData<string, string> EveryActionOnEveryHost()
    {
        var data = new TheoryData<string, string>();

        foreach (var id in PortableActions.Concat(EncoderActions))
            data.Add("streamdeck", id);

        foreach (var id in PortableActions)
            data.Add("creatorcentral", id);

        return data;
    }
}
