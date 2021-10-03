using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using BarRaider.SdTools;
using BarRaider.SdTools.Events;
using BarRaider.SdTools.Wrappers;
using Cazzar.StreamDeck.VTubeStudio.Models;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Requests;
using Newtonsoft.Json.Linq;
using NLog.Internal;
using NLog.LayoutRenderers;

namespace Cazzar.StreamDeck.VTubeStudio.Actions
{
    public abstract class BaseAction<T> : PluginBase where T: class, new()
    {
        protected VTubeStudioWebsocketClient Vts => VTubeStudioWebsocketClient.Instance;
        protected T Settings;
        private TitleParameters _titleParms;
        private readonly Dictionary<string, MethodInfo> _commands = new ();

        private string _title;
        protected string Title
        {
            get => _title;
            set
            {
                _title = value;
                Connection.SetTitleAsync(value is null ? null : Tools.SplitStringToFit(value, _titleParms));
            }
        }

        public BaseAction(ISDConnection connection, InitialPayload payload) : base(connection, payload)
        {
            GlobalSettingsManager.Instance.Load();
            Vts.ConnectIfNeeded();

            if (payload.Settings == null || payload.Settings.Count == 0)
                Settings = new T();
            else
                Settings = payload.Settings.ToObject<T>();
        
            LoadCommands();
            
            Connection.OnSendToPlugin += DataFromPropertyInspector;
            Connection.OnPropertyInspectorDidAppear += PropertyInspectorDidAppear;
            Connection.OnTitleParametersDidChange += TitleParamsUpdated;
        }

        private async void PropertyInspectorDidAppear(object sender, SDEventReceivedEventArgs<PropertyInspectorDidAppear> e)
        {
            await UpdateClient();
        }
        
        private void TitleParamsUpdated(object sender, SDEventReceivedEventArgs<TitleParametersDidChange> e)
        {
            _titleParms = e?.Event?.Payload?.TitleParameters;
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

        private void DataFromPropertyInspector(object sender, SDEventReceivedEventArgs<SendToPlugin> e)
        {
            var pl = e.Event.Payload.ToObject<PluginPayload>();

            if (pl?.Command == null)
            {
                Logger.Instance.LogMessage(TracingLevel.INFO, "Payload is null");
                return;
            }

            var command = pl.Command.ToLower();
            if (!_commands.ContainsKey(command))
                return;

            _commands[command].Invoke(this, new object[] { pl });
        }
        
        public override async void KeyPressed(KeyPayload payload)
        {
            if (!Vts.IsAuthed)
            {
                await Connection.ShowAlert();
                return;
            }
            
            Pressed(payload);
        }
        
        public override void KeyReleased(KeyPayload payload)
        {
            Released(payload);
        }
        
        public override void ReceivedSettings(ReceivedSettingsPayload payload)
        {
            try
            {
                Settings = payload.Settings.ToObject<T>();
                // Tools.AutoPopulateSettings(_settings, payload.Settings);
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
            await UpdateClient();
            
            VTubeStudioWebsocketClient.Instance.ConnectIfNeeded();
        }

        public override void Dispose()
        {
            Connection.OnSendToPlugin -= DataFromPropertyInspector;
            Connection.OnPropertyInspectorDidAppear -= PropertyInspectorDidAppear;
            Connection.OnTitleParametersDidChange -= TitleParamsUpdated;
        }

        protected async Task UpdateClient()
        {
            await Connection.SendToPropertyInspectorAsync(JObject.FromObject(this.GetClientData()));
        }
        
        protected async Task SaveSettings()
        {
            var settings = JObject.FromObject(Settings);
            await Connection.SetSettingsAsync(settings);
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
            GlobalSettingsManager.Instance.SetVts(host, port);
        }

        protected abstract void Pressed(KeyPayload payload);
        protected abstract void Released(KeyPayload payload);
        protected abstract object GetClientData();
    }
}
