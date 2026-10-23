using HandOfFate2Access.Localization;

namespace HandOfFate2Access.Screens {
	/// <summary>The screens whose entry is announced by name.</summary>
	public enum ScreenId {
		None,
		MainMenu,
		Map,
		ChallengeMap,
		DeckBuilder,
		Inventory,
		Paused,
		Results,
		Customisation,
	}

	/// <summary>
	/// Maps the game's menu stacks (the name of the menu on top of the primary stack,
	/// and whether the pause menu is up on the secondary one) to the screen the player is
	/// on, and names it. Screens not listed (encounters, minigames, combat) announce
	/// themselves through their own narration.
	/// </summary>
	public static class ScreenCatalog {
		public static ScreenId Identify(string primaryTop, string secondaryTop) {
			if (!string.IsNullOrEmpty(secondaryTop) && secondaryTop.IndexOf("Pause", System.StringComparison.Ordinal) >= 0)
				return ScreenId.Paused;
			switch (primaryTop) {
				case "StartScreen": return ScreenId.MainMenu;
				case "Map": return ScreenId.Map;
				case "Inventory": return ScreenId.Inventory;
				case "ChallengeMap": return ScreenId.ChallengeMap;
				case "Deckbuilder_SelectMode":
				case "Deckbuilder_Build":
				case "Deckbuilder_Build_Inspect": return ScreenId.DeckBuilder;
				case "CustomisationMenu": return ScreenId.Customisation;
				case "Post":
				case "ScoreDisplay": return ScreenId.Results;
				default: return ScreenId.None;
			}
		}

		public static string NameOf(ScreenId id) {
			switch (id) {
				case ScreenId.MainMenu: return Strings.ScreenMainMenu;
				case ScreenId.Map: return Strings.ScreenMap;
				case ScreenId.ChallengeMap: return Strings.ScreenChallengeMap;
				case ScreenId.DeckBuilder: return Strings.ScreenDeckBuilder;
				case ScreenId.Inventory: return Strings.ScreenInventory;
				case ScreenId.Paused: return Strings.ScreenPaused;
				case ScreenId.Results: return Strings.ScreenResults;
				case ScreenId.Customisation: return Strings.ScreenCustomisation;
				default: return null;
			}
		}
	}
}
