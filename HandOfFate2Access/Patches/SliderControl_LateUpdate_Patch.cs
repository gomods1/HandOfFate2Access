namespace HandOfFate2Access.Patches {
	/// <summary>
	/// A settings slider moves one increment on the navigate event, then keeps moving
	/// every frame the arrow is still down (SliderControl.LateUpdate scrolls it while
	/// the direction, repeat ignored, is non-zero). A key tap lasts several frames, so
	/// one press of Left jumped many steps and the value was spoken several times. The
	/// per-frame scroll is skipped: a press moves one step, and a held key repeats at
	/// the game's own navigate repeat rate.
	/// </summary>
	internal static class SliderControl_LateUpdate_Patch {
		private static bool Prefix() {
			return false;
		}
	}
}
