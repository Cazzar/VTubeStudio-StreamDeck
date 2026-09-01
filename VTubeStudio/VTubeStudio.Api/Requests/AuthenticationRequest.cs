using System.Text.Json.Serialization;

namespace VTubeStudio.Api.Requests;

public sealed record AuthenticationRequest(
    [property: JsonPropertyName("pluginName")] string PluginName,
    [property: JsonPropertyName("pluginDeveloper")] string PluginDeveloper,
    [property: JsonPropertyName("authenticationToken")] string Token) : IUnauthenticatedRequest
{
    public string MessageType => "AuthenticationRequest";
}
