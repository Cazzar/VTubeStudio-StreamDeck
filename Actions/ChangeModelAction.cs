using System.Collections.Generic;
using System.Linq;
using BarRaider.SdTools;
using Cazzar.StreamDeck.VTubeStudio.Models;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Requests;
using Newtonsoft.Json;

namespace Cazzar.StreamDeck.VTubeStudio.Actions
{
    [PluginActionId("dev.cazzar.vtubestudio.changemodel")]
    public class ChangeModelAction : BaseAction<ChangeModelAction.PluginSettings>
    {
        static ChangeModelAction()
        {
            VTubeStudioWebsocketClient.OnModelLoad += (sender, args) => RateLimiter.ChangeModel.Trigger();
        }

        public ChangeModelAction(ISDConnection connection, InitialPayload payload) : base(connection, payload) => ModelCache.Instance.ModelCacheUpdated += ModelCacheUpdated;

        private async void ModelCacheUpdated(object sender, ModelCacheUpdatedEventArgs e)
        {
            await UpdateClient();
        }

        protected override void Released(KeyPayload payload)
        {
        }

        protected override object GetClientData() => new
        {
            Models = ModelCache.Instance.Models.Select(s => new VTubeReference { Id = s.Id, Name = s.Name }).ToList(), 
            Connected = VTubeStudioWebsocketClient.Instance,
        };

        protected override void Pressed(KeyPayload payload)
        {
            Logger.Instance.LogMessage(TracingLevel.INFO, "Key Pressed");

            if (string.IsNullOrEmpty(Settings.ModelId))
            {
                Logger.Instance.LogMessage(TracingLevel.WARN, "Key pressed but model id is null!");
                return;
            }

            if (!RateLimiter.ChangeModel.IsReady)
            {
                Logger.Instance.LogMessage(TracingLevel.WARN, "Key pressed but rate limit hit!");
                return;
            }

            Vts.Send(new ModelLoadRequest(Settings.ModelId));
        }

        public override void ReceivedSettings(ReceivedSettingsPayload payload)
        {
            var showName = Settings.ShowName;
            base.ReceivedSettings(payload);

            if (showName != Settings.ShowName && !Settings.ShowName)
                Title = null;

            if (Settings.ShowName)
                Title = ModelCache.Instance.Models.FirstOrDefault(s => s.Id == Settings.ModelId)?.Name ?? "";
        }

        public override async void OnTick()
        {
            base.OnTick();

            if (Settings.ShowName)
                Title = ModelCache.Instance.Models.FirstOrDefault(s => s.Id == Settings.ModelId)?.Name ?? "";

            if (Settings.ModelId != null && Settings.ModelId == StateManager.CurrentModelId)
                await Connection.ShowOk();
        }

        public override void Dispose()
        {
            base.Dispose();
            ModelCache.Instance.ModelCacheUpdated -= ModelCacheUpdated;
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
