using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Responses;

namespace Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Requests;

public abstract class ApiRequest
{
    public abstract RequestType MessageType { get; }
}