using System;
using NLog.LayoutRenderers;

namespace Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi
{
    public class ApiEventArgs<T> : EventArgs
    {
        public T Response { get; init; }

        public ApiEventArgs(T response) => Response = response;
    }
}
