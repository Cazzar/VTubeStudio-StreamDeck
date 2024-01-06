using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cazzar.StreamDeck.VTubeStudio.Models;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Requests;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Responses;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StreamDeckLib;

namespace Cazzar.StreamDeck.VTubeStudio.Actions
{
    [StreamDeckAction("dev.cazzar.vtubestudio.movemodel")]
    class MoveModelAction : BaseAction<MoveModelAction.PluginSettings>, IDisposable
    {
        private readonly ILogger<MoveModelAction> _logger;

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

        // public MoveModelAction(GlobalSettingsManager gsm, VTubeStudioWebsocketClient vts) : base(gsm, vts)
        public MoveModelAction(GlobalSettingsManager gsm, VTubeStudioWebsocketClient vts, IStreamDeckConnection isd, ILogger<MoveModelAction> logger) : base(gsm, vts, isd, logger)
        {
            _logger = logger;
            VTubeStudioWebsocketClient.OnCurrentModelInformation += CurrentModelInformation;
        }

        private void CurrentModelInformation(object sender, ApiEventArgs<CurrentModelResponse> e)
        {
            if (string.IsNullOrEmpty(e.RequestId)) return; //I don't care about this request.
            if (_requestId != e.RequestId) return;

            var pos = e.Response.ModelPosition;
            Settings.PosX = pos.X;
            Settings.PosY = pos.Y;
            Settings.Rotation = pos.Rotation;
            Settings.Size = pos.Size;
            Settings.Relative = false;

            SaveSettings();
        }

        protected override void Pressed()
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

        protected override void Released()
        {
        }

        protected override void SettingsUpdated(PluginSettings oldSettings, PluginSettings newSettings) { }

        [PluginCommand("get-params")]
        public virtual void GetModel(PluginPayload pl)
        {
            _requestId = Guid.NewGuid().ToString();
            Vts.Send(new CurrentModelRequest(), _requestId);
        }

        public void Dispose()
        {
            VTubeStudioWebsocketClient.OnCurrentModelInformation -= CurrentModelInformation;
        }
    }
}

