using HandOfFate2Access.Util;
using InControl;
using UnityEngine;

namespace HandOfFate2Access.Input {
	/// <summary>
	/// Gives Attack, Defend and Block Break a keyboard key. The game ships them bound
	/// to the left, right and middle mouse buttons only (plus pad buttons), so a
	/// keyboard player cannot fight at all until they visit the bindings screen; a
	/// blind player would not know to. Attack gets V, Defend B and Block Break X, three
	/// keys the game leaves free in combat and the mod does not use, added as defaults
	/// (so the game's "reset to defaults" keeps them) only while the action has no
	/// keyboard key of its own: a key the player bound, or removed, is theirs. The
	/// menus' Cancel keeps Tab and gains Backspace as a second key. Re-checked every
	/// second because the game reloads the saved bindings over the set when the profile
	/// loads and when the player edits them. The game allows one binding per input type
	/// when listening for a new one, so a key never displaces a mouse or pad button.
	/// Never remaps an existing key.
	/// </summary>
	internal sealed class CombatKeyDefaults {
		private const float CheckSeconds = 1f;
		private float _next;

		public void Pump() {
			if (Time.unscaledTime < _next) return;
			_next = Time.unscaledTime + CheckSeconds;
			Defiant.InputManager input = Defiant.InputManager.Instance;
			CombatActionSet set = input != null ? input.CombatActionSet : null;
			if (set == null) return;
			Ensure(set, set.Attack, Key.V, "attack");
			Ensure(set, set.Counter, Key.B, "counter");
			Ensure(set, set.Stun, Key.X, "bash");
			UIActionSet ui = input.UIActionSet;
			if (ui != null) EnsureAlso(ui, ui.Cancel, Key.Backspace, "cancel");
		}

		private static readonly System.Collections.Generic.HashSet<string> Warned =
			new System.Collections.Generic.HashSet<string>();

		// Adds a second keyboard key to an action that already has one, unless the
		// player bound that key to another action of the set.
		private static void EnsureAlso(PlayerActionSet set, PlayerAction action, Key key, string name) {
			if (action == null) return;
			var candidate = new KeyBindingSource(key);
			foreach (BindingSource binding in action.Bindings)
				if (binding.Equals(candidate)) return;
			foreach (PlayerAction other in set.Actions) {
				if (other == action) continue;
				foreach (BindingSource binding in other.Bindings) {
					if (binding.Equals(candidate)) {
						if (Warned.Add(name)) Log.Warn(key + " is bound to " + other.Name + "; not adding it to " + name);
						return;
					}
				}
			}
			action.AddDefaultBinding(candidate);
			Log.Info(key + " added as a second keyboard key for " + name);
		}

		private static void Ensure(CombatActionSet set, PlayerAction action, Key key, string name) {
			if (action == null) return;
			foreach (BindingSource binding in action.Bindings)
				if (binding is KeyBindingSource) return;
			var candidate = new KeyBindingSource(key);
			// The key is the player's if they bound it to another action themselves.
			foreach (PlayerAction other in set.Actions) {
				if (other == action) continue;
				foreach (BindingSource binding in other.Bindings) {
					if (binding.Equals(candidate)) {
						Log.Warn(name + " has no keyboard key and " + key + " is bound to " + other.Name + "; not adding a default");
						return;
					}
				}
			}
			action.AddDefaultBinding(candidate);
			Log.Info(name + " had only mouse and pad bindings; " + key + " added as its keyboard default");
		}
	}
}
