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
    public class MoveModelAction : PluginBase
    {
        private class PluginSettings
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
        
        private readonly PluginSettings _settings;
        
        public MoveModelAction(ISDConnection connection, InitialPayload payload) : base(connection, payload)
        {
            //Ensure global settings is loaded;
            GlobalSettingsManager.Instance.Load();
            VTubeStudioWebsocketClient.Instance.ConnectIfNeeded();
            
            if (payload.Settings == null || payload.Settings.Count == 0)
                _settings = new();
            else
                _settings = payload.Settings.ToObject<PluginSettings>();
            
            ModelCache.Instance.ModelCacheUpdated += ModelCacheUpdate;
            Connection.OnPropertyInspectorDidAppear += PropertyInspectorDidAppear;
            Connection.OnSendToPlugin += DataFromPropertyInspector;
        }

        private async void DataFromPropertyInspector(object sender, SDEventReceivedEventArgs<SendToPlugin> e)
        {
            var pl = e.Event.Payload.ToObject<PluginPayload>();

            if (pl == null)
            {
                Logger.Instance.LogMessage(TracingLevel.INFO, "Payload is null");
                return;
            }

            switch (pl.Command.ToLower())
            {
                case "force-reconnect":
                    VTubeStudioWebsocketClient.Instance.Reconnect();
                    break;
                case "refresh":
                    await SendDataToClient();
                    break;
            }
        }

        private async void PropertyInspectorDidAppear(object sender, SDEventReceivedEventArgs<PropertyInspectorDidAppear> e)
        {
            await SendDataToClient();
        }

        private async Task SendDataToClient()
        {
            await Connection.SendToPropertyInspectorAsync(JObject.FromObject(new {Connected = VTubeStudioWebsocketClient.Instance.IsAuthed}));
        }

        private async void ModelCacheUpdate(object sender, ModelCacheUpdatedEventArgs e)
        {
            await SendDataToClient();
        }

        private async Task SaveSettings()
        {
            var settings = JObject.FromObject(_settings);
            await Connection.SetSettingsAsync(settings);
        }

        public override void KeyPressed(KeyPayload payload)
        {
            VTubeStudioWebsocketClient.Instance.Send(new MoveModelRequest()
            {
                PositionX = _settings.PosX,
                PositionY = _settings.PosY,
                RelativeMove =  _settings.Relative,
                Rotation = _settings.Rotation,
                Size = null,
                TimeInSeconds = _settings.Seconds,
            });
        }

        public override void KeyReleased(KeyPayload payload)
        {
        }

        public override void ReceivedSettings(ReceivedSettingsPayload payload)
        {
            try
            {
                Tools.AutoPopulateSettings(_settings, payload.Settings);
            }
            catch (Exception e)
            {
                Logger.Instance.LogMessage(TracingLevel.FATAL, e.ToString());
            }
        }

        public override void ReceivedGlobalSettings(ReceivedGlobalSettingsPayload payload)
        {
        }

        public override async void OnTick()
        {
            await SendDataToClient();
            
            VTubeStudioWebsocketClient.Instance.ConnectIfNeeded();
            
            if (!VTubeStudioWebsocketClient.Instance.IsAuthed)
                await Connection.ShowAlert();
        }

        public override void Dispose()
        {
            Connection.OnPropertyInspectorDidAppear -= PropertyInspectorDidAppear;
            Connection.OnSendToPlugin -= DataFromPropertyInspector;
        }
    }
}

