namespace HandOfFate2Access.Patches {
	/// <summary>
	/// Confirming a challenge on the challenge map opens the challenge the map has
	/// "targeted", which the game sets when a challenge icon is hovered by a controller
	/// or the mouse. On keyboard, after switching sections, focus already rests on an
	/// icon whose challenge was never targeted, so confirm does nothing until focus
	/// moves away and back. Confirm targets the focused icon's challenge first.
	/// </summary>
	internal static class UIChallengeNav_OnTabSelected_Patch {
		private static void Prefix(UINavTab a_tab) {
			var tab = a_tab as UIChallengeNavTab;
			ChallengeMap map = ChallengeMap.Instance;
			if (tab != null && map != null && tab.ChallengeCardPrefab != null && !tab.Locked)
				map.Target = tab.ChallengeCardPrefab;
		}
	}
}
