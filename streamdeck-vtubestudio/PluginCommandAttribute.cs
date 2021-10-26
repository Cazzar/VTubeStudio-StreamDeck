using System;

namespace Cazzar.StreamDeck.VTubeStudio
{
    [AttributeUsage(AttributeTargets.Method)]
    public class PluginCommandAttribute : Attribute
    {
        public string Command { get; init; }
        public PluginCommandAttribute(string command)
        {
            this.Command = command;
        }
    }
}
