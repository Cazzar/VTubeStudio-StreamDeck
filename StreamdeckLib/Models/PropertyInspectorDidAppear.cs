namespace StreamDeckLib.Models
{
    public record PropertyInspectorDidAppear(string Action, string Context, string Device) : EventMessage;
}