using System.Collections.Concurrent;
using VTubeStudio.Api;
using VTubeStudio.Api.Requests;

namespace Cazzar.Deck.VTubeStudio.Caches;

public sealed class ExpressionCache
{
    private readonly IVTubeStudio _vts;
    private readonly ConcurrentDictionary<string, IReadOnlyList<ExpressionStatus>> _expressions = new();

    public ExpressionCache(IVTubeStudio vts)
    {
        _vts = vts;

        vts.ExpressionState += (_, e) =>
        {
            if (string.IsNullOrEmpty(e.Response.ModelId)) return;

            _expressions[e.Response.ModelId] =
            [
                .. e.Response.Expressions.Select(e => new ExpressionStatus
                {
                    Name = e.Name,
                    FileName = e.File,
                    IsActive = e.Active
                }),
            ];
            
             Task.Run(() => Updated?.Invoke(this, new(Expressions)));
        };

        vts.ExpressionToggled += (_, e) =>
        {
            if (string.IsNullOrEmpty(e.Response.ModelId)) return;

            var toggled = new ExpressionStatus
            {
                Name = e.Response.ExpressionName,
                FileName = e.Response.ExpressionFile,
                IsActive = e.Response.Active
            };

            var current = For(e.Response.ModelId);
            _expressions[e.Response.ModelId] = current.Any(x => x.File == toggled.File)
                ? [.. current.Select(x => x.File == toggled.File ? x with { IsActive = toggled.IsActive } : x)]
                : [.. current, toggled];

            Task.Run(() => Updated?.Invoke(this, new(Expressions)));
        };
    }

    private IReadOnlyDictionary<string, IReadOnlyList<ExpressionStatus>> Expressions => _expressions;

    public event EventHandler<ExpressionCacheUpdatedEventArgs>? Updated;

    public IReadOnlyList<ExpressionStatus> For(string? modelId) =>
        modelId is not null && _expressions.TryGetValue(modelId, out var expressions) ? expressions : [];

    public void Refresh() => _vts.Send(new ExpressionStateRequest());
}
