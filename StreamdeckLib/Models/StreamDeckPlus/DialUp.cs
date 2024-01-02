namespace StreamDeckLib.Models.StreamDeckPlus;

public record DialUp : ContextMessage {
	public override string Event { get; init; } = "dialUp";
	public string Action { get; set; }
	public string Device { get; set; }
	public DialPressPayload Payload { get; set; }
}
