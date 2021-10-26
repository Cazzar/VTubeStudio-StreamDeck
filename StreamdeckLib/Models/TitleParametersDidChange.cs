namespace StreamDeckLib.Models
{
    public record TitleParametersDidChange(string Action = default, string Context = default, string Device = default, AppearanceChangePayload Payload = default) : EventMessage;
}