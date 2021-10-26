using System;
using System.Collections.Generic;
using StreamDeckLib.Models;

namespace StreamDeckLib.Json
{
    public class StreamDeckMessageConverter : TypeMapConverter<EventMessage>
    {
        public StreamDeckMessageConverter() : base(new Dictionary<string, Type>()
        {
            {"didReceiveSettings", typeof(DidReceiveSettings)},
            {"didReceiveGlobalSettings", typeof(GlobalSettings)},
            {"keyDown", typeof(KeyDown)},
            {"keyUp", typeof(KeyUp)},
            {"willAppear", typeof(OnWillAppear)},
            {"willDisappear", typeof(OnWillDisappear)},
            {"titleParametersDidChange", typeof(TitleParametersDidChange)},
            {"deviceDidConnect", typeof(DeviceDidConnect)},
            {"deviceDidDisconnect", typeof(DeviceDidDisconnect)},
            {"applicationDidLaunch", typeof(ApplicationDidLaunch)},
            {"applicationDidTerminate", typeof(ApplicationDidTerminate)},
            {"systemDidWakeUp", typeof(SystemDidWakeUp)},
            {"propertyInspectorDidAppear", typeof(PropertyInspectorDidAppear)},
            {"propertyInspectorDidDisappear", typeof(PropertyInspectorDidDisappear)},
            {"sendToPlugin", typeof(SendToPlugin)},
        }, "event")
        {}
    }
} 
