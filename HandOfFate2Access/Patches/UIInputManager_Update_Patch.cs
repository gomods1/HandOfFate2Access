namespace HandOfFate2Access.Patches {
	/// <summary>
	/// UIInputManager.Update dispatches every UI key the game acts on: navigation,
	/// confirm, cancel, the F1-F3 bar buttons, pause. While the help list or the mod menu
	/// is open it is skipped, so the keys that read and close them never also act on the
	/// screen underneath.
	/// </summary>
	internal static class UIInputManager_Update_Patch {
		private static bool Prefix() {
			return !Help.HelpState.Open && !ModMenu.ModMenuState.Open;
		}
	}
}
