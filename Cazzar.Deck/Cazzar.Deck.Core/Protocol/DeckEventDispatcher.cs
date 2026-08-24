using Cazzar.Deck.Abstractions.Actions.Handlers;
using Cazzar.Deck.Core.Actions;
using Cazzar.Deck.Abstractions.Protocol.Events;
using Microsoft.Extensions.Logging;

namespace Cazzar.Deck.Core.Protocol;

public sealed class DeckEventDispatcher(
    ActionInstances instances,
    IEnumerable<IGlobalSettingsHandler> globalSettingsHandlers,
    ILogger<DeckEventDispatcher> logger)
{
    public void Dispatch(IDeckEvent @event)
    {
        switch (@event)
        {
            case ActionAppeared e: instances.Create(e.Ref, e.Settings); break;
            case ActionDisappeared e: instances.Destroy(e.Ref); break;

            case SettingsReceived e:
                instances.Invoke<ISettingsHandler>(e.Ref, h => h.GotSettings(e.Settings));
                break;

            case GlobalSettingsReceived e:
                foreach (var handler in globalSettingsHandlers)
                    Guard(() => handler.GotGlobalSettings(e.Settings), handler.GetType().Name);
                break;

            case KeyPressed e: instances.Invoke<IKeyHandler>(e.Ref, h => h.KeyDown(e.State)); break;
            case KeyReleased e: instances.Invoke<IKeyHandler>(e.Ref, h => h.KeyUp(e.State)); break;

            case PropertyViewOpened e: instances.Invoke<IPropertyViewHandler>(e.Ref, h => h.PropertyViewOpened()); break;
            case PropertyViewClosed e: instances.Invoke<IPropertyViewHandler>(e.Ref, h => h.PropertyViewClosed()); break;
            case PropertyViewMessage e: instances.Invoke<IPropertyViewHandler>(e.Ref, h => h.PropertyViewMessage(e.Body)); break;

            case DialRotated e: instances.Invoke<IEncoderHandler>(e.Ref, h => h.DialRotate(e.Ticks, e.Pressed)); break;
            case DialPressed e: instances.Invoke<IEncoderHandler>(e.Ref, h => h.DialPress()); break;
            case DialReleased e: instances.Invoke<IEncoderHandler>(e.Ref, h => h.DialRelease()); break;
            case TouchTapped e: instances.Invoke<IEncoderHandler>(e.Ref, h => h.Touch(e.X, e.Y, e.Hold)); break;

            default:
                logger.LogDebug("No route for {Event}", @event.GetType().Name);
                break;
        }
    }

    private void Guard(Action body, string who)
    {
        try
        {
            body();
        }
        catch (Exception e)
        {
            logger.LogCritical(e, "{Handler} threw handling a global settings update", who);
        }
    }
}
