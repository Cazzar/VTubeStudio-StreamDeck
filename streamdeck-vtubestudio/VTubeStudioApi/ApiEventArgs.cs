using System;

namespace Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi
{
    public class ApiEventArgs<T> : EventArgs
    {
        public T Response { get; init; }
        public string RequestId { get; init; }

        public ApiEventArgs(T response) => Response = response;
    }
}
