using System;
using Cazzar.StreamDeck.VTubeStudio.Models;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Events;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Requests;
using Cazzar.StreamDeck.VTubeStudio.VTubeStudioApi.Responses;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StreamDeckLib;
using StreamDeckLib.Models;
using StreamDeckLib.Models.StreamDeckPlus;

namespace Cazzar.StreamDeck.VTubeStudio.Actions;

[StreamDeckAction("dev.cazzar.vtubestudio.zoommodel")]
public class ZoomModelAction
    : BaseAction<ZoomModelAction.PluginSettings>, IStreamDeckPlus, IDisposable
{
    private double _currentSize = 0;

    public ZoomModelAction(GlobalSettingsManager gsm, VTubeStudioWebsocketClient vts, IStreamDeckConnection isd, ILogger<ZoomModelAction> logger) : base(gsm, vts, isd, logger)
    {
        VTubeStudioWebsocketClient.OnCurrentModelInformation += CurrentModelInformation;
        VTubeStudioWebsocketClient.OnModelMove += ModelMove;
    }
    private void ModelMove(object sender, ApiEventArgs<ModelMoveEvent> e)
    {
        _currentSize = e.Response.Position.Size;
        var percentage = (_currentSize + 100d) / 200d;
        
        UpdateFeedback(percentage);
    }
    
    private void CurrentModelInformation(object sender, ApiEventArgs<CurrentModelResponse> e)
    {
        _currentSize = e.Response.ModelPosition.Size;
        var percentage = (_currentSize + 100d) / 200d;
        if (!e.Response.IsLoaded) percentage = 0;
        
        UpdateFeedback(percentage);
    }

    private async void UpdateFeedback(double percentage)
    {
        await Connection.SendMessage(new SetFeedback() { Context = this.ContextId, Payload = new()
        {
            {"value", $"{percentage:P0}"},
            {"indicator", (int) (percentage * 100) },
        } });
    }

    public class PluginSettings
    {
        [JsonProperty("stepSize")]
        public int StepSize { get; set; } = 2;
        [JsonProperty("defaultZoom")]
        public double DefaultZoom { get; set; } = 0.0d;
    }
    
    protected override void Pressed()
    {
        Vts.Send(new MoveModelRequest() { Size = Settings.DefaultZoom });
    }
    protected override void Released()
    {
    }
    
    protected override void SettingsUpdated(PluginSettings oldSettings, PluginSettings newSettings)
    {
    }
    
    public void Touch(TouchTapPayload touchTap) => Pressed();

    public void DialDown(DialPressPayload dialDown) { }

    public void DialUp(DialPressPayload dialDown) => Pressed();

    public void DialRotate(DialRotatePayload dialRotatePayload)
    {
        _currentSize  = Math.Clamp(_currentSize + (dialRotatePayload.Ticks * Settings.StepSize), -100, 100);

        Vts.Send(new MoveModelRequest() {Size = _currentSize, TimeInSeconds = 0.05d});

        UpdateFeedback((_currentSize + 100d) / 200d);
    }

    [PluginCommand("use-current")]
    public void UseCurrent(PluginPayload pl)
    {
        Settings.DefaultZoom = _currentSize;
        SaveSettings();
    }
    
    public void Dispose()
    {
        VTubeStudioWebsocketClient.OnCurrentModelInformation -= CurrentModelInformation;
        VTubeStudioWebsocketClient.OnModelMove -= ModelMove;
    }
}
