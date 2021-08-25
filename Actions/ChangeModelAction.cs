using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using BarRaider.SdTools;
using BarRaider.SdTools.Events;
using BarRaider.SdTools.Wrappers;
using Cazzar.StreamDeck.VTubeStudio.Models;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Models;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Requests;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Responses;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog.Fluent;

namespace Cazzar.StreamDeck.VTubeStudio.Actions
{
    [PluginActionId("dev.cazzar.vtubestudio.changemodel")]
    public class ChangeModelAction : PluginBase
    {
        private class PluginSettings
        {
            [JsonProperty("modelId")]
            public string ModelId { get; set; } = String.Empty;

            [JsonProperty("models")]
            public List<VTubeReference> Models { get; set; } = null;

            [JsonProperty("showName")]
            public bool ShowName { get; set; } = true;
        }

        private readonly PluginSettings _settings;
        private TitleParameters _titleParms;

        static ChangeModelAction()
        {
            VTubeStudioWebsocketClient.OnModelLoad += (sender, args) => RateLimiter.ChangeModel.Trigger();
        }
        
        public ChangeModelAction(ISDConnection connection, InitialPayload payload) : base(connection, payload)
        {
            //Ensure global settings is loaded;
            GlobalSettingsManager.Instance.RequestGlobalSettings();
            
            VTubeStudioWebsocketClient.Instance.ConnectIfNeeded();
            if (payload.Settings == null || payload.Settings.Count == 0)
                _settings = new();
            else
                _settings = payload.Settings.ToObject<PluginSettings>();

            Connection.OnPropertyInspectorDidAppear += ConnectionOnOnPropertyInspectorDidAppear;
            Connection.OnTitleParametersDidChange += TitleParamsUpdated;
            ModelCache.Instance.ModelCacheUpdated += ModelCacheUpdated;
            Connection.OnSendToPlugin += DataFromPropertyInspector;
        }
        
        private void DataFromPropertyInspector(object sender, SDEventReceivedEventArgs<SendToPlugin> e)
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
                    ModelCache.Instance.Update();
                    break;
            }
        }

        private void TitleParamsUpdated(object sender, SDEventReceivedEventArgs<TitleParametersDidChange> e)
        {
            _titleParms = e?.Event?.Payload?.TitleParameters;
        }

        private async void ModelCacheUpdated(object sender, ModelCacheUpdatedEventArgs e)
        {
            await SendDataToClient();
        }

        private async void ConnectionOnOnPropertyInspectorDidAppear(object sender, SDEventReceivedEventArgs<PropertyInspectorDidAppear> e)
        {
            await SendDataToClient();
        }
        
        private async Task SendDataToClient()
        {
            var models = ModelCache.Instance.Models.Select(s => new VTubeReference() {Id = s.Id, Name = s.Name}).ToList();

            await Connection.SendToPropertyInspectorAsync(JObject.FromObject(new {Models = models}));
        }
        
        private Task SaveSettings()
        {
            Logger.Instance.LogMessage(TracingLevel.INFO, $"Settings: {JsonConvert.SerializeObject(_settings)}");
            return Connection.SetSettingsAsync(JObject.FromObject(_settings));
        }
        
        public override void KeyPressed(KeyPayload payload)
        {
            Logger.Instance.LogMessage(TracingLevel.INFO, "Key Pressed");

            if (string.IsNullOrEmpty(_settings.ModelId))
            {
                Logger.Instance.LogMessage(TracingLevel.WARN, "Key pressed but model id is null!");
                return;
            }

            if (!RateLimiter.ChangeModel.IsReady)
            {
                Logger.Instance.LogMessage(TracingLevel.WARN, "Key pressed but rate limit hit!");
                return;
            }
            
            VTubeStudioWebsocketClient.Instance.Send(new ModelLoadRequest(_settings.ModelId));
        }

        public override void KeyReleased(KeyPayload payload)
        {

        }

        public override async void ReceivedSettings(ReceivedSettingsPayload payload)
        {
            var showName = _settings.ShowName;
            Tools.AutoPopulateSettings(_settings, payload.Settings);
            if (showName != _settings.ShowName && !_settings.ShowName)
                await Connection.SetTitleAsync(null);

            if (_settings.ShowName && _settings.Models != null)
                await Connection.SetTitleAsync(Tools.SplitStringToFit(ModelCache.Instance.Models.FirstOrDefault(s => s.Id == _settings.ModelId)?.Name ?? "", _titleParms));
        }

        public override void ReceivedGlobalSettings(ReceivedGlobalSettingsPayload payload) 
        {
            
        }

        public override async void OnTick()
        {
            if (_settings.ShowName && ModelCache.Instance.Models != null && _titleParms != null)
                await Connection.SetTitleAsync(Tools.SplitStringToFit(ModelCache.Instance.Models.FirstOrDefault(s => s.Id == _settings.ModelId)?.Name ?? "", _titleParms));
            await SendDataToClient();
        }

        public override void Dispose()
        {
            Connection.OnPropertyInspectorDidAppear -= ConnectionOnOnPropertyInspectorDidAppear;
            Connection.OnTitleParametersDidChange -= TitleParamsUpdated;
            ModelCache.Instance.ModelCacheUpdated -= ModelCacheUpdated;
            Connection.OnSendToPlugin -= DataFromPropertyInspector;
        }
    }
}
