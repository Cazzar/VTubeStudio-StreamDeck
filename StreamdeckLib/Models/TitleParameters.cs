namespace StreamDeckLib.Models
{
    public record TitleParameters(
        string FontFamily = default,
        int FontSize = default,
        string FontStyle = default,
        bool FontUnderline = default,
        bool ShowTitle = default,
        Alignment TitleAlignment = default,
        string TitleColor = default
    );
}