using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Threading.Tasks.Dataflow;
using BarRaider.SdTools;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Requests;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Responses;
using Newtonsoft.Json.Linq;
using NLog.LayoutRenderers;

namespace Cazzar.StreamDeck.VTubeStudio
{
    public class GlobalSettings
    {
        public string Token { get; set; } = string.Empty;
        public string Host { get; set; } = "127.0.0.1";
        public ushort? Port { get; set; } = 8001;
    }

    public class GlobalSettingsManager
    {
        private static GlobalSettingsManager _instance;
        private static readonly object InstanceLock = new object();
        
        public static GlobalSettingsManager Instance// => new();
        {
            get
            {
                if (_instance != null) return _instance;
                lock (InstanceLock)
                {
                    _instance = new();
                    return _instance;
                }
            }
        }

        public GlobalSettings Settings { get; private set; } = new();

        private bool _loaded = false;
        
        private GlobalSettingsManager()
        {
            BarRaider.SdTools.GlobalSettingsManager.Instance.OnReceivedGlobalSettings += SdGlobalSettingsReceived;
            VTubeStudioWebsocketClient.Instance.ConnectIfNeeded();
        }

        private void SdGlobalSettingsReceived(object sender, ReceivedGlobalSettingsPayload e)
        {
            var current = Settings;
            
            Settings = e.Settings.ToObject<GlobalSettings>();
            if (Settings == null)
            {
                BarRaider.SdTools.GlobalSettingsManager.Instance.SetGlobalSettings(JObject.FromObject(new GlobalSettings()));
                return;
            }

            if (current.Token == Settings.Token && !string.IsNullOrEmpty(Settings.Token)) return;
            
            if (current.Host != Settings.Host || current.Port != Settings.Port)
                VTubeStudioWebsocketClient.Instance.SetConnection(Settings.Host ?? "127.0.0.1", Settings.Port ?? 8001);

            
            Logger.Instance.LogMessage(TracingLevel.INFO, "Token changed!");
            TokenChanged?.Invoke(this, Settings);
        }

        public void SaveSettings()
        {
            var settings = JObject.FromObject(Settings);
            
            BarRaider.SdTools.GlobalSettingsManager.Instance.SetGlobalSettings(settings);
        }

        public event EventHandler<GlobalSettings> TokenChanged;

        public void Load()
        {
            if (_loaded) return;
            RequestGlobalSettings();
            _loaded = true;
        }
        
        public void RequestGlobalSettings()
        {
            BarRaider.SdTools.GlobalSettingsManager.Instance.RequestGlobalSettings();
        }

        public void SetVts(string host, ushort port)
        {
            Settings.Host = host;
            Settings.Port = port;

            VTubeStudioWebsocketClient.Instance.SetConnection(host, port);
            
            SaveSettings();
        }
    }
}
