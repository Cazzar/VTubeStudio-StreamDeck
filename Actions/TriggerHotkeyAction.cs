using System;
using System.Collections.Generic;
using System.Drawing;
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
    public class TriggerHotkeyAction : BaseAction<TriggerHotkeyAction.PluginSettings>
    {
        public class PluginSettings
        {
            [JsonProperty("modelId")]
            public string ModelId { get; set; } = string.Empty;

            [JsonProperty("hotkeyId")]
            public string HotkeyId { get; set; } = string.Empty;

            [JsonProperty("showName")]
            public bool ShowName { get; set; } = true;
        }
        
        public TriggerHotkeyAction(ISDConnection connection, InitialPayload payload) : base(connection, payload)
        {
            ModelCache.Instance.ModelCacheUpdated += ModelCacheUpdate;
            HotkeyCache.Instance.Updated += HotkeyCacheUpdated;
        }

        protected override void Pressed(KeyPayload payload)
        {
            if (string.IsNullOrEmpty(Settings.HotkeyId))
            {
                Logger.Instance.LogMessage(TracingLevel.INFO, "Tried to press button but hotkey ID is null");
                return;
            }

            Vts.Send(new HotkeyTriggerRequest(Settings.HotkeyId));
        }

        protected override void Released(KeyPayload payload)
        {
        }

        protected override object GetClientData()
        {
            var models = ModelCache.Instance.Models.Select(s => new VTubeReference() { Id = s.Id, Name = s.Name }).ToList();
            List<VTubeReference> hotkeys = new();
            if (!string.IsNullOrEmpty(Settings.ModelId) && HotkeyCache.Instance.Hotkeys != null)
            {
                HotkeyCache.Instance.Hotkeys.TryGetValue(Settings.ModelId, out var keys);
                hotkeys = keys?.Select(s => new VTubeReference { Id = s.Id, Name = $"{s.Name} - ({s.ButtonTitle})", }).ToList() ?? new();
            }

            return new { Models = models, Hotkeys = hotkeys, Connected = VTubeStudioWebsocketClient.Instance.IsAuthed };
        }

        public override void Refresh(PluginPayload pl)
        {
            ModelCache.Instance.Update();
            base.Refresh(pl);
        }

        public override void ReceivedSettings(ReceivedSettingsPayload payload)
        {
            var showName = Settings.ShowName;
            base.ReceivedSettings(payload);

            if (showName != Settings.ShowName && !Settings.ShowName) Title = null;
            if (Settings.ShowName) Title = GetTitle();
        }

        public override void OnTick()
        {
            base.OnTick();

            if (!Settings.ShowName) return;
            Title = GetTitle();
        }

        private string GetTitle()
        {
            if (string.IsNullOrEmpty(Settings.ModelId)) return "";
            
            var hotkeys = HotkeyCache.Instance.Hotkeys;
            if (!hotkeys.ContainsKey(Settings.ModelId)) return "";
            
            var hotkey = hotkeys[Settings.ModelId]?.FirstOrDefault(s => s.Id == Settings.HotkeyId);
            var title = hotkey?.ButtonTitle ?? "";

            if (string.IsNullOrWhiteSpace(title) && hotkey != null)
                title = hotkey.ButtonTitle;
            return title;
        }

        public override void Dispose()
        {
            base.Dispose();
            ModelCache.Instance.ModelCacheUpdated -= ModelCacheUpdate;
            HotkeyCache.Instance.Updated -= HotkeyCacheUpdated;
        }

        private async void HotkeyCacheUpdated(object sender, HotkeyCacheUpdatedEventArgs e) => await UpdateClient();
        private async void ModelCacheUpdate(object sender, ModelCacheUpdatedEventArgs e) => await UpdateClient();
    }
}
