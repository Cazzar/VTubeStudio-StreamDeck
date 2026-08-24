using Cazzar.Deck.VTubeStudio.Caches;
using VTubeStudio.Api.Models;
using VTubeStudio.Api.Requests;
using Xunit;

namespace Cazzar.Deck.VTubeStudio.Tests;

public class ExpressionCacheTests
{
    private static (FakeVTubeStudio Vts, ExpressionCache Cache) Cache()
    {
        var vts = new FakeVTubeStudio();

        return (vts, new ExpressionCache(vts));
    }

    private static int Refreshes(FakeVTubeStudio vts) => vts.Sent.Count(r => r is ExpressionStateRequest);

    [Fact]
    public void An_expression_hotkey_fired_outside_the_deck_refreshes_the_cache()
    {
        var (vts, cache) = Cache();

        vts.RaiseExpressionToggled(new() { ModelId = "hk-1", ModelName = "ToggleExpression", ExpressionFile = "smile.exp3.json", Active = true});

        Assert.Contains(cache.For("hk-1"), s=> s is { File: "smile.exp3.json", IsActive: true });
       
        vts.RaiseExpressionToggled(new() { ModelId = "hk-1", ModelName = "ToggleExpression", ExpressionFile = "smile.exp3.json", Active = false});
       
        Assert.Contains(cache.For("hk-1"), s=> s is { File: "smile.exp3.json", IsActive: false });
        
    }

    [Fact]
    public void Multiple_expression_states_can_be_stored()
    {
        var (vts, cache) = Cache();
        
        vts.RaiseExpressionToggled(new() { ModelId = "hk-1", ModelName = "ToggleExpression", ExpressionFile = "smile.exp3.json", Active = true});
        vts.RaiseExpressionToggled(new() { ModelId = "hk-1", ModelName = "ToggleExpression", ExpressionFile = "sad.exp3.json", Active = true});

        Assert.Contains(cache.For("hk-1"), s=> s is { File: "smile.exp3.json", IsActive: true });
        Assert.Contains(cache.For("hk-1"), s=> s is { File: "sad.exp3.json", IsActive: true });
    }
    
    [Fact]
    public void A_manual_refresh_happens()
    {
        var (vts, cache) = Cache();
        var before = Refreshes(vts);
        
        cache.Refresh();
        
        Assert.Equal(before + 1, Refreshes(vts));
    }

    [Fact]
    public void Expression_state_is_kept_per_model()
    {
        var (vts, cache) = Cache();

        vts.RaiseExpressionState(new()
        {
            ModelId = "model-a",
            Expressions = [new Expression { File = "smile.exp3.json", Name = "smile" }],
        });

        Assert.Equal("smile", Assert.Single(cache.For("model-a")).Name);
        Assert.Empty(cache.For("model-b"));
    }
}
