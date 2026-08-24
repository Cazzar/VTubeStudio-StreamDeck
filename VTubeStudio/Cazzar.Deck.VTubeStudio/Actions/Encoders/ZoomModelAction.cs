using Cazzar.Deck.Abstractions.Actions;
using Cazzar.Deck.Abstractions.Surfaces;
using Cazzar.Deck.Abstractions;
using System.Text.Json.Serialization;
using VTubeStudio.Api.Events;
using VTubeStudio.Api.Requests;
using VTubeStudio.Api.Responses;
using VTubeStudio.Api;

namespace Cazzar.Deck.VTubeStudio.Actions.Encoders;

[DeckAction(Name = "Zoom Model", Tooltip = "VTubeStudio [Zoom Model]",
    Icon = "vts_logo_transparent", PropertyView = "ZoomModel")]
public sealed class ZoomModelAction : EncoderAction<ZoomModelAction.Options>, IDisposable
{
    private double _size;

    public ZoomModelAction(DeckActionContext context, IVTubeStudio vts, IEncoderSurface encoder)
        : base(context, vts, encoder)
    {
        vts.ModelMoved += OnModelMoved;
        vts.CurrentModel += OnCurrentModel;
    }

    public sealed class Options
    {
        [JsonPropertyName("stepSize")] public int StepSize { get; set; } = 2;
        [JsonPropertyName("defaultZoom")] public double DefaultZoom { get; set; }
    }

    protected override void Pressed() => Vts.Send(new MoveModelRequest { Size = Settings.DefaultZoom });

    public override void DialRotate(int ticks, bool pressed)
    {
        _size = Math.Clamp(_size + ticks * Settings.StepSize, -100, 100);

        Vts.Send(new MoveModelRequest { Size = _size, TimeInSeconds = 0.05d });
        _ = ShowOnDialAsync(AsFraction(_size));
    }

    [PropertyViewCommand("use-current")]
    public void UseCurrent(IPayload body)
    {
        Settings.DefaultZoom = _size;
        _ = SaveSettingsAsync();
        _ = UpdateClientAsync();
    }

    private void OnModelMoved(object? sender, VtsEventArgs<ModelMovedEvent> e)
    {
        _size = e.Response.Position.Size;
        _ = ShowOnDialAsync(AsFraction(_size));
    }

    private void OnCurrentModel(object? sender, VtsEventArgs<CurrentModelResponse> e)
    {
        _size = e.Response.Position.Size;
        _ = ShowOnDialAsync(e.Response.IsLoaded ? AsFraction(_size) : 0);
    }

    private static double AsFraction(double size) => (size + 100d) / 200d;

    public void Dispose()
    {
        Vts.ModelMoved -= OnModelMoved;
        Vts.CurrentModel -= OnCurrentModel;
    }
}
