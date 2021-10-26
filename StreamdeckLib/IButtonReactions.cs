using StreamDeckLib.Models;

namespace StreamDeckLib
{
    public interface IButtonReactions
    {
        public void KeyUp(KeyActionPayload keyActionPayload);

        public void KeyDown(KeyActionPayload keyActionPayload);
    }
}