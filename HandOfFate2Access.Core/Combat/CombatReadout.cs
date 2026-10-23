using HandOfFate2Access.Localization;

namespace HandOfFate2Access.Combat {
	/// <summary>
	/// Spoken combat bookkeeping, the information a sighted player reads off the fight
	/// HUD: how many enemies are left, and the health thresholds crossed downward.
	/// </summary>
	public static class CombatReadout {
		/// <summary>The fraction thresholds (of max health) that warn when crossed downward.</summary>
		public static readonly float[] HealthThresholds = { 0.5f, 0.25f, 0.1f };

		public static string FightStart(int enemies) {
			// A fight with nothing to kill (a trap room) is just a fight; its objective
			// says what ends it.
			if (enemies == 0) return Strings.FightStartPlain;
			return string.Format(Strings.FightStartFormat, Enemies(enemies));
		}

		public static string EnemiesLeft(int enemies) {
			return enemies == 0 ? Strings.EnemiesNone : Enemies(enemies);
		}

		private static string Enemies(int count) {
			return count == 1 ? Strings.EnemyOne : string.Format(Strings.EnemiesFormat, count);
		}

		/// <summary>
		/// The lowest threshold crossed going from <paramref name="before"/> to
		/// <paramref name="after"/> (fractions of max health), or -1 when none was.
		/// </summary>
		public static int CrossedThreshold(float before, float after) {
			int crossed = -1;
			for (int i = 0; i < HealthThresholds.Length; i++)
				if (before > HealthThresholds[i] && after <= HealthThresholds[i]) crossed = i;
			return crossed;
		}

		public static string HealthWarning(int health, int maxHealth) {
			return string.Format(Strings.StatHealthFormat, health, maxHealth);
		}
	}
}
