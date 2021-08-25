using System;
using NLog.Targets.Wrappers;

namespace Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class AuthLess : Attribute
    {
        
    }
}
