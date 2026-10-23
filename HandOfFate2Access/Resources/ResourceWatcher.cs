using HandOfFate2Access.Combat;
using HandOfFate2Access.Resources;
using HandOfFate2Access.Speech;
using UnityEngine;

namespace HandOfFate2Access.Resources {
	/// <summary>
	/// Announces resource changes as they happen during a run: food, gold, fame and max
	/// health always; health only outside live combat, where every hit would otherwise
	/// be spoken over the fight's own audio. Polls the live stats each frame and speaks
	/// the difference from the previous frame. Outside a run (menus, loading, the
	/// starting values written when a run begins) the baseline is reset, never spoken.
	/// The previous sample is kept only to form the per-frame difference.
	/// </summary>
	internal sealed class ResourceWatcher {
		private bool _hasBaseline;
		private int _health, _maxHealth, _food, _gold, _fame;

		public void Pump() {
			Player p = Player.Instance;
			GameState state = Game.Instance != null ? Game.Instance.ActiveGameState : null;
			bool inRun = p != null && (state is GameState_Map || state is GameState_Level_Play);
			if (!inRun) {
				_hasBaseline = false;
				return;
			}
			int health = Mathf.RoundToInt(p.Health.Get());
			int maxHealth = Mathf.RoundToInt(p.MaxHealth.Get());
			int food = p.Food.Get();
			int gold = p.Gold.Get();
			int fame = p.Fame.Get();
			if (_hasBaseline) {
				if (!CombatGate.IsLive)
					Speak(ResourceKind.Health, health - _health);
				Speak(ResourceKind.MaxHealth, maxHealth - _maxHealth);
				Speak(ResourceKind.Food, food - _food);
				Speak(ResourceKind.Gold, gold - _gold);
				Speak(ResourceKind.Fame, fame - _fame);
			}
			_health = health;
			_maxHealth = maxHealth;
			_food = food;
			_gold = gold;
			_fame = fame;
			_hasBaseline = true;
		}

		private static void Speak(ResourceKind kind, int diff) {
			if (!Settings.ModSettings.ResourceChanges.On) return;
			string line = ResourceDelta.Compose(kind, diff);
			if (line.Length > 0)
				SpeechPipeline.SpeakEvent(line);
		}
	}
}
