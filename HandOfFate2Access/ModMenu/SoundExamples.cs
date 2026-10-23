using HandOfFate2Access.Audio;
using HandOfFate2Access.Glossary;
using HandOfFate2Access.Util;
using UnityEngine;

namespace HandOfFate2Access.ModMenu {
	/// <summary>
	/// Plays the sound examples (<see cref="GlossaryCatalog"/>) for the mod menu and the
	/// help list: one entry at a time, its steps in sequence, each at the player's volume
	/// for its sound family. Timing uses unscaled time, since the pause menu freezes game
	/// time. Pumped once per frame; a new example or <see cref="Stop"/> cuts the last one.
	/// </summary>
	internal sealed class SoundExamples {
		private const int RenderSampleRate = 44100;

		private bool _clipsReady;
		private GlossaryStep[] _steps;
		private int _stepIndex;
		private float _stepEndsAt;
		private HandOfFate2Access.Audio.Voice _voice = HandOfFate2Access.Audio.Voice.None;

		/// <summary>The example whose spoken line is <paramref name="label"/>, or null.</summary>
		public static GlossaryEntry Find(string label) {
			if (string.IsNullOrEmpty(label)) return null;
			foreach (GlossaryEntry entry in GlossaryCatalog.CombatEntries)
				if (entry.Label == label) return entry;
			foreach (GlossaryEntry entry in GlossaryCatalog.MinigameEntries)
				if (entry.Label == label) return entry;
			return null;
		}

		public void Play(GlossaryEntry entry) {
			Stop();
			if (entry == null) return;
			if (!AudioEngine.IsAvailable) {
				Log.Warn("audio unavailable; sound example '" + entry.Label + "' not played");
				return;
			}
			EnsureClips();
			_steps = entry.Steps;
			_stepIndex = -1;
			AdvanceStep();
		}

		/// <summary>A single step played now, cutting any example (a volume setting's sample).</summary>
		public void PlayStep(GlossaryStep step) {
			Play(new GlossaryEntry(() => step.ClipKey, new[] { step }));
		}

		public void Pump() {
			if (_steps == null || Time.unscaledTime < _stepEndsAt) return;
			StopVoice();
			AdvanceStep();
		}

		public void Stop() {
			StopVoice();
			_steps = null;
		}

		private void AdvanceStep() {
			_stepIndex++;
			if (_steps == null || _stepIndex >= _steps.Length) {
				_steps = null;
				return;
			}
			GlossaryStep step = _steps[_stepIndex];
			if (step.Loop) {
				_voice = AudioEngine.Play(step.ClipKey, step.Params, true, step.Category);
				_stepEndsAt = Time.unscaledTime + step.HoldSeconds;
			} else {
				AudioEngine.PlayOneShot(step.ClipKey, step.Params, step.Category);
				if (_stepIndex + 1 < _steps.Length)
					_stepEndsAt = Time.unscaledTime + step.HoldSeconds;
				else
					_steps = null;
			}
		}

		private void StopVoice() {
			if (_voice.IsValid) AudioEngine.Stop(_voice);
			_voice = HandOfFate2Access.Audio.Voice.None;
		}

		// The example-only renders: projectile flights (live synth in a fight) and the
		// pendulum and wheel outcome voices (generated per minigame). The card sounds are
		// registered by the gambit (CardSounds), the fight cues by their features.
		private void EnsureClips() {
			if (_clipsReady) return;
			AudioEngine.Register(GlossaryCatalog.ProjectileKey,
				ProjectileSynth.Render(RenderSampleRate, GlossaryCatalog.ProjectileDemoSeconds, false, 1u), 1, RenderSampleRate);
			AudioEngine.Register(GlossaryCatalog.ProjectileReflectedKey,
				ProjectileSynth.Render(RenderSampleRate, GlossaryCatalog.ProjectileDemoSeconds, true, 2u), 1, RenderSampleRate);
			for (int i = 0; i < 5; i++)
				AudioEngine.Register(GlossaryCatalog.OutcomeKey((OutcomeKind)i), OutcomeTones.Loop((OutcomeKind)i, RenderSampleRate), 1, RenderSampleRate);
			_clipsReady = true;
		}
	}
}
