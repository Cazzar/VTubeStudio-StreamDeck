using JetBrains.Annotations;

namespace VTubeStudio.Api;

[PublicAPI]
public sealed class VtsEventArgs<T>(T response, string? requestId = null) : EventArgs
{
    public T Response { get; } = response;
    public string? RequestId { get; } = requestId;
}
