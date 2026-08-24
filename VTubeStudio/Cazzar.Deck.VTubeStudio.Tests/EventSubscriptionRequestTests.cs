using System.Text.Json;
using System.Text.Json.Nodes;
using VTubeStudio.Api;
using VTubeStudio.Api.Requests;
using Xunit;

namespace Cazzar.Deck.VTubeStudio.Tests;

public class EventSubscriptionRequestTests
{
    // Mirrors VTubeStudioClient.Send: the runtime type drives the type info, so an unregistered
    // Config type throws and takes the whole read loop down with it.
    private static JsonNode? Serialise(IVtsRequest request) =>
        JsonSerializer.SerializeToNode(request, VtsJson.Options.GetTypeInfo(request.GetType()));

    [Fact]
    public void Subscription_config_survives_source_generated_serialisation()
    {
        var json = Serialise(new EventSubscriptionRequest("ExpressionToggledEvent")
        {
            Config = new JsonObject
            {
                ["sendAllActiveStatesOnSubscription"] = true,
                ["ignoreLive2DItems"] = true,
            },
        })!.ToJsonString();

        Assert.Contains("\"sendAllActiveStatesOnSubscription\":true", json);
        Assert.Contains("\"ignoreLive2DItems\":true", json);
    }

    [Fact]
    public void Subscription_without_config_still_serialises()
    {
        var json = Serialise(new EventSubscriptionRequest("ModelMovedEvent"))!.ToJsonString();

        Assert.Contains("\"eventName\":\"ModelMovedEvent\"", json);
        Assert.Contains("\"config\":{}", json);
    }
}
