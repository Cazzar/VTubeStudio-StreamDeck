using System.Diagnostics.CodeAnalysis;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Models;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Responses;
using Newtonsoft.Json;

namespace Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Requests
{
    public class ModelHotkeyRequest : ApiRequest
    {
        [JsonProperty("modelID")]
        public string ModelId { get; set; }
        
        public ModelHotkeyRequest([MaybeNull] string modelId)
        {
            ModelId = modelId;
        }

        public ModelHotkeyRequest(Model model) : this(model.Id)
        {
        }

        public ModelHotkeyRequest()
        {
            
        }

        public override RequestType MessageType { get; } = RequestType.HotkeysInCurrentModelRequest;
    }
}
