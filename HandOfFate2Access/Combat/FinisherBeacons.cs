using System;
using System.Collections.Generic;
using System.IO;
using HandOfFate2Access.Audio;
using HandOfFate2Access.Util;
using UnityEngine;

namespace HandOfFate2Access.Combat {
	/// <summary>
	/// The finish beacons: every enemy the player's finisher can take (knocked down) pings
	/// the rising swish from where it lies, on Core's <see cref="FinisherBeaconComposer"/>
	/// cadence, until it gets up or dies. A sighted player sees the enemy on the ground and
	/// the game's button hint over it; this is that, by ear. <see cref="IsFinishable"/> is
	/// the game's own test for a finisher target (<c>TargetUtils.FindProneTarget</c>,
	/// without its range and facing cone): "go there". <see cref="ReadyTarget"/> is the
	/// whole test, as the finisher key's own condition (<c>ConditionProneTarget</c>) asks
	/// it: within the finisher's range and cone around the direction the player pushes
	/// (or faces, standing still). That enemy pings the ready sound instead: "press it now".
	/// </summary>
	internal sealed class FinisherBeacons {
		// How often the reach test runs (the game's own finisher hint posts at this rate).
		private const float ReadyInterval = 0.1f;

		private bool _ready;
		private readonly Dictionary<string, float> _clips = new Dictionary<string, float>();
		private readonly Dictionary<Targetable, float> _nextPing = new Dictionary<Targetable, float>();
		private readonly List<Targetable> _stale = new List<Targetable>();
		private readonly HashSet<Targetable> _down = new HashSet<Targetable>();
		private Targetable _inReach;
		private float _nextReadyCheck;

		public void Initialize(string pluginDir) {
			if (!AudioEngine.IsAvailable) {
				Log.Warn("audio backend unavailable; finish beacons disabled");
				return;
			}
			string soundsDir = Path.Combine(pluginDir, "sounds");
			LoadSample(soundsDir, FinisherBeaconComposer.Key);
			LoadSample(soundsDir, FinisherBeaconComposer.ReadyKey);
			_ready = _clips.ContainsKey(FinisherBeaconComposer.Key);
		}

		private void LoadSample(string soundsDir, string key) {
			string path = Path.Combine(soundsDir, key + ".wav");
			try {
				WavAudio.Decode(File.ReadAllBytes(path), out float[] pcm, out int channels, out int sampleRate);
				AudioEngine.Register(key, pcm, channels, sampleRate);
				_clips[key] = pcm.Length / (float)channels / sampleRate;
			} catch (Exception ex) {
				Log.Error("finish beacon sample '" + key + "' failed to load from " + path + ": " + ex);
			}
		}

		/// <summary>Whether the finish sound is loaded (the locator falls back to its ping).</summary>
		public bool Ready => _ready;

		/// <summary>
		/// The finish sample for <paramref name="target"/>: the ready sound when the
		/// finisher would land on it now, the swish when it is down out of reach, null when
		/// it cannot be finished.
		/// </summary>
		public string SampleFor(Targetable target) {
			if (!_ready || !IsFinishable(target, Player())) return null;
			if (target == ReadyTarget() && _clips.ContainsKey(FinisherBeaconComposer.ReadyKey))
				return FinisherBeaconComposer.ReadyKey;
			return FinisherBeaconComposer.Key;
		}

		/// <summary>
		/// Whether the player's finisher can take <paramref name="target"/> now: a living
		/// enemy lying prone (knocked down, or in a prone action the game counts as
		/// finishable), not locked out of finishers (Kallas's fight unlocks him), attackable
		/// by the player and not excluded from targeting.
		/// </summary>
		public static bool IsFinishable(Targetable target, Targetable player) {
			if (target == null || player == null || target.Team != TeamType.Enemy) return false;
			if (target.Destroyable == null || target.Destroyable.IsDead || target.Hidden || target.IgnoreTargeting) return false;
			Controller controller = target.Controller;
			if (object.ReferenceEquals(controller, null) || !controller.Prone.HasValue) return false;
			if (controller.Prone.Value.Locked
					&& !(CombatEffectKallasFight.KallasFight > 0 && controller.GetComponent<Kallas>() != null)) return false;
			return TargetUtils.IsTargetProne(target) && player.CanAttack(target);
		}

		/// <summary>The player's targetable, which finisher targeting is measured from; null outside a fight.</summary>
		public static Targetable Player() {
			PlayerController player = PlayerController.Instance;
			return player != null ? player.Targetable : null;
		}

		/// <summary>
		/// The enemy the finisher key would take now, as its condition picks it: the best
		/// fallen enemy within the finisher's range and cone around the move direction, or
		/// the facing with the stick at rest. Null when none.
		/// </summary>
		public static Targetable ReadyTarget() {
			PlayerController player = PlayerController.Instance;
			if (player == null || player.Combat == null || player.Targetable == null) return null;
			PlayerCombatProne prone = player.Combat.Prone;
			Defiant.InputManager input = Defiant.InputManager.Instance;
			if (prone == null || input == null) return null;
			Vector2 axis = input.GetRelativeAxis(InputAxis.Move);
			Vector3 direction = new Vector3(axis.x, 0f, axis.y).normalized;
			if (direction.sqrMagnitude < 0.1f) direction = player.transform.forward;
			return TargetUtils.FindProneTarget(player.Targetable, direction, prone.Range, prone.Angle);
		}

		public void Pump() {
			if (!_ready) return;
			if (CombatManager.Instance == null || !CombatFrame.TryGet(out CombatFrame frame)) {
				_nextPing.Clear();
				_down.Clear();
				_inReach = null;
				return;
			}
			Targetable player = Player();
			float now = Time.time;
			if (now >= _nextReadyCheck) {
				_nextReadyCheck = now + ReadyInterval;
				Targetable inReach = _down.Count > 0 ? ReadyTarget() : null;
				if (inReach != _inReach) {
					// Coming into reach is said at once, not after the far cadence's gap.
					if (inReach != null) {
						_nextPing.Remove(inReach);
						Log.Info("finisher in reach: " + inReach.name);
					}
					_inReach = inReach;
				}
			}
			List<Targetable> targets = CombatManager.Instance.Targets;
			for (int i = 0; i < targets.Count; i++) {
				Targetable target = targets[i];
				if (!IsFinishable(target, player)) continue;
				if (_down.Add(target)) Log.Info("finishable: " + target.name);
				if (_nextPing.TryGetValue(target, out float next) && now < next) continue;
				string key = target == _inReach && _clips.ContainsKey(FinisherBeaconComposer.ReadyKey)
					? FinisherBeaconComposer.ReadyKey : FinisherBeaconComposer.Key;
				frame.Project(target.transform.position, out float right, out float forward);
				SoundParams p = FinisherBeaconComposer.Compose(right, forward, out float distance);
				AudioEngine.PlayOneShot(key, p, SoundCategory.Locator);
				_nextPing[target] = FinisherBeaconComposer.NextPingTime(now, _clips[key], p.Pitch, distance);
			}
			// An enemy back on its feet (or dead) drops out, so its next fall pings at once.
			foreach (Targetable target in _down)
				if (!IsFinishable(target, player)) _stale.Add(target);
			for (int i = 0; i < _stale.Count; i++) {
				_down.Remove(_stale[i]);
				_nextPing.Remove(_stale[i]);
				if (_stale[i] == _inReach) _inReach = null;
				Log.Info("no longer finishable: " + (_stale[i] != null ? _stale[i].name : "destroyed"));
			}
			_stale.Clear();
		}
	}
}
