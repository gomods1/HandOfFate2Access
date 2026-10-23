using System;
using System.Collections.Generic;
using System.IO;
using HandOfFate2Access.Audio;
using HandOfFate2Access.Util;
using UnityEngine;

namespace HandOfFate2Access.Combat {
	/// <summary>
	/// The walk-in beacons: the places a fight ends or pays out when the player stands on
	/// them. <c>ExitTrigger</c> is the exit of an "escape" fight (the encounter's
	/// <c>CombatObjectiveExitTrigger</c> completes when the player walks into it);
	/// <c>AICourtTrigger</c> is the court (face card) fights' end circle, armed at the
	/// boss's corpse once the fight is otherwise won; both carry the exit sample, since to
	/// the player both are the exit. <c>Loot</c> is the gold and food enemies drop, which
	/// collect by walking over them and carry the chest sample ("treasure, stand here").
	///
	/// Each live object pings on its own distance-scaled cadence (Core's
	/// <see cref="BeaconComposer"/>: the gap after each ping tightens as the player closes
	/// in), positioned like every other combat sound. Discovery is a periodic
	/// <c>FindObjectsOfType</c> scan: these objects appear mid-fight and have no registry.
	/// Liveness (completion, the collider the game enables when the object can be
	/// collected) is re-read from the live components every frame.
	/// </summary>
	internal sealed class ObjectBeacons {
		private const float ScanInterval = 1f;

		private bool _ready;
		// Each loaded sample's duration in seconds by key; doubles as the loaded set.
		private readonly Dictionary<string, float> _clips = new Dictionary<string, float>();

		private ExitTrigger[] _exits = new ExitTrigger[0];
		private Loot[] _loot = new Loot[0];
		private readonly Dictionary<Component, float> _nextPing = new Dictionary<Component, float>();
		private readonly List<Component> _deadPings = new List<Component>();
		private bool _inLevel;
		private float _nextScan;
		private int _lastExits = -1;
		private int _lastLoot = -1;
		private bool _courtWasActive;

		public void Initialize(string pluginDir) {
			if (!AudioEngine.IsAvailable) {
				Log.Warn("audio backend unavailable; object beacons disabled");
				return;
			}
			string soundsDir = Path.Combine(pluginDir, "sounds");
			LoadSample(soundsDir, BeaconComposer.ChestKey);
			LoadSample(soundsDir, BeaconComposer.ExitKey);
			_ready = true;
		}

		private void LoadSample(string soundsDir, string key) {
			string path = Path.Combine(soundsDir, key + ".wav");
			try {
				byte[] bytes = File.ReadAllBytes(path);
				WavAudio.Decode(bytes, out float[] pcm, out int channels, out int sampleRate);
				AudioEngine.Register(key, pcm, channels, sampleRate);
				_clips[key] = pcm.Length / (float)channels / sampleRate;
			} catch (Exception ex) {
				Log.Error("beacon sample '" + key + "' failed to load from " + path + ": " + ex);
			}
		}

		public void Pump() {
			if (!_ready) return;
			if (!CombatGate.IsLive || !CombatFrame.TryGet(out CombatFrame frame)) {
				Reset();
				return;
			}

			float now = Time.time;
			if (!_inLevel) {
				_inLevel = true;
				_nextScan = now;
			}
			if (now >= _nextScan) {
				Scan();
				_nextScan = now + ScanInterval;
			}

			AICourtTrigger court = AICourtTrigger.Instance;
			bool courtActive = court != null && court.IsActive;
			if (courtActive != _courtWasActive) {
				Log.Info("beacons: court trigger " + (courtActive ? "active" : "inactive"));
				_courtWasActive = courtActive;
			}

			if (_clips.TryGetValue(BeaconComposer.ChestKey, out float chestClip)) {
				for (int i = 0; i < _loot.Length; i++) {
					Loot loot = _loot[i];
					if (!IsCollectable(loot)) continue;
					PingOnCadence(BeaconComposer.ChestKey, chestClip, loot, loot.transform.position, 0f, frame, now);
				}
			}

			if (_clips.TryGetValue(BeaconComposer.ExitKey, out float exitClip)) {
				for (int i = 0; i < _exits.Length; i++) {
					ExitTrigger exit = _exits[i];
					if (!IsOpen(exit)) continue;
					PingOnCadence(BeaconComposer.ExitKey, exitClip, exit, Where(exit), BeaconComposer.ExitStagger, frame, now);
				}
				if (courtActive)
					PingOnCadence(BeaconComposer.ExitKey, exitClip, court, court.transform.position, 0f, frame, now);
			}
		}

