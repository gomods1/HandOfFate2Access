using System.Collections.Generic;
using HandOfFate2Access.Speech;
using UnityEngine;

namespace HandOfFate2Access.Combat {
	/// <summary>
	/// Speaks the fight's bookkeeping a sighted player reads off the HUD: the number of
	/// enemies when the fight starts and whenever it changes (kills, new waves; changes
	/// within half a second are spoken once, so a multi-kill or a wave reads as one
	/// number), and the player's health each time it falls below half, a quarter and a
	/// tenth of the maximum. The enemy count is read live from the game's target list.
	/// </summary>
	internal sealed class CombatNarration {
		private const float SettleSeconds = 0.5f;

		private bool _live;
		private int _spoken = -1;
		private int _pending = -1;
		private float _pendingSince;
		private float _lastHealthFraction = 1f;
		private bool _wasSnared;

		public bool Pump() {
			if (!CombatGate.IsLive) {
				_live = false;
				_spoken = -1;
				_pending = -1;
				_lastHealthFraction = 1f;
				_wasSnared = false;
				return false;
			}
			bool spoke = false;
			// An enemy latched on (a gnome swarm, a mage's snare): the game's own cue is
			// a wobbling icon and the player must mash the counter key until free. Said
			// the moment it starts, with the key, and again when it ends; the 1-damage
			// ticks meanwhile are the snare itself, not strikes to telegraph.
			PlayerController player = PlayerController.Instance;
			bool snared = player != null && player.IsSnared(0f);
			if (snared != _wasSnared) {
				_wasSnared = snared;
				if (snared) {
					Help.CombatKeys keys = Help.CombatKeyNames.Read();
					// With Counter bound to a mouse button alone, the action is named instead of
					// the button (a blind player does not use the mouse; the help says to bind a key).
					string key = keys != null && keys.CounterKey != null && !keys.CounterMouseOnly
						? keys.CounterKey : Localization.Strings.CombatCounter;
					SpeechPipeline.SpeakEventInterrupt(Localization.Strings.Snared(key));
				} else {
					SpeechPipeline.SpeakEvent(Localization.Strings.SnareFree);
				}
				spoke = true;
			}
			int count = CountEnemies();
			if (!_live) {
				_live = true;
				_spoken = count;
				_pending = count;
				_lastHealthFraction = HealthFraction();
				SpeechPipeline.SpeakEvent(CombatReadout.FightStart(count));
				return true;
			}
			if (count != _pending) {
				_pending = count;
				_pendingSince = Time.unscaledTime;
			} else if (_pending != _spoken && Time.unscaledTime - _pendingSince >= SettleSeconds) {
				_spoken = _pending;
				SpeechPipeline.SpeakEvent(CombatReadout.EnemiesLeft(_spoken));
				spoke = true;
			}

			float fraction = HealthFraction();
			if (CombatReadout.CrossedThreshold(_lastHealthFraction, fraction) >= 0) {
				Player p = Player.Instance;
				SpeechPipeline.SpeakEventInterrupt(CombatReadout.HealthWarning(
					Mathf.RoundToInt(p.Health.Get()), Mathf.RoundToInt(p.MaxHealth.Get())));
				spoke = true;
			}
			_lastHealthFraction = fraction;
			return spoke;
		}

		/// <summary>
		/// Living enemies, hidden ones included: a gnome burrowed under the arena or a
		/// kraken below the surface is still an enemy to beat, and counting only the
		/// visible ones made a gnome swarm chatter "7 enemies, 8 enemies" with every dive.
		/// The locator, which must point at something attackable, does its own filtering.
		/// </summary>
		public static int CountEnemies() {
			CombatManager manager = CombatManager.Instance;
			if (manager == null) return 0;
			int count = 0;
			List<Targetable> targets = manager.Targets;
			for (int i = 0; i < targets.Count; i++) {
				Targetable t = targets[i];
				if (t == null || t.Team != TeamType.Enemy) continue;
				if (t.Destroyable == null || t.Destroyable.IsDead) continue;
				count++;
			}
			return count;
		}

		private static float HealthFraction() {
			Player p = Player.Instance;
			if (p == null) return 1f;
			float max = p.MaxHealth.Get();
			return max > 0f ? p.Health.Get() / max : 1f;
		}
	}
}
