namespace Cazzar.Deck.Core.Actions;

public sealed class ActionTimerOptions
{
    public TimeSpan Interval { get; set; } = TimeSpan.FromSeconds(1);
}
