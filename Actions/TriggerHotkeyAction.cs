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

namespace Cazzar.StreamDeck.VTubeStudio.Actions
{
    [PluginActionId("dev.cazzar.vtubestudio.triggerhotkey")]
    public class TriggerHotkeyAction : PluginBase
    {
        private class PluginSettings
        {
            [JsonProperty("modelId")]
            public string ModelId { get; set; } = string.Empty;

            [JsonProperty("hotkeyId")]
            public string HotkeyId { get; set; } = string.Empty;

            [JsonProperty("showName")]
            public bool ShowName { get; set; } = true;
        }
        
        private readonly PluginSettings _settings;
        private TitleParameters _titleParms;
        
        public TriggerHotkeyAction(ISDConnection connection, InitialPayload payload) : base(connection, payload)
        {
            //Ensure global settings is loaded;
            GlobalSettingsManager.Instance.Load();
            VTubeStudioWebsocketClient.Instance.ConnectIfNeeded();
            
            if (payload.Settings == null || payload.Settings.Count == 0)
                _settings = new PluginSettings();
            else
                _settings = payload.Settings.ToObject<PluginSettings>();
            
            ModelCache.Instance.ModelCacheUpdated += ModelCacheUpdate;
            HotkeyCache.Instance.Updated += HotkeyCacheUpdated;
            Connection.OnTitleParametersDidChange += TitleParamsUpdated;
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
            Logger.Instance.LogMessage(TracingLevel.INFO, nameof(PropertyInspectorDidAppear));

            await SendDataToClient();
        }

        private async Task SendDataToClient()
        {
            var models = ModelCache.Instance.Models.Select(s => new VTubeReference() {Id = s.Id, Name = s.Name}).ToList();
            List<VTubeReference> hotkeys = new ();
            if (!string.IsNullOrEmpty(_settings.ModelId) && HotkeyCache.Instance.Hotkeys != null)
            {
                HotkeyCache.Instance.Hotkeys.TryGetValue(_settings.ModelId, out var keys);
                hotkeys = keys?.Select(s => new VTubeReference
                {
                    Id = s.Id, 
                    Name = $"{s.Name} - ({s.ButtonTitle})",
                }).ToList() ?? new ();
            }

            await Connection.SendToPropertyInspectorAsync(JObject.FromObject(new {Models = models, Hotkeys = hotkeys, Connected = VTubeStudioWebsocketClient.Instance.IsAuthed}));
        }

        private async void HotkeyCacheUpdated(object sender, HotkeyCacheUpdatedEventArgs e)
        {
            await SendDataToClient();
        }

        private void TitleParamsUpdated(object sender, SDEventReceivedEventArgs<TitleParametersDidChange> e)
        {
            _titleParms = e?.Event?.Payload?.TitleParameters;
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
            if (string.IsNullOrEmpty(_settings.HotkeyId))
            {
                Logger.Instance.LogMessage(TracingLevel.INFO, "Tried to press button but hotkey ID is null");
                return;
            }
            
            VTubeStudioWebsocketClient.Instance.Send(new HotkeyTriggerRequest(_settings.HotkeyId));
        }

        public override void KeyReleased(KeyPayload payload)
        {
        }

        public override　async void ReceivedSettings(ReceivedSettingsPayload payload)
        {
            var showName = _settings.ShowName;
            var modelId = _settings.ModelId;
            Tools.AutoPopulateSettings(_settings, payload.Settings);

            if (showName != _settings.ShowName && !_settings.ShowName)
                await Connection.SetTitleAsync(null);
            
            await UpdateTitle();
        }

        private async Task UpdateTitle()
        {
            var hotkeys = HotkeyCache.Instance.Hotkeys;

            if (_settings.ShowName && hotkeys.ContainsKey(_settings.ModelId))
            {
                var hotkey = hotkeys[_settings.ModelId]?.FirstOrDefault(s => s.Id == _settings.HotkeyId);
                var title = hotkey?.Name ?? "";

                if (string.IsNullOrWhiteSpace(title) && hotkey != null)
                    title = hotkey.ButtonTitle;
                
                await Connection.SetTitleAsync(Tools.SplitStringToFit(title, _titleParms));
            }
            
            await SendDataToClient();
        }

        public override void ReceivedGlobalSettings(ReceivedGlobalSettingsPayload payload)
        {
        }

        public override async void OnTick()
        {
            await UpdateTitle();
            
            await SendDataToClient();
            
            VTubeStudioWebsocketClient.Instance.ConnectIfNeeded();
            
            if (!VTubeStudioWebsocketClient.Instance.IsAuthed)
                await Connection.ShowAlert();
        }

        public override void Dispose()
        {
            ModelCache.Instance.ModelCacheUpdated -= ModelCacheUpdate;
            HotkeyCache.Instance.Updated -= HotkeyCacheUpdated;
            Connection.OnTitleParametersDidChange -= TitleParamsUpdated;
            Connection.OnPropertyInspectorDidAppear -= PropertyInspectorDidAppear;
            Connection.OnSendToPlugin -= DataFromPropertyInspector;

        }
    }
}
