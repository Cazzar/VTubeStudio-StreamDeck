using System;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Events;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Requests;
using Microsoft.Extensions.Logging;
using StreamDeckLib;
using StreamDeckLib.Models;
using StreamDeckLib.Models.StreamDeckPlus;

namespace Cazzar.StreamDeck.VTubeStudio.Actions;

[StreamDeckAction("dev.cazzar.vtubestudio.rotatemodel")]
public class RotateModelAction : BaseAction<RotateModelAction.Settings>, IStreamDeckPlus, IDisposable
{
    private double _currentRotation = 0;
    
    public class Settings;

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
    
    protected override void SettingsUpdated(Settings oldSettings, Settings newSettings)
    {
    }
    public void Touch(TouchTapPayload touchTap) => Pressed();
    
    public void DialDown(DialPressPayload dialDown) => Pressed();

    public void DialUp(DialPressPayload dialDown) => Released();
    
    public void DialRotate(DialRotatePayload dialRotatePayload)
    {
        _currentRotation += dialRotatePayload.Ticks + ((Math.Sign(dialRotatePayload.Ticks) * Math.Pow(2, Math.Clamp(Math.Abs(dialRotatePayload.Ticks), 1, 8) - 1)) / 2);
        
        Vts.Send(new MoveModelRequest
        {
            Rotation = _currentRotation % 360d,
            TimeInSeconds = 0.05d,
        });
    }
    
    public void Dispose()
    {
        VTubeStudioWebsocketClient.OnModelMove -= ModelMove;
    }
}