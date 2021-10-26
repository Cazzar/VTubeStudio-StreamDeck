using System;

namespace StreamDeckLib
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class StreamDeckAction : Attribute
    {
        public StreamDeckAction(string actionId)
        {
            ActionId = actionId;
        }

        public string ActionId { get; init; }
    }
}