		/// <summary>
		/// Whether anything is left to walk to: an open exit, the armed court circle or
		/// uncollected loot. The locator key answers with <see cref="PingNearest"/> then
		/// instead of "no enemies left".
		/// </summary>
		public bool HasTargets {
			get {
				if (!_ready || !CombatGate.IsLive) return false;
				AICourtTrigger court = AICourtTrigger.Instance;
				if (court != null && court.IsActive) return true;
				foreach (ExitTrigger exit in _exits) if (IsOpen(exit)) return true;
				foreach (Loot loot in _loot) if (IsCollectable(loot)) return true;
				return false;
			}
		}

		/// <summary>One ping at the nearest exit (preferred) or, with none, the nearest loot.</summary>
		public void PingNearest() {
			if (!_ready || !CombatFrame.TryGet(out CombatFrame frame)) return;
			var exits = new List<Vector3>();
			AICourtTrigger court = AICourtTrigger.Instance;
			if (court != null && court.IsActive) exits.Add(court.transform.position);
			foreach (ExitTrigger exit in _exits) if (IsOpen(exit)) exits.Add(Where(exit));
			if (exits.Count > 0 && _clips.ContainsKey(BeaconComposer.ExitKey)) {
				Vector3 target = Nearest(exits, frame.Origin);
				frame.Project(Guide(frame.Origin, target), out float right, out float forward);
				bool played = Ping(BeaconComposer.ExitKey, target, frame, out SoundParams p, out float distance);
				Log.Info("locator: exit " + distance.ToString("F1") + " away (right " + right.ToString("F1")
					+ ", forward " + forward.ToString("F1") + ")" + (played ? ", pan " + p.Pan.ToString("F2") : ", underfoot"));
				return;
			}
			var loot = new List<Vector3>();
			foreach (Loot l in _loot) if (IsCollectable(l)) loot.Add(l.transform.position);
			if (loot.Count > 0 && _clips.ContainsKey(BeaconComposer.ChestKey)) {
				Ping(BeaconComposer.ChestKey, Nearest(loot, frame.Origin), frame, out _, out float lootDistance);
				Log.Info("locator: loot " + lootDistance.ToString("F1") + " away");
			}
		}

		private static Vector3 Nearest(List<Vector3> points, Vector3 origin) {
			Vector3 best = points[0];
			for (int i = 1; i < points.Count; i++)
				if ((points[i] - origin).sqrMagnitude < (best - origin).sqrMagnitude) best = points[i];
			return best;
		}

		// An exit the player can still walk into: active, not yet completed, its trigger
		// collider enabled (the game disables it the moment the player enters).
		private static bool IsOpen(ExitTrigger exit) {
			if (exit == null || exit.IsComplete || !exit.gameObject.activeInHierarchy) return false;
			Collider collider = exit.GetComponent<Collider>();
			return collider == null || collider.enabled;
		}

		// Dropped loot becomes collectable once the game enables its collider (after a
		// short delay while it spills out); collecting destroys it.
		private static bool IsCollectable(Loot loot) {
			if (loot == null || !loot.gameObject.activeInHierarchy) return false;
			Collider collider = loot.GetComponent<Collider>();
			return collider != null && collider.enabled;
		}

