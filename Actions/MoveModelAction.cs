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
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Responses;
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
            public double? Rotation { get; set; } = 0;

            [JsonProperty("size")]
            public double? Size { get; set; } = null;

            [JsonProperty("relative")]
            public bool Relative { get; set; } = true;
        }
        
        private string _requestId = string.Empty;
        
        public MoveModelAction(ISDConnection connection, InitialPayload payload) : base(connection, payload)
        {
            VTubeStudioWebsocketClient.OnCurrentModelInformation += CurrentModelInformation;
        }

        private async void CurrentModelInformation(object sender, ApiEventArgs<CurrentModelResponse> e)
        {
            if (string.IsNullOrEmpty(e.RequestId) || e.RequestId != _requestId) return; //I don't care about this request.

            var pos = e.Response.ModelPosition;
            Settings.PosX = pos.X;
            Settings.PosY = pos.Y;
            Settings.Rotation = pos.Rotation;
            Settings.Size = pos.Size;
            Settings.Relative = false;
            
            await SaveSettings();
        }

        protected override void Pressed(KeyPayload payload)
        {
            Vts.Send(new MoveModelRequest()
            {
                PositionX = Settings.PosX,
                PositionY = Settings.PosY,
                RelativeMove =  Settings.Relative,
                Rotation = Settings.Rotation,
                Size = Settings.Size,
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

        [PluginCommand("get-params")]
        public virtual void GetModel(PluginPayload pl)
        {
            _requestId = Guid.NewGuid().ToString();
            Vts.Send(new CurrentModelRequest(), _requestId);
        }

        public override void Dispose()
        {
            VTubeStudioWebsocketClient.OnCurrentModelInformation -= CurrentModelInformation;
            base.Dispose();
        }
    }
}

