namespace StreamDeckLib.Models
{
    public record OnWillDisappear(string Action, string Context, string Device, AppearanceChangePayload Payload) : EventMessage;
}
