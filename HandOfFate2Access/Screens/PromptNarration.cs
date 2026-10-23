using System.Collections.Generic;
using System.Reflection;
using HandOfFate2Access.Focus;
using HandOfFate2Access.Speech;
using HandOfFate2Access.UI;
using HarmonyLib;
using UnityEngine;

namespace HandOfFate2Access.Screens {
	/// <summary>
	/// The prompt the game shows in its bottom navigation bar for the current decision
	/// ("Equip the Warrior's Axe?" with Yes / No). The bar's title comes from the focused
	/// selectable's or group's TitleContextText and its buttons from their Select/Cancel
	/// context texts, so this is where a decision's question and answers are written.
	/// When a bar's title changes to a new prompt it is spoken with the bar's visible
	/// button labels. A bar with no title (the map's standing inventory/map shortcuts)
	/// is not spoken.
	///
	/// The bar's hotkey buttons (F1/F2/F3: "Finish" on the character screen, the map's
	/// shortcuts) are actions reachable only by their key, never by focus, so a button
	/// is spoken with its key ("Finish, F3") when it appears; one heard in the last few
	/// minutes is not repeated (the map's shortcuts come back after every encounter).
	/// While the dice wait in hand, the bar ("Click and hold to grab dice", Throw) is not
	/// read: it is a mouse instruction, and the focus readout already says which key
	/// throws. The inventory's Inspect button is never announced: Enter equips directly, and the
	/// inspect view shows the same card panel the details buffer (Control plus up arrow)
	/// already reads. The action keys setting turns the hotkey announcements off.
	/// </summary>
	internal sealed class PromptNarration {
		private static readonly FieldInfo TitleField = AccessTools.Field(typeof(NavBar), "m_title");

		private readonly Dictionary<NavBar, LabelWatch> _titles = new Dictionary<NavBar, LabelWatch>();
		private NavBar[] _bars = new NavBar[0];
		private float _nextScan;
		private readonly Dictionary<NavBar, string> _hotkeys = new Dictionary<NavBar, string>();
		private readonly Dictionary<string, float> _hotkeySpokenAt = new Dictionary<string, float>();
		private const float HotkeyRepeatSeconds = 300f;

		public bool Pump() {
			if (Time.unscaledTime >= _nextScan) {
				_nextScan = Time.unscaledTime + 1f;
				_bars = Object.FindObjectsOfType<NavBar>();
			}
			bool spoke = false;
			foreach (NavBar bar in _bars) {
				if (bar == null) continue;
				LabelWatch watch;
				if (!_titles.TryGetValue(bar, out watch)) {
					watch = new LabelWatch(ignoreFade: true);
					_titles[bar] = watch;
				}
				var title = (UILabel)TitleField.GetValue(bar);
				string text = bar.isActiveAndEnabled ? watch.Poll(title) : watch.Poll(null);
				bool diceInHand = Focus.ProxyFactory.IsDiceInHand(FocusTracker.Current());
				if (text == null || diceInHand) {
					if (PumpHotkeys(bar, diceInHand)) spoke = true;
					continue;
				}
				var message = new Message().Add(text);
				// An equip decision: the game shows the item already worn ("Equipped") beside
				// the offered one, filled in after focus lands, so it is read with the question.
				UI2D ui = UI2D.Instance;
				Card equipped = ui != null && ui.OldCardInfoPanel != null ? ui.OldCardInfoPanel.Card : null;
				if (equipped != null)
					message.Add(Localization.Strings.Replacing + " " + new Message().AddRange(CardInfoReader.ReadParts(equipped).Summary()).Resolve());
				// The confirm and cancel buttons carry their key as an icon only; on a
				// keyboard the key is named after the button's text (Enter, and Backspace,
				// the mod's second cancel key beside the game's Tab).
				var selectionBar = bar as SelectionNavBar;
				SelectionNavBarControlSet set = selectionBar != null ? selectionBar.ControlSet : null;
				bool keyboard = Defiant.InputManager.ActiveDeviceClass != InControl.InputDeviceClass.Controller;
				foreach (UILabel label in bar.GetComponentsInChildren<UILabel>(false)) {
					if (label == title || !UiText.IsVisible(label)) continue;
					string key = null;
					if (keyboard && set != null) {
						if (set.ConfirmButton != null && label.transform.IsChildOf(set.ConfirmButton.transform)) key = Localization.Strings.KeyEnter;
						else if (set.CancelButton != null && label.transform.IsChildOf(set.CancelButton.transform)) key = Localization.Strings.KeyBackspace;
					}
					string spoken = key == null ? label.text : label.text + " " + key;
					message.Add(spoken);
					// Said here with the question: the action announcement stays quiet.
					_hotkeySpokenAt[spoken] = Time.unscaledTime;
				}
				SpeechPipeline.SpeakEvent(message.Resolve());
				PumpHotkeys(bar);
				spoke = true;
			}
			return spoke;
		}

