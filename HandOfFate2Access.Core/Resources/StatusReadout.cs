using HandOfFate2Access.Localization;
using HandOfFate2Access.UI;

namespace HandOfFate2Access.Resources {
	/// <summary>
	/// The on-demand resource readout (the status key): health first, then food, gold
	/// and fame, then poison only when poisoned, since its absence is the normal case.
	/// </summary>
	public static class StatusReadout {
		public static string Compose(int health, int maxHealth, int food, int gold, int fame, int poison) {
			return new Message().AddRange(Lines(health, maxHealth, food, gold, fame, poison)).Resolve();
		}

		/// <summary>The same readout one resource per line: the status review buffer.</summary>
		public static System.Collections.Generic.List<string> Lines(int health, int maxHealth, int food, int gold, int fame, int poison) {
			var lines = new System.Collections.Generic.List<string> {
				string.Format(Strings.StatHealthFormat, health, maxHealth),
				string.Format(Strings.StatFoodFormat, food),
				string.Format(Strings.StatGoldFormat, gold),
				string.Format(Strings.StatFameFormat, fame),
			};
			if (poison > 0)
				lines.Add(string.Format(Strings.StatPoisonFormat, poison));
			return lines;
		}
	}
}
