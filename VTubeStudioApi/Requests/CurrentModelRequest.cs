using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Responses;

namespace Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Requests
{
    public class CurrentModelRequest : ApiRequest
    {
        public override RequestType MessageType { get; } = RequestType.CurrentModelRequest;
    }
}
