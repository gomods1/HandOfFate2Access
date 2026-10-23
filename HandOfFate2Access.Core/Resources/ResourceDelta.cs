using HandOfFate2Access.Localization;

namespace HandOfFate2Access.Resources {
	public enum ResourceKind { Health, MaxHealth, Food, Gold, Fame }

	/// <summary>
	/// A live resource change ("+7 food", "-4 health"), spoken when a stat moves on its
	/// own (an encounter reward, a cost, starvation). The sign is always written so a gain
	/// and a loss of the same amount never sound alike.
	/// </summary>
	public static class ResourceDelta {
		public static string Compose(ResourceKind kind, int diff) {
			if (diff == 0) return "";
			string amount = (diff > 0 ? "+" : "") + diff;
			switch (kind) {
				case ResourceKind.Health: return string.Format(Strings.DeltaHealthFormat, amount);
				case ResourceKind.MaxHealth: return string.Format(Strings.DeltaMaxHealthFormat, amount);
				case ResourceKind.Food: return string.Format(Strings.StatFoodFormat, amount);
				case ResourceKind.Gold: return string.Format(Strings.StatGoldFormat, amount);
				case ResourceKind.Fame: return string.Format(Strings.StatFameFormat, amount);
				default: return "";
			}
		}
	}
}
