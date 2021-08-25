using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Responses;

namespace Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Requests
{
    [AuthLess]
    public class ApiStateRequest : ApiRequest<ApiStateResponse>
    {
        public override RequestType MessageType { get; } = RequestType.APIStateRequest;
    }
}
