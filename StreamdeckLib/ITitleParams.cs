using StreamDeckLib.Models;

namespace StreamDeckLib
{
    public interface ITitleParams
    {
        void GotTitleParams(TitleParametersDidChange titleParameters);
    }
}