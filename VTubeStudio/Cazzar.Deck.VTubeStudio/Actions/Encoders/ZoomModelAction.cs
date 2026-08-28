using Cazzar.Deck.Abstractions.Actions;
using Cazzar.Deck.Abstractions.Surfaces;
using Cazzar.Deck.Abstractions;
using Cazzar.Deck.VTubeStudio.Actions.Movement;
using System.Text.Json.Serialization;
using VTubeStudio.Api.Requests;
using VTubeStudio.Api;

namespace Cazzar.Deck.VTubeStudio.Actions.Encoders;

[DeckAction(Name = "Zoom Model", Tooltip = "VTubeStudio [Zoom Model]",
    Icon = "vts_logo_transparent", PropertyView = "ZoomModel")]
public sealed class ZoomModelAction : EncoderAction<ZoomModelAction.Options>, IDisposable
{
    private readonly ModelPositionTracker _tracker;

    private double _size;

    public ZoomModelAction(
        DeckActionContext context, IVTubeStudio vts, IEncoderSurface encoder, ModelPositionTracker tracker)
        : base(context, vts, encoder)
    {
        _tracker = tracker;
        tracker.Updated += OnPositionChanged;
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

    private void OnPositionChanged(object? sender, EventArgs e)
    {
        _size = _tracker.Position.Size;
        _ = ShowOnDialAsync(_tracker.IsLoaded ? AsFraction(_size) : 0);
    }

    private static double AsFraction(double size) => (size + 100d) / 200d;

    public void Dispose() => _tracker.Updated -= OnPositionChanged;
}
