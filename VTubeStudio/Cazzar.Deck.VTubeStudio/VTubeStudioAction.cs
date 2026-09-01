using Cazzar.Deck.Abstractions.Actions.Handlers;
using Cazzar.Deck.Abstractions.Actions;
using Cazzar.Deck.Abstractions;
using Cazzar.Deck.Core.Actions;
using System.Text.Json.Nodes;
using VTubeStudio.Api;

namespace Cazzar.Deck.VTubeStudio;

public abstract class VTubeStudioAction<TSettings, TState>(DeckActionContext context, IVTubeStudio vts)
    : DeckAction<TSettings, TState>(context), IPropertyViewHandler, IPropertyViewCommands, ITickHandler
    where TSettings : new()
    where TState : struct, Enum
{
    protected IVTubeStudio Vts { get; } = vts;

    private bool _propertyViewOpen;

    public override void KeyDown(uint state)
    {
        Vts.EnsureConnected();

        if (!Vts.IsAuthenticated)
        {
            _ = ShowAlertAsync();
            return;
        }

        Pressed();
    }

    public override void KeyUp(uint state) => Released();

    protected abstract void Pressed();

    protected virtual void Released()
    {
    }

    protected static JsonArray Choices<T>(IEnumerable<T> items, Func<T, string> id, Func<T, string> name) =>
        [..items.Select(JsonNode (i) => new JsonObject { ["id"] = id(i), ["name"] = name(i) }).ToArray()];

    protected virtual JsonNode ClientData() => new JsonObject { ["connected"] = Vts.IsAuthenticated };

    protected ValueTask UpdateClientAsync() => SendToPropertyViewAsync(ClientData());

    public void PropertyViewOpened()
    {
        _propertyViewOpen = true;
        _ = UpdateClientAsync();
    }

    public void PropertyViewClosed() => _propertyViewOpen = false;

    public void PropertyViewMessage(IPayload body) => PropertyViewCommandRouter.Route(this, body);

    [PropertyViewCommand("refresh")]
    public virtual void Refresh(IPayload body) => _ = UpdateClientAsync();

    [PropertyViewCommand("force-reconnect")]
    public void ForceReconnect(IPayload body) => Vts.Reconnect();

    [PropertyViewCommand("set-vtsinfo")]
    public void SetVtsInfo(IPayload body)
    {
        if (!body.TryGet<VtsInfoPayload>("payload", out var info) || info is null) return;

        Vts.SetConnection(info.Host, info.Port);
    }

    public virtual void Tick()
    {
        Vts.EnsureConnected();

        if (_propertyViewOpen) _ = UpdateClientAsync();
    }
}

public abstract class VTubeStudioAction<TSettings>(DeckActionContext context, IVTubeStudio vts)
    : VTubeStudioAction<TSettings, SingleState>(context, vts)
    where TSettings : new();
