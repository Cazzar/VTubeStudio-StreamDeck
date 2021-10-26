using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Responses;

namespace Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Requests
{
    [AuthLess]
    public class ApiStateRequest : ApiRequest
    {
        public override RequestType MessageType { get; } = RequestType.APIStateRequest;
    }
}
