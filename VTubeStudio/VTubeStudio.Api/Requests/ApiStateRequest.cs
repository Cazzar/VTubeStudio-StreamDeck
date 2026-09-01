namespace VTubeStudio.Api.Requests;

public sealed record ApiStateRequest : IUnauthenticatedRequest
{
    public string MessageType => "APIStateRequest";
}
