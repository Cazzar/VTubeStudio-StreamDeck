using System.Text.Json.Nodes;
using Cazzar.Deck.Abstractions.Actions.Handlers;
using Cazzar.Deck.Abstractions.Actions;
using Cazzar.Deck.Abstractions.Surfaces;
using VTubeStudio.Api;

namespace Cazzar.Deck.VTubeStudio.Actions.Encoders;

// Implementing IEncoderHandler is what restricts these to hosts with dials — no Hosts declaration needed.
public abstract class EncoderAction<TSettings>(
    DeckActionContext context,
    IVTubeStudio vts,
    IEncoderSurface encoder) : VTubeStudioAction<TSettings>(context, vts), IEncoderHandler
    where TSettings : new()
{
    protected IEncoderSurface Encoder { get; } = encoder;

    protected ValueTask ShowOnDialAsync(double fraction) => ShowOnDialAsync($"{fraction:P1}", fraction);

    protected ValueTask ShowOnDialAsync(string value, double fraction) =>
        Encoder.SetFeedbackAsync(Ref, new()
        {
            ["value"] = value,
            ["indicator"] = (int)(Math.Clamp(fraction, 0, 1) * 100),
        });

    public void DialPress()
    {
    }

    public void DialRelease() => Pressed();

    public void Touch(int x, int y, bool hold) => Pressed();

    public abstract void DialRotate(int ticks, bool pressed);
}
