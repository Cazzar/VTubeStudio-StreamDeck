namespace Cazzar.Deck.VTubeStudio.Caches;

public sealed class ExpressionCacheUpdatedEventArgs(IReadOnlyDictionary<string, List<ExpressionStatus>> expressions) : EventArgs
{
    public IReadOnlyDictionary<string, List<ExpressionStatus>> Expressions { get; } = expressions;
}
