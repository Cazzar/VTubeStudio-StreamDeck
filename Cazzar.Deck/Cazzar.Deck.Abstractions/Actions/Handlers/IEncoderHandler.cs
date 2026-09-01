namespace Cazzar.Deck.Abstractions.Actions.Handlers;

[RequiresFeature(DeckFeature.Encoder)]
public interface IEncoderHandler
{
    void DialRotate(int ticks, bool pressed);
    void DialPress();
    void DialRelease();
    void Touch(int x, int y, bool hold);
}
