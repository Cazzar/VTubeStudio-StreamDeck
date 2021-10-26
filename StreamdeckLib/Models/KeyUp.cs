namespace StreamDeckLib.Models
{
    public record KeyUp(string Action, string Context, string Device, KeyActionPayload Payload) : EventMessage;
}