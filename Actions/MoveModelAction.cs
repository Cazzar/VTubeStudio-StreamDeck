using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BarRaider.SdTools;
using BarRaider.SdTools.Events;
using BarRaider.SdTools.Wrappers;
using Cazzar.StreamDeck.VTubeStudio.Models;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Requests;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using streamdeck_client_csharp.Events;
using KeyPayload = BarRaider.SdTools.KeyPayload;

namespace Cazzar.StreamDeck.VTubeStudio.Actions
{
    [PluginActionId("dev.cazzar.vtubestudio.movemodel")]
    class MoveModelAction : BaseAction<MoveModelAction.PluginSettings>
    {
        public class PluginSettings
        {
            [JsonProperty("seconds")]
            public double? Seconds { get; set; } = 0;
            
            [JsonProperty("posX")]
            public double? PosX { get; set; } = 0;

            [JsonProperty("posY")]
            public double? PosY { get; set; } = 0;

            [JsonProperty("rotation")]
            public int? Rotation { get; set; } = 0;

            [JsonProperty("relative")]
            public bool Relative { get; set; } = true;
        }
        
        public MoveModelAction(ISDConnection connection, InitialPayload payload) : base(connection, payload)
        {
        }

        protected override void Pressed(KeyPayload payload)
        {
            Vts.Send(new MoveModelRequest()
            {
                PositionX = Settings.PosX,
                PositionY = Settings.PosY,
                RelativeMove =  Settings.Relative,
                Rotation = Settings.Rotation,
                Size = null,
                TimeInSeconds = Settings.Seconds,
            });
        }

        protected override void Released(KeyPayload payload)
        {
        }

        protected override object GetClientData() => new
        {
            Connected = VTubeStudioWebsocketClient.Instance.IsAuthed,
        };
    }
}

