using Cazzar.Deck.Abstractions;
using Cazzar.Deck.Shared;
using Xunit;

namespace Cazzar.Deck.Tests;

public class MirroredConstantTests
{
    private static IEnumerable<T> Flags<T>(params T[] aggregates) where T : struct, Enum =>
        Enum.GetValues<T>().Where(v => Convert.ToInt32(v) != 0 && !aggregates.Contains(v));

    [Fact]
    public void Deck_host_flags_mirror_the_host_enum()
    {
        Assert.Equal((int)DeckHost.StreamDeck, DeckHostFlags.StreamDeck);
        Assert.Equal((int)DeckHost.CreatorCentral, DeckHostFlags.CreatorCentral);
        Assert.Equal((int)DeckHost.All, DeckHostFlags.All);
    }

    [Fact]
    public void Deck_host_flag_names_cover_every_declared_host()
    {
        Assert.Equal(
            Flags(DeckHost.All).Select(h => h.ToString()).OrderBy(n => n),
            DeckHostFlags.Names(DeckHostFlags.All).OrderBy(n => n));
    }

    [Fact]
    public void The_encoder_feature_constant_mirrors_the_feature_enum()
    {
        Assert.Equal((int)DeckFeature.Encoder, DeckHostFlags.EncoderFeature);
    }

    [Fact]
    public void Encoder_hosts_are_the_hosts_that_advertise_an_encoder()
    {
        Assert.Equal((int)DeckHost.StreamDeck, DeckHostFlags.EncoderHosts);
    }

    [Fact]
    public void Deck_feature_all_covers_every_declared_feature()
    {
        foreach (var feature in Flags(DeckFeature.All))
            Assert.True(DeckFeature.All.HasFlag(feature), $"DeckFeature.All is missing {feature}");
    }

    [Fact]
    public void Deck_host_all_covers_every_declared_host()
    {
        foreach (var host in Flags(DeckHost.All))
            Assert.True(DeckHost.All.HasFlag(host), $"DeckHost.All is missing {host}");
    }
}
