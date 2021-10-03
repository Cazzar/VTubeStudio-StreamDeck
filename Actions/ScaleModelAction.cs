using System.Runtime.Serialization;
using BarRaider.SdTools;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Requests;

namespace Cazzar.StreamDeck.VTubeStudio.Actions
{
    [PluginActionId("dev.cazzar.vtubestudio.scalemodel")]
    public class ScaleModelAction : BaseAction<ScaleModelAction.PluginSettings>
    {
        public class PluginSettings
        {
            [DataMember(Name = "size")]
            public float Size { get; set; } = 0;
        }

        public ScaleModelAction(ISDConnection connection, InitialPayload payload) : base(connection, payload) { }

        protected override void Pressed(KeyPayload payload) => Vts.Send(new MoveModelRequest() { Size = Settings.Size, });

        protected override void Released(KeyPayload payload) { }

        protected override object GetClientData() => new
        {
            Connected = Vts.IsAuthed,
        };
    }
}
