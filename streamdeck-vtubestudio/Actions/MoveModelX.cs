using System;
using Cazzar.StreamDeck.VTubeStudio.Models;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Events;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Requests;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StreamDeckLib;
using StreamDeckLib.Models;
using StreamDeckLib.Models.StreamDeckPlus;

namespace Cazzar.StreamDeck.VTubeStudio.Actions;

[StreamDeckAction("dev.cazzar.vtubestudio.movemodel.x")]
public class MoveModelX : BaseAction<MoveModelX.MoveSettings>, IStreamDeckPlus, IDisposable
{
    private double _currentPosition = 0;

    public class MoveSettings
    {
        [JsonProperty("stepSize")]
        public int StepSize { get; set; } = 2;
        [JsonProperty("defaultPosition")]
        public double DefaultPosition { get; set; } = 0.0d;
    };

    public MoveModelX(GlobalSettingsManager gsm, VTubeStudioWebsocketClient vts, IStreamDeckConnection isd, ILogger<MoveModelX> logger) : base(gsm, vts, isd, logger)
    {
        VTubeStudioWebsocketClient.OnModelMove += ModelMove;
    }
    
    private async void ModelMove(object sender, ApiEventArgs<ModelMoveEvent> e)
    {
        _currentPosition = e.Response.Position.X;
        var percentage = (_currentPosition + 1d) / 2d;
        
        await Connection.SendMessage(new SetFeedback() { Context = this.ContextId, Payload = new()
        {
            {"value", $"{percentage:P1}"},
            {"indicator", (int) (percentage * 100) },
        } });
    }

    protected override void Pressed()
    {
        Vts.Send(new MoveModelRequest
        {
            PositionX = Settings.DefaultPosition,
            TimeInSeconds = 0.05d,
        });
    }
    protected override void Released()
    {
    }
    
    protected override void SettingsUpdated(MoveSettings oldSettings, MoveSettings newSettings)
    {
    }
    
    public void Touch(TouchTapPayload touchTap) => Pressed();
    
    public void DialDown(DialPressPayload dialDown) => Pressed();

    public void DialUp(DialPressPayload dialDown) => Released();
    
    public void DialRotate(DialRotatePayload dialRotatePayload)
    {
        _currentPosition = Math.Clamp(_currentPosition + (dialRotatePayload.Ticks * (Settings.StepSize / 200d)), -2, 2);
        
        Vts.Send(new MoveModelRequest
        {
            PositionX = _currentPosition,
            TimeInSeconds = 0.01d,
        });
    }

    [PluginCommand("use-current")]
    public void UseCurrent(PluginPayload pl)
    {
        Settings.DefaultPosition = _currentPosition;
        SaveSettings();
    }
    
    public void Dispose()
    {
        VTubeStudioWebsocketClient.OnModelMove -= ModelMove;
    }
}