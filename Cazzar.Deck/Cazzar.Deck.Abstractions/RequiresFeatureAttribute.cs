namespace Cazzar.Deck.Abstractions;

[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, Inherited = true)]
public sealed class RequiresFeatureAttribute(DeckFeature feature) : Attribute
{
    public DeckFeature Feature { get; } = feature;
}
