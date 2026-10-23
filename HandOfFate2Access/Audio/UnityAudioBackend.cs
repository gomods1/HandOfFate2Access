using System;
using HandOfFate2Access.Util;
using UnityEngine;

namespace HandOfFate2Access.Audio {
	/// <summary>
	/// Hosts Core's one stereo mixer inside Unity's own audio graph. A persistent
	/// GameObject carries a 2D AudioSource playing a silent looping clip; the filter
	/// component on it overwrites the source's buffer with the mixer's output from
	/// OnAudioFilterRead, on Unity's audio thread. Every placement decision (pan,
	/// interaural delay, pitch, distance) is made in Core, so the source itself is
	/// kept neutral: fully 2D, centre pan, no effects, and it ignores the game's
	/// listener pause so the mod's cues keep sounding over a paused menu.
	/// </summary>
	public sealed class UnityAudioBackend : IAudioBackend {
		private GameObject _host;
		private int _rate;
		private bool _mono;
		private bool _initialized;
		private bool _available;
		private MixerFilter _filter;
		private AudioSource _source;

		public bool IsInitialized => _initialized;
		public bool IsAvailable => _available;
		public int OutputSampleRate => _rate;
		public bool IsOutputMono => _mono;

		public bool Initialize() {
			if (_initialized) return _available;
			_initialized = true;
			try {
				_rate = AudioSettings.outputSampleRate;
				_mono = AudioSettings.speakerMode == AudioSpeakerMode.Mono;
				_available = _rate > 0;
				Log.Info("unity audio backend: " + _rate + "Hz, " + AudioSettings.speakerMode);
				return _available;
			} catch (Exception ex) {
				Log.Error("unity audio backend init failed: " + ex);
				_available = false;
				return false;
			}
		}

		public bool Host(IPcmSource mixer) {
			if (!_available) return false;
			if (_host != null) return true;
			try {
				_host = new GameObject("HoF2Access.Audio");
				ScenePersistence.Protect(_host);
				AudioSource source = _host.AddComponent<AudioSource>();
				_source = source;
				source.clip = AudioClip.Create("HoF2Access.Silence", _rate, 1, _rate, false);
				source.loop = true;
				source.spatialBlend = 0f;
				source.panStereo = 0f;
				source.volume = 1f;
				source.priority = 0;
				source.bypassEffects = true;
				source.bypassListenerEffects = true;
				source.bypassReverbZones = true;
				source.ignoreListenerPause = true;
				source.dopplerLevel = 0f;
				_filter = _host.AddComponent<MixerFilter>();
				_filter.Mixer = mixer;
				source.Play();
				return true;
			} catch (Exception ex) {
				Log.Error("unity audio backend host failed: " + ex);
				_available = false;
				return false;
			}
		}

		private float _nextReport;

		public void Pump() {
			if (_filter == null) return;
			string fault = _filter.TakeFault();
			if (fault != null)
				Log.Warn("audio mixer callback faulted (" + fault + ")");
			// The game resets Unity's audio system when it applies its audio settings, which
			// stops every playing source; restart ours so the mod's sound never goes dead.
			if (!_source.isPlaying) {
				Log.Warn("audio host source was stopped (audio reset?); restarting");
				_source.Play();
			}
			if (Time.unscaledTime >= _nextReport) {
				_nextReport = Time.unscaledTime + 10f;
				Log.Info("audio host: " + _filter.Callbacks + " callbacks, peak " + _filter.Peak.ToString("0.000")
					+ ", playing " + _source.isPlaying + ", listener volume " + AudioListener.volume
					+ ", listener paused " + AudioListener.pause + ", rate " + AudioSettings.outputSampleRate);
				_filter.Callbacks = 0;
				_filter.Peak = 0f;
			}
		}

		public void Shutdown() {
			if (_host != null)
				UnityEngine.Object.Destroy(_host);
			_host = null;
			_initialized = false;
			_available = false;
		}
	}

	/// <summary>The audio-thread hook: fills Unity's buffer from the Core mixer.</summary>
	public sealed class MixerFilter : MonoBehaviour {
		public IPcmSource Mixer;
		private volatile string _fault;

		public string TakeFault() {
			string f = _fault;
			_fault = null;
			return f;
		}

		// Health counters for the periodic diagnostic: callbacks since the last report and
		// the loudest sample written. Written on the audio thread, read and reset on main.
		public volatile int Callbacks;
		public volatile float Peak;

		private void OnAudioFilterRead(float[] data, int channels) {
			IPcmSource mixer = Mixer;
			if (mixer == null) return;
			try {
				mixer.Fill(data, channels, data.Length / channels);
				float peak = Peak;
				for (int i = 0; i < data.Length; i++) {
					float a = data[i] < 0 ? -data[i] : data[i];
					if (a > peak) peak = a;
				}
				Peak = peak;
				Callbacks++;
			} catch (Exception ex) {
				_fault = ex.Message;
				Array.Clear(data, 0, data.Length);
			}
		}
	}
}
