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

[StreamDeckAction("dev.cazzar.vtubestudio.rotatemodel")]
public class RotateModelAction : BaseAction<RotateModelAction.PluginSettings>, IStreamDeckPlus, IDisposable
{
    private double _currentRotation = 0;
    
    public class PluginSettings
    {
        [JsonProperty("stepSize")]
        public int StepSize { get; set; } = 2;
        [JsonProperty("defaultRotation")]
        public double DefaultRotation { get; set; } = 0.0d;
    }

    public RotateModelAction(GlobalSettingsManager gsm, VTubeStudioWebsocketClient vts, IStreamDeckConnection isd, ILogger<RotateModelAction> logger) : base(gsm, vts, isd, logger)
    {
        VTubeStudioWebsocketClient.OnModelMove += ModelMove;
    }
    
    private async void ModelMove(object sender, ApiEventArgs<ModelMoveEvent> e)
    {
        _currentRotation = e.Response.Position.Rotation;
        var rotation = _currentRotation % 360d;
        
        await Connection.SendMessage(new SetFeedback() { Context = this.ContextId, Payload = new()
        {
            {"value", $"{rotation:N2}\u00b0"},
        } });
    }

    protected override void Pressed()
    {
        Vts.Send(new MoveModelRequest
        {
            Rotation = 0,
            TimeInSeconds = 0.05d,
        });
    }
    protected override void Released()
    {
    }
    
    protected override void SettingsUpdated(PluginSettings oldSettings, PluginSettings newSettings)
    {
    }
    public void Touch(TouchTapPayload touchTap) => Pressed();
    
    public void DialDown(DialPressPayload dialDown) => Pressed();

    public void DialUp(DialPressPayload dialDown) => Released();
    
    public void DialRotate(DialRotatePayload dialRotatePayload)
    {
        _currentRotation += dialRotatePayload.Ticks * (Settings.StepSize / 10d);
        
        Vts.Send(new MoveModelRequest
        {
            Rotation = _currentRotation % 360d,
            TimeInSeconds = 0.05d,
        });
    }
    
    [PluginCommand("use-current")]
    public void UseCurrent(PluginPayload pl)
    {
        Settings.DefaultRotation = _currentRotation;
        SaveSettings();
    }

    
    public void Dispose()
    {
        VTubeStudioWebsocketClient.OnModelMove -= ModelMove;
    }
}