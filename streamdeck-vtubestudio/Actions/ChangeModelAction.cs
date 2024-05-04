using System.Collections.Generic;
using System.Linq;
using Cazzar.StreamDeck.VTubeStudio.Models;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Requests;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StreamDeckLib;

namespace Cazzar.StreamDeck.VTubeStudio.Actions
{
    [StreamDeckAction("dev.cazzar.vtubestudio.changemodel")]
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ChangeModelAction(GlobalSettingsManager gsm, VTubeStudioWebsocketClient vts, IStreamDeckConnection isd, ILogger<ChangeModelAction> logger, ModelCache modelCache)
        : BaseAction<ChangeModelAction.PluginSettings>(gsm, vts, isd, logger)
    {

        static ChangeModelAction()
        {
            VTubeStudioWebsocketClient.OnModelLoad += (sender, args) => RateLimiter.ChangeModel.Trigger();
        }

        protected override void Released()
        {
        }

        protected override object GetClientData() => new
        {
            Models = modelCache.Models.Select(s => new VTubeReference { Id = s.Id, Name = s.Name }).ToList(),
            Connected = Vts.WsIsAlive,
        };

        protected override void Pressed()
        {
            _logger.LogInformation("Key Pressed");

            if (string.IsNullOrEmpty(Settings.ModelId))
            {
                _logger.LogWarning("Key pressed but model id is null!");
                return;
            }

            if (!RateLimiter.ChangeModel.IsReady)
            {
                _logger.LogWarning("Key pressed but rate limit hit!");
                return;
            }

            Vts.Send(new ModelLoadRequest(Settings.ModelId));
        }

        protected override void SettingsUpdated(PluginSettings oldSettings, PluginSettings newSettings)
        {
            if (oldSettings.ShowName != newSettings.ShowName && !newSettings.ShowName)
                SetTitle(null);

            UpdateTitle();
        }

        private string GetTitle()
        {
            var model = modelCache.Models.FirstOrDefault(m => m.Id == Settings.ModelId);
            return model?.Name;
        }

        public override void Tick()
        {
            base.Tick();

            UpdateTitle();
        }

        private void UpdateTitle()
        {
            var title = GetTitle();

            if (!string.IsNullOrEmpty(title) && Settings.ShowName)
                SetTitle(title);
        }

        public class PluginSettings
        {
            [JsonProperty("modelId")]
            public string ModelId { get; set; } = string.Empty;

            [JsonProperty("models")]
            public List<VTubeReference> Models { get; set; }

            [JsonProperty("showName")]
            public bool ShowName { get; set; } = true;
        }
    }
}
