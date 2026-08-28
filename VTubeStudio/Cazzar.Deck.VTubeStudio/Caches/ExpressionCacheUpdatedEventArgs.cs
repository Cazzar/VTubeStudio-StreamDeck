namespace Cazzar.Deck.VTubeStudio.Caches;

public sealed class ExpressionCacheUpdatedEventArgs(IReadOnlyDictionary<string, IReadOnlyList<ExpressionStatus>> expressions) : EventArgs
{
    public IReadOnlyDictionary<string, IReadOnlyList<ExpressionStatus>> Expressions { get; } = expressions;
}
