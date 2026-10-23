using System;
using System.Collections.Generic;
using HandOfFate2Access.Audio;

namespace HandOfFate2Access.Combat {
	/// <summary>
	/// Decides when the enemies around the player make themselves heard, so their movement
	/// is never silent: a footstep from each of the nearest few enemies while it moves (at a
	/// walking cadence, panned and pitched by the shared placement law, quieter with
	/// distance), and a double knock the moment an enemy comes within reach of the
	/// player's weapon (once per approach: it re-arms only after the enemy backs off past a
	/// margin, so an enemy hovering at the edge does not chatter). Movement is measured on
	/// the ground in world space, never in the camera's frame, so turning the camera is
	/// not mistaken for enemies walking.
	/// </summary>
	public sealed class EnemyPresenceTracker {
		public const string StepKey = "enemy_step";
		public const string ReachKey = "enemy_reach";

		/// <summary>Seconds between one moving enemy's footsteps.</summary>
		public const float StepSeconds = 0.38f;
		/// <summary>Ground speed, world units per second, above which an enemy counts as moving.</summary>
		public const float MovingSpeed = 1.5f;
		/// <summary>How many of the nearest moving enemies step at once; the rest stay quiet.</summary>
		public const int MaxStepping = 3;
		/// <summary>Ground distance at which an enemy is in reach (the player's default melee reach).</summary>
		public const float ReachRange = RangingCurve.FullRange;
		/// <summary>Distance an enemy in reach must back off past before its next approach knocks again.</summary>
		public const float ReachRelease = ReachRange + 1.5f;

		private const float StepMaxVolume = 0.7f;
		private const float StepMinVolume = 0.08f;
		private const float ReachVolume = 0.85f;

		/// <summary>One living enemy this frame: an identity, where it is on the ground
		/// (world X and Z, for its speed), and where it is from the player in the camera's
		/// frame (for its sound).</summary>
		public struct Enemy {
			public int Id;
			public float WorldX;
			public float WorldZ;
			public float Right;
			public float Forward;
		}

		/// <summary>A sound to play this frame.</summary>
		public struct Cue {
			public string Key;
			public SoundParams Params;
		}

		private sealed class State {
			public float X, Z;
			public float NextStep;
			public bool InReach;
			public bool Seen;
		}

		private readonly Dictionary<int, State> _states = new Dictionary<int, State>();
		private readonly List<int> _gone = new List<int>();
		private readonly List<KeyValuePair<float, int>> _moving = new List<KeyValuePair<float, int>>();

		/// <summary>Forgets every enemy (a fight ended).</summary>
		public void Clear() => _states.Clear();

		/// <summary>
		/// The cues for this frame, given every living enemy now, the time, and the frame's
		/// length. An enemy seen for the first time only sets its baseline.
		/// </summary>
		public List<Cue> Update(IList<Enemy> enemies, float now, float deltaTime) {
			var cues = new List<Cue>();
			foreach (State s in _states.Values) s.Seen = false;
			_moving.Clear();
			for (int i = 0; i < enemies.Count; i++) {
				Enemy e = enemies[i];
				float distance = (float)Math.Sqrt(e.Right * e.Right + e.Forward * e.Forward);
				State s;
				if (!_states.TryGetValue(e.Id, out s)) {
					s = new State { X = e.WorldX, Z = e.WorldZ, NextStep = now, InReach = distance <= ReachRange };
					_states[e.Id] = s;
					s.Seen = true;
					continue;
				}
				s.Seen = true;
				float dx = e.WorldX - s.X, dz = e.WorldZ - s.Z;
				s.X = e.WorldX;
				s.Z = e.WorldZ;
				float speed = deltaTime > 0f ? (float)Math.Sqrt(dx * dx + dz * dz) / deltaTime : 0f;
				if (speed > MovingSpeed && !float.IsNaN(speed)) _moving.Add(new KeyValuePair<float, int>(distance, i));
				else if (now > s.NextStep) s.NextStep = now;   // a fresh start steps at once

				if (!s.InReach && distance <= ReachRange) {
					s.InReach = true;
					cues.Add(new Cue { Key = ReachKey, Params = Placed(e, ReachVolume) });
				} else if (s.InReach && distance > ReachRelease) {
					s.InReach = false;
				}
			}
			_moving.Sort((a, b) => a.Key.CompareTo(b.Key));
			for (int m = 0; m < _moving.Count && m < MaxStepping; m++) {
				Enemy e = enemies[_moving[m].Value];
				State s = _states[e.Id];
				if (now < s.NextStep) continue;
				s.NextStep = now + StepSeconds;
				float volume = StepMinVolume + (StepMaxVolume - StepMinVolume) * RangingCurve.Closeness(_moving[m].Key);
				cues.Add(new Cue { Key = StepKey, Params = Placed(e, volume) });
			}
			_gone.Clear();
			foreach (KeyValuePair<int, State> pair in _states)
				if (!pair.Value.Seen) _gone.Add(pair.Key);
			foreach (int id in _gone) _states.Remove(id);
			return cues;
		}

		// Bearing by the shared placement law (pan and the behind-darkening pitch), at a volume.
		private static SoundParams Placed(Enemy e, float volume) {
			SoundParams bearing = ProjectileSonifier.Compose(e.Right, e.Forward);
			return new SoundParams(bearing.Pan, bearing.Pitch, volume);
		}
	}
}
