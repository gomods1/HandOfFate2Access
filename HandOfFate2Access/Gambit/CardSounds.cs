using System.Collections.Generic;
using System.IO;
using HandOfFate2Access.Audio;
using HandOfFate2Access.Util;

namespace HandOfFate2Access.Gambit {
	/// <summary>
	/// Registers the chance cards' sounds as clips under <see cref="GambitSounds"/>' keys:
	/// each card's instrument and each outcome's stinger, read from the plugin's sounds
	/// folder (card_*.wav and outcome_*.wav, made with the genny CLI from
	/// sounds/cards_batch.json and sounds/outcomes_batch.json). An instrument that fails to
	/// load falls back to the slot's synthesized tone (logged), so the cards stay
	/// distinguishable; a stinger that fails to load is left out (logged), and that outcome
	/// is then heard by its instrument alone. Lengths are kept so the gambit can time its
	/// walk.
	/// </summary>
	internal static class CardSounds {
		private static readonly Dictionary<string, float> Seconds = new Dictionary<string, float>();

		public static void Load(string pluginDir) {
			if (!AudioEngine.IsAvailable) return;
			string soundsDir = Path.Combine(pluginDir, "sounds");
			for (int slot = 0; slot < GambitSounds.CardFiles.Length; slot++) {
				string key = GambitSounds.CardFiles[slot];
				if (TryLoad(soundsDir, key)) continue;
				int rate = AudioEngine.OutputSampleRate;
				float[] tone = GambitTones.Generate(slot, rate);
				AudioEngine.Register(key, tone, 1, rate);
				Seconds[key] = tone.Length / (float)rate;
				Log.Warn("card sound '" + key + "' replaced by its synthesized tone");
			}
			foreach (ChanceOutcome outcome in new[] { ChanceOutcome.HugeSuccess, ChanceOutcome.Success, ChanceOutcome.Failure, ChanceOutcome.HugeFailure })
				TryLoad(soundsDir, GambitSounds.OutcomeKey(outcome));
		}

		public static bool IsLoaded(string key) => Seconds.ContainsKey(key);

		/// <summary>A loaded clip's length in seconds; 0 when it did not load.</summary>
		public static float Length(string key) {
			float seconds;
			return Seconds.TryGetValue(key, out seconds) ? seconds : 0f;
		}

		private static bool TryLoad(string soundsDir, string key) {
			string path = Path.Combine(soundsDir, key + ".wav");
			try {
				WavAudio.Decode(File.ReadAllBytes(path), out float[] pcm, out int channels, out int sampleRate);
				AudioEngine.Register(key, pcm, channels, sampleRate);
				Seconds[key] = pcm.Length / (float)channels / sampleRate;
				return true;
			} catch (System.Exception ex) {
				Log.Error("chance card sound '" + key + "' failed to load from " + path + ": " + ex.Message);
				return false;
			}
		}
	}
}
