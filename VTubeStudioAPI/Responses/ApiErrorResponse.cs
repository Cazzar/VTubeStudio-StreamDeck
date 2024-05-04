using Newtonsoft.Json;

namespace Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Responses;

public class ApiErrorResponse
{
    [JsonProperty("errorID")]
    public ApiError Id { get; set; }
    
    [JsonProperty("message")]
    public required string Message { get; set; }
}