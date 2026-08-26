using System.Collections.Concurrent;
using VTubeStudio.Api;
using VTubeStudio.Api.Requests;

namespace Cazzar.Deck.VTubeStudio.Caches;

public sealed class ExpressionCache
{
    private readonly IVTubeStudio _vts;
    private readonly ConcurrentDictionary<string, List<ExpressionStatus>> _expressions = new();

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
            
            Updated?.Invoke(this, new(Expressions));
        };

        vts.ExpressionToggled += (_, e) =>
        {
            if (string.IsNullOrEmpty(e.Response.ModelId)) return;

            if (!_expressions.TryGetValue(e.Response.ModelId, out var modelExpressions)) 
                _expressions[e.Response.ModelId] = modelExpressions = new ();

            var expressionStatus = new ExpressionStatus
            {
                Name = e.Response.ExpressionName,
                FileName = e.Response.ExpressionFile,
                IsActive = e.Response.Active
            };
            
            var existingExpression = modelExpressions.FirstOrDefault(x => x.FileName == expressionStatus.File);
            if (existingExpression != null)
                existingExpression.IsActive = expressionStatus.IsActive;
            else
                modelExpressions.Add(expressionStatus);
            
            Updated?.Invoke(this, new(Expressions));
        };
    }

    private IReadOnlyDictionary<string, List<ExpressionStatus>> Expressions => _expressions.AsReadOnly();

    public event EventHandler<ExpressionCacheUpdatedEventArgs>? Updated;

    public IReadOnlyList<ExpressionStatus> For(string? modelId) =>
        modelId is not null && _expressions.TryGetValue(modelId, out var expressions) ? expressions.AsReadOnly() : [];

    public void Refresh() => _vts.Send(new ExpressionStateRequest());
}
