using Cazzar.Deck.Abstractions.Protocol;
using Cazzar.Deck.Abstractions.Surfaces;
using Microsoft.Extensions.Logging;

namespace Cazzar.Deck.Abstractions.Actions;

public sealed record DeckActionContext(
    IWidgetSurface Widget,
    ISettingsStore Settings,
    IPropertyViewChannel PropertyView,
    IDeckHostInfo Host,
    ILoggerFactory LoggerFactory);
