using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using HandOfFate2Access.Util;
using UnityEngine;

namespace HandOfFate2Access.Diagnostics {
	/// <summary>
	/// Playtest-only latency probe, active while <c>latency-probe.enabled</c> exists in the
	/// plugin folder. Logs, for each key press, how long after the frame that saw it the
	/// game dispatched it, focus changed and speech went out; every five seconds, the
	/// frame times and the mod's own cost per frame, with the pumps that cost the most.
	/// </summary>
	internal static class LatencyProbe {
		private static bool _enabled;
		private static string _pluginDir;
		private static float _keyAt = -1f;
		private static int _keyFrame;
		private static readonly HashSet<string> _marked = new HashSet<string>();

		private static float _windowStart;
		private static int _frames;
		private static float _maxFrame;
		private static double _modTotal;
		private static double _modMax;
		private static readonly Dictionary<string, double> _pumpMax = new Dictionary<string, double>();
		private static readonly Stopwatch _frameWatch = new Stopwatch();

		public static bool Enabled => _enabled;

		public static void Initialize(string pluginDir) {
			_pluginDir = pluginDir;
			_enabled = File.Exists(Path.Combine(pluginDir, "latency-probe.enabled"));
			if (_enabled) Log.Info("latency probe enabled");
		}

		/// <summary>Start of the plugin's Update: stamps key presses and frame times.</summary>
		public static void BeginFrame() {
			if (!_enabled) return;
			_frameWatch.Reset();
			_frameWatch.Start();
			_frames++;
			if (Time.unscaledDeltaTime > _maxFrame) _maxFrame = Time.unscaledDeltaTime;
			if (UnityEngine.Input.anyKeyDown) {
				// Time.unscaledTime is the start of this frame, when Unity read the key.
				_keyAt = Time.unscaledTime;
				_keyFrame = Time.frameCount;
				_marked.Clear();
				Log.Info("lat key down frame " + _keyFrame + ", frame time " + Ms(Time.unscaledDeltaTime));
			}
		}

		/// <summary>One pump's cost this frame.</summary>
		public static void Pump(string name, double ms) {
			if (!_enabled) return;
			double max;
			if (!_pumpMax.TryGetValue(name, out max) || ms > max) _pumpMax[name] = ms;
		}

		/// <summary>End of the plugin's Update: totals the mod's cost and logs the window.</summary>
		public static void EndFrame() {
			if (!_enabled) return;
			_frameWatch.Stop();
			double ms = _frameWatch.Elapsed.TotalMilliseconds;
			_modTotal += ms;
			if (ms > _modMax) _modMax = ms;
			float now = Time.realtimeSinceStartup;
			if (now - _windowStart < 5f) return;
			if (File.Exists(Path.Combine(_pluginDir, "latency-probe.novsync")) && QualitySettings.vSyncCount != 0) {
				QualitySettings.vSyncCount = 0;
				Log.Info("lat experiment: vsync off");
			}
			var top = new List<KeyValuePair<string, double>>(_pumpMax);
			top.Sort((a, b) => b.Value.CompareTo(a.Value));
			var parts = new List<string>();
			for (int i = 0; i < top.Count && i < 5; i++)
				parts.Add(top[i].Key + " " + top[i].Value.ToString("0.0"));
			Log.Info("lat window: " + _frames + " frames in " + (now - _windowStart).ToString("0.0")
				+ " s, max frame " + Ms(_maxFrame) + ", mod avg " + (_frames > 0 ? (_modTotal / _frames).ToString("0.00") : "0")
				+ " ms max " + _modMax.ToString("0.0") + " ms; slowest pumps (ms): " + string.Join(", ", parts.ToArray())
				+ "; vsync " + QualitySettings.vSyncCount + ", target fps " + Application.targetFrameRate
				+ ", screen " + Screen.width + "x" + Screen.height + " @" + Screen.currentResolution.refreshRate
				+ (Screen.fullScreen ? " fullscreen" : " windowed") + ", focused " + Application.isFocused
				+ ", run in background " + Application.runInBackground + ", gpu " + SystemInfo.graphicsDeviceName);
			_windowStart = now;
			_frames = 0;
			_maxFrame = 0f;
			_modTotal = 0;
			_modMax = 0;
			_pumpMax.Clear();
		}

		/// <summary>A stage reached for the last key press, logged once per press.</summary>
		public static void Mark(string stage) {
			if (!_enabled || _keyAt < 0f) return;
			float since = Time.realtimeSinceStartup - _keyAt;
			if (since > 3f || !_marked.Add(stage)) return;
			Log.Info("lat " + stage + " +" + Ms(since) + ", +" + (Time.frameCount - _keyFrame) + " frames");
		}

		private static string Ms(float seconds) => (seconds * 1000f).ToString("0") + " ms";
	}
}
