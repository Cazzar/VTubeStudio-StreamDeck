namespace Cazzar.Deck.Abstractions.Actions.Handlers;

public interface IGlobalSettingsHandler
{
    void GotGlobalSettings(IPayload settings);
}
