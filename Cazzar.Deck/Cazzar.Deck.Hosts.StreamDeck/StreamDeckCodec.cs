using Cazzar.Deck.Abstractions.Protocol.Commands;
using Cazzar.Deck.Abstractions.Protocol.Events;
using Cazzar.Deck.Abstractions.Protocol;
using Cazzar.Deck.Abstractions;
using Cazzar.Deck.Core;
using System.Text.Json.Nodes;

namespace Cazzar.Deck.Hosts.StreamDeck;

public sealed class StreamDeckCodec(IDeckLaunchOptions options) : IDeckCodec
{
    public IReadOnlyList<IDeckEvent> Decode(string frame)
    {
        if (JsonNode.Parse(frame) is not JsonObject message) return [];

        var payload = message["payload"] as JsonObject;

        return message["event"]?.GetValue<string>() switch
        {
            "willAppear" => [new ActionAppeared(Ref(message, payload), Settings(payload))],
            "willDisappear" => [new ActionDisappeared(Ref(message, payload))],
            "didReceiveSettings" => [new SettingsReceived(Ref(message, payload), Settings(payload))],
            "didReceiveGlobalSettings" => [new GlobalSettingsReceived(Settings(payload))],

            "keyDown" => [new KeyPressed(Ref(message, payload), State(payload))],
            "keyUp" => [new KeyReleased(Ref(message, payload), State(payload))],

            "propertyInspectorDidAppear" => [new PropertyViewOpened(Ref(message, payload))],
            "propertyInspectorDidDisappear" => [new PropertyViewClosed(Ref(message, payload))],
            "sendToPlugin" => [new PropertyViewMessage(Ref(message, payload), JsonPayload.From(message["payload"]))],

            "dialRotate" => [new DialRotated(Ref(message, payload), DeckFrame.Int(payload, "ticks"), DeckFrame.Bool(payload, "pressed"))],
            "dialDown" => [new DialPressed(Ref(message, payload))],
            "dialUp" => [new DialReleased(Ref(message, payload))],
            "touchTap" => [new TouchTapped(Ref(message, payload), TapPos(payload, 0), TapPos(payload, 1), DeckFrame.Bool(payload, "hold"))],

            "deviceDidConnect" => [Device(message)],
            "deviceDidDisconnect" => [new DeviceDisconnected(DeckFrame.Str(message, "device") ?? string.Empty)],
            "applicationDidLaunch" => [new ApplicationLaunched(DeckFrame.Str(payload, "application") ?? string.Empty)],
            "applicationDidTerminate" => [new ApplicationTerminated(DeckFrame.Str(payload, "application") ?? string.Empty)],
            "systemDidWakeUp" => [new HostWokeUp()],

            _ => [],
        };
    }

    public string? Encode(IDeckCommand command) => command switch
    {
        Register c => DeckFrame.Build(c.Event, o => o["uuid"] = c.Uuid),

        SetTitle c => Frame("setTitle", c.Ref, new JsonObject
        {
            ["title"] = c.Title,
            ["target"] = 0,
            ["state"] = c.State,
        }),

        SetImage c => Frame("setImage", c.Ref, new JsonObject
        {
            ["image"] = c.Image,
            ["target"] = 0,
            ["state"] = c.State,
        }),

        SetState c => Frame("setState", c.Ref, new JsonObject { ["state"] = c.State }),
        ShowAlert c => Frame("showAlert", c.Ref, payload: null),
        ShowOk c => Frame("showOk", c.Ref, payload: null),

        SaveSettings c => Frame("setSettings", c.Ref, c.Settings),
        RequestSettings c => Frame("getSettings", c.Ref, payload: null),

        // Addressed to the plugin, not a placed action.
        SaveGlobalSettings c => DeckFrame.Build("setGlobalSettings", o =>
        {
            o["context"] = options.Uuid;
            o["payload"] = c.Settings;
        }),
        RequestGlobalSettings => DeckFrame.Build("getGlobalSettings", o => o["context"] = options.Uuid),

        SendToPropertyView c => Frame("sendToPropertyInspector", c.Ref, c.Payload),

        SetFeedback c => Frame("setFeedback", c.Ref, c.Layout),
        SetFeedbackLayout c => Frame("setFeedbackLayout", c.Ref, new JsonObject { ["layout"] = c.LayoutId }),

        OpenUrl c => DeckFrame.Build("openUrl", o => o["payload"] = new JsonObject { ["url"] = c.Url.ToString() }),

        SendLog => null,

        _ => null,
    };

    private static ActionRef Ref(JsonObject message, JsonObject? payload) => new(
        DeckFrame.Str(message, "context") ?? string.Empty,
        DeckFrame.Str(message, "action") ?? string.Empty,
        DeckFrame.Str(message, "device"),
        DeckFrame.Str(payload, "controller") == "Encoder" ? DeckController.Encoder : DeckController.Keypad);

    private static IPayload Settings(JsonObject? payload) => JsonPayload.From(payload?["settings"]);

    private static uint State(JsonObject? payload) => (uint)DeckFrame.Int(payload, "state");

    private static IDeckEvent Device(JsonObject message)
    {
        var info = message["deviceInfo"] as JsonObject;
        var size = info?["size"] as JsonObject;

        return new DeviceConnected(
            DeckFrame.Str(message, "device") ?? string.Empty,
            DeckFrame.Str(info, "name"),
            DeckFrame.Int(size, "columns"),
            DeckFrame.Int(size, "rows"));
    }

    private static int TapPos(JsonObject? payload, int index) =>
        payload?["tapPos"] is JsonArray pos && pos.Count > index ? pos[index]!.GetValue<int>() : 0;

    private static string Frame(string @event, ActionRef @ref, JsonNode? payload) => DeckFrame.Build(@event, message =>
    {
        message["context"] = @ref.ContextId;
        if (!string.IsNullOrEmpty(@ref.ActionId)) message["action"] = @ref.ActionId;
        if (payload is not null) message["payload"] = payload;
    });
}
