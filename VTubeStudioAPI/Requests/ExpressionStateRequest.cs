using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Responses;

namespace Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Requests;

public class ExpressionStateRequest(string? expressionFile = null) : ApiRequest
{
    public string? ExpressionFile { get; set; } = expressionFile;

    public override RequestType MessageType { get; } = RequestType.ExpressionStateRequest;
}