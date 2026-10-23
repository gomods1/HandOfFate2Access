using System.Collections.Generic;
using HandOfFate2Access.Localization;

namespace HandOfFate2Access.UI {
	/// <summary>
	/// Where a map card lies from the player's card, for the survey cursor: "2 left, 1 up
	/// from you". Board rows grow downward (a step up is y - 1). The player's own card
	/// has no offset (null): its readout already says "here".
	/// </summary>
	public static class MapOffset {
		public static string Describe(int dx, int dy) {
			var parts = new List<string>();
			if (dx != 0) parts.Add(Strings.MapSteps(dx < 0 ? -dx : dx, dx < 0 ? Strings.DirLeft : Strings.DirRight));
			if (dy != 0) parts.Add(Strings.MapSteps(dy < 0 ? -dy : dy, dy < 0 ? Strings.DirUp : Strings.DirDown));
			if (parts.Count == 0) return null;
			return Strings.MapFromYou(string.Join(", ", parts.ToArray()));
		}
	}
}
