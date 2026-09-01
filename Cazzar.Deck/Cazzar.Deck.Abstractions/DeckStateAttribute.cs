namespace Cazzar.Deck.Abstractions;

[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed class DeckStateAttribute : Attribute
{
    public string? Icon { get; init; }
    public string? Name { get; init; }
}
