namespace Cazzar.Deck.Build;

public sealed record StateMetadata
{
    public required uint Value { get; init; }
    public string? Icon { get; init; }
    public string? Name { get; init; }
}
