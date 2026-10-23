using HandOfFate2Access.Audio;
using HandOfFate2Access.Focus;

namespace HandOfFate2Access.Minigames {
	/// <summary>Maps the game's chance outcomes onto the minigame sound vocabulary and words.</summary>
	internal static class Outcomes {
		public static OutcomeKind Kind(ChanceType type) {
			switch (type) {
				case ChanceType.HugeFailure: return OutcomeKind.HugeFailure;
				case ChanceType.Failure: return OutcomeKind.Failure;
				case ChanceType.Success: return OutcomeKind.Success;
				default: return OutcomeKind.HugeSuccess;
			}
		}

		/// <summary>The outcome a card stands for: its chance type, or neutral for any other card.</summary>
		public static OutcomeKind Kind(Card card) {
			var chance = card as ChanceCard;
			return chance != null ? Kind(chance.ChanceType) : OutcomeKind.Neutral;
		}

		/// <summary>The game's own localized name for an outcome: the title of its chance card.</summary>
		public static string Name(ChanceType type) {
			CardManager manager = CardManager.Instance;
			ChanceCard prefab = manager != null ? manager.GetChanceCardPrefab(type) : null;
			return prefab != null ? CardInfoReader.Title(prefab) : type.ToString();
		}
	}
}
