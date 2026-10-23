using HandOfFate2Access.Localization;

namespace HandOfFate2Access.Screens {
	/// <summary>
	/// Names for the input glyphs the game draws as sprites with no text (mouse buttons
	/// on its key-hint icons). Keyboard keys are drawn as a key cap with a text label and
	/// need no mapping. Null for a sprite that is not a known input glyph.
	/// </summary>
	internal static class InputGlyphs {
		public static string Name(string spriteName) {
			switch (spriteName) {
				case "mouse_left": return Strings.MouseLeft;
				case "mouse_right": return Strings.MouseRight;
				case "mouse_middle": return Strings.MouseMiddle;
				default: return null;
			}
		}
	}
}
