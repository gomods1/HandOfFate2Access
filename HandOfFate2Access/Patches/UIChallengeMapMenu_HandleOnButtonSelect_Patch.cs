namespace HandOfFate2Access.Patches {
	/// <summary>
	/// On the challenge map, Up from the challenge row enters the list of sections
	/// (Campaign, Adventure, Endless), and the game only leaves that list when a
	/// different section is picked; confirming the section already showing does nothing
	/// and the list wraps, so a keyboard player is trapped in it. Confirming the current
	/// section returns to its challenges.
	/// </summary>
	internal static class UIChallengeMapMenu_HandleOnButtonSelect_Patch {
		private static bool Prefix(UIChallengeMapMenu __instance, UIChallengeMapMenuButton a_button) {
			if (a_button == null || !a_button.IsSelected) return true;
			__instance.ResumeSelection();
			return false;
		}
	}
}
