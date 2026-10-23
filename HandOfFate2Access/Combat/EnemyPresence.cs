using System.Collections.Generic;
using System.IO;
using HandOfFate2Access.Audio;
using HandOfFate2Access.Util;
using UnityEngine;

namespace HandOfFate2Access.Combat {
	/// <summary>
	/// Makes the enemies' movement heard from where they are relative to the player: the
	/// nearest few moving enemies step (sounds/enemy_step.wav) and an enemy coming within
	/// reach of the player's weapon knocks twice (sounds/enemy_reach.wav), as decided by
	/// Core's <see cref="EnemyPresenceTracker"/>. The game's own footsteps play as always
	/// (they are placed from the camera, above and behind the player, so they carry little
	/// direction); these are added on top. Live fights only; hidden enemies (a burrowed
	/// gnome, a submerged kraken) are not heard, as they are not seen.
	/// </summary>
	internal sealed class EnemyPresence {
		private readonly EnemyPresenceTracker _tracker = new EnemyPresenceTracker();
		private readonly List<EnemyPresenceTracker.Enemy> _enemies = new List<EnemyPresenceTracker.Enemy>();
		private bool _ready;
		private bool _wasLive;

		public void Initialize(string pluginDir) {
			if (!AudioEngine.IsAvailable) {
				Log.Warn("audio backend unavailable; enemy movement cues disabled");
				return;
			}
			string soundsDir = Path.Combine(pluginDir, "sounds");
			bool step = Load(soundsDir, EnemyPresenceTracker.StepKey);
			bool reach = Load(soundsDir, EnemyPresenceTracker.ReachKey);
			_ready = step || reach;
		}

		private static bool Load(string soundsDir, string key) {
			string path = Path.Combine(soundsDir, key + ".wav");
			try {
				WavAudio.Decode(File.ReadAllBytes(path), out float[] pcm, out int channels, out int sampleRate);
				AudioEngine.Register(key, pcm, channels, sampleRate);
				return true;
			} catch (System.Exception ex) {
				Log.Error("enemy movement sound '" + key + "' failed to load from " + path + ": " + ex.Message);
				return false;
			}
		}

		public void Pump() {
			if (!_ready) return;
			bool live = CombatGate.IsLive && CombatManager.Instance != null;
			if (!live) {
				if (_wasLive) _tracker.Clear();
				_wasLive = false;
				return;
			}
			_wasLive = true;
			if (!CombatFrame.TryGet(out CombatFrame frame)) return;

			_enemies.Clear();
			List<Targetable> targets = CombatManager.Instance.Targets;
			for (int i = 0; i < targets.Count; i++) {
				Targetable target = targets[i];
				if (target == null || target.Team != TeamType.Enemy) continue;
				if (target.Destroyable == null || target.Destroyable.IsDead || target.Hidden) continue;
				Vector3 position = target.transform.position;
				frame.Project(position, out float right, out float forward);
				_enemies.Add(new EnemyPresenceTracker.Enemy {
					Id = target.GetInstanceID(),
					WorldX = position.x,
					WorldZ = position.z,
					Right = right,
					Forward = forward,
				});
			}
			foreach (EnemyPresenceTracker.Cue cue in _tracker.Update(_enemies, Time.time, Time.deltaTime))
				AudioEngine.PlayOneShot(cue.Key, cue.Params, SoundCategory.EnemyMovement);
		}
	}
}
