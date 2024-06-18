using System;
using System.Threading.Tasks;
using Cazzar.StreamDeck.VTubeStudio.Models;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Events;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Requests;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Responses;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StreamDeckLib;

namespace Cazzar.StreamDeck.VTubeStudio.Actions;

[StreamDeckAction("dev.cazzar.vtubestudio.holdtozoom")]
public class HoldToZoom : BaseAction<HoldToZoom.HoldToZoomPayload>
{
    public class HoldToZoomPayload
    {
        
        [JsonProperty("seconds")]
        public double? Seconds { get; set; } = 0;
            
        [JsonProperty("posX")]
        public double? PosX { get; set; } = 0;

        [JsonProperty("posY")]
        public double? PosY { get; set; } = 0;

        [JsonProperty("rotation")]
        public double? Rotation { get; set; } = 0;

        [JsonProperty("size")]
        public double? Size { get; set; } = null;
    }

    private ModelPosition _lastPosition; 
    private bool _held;

    public HoldToZoom(GlobalSettingsManager gsm, VTubeStudioWebsocketClient vts, IStreamDeckConnection isd, ILogger<HoldToZoom> logger) : base(gsm, vts, isd, logger)
    {
        VTubeStudioWebsocketClient.OnModelMove += OnModelMove;
    }
    private void OnModelMove(object sender, ApiEventArgs<ModelMoveEvent> e)
    {
        if (_held) return;
        
        _lastPosition = e.Response.Position;
    }

    protected override void Pressed()
    {
        _held = true;
        Vts.Send(new MoveModelRequest()
        {
            PositionX = Settings.PosX,
            PositionY = Settings.PosY,
            RelativeMove = false,
            Rotation = Settings.Rotation,
            Size = Settings.Size,
            TimeInSeconds = Settings.Seconds,
        });
    }
    protected override void Released()
    {
        Task.Delay(TimeSpan.FromSeconds(Settings.Seconds ?? 0)).ContinueWith(task => _held = false);
        
        Vts.Send(new MoveModelRequest()
        {
            PositionX = _lastPosition.X,
            PositionY = _lastPosition.Y,
            RelativeMove = false,
            Rotation = _lastPosition.Rotation,
            Size = _lastPosition.Size,
            TimeInSeconds = Settings.Seconds,
        });
    }
    protected override void SettingsUpdated(HoldToZoomPayload oldSettings, HoldToZoomPayload newSettings)
    {
    }

    [PluginCommand("get-params")]
    public virtual void GetParams(PluginPayload pl)
    {
        Settings.Rotation = _lastPosition.Rotation;
        Settings.Size = _lastPosition.Size;
        Settings.PosX = _lastPosition.X;
        Settings.PosY = _lastPosition.Y;
        SaveSettings();
    }
}
