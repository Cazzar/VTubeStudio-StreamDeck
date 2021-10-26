namespace StreamDeckLib.Models
{
    public record PropertyInspectorDidDisappear(string Action, string Context, string Device) : EventMessage;
}