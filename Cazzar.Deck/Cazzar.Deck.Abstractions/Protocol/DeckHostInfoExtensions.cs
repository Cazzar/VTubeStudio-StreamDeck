namespace Cazzar.Deck.Abstractions.Protocol;

public static class DeckHostInfoExtensions
{
    public static bool Has(this IDeckHostInfo host, DeckFeature feature) => (host.Features & feature) == feature;
}
