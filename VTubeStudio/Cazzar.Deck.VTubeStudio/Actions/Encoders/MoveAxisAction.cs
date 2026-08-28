using Cazzar.Deck.Abstractions.Actions;
using Cazzar.Deck.Abstractions.Surfaces;
using Cazzar.Deck.Abstractions;
using System.Text.Json.Serialization;
using VTubeStudio.Api.Events;
using VTubeStudio.Api.Models;
using VTubeStudio.Api.Requests;
using VTubeStudio.Api.Responses;
using VTubeStudio.Api;

namespace Cazzar.Deck.VTubeStudio.Actions.Encoders;

public abstract class MoveAxisAction : EncoderAction<MoveAxisAction.Options>, IDisposable
{
    private double _position;

    protected MoveAxisAction(DeckActionContext context, IVTubeStudio vts, IEncoderSurface encoder)
        : base(context, vts, encoder)
    {
        vts.ModelMoved += OnModelMoved;
        vts.CurrentModel += OnCurrentModel;
    }

    public sealed class Options
    {
        [JsonPropertyName("stepSize")] public int StepSize { get; set; } = 2;
        [JsonPropertyName("defaultPosition")] public double DefaultPosition { get; set; }
    }

    protected abstract double Read(ModelPosition position);
    protected abstract MoveModelRequest Write(double value);

    protected override void Pressed() => Vts.Send(Write(Settings.DefaultPosition));

    public override void DialRotate(int ticks, bool pressed)
    {
        _position = Math.Clamp(_position + ticks * (Settings.StepSize / 200d), -2, 2);

        Vts.Send(Write(_position));
        _ = ShowOnDialAsync(AsFraction(_position));
    }

    [PropertyViewCommand("use-current")]
    public void UseCurrent(IPayload body)
    {
        Settings.DefaultPosition = _position;
        _ = SaveSettingsAsync();
        _ = UpdateClientAsync();
    }

    private void OnModelMoved(object? sender, VtsEventArgs<ModelMovedEvent> e)
    {
        _position = Read(e.Response.Position);
        _ = ShowOnDialAsync(AsFraction(_position));
    }

    private void OnCurrentModel(object? sender, VtsEventArgs<CurrentModelResponse> e)
    {
        _position = Read(e.Response.Position);
        _ = ShowOnDialAsync(e.Response.IsLoaded ? AsFraction(_position) : 0);
    }

    private static double AsFraction(double position) => Math.Clamp((position + 1d) / 2d, 0, 1);

    public void Dispose()
    {
        Vts.ModelMoved -= OnModelMoved;
        Vts.CurrentModel -= OnCurrentModel;
    }
}
