namespace VTubeStudio.Api.Requests;

public sealed record CurrentModelRequest : IVtsRequest
{
    public string MessageType => "CurrentModelRequest";
}
