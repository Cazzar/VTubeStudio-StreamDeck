using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Cazzar.StreamDeck.VTubeStudio.Models;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using StreamDeckLib;
using StreamDeckLib.Models;

namespace Cazzar.StreamDeck.VTubeStudio.Actions
{
    public abstract class BaseAction<T> : StreamDeckLib.BaseAction<T>, IPropertyInspector, ITickHandler where T : new()
    {
        protected VTubeStudioWebsocketClient Vts;
        private readonly Dictionary<string, MethodInfo> _commands = new ();
        protected readonly GlobalSettingsManager Gsm;
        protected readonly ILogger _logger;

        public BaseAction(GlobalSettingsManager gsm, VTubeStudioWebsocketClient vts, IStreamDeckConnection isd, ILogger logger) : base(isd)
        {
            Gsm = gsm;
            Vts = vts;
            _logger = logger;
            Vts.ConnectIfNeeded();
            LoadCommands();
        }

        private void LoadCommands()
        {
            foreach (var methodInfo in this.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public))
            {
                var attribute = methodInfo.GetCustomAttribute<PluginCommandAttribute>();
                if (attribute is null)
                    continue;

                _commands.Add(attribute.Command.ToLower(), methodInfo);
            }
        }

        public override void KeyDown(KeyActionPayload keyActionPayload)
        {
            Vts.ConnectIfNeeded();
            if (!Vts.IsAuthed)
            {
                ShowAlert();
                return;
            }

            Pressed();
        }

        public override void KeyUp(KeyActionPayload keyActionPayload) => Released();

        protected async Task UpdateClient()
        {
            await Connection.SendMessage(new SendToPropertyInspector() { Context = this.ContextId, Payload = GetClientData(), });
        }

        [PluginCommand("refresh")]
        public virtual async void Refresh(PluginPayload pl) => await UpdateClient();

        [PluginCommand("force-reconnect")]
        public void ForceReconnect(PluginPayload pl) => Vts.Reconnect();

        [PluginCommand("set-vtsinfo")]
        public void SetVtsInfo(PluginPayload pl)
        {
            string host = pl.Payload.host;
            ushort port = pl.Payload.port;
            Gsm.SetVts(host, port);
        }

        protected abstract void Pressed();
        protected abstract void Released();
        protected abstract object GetClientData();
        protected abstract void SettingsUpdated(T oldSettings, T newSettings);

        public async void Appeared(PropertyInspectorDidAppear didAppear)
        {
            await UpdateClient();
        }

        public void Disappeared(PropertyInspectorDidDisappear didDisappear)
        {
        }

        public void OnSendToPlugin(SendToPlugin sendToPlugin)
        {
            var pl = sendToPlugin.Payload.ToObject<PluginPayload>();
            if (pl?.Command == null)
            {
                return;
            }

            var command = pl.Command.ToLower();
            if (!_commands.ContainsKey(command))
                return;

            _commands[command].Invoke(this, new object[] { pl });
        }

        public override void GotSettings(JObject settings)
        {
            var oldSettings = Settings;
            base.GotSettings(settings);

            SettingsUpdated(oldSettings, Settings);
        }

        public virtual async void Tick()
        {
            Vts.ConnectIfNeeded();
            await UpdateClient();
        }
    }
}
