using System;

namespace Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi;

public class ApiEventArgs<T>(T response) : EventArgs
{
    public T Response { get; init; } = response;
    public string? RequestId { get; init; }

}