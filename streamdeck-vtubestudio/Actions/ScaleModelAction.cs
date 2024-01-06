using System.Runtime.Serialization;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Requests;
using Microsoft.Extensions.Logging;
using StreamDeckLib;

namespace Cazzar.StreamDeck.VTubeStudio.Actions
{
    [StreamDeckAction("dev.cazzar.vtubestudio.scalemodel")]
    public class ScaleModelAction : BaseAction<ScaleModelAction.PluginSettings>
    {
        public class PluginSettings
        {
            [DataMember(Name = "size")]
            public float Size { get; set; } = 0;
        }

        protected override void Pressed() => Vts.Send(new MoveModelRequest() { Size = Settings.Size, });

        protected override void Released() { }
        
        protected override void SettingsUpdated(PluginSettings oldSettings, PluginSettings newSettings)
        {
        }

        public ScaleModelAction(GlobalSettingsManager gsm, VTubeStudioWebsocketClient vts, IStreamDeckConnection isd, ILogger<ScaleModelAction> logger) : base(gsm, vts, isd, logger)
        {
        }
    }
}
