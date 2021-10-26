using System;

namespace StreamDeckLib.Models
{
    public class StreamDeckEventArgs<T> : EventArgs
    {
        public T Payload { get; set; }
        public string Event { get; set; }
    }
}
