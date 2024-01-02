using System;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Events;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Requests;
using Microsoft.Extensions.Logging;
using StreamDeckLib;
using StreamDeckLib.Models;
using StreamDeckLib.Models.StreamDeckPlus;

namespace Cazzar.StreamDeck.VTubeStudio.Actions;

[StreamDeckAction("dev.cazzar.vtubestudio.movemodel.y")]
public class MoveModelY : BaseAction<MoveModelY.Settings>, IStreamDeckPlus, IDisposable
{
    private double _currentPosition = 0;
    
    public class Settings;

    public MoveModelY(GlobalSettingsManager gsm, VTubeStudioWebsocketClient vts, IStreamDeckConnection isd, ILogger<MoveModelY> logger) : base(gsm, vts, isd, logger)
    {
        VTubeStudioWebsocketClient.OnModelMove += ModelMove;
    }
    
    private async void ModelMove(object sender, ApiEventArgs<ModelMoveEvent> e)
    {
        _currentPosition = e.Response.Position.Y;
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
            PositionY = 0,
            TimeInSeconds = 0.05d,
        });
    }
    protected override void Released()
    {
    }

    protected override object GetClientData() => null;
    
    protected override void SettingsUpdated(Settings oldSettings, Settings newSettings)
    {
    }
    public void Touch(TouchTapPayload touchTap) => Pressed();
    
    public void DialDown(DialPressPayload dialDown) => Pressed();

    public void DialUp(DialPressPayload dialDown) => Released();
    
    public void DialRotate(DialRotatePayload dialRotatePayload)
    {
        Vts.Send(new MoveModelRequest
        {
            PositionY = Math.Clamp(_currentPosition + (dialRotatePayload.Ticks / 30d), -2, 2),
            TimeInSeconds = 0.05d,
        });
    }
    
    public void Dispose()
    {
        VTubeStudioWebsocketClient.OnModelMove -= ModelMove;
    }
}