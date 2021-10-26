namespace StreamDeckLib.Models
{
    public record StatePayload(uint Payload);

    record TitlePayload(string? Title, int Target, int State);
}