using Newtonsoft.Json.Linq;

namespace StreamDeckLib.Models.StreamDeckPlus;

public record DialRotatePayload {
	public string Controller { get; set; }
	public JObject Settings { get; set; }
	public Coordinates Coordinates { get; set; }
	public int Ticks { get; set; }
	public bool Pressed { get; set; }
}