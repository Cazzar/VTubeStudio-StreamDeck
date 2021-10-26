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
    public class ChangeModelAction : BaseAction<ChangeModelAction.PluginSettings>
    {
        private readonly ILogger<ChangeModelAction> _logger;
        private readonly ModelCache _modelCache;

        static ChangeModelAction()
        {
            VTubeStudioWebsocketClient.OnModelLoad += (sender, args) => RateLimiter.ChangeModel.Trigger();
        }

        public ChangeModelAction(GlobalSettingsManager gsm, VTubeStudioWebsocketClient vts, IStreamDeckConnection isd, ILogger<ChangeModelAction> logger, ModelCache modelCache) : base(gsm, vts, isd, logger)
        {
            _logger = logger;
            _modelCache = modelCache;
        }

        protected override void Released()
        {
        }

        protected override object GetClientData() => new
        {
            Models = _modelCache.Models.Select(s => new VTubeReference { Id = s.Id, Name = s.Name }).ToList(), 
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

            if (Settings.ShowName)
                SetTitle(_modelCache.Models.FirstOrDefault(s => s.Id == Settings.ModelId)?.Name ?? "");
        }

        public override void Tick()
        {
            base.Tick();
            _logger.LogDebug("Tick Handler called.... {VtsIsAuthed}, {VtsSocketIsAlive}", Vts.IsAuthed, Vts.WsIsAlive);
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
