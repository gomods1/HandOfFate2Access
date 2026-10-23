namespace HandOfFate2Access.Localization {
	/// <summary>
	/// The game prints keyboard key caps in English ("L Shift", "Space", "Down") in every
	/// language. A key cap's whole text is swapped for the mod's name for that key in the
	/// player's language; anything else (letters, F-keys, text that is not a key cap)
	/// passes through unchanged.
	/// </summary>
	public static class KeyNames {
		public static string Localize(string label) {
			if (string.IsNullOrEmpty(label)) return label;
			StringTable t = Strings.Active;
			switch (label.Trim()) {
				case "L Shift": case "Left Shift": return t.KeyLeftShift;
				case "R Shift": case "Right Shift": return t.KeyRightShift;
				case "L Ctrl": case "Left Control": return t.KeyLeftCtrl;
				case "R Ctrl": case "Right Control": return t.KeyRightCtrl;
				case "L Alt": case "Left Alt": return t.KeyLeftAlt;
				case "R Alt": case "Right Alt": case "Alt Graphic": return t.KeyRightAlt;
				case "Space": return t.KeySpace;
				case "Return": case "Enter": return t.KeyEnter;
				case "Tab": return t.KeyTab;
				case "Escape": case "Esc": return t.KeyEscape;
				case "Backspace": return t.KeyBackspace;
				case "Up": case "Up Arrow": return string.Format(t.KeyArrowFormat, t.DirUp);
				case "Down": case "Down Arrow": return string.Format(t.KeyArrowFormat, t.DirDown);
				case "Left": case "Left Arrow": return string.Format(t.KeyArrowFormat, t.DirLeft);
				case "Right": case "Right Arrow": return string.Format(t.KeyArrowFormat, t.DirRight);
				default: return label;
			}
		}
	}
}
