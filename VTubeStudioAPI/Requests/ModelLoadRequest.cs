using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Models;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Responses;
using Newtonsoft.Json;

namespace Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Requests;

public class ModelLoadRequest : ApiRequest
{
    [JsonProperty("modelID")]
    public string Id { get; set; }
        
    public ModelLoadRequest(Model model)
    {
            this.Id = model.Id;
        }
        
    public ModelLoadRequest(string id)
    {
            this.Id = id;
        }

    public override RequestType MessageType { get; } = RequestType.ModelLoadRequest;
}