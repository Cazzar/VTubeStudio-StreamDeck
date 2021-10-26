namespace StreamDeckLib.Models
{
    public record KeyDown(string Action, string Context, string Device, KeyActionPayload Payload) : EventMessage;
}