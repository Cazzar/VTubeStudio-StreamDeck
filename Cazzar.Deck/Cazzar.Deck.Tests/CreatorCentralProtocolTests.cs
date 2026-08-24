using Cazzar.Deck.Core.Protocol;
using Cazzar.Deck.Abstractions.Protocol.Commands;
using Cazzar.Deck.Abstractions.Protocol.Events;
using Cazzar.Deck.Abstractions;
using Cazzar.Deck.Hosts.CreatorCentral;
using System.Text.Json.Nodes;
using Xunit;

namespace Cazzar.Deck.Tests;

using static CodecFixture;

// Shapes here are taken from the Creator Central SDK wiki, not from the older shipped package,
// which had several event names left over from its Elgato original.
public class CreatorCentralProtocolTests
{

    private readonly CreatorCentralCodec _codec = new(new DeckLaunchOptions { Uuid = "dev.cazzar.package" });

    [Fact]
    public void Changes_the_state_icon_with_change_icon()
    {
        var json = JsonNode.Parse(_codec.Encode(new SetImage(Ref, "base64", 1))!)!;

        // changeImage is a separate stateless overlay; the state image is changeIcon.
        Assert.Equal("changeIcon", (string?)json["event"]);
        Assert.Equal("base64", (string?)json["payload"]!["image"]);
        Assert.Equal(1, (int?)json["payload"]!["state"]);
    }

    [Fact]
    public void Nests_package_settings_under_a_settings_member()
    {
        var json = JsonNode.Parse(_codec.Encode(new SaveGlobalSettings(new JsonObject { ["token"] = "t" }))!)!;

        Assert.Equal("setPackageSettings", (string?)json["event"]);
        Assert.Equal("dev.cazzar.package", (string?)json["context"]);
        Assert.Equal("t", (string?)json["payload"]!["settings"]!["token"]);
    }

    [Fact]
    public void Reads_package_settings_straight_from_the_payload()
    {
        var events = _codec.Decode("""
            { "event": "didReceivePackageSettings", "payload": { "token": "abc" } }
            """);

        var received = Assert.IsType<GlobalSettingsReceived>(Assert.Single(events));
        Assert.True(received.Settings.TryGet<string>("token", out var token));
        Assert.Equal("abc", token);
    }

    [Fact]
    public void Reads_appear_settings_from_the_payload_settings_member()
    {
        var events = _codec.Decode("""
            { "event": "widgetWillAppear", "context": "ctx-1", "widget": "w",
              "payload": { "state": 0, "layout": 0, "settings": { "modelId": "m" } } }
            """);

        var appeared = Assert.IsType<ActionAppeared>(Assert.Single(events));
        Assert.True(appeared.Settings.TryGet<string>("modelId", out var model));
        Assert.Equal("m", model);
    }

    [Fact]
    public void Supports_opening_a_url()
    {
        var json = JsonNode.Parse(_codec.Encode(new OpenUrl(new("https://cazzar.dev/")))!)!;

        Assert.Equal("openUrl", (string?)json["event"]);
        Assert.Equal("https://cazzar.dev/", (string?)json["payload"]!["url"]);
    }

    [Fact]
    public void Supports_writing_to_the_host_log()
    {
        var json = JsonNode.Parse(_codec.Encode(new SendLog("hello"))!)!;

        Assert.Equal("sendLog", (string?)json["event"]);
        Assert.Equal("hello", (string?)json["payload"]!["message"]);
    }

    [Fact]
    public void Has_no_alert_or_ok_flash()
    {
        Assert.Null(_codec.Encode(new ShowAlert(Ref)));
        Assert.Null(_codec.Encode(new ShowOk(Ref)));
    }

    [Fact]
    public void Notices_the_host_shutting_down()
    {
        Assert.IsType<HostShuttingDown>(Assert.Single(_codec.Decode("""{ "event": "applicationWillTerminate" }""")));
    }
}
