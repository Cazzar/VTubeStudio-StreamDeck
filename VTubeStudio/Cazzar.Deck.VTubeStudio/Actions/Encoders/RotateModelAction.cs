using Cazzar.Deck.Abstractions.Actions;
using Cazzar.Deck.Abstractions.Surfaces;
using Cazzar.Deck.Abstractions;
using System.Text.Json.Serialization;
using VTubeStudio.Api.Events;
using VTubeStudio.Api.Requests;
using VTubeStudio.Api.Responses;
using VTubeStudio.Api;

namespace Cazzar.Deck.VTubeStudio.Actions.Encoders;

[DeckAction(Name = "Rotate Model", Tooltip = "VTubeStudio [Rotate Model]",
    Icon = "vts_logo_transparent", PropertyView = "RotateModel")]
public sealed class RotateModelAction : EncoderAction<RotateModelAction.Options>, IDisposable
{
    private double _rotation;

    public RotateModelAction(DeckActionContext context, IVTubeStudio vts, IEncoderSurface encoder)
        : base(context, vts, encoder)
    {
        vts.ModelMoved += OnModelMoved;
        vts.CurrentModel += OnCurrentModel;
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

    private void OnModelMoved(object? sender, VtsEventArgs<ModelMovedEvent> e) => Track(e.Response.Position.Rotation);

    private void OnCurrentModel(object? sender, VtsEventArgs<CurrentModelResponse> e) => Track(e.Response.Position.Rotation);

    private void Track(double rotation)
    {
        _rotation = rotation;
        _ = ShowRotationAsync();
    }

    private ValueTask ShowRotationAsync() => ShowOnDialAsync(
        $"{_rotation:N2}\u00b0",
        Math.Abs(_rotation) % 360d / 360d);

    public void Dispose()
    {
        Vts.ModelMoved -= OnModelMoved;
        Vts.CurrentModel -= OnCurrentModel;
    }
}
