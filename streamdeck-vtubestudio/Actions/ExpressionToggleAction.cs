#nullable enable
using System.Linq;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Requests;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StreamDeckLib;
using StreamDeckLib.Models;

namespace Cazzar.StreamDeck.VTubeStudio.Actions;


[StreamDeckAction("dev.cazzar.vtubestudio.expressiontoggle")]
public class ExpressionToggleAction : BaseAction<ExpressionToggleAction.ExpressionTogglePayload>
{
    private readonly ExpressionCache _expressionCache;

    public class ExpressionTogglePayload
    {
        [JsonProperty("expression")]
        public string? Expression { get; set; } = "Cat Tail.exp3.json";
        
        [JsonProperty("fadeTime")]
        public float FadeTime { get; set; } = 0.5f;
    }

    public ExpressionToggleAction(GlobalSettingsManager gsm, VTubeStudioWebsocketClient vts, IStreamDeckConnection isd, ILogger<ExpressionToggleAction> logger, ExpressionCache expressionCache) : base(gsm, vts, isd, logger)
    {
        _expressionCache = expressionCache;
    }
    
protected override void Pressed()
{
    if (!_expressionCache.ModelLoaded)
        return;

    var activate = !_expressionCache.Expressions.FirstOrDefault(e => e.File == Settings.Expression)?.Active ?? false;
    
    Vts.Send(new ExpressionActivationRequest(Settings.Expression, activate, Settings.FadeTime));
    
    Connection.SendMessage(new SetStateRequest
    {
        Payload = new ((uint) (activate ? 1 : 0)),
        Context = ContextId,
    });
}

    public override void Tick()
    {
        base.Tick();
        var activate = !_expressionCache.Expressions.FirstOrDefault(e => e.File == Settings.Expression)?.Active ?? false;

        Connection.SendMessage(new SetStateRequest
        {
            Payload = new ((uint) (activate ? 1 : 0)),
            Context = ContextId,
        });
    }

    protected override object GetClientData() => new
    {
        expressions = _expressionCache.Expressions.DistinctBy(e => e.File).Select(e => new { e.Name, e.File }),
    };

    protected override void Released()
    {
    }
    
    protected override void SettingsUpdated(ExpressionTogglePayload oldSettings, ExpressionTogglePayload newSettings)
    {
    }
}