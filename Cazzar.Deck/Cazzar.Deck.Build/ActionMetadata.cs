namespace Cazzar.Deck.Build;

public sealed record ActionMetadata
{
    public required string ActionId { get; init; }
    public required string TypeName { get; init; }
    public string Hosts { get; init; } = "All";
    public string? Name { get; init; }
    public string? Tooltip { get; init; }
    public string? Icon { get; init; }
    public string? PropertyView { get; init; }
    public bool NeedsEncoder { get; init; }
    public IReadOnlyList<StateMetadata> States { get; init; } = [];
    public bool AutomaticStates { get; init; }

    public bool RunsOn(string host) =>
        (Hosts == "All" || Hosts.Split(',').Any(h => h.Trim() == host)) &&
        (!NeedsEncoder || host == "StreamDeck");
}
