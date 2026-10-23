using System.Collections.Generic;
using System.Reflection;
using HandOfFate2Access.Focus;
using HandOfFate2Access.Help;
using HandOfFate2Access.Localization;
using HandOfFate2Access.Speech;
using HarmonyLib;
using InControl;

namespace HandOfFate2Access.Screens {
	/// <summary>
	/// Speaks modal dialogues as they open. Dialogues (confirmations, errors, the
	/// combat controls tutorial) are pushes on MenuManager's Dialogue stack, invisible to
	/// the game-state machine. When a new dialogue tops the stack its body text is spoken;
	/// the controls tutorial instead speaks each control it shows as a line of its own
	/// ("Attack: V"), with the key read from the live InControl action the row stands
	/// for (keyboard first, a mouse button only when nothing else is bound: the diagram
	/// itself draws the mouse buttons the game ships). Each line is queued, so the
	/// player hears them one by one and the Events buffer keeps one per control. The
	/// buttons are read by the focus path.
	/// </summary>
	internal sealed class DialogueNarration {
		private static readonly FieldInfo BodyField = AccessTools.Field(typeof(UIDialogue), "m_body");

		private Menu _current;
		private readonly LabelWatch _body = new LabelWatch();
		private bool _controlsSpoken;

		/// <summary>Speaks a newly opened dialogue; true when something was spoken.</summary>
		public bool Pump() {
			MenuManager menus = MenuManager.Instance;
			// The splash scene's MenuManager has no Dialogue stack at all.
			Menu top = menus != null && menus.GetStackIndex(MenuManager.StackPriority.Dialogue) >= 0
				? menus.CurrentMenu(MenuManager.StackPriority.Dialogue) : null;
			if (top != _current) {
				_current = top;
				_body.Reset();
				_controlsSpoken = false;
			}
			if (top == null) return false;

			ControlDiagramManager diagram = top.GetComponentInChildren<ControlDiagramManager>();
			if (diagram != null) {
				if (_controlsSpoken) return false;
				List<string> controls = ReadControls(diagram);
				if (controls.Count == 0) return false;
				_controlsSpoken = true;
				foreach (string line in controls) SpeechPipeline.SpeakEvent(line);
				return true;
			}

			UIDialogue ui = top.GetComponentInChildren<UIDialogue>();
			if (ui == null) return false;
			string body = _body.Poll((UILabel)BodyField.GetValue(ui));
			if (body == null) return false;
			SpeechPipeline.SpeakEvent(body);
			return true;
		}

		// Every control row the active diagram shows, in hierarchy order: the action's
		// localized label, then its key. A row is a transform with a "Label" child (the
		// action) and an "Input" child; the key comes from the row's InputControlsButton
		// (the game's action it stands for) through the live bindings, else from the
		// key cap the row draws.
		private static List<string> ReadControls(ControlDiagramManager diagram) {
			var lines = new List<string>();
			foreach (UnityEngine.Transform row in diagram.GetComponentsInChildren<UnityEngine.Transform>(false)) {
				UnityEngine.Transform labelNode = row.Find("Label");
				UnityEngine.Transform input = row.Find("Input");
				if (labelNode == null || input == null) continue;
				string name = UiText.Read(labelNode.GetComponent<UILabel>());
				if (string.IsNullOrEmpty(name)) continue;
				string key = BoundKey(row) ?? DrawnKey(input);
				lines.Add(string.IsNullOrEmpty(key) ? name : Strings.HelpKey(name, key));
			}
			return lines;
		}

		// The preferred binding of the combat action the row stands for; null when the
		// row names no action or the action has no binding.
		private static string BoundKey(UnityEngine.Transform row) {
			InputControlsButton button = row.GetComponentInChildren<InputControlsButton>(true);
			Defiant.InputManager input = Defiant.InputManager.Instance;
			CombatActionSet set = input != null ? input.CombatActionSet : null;
			if (button == null || set == null) return null;
			string wanted = button.Button.ToString();
			foreach (PlayerAction action in set.Actions)
				if (action.Name == wanted) return CombatKeyNames.Preferred(action);
			return null;
		}

		// The key the diagram draws: a key cap label (W, Space, L Ctrl) or a button
		// sprite named in the mod's strings.
		private static string DrawnKey(UnityEngine.Transform input) {
			foreach (UILabel keyLabel in input.GetComponentsInChildren<UILabel>(false)) {
				string key = UiText.Read(keyLabel);
				if (!string.IsNullOrEmpty(key)) return KeyNames.Localize(key);
			}
			foreach (UISprite sprite in input.GetComponentsInChildren<UISprite>(false)) {
				string key = InputGlyphs.Name(sprite.spriteName);
				if (key != null) return KeyNames.Localize(key);
			}
			return null;
		}
	}
}
