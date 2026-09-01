using Cazzar.Deck.Abstractions.Actions.Handlers;
using Cazzar.Deck.Abstractions.Actions;
using Cazzar.Deck.Abstractions.Protocol;
using Cazzar.Deck.Abstractions;
using Microsoft.Extensions.Logging;
using System.Text.Json.Nodes;
using System.Text.Json;

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

        try
        {
            Settings = settings.As<TSettings>() ?? new ();
        }
        catch (JsonException e)
        {
            Logger.LogWarning(e, "Could not read stored settings; falling back to defaults");
            Settings = new ();
        }

        OnSettingsChanged(previous, Settings);
    }

    protected virtual void OnAppeared() { }
    protected virtual void OnSettingsChanged(TSettings previous, TSettings current) { }

    public virtual void KeyDown(uint state) { }
    public virtual void KeyUp(uint state) { }

    protected ValueTask SaveSettingsAsync(TSettings? settings = default) =>
        Context.Settings.SaveAsync(Ref, DeckJson.ToNode(settings ?? Settings!) ?? new JsonObject());

    protected ValueTask RequestSettingsAsync() => Context.Settings.RequestAsync(Ref);

    private bool _titleWritten;

    /// <summary>Pushes only on change. <c>null</c> clears to the host-configured title.</summary>
    protected string? Title
    {
        get;
        set
        {
            if (_titleWritten && field == value) return;

            (field, _titleWritten) = (value, true);
            _ = SetTitleAsync(value);
        }
    }

    private ValueTask SetTitleAsync(string? title, uint? state = null) => Context.Widget.SetTitleAsync(Ref, title, state);
    protected ValueTask SetImageAsync(string image, uint state = 0) => Context.Widget.SetImageAsync(Ref, image, state);
    protected ValueTask SetStateAsync(uint state) => Context.Widget.SetStateAsync(Ref, state);
    protected ValueTask ShowAlertAsync() =>
        Context.Host.Has(DeckFeature.Alerts) ? Context.Widget.ShowAlertAsync(Ref) : ValueTask.CompletedTask;

    protected ValueTask ShowOkAsync() =>
        Context.Host.Has(DeckFeature.Alerts) ? Context.Widget.ShowOkAsync(Ref) : ValueTask.CompletedTask;

    protected ValueTask SendToPropertyViewAsync(JsonNode payload) => Context.PropertyView.SendAsync(Ref, payload);
}

public abstract class DeckAction<TSettings, TState> : DeckAction<TSettings>
    where TSettings : new()
    where TState : struct, Enum
{
    protected DeckAction(DeckActionContext context) : base(context) { }

    private bool _stateWritten;

    /// <summary>Pushes only on change.</summary>
    protected TState CurrentState
    {
        get;
        set
        {
            if (_stateWritten && EqualityComparer<TState>.Default.Equals(field, value)) return;

            (field, _stateWritten) = (value, true);
            _ = SetStateAsync(Convert.ToUInt32(value));
        }
    }
}
