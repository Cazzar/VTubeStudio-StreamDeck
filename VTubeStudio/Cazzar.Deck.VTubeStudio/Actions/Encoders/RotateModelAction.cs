using Cazzar.Deck.Abstractions.Actions;
using Cazzar.Deck.Abstractions.Surfaces;
using Cazzar.Deck.Abstractions;
using Cazzar.Deck.VTubeStudio.Actions.Movement;
using System.Text.Json.Serialization;
using VTubeStudio.Api.Requests;
using VTubeStudio.Api;

namespace Cazzar.Deck.VTubeStudio.Actions.Encoders;

[DeckAction(Name = "Rotate Model", Tooltip = "VTubeStudio [Rotate Model]",
    Icon = "vts_logo_transparent", PropertyView = "RotateModel")]
public sealed class RotateModelAction : EncoderAction<RotateModelAction.Options>, IDisposable
{
    private readonly ModelPositionTracker _tracker;

    private double _rotation;

    public RotateModelAction(
        DeckActionContext context, IVTubeStudio vts, IEncoderSurface encoder, ModelPositionTracker tracker)
        : base(context, vts, encoder)
    {
        _tracker = tracker;
        tracker.Updated += OnPositionChanged;
    }

    public sealed class Options
    {
        [JsonPropertyName("stepSize")] public int StepSize { get; set; } = 2;
        [JsonPropertyName("defaultRotation")] public double DefaultRotation { get; set; }
    }

    protected override void Pressed() =>
        Vts.Send(new MoveModelRequest { Rotation = Settings.DefaultRotation, TimeInSeconds = 0.05d });

    public override void DialRotate(int ticks, bool pressed)
    {
        _rotation = (_rotation + ticks * (Settings.StepSize / 10d)) % 360d;

        Vts.Send(new MoveModelRequest { Rotation = _rotation, TimeInSeconds = 0.05d });
        _ = ShowRotationAsync();
    }

    [PropertyViewCommand("use-current")]
    public void UseCurrent(IPayload body)
    {
        Settings.DefaultRotation = _rotation;
        _ = SaveSettingsAsync();
        _ = UpdateClientAsync();
    }

    private void OnPositionChanged(object? sender, EventArgs e)
    {
        _rotation = _tracker.Position.Rotation;
        _ = ShowRotationAsync();
    }

    private ValueTask ShowRotationAsync() => ShowOnDialAsync(
        $"{_rotation:N2}\u00b0",
        Math.Abs(_rotation) % 360d / 360d);

    public void Dispose() => _tracker.Updated -= OnPositionChanged;
}
