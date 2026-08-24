namespace Cazzar.Deck.Abstractions.Actions;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class PropertyViewCommandAttribute(string command) : Attribute
{
    public string Command { get; } = command;
}
