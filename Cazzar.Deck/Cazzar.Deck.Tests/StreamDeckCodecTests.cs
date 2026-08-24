using Cazzar.Deck.Core.Protocol;
using Cazzar.Deck.Abstractions.Protocol.Commands;
using Cazzar.Deck.Abstractions.Protocol.Events;
using Cazzar.Deck.Abstractions;
using Cazzar.Deck.Hosts.StreamDeck;
using System.Text.Json.Nodes;
using Xunit;

namespace Cazzar.Deck.Tests;

public class StreamDeckCodecTests
{
    private readonly StreamDeckCodec _codec = new(new DeckLaunchOptions { Uuid = "dev.cazzar.plugin" });

    [Fact]
    public void Decodes_will_appear_with_settings_and_controller()
    {
        var events = _codec.Decode("""
            {
              "event": "willAppear",
              "context": "ctx-1",
              "action": "dev.cazzar.zoom",
              "device": "dev-1",
              "payload": { "controller": "Encoder", "settings": { "stepSize": 4 } }
            }
            """);

        var appeared = Assert.IsType<ActionAppeared>(Assert.Single(events));
        Assert.Equal("ctx-1", appeared.Ref.ContextId);
        Assert.Equal("dev.cazzar.zoom", appeared.Ref.ActionId);
        Assert.Equal("dev-1", appeared.Ref.DeviceId);
        Assert.Equal(DeckController.Encoder, appeared.Ref.Controller);
        Assert.True(appeared.Settings.TryGet<int>("stepSize", out var step));
        Assert.Equal(4, step);
    }

    [Fact]
    public void Decodes_dial_rotate()
    {
        var events = _codec.Decode("""
            { "event": "dialRotate", "context": "ctx-1", "action": "a",
              "payload": { "ticks": -3, "pressed": true } }
            """);

        var rotated = Assert.IsType<DialRotated>(Assert.Single(events));
        Assert.Equal(-3, rotated.Ticks);
        Assert.True(rotated.Pressed);
    }

    [Fact]
    public void Ignores_frames_it_does_not_model()
    {
        Assert.Empty(_codec.Decode("""{ "event": "somethingElgatoAddedLastTuesday" }"""));
    }

    [Fact]
    public void Encodes_set_title_in_elgato_shape()
    {
        var frame = _codec.Encode(new SetTitle(new("ctx-1", "a"), "Zoom", 1));

        var json = JsonNode.Parse(frame!)!;
        Assert.Equal("setTitle", (string?)json["event"]);
        Assert.Equal("ctx-1", (string?)json["context"]);
        Assert.Equal("Zoom", (string?)json["payload"]!["title"]);
        Assert.Equal(1, (int?)json["payload"]!["state"]);
    }

    [Fact]
    public void Addresses_global_settings_to_the_plugin_uuid()
    {
        var json = JsonNode.Parse(_codec.Encode(new RequestGlobalSettings())!)!;

        Assert.Equal("getGlobalSettings", (string?)json["event"]);
        Assert.Equal("dev.cazzar.plugin", (string?)json["context"]);
    }

    [Fact]
    public void Supports_encoder_commands()
    {
        Assert.NotNull(_codec.Encode(new SetFeedbackLayout(new("ctx-1", "a"), "$B1")));
    }
}
