using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Cazzar.StreamDeck.VTubeStudio.Models;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Requests;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StreamDeckLib;

namespace Cazzar.StreamDeck.VTubeStudio.Actions
{
    [StreamDeckAction("dev.cazzar.vtubestudio.triggerhotkey")]
    public class TriggerHotkeyAction : BaseAction<TriggerHotkeyAction.PluginSettings>
    {
        private readonly ModelCache _modelCache;
        private readonly HotkeyCache _hotkeyCache;

        public class PluginSettings
        {
            [JsonProperty("modelId")]
            public string ModelId { get; set; } = string.Empty;

            [JsonProperty("hotkeyId")]
            public string HotkeyId { get; set; } = string.Empty;

            [JsonProperty("showName")]
            public bool ShowName { get; set; } = true;
        }

        protected override void Pressed()
        {
            if (string.IsNullOrEmpty(Settings.HotkeyId))
            {
                // Logger.Instance.LogMessage(TracingLevel.INFO, "Tried to press button but hotkey ID is null");
                return;
            }

            Vts.Send(new HotkeyTriggerRequest(Settings.HotkeyId));
        }

        protected override void Released()
        {
        }

        protected override object GetClientData()
        {
            var models = _modelCache.Models.Select(s => new VTubeReference() { Id = s.Id, Name = s.Name }).ToList();
            List<VTubeReference> hotkeys = new();
            if (!string.IsNullOrEmpty(Settings.ModelId) && _hotkeyCache.Hotkeys != null)
            {
                _hotkeyCache.Hotkeys.TryGetValue(Settings.ModelId, out var keys);
                hotkeys = keys?.Select(s => new VTubeReference { Id = s.Id, Name = $"{s.Name} - ({s.ButtonTitle})", }).ToList() ?? new();
            }

            return new { Models = models, Hotkeys = hotkeys, Connected = Vts.IsAuthed };
        }

        protected override void SettingsUpdated(PluginSettings oldSettings, PluginSettings newSettings)
        {
            if (newSettings.ShowName != oldSettings.ShowName && !Settings.ShowName)
                SetTitle(null);

            if (Settings.ShowName)
                SetTitle(GetTitle());
        }

        public override void Refresh(PluginPayload pl)
        {
            _modelCache.Update();
            base.Refresh(pl);
        }

        private string GetTitle()
        {
            if (string.IsNullOrEmpty(Settings.ModelId)) return "";
            
            var hotkeys = _hotkeyCache.Hotkeys;
            if (!hotkeys.ContainsKey(Settings.ModelId)) return "";
            
            var hotkey = hotkeys[Settings.ModelId]?.FirstOrDefault(s => s.Id == Settings.HotkeyId);
            var title = hotkey?.ButtonTitle ?? "";

            if (string.IsNullOrWhiteSpace(title) && hotkey != null)
                title = hotkey.ButtonTitle;
            return title;
        }

        public TriggerHotkeyAction(GlobalSettingsManager gsm, VTubeStudioWebsocketClient vts, IStreamDeckConnection isd, ILogger<TriggerHotkeyAction> logger, ModelCache modelCache, HotkeyCache hotkeyCache) : base(gsm, vts, isd, logger)
        {
            _modelCache = modelCache;
            _hotkeyCache = hotkeyCache;
        }
    }
}
