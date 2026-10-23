namespace HandOfFate2Access.Gambit {
	/// <summary>
	/// The chance cards' sounds as registered clips: each card's instrument (by the slot it
	/// held when the cards were shown: harp, flute, handpan, music box from the left) and
	/// each outcome's stinger, both from the plugin's sounds folder. A gambit of more than
	/// four cards reuses the instruments an octave up (and two for the ninth), so every
	/// card still sounds different.
	/// </summary>
	public static class GambitSounds {
		/// <summary>The instruments' files in the sounds folder, without extension, in slot order.</summary>
		public static readonly string[] CardFiles = { "card_harp", "card_flute", "card_handpan", "card_music_box" };

		public static string CardKey(int slot) => CardFiles[Normalize(slot) % CardFiles.Length];

		/// <summary>The playback pitch of a slot's instrument: 1 for the first four.</summary>
		public static float CardPitch(int slot) {
			int octave = Normalize(slot) / CardFiles.Length;
			return octave == 0 ? 1f : (octave == 1 ? 2f : 4f);
		}

		/// <summary>The stinger's file (and clip key) for an outcome.</summary>
		public static string OutcomeKey(ChanceOutcome outcome) {
			switch (outcome) {
				case ChanceOutcome.HugeSuccess: return "outcome_huge_success";
				case ChanceOutcome.Success: return "outcome_success";
				case ChanceOutcome.Failure: return "outcome_failure";
				default: return "outcome_huge_failure";
			}
		}

		private static int Normalize(int slot) => slot < 0 ? 0 : slot;
	}
}
