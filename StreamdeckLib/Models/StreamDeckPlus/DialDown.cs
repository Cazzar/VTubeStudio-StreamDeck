namespace StreamDeckLib.Models.StreamDeckPlus;

public record DialDown : ContextMessage {
	public override string Event { get; init; } = "dialDown";
	public string Action { get; set; }
	public string Device { get; set; }
	public DialPressPayload Payload { get; set; }
}