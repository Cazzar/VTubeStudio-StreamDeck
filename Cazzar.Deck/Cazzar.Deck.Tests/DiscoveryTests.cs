using Cazzar.Deck.Abstractions;
using Cazzar.Deck.Core.Actions;
using Xunit;

namespace Cazzar.Deck.Tests;

public class DiscoveryTests
{
    [Fact]
    public async Task Stream_deck_registers_everything()
    {
        await using var harness = new Harness(DeckHost.StreamDeck);

        Assert.NotNull(harness.Catalog.Find("test.portable"));
        Assert.NotNull(harness.Catalog.Find("test.dial"));
        Assert.NotNull(harness.Catalog.Find("test.policy"));
    }

    [Fact]
    public async Task Implementing_an_encoder_handler_excludes_a_host_without_encoders()
    {
        await using var harness = new Harness(DeckHost.CreatorCentral, DeckFeature.MultiState | DeckFeature.GlobalSettings);

        Assert.Null(harness.Catalog.Find("test.dial"));
        Assert.NotNull(harness.Catalog.Find("test.portable"));
    }

    [Fact]
    public async Task A_declared_host_restriction_is_honoured()
    {
        await using var harness = new Harness(DeckHost.CreatorCentral, DeckFeature.MultiState | DeckFeature.GlobalSettings);

        Assert.Null(harness.Catalog.Find("test.policy"));
    }

    [Fact]
    public async Task An_omitted_id_is_derived_from_the_class_name()
    {
        await using var harness = new Harness(DeckHost.StreamDeck);

        Assert.NotNull(harness.Catalog.Find("derivedid"));
    }

    [Fact]
    public void Encoder_requirement_is_derived_from_the_interface_not_declared()
    {
        Assert.Equal(DeckFeature.Encoder, ReflectionActionProvider.RequiredFeatures(typeof(DialAction)));
        Assert.Equal(DeckFeature.None, ReflectionActionProvider.RequiredFeatures(typeof(PortableAction)));
    }
}
