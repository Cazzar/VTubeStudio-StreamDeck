namespace Cazzar.Deck.Abstractions.Actions;

public sealed record DeckActionDescriptor
{
    public required string ActionId { get; init; }
    public required Type ActionType { get; init; }

    public required DeckHost Hosts { get; init; }

    public DeckFeature RequiredFeatures { get; init; } = DeckFeature.None;

    public string? Name { get; init; }
    public string? Tooltip { get; init; }
    public string? Icon { get; init; }
    public string? PropertyView { get; init; }

    public required Func<IServiceProvider, object> Activate { get; init; }
}
