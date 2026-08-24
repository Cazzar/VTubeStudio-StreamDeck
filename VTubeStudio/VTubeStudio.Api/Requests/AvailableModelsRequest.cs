namespace VTubeStudio.Api.Requests;

public sealed record AvailableModelsRequest : IVtsRequest
{
    public string MessageType => "AvailableModelsRequest";
}
