using Cazzar.Deck.Core.Protocol;
using Cazzar.Deck.Abstractions.Protocol.Commands;
using Cazzar.Deck.Abstractions.Protocol.Events;
using Cazzar.Deck.Abstractions;
using Cazzar.Deck.Hosts.CreatorCentral;
using System.Text.Json.Nodes;
using Xunit;

namespace Cazzar.Deck.Tests;

using static CodecFixture;

public class CreatorCentralCodecTests
{
    private readonly CreatorCentralCodec _codec = new(new DeckLaunchOptions { Uuid = "dev.cazzar.package" });

    [Fact]
    public void Treats_the_settings_payload_as_the_settings_object()
    {
        var events = _codec.Decode("""
            { "event": "didReceiveWidgetSettings", "context": "ctx-1", "widget": "w",
              "payload": { "hotkeyId": "abc" } }
            """);

        var received = Assert.IsType<SettingsReceived>(Assert.Single(events));
        Assert.True(received.Settings.TryGet<string>("hotkeyId", out var id));
        Assert.Equal("abc", id);
    }

    [Fact]
    public void Synthesises_a_release_for_fire_and_forget_triggers()
    {
        var events = _codec.Decode("""
            { "event": "actionTriggered", "context": "ctx-1", "widget": "w", "payload": { "state": 0 } }
            """);

        Assert.Collection(events,
            e => Assert.IsType<KeyPressed>(e),
            e => Assert.IsType<KeyReleased>(e));
    }

    [Fact]
    public void Reads_the_widget_field_as_the_action_id()
    {
        var events = _codec.Decode("""
            { "event": "actionDown", "context": "ctx-1", "widget": "dev.cazzar.hotkey", "payload": {} }
            """);

        Assert.Equal("dev.cazzar.hotkey", Assert.IsType<KeyPressed>(Assert.Single(events)).Ref.ActionId);
    }

    [Fact]
    public void Uses_the_sdk_event_names_for_widget_changes()
    {
        Assert.Equal("changeTitle", Event(_codec.Encode(new SetTitle(Ref, "Hi"))));
        Assert.Equal("changeIcon", Event(_codec.Encode(new SetImage(Ref, "data"))));
        Assert.Equal("changeState", Event(_codec.Encode(new SetState(Ref, 1))));
    }

    [Fact]
    public void Carries_the_widget_id_on_send_to_property_view()
    {
        var json = JsonNode.Parse(_codec.Encode(new SendToPropertyView(Ref, new JsonObject { ["a"] = 1 }))!)!;

        Assert.Equal("sendToPropertyView", (string?)json["event"]);
        Assert.Equal("dev.cazzar.action", (string?)json["widget"]);
    }

    [Fact]
    public void Has_no_encoding_for_encoder_commands()
    {
        Assert.Null(_codec.Encode(new SetFeedbackLayout(new("ctx-1", "a"), "$B1")));
        Assert.Null(_codec.Encode(new SetFeedback(new("ctx-1", "a"), new())));
    }
}
