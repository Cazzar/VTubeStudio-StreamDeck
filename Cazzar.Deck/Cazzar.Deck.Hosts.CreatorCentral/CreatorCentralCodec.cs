using Cazzar.Deck.Abstractions.Protocol.Commands;
using Cazzar.Deck.Abstractions.Protocol.Events;
using Cazzar.Deck.Abstractions.Protocol;
using Cazzar.Deck.Abstractions;
using Cazzar.Deck.Core;
using System.Text.Json.Nodes;
using System.Text.Json;

namespace Cazzar.Deck.Hosts.CreatorCentral;

public sealed class CreatorCentralCodec(IDeckLaunchOptions options) : IDeckCodec
{
    public IReadOnlyList<IDeckEvent> Decode(string frame)
    {
        if (JsonNode.Parse(frame) is not JsonObject message) return [];

        var payload = message["payload"] as JsonObject;

        return message["event"]?.GetValue<string>() switch
        {
            "widgetWillAppear" => [new ActionAppeared(Ref(message), JsonPayload.From(payload?["settings"]))],
            "widgetWillDisappear" => [new ActionDisappeared(Ref(message))],

            // Unlike Elgato, the payload *is* the settings object.
            "didReceiveWidgetSettings" => [new SettingsReceived(Ref(message), JsonPayload.From(message["payload"]))],
            "didReceivePackageSettings" => [new GlobalSettingsReceived(JsonPayload.From(message["payload"]))],

            "actionDown" => [new KeyPressed(Ref(message), State(payload))],
            "actionUp" => [new KeyReleased(Ref(message), State(payload))],

            // Fire-and-forget: no matching release, so synthesise one.
            "actionTriggered" =>
            [
                new KeyPressed(Ref(message), State(payload)),
                new KeyReleased(Ref(message), State(payload)),
            ],

            "applicationWillTerminate" => [new HostShuttingDown()],

            "propertyViewDidAppear" => [new PropertyViewOpened(Ref(message))],
            "propertyViewDidDisappear" => [new PropertyViewClosed(Ref(message))],
            "sendToPackage" => [new PropertyViewMessage(Ref(message), JsonPayload.From(message["payload"]))],

            _ => [],
        };
    }

    public string? Encode(IDeckCommand command) => command switch
    {
        Register c => DeckFrame.Build(c.Event, o => o["uuid"] = c.Uuid),

        SetTitle c => Frame("changeTitle", c.Ref, Title(c)),

        SetImage c => Frame("changeIcon", c.Ref, new JsonObject
        {
            ["image"] = c.Image,
            ["state"] = c.State,
        }),

        SetState c => Frame("changeState", c.Ref, new JsonObject { ["state"] = c.State }),

        SaveSettings c => Frame("setWidgetSettings", c.Ref, c.Settings),
        RequestSettings c => Frame("getWidgetSettings", c.Ref, payload: null),

        SaveGlobalSettings c => DeckFrame.Build("setPackageSettings", o =>
        {
            o["context"] = options.Uuid;
            o["payload"] = new JsonObject { ["settings"] = c.Settings };
        }),
        RequestGlobalSettings => DeckFrame.Build("getPackageSettings", o => o["context"] = options.Uuid),

        SendToPropertyView c => Frame("sendToPropertyView", c.Ref, c.Payload, includeWidget: true),

        SendLog c => DeckFrame.Build("sendLog", o => o["payload"] = new JsonObject { ["message"] = c.Message }),

        OpenUrl c => DeckFrame.Build("openUrl", o => o["payload"] = new JsonObject { ["url"] = c.Url.ToString() }),

        // No dials, and no alert/ok flash. changeActionEffect is the nearest thing but means
        // something else: press/clear/invalid rather than a transient success or failure blink.
        SetFeedback or SetFeedbackLayout or ShowAlert or ShowOk => null,

        _ => null,
    };

    private static ActionRef Ref(JsonObject message) => new(
        DeckFrame.Str(message, "context") ?? string.Empty,
        DeckFrame.Str(message, "widget") ?? string.Empty);

    private static uint State(JsonObject? payload) =>
        payload?["state"] is { } node && node.GetValueKind() == JsonValueKind.Number ? node.GetValue<uint>() : 0;

    private static JsonObject Title(SetTitle command)
    {
        var payload = new JsonObject { ["title"] = command.Title };

        if (command.State is { } state) payload["state"] = state;

        return payload;
    }

    private static string Frame(string @event, ActionRef @ref, JsonNode? payload, bool includeWidget = false) =>
        DeckFrame.Build(@event, message =>
        {
            message["context"] = @ref.ContextId;
            if (includeWidget && !string.IsNullOrEmpty(@ref.ActionId)) message["widget"] = @ref.ActionId;
            if (payload is not null) message["payload"] = payload;
        });
}
