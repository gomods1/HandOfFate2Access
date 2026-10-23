namespace HandOfFate2Access.UI {
	/// <summary>
	/// The game's stand-ins for text it could not find: "****" for a missing localization,
	/// and a missing key printed between double asterisks ("**7b79f6b0-...**", a card whose
	/// title string is absent). Neither is ever spoken; a reader falls back to the next
	/// readable line.
	/// </summary>
	public static class Placeholder {
		public static bool IsMissing(string text) {
			if (string.IsNullOrEmpty(text)) return true;
			string t = text.Trim();
			if (t.Trim('*', ' ').Length == 0) return true;
			return t.Length > 4 && t.StartsWith("**") && t.EndsWith("**");
		}
	}
}
