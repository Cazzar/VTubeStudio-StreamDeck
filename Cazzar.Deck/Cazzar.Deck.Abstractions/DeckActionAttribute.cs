namespace Cazzar.Deck.Abstractions;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class DeckActionAttribute : Attribute
{
    public DeckActionAttribute() { }

    public DeckActionAttribute(string actionId) => ActionId = actionId;

    public string? ActionId { get; }

    public DeckHost Hosts { get; init; } = DeckHost.All;

    public string? Name { get; init; }
    public string? Tooltip { get; init; }
    public string? Icon { get; init; }
    public string? PropertyView { get; init; }

    public bool AutomaticStates { get; init; }
}
