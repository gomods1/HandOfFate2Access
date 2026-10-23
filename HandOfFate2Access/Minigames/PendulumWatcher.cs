using System.Reflection;
using HandOfFate2Access.Audio;
using HandOfFate2Access.Speech;
using HandOfFate2Access.Util;
using HarmonyLib;
using UnityEngine;

namespace HandOfFate2Access.Minigames {
	/// <summary>
	/// Makes the pendulum minigame playable by ear. While the pendulum swings, one
	/// sustained voice plays the outcome currently under the beam (the game's own
	/// <c>Pendulum.Evaluate</c>: the zone that would be chosen if the player stopped now),
	/// panned with the arm from its left edge to its right. Better zones sound higher and
	/// brighter, so the player presses confirm on the tone they want. When the swing stops,
	/// the outcome it stopped on is spoken with the game's own word for it.
	/// </summary>
	internal sealed class PendulumWatcher {
		private static readonly FieldInfo PivotField = AccessTools.Field(typeof(Pendulum), "m_pivot");
		private const float ArcHalfDegrees = 34f;

		private bool _available;
		private int _rate;
		private float[][] _loops;
		private HandOfFate2Access.Audio.Voice _voice = HandOfFate2Access.Audio.Voice.None;
		private OutcomeKind _playing;
		private bool _wasSwinging;

		public void Initialize() {
			_available = AudioEngine.IsAvailable;
			if (!_available) return;
			_rate = AudioEngine.OutputSampleRate;
			_loops = new float[5][];
			for (int i = 0; i < 5; i++)
				_loops[i] = OutcomeTones.Loop((OutcomeKind)i, _rate);
		}

		public void Pump() {
			if (!_available) return;
			Pendulum pendulum = PendulumTask.RunningTask != null && CardTable.Instance != null ? CardTable.Instance.Pendulum : null;
			bool swinging = pendulum != null && pendulum.IsSwinging;
			if (!swinging) {
				if (_wasSwinging && pendulum != null) {
					string result = Outcomes.Name(pendulum.Evaluate());
					SpeechPipeline.SpeakEventInterrupt(result);
					Log.Info("pendulum stopped on " + result);
				}
				_wasSwinging = false;
				Stop();
				return;
			}
			OutcomeKind kind = Outcomes.Kind(pendulum.Evaluate());
			if (!_wasSwinging) Log.Info("pendulum swinging; outcome voices playing (now " + kind + ")");
			_wasSwinging = true;
			var parameters = new SoundParams(Pan(pendulum), 1f, 1f);
			if (!_voice.IsValid || kind != _playing) {
				Stop();
				_playing = kind;
				_voice = AudioEngine.PlayBuffer(_loops[(int)kind], _rate, true, parameters, SoundCategory.Minigames);
			} else {
				AudioEngine.Update(_voice, parameters);
			}
		}

		// The arm swings from -34 degrees (the game's left edge) to +34 (its right edge).
		private static float Pan(Pendulum pendulum) {
			var pivot = (Transform)PivotField.GetValue(pendulum);
			float angle = Mathf.DeltaAngle(0f, pivot.localEulerAngles.z);
			return Mathf.Clamp(angle / ArcHalfDegrees, -1f, 1f);
		}

		private void Stop() {
			if (_voice.IsValid) AudioEngine.Stop(_voice);
			_voice = HandOfFate2Access.Audio.Voice.None;
		}
	}
}
