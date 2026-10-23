using System.Collections.Generic;
using HandOfFate2Access.Localization;
using InControl;

namespace HandOfFate2Access.Help {
	/// <summary>
	/// The bindings of the combat actions in spoken form, read from the game's live
	/// action set (the player can rebind them in the settings). A blind player does not
	/// aim a mouse, so a keyboard key is named first; with a controller in use its
	/// button comes first; a mouse button is named only when nothing else is bound to
	/// the action (Defend ships bound to the right mouse button alone, which the help
	/// then points out).
	/// </summary>
	internal static class CombatKeyNames {
		public static CombatKeys Read() {
			Defiant.InputManager input = Defiant.InputManager.Instance;
			CombatActionSet set = input != null ? input.CombatActionSet : null;
			if (set == null) return null;
			LogGameBashLabel();
			return new CombatKeys {
				Move = Join(set.Up, set.Left, set.Down, set.Right),
				Attack = Name(set.Attack),
				Bash = Name(set.Stun),
				Counter = Name(set.Counter),
				Dodge = Name(set.Dodge),
				Weapon = Name(set.Weapon),
				Artifact = Name(set.Artifact),
				Companion = Name(set.Companion),
				Finisher = Name(set.ProneFinisher),
				Run = Name(set.Run),
				Camera = Join(set.OrbitUp, set.OrbitLeft, set.OrbitDown, set.OrbitRight),
				CounterKey = First(set.Counter),
				CounterMouseOnly = MouseOnly(set.Counter),
				BashKey = First(set.Stun),
			};
		}

		private static bool _bashLabelLogged;

		// The game's own label for the action the mod calls Bash (its input is named Stun;
		// the game's cards and encounters say Bash), logged once so each language's
		// table can follow the game's word.
		private static void LogGameBashLabel() {
			if (_bashLabelLogged) return;
			GlobalStrings strings = GlobalStrings.Instance;
			if (strings == null || strings.ControlStun == null) return;
			_bashLabelLogged = true;
			Util.Log.Info("game label for the Bash control (" + (LanguageManager.Instance != null ? LanguageManager.Instance.CurrentLanguage : "?") + "): '"
				+ strings.ControlStun.GetValue(strings.StringLibrary) + "'");
		}

		// A pad binding is only real while a pad is attached; without one its name is a
		// generic "Action4" that no player can press.
		private static bool PadAttached() {
			var devices = InControl.InputManager.Devices;
			return devices != null && devices.Count > 0;
		}

		// One key per action (the preferred binding of each), joined.
		private static string Join(params PlayerAction[] actions) {
			var keys = new List<string>();
			foreach (PlayerAction action in actions) {
				string key = First(action);
				if (key != null) keys.Add(key);
			}
			return keys.Count > 0 ? string.Join(" ", keys.ToArray()) : null;
		}

		// Every binding of one action, preferred first.
		private static string Name(PlayerAction action) {
			List<string> keys = Ordered(action);
			return keys.Count > 0 ? string.Join(", ", keys.ToArray()) : null;
		}

		/// <summary>The one binding to name for an action (keyboard first; a mouse button
		/// only when nothing else is bound), or null when it has none.</summary>
		public static string Preferred(PlayerAction action) => First(action);

		private static string First(PlayerAction action) {
			List<string> keys = Ordered(action);
			return keys.Count > 0 ? keys[0] : null;
		}

		/// <summary>True when the action can only be pressed with the mouse.</summary>
		public static bool MouseOnly(PlayerAction action) {
			if (action == null) return false;
			bool mouse = false;
			bool pad = PadAttached();
			foreach (BindingSource binding in action.Bindings) {
				if (binding is MouseBindingSource) mouse = true;
				else if (binding is KeyBindingSource || (pad && binding is DeviceBindingSource)) return false;
			}
			return mouse;
		}

		// Keyboard keys, then controller buttons, then mouse buttons; with a controller
		// in use its buttons come first. Duplicates collapse.
		private static List<string> Ordered(PlayerAction action) {
			var keyboard = new List<string>();
			var controller = new List<string>();
			var mouse = new List<string>();
			if (action == null) return keyboard;
			bool pad = PadAttached();
			foreach (BindingSource binding in action.Bindings) {
				if (binding is KeyBindingSource) Add(keyboard, KeyNames.Localize(binding.Name));
				else if (binding is MouseBindingSource) Add(mouse, MouseName(binding.Name));
				else if (pad && binding is DeviceBindingSource) Add(controller, ControllerName((DeviceBindingSource)binding));
			}
			bool usingController = Defiant.InputManager.ActiveDeviceClass == InputDeviceClass.Controller;
			var ordered = new List<string>();
			if (usingController) { ordered.AddRange(controller); ordered.AddRange(keyboard); }
			else { ordered.AddRange(keyboard); ordered.AddRange(controller); }
			ordered.AddRange(mouse);
			return ordered;
		}

		private static void Add(List<string> list, string key) {
			if (!string.IsNullOrEmpty(key) && !list.Contains(key)) list.Add(key);
		}

		private static string MouseName(string name) {
			switch (name) {
				case "LeftButton": return Strings.MouseLeft;
				case "RightButton": return Strings.MouseRight;
				case "MiddleButton": return Strings.MouseMiddle;
				default: return name;
			}
		}

		// The pad's own name for the button ("B" on an Xbox pad) when a pad is attached,
		// else the generic control name ("Action 2").
		private static string ControllerName(DeviceBindingSource binding) {
			string name = binding.Name;
			if (string.IsNullOrEmpty(name)) name = binding.Control.ToString();
			return KeyNames.Localize(name);
		}
	}
}
