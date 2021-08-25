using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Responses;

namespace Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Requests
{
    public class AvailableModelsRequest : ApiRequest<AvailableModelsResponse>
    {
        public override RequestType MessageType { get; } = RequestType.AvailableModelsRequest;
    }
}
