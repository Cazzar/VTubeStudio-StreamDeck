using StreamDeckLib.Models.StreamDeckPlus;

namespace StreamDeckLib.Models;

public interface IStreamDeckPlus {

	void Touch(TouchTapPayload touchTap);
	void DialDown(DialPressPayload dialDown);
	void DialUp(DialPressPayload dialDown);
	void DialRotate(DialRotatePayload dialRotatePayload);
}
