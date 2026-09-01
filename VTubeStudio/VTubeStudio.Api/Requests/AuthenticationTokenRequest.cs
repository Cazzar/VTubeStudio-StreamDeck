using System.Text.Json.Serialization;

namespace VTubeStudio.Api.Requests;

public sealed record AuthenticationTokenRequest(
    [property: JsonPropertyName("pluginName")] string PluginName,
    [property: JsonPropertyName("pluginDeveloper")] string PluginDeveloper,
    [property: JsonPropertyName("pluginIcon")] string? PluginIcon) : IUnauthenticatedRequest
{
    public string MessageType => "AuthenticationTokenRequest";
}
