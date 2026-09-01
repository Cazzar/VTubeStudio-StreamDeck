using Microsoft.Extensions.Logging;

namespace VTubeStudio.Api;

public partial class VTubeStudioClient
{
    [LoggerMessage(LogLevel.Debug, "Not {state}; dropping {MessageType}")]
    partial void LogNotStateDropping(string state, string messageType);
    
    [LoggerMessage(LogLevel.Trace, "-> {Json}")]
    partial void LogJson(string json);
}