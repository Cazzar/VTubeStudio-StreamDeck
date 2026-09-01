namespace Cazzar.Deck.Shared;

static class DeckHostFlags
{
    public const int EncoderFeature = 1;

    public const int StreamDeck = 1;
    public const int CreatorCentral = 2;

    public const int EncoderHosts = StreamDeck;

    public static readonly (int Flag, string Name)[] Hosts =
    [
        (StreamDeck, nameof(StreamDeck)),
        (CreatorCentral, nameof(CreatorCentral)),
    ];

    public static readonly int All = Hosts.Aggregate(0, (all, host) => all | host.Flag);

    public static IEnumerable<string> Names(int flags) =>
        Hosts.Where(h => (flags & h.Flag) == h.Flag).Select(h => h.Name);
}
