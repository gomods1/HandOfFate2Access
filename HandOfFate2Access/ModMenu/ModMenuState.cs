namespace HandOfFate2Access.ModMenu {
	/// <summary>
	/// Whether the accessibility mod menu is open (the game's UI input is skipped and the
	/// arrows belong to the menu while it is), and the hand-off from the menu options
	/// injected into the game's menus, which request it opened when chosen.
	/// </summary>
	internal static class ModMenuState {
		public static bool Open;
		private static bool _openRequested;

		public static void RequestOpen() => _openRequested = true;

		public static bool ConsumeOpen() {
			bool v = _openRequested;
			_openRequested = false;
			return v;
		}
	}
}