		private static Vector3 Where(ExitTrigger exit) {
			Collider collider = exit.GetComponent<Collider>();
			return collider != null && collider.enabled ? collider.bounds.center : exit.transform.position;
		}

		private void PingOnCadence(string sampleKey, float clipDuration, Component key,
				Vector3 position, float firstDelay, CombatFrame frame, float now) {
			if (!_nextPing.TryGetValue(key, out float next)) {
				next = now + firstDelay;
				_nextPing[key] = next;
			}
			if (now < next) return;
			if (Ping(sampleKey, position, frame, out SoundParams played, out float distance))
				_nextPing[key] = BeaconComposer.NextPingTime(now, clipDuration, played.Pitch, distance);
			else
				_nextPing[key] = now + BeaconComposer.PingGap;
		}

		private static readonly UnityEngine.AI.NavMeshPath NavPath = new UnityEngine.AI.NavMeshPath();

		/// <summary>
		/// Where to sound a beacon from so that following it walks the player there: the
		/// next turn of the walkable path (the NavMesh route from the player to the
		/// object), not the straight line, which in a trap room or a walled arena points
		/// into a wall. Falls back to the object itself when no route is found or the
		/// object is in plain line of sight.
		/// </summary>
		public static Vector3 Guide(Vector3 origin, Vector3 target) {
			UnityEngine.AI.NavMeshHit from, to;
			if (!UnityEngine.AI.NavMesh.SamplePosition(origin, out from, 3f, UnityEngine.AI.NavMesh.AllAreas)) return target;
			if (!UnityEngine.AI.NavMesh.SamplePosition(target, out to, 5f, UnityEngine.AI.NavMesh.AllAreas)) return target;
			if (!UnityEngine.AI.NavMesh.CalculatePath(from.position, to.position, UnityEngine.AI.NavMesh.AllAreas, NavPath)) return target;
			Vector3[] corners = NavPath.corners;
			if (corners.Length < 3) return target;
			// The first corner more than a step away: a turn right at the player's feet
			// would flick the bearing about as they pass it.
			for (int i = 1; i < corners.Length - 1; i++) {
				Vector3 gap = corners[i] - origin;
				gap.y = 0f;
				if (gap.magnitude > 1.5f) return corners[i];
			}
			return target;
		}

		private static bool Ping(string key, Vector3 position, CombatFrame frame,
				out SoundParams parameters, out float distance) {
			position = Guide(frame.Origin, position);
			frame.Project(position, out float right, out float forward);
			if (!BeaconComposer.TryCompose(right, forward, out parameters, out distance)) return false;
			AudioEngine.PlayOneShot(key, parameters, SoundCategory.Locator);
			return true;
		}

		private void Scan() {
			_exits = UnityEngine.Object.FindObjectsOfType<ExitTrigger>();
			_loot = UnityEngine.Object.FindObjectsOfType<Loot>();

			foreach (KeyValuePair<Component, float> kv in _nextPing)
				if (kv.Key == null) _deadPings.Add(kv.Key);
			for (int i = 0; i < _deadPings.Count; i++) _nextPing.Remove(_deadPings[i]);
			_deadPings.Clear();

			if (_exits.Length != _lastExits || _loot.Length != _lastLoot) {
				Log.Info("beacons: " + _exits.Length + " exit(s), " + _loot.Length + " loot");
				foreach (ExitTrigger exit in _exits) {
					Vector3 at = Where(exit);
					Log.Info("beacon exit '" + exit.name + "' at (" + at.x.ToString("F1") + ", " + at.z.ToString("F1")
						+ "), open " + IsOpen(exit));
				}
				_lastExits = _exits.Length;
				_lastLoot = _loot.Length;
			}
		}

		private void Reset() {
			if (!_inLevel) return;
			_inLevel = false;
			_exits = new ExitTrigger[0];
			_loot = new Loot[0];
			_nextPing.Clear();
			_lastExits = -1;
			_lastLoot = -1;
			_courtWasActive = false;
		}
	}
}
