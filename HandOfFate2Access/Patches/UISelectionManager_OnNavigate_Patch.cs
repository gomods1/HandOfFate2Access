namespace HandOfFate2Access.Patches {
	/// <summary>
	/// Every directional UI input reaches the game's selection through
	/// UISelectionManager.OnNavigate (subscribed to UIInputManager.OnNavigate). On the map
	/// the arrows are handled by <see cref="Maps.MapNavigation"/> instead (a direction from
	/// the player's card), and the game's screen-position navigation does not run.
	/// </summary>
	internal static class UISelectionManager_OnNavigate_Patch {
		private static bool Prefix(InputButton a_button) {
			if (!Maps.MapInput.OnMap) return true;
			return !Maps.MapNavigation.Navigate(a_button);
		}
	}
}
