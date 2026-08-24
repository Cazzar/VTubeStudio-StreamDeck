using Cazzar.Deck.Abstractions.Actions.Handlers;
using Cazzar.Deck.Abstractions.Actions;
using Cazzar.Deck.Abstractions.Surfaces;
using Cazzar.Deck.Abstractions;
using Cazzar.Deck.Core.Actions;

namespace Cazzar.Deck.Tests;

[DeckAction("test.dial")]
public class DialAction(DeckActionContext context, IEncoderSurface encoder)
    : DeckAction<PortableAction.Options>(context), IEncoderHandler
{
    public IEncoderSurface Encoder { get; } = encoder;

    public void DialRotate(int ticks, bool pressed) { }
    public void DialPress() { }
    public void DialRelease() { }
    public void Touch(int x, int y, bool hold) { }
}