		// Confirm labels that only restate "confirm" (English and Spanish wordings);
		// announcing them on every screen would be noise. In other languages a generic
		// confirm is announced like a specific one, at most once every few minutes.
		private static readonly string[] GenericConfirms = {
			"Seleccionar", "Select", "Continuar", "Continue", "Aceptar", "Accept", "OK" };

		private static bool IsSpecificAction(string label) {
			foreach (string generic in GenericConfirms)
				if (string.Equals(label.Trim(), generic, System.StringComparison.OrdinalIgnoreCase)) return false;
			return true;
		}

		/// <param name="silent">Track the bar's buttons without announcing them.</param>
		private bool PumpHotkeys(NavBar bar, bool silent = false) {
			var selectionBar = bar as SelectionNavBar;
			if (selectionBar == null) return false;
			var current = new List<string>();
			SelectionNavBarControlSet set = selectionBar.isActiveAndEnabled ? selectionBar.ControlSet : null;
			if (set != null) {
				bool inventoryBar = UI2D.Instance != null && UI2D.Instance.InventoryNavBar == bar;
				foreach (NavBarButton button in new[] { set.F1Button, set.F2Button, set.F3Button }) {
					if (button == null || !button.gameObject.activeInHierarchy) continue;
					if (inventoryBar && button == set.F2Button) continue;
					List<string> labels = UiText.Labels(button.gameObject);
					if (labels.Count > 0) current.Add(string.Join(" ", labels.ToArray()));
				}
				// The confirm action when it names something specific (a shop's "Buy", an
				// examined card's "Equip"); generic confirms are not announced.
				bool keyboard = Defiant.InputManager.ActiveDeviceClass != InControl.InputDeviceClass.Controller;
				if (keyboard && set.ConfirmButton != null && set.ConfirmButton.gameObject.activeInHierarchy) {
					List<string> labels = UiText.Labels(set.ConfirmButton.gameObject);
					if (labels.Count > 0 && IsSpecificAction(labels[0]))
						current.Add(labels[0] + " " + Localization.Strings.KeyEnter);
				}
			}
			string key = string.Join("|", current.ToArray());
			_hotkeys.TryGetValue(bar, out string previous);
			if (key == previous) return false;
			_hotkeys[bar] = key;
			// Only buttons that just appeared, and not one heard in the last few minutes:
			// the standing shortcuts come and go with every screen.
			var fresh = new List<string>();
			float now = Time.unscaledTime;
			string[] before = string.IsNullOrEmpty(previous) ? new string[0] : previous.Split('|');
			foreach (string button in current) {
				if (System.Array.IndexOf(before, button) >= 0) continue;
				if (_hotkeySpokenAt.TryGetValue(button, out float at) && now - at < HotkeyRepeatSeconds) continue;
				_hotkeySpokenAt[button] = now;
				fresh.Add(button);
			}
			if (fresh.Count == 0 || silent || !Settings.ModSettings.ActionKeys.On) return false;
			SpeechPipeline.SpeakEvent(string.Join(", ", fresh.ToArray()));
			return true;
		}
	}
}
