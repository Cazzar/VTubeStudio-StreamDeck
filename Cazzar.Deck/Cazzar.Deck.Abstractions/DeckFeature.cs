namespace Cazzar.Deck.Abstractions;

[Flags]
public enum DeckFeature
{
    None = 0,
    Encoder = 1,
    Touchscreen = 2,
    MultiState = 4,
    GlobalSettings = 8,
    DeviceEvents = 16,
    ApplicationEvents = 32,
    OpenUrl = 64,
    Alerts = 128,
    HostLog = 256,

    All = Encoder | Touchscreen | MultiState | GlobalSettings |
          DeviceEvents | ApplicationEvents | OpenUrl | Alerts | HostLog,
}
