using Cazzar.Deck.Abstractions.Actions.Handlers;
using Cazzar.Deck.Abstractions.Actions;
using Cazzar.Deck.Abstractions.Protocol;
using Cazzar.Deck.Abstractions;
using Microsoft.Extensions.Logging;
using System.Text.Json.Nodes;

namespace Cazzar.Deck.Core.Actions;

public abstract class DeckAction<TSettings> : IContextBound, ISettingsHandler, IKeyHandler
    where TSettings : new()
{
    protected DeckAction(DeckActionContext context)
    {
        Context = context;
        Logger = context.LoggerFactory.CreateLogger(GetType());
    }

    protected DeckActionContext Context { get; }

    protected ILogger Logger { get; }

    public ActionRef Ref { get; private set; }

    public TSettings Settings { get; private set; } = new();

    void IContextBound.Bind(ActionRef @ref)
    {
        Ref = @ref;
        OnAppeared();
    }

    public virtual void GotSettings(IPayload settings)
    {
        var previous = Settings;
        Settings = settings.As<TSettings>() ?? new TSettings();
        OnSettingsChanged(previous, Settings);
    }

    protected virtual void OnAppeared() { }
    protected virtual void OnSettingsChanged(TSettings previous, TSettings current) { }

    public virtual void KeyDown(uint state) { }
    public virtual void KeyUp(uint state) { }

    protected ValueTask SaveSettingsAsync(TSettings? settings = default) =>
        Context.Settings.SaveAsync(Ref, DeckJson.ToNode(settings ?? Settings!) ?? new JsonObject());

    protected ValueTask RequestSettingsAsync() => Context.Settings.RequestAsync(Ref);

    protected ValueTask SetTitleAsync(string? title, uint state = 0) => Context.Widget.SetTitleAsync(Ref, title, state);
    protected ValueTask SetImageAsync(string image, uint state = 0) => Context.Widget.SetImageAsync(Ref, image, state);
    protected ValueTask SetStateAsync(uint state) => Context.Widget.SetStateAsync(Ref, state);
    protected ValueTask ShowAlertAsync() =>
        Context.Host.Has(DeckFeature.Alerts) ? Context.Widget.ShowAlertAsync(Ref) : ValueTask.CompletedTask;

    protected ValueTask ShowOkAsync() =>
        Context.Host.Has(DeckFeature.Alerts) ? Context.Widget.ShowOkAsync(Ref) : ValueTask.CompletedTask;

    protected ValueTask SendToPropertyViewAsync(JsonNode payload) => Context.PropertyView.SendAsync(Ref, payload);
}
