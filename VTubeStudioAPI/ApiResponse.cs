using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Responses;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi;

class ApiResponse<T> : ApiResponse
{
    [JsonProperty("data")] 
    public new required T Data { get; init; }
}
    
class ApiResponse
{
    [JsonProperty("apiName")]
    public required string ApiName { get; set; }

    [JsonProperty("apiVersion")]
    public required string ApiVersion { get; set; }
        
    [JsonProperty("messageType")] 
    public ResponseType MessageType { get; init; }
        
    [JsonProperty("data")] 
    public JObject? Data { get; init; }
    
    [JsonProperty("requestID")]
    public required string RequestId { get; init; }
}