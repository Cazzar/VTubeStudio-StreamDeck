using System.Numerics;

using Newtonsoft.Json.Linq;

namespace StreamDeckLib.Models.StreamDeckPlus {
	public record TouchTapPayload {
		public JObject Settings { get; set; }
		public Coordinates Coordinates { get; set; }
		public int[] TapPos { get; set; }
		public bool Hold { get; set; }
	}
}
