namespace Cazzar.Deck.Abstractions.Surfaces;

public interface IWidgetSurface
{
    ValueTask SetTitleAsync(ActionRef @ref, string? title, uint? state = null);
    ValueTask SetImageAsync(ActionRef @ref, string image, uint state = 0);
    ValueTask SetStateAsync(ActionRef @ref, uint state);
    ValueTask ShowAlertAsync(ActionRef @ref);
    ValueTask ShowOkAsync(ActionRef @ref);
}
